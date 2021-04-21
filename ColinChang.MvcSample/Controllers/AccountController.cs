using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using ColinChang.MvcSample.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace ColinChang.MvcSample.Controllers
{
    public class AccountController : Controller
    {
        private static readonly IEnumerable<User> Users = new[]
        {
            new User("Colin", "123123", new[] {new Role("Administrator"), new Role("User")}),
            new User("Robin", "123123", new[] {new Role("User")})
        };

        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(User user)
        {
            if (user == null)
                return Challenge();

            var usr = Users.SingleOrDefault(u =>
                string.Equals(u.Username, user.Username, StringComparison.OrdinalIgnoreCase));
            if (!string.Equals(usr?.Password, user.Password, StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.Message = "invalid username or password";
                return View(user);
            }

            var identity = new GenericIdentity(user.Username, CookieAuthenticationDefaults.AuthenticationScheme);
            foreach (var role in usr.Roles)
                identity.AddClaim(new Claim(ClaimTypes.Role, role.Name));
            var principal = new GenericPrincipal(identity, null);
            return SignIn(principal);
        }

        public IActionResult Logout() =>
            SignOut(new AuthenticationProperties {RedirectUri = "/"});

        public IActionResult AccessDenied() => View();
    }
}