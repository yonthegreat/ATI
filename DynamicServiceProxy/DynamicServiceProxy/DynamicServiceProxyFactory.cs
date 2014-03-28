using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Data;
using System.Data.Design;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Reflection.Emit;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using WsdlNS = System.Web.Services.Description;
using System.Web.Services.Discovery;
using System.Linq;
using Microsoft.CSharp;
using System.Diagnostics;
using System.Configuration;
using System.Xml.XPath;

namespace DynamicServiceProxyNamespace
{
    /// <summary>
    /// Dynamic Service Proxy Factory creates the proxies. This is a Factory Pattern 
    /// </summary>
    public class DynamicServiceProxyFactory
    {
        private string wsdlUri;
        private string xmlServiceUri;
        private string serviceName; 
        private string serviceMethod;
        private string xmlSignatureType;
        private string proxyName;
        private TraceSource _proxyTrace = new TraceSource("WrapperService");
        private DynamicServiceProxyFactoryOptions options;
        private List<XmlServiceParameters> xmlServiceParameters;

        private CodeCompileUnit codeCompileUnit;
        private CodeDomProvider codeDomProvider;
        private ServiceContractGenerator contractGenerator;

        private Collection<MetadataSection> metadataCollection;
        private IEnumerable<Binding> bindings;
        private IEnumerable<ContractDescription> contracts;
        private ServiceEndpointCollection endpoints;
        private IEnumerable<MetadataConversionError> importWarnings;
        private IEnumerable<MetadataConversionError> codegenWarnings;
        private IEnumerable<CompilerError> compilerWarnings;

        private Assembly proxyAssembly;
        private string proxyCode;
        private string proxyWsdl;

        //static public System.Diagnostics.TraceSource traceRequest;

        static public TraceSource _trace = new TraceSource("WrapperService");

        static public string wrapperAssemblyLocation;
        static public string WSDLLOCATION = @"C:\Temp\wsldFile";

        /// <summary>
        /// get the contract descriptions
        /// </summary>
        public IEnumerable<ContractDescription> GetContracts
        {
            get { return contracts; }
        }

        /// <summary>
        /// Dynamic Service Proxy Factory for XML based proxy services constructor
        /// </summary>
        /// <param name="xmlServiceUri">Dynamic Service Proxy Uri</param>
        /// <param name="serviceName">Dynamic Service Proxy Name</param>
        /// <param name="serviceMethod">Dynamic Service Proxy method name</param>
        /// <param name="xmlSignatureType">Dynamic Service Proxy method Type</param>
        /// <param name="options">See DynamicServiceProxyFactoryOptions</param>
        /// <param name="xmlServiceParameters">Dynamic Service Proxy list of XmlServiceParameters (Name and Type of parameters)</param>
        public DynamicServiceProxyFactory(string xmlServiceUri, string serviceName, string serviceMethod, string xmlSignatureType, DynamicServiceProxyFactoryOptions options, List<XmlServiceParameters> xmlServiceParameters, string wrapperAssemblyName)
        {

            //traceRequest = _traceRequest;
            wrapperAssemblyLocation = wrapperAssemblyName;
            Trace.CorrelationManager.ActivityId = Guid.NewGuid();
            Trace.CorrelationManager.StartLogicalOperation("Building Dynamic Service Proxies");
            _trace.TraceEvent(TraceEventType.Start, 100000, "Start building dynamic service proxies");
            if (_trace == null)
            {
                _trace.TraceEvent(TraceEventType.Start, 100001, "_proxyTrace is null");
            }
            if (xmlServiceUri == null)
            {
                _trace.TraceEvent(TraceEventType.Error, 100100, "xmlServiceUri is null");
                throw new ArgumentNullException("xmlServiceUri");
            }
            if (serviceName == null)
            {
                _trace.TraceEvent(TraceEventType.Error, 100200, "serviceName is null");
                throw new ArgumentNullException("serviceName");
            }
            if (serviceMethod == null)
            {
                _trace.TraceEvent(TraceEventType.Error, 100300, "serviceMethod is null");
                throw new ArgumentNullException("serviceMethod");
            }
            if (xmlSignatureType == null)
            {
                _trace.TraceEvent(TraceEventType.Error, 100400, "xmlWrapperName is null");
                throw new ArgumentNullException("xmlWrapperName");
            }
            if (options == null)
            {
                _trace.TraceEvent(TraceEventType.Error, 100500, "options are null");
                throw new ArgumentNullException("options");
            }

            this.options = options;
            this.xmlServiceUri = xmlServiceUri;
            this.xmlSignatureType = xmlSignatureType;
            this.xmlServiceParameters = xmlServiceParameters;
            this.serviceName = serviceName;
            this.serviceMethod = serviceMethod;
            
            

            proxyName = this.serviceName + "_" + this.serviceMethod;

            _trace.TraceEvent(TraceEventType.Information, 100001, string.Format("DynamicServiceProxy for: {0}", proxyName));
            //if (options.Logging)
            //{
            //    Trace.CorrelationManager.ActivityId = Guid.NewGuid(); 
            //    Trace.CorrelationManager.StartLogicalOperation("Log Service Requests");
            //    _traceRequest.TraceEvent(TraceEventType.Start, 1000, "Start Logging requests");
                
            //}

            proxyCode = CreateProxyForTypes(this.proxyName, this.xmlSignatureType, this.xmlServiceUri, this.xmlServiceParameters, options.Logging);

            CompilerResults compilerResults = CompileSrcipt(proxyCode, options.Logging);

            //if (options.Logging)
            //{
            //    _traceRequest.TraceEvent(TraceEventType.Stop, 1000);
            //    Trace.CorrelationManager.StopLogicalOperation();
            //}

            _trace.TraceEvent(TraceEventType.Information, 101099, proxyCode);

            if (compilerResults.Errors.HasErrors)
            {
                _trace.TraceEvent(TraceEventType.Error, 101000, string.Format("Compiler Error for {0}.{1}", serviceName, serviceMethod));
                
                int i = 0;
                foreach(CompilerError err in compilerResults.Errors)
                {
                    i++;
                    _trace.TraceEvent(TraceEventType.Error, (101000 + i), string.Format("ErrorNo.LineNo  Warning Text {0}.{1} {2} {3}", err.ErrorNumber, err.Line, err.IsWarning, err.ErrorText));
                }

                _trace.TraceEvent(TraceEventType.Information, 101099, proxyCode);
                

                throw new InvalidOperationException("Expression has a syntax error.");

            }
            if (compilerResults.CompiledAssembly != null)
            {
                proxyAssembly = compilerResults.CompiledAssembly;
            }
            else
            {
                _trace.TraceEvent(TraceEventType.Error, 101100, string.Format("Compiled Assembly Error: Native Return {0} Output {1} Path {2}", compilerResults.NativeCompilerReturnValue, compilerResults.Output, compilerResults.PathToAssembly));
                throw new InvalidOperationException("CompiledAssembly is null.");
            }
        }
        /// <summary>
        /// Dynamic Service Proxy Factory for WSDL based proxy services constructor
        /// </summary>
        /// <param name="wsdlUri">Dynamic Service Proxy URL for WSDL</param>
        /// <param name="options">See DynamicServiceProxyFactoryOptions</param>
       
