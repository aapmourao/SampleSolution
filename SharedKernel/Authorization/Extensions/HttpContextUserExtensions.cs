using System.Security.Claims;
using SharedKernel.Domain.Profiles;

namespace SharedKernel.Authorization.Extensions;

public static class HttpContextExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        return Guid.Parse(principal.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value);
    }
    public static bool IsAdmin(this ClaimsPrincipal principal)
    {
        return principal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role && claim.Value == ProfileEnum.Admin.Name) is not null;
    }

    public static bool IsOwner(this ClaimsPrincipal principal, Guid userId)
    {
        var requestUserId = Guid.Parse(principal.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value);
        return requestUserId == userId;
    }

    public static bool IsProvider(this ClaimsPrincipal principal)
    {
        return principal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role && claim.Value == ProfileEnum.Provider.Name) is not null;
    }

    public static bool IsParticipant(this ClaimsPrincipal principal)
    {
        return principal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role && claim.Value == ProfileEnum.Participant.Name) is not null;
    }

    public static string SubscriptionId(this ClaimsPrincipal principal)
    {
        return principal.Claims.First(claim => claim.Type == CustomClaimTypes.Subscription)?.Value ?? string.Empty;
    }
}