using System.ComponentModel.DataAnnotations;
using Mailroom.Models;
using Mailroom.Pages.Mail;
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

    private readonly IBackgroundTaskQueue _backgroundQueue;

    public List<User> Users { get; set; } = default!;

    public SelectList UserDropdown { get; set; } = default!;

    [Display(Name = "Name")]
    [BindProperty]
    [Required]
    public int FullName { get; set; }

    [BindProperty] public Packages Package { get; set; }

    public PackageDelivery(MailroomDbContext context, ILogger<PackageDelivery> logger, IConfiguration configuration,
        IBackgroundTaskQueue backgroundQueue)
    {
        _context = context;
        _logger = logger;
        _configuration = configuration;
        _backgroundQueue = backgroundQueue;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        Users = await _context.Users.ToListAsync();
        PopulateUserDropdown();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        User user = await _context.Users.FindAsync(FullName);
        if (user == null)
        {
            ModelState.AddModelError("FullName", "The Name field is required.");
        }

        if (user != null)
        {
            Package.UserId = user.UserId;
            Package.DeliveredDate = DateTime.UtcNow;
        }

        if (!ModelState.IsValid)
        {
            Users = await _context.Users.ToListAsync();
            PopulateUserDropdown();
            return Page();
        }

        await _context.Packages.AddAsync(Package);
        await _context.SaveChangesAsync();

        _backgroundQueue.Enqueue(async token =>
        {
            try
            {
                var emailUtil = new EmailUtil(_configuration);
                await emailUtil.SendEmail(user.Email);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Email sending failed in background task.");
            }
        });

        return RedirectToPage("/Index");
    }

    private void PopulateUserDropdown()
    {
        if (Users.Count == 0)
        {
            throw new ApplicationException("No users found");
        }

        var fullNameList = Users
            .Select(u => new
            {
                UserId = u.UserId,
                UserInfo = $"{u.First_Name} {u.Last_Name} -- {u.Building} Unit {u.Unit}"
            }).ToList();

        UserDropdown = new SelectList(fullNameList, "UserId", "UserInfo");
    }
}