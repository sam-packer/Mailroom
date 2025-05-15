using Mailroom.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Mailroom.Pages.ManageUsers
{
    [Authorize(Roles = "Admin")]
    public class DeleteModel : PageModel
    {
        private readonly MailroomDbContext _context;
        private readonly IMemoryCache _cache;

        public DeleteModel(MailroomDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        [BindProperty] public User User { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FirstOrDefaultAsync(m => m.UserId == id);

            if (user == null)
            {
                return NotFound();
            }

            User = user;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            User = user;

            var adminCount = await _context.Users.CountAsync(u => u.Role == "Admin");

            if (adminCount <= 1)
            {
                TempData["ErrorMessage"] = "You are the last admin in the database and cannot delete yourself!";
                return Page();
            }

            _cache.Remove($"user_{user.UserId}");
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}