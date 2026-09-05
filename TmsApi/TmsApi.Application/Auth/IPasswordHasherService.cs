using Microsoft.AspNetCore.Identity;
using TmsApi.Domain.Entities;

namespace TmsApi.Application.Auth;

public record HashComparisonResult(
    string RawPassword,
    string IdentityPbkdf2Hash,
    long Pbkdf2DurationTicks,
    double Pbkdf2DurationMs,
    string BcryptHash,
    long BcryptDurationTicks,
    double BcryptDurationMs,
    int BcryptWorkFactor
);

public interface IPasswordHasherService
{
    HashComparisonResult ComparePasswordHashing(string rawPassword, int workFactor = 11);
    string HashPasswordBcrypt(string password, int workFactor = 11);
    bool VerifyPasswordBcrypt(string password, string hashedPassword);
    string HashPasswordIdentity(ApplicationUser user, string password);
    PasswordVerificationResult VerifyPasswordIdentity(ApplicationUser user, string hashedPassword, string providedPassword);
}
