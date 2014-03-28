using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace DataTransformationService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IDataTransfomationService
    {
        [OperationContract]
        string GetErrorTextByErrorCode(string cutomerName, int errorCode);

        [OperationContract]
        int GetErrorCodeByErrorText(string customerName, string errorText);

        [OperationContract]
        string GetErrorDescriptionByErrorCode(string cutomerName, int errorCode);

        [OperationContract]
        string GetErrorDescriptionByErrorText(string customerName, string errorText);

    }

    
}
