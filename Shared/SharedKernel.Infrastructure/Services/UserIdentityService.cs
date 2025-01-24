using Microsoft.AspNetCore.Http;
using SharedKernel.Authorization.Interfaces;

namespace SharedKernel.Infrastructure.Services;

public class UserIdentityService : IUserIdentityService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserIdentityService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string UserHasPermission(string permission)
    {
        return _httpContextAccessor.HttpContext?.User?.HasClaim("permissions", permission) ?? false ? "true" : "false";
    }

    public string GetUserId()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value ?? string.Empty;
    }

    public string GetUserName()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirst("name")?.Value ?? string.Empty;
    }

    public string GetUserEmail()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirst("email")?.Value ?? string.Empty;
    }

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public bool IsAdmin => _httpContextAccessor.HttpContext?.User?.HasClaim("role", "admin") ?? false;
}