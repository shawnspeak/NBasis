using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBasis.Setup
{
    public static class ContainerConfig
    {
        private static object _Lock = new object();
        private static IList<IContainerSetup> _Setups;

        public static IUnityContainer Container { get; private set; }

        public static void SetContainer(IUnityContainer container)
        {
            if (Container != null)
                throw new ApplicationException("Container has already been set");

            Container = container;
        }

        public static void Setup(params IContainerSetup[] setups)
        {
            lock (_Lock)
            {
                if (_Setups != null)
                    return; // already built up
                _Setups = new List<IContainerSetup>();

                // if no container, create one automatically
                if (Container == null)
                    SetContainer(new UnityContainer());

                // run through build ups
                setups.ForEach((s) =>
                {
                    s.BuildUp(Container);
                    _Setups.Add(s);
                });
            }
        }

        public static void TearDown()
        {
            lock (_Lock)
            {
                if (_Setups == null)
                    return; // already torn down

                // run through teardowns
                _Setups.ForEach((s) =>
                {
                    s.TearDown();
                });

                _Setups.Clear();
                _Setups = null;

                Container.Dispose();
                Container = null;
            }
        }
    }
}
