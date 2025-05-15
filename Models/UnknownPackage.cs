using System.ComponentModel.DataAnnotations;

namespace Mailroom.Models;

public class UnknownPackage
{
    public int UnknownPackageId { get; set; }

    [Display(Name = "Full Name")]
    [StringLength(255, MinimumLength = 1)]
    [Required]
    public string FullName { get; set; } = string.Empty;

    [StringLength(50, MinimumLength = 1)]
    [Required]
    public string Carrier { get; set; } = string.Empty;

    [Display(Name = "Delivery Date")]
    [Required]
    public DateTime DeliveredDate { get; set; }
}