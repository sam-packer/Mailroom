using System.Security.Claims;
using Mailroom.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Mailroom.Pages.ManageUsers
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly MailroomDbContext _context;
        private readonly IMemoryCache _cache;

        public EditModel(MailroomDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        [BindProperty] public User EditUser { get; set; } = default!;

        private PasswordHasher<User> _hasher = new();

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

            EditUser = user;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!string.IsNullOrWhiteSpace(EditUser.Email))
            {
                EditUser.Email = EditUser.Email.ToLower();

                if (await _context.Users.AnyAsync(u =>
                        u.Email.ToLower() == EditUser.Email && u.UserId != EditUser.UserId))
                {
                    ModelState.AddModelError("EditUser.Email", "The email is already in use.");
                }
            }

            if (EditUser.Role == "User")
            {
                var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                bool isSelf = EditUser.UserId == currentUserId;
                var adminCount = await _context.Users.CountAsync(u => u.Role == "Admin");
                
                if (isSelf && adminCount <= 1)
                {
                    ModelState.AddModelError("EditUser.Role",
                        "You are the only admin in the database and cannot set your role to User.");
                }
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(EditUser).State = EntityState.Modified;

            if (!string.IsNullOrWhiteSpace(EditUser.Password))
            {
                EditUser.Password = _hasher.HashPassword(EditUser, EditUser.Password);
                _context.Entry(EditUser).Property(u => u.Password).IsModified = true;
            }
            else
            {
                _context.Entry(EditUser).Property(u => u.Password).IsModified = false;
            }

            try
            {
                await _context.SaveChangesAsync();
                _cache.Remove($"user_{EditUser.UserId}");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(EditUser.UserId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}