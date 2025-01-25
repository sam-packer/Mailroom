using Mailroom.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Mailroom.Pages.UnknownPackageModify
{
    [Authorize(Roles = "Admin")]
    public class DeleteModel : PageModel
    {
        private readonly MailroomDbContext _context;

        public DeleteModel(MailroomDbContext context)
        {
            _context = context;
        }

        [BindProperty] public UnknownPackage UnknownPackage { get; set; } = default!;

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
                UnknownPackage = packages;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var packages = await _context.UnknownPackages.FindAsync(id);
            if (packages != null)
            {
                UnknownPackage = packages;
                _context.UnknownPackages.Remove(UnknownPackage);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("../Index");
        }
    }
}