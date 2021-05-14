using Microsoft.AspNetCore.Builder;

namespace ColinChang.IdentityServer.ImplicitJavaScriptClient
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app) =>
            app.UseDefaultFiles().UseStaticFiles();
    }
}