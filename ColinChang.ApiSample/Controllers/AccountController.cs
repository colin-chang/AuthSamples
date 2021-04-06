using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using ColinChang.ApiSample.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ColinChang.ApiSample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private static readonly IEnumerable<User> Users = new[]
        {
            new User("Colin", "123123", new[] {new Role("Administrator")}),
            new User("Robin", "123123", new[] {new Role("User")})
        };

        [HttpPost]
        public IActionResult Post([FromServices] IOptions<JwtOptions> options, [FromBody]User user)
        {
            if (user == null)
                return BadRequest("user cannot be null");

            var usr = Users.SingleOrDefault(u =>
                string.Equals(u.Username, user.Username, StringComparison.OrdinalIgnoreCase));
            if (!string.Equals(usr?.Password, user.Password, StringComparison.OrdinalIgnoreCase))
                return BadRequest("invalid username or password");

            var claims = new List<Claim> {new Claim(ClaimTypes.Name, user.Username)};
            claims.AddRange(usr.Roles.Select(role => new Claim(ClaimTypes.Role, role.Name)));

            var jwtOptions = options.Value;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.IssuerSigningKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(jwtOptions.ValidIssuer, jwtOptions.ValidAudience, claims, DateTime.Now,
                DateTime.Now.AddMinutes(jwtOptions.Expires), credentials);
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(jwt);
        }
    }
}