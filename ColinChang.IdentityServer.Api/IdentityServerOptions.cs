using System.Collections.Generic;

namespace ColinChang.IdentityServer.Api
{
    public class IdentityServerOptions
    {
        public string Address { get; set; }
        public IEnumerable<string> Scopes { get; set; }
    }
}