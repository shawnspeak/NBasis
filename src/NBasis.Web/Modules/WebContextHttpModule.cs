using Microsoft.Practices.Unity;
using NBasis.Setup;
using NBasis.Web.MultiTenant;
using System;
using System.IO;
using System.Web;

namespace NBasis.Web.Modules
{
    internal class WebContextHttpModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            // do we have a multi-tenant setup
            if (MultiTenantConfig.Current.Running)
                context.BeginRequest += MultiTenant_BeginRequest;
            else
                context.BeginRequest += SingleTenant_BeginRequest;

            context.EndRequest += Ctx_EndRequest;
        }

        void SingleTenant_BeginRequest(object sender, EventArgs e)
        {
            WebContext.InitializeCurrent(ContainerConfig.Container);
        }
        
        void MultiTenant_BeginRequest(object sender, EventArgs e)
        {
            HttpContext context = ((HttpApplication)sender).Context;

            // resolve tenant
            ITenant tenant = MultiTenantConfig.Current.Resolver.Resolve(context);
            if (tenant == null)
            {
                // if no local file
                if (!File.Exists(context.Request.PhysicalPath))
                    throw new HttpException(404, "File Not Found");
            }

            // tenant might be null if there is a physical file
            if (tenant != null)
            {
                // setup web context w/ tenant
                WebContext.InitializeCurrent(ContainerConfig.Container, (childContainer) =>
                {
                    childContainer.RegisterInstance<ITenant>(tenant);
                });
            }
        }

        void Ctx_EndRequest(object sender, EventArgs e)
        {
            // clear web context
            WebContext.ClearCurrent();
        }

        public void Dispose()
        {
            // nothing to do here
        }
    }
}
