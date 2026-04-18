using System.Security.Claims;
using System.Security.Principal;

namespace AcmePortal.Helpers;

public static class IdentityExtension
{
    public static string? GetFullName(this IIdentity identity)
    {
        return ((ClaimsIdentity)identity).FindFirst("FullName")?.Value;
    }
}
