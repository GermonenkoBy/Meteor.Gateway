using Meteor.Gateway.Core.Dtos;

namespace Meteor.Gateway.Core.Contracts;

public interface IAccessTokenGenerator
{
    TokenInfo GenerateAccessToken(UserTokenData userData);
}