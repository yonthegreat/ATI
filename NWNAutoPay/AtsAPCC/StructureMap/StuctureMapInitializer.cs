using AtsATPCC.AutoPayCreditCardService;
using AtsAPCC.Models;
using StructureMap;

namespace AtsAPCC.StructureMap
{
    public class StuctureMapInitializer
    {
        public static void Initialize()
        {
            if (!ObjectFactory.Model.HasImplementationsFor(typeof(AutoPayCreditCardService)))
            {
                ObjectFactory.Initialize(Initialize);
            }
        }

        private static void Initialize(IInitializationExpression initializer)
        {
            initializer.AddRegistry(new FrameworkServicesRegistry());
        }
    }
}