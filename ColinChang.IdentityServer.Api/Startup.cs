using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using ColinChang.IdentityServer.Api.Requirements;
using IdentityModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

namespace ColinChang.IdentityServer.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration) =>
            Configuration = configuration;

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddCors();

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            var identityServerOptions =
                Configuration.GetSection(nameof(IdentityServerOptions)).Get<IdentityServerOptions>();
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = identityServerOptions.Address;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false,
                        RequireExpirationTime = true,
                        ClockSkew = TimeSpan.FromSeconds(25),

                        NameClaimType = JwtClaimTypes.Name,
                        RoleClaimType = JwtClaimTypes.Role
                    };
                });
            services.AddAuthorization(options =>
            {
                foreach (var scope in identityServerOptions.Scopes)
                    options.AddPolicy(scope, policy =>
                    {
                        policy.RequireAuthenticatedUser();
                        policy.RequireClaim("scope", scope);
                    });

                options.AddPolicy("ChineseAdministrator", policy =>
                {
                    // policy.RequireAuthenticatedUser();
                    // policy.RequireClaim("nationality", "China");
                    // policy.RequireRole("Administrator");

                    policy.AddRequirements(new ChineseAdministratorRequirement());
                });
            });
            services.AddSingleton<IAuthorizationRequirement, ChineseAdministratorRequirement>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "ColinChang.IdentityServer.Api",
                    Description = "Identity Server Client Credentials Sample",
                    Contact = new OpenApiContact
                    {
                        Name = "Colin Chang",
                        Email = "zhangcheng@xymind.cn",
                        Url = new Uri("https://ccstudio.com.cn/dotnet/identity_server4/client_credentials.html")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "ColinChang License",
                        Url = new Uri("https://ccstudio.com.cn")
                    }
                });
                c.IncludeXmlComments(
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{GetType().Assembly.GetName().Name}.xml"),
                    true);

                //API授权
                c.AddSecurityDefinition("OAuth2", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "请在下方输入Bearer {token}（注意两者之间是一个空格）",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });
                c.OperationFilter<AddResponseHeadersFilter>();
                c.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
                c.OperationFilter<SecurityRequirementsOperationFilter>();
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ColinChang.IdentityServer.Api v1"));
            }

            app.UseCors(policy =>
            {
                policy.WithOrigins("https://localhost:8000");
                policy.AllowAnyHeader();
                policy.AllowAnyMethod();
                policy.WithExposedHeaders("WWW-Authenticate");
            });

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}