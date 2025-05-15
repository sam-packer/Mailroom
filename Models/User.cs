using System.ComponentModel.DataAnnotations;

namespace Mailroom.Models;

public class User
{
    public int UserId { get; set; }

    [Display(Name = "First Name")]
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string? First_Name { get; set; }

    [StringLength(100, MinimumLength = 1)]
    [Display(Name = "Last Name")]
    [Required]
    public string? Last_Name { get; set; }

    [Required]
    [EmailAddress]
    [StringLength(255, MinimumLength = 1)]
    public string? Email { get; set; }

    [StringLength(32, MinimumLength = 8)] public string? Password { get; set; }

    [StringLength(100, MinimumLength = 1)] public string? Building { get; set; }

    public int? Unit { get; set; } = 0;

    [StringLength(5, MinimumLength = 1)] public string? Role { get; set; }
    
    [StringLength(255)]
    public string Timezone { get; set; } = string.Empty;
    
    public ICollection<Packages>? Packages { get; set; }
}