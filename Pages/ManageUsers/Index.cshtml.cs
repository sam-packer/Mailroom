using Mailroom.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Mailroom.Pages.ManageUsers
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly MailroomDbContext _context;

        [BindProperty(SupportsGet = true)] public string? Search { get; set; }

        public IndexModel(MailroomDbContext context)
        {
            _context = context;
        }

        public IList<User> User { get; set; } = default!;

        public async Task OnGetAsync()
        {
            User = await _context.Users.ToListAsync();
            if (!string.IsNullOrEmpty(Search))
            {
                var SearchResults = User.Where(s =>
                    // first name only
                    s is { First_Name: not null } &&
                    s.First_Name.Contains(Search, StringComparison.CurrentCultureIgnoreCase)
                    // last name only
                    || s is { Last_Name: not null } &&
                    s.Last_Name.Contains(Search, StringComparison.CurrentCultureIgnoreCase)
                    // first and last name
                    || s is { First_Name: not null, Last_Name: not null } &&
                    (s.First_Name + " " + s.Last_Name).Contains(Search, StringComparison.CurrentCultureIgnoreCase)
                    // building
                    || s is { Building: not null } &&
                    s.Building.Equals(Search, StringComparison.CurrentCultureIgnoreCase)
                    // unit
                    || ("unit " + s.Unit).Equals(Search, StringComparison.CurrentCultureIgnoreCase)
                    // building and unit
                    || (s is { Building: not null } &&
                        (s.Building + " unit " + s.Unit).Contains(Search, StringComparison.CurrentCultureIgnoreCase))
                );
                User = SearchResults.ToList();
            }
        }
    }
}