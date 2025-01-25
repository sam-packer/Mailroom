using Mailroom.Jwt;
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

    public Models.User CurrentUser { get; private set; }

    public string FullName { get; private set; }

    public string UserRole { get; private set; }

    private string Token { get; set; }


    public IndexModel(MailroomDbContext context, ILogger<IndexModel> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        Token = JwtHelper.FetchToken(HttpContext);

        var userClaim = JwtHelper.FetchUserClaim(Token);
        CurrentUser = _context.Users.FirstOrDefault(u => u.UserId.ToString() == userClaim);

        UserRole = JwtHelper.GetRoleFromJwt(Token);
        FullName = JwtHelper.GetNameFromJwt(Token); // this one is just a test

        if (CurrentUser == null)
        {
            return NotFound();
        }

        if (UserRole == "Admin")
        {
            Packages = await _context.Packages.Include(p => p.User).Where(p => p.Delivered == false)
                .OrderBy(p => p.DeliveredDate).ToListAsync();
            UnknownPackages = await _context.UnknownPackages.ToListAsync();
        }
        else
        {
            Packages = await _context.Packages.Include(p => p.User).Where(p => p.Delivered == false)
                .Where(p => p.UserId == CurrentUser.UserId).ToListAsync();
        }

        return Page();
    }
}