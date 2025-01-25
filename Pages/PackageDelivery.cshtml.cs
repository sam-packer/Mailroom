using System.ComponentModel.DataAnnotations;
using Mailroom.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Mailroom.Pages;

[Authorize(Roles = "Admin")]
public class PackageDelivery : PageModel
{
    private readonly ILogger<PackageDelivery> _logger;

    private readonly MailroomDbContext _context;

    private readonly IConfiguration _configuration;

    public List<Models.User> Users { get; set; } = default!;

    public SelectList UserDropdown { get; set; } = default!;

    [BindProperty]
    [Display(Name = "Name")]
    public int FullName { get; set; }

    [BindProperty]
    [Display(Name = "Carrier")]
    [StringLength(50, MinimumLength = 1)]
    public string Carrier { get; set; }

    public PackageDelivery(MailroomDbContext context, ILogger<PackageDelivery> logger, IConfiguration configuration)
    {
        _context = context;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        Users = await _context.Users.ToListAsync();
        var fullNameList =
            _context.Users.Select(u => new
            {
                UserId = u.UserId,
                UserInfo = $"{u.First_Name} {u.Last_Name} -- {u.Building} Unit {u.Unit}"
            }).ToList();
        UserDropdown = new SelectList(fullNameList, "UserId", "UserInfo");
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        Models.User user = await _context.Users.FindAsync(FullName);
        if (user == null)
        {
            return NotFound();
        }

        Packages package = new Packages();
        package.UserId = user.UserId;
        package.Carrier = Carrier;
        package.DeliveredDate = DateTime.Now;

        await _context.Packages.AddAsync(package);
        await _context.SaveChangesAsync();

        EmailUtil emailUtil = new EmailUtil(_configuration);
        await emailUtil.SendEmail(user.Email);

        return RedirectToPage("/Index");
    }
}