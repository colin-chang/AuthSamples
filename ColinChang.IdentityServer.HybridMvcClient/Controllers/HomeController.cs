using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ColinChang.IdentityServer.HybridMvcClient.Models;
using Microsoft.AspNetCore.Mvc;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace ColinChang.IdentityServer.HybridMvcClient.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IdentityServerOptions _options;

        public HomeController(IOptions<IdentityServerOptions> options) =>
            _options = options.Value;

        public async Task<IActionResult> Index()
        {
            using var client = new HttpClient();
            var accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            client.SetBearerToken(accessToken);
            var response = await client.GetAsync(_options["WeatherApi"]);
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode != HttpStatusCode.Unauthorized)
                    return StatusCode((int) response.StatusCode);
                
                await RefreshTokenAsync();
                return RedirectToAction();
            }
            
            var content = await response.Content.ReadAsStringAsync();
            return View((object) content);
        }

        private async Task<string> RefreshTokenAsync()
        {
            using var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync(_options.Address);
            if (disco.IsError)
                throw new Exception(disco.Error);
            //后去当前RefreshToken
            var refreshToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);
            //请求刷新令牌
            var response = await client.RequestRefreshTokenAsync(new RefreshTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = _options.ClientId,
                ClientSecret = _options.ClientSecret,
                Scope = string.Join(" ", _options.Scopes.Select(s => s.Name)), //刷新令牌时可重设Scope按需缩小授权范围
                GrantType = OpenIdConnectGrantTypes.RefreshToken,
                RefreshToken = refreshToken
            });
            if (response.IsError)
                throw new Exception(response.Error);

            //整理更新的令牌
            var tokens = new[]
            {
                new AuthenticationToken
                {
                    Name = OpenIdConnectParameterNames.IdToken,
                    Value = response.IdentityToken
                },
                new AuthenticationToken
                {
                    Name = OpenIdConnectParameterNames.AccessToken,
                    Value = response.AccessToken
                },
                new AuthenticationToken
                {
                    Name = OpenIdConnectParameterNames.RefreshToken,
                    Value = response.RefreshToken
                },
                new AuthenticationToken
                {
                    Name = "expires_at",
                    Value = DateTime.UtcNow.AddSeconds(response.ExpiresIn).ToString("o", CultureInfo.InvariantCulture)
                }
            };

            //获取 身份认证票据
            var authenticationResult =
                await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            //使用刷新后的令牌更新认证票据
            authenticationResult.Properties.StoreTokens(tokens);
            //重新登录以 重新颁发票据给客户端浏览器
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                authenticationResult.Principal,
                authenticationResult.Properties);
            return response.AccessToken;
        }

        public async Task<IActionResult> Privacy()
        {
            ViewBag.AccessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            ViewBag.IdToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.IdToken);
            ViewBag.RefreshToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);

            return View();
        }


        public async Task SignOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}