using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Ati.ServiceHost;
using Ati.ServiceHost.Web;
using System.ComponentModel.Composition;

namespace RulesEngine
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IRulesEngine
    {
        [OperationContract]
        RulesResult ApplyRuleById(int customerId, int ruleId, IEnumerable<object> parameters);

        [OperationContract]
        RulesResult ApplyRuleByName(string customerName, string ruleName, IEnumerable<object> parameters);

    }

}
