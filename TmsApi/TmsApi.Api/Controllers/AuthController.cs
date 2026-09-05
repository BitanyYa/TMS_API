using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TmsApi.Application.Auth;

namespace TmsApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAntiforgery _antiforgery;

    public AuthController(IAntiforgery antiforgery)
    {
        _antiforgery = antiforgery;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return Unauthorized(new { message = "Invalid username or password" });
        }

        var role = request.Username.Equals("admin", StringComparison.OrdinalIgnoreCase) ? "Admin" : "Instructor";
        var displayName = request.Username.Equals("admin", StringComparison.OrdinalIgnoreCase) ? "System Administrator" : "Instructor Demo";

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, request.Username),
            new(ClaimTypes.Name, displayName),
            new(ClaimTypes.Role, role)
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

        var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
        if (!string.IsNullOrEmpty(tokens.RequestToken))
        {
            Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken, new CookieOptions
            {
                HttpOnly = false,
                Path = "/",
                SameSite = SameSiteMode.Lax,
                IsEssential = true
            });
        }

        return Ok(new UserProfileDto(displayName, role));
    }

    [HttpGet("xsrf-token")]
    [AllowAnonymous]
    public IActionResult GetXsrfToken()
    {
        var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
        if (!string.IsNullOrEmpty(tokens.RequestToken))
        {
            Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken, new CookieOptions
            {
                HttpOnly = false,
                Path = "/",
                SameSite = SameSiteMode.Lax,
                IsEssential = true
            });
        }

        return Ok(new { message = "XSRF token issued" });
    }

    [HttpGet("me")]
    public IActionResult GetProfile()
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return Unauthorized(new { message = "Not authenticated" });
        }

        var displayName = User.FindFirst(ClaimTypes.Name)?.Value ?? User.Identity?.Name ?? "User";
        var role = User.FindFirst(ClaimTypes.Role)?.Value ?? "User";

        return Ok(new UserProfileDto(displayName, role));
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        Response.Cookies.Delete("tms_auth");
        Response.Cookies.Delete("XSRF-TOKEN");
        return Ok(new { message = "Logged out successfully" });
    }
}
