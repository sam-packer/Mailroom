using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Mailroom.Models;
using Mailroom.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Mailroom.Pages.PackageModify
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly MailroomDbContext _context;
        private readonly ILogger<EditModel> _logger;
        private readonly ICurrentUserService _currentUser;

        public EditModel(MailroomDbContext context, ILogger<EditModel> logger, ICurrentUserService currentUser)
        {
            _context = context;
            _logger = logger;
            _currentUser = currentUser;
        }

        [BindProperty] public Packages CurrentPackage { get; set; } = default!;

        [BindProperty]
        [Display(Name = "Carrier")]
        [StringLength(50, MinimumLength = 1)]
        public string Carrier { get; set; } = default!;

        [BindProperty]
        [Display(Name = "Pickup Date")]
        public DateTime PickupDate { get; set; } = default!;


        [BindProperty]
        [Display(Name = "Picked up by user?")]
        public bool Delivered { get; set; } = default!;
        
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var package = await _context.Packages.Include(u => u.User).FirstOrDefaultAsync(m => m.PackageId == id);
            if (package == null)
            {
                return NotFound();
            }

            CurrentPackage = package;
            Carrier = CurrentPackage.Carrier;
            PickupDate = TimeZoneUtil.FromUtc(DateTime.UtcNow, _currentUser.Timezone);
            Delivered = package.Delivered;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var package = _context.Attach(CurrentPackage);

            package.Entity.Carrier = Carrier;
            package.Entity.PickedUpDate = TimeZoneUtil.ToUtc(PickupDate, _currentUser.Timezone);
            package.Entity.Delivered = Delivered;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PackagesExists(CurrentPackage.PackageId))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToPage("../Index");
        }

        private bool PackagesExists(int id)
        {
            return _context.Packages.Any(e => e.PackageId == id);
        }
    }
}