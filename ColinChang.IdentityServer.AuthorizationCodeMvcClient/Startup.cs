using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ColinChang.IdentityServer.AuthorizationCodeMvcClient
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            //关闭JWT Claim类型映射，以便返回WellKnown Claims
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            // JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

            var is4Configuration = Configuration.GetSection(nameof(IdentityServerOptions));
            services.Configure<IdentityServerOptions>(is4Configuration);
            var is4Options = is4Configuration.Get<IdentityServerOptions>();
            services
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = is4Options.Address;
                    options.ClientId = is4Options.ClientId;
                    options.ClientSecret = is4Options.ClientSecret;
                    options.ResponseType = OidcConstants.ResponseTypes.Code;

                    options.SaveTokens = true; //保存token到cookie
                    options.RequireHttpsMetadata = false; //关闭https验证

                    options.Scope.Clear();
                    // options.Scope.Add(OidcConstants.StandardScopes.OpenId);
                    foreach (var scope in is4Options.Scopes)
                        options.Scope.Add(scope.Name);

                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}