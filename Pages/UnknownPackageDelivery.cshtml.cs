using Mailroom.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Mailroom.Pages;

[Authorize(Roles = "Admin")]
public class UnknownPackageDelivery : PageModel
{
    private readonly ILogger<UnknownPackageDelivery> _logger;

    private readonly MailroomDbContext _context;

    [BindProperty] public UnknownPackage UnknownPackage { get; set; }

    public UnknownPackageDelivery(MailroomDbContext context, ILogger<UnknownPackageDelivery> logger)
    {
        _context = context;
        _logger = logger;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        UnknownPackage.DeliveredDate = DateTime.UtcNow;

        if (!ModelState.IsValid)
        {
            return Page();
        }

        await _context.UnknownPackages.AddAsync(UnknownPackage);
        await _context.SaveChangesAsync();

        return RedirectToPage("/Index");
    }
}