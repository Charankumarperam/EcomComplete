using System.Security.Claims;
using Interfaces.IManagers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs;
namespace EcomComplete.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthManager _authManager;
        
        public AuthController(IAuthManager authManager)
        {
            
            _authManager = authManager;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Models.DTOs.Register dto)
        {
            var result= await _authManager.Register(dto);
            if(!result.Success)
            {
                return BadRequest(result.Message);
            }
            return Ok(result);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Login dto)
        {
            var result = await _authManager.Login(dto);
            if(!result.Success)
            {
                return Unauthorized(result.Message);
            }
            var claims= new List<Claim>
            {
                new Claim(ClaimTypes.Name, result.Data.UserName),
                new Claim(ClaimTypes.Email, result.Data.Email),
                new Claim("Age", result.Data.Age.ToString()),

            };
            foreach(var role in result.Data.Roles)
            {
               claims.Add(new Claim(ClaimTypes.Role, role));
            }
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties { IsPersistent=true});
            return Ok(result);
        }
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(new { Message = "Logged out successfully" });
        }
        [Authorize(Policy = "MinimumAgePolicy")]
        [HttpGet("adult-section")]
        public IActionResult AdultSection() => Ok("Welcome to Adult Section!");

      


        [Authorize(Policy = "AdminPolicy")]
        [HttpGet("admin-section")]
        public IActionResult AdminSection() => Ok("Welcome Admin Department!");


    }
}
