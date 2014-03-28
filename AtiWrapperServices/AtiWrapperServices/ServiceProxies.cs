using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;
using System.Reflection;
using AtiWrapperServicesORM.OpenAccess;
using AtiWrapperServices.AtiWrapperServicesEnums;
using DynamicServiceProxyNamespace;
using System.Diagnostics;
using System.Configuration;
using System.ServiceModel.Configuration;


namespace AtiWrapperServices
{
    /// <summary>
    /// AtiWrapperServices ServiceProxies Class Builds and deploys the Service proxies for external web services
    /// </summary>
    public class ServiceProxies
    {
        /// <summary>
        /// Globals
        /// </summary>
        static private string CUSTOMSOAPHEADERSNAMESPACE = "CustomMessageHeaders.CustomSoapHeaders";
        static public Dictionary<int, ProxyInformation> TestProxies = new Dictionary<int,ProxyInformation>();
        static public Dictionary<int, ProxyInformation> ProductionProxies = new Dictionary<int, ProxyInformation>();
        static public TraceSource WrapperTrace = new TraceSource("WrapperService");
        static public System.Configuration.Configuration WrapperConfig;
        static public string DBServer;
        static public System.Collections.Specialized.NameValueCollection AppSettings;
        static public System.Configuration.ConnectionStringSettings ConnectionString;
        static public TraceSource CardTrace = new TraceSource("AtiWrapperServiceCard"); 
        static public int CardTraceFlag;
        static public AtiWrapperServicesModel DbContext = new AtiWrapperServicesModel();
        
        /// <summary>
        /// AtiWrapperServices BuildServiceProxies opens the ServiceData database and uses the data to build and publish external web service clients
        /// </summary>
        static public void BuildServiceProxies()
        {
            Trace.CorrelationManager.ActivityId = Guid.NewGuid();
            Trace.CorrelationManager.StartLogicalOperation("Building Service Proxies");
            WrapperTrace.TraceEvent(TraceEventType.Start, 1000, "Start building service proxies");


            

            //Check to see if card tracing is turned on
            if (!int.TryParse(System.Configuration.ConfigurationManager.AppSettings["CardTrace"], out CardTraceFlag))
            {
                WrapperTrace.TraceEvent(TraceEventType.Critical, 1001, "CardTrace is not set in webconfig");
                return;
            }
            else if (CardTraceFlag == 1)
            {
                Trace.CorrelationManager.ActivityId = Guid.NewGuid();
                Trace.CorrelationManager.StartLogicalOperation("Card Trace");
                CardTrace.TraceEvent(TraceEventType.Start, 200000, "Start Card Trace");
            }

            //WrapperConfig = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration(null);
            ConnectionString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["ServiceDataConnection"];

            if( ConnectionString == null)
            {
                WrapperTrace.TraceEvent(TraceEventType.Critical, 1001, "Critical Error ConnectionString is null");
                return;
            }
            else
            {
                try
                {
                    AppSettings = ConfigurationManager.AppSettings;
                    WrapperTrace.TraceEvent(TraceEventType.Information, 1001, string.Format("WrapperServiceMode is: {0}", AppSettings["WrapperServiceMode"]));
                    var elems = ConnectionString.ToString().Split(' ');
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
                                    DBServer = oElems[1];
                                    break;
                                }
                            }
                        }
                    }

