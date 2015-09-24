using System;
using System.Web;

namespace NBasis.Web.MultiTenant
{
    public enum TenantStatus
    {
        Unloaded,
        Loaded,
        Error
    }

    public class Tenant : ITenant
    {
        object _loadLock = new object();

        public Tenant()
        {
        }

        public Tenant(Guid id, String hostName, long timeStamp, int flags, string timezoneId = null)
        {
            Id = id;
            HostName = hostName;
            Timestamp = timeStamp;
            TimezoneId = timezoneId;
            Flags = flags;
        }

        public Guid Id
        {
            get;
            internal set;
        }

        public string HostName
        {
            get;
            internal set;
        }

        public string TimezoneId
        {
            get;
            internal set;
        }

        public long Timestamp
        {
            get;
            internal set;
        }

        public int Flags
        {
            get;
            internal set;
        }

        public TenantStatus Status { get; internal set; }
        
        public void Load(HttpContext context)
        {
            if (Status == TenantStatus.Unloaded)
            {
                lock (_loadLock)
                {
                    if (Status == TenantStatus.Unloaded)
                    {
                        // invoke load action
                        MultiTenantConfig.Current.TenantLoadAction.Invoke(this);

                        Status = TenantStatus.Loaded;
                    }
                }
            }
        }

        public void Unload()
        {
            if (Status == TenantStatus.Loaded)
            {
                lock (_loadLock)
                {
                    if (Status == TenantStatus.Loaded)
                    {
                        // invoke unload action
                        MultiTenantConfig.Current.TenantUnloadAction.Invoke(this);

                        Status = TenantStatus.Unloaded;
                    }
                }
            }
        }
    }
}
