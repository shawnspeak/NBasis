using NBasis.Configuration;
using System;

namespace NBasis.Web.MultiTenant
{
    public interface IMultiTenantBuilder : IConfigurable<MultiTenantConfig, IMultiTenantBuilder>
    {
        void StartResolver();
    }

    public static class MultiTenantConfigExtensions
    {
        public static IMultiTenantBuilder WithResolver(this IMultiTenantBuilder builder, ITenantResolver resolver)
        {
            return builder.Configure((config) =>
            {
                config.Resolver = resolver;
            });
        }

        public static IMultiTenantBuilder OnTenantLoad(this IMultiTenantBuilder builder, Action<Tenant> tenantLoadAction)
        {
            return builder.Configure((config) =>
            {
                config.TenantLoadAction = tenantLoadAction;
            });
        }

        public static IMultiTenantBuilder OnTenantUnload(this IMultiTenantBuilder builder, Action<Tenant> tenantUnloadAction)
        {
            return builder.Configure((config) =>
            {
                config.TenantUnloadAction = tenantUnloadAction;
            });
        }
    }

    public class MultiTenantConfig : IMultiTenantBuilder, IDisposable
    {
        static MultiTenantConfig _instance;

        public static MultiTenantConfig Current
        {
            get
            {
                if (_instance == null)
                    _instance = new MultiTenantConfig();
                return _instance;
            }
        }

        private MultiTenantConfig()
        {
            // set default actions
            TenantLoadAction = (t) => { return; };
            TenantUnloadAction = (t) => { return; };
        }

        public IMultiTenantBuilder Configure(Action<MultiTenantConfig> configurator)
        {
            configurator(this);
            return this;
        }

        internal ITenantResolver Resolver
        {
            get;
            set;
        }

        internal Action<Tenant> TenantLoadAction
        {
            get;
            set;
        }

        internal Action<Tenant> TenantUnloadAction
        {
            get;
            set;
        }

        public bool Running { get; private set; }

        public void StartResolver()
        {
            if (Resolver == null)
                throw new TenantResolverNotConfiguredException();
            Resolver.Start();
            Running = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
            if (Resolver != null)
                Resolver.Dispose();
        }
    }
}
