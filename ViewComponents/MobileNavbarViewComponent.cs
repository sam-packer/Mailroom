using Mailroom.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Mailroom.ViewComponents;

public class MobileNavbarViewComponent : ViewComponent
{
    private readonly ICurrentUserService _currentUser;

    public MobileNavbarViewComponent(ICurrentUserService currentUser)
    {
        _currentUser = currentUser;
    }

    public IViewComponentResult Invoke()
    {
        if (_currentUser.UserId == null || _currentUser.User == null)
        {
            return View<string>("Default", null); // Not logged in
        }

        var role = _currentUser.Role;
        return View("Default", role);
    }
}