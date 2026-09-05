using Microsoft.AspNetCore.Identity;

namespace TmsApi.Domain.Entities;

public class ApplicationRole : IdentityRole<int>
{
    public string Description { get; set; } = string.Empty;
}
