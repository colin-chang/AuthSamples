using System.Diagnostics;
using ColinChang.MvcSample.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ColinChang.MvcSample.Controllers
{
    public class HomeController : Controller
    {
        [Authorize("admin")]
        public IActionResult Index() => View();

        [Authorize(Roles = "Administrator,User")]
        public IActionResult Privacy() => View();

        [Authorize]
        public IActionResult Test() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel
            {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
    }
}