        public DynamicServiceProxyFactory(string wsdlUri, DynamicServiceProxyFactoryOptions options)
        {
            if (wsdlUri == null)
            {
                _trace.TraceEvent(TraceEventType.Error, 102000, "wsdlURI is null");
                throw new ArgumentNullException("wsdlUri");
            }
            if (options == null)
            {
                _trace.TraceEvent(TraceEventType.Error, 102100, "options are null");
                throw new ArgumentNullException("options");
            }

            this.wsdlUri = wsdlUri;
            this.options = options;

            DownloadMetadata();
            ImportMetadata();
            CreateProxy();
            WriteCode();
            try
            {
                CompileProxy();
            }
            catch (DynamicServiceProxyException ce)
            {
                if (ce != null && ce.Message != null && ce.Message != string.Empty)
                {
                    _trace.TraceEvent(TraceEventType.Error, 102201, string.Format("CompileProxy Error Exiting ex: {0}", ce.Message));
                }
                else
                {
                    _trace.TraceEvent(TraceEventType.Error, 102202, "CompileProxy Error Exiting");
                }
                throw ce;
            }

        }

        /// <summary>
        /// Dynamic Service Proxy Factory for WSDL based proxy services constructor
        /// </summary>
        /// <param name="wsdl">File where the stored wsdl has been written</param>
        /// <param name="options">See DynamicServiceProxyFactoryOptions</param>

        public DynamicServiceProxyFactory(DynamicServiceProxyWsldFile wsdl, string wsdlUri, DynamicServiceProxyFactoryOptions options)
        {
            if (wsdl == null)
            {
                _trace.TraceEvent(TraceEventType.Error, 202000, "wsdl is null");
                throw new ArgumentNullException("wsdl File");
            }
            if (options == null)
            {
                _trace.TraceEvent(TraceEventType.Error, 202100, "options are null");
                throw new ArgumentNullException("options");
            }
            if (wsdlUri == null)
            {
                _trace.TraceEvent(TraceEventType.Error, 102000, "wsdlURI is null");
                throw new ArgumentNullException("wsdlUri");
            }
            
            this.wsdlUri = wsdlUri;
            this.options = options;

            LoadMetadata(wsdl);
            ImportMetadata();
            CreateProxy();
            WriteCode();
            try
            {
                CompileProxy();
            }
            catch (DynamicServiceProxyException ce)
            {
                if (ce != null && ce.Message != null && ce.Message != string.Empty)
                {
                    _trace.TraceEvent(TraceEventType.Error, 202201, string.Format("CompileProxy Error Exiting ex: {0}", ce.Message));
                }
                else
                {
                    _trace.TraceEvent(TraceEventType.Error, 202202, "CompileProxy Error Exiting");
                }
                throw ce;
            }

        }

        public DynamicServiceProxyFactory(DynamicServiceProxyCode code, string wsdlUri, DynamicServiceProxyFactoryOptions options)
        {

            if (code == null)
            {
                _trace.TraceEvent(TraceEventType.Error, 302000, "wsdl is null");
                throw new ArgumentNullException("wsdl File");
            }
            if (options == null)
            {
                _trace.TraceEvent(TraceEventType.Error, 302100, "options are null");
                throw new ArgumentNullException("options");
            }
            if (wsdlUri == null)
            {
                _trace.TraceEvent(TraceEventType.Error, 102000, "wsdlURI is null");
                throw new ArgumentNullException("wsdlUri");
            }
            
            this.wsdlUri = wsdlUri;
            this.options = options;

            if (string.IsNullOrEmpty(code.Code))
            {
                _trace.TraceEvent(TraceEventType.Error, 302200, "wsdl Code is null");
                throw new ArgumentNullException("wsdl Code");
            }

            this.proxyCode = code.Code;

            // use the modified proxy code, if code modifier is set.
            if (this.options.CodeModifier != null)
                this.proxyCode = this.options.CodeModifier(this.proxyCode);

            // Still need to get the metadata to set the end points and other service information
            DownloadMetadata();
            ImportMetadata();
            try
            {
                CompileProxy();
            }
            catch (DynamicServiceProxyException ce)
            {
                if (ce != null && ce.Message != null && ce.Message != string.Empty)
                {
                    _trace.TraceEvent(TraceEventType.Error, 302201, string.Format("CompileProxy Error Exiting ex: {0}", ce.Message));
                }
                else
                {
                    _trace.TraceEvent(TraceEventType.Error, 302202, "CompileProxy Error Exiting");
                }
                throw ce;
            }
        }

        /// <summary>
        /// Dynamic Service Proxy Factory for WSDL based proxy services constructor overload
        /// </summary>
        /// <param name="wsdlUri">Dynamic Service Proxy URL for WSDL</param>
        
