using System;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace DynamicServiceProxyNamespace
{
    /// <summary>
    /// Dynamic Proxy Class that wraps a Dynamic Object and adds the proxy constructors for the XML based services
    /// and the WSDL services. 
    /// </summary>
    public class DynamicServiceProxy : DynamicObject
    {
        /// <summary>
        /// This is the Dynamic Proxy constructor for XML type Proxies
        /// </summary>
        /// <param name="proxyType">Type of the proxy that will be created as a Dynamic Object</param>
        public DynamicServiceProxy(Type proxyType)
            : base(proxyType)
        {
            CallConstructor(new Type[0], new object[0]);
        }

        /// <summary>
        /// This is the constructor for WSDL type Proxies
        /// </summary>
        /// <param name="proxyType">Type of the proxy that will be created as a Dynamic Object</param>
        /// <param name="binding">Binding of the proxy that will be created as a Dynamic Object</param>
        /// <param name="address">EndPoint Address of the proxy that will be created as a Dynamic Object</param>
        public DynamicServiceProxy(Type proxyType, Binding binding, 
                EndpointAddress address)
            : base(proxyType)
        {
            Type[] paramTypes = new Type[2];
            paramTypes[0] = typeof(Binding);
            paramTypes[1] = typeof(EndpointAddress);

            object[] paramValues = new object[2];
            paramValues[0] = binding;
            paramValues[1] = address;

            CallConstructor(paramTypes, paramValues);
        }

        /// <summary>
        /// Proxy Type getter
        /// </summary>
        public Type ProxyType
        {
            get
            {
                return ObjectType;
            }
        }

        /// <summary>
        /// Proxy Instance getter
        /// </summary>
        public object Proxy
        {
            get
            {
                return ObjectInstance;
            }
        }

        /// <summary>
        /// Close method for the Proxy
        /// </summary>
        public void Close()
        {
            CallMethod("Close");
        }
    }
}