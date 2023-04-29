using System.ComponentModel.DataAnnotations;

namespace Meteor.Gateway.Core.Dtos;

public record MigrationsTriggerDto
{
    [Required(ErrorMessage = "Customer IDs are required.")]
    [MinLength(1, ErrorMessage = "At least one customer ID must be provided.")]
    public List<int> CustomerIds { get; set; } = new();
}