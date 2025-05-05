using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using System.Security.Claims;

namespace Mailroom.ViewComponents;

public class NavbarViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        var user = HttpContext.User;
        if (user?.Identity?.IsAuthenticated != true)
        {
            return View<string>("Default", null); // Show no nav or limited nav
        }

        var role = user.FindFirst(ClaimTypes.Role)?.Value;
        return View("Default", role);
    }
}