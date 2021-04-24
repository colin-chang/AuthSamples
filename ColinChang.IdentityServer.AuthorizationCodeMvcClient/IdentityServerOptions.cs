using System.Linq;

namespace ColinChang.IdentityServer.AuthorizationCodeMvcClient
{
    public class IdentityServerOptions
    {
        public string Address { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public ScopeItem[] Scopes { get; set; }

        public class ScopeItem
        {
            public string Name { get; set; }
            public string Url { get; set; }
        }

        public string Scope =>
            string.Join(" ", Scopes.Select(s => s.Name));

        public string this[string index]
            => Scopes.FirstOrDefault(s => string.Equals(index, s.Name))?.Url;
    }
}