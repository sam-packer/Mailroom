﻿using System.ComponentModel.DataAnnotations;
using Mailroom.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Mailroom.Pages;

[AllowAnonymous]
public class Initialize : PageModel
{
    private readonly MailroomDbContext _context;
    private readonly ILogger<Initialize> _logger;
    private readonly IConfiguration _configuration;

    PasswordHasher<Models.User> hasher = new();

    [Required]
    [Display(Name = "First Name")]
    [BindProperty]
    public string FirstName { get; set; }

    [Required]
    [Display(Name = "Last Name")]
    [BindProperty]
    public string LastName { get; set; }

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

    private string Token { get; set; }

    public Initialize(MailroomDbContext context, ILogger<Initialize> logger, IConfiguration configuration)
    {
        _context = context;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        if (await _context.Users.AnyAsync())
        {
            return Unauthorized();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (await _context.Users.AnyAsync())
        {
            return Unauthorized();
        }

        Models.User user = new Models.User
        {
            First_Name = FirstName,
            Last_Name = LastName,
            Email = Email,
            Role = "Admin"
        };
        user.Password = hasher.HashPassword(user, Password);

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

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