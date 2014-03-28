using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services.Protocols;

namespace AtiWrapperServicesUI
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple=false)]
    public class CustomSoapHeaderAttribute : SoapExtensionAttribute
    {
        int priority = 1;

        public override System.Type ExtensionType { get { return typeof(SoapHeader); } }
        public override int Priority { get { return priority; } set {priority = value; } }
    }
}