using System.Diagnostics;
using BCrypt.Net;
using Microsoft.AspNetCore.Identity;
using TmsApi.Application.Auth;
using TmsApi.Domain.Entities;

namespace TmsApi.Infrastructure.Services;

public class PasswordHasherService : IPasswordHasherService
{
    private readonly IPasswordHasher<ApplicationUser> _identityPasswordHasher;

    public PasswordHasherService(IPasswordHasher<ApplicationUser> identityPasswordHasher)
    {
        _identityPasswordHasher = identityPasswordHasher;
    }

    public string HashPasswordBcrypt(string password, int workFactor = 11)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, workFactor);
    }

    public bool VerifyPasswordBcrypt(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }

    public string HashPasswordIdentity(ApplicationUser user, string password)
    {
        return _identityPasswordHasher.HashPassword(user, password);
    }

    public PasswordVerificationResult VerifyPasswordIdentity(ApplicationUser user, string hashedPassword, string providedPassword)
    {
        return _identityPasswordHasher.VerifyHashedPassword(user, hashedPassword, providedPassword);
    }

    public HashComparisonResult ComparePasswordHashing(string rawPassword, int workFactor = 11)
    {
        var dummyUser = new ApplicationUser { UserName = "demo_user" };

        var swPbkdf2 = Stopwatch.StartNew();
        var pbkdf2Hash = _identityPasswordHasher.HashPassword(dummyUser, rawPassword);
        swPbkdf2.Stop();

        var swBcrypt = Stopwatch.StartNew();
        var bcryptHash = BCrypt.Net.BCrypt.HashPassword(rawPassword, workFactor);
        swBcrypt.Stop();

        return new HashComparisonResult(
            RawPassword: rawPassword,
            IdentityPbkdf2Hash: pbkdf2Hash,
            Pbkdf2DurationTicks: swPbkdf2.ElapsedTicks,
            Pbkdf2DurationMs: swPbkdf2.Elapsed.TotalMilliseconds,
            BcryptHash: bcryptHash,
            BcryptDurationTicks: swBcrypt.ElapsedTicks,
            BcryptDurationMs: swBcrypt.Elapsed.TotalMilliseconds,
            BcryptWorkFactor: workFactor
        );
    }
}
