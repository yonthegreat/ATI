using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel.Channels;
using DynamicServiceProxyNamespace;

namespace AtiWrapperServices
{
    /// <summary>
    /// AtiWrapperServices  ProxyInformation Container class
    /// </summary>
    public class ProxyInformation
    {
        public DynamicServiceProxy proxy;
        public Dictionary<int, MessageHeader> proxyHeaders = new Dictionary<int, MessageHeader>();
    }
}