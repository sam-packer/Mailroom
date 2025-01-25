using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Mailroom.Pages.User
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly MailroomDbContext _context;

        public EditModel(MailroomDbContext context)
        {
            _context = context;
        }

        [BindProperty] public Models.User EditUser { get; set; } = default!;

        private PasswordHasher<Models.User> _hasher = new();

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
                var adminCount = await _context.Users.CountAsync(u => u.Role == "Admin");
                if (adminCount <= 1)
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