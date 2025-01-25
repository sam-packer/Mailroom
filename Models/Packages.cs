using System.ComponentModel.DataAnnotations;

namespace Mailroom.Models;

public class Packages
{
    public int PackageId { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public string Carrier { get; set; }

    [Display(Name = "Delivery Date")] public DateTime DeliveredDate { get; set; }

    [Display(Name = "Pickup Date")] public DateTime PickedUpDate { get; set; }

    [Display(Name = "Picked up by user?")] public bool Delivered { get; set; }
}