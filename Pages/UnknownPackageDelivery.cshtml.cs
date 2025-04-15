using System.ComponentModel.DataAnnotations;
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

    [BindProperty]
    [Display(Name = "Carrier")]
    [StringLength(25, MinimumLength = 1)]
    public string Carrier { get; set; }

    [BindProperty]
    [Display(Name = "Full Name")]
    [StringLength(255, MinimumLength = 1)]
    public string FullName { get; set; }

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
        UnknownPackage unknownPackage = new UnknownPackage
        {
            FullName = FullName,
            Carrier = Carrier,
            DeliveredDate = DateTime.Now
        };

        await _context.UnknownPackages.AddAsync(unknownPackage);
        await _context.SaveChangesAsync();

        return RedirectToPage("/Index");
    }
}