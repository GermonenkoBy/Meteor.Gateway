namespace Meteor.Gateway.Api.Middleware;

public class RequestBufferingMiddleware : IMiddleware
{
    public Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        context.Request.EnableBuffering();
        return next(context);
    }
}