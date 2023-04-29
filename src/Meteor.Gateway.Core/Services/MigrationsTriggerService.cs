using Meteor.Common.Messaging.Abstractions;
using Meteor.Gateway.Core.Dtos;
using Meteor.Gateway.Core.Services.Abstractions;

namespace Meteor.Gateway.Core.Services;

public class MigrationsTriggerService : IMigrationsTriggerService
{
    private readonly IPublisher<MigrationsTriggerDto> _publisher;

    public MigrationsTriggerService(IPublisher<MigrationsTriggerDto> publisher)
    {
        _publisher = publisher;
    }

    public Task TriggerASync(MigrationsTriggerDto migrationsDto)
    {
        return _publisher.PublishAsync(migrationsDto);
    }
}