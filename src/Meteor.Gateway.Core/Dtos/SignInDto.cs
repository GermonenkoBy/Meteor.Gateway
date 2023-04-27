namespace Meteor.Gateway.Core.Dtos;

public record struct SignInDto
{
    public string Username;
    public string Password;
    public string Domain;
    public string IpAddress;
    public string DeviceName;
}