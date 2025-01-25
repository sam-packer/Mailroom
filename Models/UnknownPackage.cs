using System.ComponentModel.DataAnnotations;

namespace Mailroom.Models;

public class UnknownPackage
{
    public int UnknownPackageId { get; set; }

    [Display(Name = "Full Name")] public string FullName { get; set; }

    public string Carrier { get; set; }

    [Display(Name = "Delivery Date")] public DateTime DeliveredDate { get; set; }
}