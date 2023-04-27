using Meteor.Gateway.Core.Dtos;

namespace Meteor.Gateway.Api.Models.Auth;

public record AuthResult
{
    public TokenInfo AccessToken { get; set; }
    public TokenInfo RefreshToken { get; set; }
}