                    if (DBServer == string.Empty)
                    {
                        throw new Exception("dBServer is Empty");
                    }
                }
                catch (Exception dbEx)
                {
                    WrapperTrace.TraceEvent(TraceEventType.Critical, 1002, string.Format("Critical Error DBServerName is missing: {0}", dbEx.Message));
                    return;
                }
                   
            }
            WrapperTrace.TraceEvent(TraceEventType.Information, 1005, string.Format("database server: {0}", DBServer));
            ClientSection clientSection = (ClientSection)ConfigurationManager.GetSection("system.serviceModel/client");
            string address;
            for (int i = 0; i < clientSection.Endpoints.Count; i++)
            {
                address = clientSection.Endpoints[i].Address.ToString();
                WrapperTrace.TraceEvent(TraceEventType.Information, 1010, string.Format("WrapperService Endpoints: {0}", address));
            }

            

            using (DbContext = new AtiWrapperServicesModel())
            { 
                //TODo for testing query
                var serviceMethods = (from sm in DbContext.ServiceMethods
                                      join sn in DbContext.ServiceNames on sm.ServiceName.Id equals sn.Id
                                      join su in DbContext.ServiceURLs on sn.ServiceURL.Id equals su.Id
                                      select new { ServiceName_Id = sn.Id, MethodName = sm.Name, TestUrl = su.TestURL, ProductionUrl = su.ProductionURL, ProxyServiceType = sn.ServiceType,
                                                   EndpointAddressModifierOption = sm.EndpointAddressModifierOption, XmlTypeSignature = sm.XmlTypeSignature, ServiceMethod_Id = sm.Id,
                                                   ServiceName = sn.Name, TestUseSource = su.TestUseSource, TestWSDL = su.TestWSDL, TestCode = su.TestCode, 
                                                   ProductionUseSource = su.ProductionUseSource, ProdutionWSDL = su.ProductionWSDL, ProductionCode = su.ProductionCode}).ToList();

                foreach (var proxyCreateData in serviceMethods)
                {
                    WrapperTrace.TraceEvent(TraceEventType.Information, 2000, string.Format("building service for: {0}", proxyCreateData.MethodName));
                    //var proxyCreateData = (from n in DbContext.ServiceNames
                    //                       join u in DbContext.ServiceURLs on n.ServiceURL.Id equals u.Id
                    //                       where n.Id == s.ServiceName_Id
                    //                       select new { Id = n.Id, Name = n.Name, TestUrl = u.TestURL, ProductionUrl = u.ProductionURL, ProxyServiceType = n.ServiceType}).First();

                    DynamicServiceProxyFactoryOptions options = new DynamicServiceProxyFactoryOptions();

                    if (AppSettings["LogRequests"] == "1")
                    {
                        options.Logging = true;
                    }
                    options.ServiceProxyType = (DynamicServiceProxyFactoryOptions.ServiceProxyTypes)proxyCreateData.ProxyServiceType;
                    if (options.ServiceProxyType.Equals(DynamicServiceProxyFactoryOptions.ServiceProxyTypes.Wsdl))
                    {
                        options.EndpointAddress = (DynamicServiceProxyFactoryOptions.EndpointAddressModifierOptions)proxyCreateData.EndpointAddressModifierOption;
                    }
                    string url = string.Empty;
                    if (AppSettings["WrapperServiceMode"] == "Test" || AppSettings["WrapperServiceMode"] == "Development")
                    {
                        url = proxyCreateData.TestUrl;
                        WrapperTrace.TraceEvent(TraceEventType.Information, 2060, string.Format("using url: {0}", url));
                        WrapperTrace.TraceEvent(TraceEventType.Information, 2100, string.Format("populating dictionaries for: {0}", proxyCreateData.MethodName));
                        WrapperTrace.TraceEvent(TraceEventType.Information, 2150, string.Format("Proxy: Name: {0} Mode: {1} Type: {2} Url: {3} Opt: {4}", proxyCreateData.MethodName, AppSettings["WrapperServiceMode"], proxyCreateData.XmlTypeSignature, url, options));
                        PopulateProxyDictionary(ref TestProxies, DbContext, proxyCreateData.ServiceName_Id, proxyCreateData.ServiceMethod_Id, proxyCreateData.ServiceName, proxyCreateData.MethodName,
                            proxyCreateData.XmlTypeSignature, proxyCreateData.TestUrl, options, proxyCreateData.TestUseSource, proxyCreateData.TestWSDL, proxyCreateData.TestCode);
                    }
                    else if (AppSettings["WrapperServiceMode"] == "Production")
                    {
                        WrapperTrace.TraceEvent(TraceEventType.Information, 2060, string.Format("using url: {0}", url));
                        WrapperTrace.TraceEvent(TraceEventType.Information, 2100, string.Format("populating dictionaries for: {0}", proxyCreateData.MethodName));
                        WrapperTrace.TraceEvent(TraceEventType.Information, 2160, string.Format("Proxy: Name: {0} Mode: {1} Type: {2} Url: {3} Opt: {4}", proxyCreateData.MethodName, AppSettings["WrapperServiceMode"], proxyCreateData.XmlTypeSignature, url, options));
                        PopulateProxyDictionary(ref ProductionProxies, DbContext, proxyCreateData.ServiceName_Id, proxyCreateData.ServiceMethod_Id, proxyCreateData.ServiceName, proxyCreateData.MethodName,
                            proxyCreateData.XmlTypeSignature, proxyCreateData.ProductionUrl, options, proxyCreateData.ProductionUseSource, proxyCreateData.ProdutionWSDL, proxyCreateData.ProductionCode);
                        url = proxyCreateData.ProductionUrl;
                    }

                    if (url == string.Empty)
                    {
                        WrapperTrace.TraceEvent(TraceEventType.Critical, 2050, "url is not defined");
                        return;
                    }
                }
                WrapperTrace.TraceEvent(TraceEventType.Stop, 7000, "Service Proxies Built");
                if (CardTraceFlag == 1)
                {
                    CardTrace.TraceEvent(TraceEventType.Stop, 25000, "Card Logging is enabled");
                }
                Trace.CorrelationManager.StopLogicalOperation();
            }

        }


        static internal void PopulateProxyDictionary(ref Dictionary<int, ProxyInformation> proxies,
                                            AtiWrapperServicesModel dbContext,
                                            int serviceNameId, 
                                            int serviceMethodId,
                                            string serviceName,
                                            string methodName,
                                            string xmlSignatureType,
                                            string urlName, 
                                            DynamicServiceProxyFactoryOptions options,
                                            UseSource useSource,
                                            string wsdl,
                                            string code)       
        {
            WrapperTrace.TraceEvent(TraceEventType.Information, 2151, string.Format("Proxy: Name: {0} Type: {1} Url: {2} Opt: {3}", serviceName, xmlSignatureType, urlName, options));
            DynamicServiceProxyFactory serviceFactory = null;
            DynamicServiceProxy proxy;
            if (options.ServiceProxyType.Equals(DynamicServiceProxyFactoryOptions.ServiceProxyTypes.Wsdl))
            {
                WrapperTrace.TraceEvent(TraceEventType.Information, 2152, string.Format("Proxy WSDL: Name: {0}", serviceName));
                try
                {
                    WrapperTrace.TraceEvent(TraceEventType.Information, 2160, string.Format("ProxyFactory Method Name: {0} use source: {1}", serviceName + "_" + methodName, useSource));
                    if (useSource == UseSource.Url)
                    {             
                        serviceFactory = new DynamicServiceProxyFactory(urlName + "?wsdl", options);
                    }
                    else if (useSource == UseSource.Wsdl)
                    {
                        DynamicServiceProxyWsldFile wsdlFile = new DynamicServiceProxyWsldFile(wsdl);
                        serviceFactory = new DynamicServiceProxyFactory(wsdlFile, urlName + "?wsdl", options);
                    }
                    else if (useSource == UseSource.Code)
                    {
                        DynamicServiceProxyCode codeObj = new DynamicServiceProxyCode(code);
                        serviceFactory = new DynamicServiceProxyFactory(codeObj, urlName + "?wsdl", options);
                    }
                    else
                    {
                        WrapperTrace.TraceEvent(TraceEventType.Error, 2163, string.Format("Invalid use source: {1}", serviceName + "_" + methodName, useSource));
                    }
                    
                    proxy = serviceFactory.CreateProxy(serviceName);
                }
                catch (DynamicServiceProxyException ce)
                {
                    if (ce != null && ce.Message != null && ce.Message != string.Empty)
                    {
                        WrapperTrace.TraceEvent(TraceEventType.Critical, 2153, string.Format("No Proxy for WSDL: Name: {0} ex: {1}", serviceName, ce.Message));  
                    }
                    else
                    {
                        WrapperTrace.TraceEvent(TraceEventType.Critical, 2154, string.Format("No Proxy for WSDL: Name: {0}", serviceName));   
                    }
                    WrapperTrace.TraceEvent(TraceEventType.Stop, 2155);
                    return;
                }
            }
            else // if (options.ServiceProxyType.Equals(DynamicServiceProxyFactoryOptions.ServiceProxyTypes.Xml))
            {
                List<XmlServiceParameters> inputParamters = (from n in dbContext.ServiceNames
                                           join u in dbContext.ServiceURLs on n.ServiceURL.Id equals u.Id
                                           join m in dbContext.ServiceMethods on n.Id equals m.ServiceName.Id
                                           join c in dbContext.Customers on u.Customer.Id equals c.Id
                                           join i in dbContext.InputParamaters on m.Id equals i.ServiceMethod.Id
                                           join t in dbContext.ServiceTypes on i.Id equals t.InputParamater.Id
                                           join q in dbContext.QualifiedNames on t.QualifiedName.Id equals q.Id
                                           orderby i.Order
                                           where serviceNameId == n.Id && serviceMethodId == m.Id
                                           select new XmlServiceParameters { Name = q.Name, TypeName = t.TypeName}).ToList();
                WrapperTrace.TraceEvent(TraceEventType.Information, 2152, string.Format("Proxy XML: Name: {0}", serviceName)); 
                                           
                WrapperTrace.TraceEvent(TraceEventType.Information, 2160, string.Format("ProxyFactory Method Name: {0} use source: {1}", serviceName + "_" + methodName, useSource));

                try
                {
                    serviceFactory = new DynamicServiceProxyFactory(urlName, serviceName, methodName, xmlSignatureType, options, inputParamters, Assembly.GetExecutingAssembly().Location);
                    proxy = serviceFactory.CreateProxy(serviceName + "_" + methodName);
                }
                catch (DynamicServiceProxyException ce)
                {
                    if (ce != null && ce.Message != null && ce.Message != string.Empty)
                    {
                        WrapperTrace.TraceEvent(TraceEventType.Critical, 2153, string.Format("No Proxy for WSDL: Name: {0} ex: {1}", serviceName, ce.Message));
                    }
                    else
                    {
                        WrapperTrace.TraceEvent(TraceEventType.Critical, 2154, string.Format("No Proxy for WSDL: Name: {0}", serviceName));
                    }
                    WrapperTrace.TraceEvent(TraceEventType.Stop, 2155);
                    return;
                }
            }


            ProxyInformation proxyInfo = new ProxyInformation();
            proxyInfo.proxy = proxy;
            WrapperTrace.TraceEvent(TraceEventType.Start, 2199, string.Format("Proxy for: {0} created for Url: {1} Id: {2}", serviceName, urlName, serviceNameId));


            var serviceMessageHeaders = (from h in dbContext.ServiceMessageHeaders
                                         where serviceMethodId == h.ServiceMethod_Id
                                         orderby h.Order
                                         select new { Id = h.Id, TypeName = h.TypeName, AssemblyName = h.AssemblyName }).ToList();

            foreach (var shm in serviceMessageHeaders)
            {
                WrapperTrace.TraceEvent(TraceEventType.Start, 3000, string.Format("Added MessageHeader: {0} on Proxy: {1}", shm.TypeName, serviceName));
                ConstructorInfo ctor = Type.GetType(CUSTOMSOAPHEADERSNAMESPACE + "." + shm.TypeName + "," + shm.AssemblyName).GetConstructor(new Type[] { });
                MessageHeader messageHeader = ctor.Invoke(null) as MessageHeader;
                SoapEndpointBehavior soapEndpointBehavior = new SoapEndpointBehavior(new SoapHeaderInspector(messageHeader));
                ((ServiceEndpoint)proxy.GetProperty("Endpoint")).EndpointBehaviors.Add(soapEndpointBehavior);
                proxyInfo.proxyHeaders.Add(shm.Id, messageHeader);
                
            }
            proxies.Add(serviceMethodId, proxyInfo);
        }
        

        /// <summary>
        /// ServiceProxies AddNewServiceProxy adds a new service proxy to the running Wrapper Service without requiring an IIS
        /// stop and restart.
        /// </summary>
        /// <param name="customerId">AtiWrapperServices customerId</param>
        /// <param name="serviceMethodName">AtiWrapperServices serviceMethodName</param>
        static public void AddNewServiceProxy(int customerId, string serviceMethodName)
        {
            using (DbContext = new AtiWrapperServicesModel())
            {

                var proxyCreateData = (from n in DbContext.ServiceNames
                                       join u in DbContext.ServiceURLs on n.ServiceURL.Id equals u.Id
                                       join c in DbContext.Customers on u.Customer.Id equals c.Id
                                       join m in DbContext.ServiceMethods on n.Id equals m.ServiceName.Id
                                       where m.Name == serviceMethodName && c.Id == customerId
                                       select new
                                       {
                                           Id = n.Id,
                                           Name = n.Name,
                                           TestUrl = u.TestURL,
                                           ProductionUrl = u.ProductionURL,
                                           MethodId = m.Id,
                                           EndpointModifier = m.EndpointAddressModifierOption,
                                           XmlSignatureType = m.XmlTypeSignature,
                                           ProxyServiceType = n.ServiceType,
                                           TestUseSource = u.TestUseSource,
                                           TestWSDL = u.TestWSDL,
                                           TestCode = u.TestCode,
                                           ProductionUseSource = u.ProductionUseSource,
                                           ProdutionWSDL = u.ProductionWSDL,
                                           ProductionCode = u.ProductionCode
                                       }).FirstOrDefault();
                if (proxyCreateData != null)
                {
                    DynamicServiceProxyFactoryOptions options = new DynamicServiceProxyFactoryOptions();
                    options.ServiceProxyType = (DynamicServiceProxyFactoryOptions.ServiceProxyTypes)proxyCreateData.ProxyServiceType;
                    if (options.ServiceProxyType.Equals(DynamicServiceProxyFactoryOptions.ServiceProxyTypes.Wsdl))
                    {
                        options.EndpointAddress = (DynamicServiceProxyFactoryOptions.EndpointAddressModifierOptions)proxyCreateData.EndpointModifier;
                    }

                    PopulateProxyDictionary(ref TestProxies, DbContext, proxyCreateData.Id, proxyCreateData.MethodId, proxyCreateData.Name, serviceMethodName, proxyCreateData.XmlSignatureType, proxyCreateData.TestUrl, options, proxyCreateData.TestUseSource, proxyCreateData.TestWSDL, proxyCreateData.TestCode);
                    PopulateProxyDictionary(ref ProductionProxies, DbContext, proxyCreateData.Id, proxyCreateData.MethodId, proxyCreateData.Name, serviceMethodName, proxyCreateData.XmlSignatureType, proxyCreateData.ProductionUrl, options, proxyCreateData.ProductionUseSource, proxyCreateData.ProdutionWSDL, proxyCreateData.ProductionCode);
                }
            }
        }
    }
}