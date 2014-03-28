using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Xml;


namespace DynamicServiceProxyNamespace
{
    /// <summary>
    /// DynamicServiceProxy SoapHeaderInspector is use to allow ATIWrapperServices which uses WCF to
    /// Inspect and Generate the old WSE authentication and security headers for the older WebService style
    /// services that need to be supported.
    /// </summary>
    public class SoapHeaderInspector : IClientMessageInspector
    {
        private MessageHeader inspectorMessageHeader;

        /// <summary>
        /// DynamicServiceProxy SoapHeaderInspector default constructor
        /// </summary>
        public SoapHeaderInspector()
        {
        }

        /// <summary>
        /// DynamicServiceProxy SoapHeaderInspector constructor with message header
        /// </summary>
        /// <param name="messageHeader">DynamicServiceProxy messageHeader</param>
        public SoapHeaderInspector(MessageHeader messageHeader)
        {
            inspectorMessageHeader = messageHeader;
        }

        /// <summary>
        /// DynamicServiceProxy AfterReceiveReply not implemented but required
        /// </summary>
        /// <param name="reply"></param>
        /// <param name="correlationState"></param>
        public void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        {
        }

        /// <summary>
        /// DynamicServiceProxy BeforeSendRequest adds headers before the request is sent
        /// </summary>
        /// <param name="request">DynamicServiceProxy request</param>
        /// <param name="channel">DynamicServiceProxy channel</param>
        /// <returns></returns>
        public object BeforeSendRequest(ref System.ServiceModel.Channels.Message request, System.ServiceModel.IClientChannel channel)
        {
            request.Headers.Add(inspectorMessageHeader);

            return request;
        }
    }


    /// <summary>
    /// DynamicServiceProxy SoapEndpointBehavior hooks up the SoapHeaderInspector to the endpoint
    /// </summary>
    public class SoapEndpointBehavior : IEndpointBehavior
    {
        private SoapHeaderInspector inspector;

        /// <summary>
        /// DynamicServiceProxy SoapEndpointBehavior default constructor
        /// </summary>
        public SoapEndpointBehavior()
        {
        }

        /// <summary>
        /// DynamicServiceProxy SoapEndpointBehavior constructor with inspector
        /// </summary>
        /// <param name="inspector">DynamicServiceProxy SoapHeaderInspector</param>
        public SoapEndpointBehavior(SoapHeaderInspector inspector)
        {
            this.inspector = inspector;
        }

        /// <summary>
        /// DynamicServiceProxy AddBindingParameters Not implemented but required
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="bindingParameters"></param>
        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
            //no-op
        }

        /// <summary>
        /// DynamicServiceProxy ApplyClientBehavior Not implemented but required
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="clientRuntime"></param>
        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.MessageInspectors.Add(inspector);
        }

        /// <summary>
        /// DynamicServiceProxy ApplyDispatchBehavior Not implemented but required
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="endpointDispatcher"></param>
        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            //no-op
        }

        /// <summary>
        /// DynamicServiceProxy Validate Not implemented but required
        /// </summary>
        /// <param name="endpoint"></param>
        public void Validate(ServiceEndpoint endpoint)
        {
            //no-op
        }
    }
}