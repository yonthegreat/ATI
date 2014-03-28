using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AtiWrapperServicesUI.Models
{
    public class WrapperMethodViewModel
    {
        public string CustomerName { get; set; }
        public List<SelectListItem> CustomerNames = new List<SelectListItem>();
        public string ServiceMethod { get; set; }
        public List<SelectListItem> ServiceMethods = new List<SelectListItem>();
        public string WrapperMethod { get; set; }
    }
}