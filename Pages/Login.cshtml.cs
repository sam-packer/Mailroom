using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Mailroom.Pages;

[AllowAnonymous]
public class LoginModel : PageModel
{
    private readonly MailroomDbContext _context;
    private readonly ILogger<LoginModel> _logger;
    private readonly IConfiguration _configuration;
    PasswordHasher<Models.User> hasher = new();

    [Required]
    [Display(Name = "Email")]
    [EmailAddress]
    [BindProperty]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    [StringLength(32, MinimumLength = 8)]
    [BindProperty]
    public string Password { get; set; }
    
    [BindProperty(SupportsGet = false)]
    public string Timezone { get; set; } = "UTC";

    public string ErrorMessage { get; set; } = default!;

    public LoginModel(MailroomDbContext context, ILogger<LoginModel> logger, IConfiguration configuration)
    {
        _context = context;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        if (!await _context.Users.AnyAsync())
        {
            return RedirectToPage("/Initialize");
        }

        if (User.Identity is { IsAuthenticated: true })
        {
            return RedirectToPage("/Index");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        Email = Email.Trim();

        var user = await _context.Users.FirstOrDefaultAsync(u =>
            u.Email == Email);

        if (user == null)
        {
            ErrorMessage = "Invalid email or password";
            return Page();
        }

        var result = hasher.VerifyHashedPassword(user, user.Password, Password);

        if (result == PasswordVerificationResult.Failed)
        {
            ErrorMessage = "Invalid email or password";
            return Page();
        }
        
        if (!string.IsNullOrEmpty(Timezone) && user.Timezone != Timezone)
        {
            user.Timezone = Timezone;
            await _context.SaveChangesAsync();
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.First_Name + " " + user.Last_Name),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
        };

        var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        await HttpContext.SignInAsync("Cookies", claimsPrincipal);

        return RedirectToPage("/Index");
    }
}