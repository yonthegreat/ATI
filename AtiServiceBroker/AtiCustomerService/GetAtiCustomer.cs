using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Ati.ServiceHost.Web;
using Ati.ServiceHost;

namespace AtiCustomerService
{
    [ExportService("GetAtiCustomer", typeof(GetAtiCustomer))]
    public class GetAtiCustomer : IAtiCustomerService
    {

        public string GetCustomerName(int customerNumber)
        {
            return "Test";
        }


        public int GetCustomerNumber(string customerName)
        {
            return 0;
        }
    }
}
