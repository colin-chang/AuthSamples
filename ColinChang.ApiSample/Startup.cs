using System;
using System.IO;
using System.Text;
using ColinChang.ApiSample.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

namespace ColinChang.ApiSample
{
    public class Startup
    {
        public Startup(IConfiguration configuration) =>
            Configuration = configuration;

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.Configure<JwtOptions>(Configuration.GetSection(nameof(JwtOptions)));
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    var jwt = Configuration.GetSection(nameof(JwtOptions)).Get<JwtOptions>();
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = jwt.ValidIssuer,
                        ValidAudience = jwt.ValidAudience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.IssuerSigningKey)),
                        ValidateIssuerSigningKey = true
                    };
                });
            services.AddAuthorization(options => options.AddPolicy("admin", policy =>
            {
                // policy.AddRequirements(new DenyAnonymousAuthorizationRequirement());
                // policy.AddRequirements(new RolesAuthorizationRequirement(new[] {"Administrator"}));

                policy.RequireAuthenticatedUser();
                policy.RequireRole("Administrator");
            }));
            services.AddSwaggerGen(c =>
            {
                //????????????
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "ColinChang.ApiSample",
                    Description = "??????`JWT Bear`???????????? ??????",
                    Contact = new OpenApiContact
                    {
                        Name = "Colin Chang",
                        Email = "zhangcheng@xymind.cn",
                        Url = new Uri("https://ccstudio.com.cn/dotnet/auth/jwt.html#_3-2-jwt-????????????")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "ColinChang License",
                        Url = new Uri("https://ccstudio.com.cn")
                    }
                });
                //????????????
                c.IncludeXmlComments(
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{GetType().Assembly.GetName().Name}.xml"),
                    true);

                //API??????
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "??????????????????Bearer {token}???????????????????????????????????????",
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
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ColinChang.ApiSample v1"));
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}