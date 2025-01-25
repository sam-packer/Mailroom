using System.ComponentModel.DataAnnotations;
using Mailroom.Jwt;
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

    public string ErrorMessage { get; set; } = default!;

    private string Token { get; set; }

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

        Token = JwtHelper.GenerateToken(user, _configuration);

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.Now.AddHours(3),
        };

        Response.Cookies.Append("AuthToken", Token, cookieOptions);

        return RedirectToPage("/Index");
    }
}