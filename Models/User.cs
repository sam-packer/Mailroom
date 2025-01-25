using System.ComponentModel.DataAnnotations;

namespace Mailroom.Models;

public class User
{
    public int UserId { get; set; }

    [Display(Name = "First Name")]
    [Required]
    public string? First_Name { get; set; }

    [Display(Name = "Last Name")]
    [Required]
    public string? Last_Name { get; set; }

    [Required] [EmailAddress] public string? Email { get; set; }

    [StringLength(32, MinimumLength = 8)] public string? Password { get; set; }

    public string? Building { get; set; }
    public int? Unit { get; set; } = 0;
    public string? Role { get; set; }
}