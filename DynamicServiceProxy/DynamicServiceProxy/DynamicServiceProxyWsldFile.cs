using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DynamicServiceProxyNamespace
{
    public class DynamicServiceProxyWsldFile
    {
        public static string FileName = @"C:\Temp\WsdlFile";

        public DynamicServiceProxyWsldFile(string wsdl)
        {
            byte[] wsdlBytes = new byte[wsdl.Length * sizeof(char)];
            System.Buffer.BlockCopy(wsdl.ToCharArray(), 0, wsdlBytes, 0, wsdlBytes.Length);

            using (FileStream fs = new FileStream(FileName, FileMode.CreateNew))
            {
                fs.Write(wsdlBytes, 0, wsdlBytes.Length);
            }
        }
    }
}
