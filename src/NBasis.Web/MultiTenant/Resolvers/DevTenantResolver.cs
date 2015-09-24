using System;

namespace NBasis.Web.MultiTenant.Resolvers
{
    public class DevTenantResolver : ITenantResolver
    {
        private Tenant _singleTenant;

        public DevTenantResolver(Tenant tenant)
        {
            _singleTenant = tenant;
        }

        public ITenant Resolve(System.Web.HttpContext context)
        {
            if (_singleTenant.Status == TenantStatus.Unloaded)
                _singleTenant.Load(context);

            return _singleTenant;
        }
        public void Start()
        {
            // nothing to do here
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
        }
    }
}
