using System;

namespace NBasis.Web.MultiTenant
{
    public interface ITenant
    {
        Guid Id { get; }

        string HostName { get; }

        string TimezoneId { get; }

        long Timestamp { get; }

        int Flags { get; }
    }
}
