using Microsoft.Practices.Unity;
using System;

namespace NBasis
{
    public static class ContainerExtensions
    {
        public static T TryResolve<T>(this IUnityContainer container) where T : class
        {
            try
            {
                return container.Resolve<T>();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static T Resolve<T>(this IUnityContainer container) where T : class
        {
            return container.Resolve(typeof(T)) as T;
        }

        public static IUnityContainer RegisterFactory<TInterface>(this IUnityContainer container, Func<IUnityContainer, TInterface> factory, LifetimeManager lifetime) where TInterface : class
        {
            Func<IUnityContainer, object> factoryFunc = (c) =>
            {
                return factory(c);
            };
            return container.RegisterType<TInterface>(lifetime, new InjectionFactory(factoryFunc));
        }
    }
}