        public DynamicServiceProxyFactory(string wsdlUri)
            : this(wsdlUri, new DynamicServiceProxyFactoryOptions())
        {
        }

        /// <summary>
        /// Dynamic Service Proxy Factory WSDL metadata download
        /// </summary>
        private void DownloadMetadata()
        {
            EndpointAddress epr = new EndpointAddress(this.wsdlUri);

            DiscoveryClientProtocol disco = new DiscoveryClientProtocol();
            disco.AllowAutoRedirect = true;
            disco.UseDefaultCredentials = true;
            disco.DiscoverAny(this.wsdlUri);
            disco.ResolveAll();

            Collection<MetadataSection> results = new Collection<MetadataSection>();
            foreach (object document in disco.Documents.Values)
            {
                AddDocumentToResults(document, results);
            }
            this.metadataCollection = results;
        }

        private void LoadMetadata(DynamicServiceProxyWsldFile wsdl)
        {
            DiscoveryClientProtocol disco = new DiscoveryClientProtocol();
            disco.AllowAutoRedirect = true;
            disco.UseDefaultCredentials = true;
            disco.ReadAll(DynamicServiceProxyWsldFile.FileName);
        }

        /// <summary>
        /// Dynamic Service Proxy Factory WSDL service description parsing to results
        /// </summary>
        /// <param name="document">Dynamic Service Proxy Factory WsdlNS.ServiceDescription</param>
        /// <param name="results">Dynamic Service Proxy Factory Collection<MetadataSection> results</param>
        void AddDocumentToResults(object document, Collection<MetadataSection> results)
        {
            WsdlNS.ServiceDescription wsdl = document as WsdlNS.ServiceDescription;
            XmlSchema schema = document as XmlSchema;
            XmlElement xmlDoc = document as XmlElement;

            if (wsdl != null)
            {
                if (options.EndpointAddress == DynamicServiceProxyFactoryOptions.EndpointAddressModifierOptions.MakeSameAsUrl)
                {
                    foreach (System.Web.Services.Description.Service s in wsdl.Services)
                    {
                        foreach (System.Web.Services.Description.Port p in s.Ports)
                        {
                            foreach (System.Web.Services.Description.ServiceDescriptionFormatExtension a in p.Extensions)
                            {
                                if (a is System.Web.Services.Description.SoapAddressBinding)
                                {
                                    System.Web.Services.Description.SoapAddressBinding b = (System.Web.Services.Description.SoapAddressBinding)a;
                                    b.Location = wsdlUri.Split('?')[0];
                                }
                            }
                        }
                    }
                }
                results.Add(MetadataSection.CreateFromServiceDescription(wsdl));
            }
            else if (schema != null)
            {
                results.Add(MetadataSection.CreateFromSchema(schema));
            }
            else if (xmlDoc != null && xmlDoc.LocalName == "Policy")
            {
                results.Add(MetadataSection.CreateFromPolicy(xmlDoc, null));
            }
            else
            {
                MetadataSection mexDoc = new MetadataSection();
                mexDoc.Metadata = document;
                results.Add(mexDoc);
            }
        }

        /// <summary>
        /// Dynamic Service Proxy Factory WSDL Metadata Import
        /// </summary>
        private void ImportMetadata()
        {
            this.codeCompileUnit = new CodeCompileUnit();
            CreateCodeDomProvider();

            WsdlImporter importer = new WsdlImporter(new MetadataSet(metadataCollection));
            AddStateForDataContractSerializerImport(importer);
            AddStateForXmlSerializerImport(importer);

            this.bindings = importer.ImportAllBindings();
            this.contracts = importer.ImportAllContracts();
            this.endpoints = importer.ImportAllEndpoints();

            this.importWarnings = importer.Errors;

            bool success = true;
            if (this.importWarnings != null)
            {
                foreach (MetadataConversionError error in this.importWarnings)
                {
                    if (!error.IsWarning)
                    {
                        success = false;
                        break;
                    }
                }
                // ASMX files may have http bindings that cause the imprter to error. 
                // Check to see if any bindings were sucessfully imported and try those.
                List<Binding> bindings = this.bindings.ToList();
                if (!success && bindings.Count > 0)
                {
                    List<ContractDescription> bindingContracts = new List<ContractDescription>();
                    foreach (Binding b in bindings)
                    {
                        foreach(ServiceEndpoint e in this.endpoints)
                        {
                            if (b.Name.Equals(e.Name))
                            {
                                bindingContracts.Add(e.Contract);
                                break;
                            }
                        }

                    }
                    if (bindingContracts.Count > 0)
                    {
                        this.contracts = bindingContracts.Distinct();
                        success = true;
                    }
                }
            }

            if (!success)
            {
                _proxyTrace.TraceEvent(TraceEventType.Error, 106000, string.Format("ImportMetadata Error for {0}.{1}: {2}", serviceName, serviceMethod, this.importWarnings));
                DynamicServiceProxyException exception = new DynamicServiceProxyException(
                    Constants.ErrorMessages.ImportError);
                exception.MetadataImportErrors = this.importWarnings;
                throw exception;
            }
        }


        /// <summary>
        /// Dynamic Service Proxy Factory WSDL XMLSerializer configuration
        /// </summary>
        /// <param name="importer">Dynamic Service Proxy Factory WsdlImporter</param>
        void AddStateForXmlSerializerImport(WsdlImporter importer)
        {
            XmlSerializerImportOptions importOptions =
                new XmlSerializerImportOptions(this.codeCompileUnit);
            importOptions.CodeProvider = this.codeDomProvider;

            importOptions.WebReferenceOptions = new WsdlNS.WebReferenceOptions();
            importOptions.WebReferenceOptions.CodeGenerationOptions =
                CodeGenerationOptions.GenerateProperties |
                CodeGenerationOptions.GenerateOrder;

            importOptions.WebReferenceOptions.SchemaImporterExtensions.Add(
                typeof(TypedDataSetSchemaImporterExtension).AssemblyQualifiedName);
            importOptions.WebReferenceOptions.SchemaImporterExtensions.Add(
                typeof(DataSetSchemaImporterExtension).AssemblyQualifiedName);

            importer.State.Add(typeof(XmlSerializerImportOptions), importOptions);
        }

