using Meteor.Gateway.Core.Dtos;
using Meteor.Gateway.Core.Models;

namespace Meteor.Gateway.Core.Contracts;

public interface ISessionsClient
{
    Task<Session> StartSessionAsync(SignInDto signInDto);

    Task<Session> RefreshTokenAsync(string token);
}