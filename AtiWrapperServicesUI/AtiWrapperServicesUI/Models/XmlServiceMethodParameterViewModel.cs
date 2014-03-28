using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AtiWrapperServicesUI.Models
{
    public class XmlServiceMethodParameterViewModel
    {
        public List<XmlServiceMethodTypeViewModel> XmlServiceMethodTypes { get; set; }
    }

    public class XmlServiceMethodTypeViewModel
    {
        public string Name { get; set; }
        public string TypeName { get; set; }
        public bool IsSystemType { get; set; }
        public bool Include { get; set; }
        public List<SelectListItem> SupportedTypes { get; set; }
    }
}