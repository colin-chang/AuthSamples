using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ColinChang.IdentityServer.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthorizationController : ControllerBase
    {
        [Authorize(Roles = "User")]
        [HttpGet]
        public string Get() => "you successfully called API with role User";

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public string Post() => "you successfully called API with role Administrator";

        [Authorize("ChineseAdministrator")]
        [HttpPut]
        public string Put() => "you successfully called API with ChinaAdministrator Policy";
    }
}