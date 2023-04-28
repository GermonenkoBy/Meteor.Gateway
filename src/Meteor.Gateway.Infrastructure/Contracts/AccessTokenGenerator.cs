using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Meteor.Gateway.Core.Contracts;
using Meteor.Gateway.Core.Dtos;
using Meteor.Gateway.Infrastructure.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Meteor.Gateway.Infrastructure.Contracts;

public class AccessTokenGenerator : IAccessTokenGenerator
{
    private readonly IOptionsSnapshot<JwtOptions> _jwtOptions;

    public AccessTokenGenerator(IOptionsSnapshot<JwtOptions> jwtOptions)
    {
        _jwtOptions = jwtOptions;
    }

    public TokenInfo GenerateAccessToken(UserTokenData userData)
    {
        var issueDate = DateTime.UtcNow;
        var expireDate = issueDate.Add(_jwtOptions.Value.TokenLifetime);

        var tokenParameters = GetTokenValidationParameters();

        var userIdClaim = new Claim("uid", userData.EmployeeId.ToString());
        var orgIdClaim = new Claim("org", userData.CustomerId.ToString());

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            IssuedAt = issueDate,
            Expires = expireDate,
            Subject = new (new []{userIdClaim, orgIdClaim}),
            SigningCredentials = new SigningCredentials(
                tokenParameters.IssuerSigningKey,
                SecurityAlgorithms.HmacSha256Signature
            )
        };

        if (tokenParameters.ValidateIssuer)
        {
            tokenDescriptor.Issuer = tokenParameters.ValidIssuer;
        }

        if (tokenParameters.ValidateAudience)
        {
            tokenDescriptor.Issuer = tokenParameters.ValidAudience;
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenStringRepresentation = tokenHandler.CreateToken(tokenDescriptor);
        return new TokenInfo
        {
            IssueDate = issueDate,
            ExpireDate = expireDate,
            Token = tokenHandler.WriteToken(tokenStringRepresentation),
        };
    }

    public TokenValidationParameters GetTokenValidationParameters()
    {
        var jwtSecretBytes = Encoding.UTF8.GetBytes(_jwtOptions.Value.SecretKey);
        var tokenParameters = new TokenValidationParameters
        {
            ValidateIssuer = _jwtOptions.Value.ValidateIssuer,
            ValidateAudience = _jwtOptions.Value.ValidateAudience,
            ValidateLifetime = _jwtOptions.Value.ValidateLifetime,
            RequireExpirationTime = _jwtOptions.Value.RequireExpirationTime,
            ClockSkew = TimeSpan.Zero,
            IssuerSigningKey = new SymmetricSecurityKey(jwtSecretBytes)
        };

        var audience = _jwtOptions.Value.Audience;
        if (tokenParameters.ValidateAudience && !string.IsNullOrEmpty(audience))
        {
            tokenParameters.ValidAudience = audience;
        }

        var issuer = _jwtOptions.Value.Issuer;
        if (tokenParameters.ValidateIssuer && !string.IsNullOrEmpty(issuer))
        {
            tokenParameters.ValidIssuer = issuer;
        }

        return tokenParameters;
    }
}