        /// <summary>
        /// Dynamic Service Proxy Factory WSDL DataContract configuration
        /// </summary>
        /// <param name="importer">Dynamic Service Proxy Factory WsdlImporter</param>
        void AddStateForDataContractSerializerImport(WsdlImporter importer)
        {
            XsdDataContractImporter xsdDataContractImporter =
                new XsdDataContractImporter(this.codeCompileUnit);
            xsdDataContractImporter.Options = new ImportOptions();
            xsdDataContractImporter.Options.ImportXmlType =
                (this.options.FormatMode ==
                    DynamicServiceProxyFactoryOptions.FormatModeOptions.DataContractSerializer);

            xsdDataContractImporter.Options.CodeProvider = this.codeDomProvider;
            importer.State.Add(typeof(XsdDataContractImporter),
                    xsdDataContractImporter);

            foreach (IWsdlImportExtension importExtension in importer.WsdlImportExtensions)
            {
                DataContractSerializerMessageContractImporter dcConverter =
                    importExtension as DataContractSerializerMessageContractImporter;

                if (dcConverter != null)
                {
                    if (this.options.FormatMode ==
                        DynamicServiceProxyFactoryOptions.FormatModeOptions.XmlSerializer)
                        dcConverter.Enabled = false;
                    else
                        dcConverter.Enabled = true;
                }

            }
        }

        /// <summary>
        /// Dynamic Service Proxy Factory Proxy constructor
        /// </summary>
        private void CreateProxy()
        {
            
            CreateServiceContractGenerator();
            StringBuilder wsdl = new StringBuilder();

            foreach (ContractDescription contract in this.contracts)
            {
                this.contractGenerator.GenerateServiceContractType(contract);
                wsdl.Append(contract);
            }

            bool success = true;
            this.codegenWarnings = this.contractGenerator.Errors;
            if (this.codegenWarnings != null)
            {
                foreach (MetadataConversionError error in this.codegenWarnings)
                {
                    if (!error.IsWarning)
                    {
                        success = false;
                        break;
                    }
                }
            }

            if (!success)
            {

                _proxyTrace.TraceEvent(TraceEventType.Error, 103000, string.Format("Code Generation Error for {0}.{1}: {2}", serviceName, serviceMethod, codegenWarnings));
                DynamicServiceProxyException exception = new DynamicServiceProxyException(
                 Constants.ErrorMessages.CodeGenerationError);
                exception.CodeGenerationErrors = this.codegenWarnings;
                throw exception;
            }
            else
            {
                proxyWsdl = wsdl.ToString();
            }
        }

        /// <summary>
        /// Dynamic Service Proxy Factory Compiler
        /// </summary>
        private void CompileProxy()
        {
            // reference the required assemblies with the correct path.
            CompilerParameters compilerParams = new CompilerParameters();

            AddAssemblyReference(
                typeof(System.ServiceModel.ServiceContractAttribute).Assembly,
                compilerParams.ReferencedAssemblies);

            AddAssemblyReference(
                typeof(System.Web.Services.Description.ServiceDescription).Assembly,
                compilerParams.ReferencedAssemblies);

            AddAssemblyReference(
                typeof(System.Runtime.Serialization.DataContractAttribute).Assembly,
                compilerParams.ReferencedAssemblies);

            AddAssemblyReference(typeof(System.Xml.XmlElement).Assembly,
                compilerParams.ReferencedAssemblies);

            AddAssemblyReference(typeof(System.Uri).Assembly,
                compilerParams.ReferencedAssemblies);

            AddAssemblyReference(typeof(System.Data.DataSet).Assembly,
                compilerParams.ReferencedAssemblies);

            CompilerResults results =
                this.codeDomProvider.CompileAssemblyFromSource(
                    compilerParams,
                    this.proxyCode);

            if ((results.Errors != null) && (results.Errors.HasErrors))
            {
                foreach (CompilerError error in results.Errors)
                {
                    if (error != null && error.ToString() != string.Empty && error.ErrorNumber != null && error.ErrorText != null && error.FileName != null)
                    {
                        _proxyTrace.TraceEvent(TraceEventType.Error, 103010, string.Format("CompileProxy Error:  ErrNo {0} Text: {1} File: {2} IsWarn: {3} Line: {4}", error.ErrorNumber, error.ErrorText, error.FileName, error.IsWarning.ToString(), error.Line.ToString()));
                    }
                    else
                    {
                        _proxyTrace.TraceEvent(TraceEventType.Error, 103011,string.Format("CompileProxy Unknown Error: for service {0} method {1}", serviceName, serviceMethod));
                    }
                }
                DynamicServiceProxyException exception = new DynamicServiceProxyException(
                    Constants.ErrorMessages.CompilationError);
                exception.CompilationErrors = ToEnumerable(results.Errors);

                throw exception;
            }

            this.compilerWarnings = ToEnumerable(results.Errors);
            this.proxyAssembly = Assembly.LoadFile(results.PathToAssembly);
        }

        /// <summary>
        /// Dynamic Service Proxy Factory Code Generator
        /// </summary>
        private void WriteCode()
        {
            using (StringWriter writer = new StringWriter())
            {
                CodeGeneratorOptions codeGenOptions = new CodeGeneratorOptions();
                codeGenOptions.BracingStyle = "C";
                this.codeDomProvider.GenerateCodeFromCompileUnit(
                        this.codeCompileUnit, writer, codeGenOptions);
                writer.Flush();
                this.proxyCode = writer.ToString();
            }

            // use the modified proxy code, if code modifier is set.
            if (this.options.CodeModifier != null)
                this.proxyCode = this.options.CodeModifier(this.proxyCode);
        }

