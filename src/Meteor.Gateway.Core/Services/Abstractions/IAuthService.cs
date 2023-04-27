using Meteor.Gateway.Core.Dtos;

namespace Meteor.Gateway.Core.Services.Abstractions;

public interface IAuthService
{
    Task<AuthResult> AuthorizeAsync(SignInDto signInDto);

    Task<AuthResult> RefreshAsync(string token);
}