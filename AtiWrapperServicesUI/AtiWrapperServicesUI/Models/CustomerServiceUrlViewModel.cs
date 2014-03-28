using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using DynamicServiceProxyNamespace;

namespace AtiWrapperServicesUI.Models
{
    public class CustomerServiceUrlViewModel
    {
        [Required(ErrorMessage = "A Customer Name is Required")]
        public string CustomerName { get; set; }
        [Required(ErrorMessage = "A Test URL is Required")]
        public string TestUrl { get; set; }
        //[Range (0, 2, ErrorMessage = "0 = Url, 1 = WSLD, 2 = Code")]
        public AtiWrapperServices.AtiWrapperServicesEnums.UseSource TestUseSource { get; set; }
        [Required(ErrorMessage = "A Production URL is Required")]
        public string ProductionUrl { get; set; }
        //[Range(0, 2, ErrorMessage = "0 = Url, 1 = WSLD, 2 = Code")]
        public AtiWrapperServices.AtiWrapperServicesEnums.UseSource ProductionUseSource { get; set; }
        public string ServiceName { get; set; }
        public List<SelectListItem> ServiceNames = new List<SelectListItem>();
        public string ServiceMethod { get; set; }
        public List<SelectListItem> ServiceMethods = new List<SelectListItem>();
        public TypeData ReturnType { get; set; }
        public List<TypeData> Parameters { get; set; }
        public List<TypeData> FlatReturnType { get; set; }
        public List<TypeData> FlatParameters { get; set; }
        public string CustomSoapHeaderName { get; set; }
        public SelectList CustomSoapHeaderList { get; set; }
        public DynamicServiceProxyFactoryOptions.EndpointAddressModifierOptions EndpointAddressModifierOption { get; set; } 
    }

    public class TypeData
    {
        public string Type { get; set; }
        public string Name { get; set; }
    }
}