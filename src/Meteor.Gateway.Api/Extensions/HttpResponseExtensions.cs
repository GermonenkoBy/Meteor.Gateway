using Meteor.Common.Core.Exceptions;
using Meteor.Gateway.Api.Models;

namespace Meteor.Gateway.Api.Extensions;

public static class HttpResponseExtensions
{
    public static async Task WriteResponseError(this HttpResponse response, MeteorException exception)
    {
        ErrorResponse responseModel;

        if (exception is MeteorNotFoundException)
        {
            response.StatusCode = StatusCodes.Status404NotFound;
        }
        else
        {
            response.StatusCode = StatusCodes.Status400BadRequest;
        }

        if (exception is MeteorValidationException validationException)
        {
            responseModel = new ValidationErrorResponse(exception.Message, validationException.Errors);
        }
        else
        {
            responseModel = new ErrorResponse(exception.Message);
        }

        await response.WriteAsJsonAsync(responseModel);
    }
}