using Mailroom.Models;
using Mailroom.Utils;
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
    private readonly ICurrentUserService _currentUser;

    private const int PageSize = 10;

    public PaginatedList<Packages> Packages { get; set; }
    public PaginatedList<UnknownPackage> UnknownPackages { get; set; }

    public IndexModel(MailroomDbContext context, ILogger<IndexModel> logger, ICurrentUserService currentUser)
    {
        _context = context;
        _logger = logger;
        _currentUser = currentUser;
    }

    public async Task<IActionResult> OnGetAsync(
        int packagesPage = 1,
        int unknownPage = 1)
    {
        if (_currentUser.Role == "Admin")
        {
            var packagesQuery = _context.Packages
                .Include(p => p.User)
                .Where(p => !p.Delivered)
                .OrderBy(p => p.DeliveredDate);

            Packages = await PaginatedList<Packages>
                .CreateAsync(packagesQuery, packagesPage, PageSize);

            var unknownQuery = _context.UnknownPackages
                .OrderBy(u => u.DeliveredDate);

            UnknownPackages = await PaginatedList<UnknownPackage>
                .CreateAsync(unknownQuery, unknownPage, PageSize);
        }
        else
        {
            var userPackages = _context.Packages
                .Include(p => p.User)
                .Where(p => !p.Delivered && p.UserId == _currentUser.UserId)
                .OrderBy(p => p.DeliveredDate);

            Packages = await PaginatedList<Packages>
                .CreateAsync(userPackages, packagesPage, PageSize);
        }

        return Page();
    }
}