using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Configuration;
using System.Collections;
using System.Collections.Specialized;
using Ati.ServiceHost.Composition;
using Ati.ServiceHost.Hosting;
using System.Diagnostics;
using System.ServiceModel.Configuration;
using System.IO;


namespace Ati.ServiceHost.Web
{
    /// <summary>
    /// Defines a service host factory for dynamic web services.
    /// </summary>
    public class WebServiceHostFactory : ServiceHostFactory
    {
        static public TraceSource _trace = new TraceSource("AtiServiceBroker");
        static public TraceSource _cardTrace = new TraceSource("AtiCardTrace");
        static public int cardTrace;
        static public string dBServer;
        static public NameValueCollection appSettings;
        #region Fields
        private static CompositionContainer _container;
        private static readonly object sync = new object();
        #endregion

        #region Properties
        /// <summary>
        /// Gets the composition container.
        /// </summary>
        public CompositionContainer Container
        {
            get
            {
                lock (sync)
                {
                    return _container;
                }
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Creates the service host that will handle web service requests.
        /// </summary>
        /// <param name="constructorString">The constructor string used to select the service.</param>
        /// <param name="baseAddresses">The set of base address for the service.</param>
        /// <returns>An instance of <see cref="ServiceHostBase"/> for the service.</returns>
        public override ServiceHostBase CreateServiceHost(string constructorString, Uri[] baseAddresses)
        {
            WebServiceHostFactory._trace.TraceEvent(TraceEventType.Start, 2000, "CreateServiceHost");
            
            if (cardTrace == 1)
            {
                Trace.CorrelationManager.ActivityId = Guid.NewGuid();
                Trace.CorrelationManager.StartLogicalOperation("Start Card Trace");
                _cardTrace.TraceEvent(TraceEventType.Start, 200000, "Start Card Trace");
                _cardTrace.TraceEvent(TraceEventType.Information, 200010, "Using Card Trace Log");
                _cardTrace.TraceEvent(TraceEventType.Stop, 200100, "Stop card trace log");
            }

            _trace.TraceEvent(TraceEventType.Information, 2005, string.Format("CardTrace Value: {0}", cardTrace.ToString()));

            

            var meta = Container
                .GetExports<IHostedService, IHostedServiceMetadata>()
                .Where(e => e.Metadata.Name.Equals(constructorString, StringComparison.OrdinalIgnoreCase))
                .Select(e => e.Metadata)
                .SingleOrDefault();

            if (meta == null)
            {
                WebServiceHostFactory._trace.TraceEvent(TraceEventType.Error, 2999, "CreateServiceHost No Services");
                return null;
            }

            WebServiceHostFactory._trace.TraceEvent(TraceEventType.Information, 2500, string.Format("CreateServiceHost Service Name: {0}", meta.Name));

            var host = new ExportServiceHost(meta, baseAddresses);
            host.Description.Behaviors.Add(new ExportServiceBehavior(Container, meta.Name));

            var contracts = meta.ServiceType
                .GetInterfaces()
                .Where(t => t.IsDefined(typeof(ServiceContractAttribute), true));

            EnsureHttpBinding(host, contracts);

            WebServiceHostFactory._trace.TraceEvent(TraceEventType.Stop, 2999, "CreateServiceHost Complete");
            
            return host;
        }

        /// <summary>
        /// Ensures that the Http binding has been created.
        /// </summary>
        /// <param name="host">The Http binding.</param>
        /// <param name="contracts">The set of contracts</param>
        private static void EnsureHttpBinding(ExportServiceHost host, IEnumerable<Type> contracts)
        {
            var binding = new BasicHttpBinding();

            host.Description.Endpoints.Clear();

            foreach (var contract in contracts)
                host.AddServiceEndpoint(contract.FullName, binding, "");
        }

        /// <summary>
        /// Sets the composition container factory.
        /// </summary>
        /// <param name="factory">The container factory.</param>
        public static void SetCompositionContainerFactory(ICompositionContainerFactory factory)
        {
            if (factory == null)
                throw new ArgumentNullException("factory");

            lock (sync)
            {
                var provider = new ServiceHostExportProvider();
                _container = factory.CreateCompositionContainer(provider);

                provider.SourceContainer = _container;
            }
        }

        /// <summary>
        /// Sets the composition container factory.
        /// </summary>
        /// <param name="factory">The container factory.</param>
        public static void SetCompositionContainerFactory(Func<ExportProvider[], CompositionContainer> factory)
        {
            SetCompositionContainerFactory(new DelegateCompositionContainerFactory(factory));
        }
        #endregion
    }
}