        /// <summary>
        /// Dynamic Service Proxy Factory Assembly reference Adder
        /// </summary>
        /// <param name="referencedAssembly">Dynamic Service Proxy Factory referenced Assembly</param>
        /// <param name="refAssemblies">Dynamic Service Proxy Factory referenced Assembly references</param>
        void AddAssemblyReference(Assembly referencedAssembly,
            StringCollection refAssemblies)
        {
            string path = Path.GetFullPath(referencedAssembly.Location);
            string name = Path.GetFileName(path);
            if (!(refAssemblies.Contains(name) ||
                  refAssemblies.Contains(path)))
            {
                refAssemblies.Add(path);
            }
        }

        /// <summary>
        /// Dynamic Service Proxy Factory Service Endpoint Getter
        /// </summary>
        /// <param name="contractName">Dynamic Service Proxy Factory Name of Service Contract</param>
        /// <returns>ServiceEndpoint</returns>
        public ServiceEndpoint GetEndpoint(string contractName)
        {
            return GetEndpoint(contractName, null);
        }

        /// <summary>
        /// Dynamic Service Proxy Factory Service Endpoint Getter with Namespace
        /// </summary>
        /// <param name="contractName">Dynamic Service Proxy Factory Name of Service Contract</param>
        /// <param name="contractNamespace">Dynamic Service Proxy Factory Name of Service Contract Namespace</param>
        /// <returns>ServiceEndpoint</returns>
        public ServiceEndpoint GetEndpoint(string contractName,
                string contractNamespace)
        {
            ServiceEndpoint matchingEndpoint = null;

            foreach (ServiceEndpoint endpoint in Endpoints)
            {
                if (endpoint.Contract.Name.Equals(contractName) &&
                    (contractNamespace == null || endpoint.Contract.Namespace.Equals(contractNamespace)))
                {
                    matchingEndpoint = endpoint;
                    break;
                }
            }

            if (matchingEndpoint == null)
            {
                _proxyTrace.TraceEvent(TraceEventType.Error, 107000, string.Format("GetEndpoint Error for {0}.{1}: No Endpoint Found", serviceName, serviceMethod));
                throw new ArgumentException(string.Format(
                    Constants.ErrorMessages.EndpointNotFound,
                    contractName, contractNamespace));
            }

            return matchingEndpoint;
        }

        ///// <summary>
        ///// Dynamic Service Proxy Factory Contract Name Match Utility
        ///// </summary>
        ///// <param name="cDesc">Dynamic Service Proxy Factory ContractDescription</param>
        ///// <param name="name">Dynamic Service Proxy Factory Contract Name</param>
        ///// <returns>boolean</returns>
        //private bool ContractNameMatch(ContractDescription cDesc, string name)
        //{
        //    return (string.Compare(cDesc.Name, name, true) == 0);
        //}

        ///// <summary>
        ///// Dynamic Service Proxy Factory Contract Namespace Match Utility
        ///// </summary>
        ///// <param name="cDesc">Dynamic Service Proxy Factory ContractDescription</param>
        ///// <param name="ns">Dynamic Service Proxy Factory Contract Name</param>
        ///// <returns>boolean</returns>
        //private bool ContractNsMatch(ContractDescription cDesc, string ns)
        //{
        //    return ((ns == null) ||
        //            (string.Compare(cDesc.Namespace, ns, true) == 0));
        //}

        
        /// <summary>
        /// Dynamic Service Proxy Factory DynamicProxy Contract Name constructor
        /// </summary>
        /// <param name="contractName">Dynamic Service Proxy Factory Contract Name</param>
        /// <returns>DynamicPoxy</returns>
        public DynamicServiceProxy CreateProxy(string contractName)
        {
            return CreateProxy(contractName, null);
        }

        /// <summary>
        /// Dynamic Service Proxy Factory DynamicProxy Contract Name constructor with Namespace
        /// </summary>
        /// <param name="contractName">Dynamic Service Proxy Factory Contract Name</param>
        /// <param name="contractNamespace">Dynamic Service Proxy Factory Contract Namespace</param>
        /// <returns>DynamicProxy</returns>
        public DynamicServiceProxy CreateProxy(string contractName,
                string contractNamespace)
        {
            if (this.options.ServiceProxyType.Equals(DynamicServiceProxyFactoryOptions.ServiceProxyTypes.Wsdl))
            {
                ServiceEndpoint endpoint = GetEndpoint(contractName,
                        contractNamespace);

                return CreateProxy(endpoint);
            }
            else //if (this.options.ServiceProxyType.Equals(DynamicServiceProxyFactoryOptions.ServiceProxyTypes.Xml))
            {
                Type xmlContractType = GetXmlEnvelopeType(contractName, contractNamespace);
                return new DynamicServiceProxy(xmlContractType);
            }
        }

        /// <summary>
        /// Dynamic Service Proxy Factory DynamicProxy ServiceEndpoint constructor
        /// </summary>
        /// <param name="endpoint">Dynamic Service Proxy Factory ServiceEndpoint</param>
        /// <returns>DynamicProxy</returns>
        public DynamicServiceProxy CreateProxy(ServiceEndpoint endpoint)
        {
            Type contractType = GetContractType(endpoint.Contract.Name,
                endpoint.Contract.Namespace);

            Type proxyType = GetProxyType(contractType);

            return new DynamicServiceProxy(proxyType, endpoint.Binding,
                    endpoint.Address);
        }

