using Mailroom;
using Mailroom.Utils;
using Microsoft.AspNetCore.Mvc;

public class NavbarViewComponent : ViewComponent
{
    private readonly ICurrentUserService _currentUser;

    public NavbarViewComponent(ICurrentUserService currentUser)
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