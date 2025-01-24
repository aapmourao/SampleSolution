using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SharedKernel.Authentication;
using SharedKernel.Infrastructure.Authentication.TokenGenerator;

namespace SharedKernel.Infrastructure.Authentication.Decorator;

public class JwtAuthorizeFilter(IJwtTokenValidator jwtTokenValidator) : IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // Check if the [Authorize] attribute is explicitly applied to the action or controller.
        var hasAuthorizeAttribute = context.ActionDescriptor.EndpointMetadata
            .Any(em => em is AuthorizeAttribute);

        if (hasAuthorizeAttribute)
        {
            var token = context.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    // Validate the token and extract claims
                    var claimsPrincipal = jwtTokenValidator.ValidateJwtToken(token);

                    // Extract the user ID from the token
                    context.HttpContext.Items["NameIdentification"] = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    context.HttpContext.Items["Name"] = claimsPrincipal.FindFirst(ClaimTypes.Name)?.Value;

                    claimsPrincipal.FindAll(ClaimTypes.Role).ToList().ForEach(role =>
                    {
                        context.HttpContext.Items["Role"] = role.Value;
                    });

                    claimsPrincipal.FindAll(CustomClaimTypes.Profile).ToList().ForEach(profile =>
                    {
                        context.HttpContext.Items["Profile"] = profile.Value;
                    });
                }
                catch (Exception)
                {
                    context.Result = new UnauthorizedResult();
                }
            }
            else
            {
                context.Result = new UnauthorizedResult();
            }
        }
    }

}