using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace CustomerPassThruService
{
    [ServiceContract]
    public interface ICustomerPassThruService
    {
        [OperationContract]
        object PassThruByName(string customerName, string seriviceName, bool isTest, IEnumerable<object> parameters);

        [OperationContract]
        object PassThruByNumber(int customerNummber, string serviceName, bool isTest, IEnumerable<object> parameters);
    }
}
