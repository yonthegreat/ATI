using System.Configuration;
using System.Web;
using System.Web.Caching;
using AtsATPCC.AutoPayCreditCardService;
using AtsAPCC.Models;
using StructureMap.Configuration.DSL;

namespace AtsAPCC.StructureMap
{
    public class FrameworkServicesRegistry : Registry
    {
        public FrameworkServicesRegistry()
        {
            //For<AutoPayCreditCardService>()
            //    .Use(ServiceProxyManager<AutoPayCreditCardServiceClient>.GetService);

            For<Cache>()
                .Use(HttpRuntime.Cache);
        }
    }
}
