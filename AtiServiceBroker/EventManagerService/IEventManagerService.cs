using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace EventManagerService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IEventManagerService
    {
        [OperationContract]
        EventResult StartEventHandlerById(int customerId, int eventId);

        [OperationContract]
        EventResult StartEventHandlerByName(string customerName, string eventName);

        [OperationContract]
        EventResult StopEventHandlerById(int customerId, int eventId);

        [OperationContract]
        EventResult StopEventHandlerByName(string customerName, string eventName);

        [OperationContract]
        EventResult CreateEventValuesOnly(string typeName, IEnumerable<object> parametersValues);

        [OperationContract]
        EventResult CreateEventNamesAndValues(string typeName, IEnumerable<string> parameterNames, IEnumerable<object> parametersValues);
        
    }

    
}
