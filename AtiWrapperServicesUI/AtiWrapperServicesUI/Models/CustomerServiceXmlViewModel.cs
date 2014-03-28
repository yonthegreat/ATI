using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AtiWrapperServicesUI.Models
{
    public class CustomerServiceXmlViewModel
    {
        [Required(ErrorMessage = "A Customer Name is Required")]
        public string CustomerName { get; set; }
        [Required(ErrorMessage = "A Test URL is Required")]
        [RegularExpression(@"^http(s?)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&amp;%\$#_]*)?$", ErrorMessage = "Invalid Url Format")]
        public string TestUrl { get; set; }
        [Required(ErrorMessage = "A Production URL is Required")]
        [RegularExpression(@"^http(s?)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&amp;%\$#_]*)?$", ErrorMessage = "Invalid Url Format")]
        public string ProductionUrl { get; set; }
        [Required(ErrorMessage = "A Service Name is Required")]
        public string ServiceName { get; set; }
        [Required(ErrorMessage = "A Service Method Name is Required")]
        public string ServiceMethod { get; set; }
        [Required(ErrorMessage = "A Service Method XML Template Location is Required")]
        public string ServiceMethodXmlTemplateLocation { get; set; }
        public TypeData ReturnType { get; set; }
        public List<TypeData> Parameters { get; set; }
        public List<TypeData> FlatReturnType { get; set; }
        public List<TypeData> FlatParameters { get; set; }
    }
}