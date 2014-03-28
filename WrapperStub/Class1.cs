using System;
using System.Diagnostics;
using System.IO;
using DynamicServiceProxyNamespace;
public class TEMPUS_CARD_SERVICES_AtiTempusCCAUTHSALE 
{ 
    static public System.Diagnostics.TraceSource _trace = new TraceSource("AtiWrapperServiceRequestsLog"); 
    public string TEMPUS_CARD_SERVICES_AtiTempusCCAUTHSALE_Proxy(System.Int32 RNID, System.String RNCERT, System.String TRANSACTIONTYPE, System.String CCATHTYPE, System.String CCACCOUNT, System.String CCEXP, System.String CCAMT, System.Int32 CCCVV) 
    { 
        System.Collections.Generic.List<DynamicServiceProxyNamespace.XmlServiceParameters> myListOfTypes = new System.Collections.Generic.List<DynamicServiceProxyNamespace.XmlServiceParameters>(); 
        myListOfTypes.Add(new DynamicServiceProxyNamespace.XmlServiceParameters { Name = "RNID", TypeName = "System.Int32" }); 
        myListOfTypes.Add(new DynamicServiceProxyNamespace.XmlServiceParameters { Name = "RNCERT", TypeName = "System.String" }); 
        myListOfTypes.Add(new DynamicServiceProxyNamespace.XmlServiceParameters { Name = "TRANSACTIONTYPE", TypeName = "System.String" }); 
        myListOfTypes.Add(new DynamicServiceProxyNamespace.XmlServiceParameters { Name = "CCATHTYPE", TypeName = "System.String" }); 
        myListOfTypes.Add(new DynamicServiceProxyNamespace.XmlServiceParameters { Name = "CCACCOUNT", TypeName = "System.String" }); 
        myListOfTypes.Add(new DynamicServiceProxyNamespace.XmlServiceParameters { Name = "CCEXP", TypeName = "System.String" }); 
        myListOfTypes.Add(new DynamicServiceProxyNamespace.XmlServiceParameters { Name = "CCAMT", TypeName = "System.String" }); 
        myListOfTypes.Add(new DynamicServiceProxyNamespace.XmlServiceParameters { Name = "CCCVV", TypeName = "System.Int32" }); 
        System.Type typeBuilderType = System.Reflection.Assembly.GetCallingAssembly().GetType("DynamicServiceProxyNamespace.XmlServiceTypeBuilder"); 
        if (typeBuilderType == null) 
            throw new System.Exception("typeBuilderType is null"); 
        object typeBuilderObject = System.Activator.CreateInstance(typeBuilderType, new object[] { myListOfTypes, "TEMPUS_CARD_SERVICES_AtiTempusCCAUTHSALE", "TRANSACTION" }); 
        object proxyObject = typeBuilderObject.GetType().InvokeMember("CreateNewObject", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.InvokeMethod, null, typeBuilderObject, null); 
        proxyObject.GetType().InvokeMember("RNID", System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.SetProperty, null, proxyObject, new object[] { RNID }); 
        proxyObject.GetType().InvokeMember("RNCERT", System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.SetProperty, null, proxyObject, new object[] { RNCERT });
        proxyObject.GetType().InvokeMember("TRANSACTIONTYPE", System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.SetProperty, null, proxyObject, new object[] { TRANSACTIONTYPE });
        proxyObject.GetType().InvokeMember("CCATHTYPE", System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.SetProperty, null, proxyObject, new object[] { CCATHTYPE });
        proxyObject.GetType().InvokeMember("CCACCOUNT", System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.SetProperty, null, proxyObject, new object[] { CCACCOUNT });
        proxyObject.GetType().InvokeMember("CCEXP", System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.SetProperty, null, proxyObject, new object[] { CCEXP });
        proxyObject.GetType().InvokeMember("CCAMT", System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.SetProperty, null, proxyObject, new object[] { CCAMT });
        proxyObject.GetType().InvokeMember("CCCVV", System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.SetProperty, null, proxyObject, new object[] { CCCVV });
        System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(proxyObject.GetType()); 
        System.Xml.Serialization.XmlSerializerNamespaces serializerNamespace = new System.Xml.Serialization.XmlSerializerNamespaces(); 
        serializerNamespace.Add("", ""); System.IO.MemoryStream memoryStream = new System.IO.MemoryStream(); 
        using (System.Xml.XmlWriter writer = System.Xml.XmlWriter.Create(memoryStream, new System.Xml.XmlWriterSettings { OmitXmlDeclaration = true, Encoding = System.Text.ASCIIEncoding.ASCII, Indent = true })) 
        { 
            serializer.Serialize(writer, proxyObject, serializerNamespace); 
        } 
        System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.WebRequest.Create("https://dev.spectrumretailnet.com/ppsapi"); 
        req.KeepAlive = false; 
        req.ProtocolVersion = System.Net.HttpVersion.Version10; 
        req.Method = "POST"; 
        req.ContentType = "application/x-www-form-urlencoded"; 
        byte[] postBytes = memoryStream.ToArray(); 
        req.ContentLength = postBytes.Length; 
        System.IO.Stream requestStream = req.GetRequestStream(); 
        requestStream.Write(postBytes, 0, postBytes.Length); 
        string logRequest = "TESTING"; 
        ServiceProxies._traceRequest.TraceEvent(TraceEventType.Information, 1001, logRequest); 
        requestStream.Close(); System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)req.GetResponse(); 
        System.IO.Stream resStream = response.GetResponseStream(); 
        System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream()); 
        string responseText = sr.ReadToEnd(); 
        sr.Close(); 
        return responseText;
    } 
}
