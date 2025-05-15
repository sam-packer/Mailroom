using System.ComponentModel.DataAnnotations;

namespace Mailroom.Models;

public class Packages
{
    public int PackageId { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }

    [StringLength(50, MinimumLength = 1)] public string Carrier { get; set; } = string.Empty;

    [Display(Name = "Delivery Date")]
    [Required]
    public DateTime DeliveredDate { get; set; }

    [Display(Name = "Pickup Date")] public DateTime PickedUpDate { get; set; }

    [Display(Name = "Picked up by user?")] public bool Delivered { get; set; }
}