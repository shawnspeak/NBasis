using System;
using System.Web;

namespace NBasis.Web.MultiTenant
{
    public interface ITenantResolver : IDisposable
    {
        ITenant Resolve(HttpContext context);

        void Start();
    }
}
