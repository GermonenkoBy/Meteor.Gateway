namespace Meteor.Gateway.Core.Dtos;

public record struct AuthResult
{
    public int CustomerId;
    public int EmployeeId;
    public TokenInfo AccessToken;
    public TokenInfo RefreshToken;
}