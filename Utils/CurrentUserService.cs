using System.Security.Claims;
using Mailroom.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Mailroom.Utils;

public interface ICurrentUserService
{
    int? UserId { get; }
    User? User { get; }
    string Timezone { get; }
    string FullName { get; }
    string? Role { get; }
}

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly MailroomDbContext _context;
    private readonly IMemoryCache _cache;
    private readonly ILogger<CurrentUserService> _logger;

    private readonly User? _user;

    public int? UserId { get; }
    public User? User => _user;
    public string Timezone => _user?.Timezone ?? "UTC";
    public string FullName => $"{_user?.First_Name} {_user?.Last_Name}".Trim();
    public string? Role => _user?.Role;

    public CurrentUserService(
        IHttpContextAccessor accessor,
        MailroomDbContext context,
        IMemoryCache cache,
        ILogger<CurrentUserService> logger)
    {
        _httpContextAccessor = accessor;
        _context = context;
        _cache = cache;
        _logger = logger;

        var user = accessor.HttpContext?.User;
        var idClaim = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(idClaim, out var id)) return;

        UserId = id;
        var cacheKey = $"user_{id}";

        if (_cache.TryGetValue<User>(cacheKey, out var cachedUser))
        {
            _logger.LogInformation("Cache hit for user {UserId}", id);
            _user = cachedUser;
            return;
        }

        _logger.LogInformation("Cache miss for user {UserId}", id);
        _user = _context.Users.AsNoTracking().FirstOrDefault(u => u.UserId == id);

        if (_user != null)
        {
            _cache.Set(cacheKey, _user, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(3),
                SlidingExpiration = TimeSpan.FromMinutes(30)
            });
        }
    }
}