using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AtiWrapperServicesUI.Models
{
    public class ServiceMethodParameterViewModel
    {
        public string ServiceMethodName { get; set; }
        public string WrapperMethodName { get; set; }
        public string SoapHeaderTypeName { get; set; }
        public List<ParameterViewModel> SoapHeaders { get; set; }
        public List<ParameterViewModel> Parameters { get; set; }
    }

    public class WrapperMethodParameterViewModel
    {
        public string CustomerName { get; set; }
        public string ServiceMethodName { get; set; }
        public string WrapperMethodName { get; set; }
        public string SoapHeaderTypeName { get; set; }
        public List<ParameterDefaultViewModel> SoapHeaders { get; set; }
        public List<ParameterDefaultViewModel> Parameters { get; set; }
    }

    
    public class ParameterViewModel
    {
        public string Name { get; set; }
        public string TypeName { get; set; }
        public Object Value { get; set; }
    }


    public class ParameterDefaultViewModel : ParameterViewModel
    {
        public int Id { get; set; }
        public bool Required { get; set; }
        public SelectList EnumList { get; set; }
    }
}