using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;



namespace AtiWrapperServicesUI.Models
{
    public class SoapHeaderModel
    {
        [DataType(DataType.MultilineText)]
        public string SoapHeader { get; set; }
    }
}