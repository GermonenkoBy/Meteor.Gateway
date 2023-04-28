namespace Meteor.Gateway.Core.Dtos;

public record TokenInfo
{
    public string Token { get; set; } = string.Empty;

    public DateTimeOffset ExpireDate { get; set; }

    public DateTimeOffset IssueDate { get; set; }
}