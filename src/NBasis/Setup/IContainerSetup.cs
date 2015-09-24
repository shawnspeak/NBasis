using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBasis.Setup
{
    public interface IContainerSetup
    {
        void BuildUp(IUnityContainer container);

        void TearDown();
    }
}
