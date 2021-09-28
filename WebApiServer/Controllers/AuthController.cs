using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace WebApiServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpPost]
        public IActionResult Login([FromBody] Auth command)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (command.Username != "pedram" || command.Password != "1234")
                return Unauthorized();

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("AfasGadsgaeE689ASdeqte"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "http://localhost:3946",
                claims: new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, command.Username)
                },
                expires:DateTime.UtcNow.AddDays(2),
                signingCredentials: credentials);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }
    }

    public class Auth
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}