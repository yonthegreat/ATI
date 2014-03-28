using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AtiWrapperServicesUI.Models
{
    public class XmlServiceMethodTestViewModel
    {
        public string CustomerName { get; set; }
        public string ServiceName { get; set; }
        public string ServiceMethod { get; set; }
        public List<ParameterViewModel> MethodParameters { get; set; }
    }
}