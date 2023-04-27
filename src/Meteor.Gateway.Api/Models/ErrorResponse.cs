using System.ComponentModel.DataAnnotations;

namespace Meteor.Gateway.Api.Models;

public record ErrorResponse(string Message);

public record ValidationErrorResponse(
    string Message,
    ICollection<ValidationResult> Errors
) : ErrorResponse(Message);