        /// <summary>
        /// Dynamic Service Proxy Factory ServiceContract Type getter
        /// </summary>
        /// <param name="contractName">Dynamic Service Proxy Factory Contract Name</param>
        /// <param name="contractNamespace">Dynamic Service Proxy Factory Contract Namespace</param>
        /// <returns>Type</returns>
        private Type GetContractType(string contractName,
                string contractNamespace)
        {
            Type[] allTypes = proxyAssembly.GetTypes();
            ServiceContractAttribute scAttr = null;
            Type contractType = null;
            XmlQualifiedName cName;
            foreach (Type type in allTypes)
            {
                // Is it an interface?
                if (!type.IsInterface) continue;

                // Is it marked with ServiceContract attribute?
                object[] attrs = type.GetCustomAttributes(
                    typeof(ServiceContractAttribute), false);
                if ((attrs == null) || (attrs.Length == 0)) continue;

                // is it the required service contract?
                scAttr = (ServiceContractAttribute)attrs[0];
                cName = GetContractName(type, scAttr.Name, scAttr.Namespace);

                if (string.Compare(cName.Name, contractName, true) != 0)
                    continue;

                if (string.Compare(cName.Namespace, contractNamespace,
                            true) != 0)
                    continue;

                contractType = type;
                break;
            }

            if (contractType == null)
            {
                _proxyTrace.TraceEvent(TraceEventType.Error, 107001, string.Format("GetContractType Error for {0}.{1}: {2}", serviceName, serviceMethod, Constants.ErrorMessages.UnknownContract));
                throw new ArgumentException(
                    Constants.ErrorMessages.UnknownContract);
            }
            return contractType;
        }

        /// <summary>
        /// Dynamic Service Proxy Factory XML "Contract Type" getter
        /// </summary>
        /// <param name="envelopeName">Dynamic Service Proxy Factory XML envelope Name</param>
        /// <param name="envelopeNamespace">Dynamic Service Proxy Factory XML envelope Namespace</param>
        /// <returns>Type</returns>
        private Type GetXmlEnvelopeType(string envelopeName,
                string envelopeNamespace)
        {
            
            Type[] allTypes = proxyAssembly.GetTypes();
            
            Type envelopeType = null;
            foreach (Type type in allTypes)
            {
                
                if (string.Compare(type.Name, envelopeName, true) != 0)
                    continue;

                if (envelopeNamespace != null && (string.Compare(type.Namespace, envelopeNamespace, true) != 0))
                    continue;

                envelopeType = type;
                break;
            }

            if (envelopeType == null)
            {
                _proxyTrace.TraceEvent(TraceEventType.Error, 105000, string.Format("XmlEnvelopeType Error for {0}.{1}: {2}", serviceName, serviceMethod, Constants.ErrorMessages.UnknownContract));
                throw new ArgumentException(
                    Constants.ErrorMessages.UnknownContract);
            }

            return envelopeType;
        }

        internal const string DefaultNamespace = "http://tempuri.org/";

        /// <summary>
        /// Dynamic Service Proxy Factory GetContractName in XmlQualifiedName format
        /// </summary>
        /// <param name="contractType">Dynamic Service Proxy Factory service contract type</param>
        /// <param name="name">Dynamic Service Proxy Factory service contract name</param>
        /// <param name="ns">Dynamic Service Proxy Factory service contract namespace</param>
        /// <returns>XmlQualifiedName</returns>
        internal static XmlQualifiedName GetContractName(Type contractType,
            string name, string ns)
        {
            if (String.IsNullOrEmpty(name))
            {
                name = contractType.Name;
            }

            if (ns == null)
            {
                ns = DefaultNamespace;
            }
            else
            {
                ns = Uri.EscapeUriString(ns);
            }

            return new XmlQualifiedName(name, ns);
        }

        /// <summary>
        /// Dynamic Service Proxy Factory GetProxyType by contract type
        /// </summary>
        /// <param name="contractType">Dynamic Service Proxy Factory contract type</param>
        /// <returns>Type</returns>
        private Type GetProxyType(Type contractType)
        {
            Type clientBaseType = typeof(ClientBase<>).MakeGenericType(
                    contractType);

            Type[] allTypes = ProxyAssembly.GetTypes();
            Type proxyType = null;

            foreach (Type type in allTypes)
            {
                // Look for a proxy class that implements the service 
                // contract and is derived from ClientBase<service contract>
                if (type.IsClass && contractType.IsAssignableFrom(type)
                    && type.IsSubclassOf(clientBaseType))
                {
                    proxyType = type;
                    break;
                }
            }

            if (proxyType == null)
                throw new DynamicServiceProxyException(string.Format(
                            Constants.ErrorMessages.ProxyTypeNotFound,
                            contractType.FullName));

            return proxyType;
        }

        /// <summary>
        /// Dynamic Service Proxy Factory CodeDom Provider
        /// </summary>
        private void CreateCodeDomProvider()
        {
            this.codeDomProvider = CodeDomProvider.CreateProvider(options.Language.ToString());
        }

        /// <summary>
        /// Dynamic Service Proxy Factory CreateServiceContractGenerator
        /// </summary>
        private void CreateServiceContractGenerator()
        {
            this.contractGenerator = new ServiceContractGenerator(
                this.codeCompileUnit);
            this.contractGenerator.Options |= ServiceContractGenerationOptions.ClientClass;
        }

        /// <summary>
        /// Dynamic Service Proxy Factory Metadata getter
        /// </summary>
        public IEnumerable<MetadataSection> Metadata
        {
            get
            {
                return this.metadataCollection;
            }
        }

        /// <summary>
        /// Dynamic Service Proxy Factory Bindings getter
        /// </summary>
        public IEnumerable<Binding> Bindings
        {
            get
            {
                return this.bindings;
            }
        }

        /// <summary>
        /// Dynamic Service Proxy Factory Contracts getter
        /// </summary>
        public IEnumerable<ContractDescription> Contracts
        {
            get
            {
                return this.contracts;
            }
        }

        /// <summary>
        /// Dynamic Service Proxy Factory Endpoints getter
        /// </summary>
        public IEnumerable<ServiceEndpoint> Endpoints
        {
            get
            {
                return this.endpoints;
            }
        }

        /// <summary>
        /// Dynamic Service Proxy Factory ProxyAssembly getter
        /// </summary>
        public Assembly ProxyAssembly
        {
            get
            {
                return this.proxyAssembly;
            }
        }

        /// <summary>
        /// Dynamic Service Proxy Factory ProxyCode getter
        /// </summary>
        public string ProxyCode
        {
            get
            {
                return this.proxyCode;
            }
        }

