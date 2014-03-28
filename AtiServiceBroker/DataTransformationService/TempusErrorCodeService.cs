using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Ati.ServiceHost;
using Ati.ServiceHost.Web;

namespace DataTransformationService
{
    [ExportService("TempusErrorCodeService", typeof(TempusErrorCodeService))]
    public class TempusErrorCodeService : IDataTransfomationService
    {
        public string GetErrorTextByErrorCode(string cutomerName, int errorCode)
        {
            return "Test";
        }


        public int GetErrorCodeByErrorText(string customerName, string errorText)
        {
            return 0;
        }


        public string GetErrorDescriptionByErrorCode(string cutomerName, int errorCode)
        {
            return "Test";
        }

        public string GetErrorDescriptionByErrorText(string customerName, string errorText)
        {
            return "Test";
        }
    }
}
