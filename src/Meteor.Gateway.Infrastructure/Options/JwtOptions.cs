namespace Meteor.Gateway.Infrastructure.Options;

public record JwtOptions
{
    public TimeSpan TokenLifetime { get; private set; }

    public int TokenLifetimeSeconds
    {
        get => TokenLifetime.Seconds;
        set => TokenLifetime = TimeSpan.FromMinutes(value);
    }

    public bool ValidateIssuer { get; set; }

    public bool ValidateAudience { get; set; }

    public bool ValidateLifetime { get; set; }

    public bool RequireExpirationTime { get; set; }

    public string? Audience { get; set; }

    public string? Issuer { get; set; }

    public string SecretKey { get; set; } = string.Empty;
}