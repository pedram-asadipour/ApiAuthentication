using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using WebSite.Models;
using JsonConverter = System.Text.Json.Serialization.JsonConverter;

namespace WebSite.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClient;

        public HomeController(IHttpClientFactory httpClient)
        {
            _httpClient = httpClient;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index([FromForm] LoginViewModel command)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("username","all Field Required");
                return View(ModelState);
            }

            var client = _httpClient.CreateClient("ApiServer");

            var content = new StringContent(
                JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json");

            var result = client.PostAsync("api/auth",content).Result;

            var tokenResult = result.Content.ReadAsStringAsync().Result;

            var token = JsonConvert.DeserializeObject<TokeViewModel>(tokenResult);

            //Authentication
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,command.Username),
                new Claim("Token",token.Token)
            };
            var identity = new ClaimsIdentity(claims,CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            var properties = new AuthenticationProperties()
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(2),
                AllowRefresh = true
            };

            HttpContext.SignInAsync(principal, properties);

            return RedirectToAction("Result");
        }

        [Authorize]
        public IActionResult Result()
        {
            var token = User.Claims.SingleOrDefault(x => x.Type == "Token")?.Value;

            var client = _httpClient.CreateClient("ApiServer");

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var result = client.GetAsync("api/Result").Result;

            var contentResult = result.Content.ReadAsStringAsync().Result;

            return new JsonResult(JsonConvert.DeserializeObject<Personal>(contentResult));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

