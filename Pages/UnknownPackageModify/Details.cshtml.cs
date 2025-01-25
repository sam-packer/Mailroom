using Mailroom.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Mailroom.Pages.UnknownPackageModify
{
    [Authorize(Roles = "Admin")]
    public class DetailsModel : PageModel
    {
        private readonly MailroomDbContext _context;

        public DetailsModel(MailroomDbContext context)
        {
            _context = context;
        }

        public UnknownPackage UnknownPackages { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var packages = await _context.UnknownPackages.FirstOrDefaultAsync(m => m.UnknownPackageId == id);
            if (packages == null)
            {
                return NotFound();
            }
            else
            {
                UnknownPackages = packages;
            }

            return Page();
        }
    }
}