        public string ProxyWsdl
        {
            get
            {
                return this.proxyWsdl;
            }
        }

        /// <summary>
        /// Dynamic Service Proxy Factory MetadataImportWarnings getter
        /// </summary>
        public IEnumerable<MetadataConversionError> MetadataImportWarnings
        {
            get
            {
                return this.importWarnings;
            }
        }

        /// <summary>
        /// Dynamic Service Proxy Factory CodeGenerationWarnings getter
        /// </summary>
        public IEnumerable<MetadataConversionError> CodeGenerationWarnings
        {
            get
            {
                return this.codegenWarnings;
            }
        }

        /// <summary>
        /// Dynamic Service Proxy Factory CompilationWarnings getter
        /// </summary>
        public IEnumerable<CompilerError> CompilationWarnings
        {
            get
            {
                return this.compilerWarnings;
            }
        }

        /// <summary>
        /// Dynamic Service Proxy Factory ToString converts MetadataConversionError to string
        /// </summary>
        /// <param name="importErrors">Dynamic Service Proxy Factory metadataConversionErrors</param>
        /// <returns>string</returns>
        public static string ToString(IEnumerable<MetadataConversionError>
            importErrors)
        {
            if (importErrors != null)
            {
                StringBuilder importErrStr = new StringBuilder();

                foreach (MetadataConversionError error in importErrors)
                {
                    if (error.IsWarning)
                        importErrStr.AppendLine("Warning : " + error.Message);
                    else
                        importErrStr.AppendLine("Error : " + error.Message);
                }

                return importErrStr.ToString();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Dynamic Service Proxy Factory ToString converts CompilerError to string
        /// </summary>
        /// <param name="importErrors">Dynamic Service Proxy Factory CompilerError</param>
        /// <returns>string</returns>
        public static string ToString(IEnumerable<CompilerError> compilerErrors)
        {
            if (compilerErrors != null)
            {
                StringBuilder builder = new StringBuilder();
                foreach (CompilerError error in compilerErrors)
                    builder.AppendLine(error.ToString());

                return builder.ToString();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Dynamic Service Proxy Factory ToEnumerable converts a collection of compiler errors to an IEnumerable
        /// </summary>
        /// <param name="collection">Dynamic Service Proxy Factory Collection of compiler Errors</param>
        /// <returns>IEnumberable compiler errors</returns>
        private static IEnumerable<CompilerError> ToEnumerable(
                CompilerErrorCollection collection)
        {
            if (collection == null) return null;

            List<CompilerError> errorList = new List<CompilerError>();
            foreach (CompilerError error in collection)
                errorList.Add(error);
            
            return errorList;
        }

        /// <summary>
        /// Dynamic Service Proxy Factory CreateProxyForTypes is used to generate Type proxies for the XML based Services
        /// </summary>
        /// <param name="qualifiedObjectName">Dynamic Service Proxy Factory fully qualified type name</param>
        /// <param name="xmlEnvelopeName">Dynamic Service Proxy Factory XML envelope Name</param>
        /// <param name="url">Dynamic Service Proxy Factory URL for XML based service</param>
        /// <param name="yourListOfFields">Dynamic Service Proxy Factory list of XmlServiceParameters (Name Type)</param>
        /// <returns>string</returns>
        private static string CreateProxyForTypes(string qualifiedObjectName, string xmlEnvelopeName, string url, List<XmlServiceParameters> yourListOfFields, bool logRequests)
        {
            if(logRequests)
            {
                _trace.TraceEvent(TraceEventType.Information, 1100, string.Format("Generating code with request logging for: {0}", qualifiedObjectName));
            }

            CultureInfo culture = new CultureInfo("ja-JP");
            DateTime dt = DateTime.Now;
            DateTime utc = DateTime.UtcNow;
            TimeSpan ts = dt - utc;
            //TODO: This was causing an exception from time to time
            //DateTimeOffset logOffset = new DateTimeOffset(dt, ts);
            //string logUtc = logOffset.ToString("zzz", CultureInfo.InvariantCulture);
            string logUtc = "-07:00";
            string logTsFormat = "M/d/yyyy hh:mm:ss tt";
            string logTs = dt.ToString(logTsFormat, CultureInfo.InvariantCulture);
            string datestring = dt.ToString("d", culture);
            string[] dateparts = datestring.Split('/');
            string fileTs = string.Format("-{0}-{1}-{2}", dateparts[0], dateparts[1], dateparts[2]);

            

            StringBuilder proxyBuilder = new StringBuilder();
            proxyBuilder.Append("using System;using System.Diagnostics;using System.IO;using DynamicServiceProxyNamespace;using AtiWrapperServices;using System.Globalization;");
            proxyBuilder.Append(string.Format("public class {0} {{ ", qualifiedObjectName));
            
            proxyBuilder.Append(string.Format(" public string {0}_Proxy (", qualifiedObjectName));
            int i = 0;
            foreach (var field in yourListOfFields)
            {
                proxyBuilder.Append(string.Format("{0} {1}", Type.GetType(field.TypeName), field.Name.Split('.').Last()));
                i++;
                if (i < yourListOfFields.Count)
                {
                    proxyBuilder.Append(", ");
                }
            }
            proxyBuilder.Append(@") { ");

            proxyBuilder.Append(@"System.Collections.Generic.List<DynamicServiceProxyNamespace.XmlServiceParameters> myListOfTypes = new System.Collections.Generic.List<DynamicServiceProxyNamespace.XmlServiceParameters>(); ");
            foreach (var field in yourListOfFields)
            {
                proxyBuilder.Append(@"myListOfTypes.Add( new DynamicServiceProxyNamespace.XmlServiceParameters { Name = """);
                proxyBuilder.Append(string.Format(@"{0}", field.Name.Split('.').Last()));
                proxyBuilder.Append(@""", TypeName = """);
                proxyBuilder.Append(string.Format("{0}", field.TypeName));
                proxyBuilder.Append(@""" });");
            }

            proxyBuilder.Append(@" System.Type typeBuilderType = System.Reflection.Assembly.GetCallingAssembly().GetType(""DynamicServiceProxyNamespace.XmlServiceTypeBuilder"");");
            proxyBuilder.Append(@"if(typeBuilderType == null) throw new System.Exception(""typeBuilderType is null"");");
            proxyBuilder.Append(@"object typeBuilderObject = System.Activator.CreateInstance(typeBuilderType, new object[] { myListOfTypes, ");
            proxyBuilder.Append(string.Format(@" ""{0}"", ""{1}""", qualifiedObjectName, xmlEnvelopeName));
            proxyBuilder.Append(@" }); object proxyObject = typeBuilderObject.GetType().InvokeMember(""CreateNewObject"", System.Reflection.BindingFlags.Public | ");
            proxyBuilder.Append(@"System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.InvokeMethod, null, typeBuilderObject, null);");

            foreach (var field in yourListOfFields)
            {
                proxyBuilder.Append(string.Format(@"proxyObject.GetType().InvokeMember(""{0}""", field.Name.Split('.').Last()));
                proxyBuilder.Append(", System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.SetProperty,");
                proxyBuilder.Append(string.Format("null, proxyObject, new object[] {{ {0} }});", field.Name.Split('.').Last()));

            }

            proxyBuilder.Append("System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(proxyObject.GetType());");
            proxyBuilder.Append("System.Xml.Serialization.XmlSerializerNamespaces serializerNamespace = new System.Xml.Serialization.XmlSerializerNamespaces();");
            proxyBuilder.Append(@"serializerNamespace.Add("""", """");");

            proxyBuilder.Append("System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();");
            proxyBuilder.Append("using (System.Xml.XmlWriter writer = System.Xml.XmlWriter.Create(memoryStream, new System.Xml.XmlWriterSettings ");
            proxyBuilder.Append(@"{ OmitXmlDeclaration = true, Encoding = System.Text.ASCIIEncoding.ASCII, Indent = true })){");
            proxyBuilder.Append(@"serializer.Serialize(writer, proxyObject, serializerNamespace); }");

            //This will disable Certificate Validation
            //proxyBuilder.Append("System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };");

            proxyBuilder.Append(string.Format(@"System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(""{0}"");", url));
            proxyBuilder.Append(@"req.KeepAlive = false; req.ProtocolVersion = System.Net.HttpVersion.Version10; req.Method = ""POST""; req.ContentType = ""application/x-www-form-urlencoded"";");

            proxyBuilder.Append("byte[] postBytes = memoryStream.ToArray(); req.ContentLength = postBytes.Length;");
            proxyBuilder.Append("System.IO.Stream requestStream = req.GetRequestStream(); requestStream.Write(postBytes, 0, postBytes.Length);");

            //Logging
            if (logRequests)
            {
                //////TODO: add this to debug
                //proxyBuilder.Append("System.Diagnostics.Debugger.Break();");
                
                using (StreamWriter w = File.AppendText(string.Format(@"E:\App_Logs\AtiWrapperService\AtiWrapperServiceRequest{0}.log", fileTs)))
                {
                    w.Write("{0} ", logTs);
                    w.WriteLine("{0} Starting log for: {1} ", logUtc, qualifiedObjectName);
                }

                proxyBuilder.Append(string.Format(@"using (StreamWriter w = File.AppendText(@""E:\App_Logs\AtiWrapperService\AtiWrapperServiceRequest{0}.log""))",fileTs));
                proxyBuilder.Append("{memoryStream.Position = 0; StreamReader rdr = new StreamReader(memoryStream); string logRequest = rdr.ReadToEnd();");

                proxyBuilder.Append(@"DateTime dt = DateTime.Now; string logTsFormat = ""M/d/yyyy hh:mm:ss tt""; string logTs = dt.ToString(logTsFormat, CultureInfo.InvariantCulture);");
                proxyBuilder.Append(@"w.Write(""{0} "", logTs); ");
                proxyBuilder.Append(string.Format(@"w.WriteLine(""{0} {1} Proxy Request: "");", logUtc, qualifiedObjectName));
                proxyBuilder.Append(@"w.WriteLine(""{0}"",logRequest);}");
            }
            
            proxyBuilder.Append("requestStream.Close();");

            proxyBuilder.Append("System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)req.GetResponse();");
            proxyBuilder.Append("System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream()); string responseText = sr.ReadToEnd(); sr.Close();");

            proxyBuilder.Append(@"return responseText;}}");
            return proxyBuilder.ToString();
        }

        /// <summary>
        /// Dynamic Service Proxy Factory CompileScript compiles the DynamicProxy code
        /// </summary>
        /// <param name="code">Dynamic Service Proxy Factory code that was generated for the service contract</param>
        /// <returns>CompilerResults</returns>
        public static CompilerResults CompileSrcipt(string code, bool logging)
        {
            CompilerParameters parms = new CompilerParameters();

            parms.GenerateExecutable = false;
            //This is the default/Production section
            parms.GenerateInMemory = true;
            parms.IncludeDebugInformation = false;


            //This is for debugging it will generate symbol tables so that code can be debugged
            //parms.GenerateInMemory = false;
            //parms.IncludeDebugInformation = true;
            //parms.TempFiles = new TempFileCollection(Environment.GetEnvironmentVariable("TEMP"), true);
            //End Debug section

            parms.ReferencedAssemblies.Add(Assembly.GetExecutingAssembly().Location);
            parms.ReferencedAssemblies.Add("System.dll");
            parms.ReferencedAssemblies.Add("mscorlib.dll");
            parms.ReferencedAssemblies.Add("System.Xml.dll");

            if (!string.IsNullOrEmpty(wrapperAssemblyLocation))
            {
                parms.ReferencedAssemblies.Add(wrapperAssemblyLocation);
            }

            CodeDomProvider compiler = CSharpCodeProvider.CreateProvider("CSharp");

            return compiler.CompileAssemblyFromSource(parms, code);


        }
    }


}