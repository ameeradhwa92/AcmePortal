using Microsoft.AspNetCore.Identity;

namespace AcmePortal.Model;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public bool IsSuspended { get; set; } = false;
}
