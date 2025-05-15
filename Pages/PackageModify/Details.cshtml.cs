using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Mailroom.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Mailroom.Pages.PackageModify
{
    [Authorize(Roles = "Admin")]
    public class DetailsModel : PageModel
    {
        private readonly MailroomDbContext _context;
        private readonly ILogger<DetailsModel> _logger;

        public DetailsModel(MailroomDbContext context, ILogger<DetailsModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public Packages Packages { get; set; } = default!;
        
        [Display(Name = "Resident")] public string FullName { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var packages = await _context.Packages.Include(p => p.User).FirstOrDefaultAsync(m => m.PackageId == id);
            if (packages == null)
            {
                return NotFound();
            }

            Packages = packages;
            FullName = packages.User.First_Name + " " + packages.User.Last_Name;

            return Page();
        }
    }
}