using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TmsApi.Application.Auth;
using TmsApi.Domain.Entities;

namespace TmsApi.Api.Controllers;

public record HashDemoRequest(string Password, int WorkFactor = 11);
public record VerifyDemoRequest(string Password, string Hash, string Algorithm);

[ApiController]
[Route("api/[controller]")]
public class PasswordDemoController : ControllerBase
{
    private readonly IPasswordHasherService _passwordHasherService;

    public PasswordDemoController(IPasswordHasherService passwordHasherService)
    {
        _passwordHasherService = passwordHasherService;
    }

    [HttpPost("benchmark")]
    [AllowAnonymous]
    public IActionResult CompareHashes([FromBody] HashDemoRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new { message = "Password string is required for benchmarking." });
        }

        var result = _passwordHasherService.ComparePasswordHashing(request.Password, request.WorkFactor);
        return Ok(result);
    }

    [HttpPost("verify")]
    [AllowAnonymous]
    public IActionResult VerifyHash([FromBody] VerifyDemoRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Password) || string.IsNullOrWhiteSpace(request.Hash))
        {
            return BadRequest(new { message = "Password and Hash are required." });
        }

        if (request.Algorithm.Equals("bcrypt", StringComparison.OrdinalIgnoreCase))
        {
            var isValid = _passwordHasherService.VerifyPasswordBcrypt(request.Password, request.Hash);
            return Ok(new { algorithm = "BCrypt", isValid });
        }

        var dummyUser = new ApplicationUser { UserName = "demo" };
        var identityResult = _passwordHasherService.VerifyPasswordIdentity(dummyUser, request.Hash, request.Password);
        var isIdentityValid = identityResult != Microsoft.AspNetCore.Identity.PasswordVerificationResult.Failed;

        return Ok(new { algorithm = "Identity (PBKDF2)", isValid = isIdentityValid, verificationResult = identityResult.ToString() });
    }
}
