using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Mailroom.Models;
using Microsoft.AspNetCore.Authorization;

namespace Mailroom.Pages.User
{
    [Authorize(Roles = "Admin")]
    public class DetailsModel : PageModel
    {
        private readonly MailroomDbContext _context;

        public DetailsModel(MailroomDbContext context)
        {
            _context = context;
        }

        public Models.User User { get; set; } = default!;

        public IList<Packages> Packages { get; set; } = default!;

        [BindProperty(SupportsGet = true)] public int PageNum { get; set; } = 1;

        public const int PageSize = 10;

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
            else
            {
                User = user;
                Packages = await _context.Packages.Include(p => p.User)
                    .Where(p => p.UserId == user.UserId)
                    .OrderByDescending(p => p.DeliveredDate)
                    .Skip((PageNum - 1) * PageSize).Take(PageSize).ToListAsync();
            }

            return Page();
        }

        public int DetermineRecords()
        {
            var packages = _context.Packages.Include(p => p.User).Where(p => p.UserId == User.UserId)
                .ToList();
            var i = PageNum;
            while (packages.Skip((i - 1) * PageSize).Take(PageSize).ToList().Count != 0)
            {
                i++;
            }

            return i;
        }
    }
}