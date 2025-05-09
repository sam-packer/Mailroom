using Mailroom.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Mailroom.Pages.PackageModify
{
    [Authorize(Roles = "Admin")]
    public class DeleteModel : PageModel
    {
        private readonly MailroomDbContext _context;

        public DeleteModel(MailroomDbContext context)
        {
            _context = context;
        }

        [BindProperty] public Packages Packages { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var packages = await _context.Packages.FirstOrDefaultAsync(m => m.PackageId == id);

            if (packages == null)
            {
                return NotFound();
            }
            else
            {
                Packages = packages;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var packages = await _context.Packages.FindAsync(id);
            if (packages != null)
            {
                Packages = packages;
                _context.Packages.Remove(Packages);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("../Index");
        }
    }
}