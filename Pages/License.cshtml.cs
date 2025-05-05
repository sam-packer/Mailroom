using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Mailroom.Pages;

public class License : PageModel
{
    public string LicenseText { get; private set; }

    public void OnGet()
    {
        var licenseFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "licenses", "gpl-3.0.txt");
        if (System.IO.File.Exists(licenseFilePath))
        {
            LicenseText = System.IO.File.ReadAllText(licenseFilePath);
        }
        else
        {
            LicenseText = "GPLv3 license file not found.";
        }
    }
}