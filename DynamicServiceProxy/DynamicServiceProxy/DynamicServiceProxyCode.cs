using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.CodeDom.Compiler;

namespace DynamicServiceProxyNamespace
{
    public class DynamicServiceProxyCode
    {
        public string Code { get; set; }
        public CompilerResults Results { get; set; }

        public DynamicServiceProxyCode(string code)
        {
            this.Code = code;
        }
    }
}
