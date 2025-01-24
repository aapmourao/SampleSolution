using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace SharedKernel.Infrastructure.Authentication.TokenGenerator;

public interface IJwtTokenValidator
{
    ClaimsPrincipal ValidateJwtToken(string token);
}

public class JwtTokenValidator: IJwtTokenValidator
{
    private readonly JwtSettings _jwtSettings;

    public JwtTokenValidator(IOptions<JwtSettings> jwtOptions)
    {
        _jwtSettings = jwtOptions.Value;
    }

    public ClaimsPrincipal ValidateJwtToken(string token)
    {
        var tokenHandler1 = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler1.ReadJwtToken(token);

        // Retrieve the JWT secret from environment variables and encode it
        var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

        try
        {
            // Create a token handler and validate the token
            var tokenHandler = new JwtSecurityTokenHandler();
            var claims = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidAudience = _jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            }, out SecurityToken validatedToken);

            // Return the claims principal
            return claims;
        }
        catch (SecurityTokenExpiredException)
        {
            // Handle token expiration
            throw new ApplicationException("Token has expired.");
        }
    }
}