using System;
using System.Linq;
using AtiWrapperServicesUI.Models;
using System.Web.Mvc;
using AtiWrapperServicesORM.OpenAccess;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Reflection.Emit;
using System.Configuration;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Transactions;
using DynamicServiceProxyNamespace;
using Microsoft.CSharp;
using System.Text;


namespace AtiWrapperServicesUI.Controllers
{
    public class CustomerServiceXmlController : Controller
    {

        public static string XSDPATH = ConfigurationManager.AppSettings["XSDPATH"];

        [HttpGet]
        public ActionResult GetCustomers()
        {
            using (AtiWrapperServicesModel dbContext = new AtiWrapperServicesModel()) 
            {
                var customerList = dbContext.Customers.Select(c => new { CustomerId = c.Id, CustomerName = c.Name });
                return Json(customerList, JsonRequestBehavior.AllowGet);
            }
        }

        public class URlNameItem
        {
            public int UrlId { get; set; }
            public string UrlName { get; set; }
        }

        [HttpGet]
        public ActionResult GetTestUrlNames(string customerId)
        {
            int custId;
            URlNameItem[] urlNames = new URlNameItem[0];
            using (AtiWrapperServicesModel dbContext = new AtiWrapperServicesModel())
            {
                if (int.TryParse(customerId, out custId))
                {
                    urlNames = (from c in dbContext.Customers
                                join u in dbContext.ServiceURLs on c.Id equals u.Customer.Id
                                where c.Id == custId
                                select new URlNameItem { UrlId = u.Id, UrlName = u.TestURL }).ToArray();
                }
                return Json(urlNames, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult GetProductionUrlNames(string customerId)
        {
           int custId;
            URlNameItem[] urlNames = new URlNameItem[0];
            if (int.TryParse(customerId, out custId))
            {
                using (AtiWrapperServicesModel dbContext = new AtiWrapperServicesModel())
                {
                    urlNames = (from c in dbContext.Customers
                                join u in dbContext.ServiceURLs on c.Id equals u.Customer.Id
                                where c.Id == custId
                                select new URlNameItem { UrlId = u.Id, UrlName = u.ProductionURL }).ToArray();
                }
               
            }
            return Json(urlNames, JsonRequestBehavior.AllowGet);
        }

        public class ServiceNameItem
        {
            public int ServiceNameId { get; set; }
            public string ServiceName { get; set; }
        }

        [HttpGet]
        public ActionResult GetCustomerServiceNames(string customerId)
        {
            int custId;
            ServiceNameItem[] serviceNames = new ServiceNameItem[0];
            if (int.TryParse(customerId, out custId))
            {
                using (AtiWrapperServicesModel dbContext = new AtiWrapperServicesModel())
                {
                    serviceNames = (from c in dbContext.Customers
                                    join u in dbContext.ServiceURLs on c.Id equals u.Customer.Id
                                    join n in dbContext.ServiceNames on u.Id equals n.ServiceURL.Id
                                    where c.Id == custId
                                    select new ServiceNameItem { ServiceNameId = n.Id, ServiceName = n.Name }).ToArray();
                }
               
            }
            return Json(serviceNames, JsonRequestBehavior.AllowGet);
        }
        //
        // GET: /CustomerXmlServiceUrl/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /CustomerXmlServiceUrl/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /CustomerXmlServiceUrl/Create


        public ActionResult Create()
        {
            using (AtiWrapperServicesModel dbContext = new AtiWrapperServicesModel())
            {
                List<SelectListItem> customerList = (from c in dbContext.Customers
                                                     select new SelectListItem { Value = c.Name, Text = c.Name }).ToList();
                TempData["CustomerList"] = customerList;
                return View();
            }
        }

        //
        // POST: /CustomerXmlServiceUrl/Create

        [HttpPost]
        public ActionResult Create(CustomerServiceXmlViewModel model)
        {
            if (ModelState.IsValid)
            {
                string xmlTemplateLocation = string.Empty;
                if (! String.IsNullOrEmpty(model.ServiceMethodXmlTemplateLocation))
                {
                    if (Path.GetPathRoot(model.ServiceMethodXmlTemplateLocation) == String.Empty || Path.GetPathRoot(model.ServiceMethodXmlTemplateLocation).Equals("/"))
                    {
                        if (Path.HasExtension(model.ServiceMethodXmlTemplateLocation) && ! Path.GetExtension(model.ServiceMethodXmlTemplateLocation).Equals("xml"))
                        {
                            ModelState.AddModelError("", string.Format("File Extenion must be xml:"));
                            return View(model);
                        }

                    }
                    else
                    {
                        ModelState.AddModelError("", string.Format("File must be local: {0} is a drive/UNC path", Path.GetPathRoot(model.ServiceMethodXmlTemplateLocation)));
                        return View(model);
                    }
                    xmlTemplateLocation = ConfigurationManager.AppSettings["XMLTemplateDrive"] + Path.DirectorySeparatorChar + model.ServiceMethodXmlTemplateLocation;
                    if(! Path.HasExtension(model.ServiceMethodXmlTemplateLocation))
                    {
                        xmlTemplateLocation = xmlTemplateLocation + ".xml";
                    }
                }
                if (!System.IO.File.Exists(xmlTemplateLocation))
                {
                    ModelState.AddModelError("",string.Format("File Not Found: {0}", model.ServiceMethodXmlTemplateLocation));
                    return View(model);
                }
                else
                {
                    TempData["CustomerXmlServiceUrlViewModel"] = model;
                    return RedirectToAction("DefineServiceParameterTypes");
                    
                }
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult DefineServiceParameterTypes()
        {
            CustomerServiceXmlViewModel model = TempData["CustomerXmlServiceUrlViewModel"] as CustomerServiceXmlViewModel;
            TempData["CustomerXmlServiceUrlViewModel"] = model;
            string xmlTemplateFilename = Path.GetFileName(model.ServiceMethodXmlTemplateLocation);
            string xmltemplateDir = Path.GetDirectoryName(model.ServiceMethodXmlTemplateLocation);
            string xmlTemplateCode;

            //prohibit the use of DTD in the ServiceMethodXmlTemplate by reading the file
            XmlDocument doc = new XmlDocument();
            XmlReaderSettings docSettings = new XmlReaderSettings();
            docSettings.DtdProcessing = DtdProcessing.Prohibit;
            string tempFileName = ConfigurationManager.AppSettings["XMLTemplateDrive"] + Path.DirectorySeparatorChar + ConfigurationManager.AppSettings["XMLTemplateTempFile"];
            try
            {
                using (StreamReader sr = new StreamReader(ConfigurationManager.AppSettings["XMLTemplateDrive"] + Path.DirectorySeparatorChar + xmltemplateDir + Path.DirectorySeparatorChar + xmlTemplateFilename + ".xml"))
                {
                    using (XmlReader reader = XmlReader.Create(sr, docSettings))
                    {
                        doc.Load(reader);
                    }
                }
            }
            catch (Exception exdtd)
            {
                ModelState.AddModelError("", string.Format("Xml Template contains external DTD: {0}", exdtd.Message));
                return View("Create");
            }

            try
            {
                System.IO.File.Delete(tempFileName);
                doc.Save(tempFileName);
            }
            catch (Exception extemp)
            {
                ModelState.AddModelError("", string.Format("Error saving XmlTemplate: {0}", extemp.Message));
                return View("Create");
            }
            
            if (! System.IO.File.Exists(tempFileName))
            {
                ModelState.AddModelError("", string.Format("Error reading XmlTemplate: {0}", tempFileName));
                return View("Create");
            }
            ProcessStartInfo xmlToXsdProcessInfo = new ProcessStartInfo();
            xmlToXsdProcessInfo.CreateNoWindow = true;
            xmlToXsdProcessInfo.UseShellExecute = true;
            xmlToXsdProcessInfo.FileName = Path.GetFullPath(XSDPATH);
            xmlToXsdProcessInfo.Arguments = string.Format("{0} /o:{1}", tempFileName, Path.GetDirectoryName(tempFileName));

            try
            {
                using (Process xmlToXsd = Process.Start(xmlToXsdProcessInfo))
                {
                    xmlToXsd.WaitForExit();
                }
            }
            catch (Exception xsdEx)
            {
                ModelState.AddModelError("", string.Format("Error Converting Xml Template to XSD: {0}: {1} : {2}", xsdEx.Message, XSDPATH, tempFileName));
                return View("Create");
            }


            ProcessStartInfo xsdToCsProcessInfo = new ProcessStartInfo();
            xsdToCsProcessInfo.CreateNoWindow = true;
            xsdToCsProcessInfo.UseShellExecute = true;
            xsdToCsProcessInfo.FileName = Path.GetFullPath(XSDPATH);
            xsdToCsProcessInfo.Arguments = string.Format("{0} /c /l:CS /o:{1} /n:{2}",
                Path.GetDirectoryName(tempFileName) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(tempFileName) + ".xsd",
                Path.GetDirectoryName(tempFileName),
                model.ServiceName + "_" + model.ServiceMethod);

            try
            {
                using (Process xsdToCs = Process.Start(xsdToCsProcessInfo))
                {
                    xsdToCs.WaitForExit();
                }
            }
            catch (Exception csEx)
            {
                ModelState.AddModelError("", string.Format("Error Converting Xsd to Cs: {0}", csEx.Message));
                return View("Create");
            }

            try
            {
                xmlTemplateCode = System.IO.File.ReadAllText(Path.GetDirectoryName(tempFileName) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(tempFileName) + ".cs");
            }
            catch (Exception codeEx)
            {
                ModelState.AddModelError("", string.Format("Error Reading Cs File: {0} : {1}", Path.GetDirectoryName(tempFileName) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(tempFileName) + ".cs", codeEx.Message));
                return View("Create");
            }

            CompilerResults xmlTemplateClass = CompileSrcipt(xmlTemplateCode);

            if (xmlTemplateClass.Errors.HasErrors)
            {
                foreach (CompilerError ce in xmlTemplateClass.Errors)
                {
                    ModelState.AddModelError("", string.Format("Error compiling class: {0}", ce.ErrorText));
                }
                return View("Create");
            }

            XmlServiceMethodParameterViewModel smp = new XmlServiceMethodParameterViewModel();
            smp.XmlServiceMethodTypes = new List<XmlServiceMethodTypeViewModel>();
            foreach (TypeInfo t in xmlTemplateClass.CompiledAssembly.DefinedTypes)
            {
                BuildXmlServiceMethodPerameterTree(smp, t, t.Name, string.Empty);
            }

            return View("XmlServiceMethodParameterView", smp);
        }



        [HttpPost]
        public ActionResult DefineServiceParameterTypes(XmlServiceMethodParameterViewModel parameterModel, string command)
        {
            CustomerServiceXmlViewModel data = TempData["CustomerXmlServiceUrlViewModel"] as CustomerServiceXmlViewModel;
            TempData["CustomerXmlServiceUrlViewModel"] = data;
            using (AtiWrapperServicesModel dbContext = new AtiWrapperServicesModel())
            {

                using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    if (command == "RollBack")
                    {

                        RollBackServiceMethod(data, dbContext);

                        scope.Complete();

                        return View(@"Create");
                    }
                    else if (command == "Test")
                    {
                        return RedirectToAction("TestServiceMethod");
                    }
                    else
                    {
                        int customerId;
                        List<InputParams> inputParams = new List<InputParams>();
                        ServiceMethod serviceMethod;
                        Customer existingCustomer;
                        if (int.TryParse(data.CustomerName, out customerId))
                        {

                            existingCustomer = (from c in dbContext.Customers
                                                where c.Id == customerId
                                                select c).FirstOrDefault();

                            if (existingCustomer == null)
                            {
                                throw new Exception(string.Format("Bad Customer Name: {0}", data.CustomerName));
                            }
                            else
                            {
                                ServiceURL existingUrl;

                                existingUrl = (from c in dbContext.Customers
                                               join u in dbContext.ServiceURLs on c.Id equals u.Customer.Id
                                               where c.Id == customerId && u.ProductionURL == data.ProductionUrl && u.TestURL == data.TestUrl
                                               select u).FirstOrDefault();

                                if (existingUrl != null)
                                {
                                    ServiceName existingServiceName;

                                    existingServiceName = (from c in dbContext.Customers
                                                           join u in dbContext.ServiceURLs on c.Id equals u.Customer.Id
                                                           join n in dbContext.ServiceNames on u.Id equals n.ServiceURL.Id
                                                           where c.Id == customerId && n.Name == data.ServiceName
                                                           select n).FirstOrDefault();

                                    if (existingServiceName != null)
                                    {
                                        serviceMethod = AddServiceMethod(data, parameterModel, existingServiceName, dbContext, out inputParams);
                                    }
                                    else
                                    {

                                        var serviceName = new ServiceName
                                        {
                                            ServiceURL_Id = existingUrl.Id,
                                            Name = data.ServiceName,
                                            ServiceType = (int)DynamicServiceProxyFactoryOptions.ServiceProxyTypes.Xml,
                                        };
                                        dbContext.Add(serviceName);
                                        serviceMethod = AddServiceMethod(data, parameterModel, serviceName, dbContext, out inputParams);

                                    }
                                }
                                else
                                {

                                    var serviceUrl = new ServiceURL
                                    {
                                        Customer_Id = existingCustomer.Id,
                                        TestURL = data.TestUrl,
                                        ProductionURL = data.ProductionUrl,
                                    };

                                    var serviceName = new ServiceName
                                    {
                                        ServiceURL_Id = serviceUrl.Id,
                                        Name = data.ServiceName,
                                        ServiceType = (int)DynamicServiceProxyFactoryOptions.ServiceProxyTypes.Xml,
                                    };
                                    dbContext.Add(serviceUrl);
                                    dbContext.Add(serviceName);
                                    serviceMethod = AddServiceMethod(data, parameterModel, serviceName, dbContext, out inputParams);

                                }
                            }
                        }
                        else
                        {

                            Customer customer = new Customer
                            {
                                Name = data.CustomerName,
                            };

                            ServiceURL serviceUrl = new ServiceURL
                            {
                                Customer_Id = customer.Id,
                                TestURL = data.TestUrl,
                                ProductionURL = data.ProductionUrl,
                            };

                            ServiceName serviceName = new ServiceName
                            {
                                ServiceURL_Id = serviceUrl.Id,
                                Name = data.ServiceName,
                                ServiceType = (int)DynamicServiceProxyFactoryOptions.ServiceProxyTypes.Xml,
                            };

                            dbContext.Add(customer);
                            dbContext.Add(serviceUrl);
                            dbContext.Add(serviceName);
                            serviceMethod = AddServiceMethod(data, parameterModel, serviceName, dbContext, out inputParams);

                        }

                        dbContext.SaveChanges();
                        int i = 0;
                        foreach (var ip in inputParams)
                        {

                            InputParamater inputParameter;
                            
                                inputParameter = new InputParamater
                                {
                                    ServiceMethod_Id = serviceMethod.Id,
                                    Order = i,

                                };
                                dbContext.Add(inputParameter);
                                dbContext.SaveChanges();
                                i++;


                            QualifiedName qualifiedName = new QualifiedName()
                            {
                                Name = ip.Name,
                            };


                            ServiceType serviceType = new ServiceType
                            {
                                InputParamater_Id = inputParameter.Id,
                                QualifiedName_Id = qualifiedName.Id,
                                ReturnType_Id = null,
                                ParentId = null,
                                Name = ip.Name.Split('.').Last(),
                                TypeName = ip.TypeName,
                                IsSystemType = true,
                                IsCollectionType = false,
                                IsEnum = false,
                            };

                            dbContext.Add(qualifiedName);
                            dbContext.Add(serviceType);
                            dbContext.SaveChanges();

                        }


                        var returnType = new ReturnType
                        {
                            ServiceMethod_Id = serviceMethod.Id
                        };
                        dbContext.Add(returnType);
                        dbContext.SaveChanges();


                        QualifiedName rqualifiedName = new QualifiedName()
                        {
                            Name = serviceMethod.Name,
                        };


                        ServiceType rserviceType = new ServiceType
                        {
                            InputParamater_Id = null,
                            QualifiedName_Id = rqualifiedName.Id,
                            ReturnType_Id = returnType.Id,
                            ParentId = null,
                            Name = serviceMethod.Name,
                            TypeName = "System.String",
                            IsSystemType = true,
                            IsCollectionType = false,
                            IsEnum = false,
                        };

                        dbContext.Add(rqualifiedName);
                        dbContext.Add(rserviceType);
                        dbContext.SaveChanges();

                        scope.Complete();

                        return View("XmlServiceMethodParameterView", parameterModel);


                    }
                }
            }
        }

        private class InputParams
        {
            public string Name { get; set; }
            public string TypeName { get; set; }
        }
  
        private ServiceMethod AddServiceMethod(CustomerServiceXmlViewModel data, XmlServiceMethodParameterViewModel parameterModel, ServiceName existingServiceName, AtiWrapperServicesModel db, out List<InputParams> inputParams)
        {
            string xmlTemplate = string.Empty;
            using (AtiWrapperServicesModel dbContext = new AtiWrapperServicesModel())
            {

                string xmlTemplateLocation = ConfigurationManager.AppSettings["XMLTemplateDrive"] + Path.DirectorySeparatorChar + data.ServiceMethodXmlTemplateLocation;
                if (!Path.HasExtension(data.ServiceMethodXmlTemplateLocation))
                {
                    xmlTemplateLocation = xmlTemplateLocation + ".xml";
                }

                using (System.IO.StreamReader xmlTemplateFile = new StreamReader(xmlTemplateLocation))
                {
                    xmlTemplate = xmlTemplateFile.ReadToEnd();
                }

                inputParams = (from p in parameterModel.XmlServiceMethodTypes
                               where p.IsSystemType == true && p.Include == true
                               select new InputParams { Name = p.Name, TypeName = p.TypeName }).ToList();

                string xmlSignatureType = string.Empty;
                if (inputParams.Count > 0)
                {
                    xmlSignatureType = inputParams[0].Name.Split('.').First();
                }

                
                ServiceMethod serviceMethod = new ServiceMethod
                {
                    Name = data.ServiceMethod,
                    ServiceName_Id = existingServiceName.Id,
                    XmlTemplate = xmlTemplate,
                    XmlTypeSignature = xmlSignatureType,
                };

                dbContext.Add(serviceMethod);
                return serviceMethod;

            }
        }

        [HttpGet]
        public ActionResult TestServiceMethod()
        {
            using (AtiWrapperServicesModel dbContext = new AtiWrapperServicesModel())
            {
                CustomerServiceXmlViewModel data = TempData["CustomerServiceXmlViewModel"] as CustomerServiceXmlViewModel;
                TempData["CustomerXmlServiceUrlViewModel"] = data;

                XmlServiceMethodTestViewModel testData = new XmlServiceMethodTestViewModel();
                testData.MethodParameters = new List<ParameterViewModel>();
                testData.CustomerName = data.CustomerName;
                testData.ServiceName = data.ServiceName;
                testData.ServiceMethod = data.ServiceMethod;

                var parameterList = (from c in dbContext.Customers
                                     join u in dbContext.ServiceURLs on c.Id equals u.Customer.Id
                                     join n in dbContext.ServiceNames on u.Id equals n.ServiceURL.Id
                                     join m in dbContext.ServiceMethods on n.Id equals m.ServiceName.Id
                                     join i in dbContext.InputParamaters on m.Id equals i.ServiceMethod.Id
                                     join t in dbContext.ServiceTypes on i.Id equals t.InputParamater.Id
                                     join q in dbContext.QualifiedNames on t.QualifiedName.Id equals q.Id
                                     orderby i.Order
                                     where c.Name == data.CustomerName && n.Name == data.ServiceName && m.Name == data.ServiceMethod
                                     select new { Name = q.Name, TypeName = t.TypeName }).ToList();
                foreach (var p in parameterList)
                {
                    ParameterViewModel pData = new ParameterViewModel();
                    pData.Name = p.Name;
                    pData.TypeName = p.TypeName;
                    if (p.TypeName == typeof(string).FullName)
                    {
                        pData.Value = pData.Value as string;
                    }
                    else
                    {
                        Type parameterType = Type.GetType(p.TypeName, false);
                        if (parameterType != null)
                        {
                            if (parameterType.IsArray)
                            {
                                //parameter.Value = Activator.CreateInstance(parameterType, new object[] { 1 });
                                pData.Value = Activator.CreateInstance(parameterType.GetElementType());
                            }
                            else
                            {
                                pData.Value = Activator.CreateInstance(parameterType);
                            }
                        }
                    }
                    testData.MethodParameters.Add(pData);
                }
                return View("XmlServiceMethodTestView", testData);
            }
        }


        [HttpPost]
        public ActionResult TestServiceMethod(XmlServiceMethodTestViewModel model)
        {
            CustomerServiceXmlViewModel data = TempData["CustomerServiceXmlViewModel"] as CustomerServiceXmlViewModel;
            TempData["CustomerXmlServiceUrlViewModel"] = data;

            List<XmlServiceParameters> theParameters = new List<XmlServiceParameters>();
            foreach (var p in model.MethodParameters)
            {
                theParameters.Add(new XmlServiceParameters { Name = p.Name, TypeName = p.TypeName });
            }
            DynamicServiceProxyFactoryOptions options = new DynamicServiceProxyFactoryOptions();
            options.ServiceProxyType = DynamicServiceProxyFactoryOptions.ServiceProxyTypes.Xml;
            DynamicServiceProxyFactory serviceFactory = new DynamicServiceProxyFactory(data.TestUrl, data.ServiceName, data.ServiceMethod,
                model.MethodParameters[0].Name.Split('.').First(), options, theParameters, Assembly.GetExecutingAssembly().ToString());

            DynamicServiceProxy dp = serviceFactory.CreateProxy(data.ServiceName + "_" + data.ServiceMethod);

            object[] methodParameters = new object[model.MethodParameters.Count];
            for (int i = 0; i < model.MethodParameters.Count; i++)
            {
                methodParameters[i] = model.MethodParameters[i].Value;
            }

            object result = dp.CallMethod(data.ServiceName + "_" + data.ServiceMethod + "_Proxy", methodParameters);

            //string proxyName = data.ServiceName + "_" + data.ServiceMethod;

            //string code = CreateProxyForTypes(proxyName, model.MethodParameters[0].Name.Split('.').First(), data.TestUrl, model.MethodParameters);

            //CompilerResults compilerResults = CompileSrcipt(code);
            //if (compilerResults.Errors.HasErrors)
            //{
            //    throw new InvalidOperationException("Expression has a syntax error.");

            //}

            //Assembly assembly = compilerResults.CompiledAssembly;
            //MethodInfo method = assembly.GetType(proxyName).GetMethod(proxyName + "_Proxy");

            //object[] methodParameters = new object[model.MethodParameters.Count];
            //for (int i = 0; i < model.MethodParameters.Count; i++)
            //{
            //    methodParameters[i] = model.MethodParameters[i].Value;
            //}

            //object proxyObject = assembly.GetType(proxyName).GetConstructor(Type.EmptyTypes).Invoke(null);
            //object result = method.Invoke(proxyObject, methodParameters);
            MethodResult mr = new MethodResult();             
               
            mr.result = result.ToString();


            return View("ServiceMethodResultView", mr);
        }

        public static CompilerResults CompileSrcipt(string code)
        {
            CompilerParameters parms = new CompilerParameters();

            parms.GenerateExecutable = false;
            parms.GenerateInMemory = true;
            parms.IncludeDebugInformation = false;
            parms.ReferencedAssemblies.Add(Assembly.GetExecutingAssembly().Location);
            parms.ReferencedAssemblies.Add("System.dll");
            parms.ReferencedAssemblies.Add("mscorlib.dll");
            parms.ReferencedAssemblies.Add("System.Xml.dll");


            CodeDomProvider compiler = CSharpCodeProvider.CreateProvider("CSharp");

            return compiler.CompileAssemblyFromSource(parms, code);


        }

        private static string CreateProxyForTypes(string qualifiedObjectName, string xmlEnvelopeName, string url, List<ParameterViewModel> yourListOfFields)
        {
            StringBuilder proxyBuilder = new StringBuilder();
            proxyBuilder.Append(string.Format("public class {0} {{ public string {0}_Proxy (", qualifiedObjectName));
            int i = 0;
            foreach (var field in yourListOfFields)
            {
                proxyBuilder.Append(string.Format("{0} {1}", Type.GetType(field.TypeName), field.Name.Split('.').Last()));
                i++;
                if (i < yourListOfFields.Count)
                {
                    proxyBuilder.Append(", ");
                }
            }
            proxyBuilder.Append(@") { System.Collections.Generic.List<ServiceData.Models.ParameterViewModel> myListOfTypes = new System.Collections.Generic.List<ServiceData.Models.ParameterViewModel>(); ");
            foreach (var field in yourListOfFields)
            {
                proxyBuilder.Append(@"myListOfTypes.Add( new ServiceData.Models.ParameterViewModel { Name = """);
                proxyBuilder.Append(string.Format(@"{0}", field.Name.Split('.').Last()));
                proxyBuilder.Append(@""", TypeName = """);
                proxyBuilder.Append(string.Format("{0}", field.TypeName));
                proxyBuilder.Append(@""" });");
            }
            //proxyBuilder.Append(@"System.Reflection.Assembly zoo = System.Reflection.Assembly.GetCallingAssembly(); throw new System.Exception(zoo.FullName);");
            proxyBuilder.Append(@" System.Type typeBuilderType = System.Reflection.Assembly.GetCallingAssembly().GetType(""ServiceData.Controllers.XmlServiceTypeBuilder"");");
            proxyBuilder.Append(@"if(typeBuilderType == null) throw new System.Exception(""typeBuilderType is null"");");
            proxyBuilder.Append(@"object typeBuilderObject = System.Activator.CreateInstance(typeBuilderType, new object[] { myListOfTypes, ");
            proxyBuilder.Append(string.Format(@" ""{0}"", ""{1}""", qualifiedObjectName, xmlEnvelopeName));
            proxyBuilder.Append(@" }); object proxyObject = typeBuilderObject.GetType().InvokeMember(""CreateNewObject"", System.Reflection.BindingFlags.Public | ");
            proxyBuilder.Append(@"System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.InvokeMethod, null, typeBuilderObject, null);");

            foreach (var field in yourListOfFields)
            {
                proxyBuilder.Append(string.Format(@"proxyObject.GetType().InvokeMember(""{0}""", field.Name.Split('.').Last()));
                proxyBuilder.Append(", System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.SetProperty,");
                proxyBuilder.Append(string.Format("null, proxyObject, new object[] {{ {0} }});", field.Name.Split('.').Last()));

            }


            proxyBuilder.Append("System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(proxyObject.GetType());");
            proxyBuilder.Append("System.Xml.Serialization.XmlSerializerNamespaces serializerNamespace = new System.Xml.Serialization.XmlSerializerNamespaces();");
            proxyBuilder.Append(@"serializerNamespace.Add("""", """");");

            proxyBuilder.Append("System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();");
            proxyBuilder.Append("using (System.Xml.XmlWriter writer = System.Xml.XmlWriter.Create(memoryStream, new System.Xml.XmlWriterSettings ");
            proxyBuilder.Append(@"{ OmitXmlDeclaration = true, Encoding = System.Text.ASCIIEncoding.ASCII, Indent = true })){");
            proxyBuilder.Append(@"serializer.Serialize(writer, proxyObject, serializerNamespace); }");

            proxyBuilder.Append("System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };");

            proxyBuilder.Append(string.Format(@"System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(""{0}"");", url));
            proxyBuilder.Append(@"req.KeepAlive = false; req.ProtocolVersion = System.Net.HttpVersion.Version10; req.Method = ""POST""; req.ContentType = ""application/x-www-form-urlencoded"";");

            proxyBuilder.Append("byte[] postBytes = memoryStream.ToArray(); req.ContentLength = postBytes.Length;");
            proxyBuilder.Append("System.IO.Stream requestStream = req.GetRequestStream(); requestStream.Write(postBytes, 0, postBytes.Length); requestStream.Close();");

            proxyBuilder.Append("System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)req.GetResponse(); System.IO.Stream resStream = response.GetResponseStream();");
            proxyBuilder.Append("System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream()); string responseText = sr.ReadToEnd(); sr.Close();");

            proxyBuilder.Append(@"return responseText;}}");
            return proxyBuilder.ToString();
        }
        //
        // GET: /CustomerXmlServiceUrl/Edit/5

        public ActionResult Edit(int id)
        {

            return View();
        }

        //
        // POST: /CustomerXmlServiceUrl/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /CustomerXmlServiceUrl/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /CustomerXmlServiceUrl/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        

        public void BuildXmlServiceMethodPerameterTree(XmlServiceMethodParameterViewModel smp, TypeInfo typeInfo, string name, string prefix)
        {
            XmlServiceMethodTypeViewModel smt = new XmlServiceMethodTypeViewModel();
            if(typeInfo.FullName.Equals("System.String"))
            {
                smt.SupportedTypes = new List<SelectListItem>()
                {
                    new SelectListItem { Text = "System.String", Value = "System.String", Selected = true},
                    new SelectListItem { Text = "System.Int32", Value = "System.Int32" },
                    new SelectListItem { Text = "System.Decimal", Value = "System.Decimal"},
                    new SelectListItem { Text = "System.DateTime", Value = "System.DateTime" },
                    new SelectListItem { Text = "System.TimeSpan", Value = "System.TimeSpan" },
                    new SelectListItem { Text = "System.Boolean", Value = "System.Boolean" },
                    new SelectListItem { Text = "System.Single", Value = "System.Single" },
                };
                smt.Name = string.IsNullOrEmpty(prefix)? name : prefix + "." + name;
                smt.TypeName = typeInfo.FullName;
                //smt.TypeName = string.Empty;
                smt.IsSystemType = true;
                smp.XmlServiceMethodTypes.Add(smt);
            }
            else
            {
                smt.Name = string.IsNullOrEmpty(prefix) ? name : prefix + "." + name;
                smt.TypeName = typeInfo.FullName;
                //smt.TypeName = string.Empty;
                smt.IsSystemType = false;
                smp.XmlServiceMethodTypes.Add(smt);
                foreach (PropertyInfo p in typeInfo.DeclaredProperties)
                {
                    BuildXmlServiceMethodPerameterTree(smp, p.PropertyType.GetTypeInfo(), p.Name, string.IsNullOrEmpty(prefix) ? name : prefix + "." + name);
                }
            }
        }

        public void RollBackServiceMethod(CustomerServiceXmlViewModel data, AtiWrapperServicesModel db)
        {
            //delete the records
            using (AtiWrapperServicesModel dbContext = new AtiWrapperServicesModel())
            {
                var deleteQualifiedNames = (from c in dbContext.Customers
                                            join u in dbContext.ServiceURLs on c.Id equals u.Customer.Id
                                            join n in dbContext.ServiceNames on u.Id equals n.ServiceURL.Id
                                            join m in dbContext.ServiceMethods on n.Id equals m.ServiceName.Id
                                            join i in dbContext.InputParamaters on m.Id equals i.ServiceMethod.Id
                                            join t in dbContext.ServiceTypes on i.Id equals t.InputParamater.Id
                                            join q in dbContext.QualifiedNames on t.QualifiedName.Id equals q.Id
                                            where c.Name == data.CustomerName && n.Name == data.ServiceName && m.Name == data.ServiceMethod
                                            select q).ToList();

                foreach (var qualifiedName in deleteQualifiedNames)
                {
                    dbContext.Delete(qualifiedName);
                }


                var deleteServiceTypes = (from c in dbContext.Customers
                                          join u in dbContext.ServiceURLs on c.Id equals u.Customer.Id
                                          join n in dbContext.ServiceNames on u.Id equals n.ServiceURL.Id
                                          join m in dbContext.ServiceMethods on n.Id equals m.ServiceName.Id
                                          join i in dbContext.InputParamaters on m.Id equals i.ServiceMethod.Id
                                          join t in dbContext.ServiceTypes on i.Id equals t.InputParamater.Id
                                          where c.Name == data.CustomerName && n.Name == data.ServiceName && m.Name == data.ServiceMethod
                                          select t).ToList();

                foreach (var serviceType in deleteServiceTypes)
                {
                    dbContext.Delete(serviceType);
                }


                var deleteInputParameters = (from c in dbContext.Customers
                                             join u in dbContext.ServiceURLs on c.Id equals u.Customer.Id
                                             join n in dbContext.ServiceNames on u.Id equals n.ServiceURL.Id
                                             join m in dbContext.ServiceMethods on n.Id equals m.ServiceName.Id
                                             join i in dbContext.InputParamaters on m.Id equals i.ServiceMethod.Id
                                             where c.Name == data.CustomerName && n.Name == data.ServiceName && m.Name == data.ServiceMethod
                                             select i).ToList();

                foreach (var inputParameter in deleteInputParameters)
                {
                    dbContext.Delete(inputParameter);
                }

                var deleteReturnQualifiedNames = (from c in dbContext.Customers
                                                  join u in dbContext.ServiceURLs on c.Id equals u.Customer.Id
                                                  join n in dbContext.ServiceNames on u.Id equals n.ServiceURL.Id
                                                  join m in dbContext.ServiceMethods on n.Id equals m.ServiceName.Id
                                                  join r in dbContext.ReturnTypes on m.Id equals r.ServiceMethod_Id
                                                  join t in dbContext.ServiceTypes on r.Id equals t.ReturnType.Id
                                                  join q in dbContext.QualifiedNames on t.QualifiedName.Id equals q.Id
                                                  where c.Name == data.CustomerName && n.Name == data.ServiceName && m.Name == data.ServiceMethod
                                                  select q).ToList();

                foreach (var qualifiedName in deleteReturnQualifiedNames)
                {
                    dbContext.Delete(qualifiedName);
                }

                var deleteReturnServiceTypes = (from c in dbContext.Customers
                                                join u in dbContext.ServiceURLs on c.Id equals u.Customer.Id
                                                join n in dbContext.ServiceNames on u.Id equals n.ServiceURL.Id
                                                join m in dbContext.ServiceMethods on n.Id equals m.ServiceName.Id
                                                join r in dbContext.ReturnTypes on m.Id equals r.ServiceMethod_Id
                                                join t in dbContext.ServiceTypes on r.Id equals t.ReturnType.Id
                                                where c.Name == data.CustomerName && n.Name == data.ServiceName && m.Name == data.ServiceMethod
                                                select t).ToList();

                foreach (var serviceType in deleteReturnServiceTypes)
                {
                    dbContext.Delete(serviceType);
                }
               
                var deleteReturnTypes = (from c in dbContext.Customers
                                         join u in dbContext.ServiceURLs on c.Id equals u.Customer.Id
                                         join n in dbContext.ServiceNames on u.Id equals n.ServiceURL.Id
                                         join m in dbContext.ServiceMethods on n.Id equals m.ServiceName.Id
                                         join r in dbContext.ReturnTypes on m.Id equals r.ServiceMethod_Id
                                         where c.Name == data.CustomerName && n.Name == data.ServiceName && m.Name == data.ServiceMethod
                                         select r).ToList();

                foreach (var returnType in deleteReturnTypes)
                {
                    dbContext.Delete(returnType);
                }

                var deleteServiceMethod = (from c in dbContext.Customers
                                                       join u in dbContext.ServiceURLs on c.Id equals u.Customer.Id
                                                       join n in dbContext.ServiceNames on u.Id equals n.ServiceURL.Id
                                                       join m in dbContext.ServiceMethods on n.Id equals m.ServiceName.Id
                                                       where c.Name == data.CustomerName && n.Name == data.ServiceName && m.Name == data.ServiceMethod
                                                       select m).ToList();
                if (deleteServiceMethod.Count != 1)
                {
                    throw new Exception(string.Format("More than One method: {0}, Service: {1}, Customer: {2}", data.ServiceMethod, data.ServiceName, data.CustomerName));
                }

                var deleteServiceName = (from c in dbContext.Customers
                                                   join u in dbContext.ServiceURLs on c.Id equals u.Customer.Id
                                                   join n in dbContext.ServiceNames on u.Id equals n.ServiceURL.Id
                                                   join m in dbContext.ServiceMethods on n.Id equals m.ServiceName.Id
                                                   where c.Name == data.CustomerName && n.Name == data.ServiceName && m.Name == data.ServiceMethod
                                                   select n).ToList();
                if (deleteServiceName.Count != 1)
                {
                    throw new Exception(string.Format("More than One Service: {0}, Service: {1}, Customer: {2}", data.ServiceMethod, data.ServiceName, data.CustomerName));
                }

                var deleteServiceUrl = (from c in dbContext.Customers
                                                 join u in dbContext.ServiceURLs on c.Id equals u.Customer.Id
                                                 join n in dbContext.ServiceNames on u.Id equals n.ServiceURL.Id
                                                 join m in dbContext.ServiceMethods on n.Id equals m.ServiceName.Id
                                                 where c.Name == data.CustomerName && n.Name == data.ServiceName && m.Name == data.ServiceMethod
                                                 select u).ToList();
                if (deleteServiceUrl.Count != 1)
                {
                    throw new Exception(string.Format("More than One Url: {0}, Service: {1}, Customer: {2}", data.ServiceMethod, data.ServiceName, data.CustomerName));
                }

                var deleteCustomer = (from c in dbContext.Customers
                                             join u in dbContext.ServiceURLs on c.Id equals u.Customer.Id
                                             join n in dbContext.ServiceNames on u.Id equals n.ServiceURL.Id
                                             join m in dbContext.ServiceMethods on n.Id equals m.ServiceName.Id
                                             where c.Name == data.CustomerName && n.Name == data.ServiceName && m.Name == data.ServiceMethod
                                             select c).ToList();
                if (deleteCustomer.Count != 1)
                {
                    throw new Exception(string.Format("More than One Customer: {0}, Service: {1}, Customer: {2}", data.ServiceMethod, data.ServiceName, data.CustomerName));
                }
                
                dbContext.Delete(deleteServiceMethod[0]);
                dbContext.Delete(deleteServiceName[0]);
                dbContext.Delete(deleteServiceUrl[0]);
                dbContext.Delete(deleteCustomer[0]);
                dbContext.SaveChanges();
               
            }
        }

    }

    public class XmlServiceTypeBuilder
    {
        public List<ParameterViewModel> parameters;
        public string qualifiedObjectName;
        string xmlEnvelopeName;

        public XmlServiceTypeBuilder(List<ParameterViewModel> parameters, string qualifiedObjectName, string xmlEnvelopeName)
        {
            this.parameters = parameters;
            this.qualifiedObjectName = qualifiedObjectName;
            this.xmlEnvelopeName = xmlEnvelopeName;
        }

        public object CreateNewObject()
        {
            var myType = CompileResultType();
            return Activator.CreateInstance(myType);
        }
        public Type CompileResultType()
        {
            TypeBuilder tb = GetTypeBuilder(this.xmlEnvelopeName, this.qualifiedObjectName);
            ConstructorBuilder constructor = tb.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);

            // NOTE: assuming your list contains Field objects with fields FieldName(string) and FieldType(Type)
            foreach (var field in parameters)
                CreateProperty(tb, field.Name.Split('.').Last(), Type.GetType(field.TypeName));

            Type objectType = tb.CreateType();
            return objectType;
        }

        private static TypeBuilder GetTypeBuilder(string typeSignature, string methodName)
        {
            var an = new AssemblyName(methodName + "_" + typeSignature);
            AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(an, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
            TypeBuilder tb = moduleBuilder.DefineType(typeSignature
                                , TypeAttributes.Public |
                                TypeAttributes.Class |
                                TypeAttributes.AutoClass |
                                TypeAttributes.AnsiClass |
                                TypeAttributes.BeforeFieldInit |
                                TypeAttributes.AutoLayout
                                , null);
            return tb;
        }

        private static void CreateProperty(TypeBuilder tb, string propertyName, Type propertyType)
        {
            FieldBuilder fieldBuilder = tb.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);

            PropertyBuilder propertyBuilder = tb.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);
            MethodBuilder getPropMthdBldr = tb.DefineMethod("get_" + propertyName, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, propertyType, Type.EmptyTypes);
            ILGenerator getIl = getPropMthdBldr.GetILGenerator();

            getIl.Emit(OpCodes.Ldarg_0);
            getIl.Emit(OpCodes.Ldfld, fieldBuilder);
            getIl.Emit(OpCodes.Ret);

            MethodBuilder setPropMthdBldr =
                tb.DefineMethod("set_" + propertyName,
                  MethodAttributes.Public |
                  MethodAttributes.SpecialName |
                  MethodAttributes.HideBySig,
                  null, new[] { propertyType });

            ILGenerator setIl = setPropMthdBldr.GetILGenerator();
            Label modifyProperty = setIl.DefineLabel();
            Label exitSet = setIl.DefineLabel();

            setIl.MarkLabel(modifyProperty);
            setIl.Emit(OpCodes.Ldarg_0);
            setIl.Emit(OpCodes.Ldarg_1);
            setIl.Emit(OpCodes.Stfld, fieldBuilder);

            setIl.Emit(OpCodes.Nop);
            setIl.MarkLabel(exitSet);
            setIl.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getPropMthdBldr);
            propertyBuilder.SetSetMethod(setPropMthdBldr);
        }
    }
}
