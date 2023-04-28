using System.ComponentModel.DataAnnotations;

namespace Meteor.Gateway.Api.Models.Auth;

public record UserCredentials
{
    [Required(AllowEmptyStrings = false, ErrorMessage = "Username is required.")]
    public string Username { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required.")]
    public string Password { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = false, ErrorMessage = "Domain is required.")]
    public string Domain { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = false, ErrorMessage = "Device name is required.")]
    public string DeviceName { get; set; } = string.Empty;
}