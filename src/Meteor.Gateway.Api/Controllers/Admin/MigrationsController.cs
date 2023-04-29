using Meteor.Gateway.Core.Dtos;
using Meteor.Gateway.Core.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Meteor.Gateway.Api.Controllers.Admin;

[ApiController, Route("api/admin/migrations")]
public class MigrationsController : ControllerBase
{
    private readonly IMigrationsTriggerService _migrationsTriggerService;

    public MigrationsController(IMigrationsTriggerService migrationsTriggerService)
    {
        _migrationsTriggerService = migrationsTriggerService;
    }

    [HttpPost("")]
    [SwaggerOperation("Triggers migrations for specified customers.")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "No content success response.")]
    public async Task<NoContentResult> TriggerMigrationsAsync(MigrationsTriggerDto migrationsDto)
    {
        await _migrationsTriggerService.TriggerASync(migrationsDto);
        return NoContent();
    }
}