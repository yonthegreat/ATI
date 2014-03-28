using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Ati.ServiceHost;
using Ati.ServiceHost.Web;

namespace RulesEngine
{
    [ExportService("BatchProcessService", typeof(BatchProcess))]
    public class BatchProcess : IRulesEngine
    {
        public RulesResult ApplyRuleById(int customerId, int ruleId, IEnumerable<object> parameters)
        {
            return new RulesResult();
        }

        public RulesResult ApplyRuleByName(string customerName, string ruleName, IEnumerable<object> parameters)
        {
            return new RulesResult();
        }
    }
}
