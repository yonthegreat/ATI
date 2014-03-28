using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Security;
using System.Web.SessionState;
using System.ComponentModel.Composition.Hosting;
using Ati.ServiceHost.Web;
using System.Diagnostics;
using System.Configuration;
using System.ServiceModel.Configuration;
using System.Collections.Specialized;

namespace Ati.ServiceBroker.WebService
{
    public class Global : System.Web.HttpApplication
    {

        public static string dBServer;
        public static int cardTrace;
        
        protected void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            Trace.CorrelationManager.ActivityId = Guid.NewGuid();
            Trace.CorrelationManager.StartLogicalOperation("Starting AtiServiceBroker");
            WebServiceHostFactory._trace.TraceEvent(TraceEventType.Start, 1000, "Starting AtiServiceBroker");

            
            ClientSection clientSection = (ClientSection)ConfigurationManager.GetSection("system.serviceModel/client");
            string address;
            for (int i = 0; i < clientSection.Endpoints.Count; i++)
            {
                address = clientSection.Endpoints[i].Address.ToString();
                WebServiceHostFactory._trace.TraceEvent(TraceEventType.Information, 2110, string.Format("WrapperService Endpoints: {0}", address));
            }

            string connectionString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["CardEnrollmentService.Properties.Settings.ServiceBrokerDBConnectionString"].ToString();

            if (connectionString == null)
            {
                WebServiceHostFactory._trace.TraceEvent(TraceEventType.Critical, 2120, "Critical Error ConnectionString is null");
                return;
            }
            else
            {
                try
                {
                    WebServiceHostFactory.appSettings = ConfigurationManager.AppSettings;
                    WebServiceHostFactory._trace.TraceEvent(TraceEventType.Information, 2120, string.Format("EnrollmentMode is: {0}", WebServiceHostFactory.appSettings["EnrollmentMode"]));
                    var elems = connectionString.ToString().Split(' ');
                    if (elems == null)
                    {
                        return;
                    }
                    foreach (string elem in elems)
                    {
                        if (elem.StartsWith("source="))
                        {
                            var nElems = elem.Split(';');
                            if (nElems != null)
                            {
                                var oElems = nElems[0].Split('=');
                                if (oElems != null)
                                {
                                    dBServer = oElems[1];
                                    break;
                                }
                            }
                        }
                    }

                    if (dBServer == string.Empty)
                    {
                        throw new Exception("dBServer is Empty");
                    }

                    string folder = System.Web.HttpContext.Current != null ?
                        System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_data") :
                        System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);


                    string value = WebServiceHostFactory.appSettings["CardTrace"];

                    bool testCardTrace = int.TryParse(WebServiceHostFactory.appSettings["CardTrace"], out cardTrace);
                    //Check to see if card tracing is turned on
                    if (!testCardTrace)
                    {
                        WebServiceHostFactory._trace.TraceEvent(TraceEventType.Critical, 2001, "CardTrace is not set in webconfig");
                        return;
                    }

                    // set the card trace value in the factory
                    WebServiceHostFactory.cardTrace = cardTrace;

                }
                catch (Exception dbEx)
                {
                    WebServiceHostFactory._trace.TraceEvent(TraceEventType.Critical, 2130, string.Format("Critical Error DBServerName is missing: {0}", dbEx.Message));
                    return;
                }

            }
            WebServiceHostFactory._trace.TraceEvent(TraceEventType.Information, 2150, string.Format("database server: {0}", dBServer));
            
            HostingEnvironment.RegisterVirtualPathProvider(new ServiceVirtualPathProvider());

            var catalog = new DirectoryCatalog(HttpRuntime.BinDirectory);
            WebServiceHostFactory.SetCompositionContainerFactory(ep => new CompositionContainer(catalog, ep));

            clientSection = (ClientSection)ConfigurationManager.GetSection("system.serviceModel/client");

            for (int i = 0; i < clientSection.Endpoints.Count; i++)
            {
                address = clientSection.Endpoints[i].Address.ToString();
                WebServiceHostFactory._trace.TraceEvent(TraceEventType.Information, 1010, string.Format("ServiceBroker Endpoints: {0}", address));
            }

            WebServiceHostFactory._trace.TraceEvent(TraceEventType.Stop, 7000, "AtiServiceBroker Started");
            
            Trace.CorrelationManager.StopLogicalOperation();
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}