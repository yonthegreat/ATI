using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.IO;
using System.Web.Services.Protocols;
using System.Xml;

namespace AtiWrapperServicesUI
{
    public class SoapHeader : SoapExtension
    {
        public bool outgoing = true;
        public bool incoming = false;
        private Stream outputStream;
        public Stream oldStream;
        public Stream newStream;

        public override Stream ChainStream(Stream stream)
        {
            // save a copy of the stream, create a new one for manipulating.
            this.outputStream = stream;
            oldStream = stream;
            newStream = new MemoryStream();
            return newStream;
        }
        public override object GetInitializer(Type serviceType)
        {
            return null;
        }

        public override object GetInitializer(LogicalMethodInfo methodInfo, SoapExtensionAttribute attribute)
        {
            return null;
        }

        public override void Initialize(object initializer)
        {

        }

        public string getXMLFromCache()
        {
            newStream.Position = 0; // start at the beginning!
            string strSOAPresponse = ExtractFromStream(newStream);
            return strSOAPresponse;
        }
        private String ExtractFromStream(Stream target)
        {
            if (target != null)
                return (new StreamReader(target)).ReadToEnd();
            return "";
        }

        public override void ProcessMessage(SoapMessage message)
        {
           StreamReader readStr;
           StreamWriter writeStr;
           string soapMsg1;
           XmlDocument xDoc = new XmlDocument();
           // a SOAP message has 4 stages. We're interested in .AfterSerialize
           switch (message.Stage)
           {
              case SoapMessageStage.BeforeSerialize:
              break;
              case SoapMessageStage.AfterSerialize:
              {
              // Get the SOAP body as a string, so we can manipulate...
              String soapBodyString = getXMLFromCache();
              // Strip off the old header stuff before the message body
              // I'm not completely sure, but the soap:encodingStyle might be
              // unique to the WebSphere environment. Dunno.
              String BodString = "<soap:Body>";
              int pos1 = soapBodyString.IndexOf(BodString) + BodString.Length;
              int pos2 = soapBodyString.Length - pos1;
              soapBodyString = soapBodyString.Substring(pos1, pos2);
              soapBodyString = "<soap:Body>" + soapBodyString;
              // Create the SOAP Message 
              // It's comprised of a <soap:Element> that's enclosed in <soap:Body>. 
              // Pack the XML document inside the <soap:Body> element 
              String xmlVersionString = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
              String soapEnvelopeBeginString =
                  //"<soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:typ=\"http://payandreinstate.service.bcbsnc.com/types\">";
              "<soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">";
              String soapEnvHeaderString = 
                  "<soap:Header><wsse:Security xmlns:wsse=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd\"><wsse:UsernameToken><wsse:Username>";
              String soapEnvHeaderString2 = "</wsse:Username><wsse:Password>";
              String soapEnvHeaderString3 = 
                  "</wsse:Password></wsse:UsernameToken></wsse:Security></soap:Header>";
              Stream appOutputStream = new MemoryStream();
              StreamWriter soapMessageWriter = new StreamWriter(appOutputStream);
              soapMessageWriter.Write(xmlVersionString);
              soapMessageWriter.Write(soapEnvelopeBeginString);
              // The heavy-handed part - forcing the right headers AND the uname/pw :)
              soapMessageWriter.Write(soapEnvHeaderString);
              soapMessageWriter.Write("ATIPaymentUser");
              soapMessageWriter.Write(soapEnvHeaderString2);
              soapMessageWriter.Write("test123");
              soapMessageWriter.Write(soapEnvHeaderString3);
              // End clubbing of baby seals
              // Add the soapBodyString back in - it's got all the closing 
              // XML we need.
              soapMessageWriter.Write(soapBodyString);
              // write it all out.
              soapMessageWriter.Flush();
              appOutputStream.Flush();
              appOutputStream.Position = 0;
              StreamReader reader = new StreamReader(appOutputStream);
              StreamWriter writer = new StreamWriter(this.outputStream);
              writer.Write(reader.ReadToEnd());
              writer.Flush();
              appOutputStream.Close();
              this.outgoing = false;
              this.incoming = true;
              break;
            }
           case SoapMessageStage.BeforeDeserialize:
           {
              // Make the output available for the client to parse...
              readStr = new StreamReader(oldStream);
              writeStr = new StreamWriter(newStream);
              soapMsg1 = readStr.ReadToEnd();
              xDoc.LoadXml(soapMsg1);
              soapMsg1 = xDoc.InnerXml;
              writeStr.Write(soapMsg1);
              writeStr.Flush();
              newStream.Position = 0;
              break;
           }
           case SoapMessageStage.AfterDeserialize:
           break;
           default:
           throw new Exception("invalid stage!");
           }
         }
    }
}