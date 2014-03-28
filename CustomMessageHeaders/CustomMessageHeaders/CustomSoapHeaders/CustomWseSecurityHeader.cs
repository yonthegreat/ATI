using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Web;
using System.Xml;
using CustomMessageHeaders;

namespace CustomMessageHeaders.CustomSoapHeaders
{

    

    

    public class CustomWseSecurityHeader : MessageHeader
    {
        private string userName;
        private string secretWord;

        public CustomWseSecurityHeader(string userName, string password)
        {
            this.userName = userName;
            this.secretWord = password;
        }

        public CustomWseSecurityHeader()
        {
            this.userName = String.Empty;
            this.secretWord = String.Empty;
        }

        [HeaderDynamicProperty]
        public string UserName
        {
            get { return this.userName; }
            set { this.userName = value; }
        }

        [HeaderDynamicProperty]
        public string Password
        {
            get { return this.secretWord; }
            set { this.secretWord = value; }
        }

        protected override void OnWriteHeaderContents(XmlDictionaryWriter writer,
            MessageVersion messageVersion)
        {
            writer.WriteStartElement("UsernameToken", Namespace);
            writer.WriteElementString("Username", Namespace, userName);
            writer.WriteElementString("Password", Namespace, secretWord);
            writer.WriteEndElement();
        }

        public override string Name
        {
            get { return "Security"; }
        }

        public override string Namespace
        {
            get { return "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"; }
        }

        public override bool MustUnderstand
        {
            get { return false; }
        }
    }
}