using System;
using Microsoft.AspNetCore.Identity;

namespace TmsApi.Domain.Entities;

public class ApplicationUser : IdentityUser<int>
{
    public string DisplayName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
