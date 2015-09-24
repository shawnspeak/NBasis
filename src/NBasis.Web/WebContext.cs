using Microsoft.Practices.Unity;
using System;
using System.Threading;
using System.Web;

namespace NBasis.Web
{
    public sealed class WebContext
    {
        internal const string CONTEXT_NAME = "_NBasisWebContext";

        private IUnityContainer _container = null;

        private WebContext(IUnityContainer container)
        {
            _container = container;            
        }

        private void Cleanup()
        {
            if (_container != null)
                _container.Dispose();
        }

        #region Singleton
        
        public static void InitializeCurrent(IUnityContainer container, Action<IUnityContainer> register = null)
        {
            if (Current == null)
            {
                var childContainer = container.CreateChildContainer();
                if (register != null)
                    register(childContainer);
                WebContext context = new WebContext(childContainer);

                // store appropriately for the setting
                if (HttpContext.Current != null)
                {
                    HttpContext.Current.Items[CONTEXT_NAME] = context;
                }
                else
                {
                    LocalDataStoreSlot slot = Thread.GetNamedDataSlot(CONTEXT_NAME);
                    Thread.SetData(slot, context);
                }
            }
        }

        public static WebContext Current
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    return HttpContext.Current.Items[CONTEXT_NAME] as WebContext;
                }
                else
                {
                    LocalDataStoreSlot slot = Thread.GetNamedDataSlot(CONTEXT_NAME);
                    return Thread.GetData(slot) as WebContext;
                }
            }
        }

        public static void ClearCurrent()
        {
            WebContext context = Current;
            if (HttpContext.Current != null)
            {
                HttpContext.Current.Items.Remove(CONTEXT_NAME);
            }
            else
            {
                Thread.FreeNamedDataSlot(CONTEXT_NAME);
            }
            if (context != null)
            {
                context.Cleanup();
            }
        }

        #endregion

        public IUnityContainer Container
        {
            get
            {
                return _container;
            }
        }
    }
}
