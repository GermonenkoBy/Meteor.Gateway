using Meteor.Gateway.Core.Contracts;
using Meteor.Gateway.Core.Dtos;
using Meteor.Gateway.Core.Models;
using Meteor.Gateway.Core.Services.Abstractions;

namespace Meteor.Gateway.Core.Services;

public class AuthService : IAuthService
{
    private readonly ISessionsClient _sessionsClient;

    private readonly IAccessTokenGenerator _accessTokenGenerator;

    public AuthService(ISessionsClient sessionsClient, IAccessTokenGenerator accessTokenGenerator)
    {
        _sessionsClient = sessionsClient;
        _accessTokenGenerator = accessTokenGenerator;
    }

    public async Task<AuthResult> AuthorizeAsync(SignInDto signInDto)
    {
        var session = await _sessionsClient.StartSessionAsync(signInDto);
        return GetAuthResult(session);
    }

    public async Task<AuthResult> RefreshAsync(string token)
    {
        var session = await _sessionsClient.RefreshTokenAsync(token);
        return GetAuthResult(session);
    }

    private AuthResult GetAuthResult(Session session)
    {
        var accessToken = _accessTokenGenerator.GenerateAccessToken(new()
        {
            CustomerId = session.CustomerId,
            EmployeeId = session.EmployeeId,
        });
        var refreshToken = new TokenInfo
        {
            Token = session.Token,
            ExpireDate = session.ExpireDate,
            IssueDate = session.LastRefreshDate,
        };
        return new()
        {
            CustomerId = session.CustomerId,
            EmployeeId = session.EmployeeId,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
        };
    }
}