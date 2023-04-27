using Grpc.Core;
using MapsterMapper;
using Meteor.Common.Core.Exceptions;
using Meteor.Gateway.Core.Contracts;
using Meteor.Gateway.Core.Dtos;
using Meteor.Gateway.Infrastructure.Grpc;

namespace Meteor.Gateway.Infrastructure.Contracts;

public class GrpcSessionsClient : ISessionsClient
{
    private readonly SessionsService.SessionsServiceClient _grpcClient;

    private readonly IMapper _mapper;

    public GrpcSessionsClient(SessionsService.SessionsServiceClient grpcClient, IMapper mapper)
    {
        _grpcClient = grpcClient;
        _mapper = mapper;
    }

    public async Task<Core.Models.Session> StartSessionAsync(SignInDto signInDto)
    {
        try
        {
            var request = _mapper.Map<StartSessionRequest>(signInDto);
            var session = await _grpcClient.StartSessionAsync(request);
            return _mapper.Map<Core.Models.Session>(session);
        }
        catch (RpcException e) when(e.StatusCode == StatusCode.InvalidArgument)
        {
            throw new MeteorException(e.Message, e);
        }
    }

    public async Task<Core.Models.Session> RefreshTokenAsync(string token)
    {
        try
        {
            var session = await _grpcClient.RefreshTokenAsync(new()
            {
                Token = token,
            });
            return _mapper.Map<Core.Models.Session>(session);
        }
        catch (RpcException e) when (e.StatusCode == StatusCode.InvalidArgument)
        {
            throw new MeteorException(e.Message, e);
        }
        catch (RpcException e) when (e.StatusCode == StatusCode.NotFound)
        {
            throw new MeteorNotFoundException(e.Message, e);
        }
    }
}