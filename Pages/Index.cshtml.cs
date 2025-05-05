using System.Security.Claims;
using Mailroom.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Mailroom.Pages;

[Authorize]
public class IndexModel : PageModel
{
    private readonly MailroomDbContext _context;
    private readonly ILogger<IndexModel> _logger;

    public IList<Packages> Packages { get; set; } = default!;
    public IList<UnknownPackage> UnknownPackages { get; set; } = default!;

    public Models.User CurrentUser { get; private set; } = default!;
    public string FullName { get; private set; } = "";
    public string UserRole { get; private set; } = "";

    public IndexModel(MailroomDbContext context, ILogger<IndexModel> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out var userId))
        {
            _logger.LogWarning("Invalid or missing user ID claim.");
            return Unauthorized();
        }

        CurrentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        if (CurrentUser == null)
        {
            _logger.LogWarning("User not found in DB for ID {UserId}", userId);
            return NotFound();
        }

        UserRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "";
        FullName = $"{CurrentUser.First_Name} {CurrentUser.Last_Name}";

        if (UserRole == "Admin")
        {
            Packages = await _context.Packages
                .Include(p => p.User)
                .Where(p => !p.Delivered)
                .OrderBy(p => p.DeliveredDate)
                .ToListAsync();

            UnknownPackages = await _context.UnknownPackages.ToListAsync();
        }
        else
        {
            Packages = await _context.Packages
                .Include(p => p.User)
                .Where(p => !p.Delivered && p.UserId == CurrentUser.UserId)
                .ToListAsync();
        }

        return Page();
    }
}