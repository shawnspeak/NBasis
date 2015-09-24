using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using NBasis.Web.Modules;
using System.Web;

[assembly: PreApplicationStartMethod(typeof(NBasis.Web.PreApplicationStart), "PreStart")]

namespace NBasis.Web
{
    public class PreApplicationStart
    {
        private static bool _isStarting;

        public static void PreStart()
        {
            if (!_isStarting)
            {
                _isStarting = true;

                DynamicModuleUtility.RegisterModule(typeof(WebContextHttpModule));
            }
        }
    }
}
