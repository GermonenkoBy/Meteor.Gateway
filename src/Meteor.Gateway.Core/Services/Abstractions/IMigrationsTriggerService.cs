using Meteor.Gateway.Core.Dtos;

namespace Meteor.Gateway.Core.Services.Abstractions;

public interface IMigrationsTriggerService
{
    Task TriggerASync(MigrationsTriggerDto migrationsDto);
}