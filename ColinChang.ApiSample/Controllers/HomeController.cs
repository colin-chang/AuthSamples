using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ColinChang.ApiSample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        [Authorize]
        public string Get() =>
            $"{User.Identity.Name} is authenticated";

        [Authorize("admin")]
        [HttpPost]
        public string Post() => $"{User.Identity.Name} is authorized with policy admin";

        [Authorize(Roles = "Administrator")]
        [HttpPut]
        public string Put() =>
            $"{User.Identity.Name} is authorized with role Administrator\nroles:{string.Join(",", User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value))}";
    }
}