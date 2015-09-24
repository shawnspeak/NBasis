using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace NBasis.Web.Mvc
{
    public class WebContextDependencyResolver : System.Web.Mvc.IDependencyResolver
    {
        public WebContextDependencyResolver()
        {
        }

        /// <summary>
        /// All MVC resolution happens against a child container. At the end of the request, the child container is disposed
        /// </summary>
        protected virtual IUnityContainer ChildContainer
        {
            get
            {
                // the contaier associated with the WebContext is request specific
                if (WebContext.Current == null)
                    throw new ApplicationException("WebContext is not initialized");
                return WebContext.Current.Container;
            }
        }

        public object GetService(Type serviceType)
        {
            // resolve controllers regardless
            if (typeof(IController).IsAssignableFrom(serviceType))
            {
                return ChildContainer.Resolve(serviceType);
            }

            if (ChildContainer.IsRegistered(serviceType))
            {
                return ChildContainer.Resolve(serviceType);
            }

            return null;
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            if (ChildContainer.IsRegistered(serviceType))
            {
                return ChildContainer.ResolveAll(serviceType);
            }
            else
            {
                return Enumerable.Empty<object>();
            }
        }
    }
}
