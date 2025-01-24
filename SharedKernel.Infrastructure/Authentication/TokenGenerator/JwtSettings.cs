namespace SharedKernel.Infrastructure.Authentication.TokenGenerator;

public class JwtSettings
{
    public const string Section = "JwtSettings";

    public string IdentityUrl { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public string Issuer { get; set; } = null!;
    public string Secret { get; set; } = null!;
    public int TokenExpirationInMinutes { get; set; }
    public bool RequireHttps { get; set; } = false;
}