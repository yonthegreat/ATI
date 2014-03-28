using System;
using System.ServiceModel;
using System.ServiceModel.Security;
using AtsAPCC.ClientProxy;
using AtsAPCC.Logging;

namespace AtsAPCC.StructureMap
{
    //public static class ServiceProxyManager<T>
    //    where T : ICommunicationObject, IServiceBase, new()
    //{
    //    private static T _instance = new T();
    //    private static readonly object _padLock = new object();

    //    public static T GetService()
    //    {
    //        try
    //        {
    //            if (_instance.State != CommunicationState.Opened)
    //            {
    //                lock (_padLock)
    //                {
    //                    if (_instance.State == CommunicationState.Faulted)
    //                    {
    //                        _instance.Abort();
    //                        CreateNewProxy();
    //                    }
    //                }
    //            }

    //            try
    //            {
    //                _instance.IsAlive();
    //            }
    //            catch (MessageSecurityException)
    //            {
    //                lock (_padLock)
    //                {
    //                    CreateNewProxy();
    //                }
    //            }

    //            return _instance;
    //        }
    //        catch (Exception e)
    //        {
    //            LoggingAttribute.LogException(e);
    //            throw;
    //        }
    //    }

    //    private static void CreateNewProxy()
    //    {
    //        _instance = new T();
    //        _instance.Faulted += InstanceFaulted;
    //        _instance.Open();
    //    }

    //    private static void InstanceFaulted(object sender, EventArgs e)
    //    {
    //        lock (_padLock)
    //        {
    //            CreateNewProxy();
    //        }
    //    }
    //}
}