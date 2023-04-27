using Meteor.Common.Core.Exceptions;
using Meteor.Gateway.Api.Extensions;
using Meteor.Gateway.Api.Models;

namespace Meteor.Gateway.Api.Middleware;

public class ExceptionsMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionsMiddleware> _logger;

    public ExceptionsMiddleware(ILogger<ExceptionsMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (MeteorException e)
        {
            await context.Response.WriteResponseError(e);
            var requestDebugView = await context.Request.ToDebugViewAsync(false);
            _logger.LogInformation("Bad request returned for the following request:\n{Request}", requestDebugView);
        }
        catch (Exception e)
        {
            var response = new ErrorResponse("Unknown error occured. If the issue persists please contact support.");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(response);

            var requestDebugView = await context.Request.ToDebugViewAsync(false);
            _logger.LogError(
                e,
                "Unhandled exception thrown when processing the following request:\n{Request}",
                requestDebugView
            );
        }
    }
}