using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Mailroom.Pages.User
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly MailroomDbContext _context;

        public CreateModel(MailroomDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty] public Mailroom.Models.User User { get; set; } = default!;

        private PasswordHasher<Models.User> _hasher = new();

        public async Task<IActionResult> OnPostAsync()
        {
            if (!string.IsNullOrWhiteSpace(User.Email))
            {
                User.Email = User.Email.ToLower();

                if (await _context.Users.AnyAsync(u => u.Email.ToLower() == User.Email))
                {
                    ModelState.AddModelError("User.Email", "The email is already in use.");
                }
            }

            if (string.IsNullOrWhiteSpace(User.Password))
            {
                ModelState.AddModelError("User.Password", "The password is required.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (!string.IsNullOrWhiteSpace(User.Password)) User.Password = _hasher.HashPassword(User, User.Password);

            _context.Users.Add(User);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}