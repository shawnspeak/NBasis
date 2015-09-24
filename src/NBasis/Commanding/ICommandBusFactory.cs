using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBasis.Commanding
{
    public interface ICommandBusFactory
    {
        ICommandBus GetCommandBus(IUnityContainer container);
    }
}
