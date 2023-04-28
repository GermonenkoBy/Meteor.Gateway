namespace Meteor.Gateway.Core.Models;

public class Session
{
    public Guid Id { get; set; }

    public int EmployeeId { get; set; }

    public int CustomerId { get; set; }

    public string Token { get; set; } = string.Empty;

    public string DeviceName { get; set; } = string.Empty;

    public string IpAddress { get; set; } = string.Empty;

    public DateTimeOffset CreateDate { get; set; }

    public DateTimeOffset LastRefreshDate { get; set; }

    public DateTimeOffset ExpireDate { get; set; }
}