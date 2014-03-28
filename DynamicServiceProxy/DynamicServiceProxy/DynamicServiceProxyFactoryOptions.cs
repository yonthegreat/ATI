using System;
using System.Text;

namespace DynamicServiceProxyNamespace
{
    /// <summary>
    /// Dynamic Service Proxy ProxyCodeModifier Delegate allows proxy code to be modified
    /// see CodeModifier below
    /// </summary>
    /// <param name="proxyCode">Dynamic Service Proxy code to be modified</param>
    /// <returns>string</returns>
    public delegate string ProxyCodeModifier(string proxyCode);


    /// <summary>
    /// Dynamic Service Proxy DynamicServiceProxyFactoryOptions class that contains the Options for the
    /// DynamicServiceProxyFactory
    /// </summary>
    public class DynamicServiceProxyFactoryOptions
    {
        /// <summary>
        /// Enumerations
        /// </summary>
        public enum LanguageOptions { CS, VB };
        public enum FormatModeOptions { Auto, XmlSerializer, DataContractSerializer };
        public enum EndpointAddressModifierOptions {  UseMetadataAddress, MakeSameAsUrl };
        public enum ServiceProxyTypes { Wsdl, Xml };
        public bool ServiceProxyLogging;

        /// <summary>
        /// internal fields
        /// </summary>
        private LanguageOptions lang;
        private FormatModeOptions mode;
        private ProxyCodeModifier codeModifier;
        private EndpointAddressModifierOptions addressModifier;
        private ServiceProxyTypes serviceProxyType;
        private string assemblyName;

        /// <summary>
        /// DynamicServiceProxyFactoryOptions constructor
        /// </summary>
        public DynamicServiceProxyFactoryOptions()
        {
            this.lang = LanguageOptions.CS;
            this.mode = FormatModeOptions.Auto;
            this.codeModifier = null;
            this.addressModifier = EndpointAddressModifierOptions.UseMetadataAddress;
            this.serviceProxyType = ServiceProxyTypes.Wsdl;
            this.ServiceProxyLogging = false;
        }

        /// <summary>
        /// DynamicServiceProxyFactoryOptions AssemblyName field getter and setter
        /// </summary>
        public string AssemblyName
        {
            get
            {
                return this.assemblyName;
            }

            set
            {
                this.assemblyName = value;
            }
        }

        /// <summary>
        /// DynamicServiceProxyFactoryOptions Endpoint filed getter and setter
        /// </summary>
        public EndpointAddressModifierOptions EndpointAddress
        {
            get
            {
                return this.addressModifier;
            }

            set
            {
                this.addressModifier = value;
            }
        }

        /// <summary>
        /// DynamicServiceProxyFactoryOptions Language field getter and setter
        /// </summary>
        public LanguageOptions Language
        {
            get
            {
                return this.lang;
            }

            set
            {
                this.lang = value;
            }
        }
        /// <summary>
        /// DynamicServiceProxyFactoryOptions FormatMode field getter and setter
        /// </summary>
        public FormatModeOptions FormatMode
        {
            get
            {
                return this.mode;
            }

            set
            {
                this.mode = value;
            }
        }

        /// <summary>
        /// DynamicServiceProxyFactoryOptions CodeModifier field getter and setter
        /// 
        /// The code modifier allows the user of the dynamic proxy factory to modify 
        /// the generated proxy code before it is compiled and used. This is useful in 
        /// situations where the generated proxy has to be modified manually for inter-operation 
        /// reason.
        /// </summary>
        public ProxyCodeModifier CodeModifier
        {
            get
            {
                return this.codeModifier;
            }

            set
            {
                this.codeModifier = value;
            }
        }

        /// <summary>
        /// DynamicServiceProxyFactoryOptions ServiceProxyType field getter and setter
        /// </summary>
        public ServiceProxyTypes ServiceProxyType
        {
            get
            {
                return this.serviceProxyType;
            }

            set
            {
                this.serviceProxyType = value;
            }
        }

        public bool Logging
        {
            get
            {
                return this.ServiceProxyLogging;
            }

            set
            {
                this.ServiceProxyLogging = value;
            }
        }

        /// <summary>
        /// DynamicServiceProxyFactoryOptions ToString returns the options fields in string format
        /// </summary>
        /// <returns>string</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("DynamicProxyFactoryOptions[");
            sb.Append("Language=" + Language);
            sb.Append(",FormatMode=" + FormatMode);
            sb.Append(",CodeModifier=" + CodeModifier);
            sb.Append(",AddressModifier=" + addressModifier);
            sb.Append(",ServiceProxyType=" + serviceProxyType);
            sb.Append(",AssemblyName=" + assemblyName);
            sb.Append("]");

            return sb.ToString();
        }
    }
}