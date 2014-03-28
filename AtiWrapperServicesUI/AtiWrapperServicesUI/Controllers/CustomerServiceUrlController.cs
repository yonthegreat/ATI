using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AtiWrapperServicesUI.Models;
using System.IO;
using System.Text;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using DynamicServiceProxyNamespace;
using System.ServiceModel.Description;
using System.Xml.Serialization;
using System.ServiceModel.Channels;
using System.ServiceModel;
using CustomMessageHeaders.CustomSoapHeaders;
using CustomMessageHeaders;
using AtiWrapperServicesORM.OpenAccess;
using AtiWrapperServices.AtiWrapperServicesEnums;
using System.Diagnostics;


namespace AtiWrapperServicesUI.Controllers
{
    public class CustomerServiceUrlController : Controller
    {
        private const string CUSTOMSOAPHEADERSNAMESPACE = "CustomMessageHeaders.CustomSoapHeaders";

        
        private DynamicServiceProxyFactory serviceFactory;
        private DynamicServiceProxy serviceProxy;
        private readonly Assembly customMessageHeadersAssembly = typeof(HeaderDynamicProperty).Assembly;
        static public TraceSource TraceSource = new TraceSource("AtiServiceDataUI");
        //
        // GET: /CustomerServiceUrl/
        public static T Cast<T>(object o, T[] x)
        {
            T foo = (T)o;
            x[0] = (T)o;
            return (T)o;
        }

        private CustomerServiceUrlViewModel myModel;

        [HttpGet]
        public ActionResult Create()
        {
            if (TempData["Data"] == null)
            {
                myModel = new CustomerServiceUrlViewModel();
                myModel.EndpointAddressModifierOption = DynamicServiceProxyFactoryOptions.EndpointAddressModifierOptions.UseMetadataAddress;
            }
            else
            {
                myModel = TempData["Data"] as CustomerServiceUrlViewModel;
            }

            if (TempData["ServiceFactory"] != null)
            {
                serviceFactory = TempData["ServiceFactory"] as DynamicServiceProxyFactory;
            }

            var selectCustomSoapHeaders = (from h in customMessageHeadersAssembly.GetTypes()
                                           where h.IsClass
                                           select new SelectListItem { Text = h.Name, Value = h.Name });
            
            myModel.CustomSoapHeaderList = new SelectList(selectCustomSoapHeaders, "Value", "Text");

           

            if (myModel.TestUseSource.Equals(UseSource.Wsdl))
            {
                GetServiceNamesFromWsdl(myModel.TestUrl);
            }

            if (myModel.ServiceName != null)
            {
                GetServiceMethods(myModel.ServiceName);
            }

            return View(myModel);
        }

        [HttpPost]
        public ActionResult Create(CustomerServiceUrlViewModel data, string command)
        {
            myModel = data;
            Trace.CorrelationManager.ActivityId = Guid.NewGuid();
            Trace.CorrelationManager.StartLogicalOperation("CustomerServiceUrlController Create");
            TraceSource.TraceEvent(TraceEventType.Start, 1000, "CustomerServiceUrlController Create Post");
            
            if (command == "FlattenReturn")
            {
                //TempData["Data"] = data;
                //myModel.FlatReturnType = wsdlWebService.GetFlatReturnTypes(myModel.ServiceMethod);
                //TempData["Data"] = myModel;
                //return RedirectToAction("ReturnTypeTable");
                TraceSource.TraceEvent(TraceEventType.Stop, 2001);
            }
            else if (command == "FlattenParameters")
            {
                //TempData["Data"] = data;
                //myModel.FlatParameters = wsdlWebService.GetFlatParameterTypes(myModel.ServiceMethod);
                //TempData["Data"] = myModel;
                //return RedirectToAction("ParameterTypeTable");
                TraceSource.TraceEvent(TraceEventType.Stop, 2002);
            }
            else if (command == "Test")
            {
                TempData["Data"] = data;
                TraceSource.TraceEvent(TraceEventType.Stop, 2003);
                return RedirectToAction("TestServiceMethod");
            }
            else if (command == "ChangeAddress")
            {
                data.EndpointAddressModifierOption = DynamicServiceProxyFactoryOptions.EndpointAddressModifierOptions.MakeSameAsUrl;
                TempData["Data"] = data;
                DynamicServiceProxyFactoryOptions options = new DynamicServiceProxyFactoryOptions();
                options.EndpointAddress = DynamicServiceProxyFactoryOptions.EndpointAddressModifierOptions.MakeSameAsUrl;
                serviceFactory = new DynamicServiceProxyFactory(data.TestUrl + "?wsdl", options);
                TempData["ServiceFactory"] = serviceFactory;
                TraceSource.TraceEvent(TraceEventType.Stop, 2004);
                return RedirectToAction("Create");
            }
            else if (command == "Save")
            {
                TraceSource.TraceEvent(TraceEventType.Stop, 2005);
                TempData["Data"] = data;
                serviceProxy = TempData["ServiceProxy"] as DynamicServiceProxy;

                if (ModelState.IsValid)
                {
                    DynamicServiceProxyFactoryOptions options = new DynamicServiceProxyFactoryOptions();
                    options.EndpointAddress = DynamicServiceProxyFactoryOptions.EndpointAddressModifierOptions.MakeSameAsUrl;
                    serviceFactory = new DynamicServiceProxyFactory(data.TestUrl + "?wsdl", options);
                    SaveServiceMethodData(data, serviceFactory);
                }
            }

            var selectCustomSoapHeaders = (from h in customMessageHeadersAssembly.GetTypes()
                                           where h.IsClass
                                           select new SelectListItem { Text = h.Name, Value = h.Name });
            data.CustomSoapHeaderList = new SelectList(selectCustomSoapHeaders, "Value", "Text");
            TraceSource.TraceEvent(TraceEventType.Stop, 2006);
            return View(data);
        }

        private List<TypeData> GetInputParamaterList(CustomerServiceUrlViewModel data)
        {
            List<TypeData> inputParametersList = new List<TypeData>();
            MethodInfo methodInfo = serviceProxy.ProxyType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                                     .First(m => m.Name == data.ServiceMethod);

            foreach (ParameterInfo p in methodInfo.GetParameters())
            {
                GetParameterNameTypeList(inputParametersList, p.ParameterType.GetTypeInfo(), p.Name, String.Empty);
            }

            return inputParametersList;
        }

        private void GetParameterNameTypeList(List<TypeData> parameterNameTypeList, TypeInfo parameterTypeInfo, string name, string prefix)
        {
            if (parameterTypeInfo.FullName.Equals("System.Runtime.Serialization.ExtensionDataObject"))
            {
                return;
            }
            if (parameterTypeInfo.Namespace == "System")
            {
                if (parameterTypeInfo.IsByRef)
                {
                    parameterNameTypeList.Add(new TypeData { Name = String.IsNullOrEmpty(prefix) ? name : prefix + "." + name, Type = parameterTypeInfo.FullName.TrimEnd('&') });
                }
                else
                {
                    parameterNameTypeList.Add(new TypeData { Name = String.IsNullOrEmpty(prefix) ? name : prefix + "." + name, Type = parameterTypeInfo.ToString() });
                }
            }
            //else if (parameterTypeInfo.BaseType != null && (parameterTypeInfo.BaseType.Equals(typeof(Enum)) || 
            //    (parameterTypeInfo.BaseType.Namespace != null && parameterTypeInfo.BaseType.Namespace.StartsWith("System"))))
            else if (parameterTypeInfo.BaseType != null && (parameterTypeInfo.BaseType.Equals(typeof(Enum)) || parameterTypeInfo.BaseType.IsGenericType))
            {
                var test = serviceProxy.ProxyType.Assembly.GetType(parameterTypeInfo.FullName, false);
                parameterNameTypeList.Add(new TypeData { Name = String.IsNullOrEmpty(prefix) ? name : prefix + "." + name, Type = parameterTypeInfo.FullName });
            }
            
            else
            {
                if (!String.IsNullOrEmpty(name))
                {
                    prefix = String.IsNullOrEmpty(prefix) ? name : prefix + "." + name;
                }
                foreach (PropertyInfo prop in parameterTypeInfo.DeclaredProperties)
                {
                    if (prop.PropertyType.FullName.Equals("System.Runtime.Serialization.ExtensionDataObject"))
                    {
                        continue;
                    }
                    if (prop.PropertyType.Namespace == "System")
                    {
                        parameterNameTypeList.Add(new TypeData { Name = String.IsNullOrEmpty(prefix) ? prop.Name : prefix + "." + prop.Name, Type = prop.PropertyType.ToString() });
                    }
                    else
                    {
                        GetParameterNameTypeList(parameterNameTypeList, serviceProxy.ProxyType.Assembly.DefinedTypes.First(m => m.Name == prop.PropertyType.Name),
                            prop.PropertyType.Name, String.IsNullOrEmpty(prefix) ? prop.PropertyType.Name : prefix + "." + prop.PropertyType.Name);
                    }
                }
                
                if (parameterTypeInfo.IsArray)
                {
                    GetParameterNameTypeList(parameterNameTypeList, serviceProxy.ProxyType.Assembly.DefinedTypes.First(m => m.Name == parameterTypeInfo.GetElementType().Name),
                        string.Empty, String.IsNullOrEmpty(prefix) ? parameterTypeInfo.GetElementType().Name : prefix + "." + parameterTypeInfo.GetElementType().Name);
                }
                else if (!parameterTypeInfo.BaseType.Equals(typeof(System.Object)))
                {
                    GetParameterNameTypeList(parameterNameTypeList, serviceProxy.ProxyType.Assembly.DefinedTypes.First(m => m.Name == parameterTypeInfo.BaseType.Name),
                        string.Empty, String.IsNullOrEmpty(prefix) ? parameterTypeInfo.BaseType.Name : prefix + "." + parameterTypeInfo.BaseType.Name);
                }
            }
            
        }

        public void SaveServiceMethodData(CustomerServiceUrlViewModel data, DynamicServiceProxyFactory serviceFactory)
        {
            using (AtiWrapperServicesModel dbContext = new AtiWrapperServicesModel())
            {

                Customer customer = new Customer()
                {
                    Name = data.CustomerName,
                };
                dbContext.Add(customer);
                dbContext.SaveChanges();

                StringBuilder proxyWsdl = new StringBuilder();
                DynamicServiceProxy theProxy = serviceFactory.CreateProxy(data.ServiceName);
                DataContractSerializer ser = new DataContractSerializer(theProxy.ProxyType);
                using (MemoryStream ms = new MemoryStream())
                {
                    ser.WriteObject(ms, theProxy.Proxy);
                    ms.Position = 0;
                    StreamReader sr = new StreamReader(ms);
                    proxyWsdl.Append(sr.ReadToEnd());
                }
 
                ServiceURL serviceUrl = new ServiceURL()
                {
                    Customer_Id = customer.Id,
                    TestURL = data.TestUrl,
                    ProductionURL = data.ProductionUrl,
                    TestUseSource = data.TestUseSource,
                    TestWSDL = proxyWsdl.ToString(),
                    TestCode = serviceFactory.ProxyCode,
                    ProductionUseSource = data.ProductionUseSource,
                    ProductionWSDL = proxyWsdl.ToString(),
                    ProductionCode = serviceFactory.ProxyCode

                };
                dbContext.Add(serviceUrl);
                dbContext.SaveChanges();

                ServiceName serviceName = new ServiceName()
                {
                    ServiceURL_Id = serviceUrl.Id,
                    Name = data.ServiceName,
                    ServiceType = (int)DynamicServiceProxyFactoryOptions.ServiceProxyTypes.Wsdl,
                };
                dbContext.Add(serviceName);
                dbContext.SaveChanges();

                ServiceMethod serviceMethod = new ServiceMethod()
                {
                    Name = data.ServiceMethod,
                    ServiceName_Id = serviceName.Id,
                    EndpointAddressModifierOption = Convert.ToInt32(data.EndpointAddressModifierOption),
                };

                dbContext.Add(serviceMethod);
                dbContext.SaveChanges();

                MethodInfo methodInfo = serviceProxy.ProxyType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly).First(m => m.Name == data.ServiceMethod);

                ParameterInfo[] methodParameters = methodInfo.GetParameters();

                for (int i = 0; i < methodParameters.Length; i++)
                {
                    InputParamater inputParameter = new InputParamater()
                    {
                        ServiceMethod_Id = serviceMethod.Id,
                        Order = i,

                    };
                    dbContext.Add(inputParameter);
                    dbContext.SaveChanges();

                    SaveInputParameterTypes(inputParameter, methodParameters[i], String.Empty, null);
                }

                if (methodInfo.ReturnType != null)
                {
                    ReturnType returnType = new ReturnType()
                    {
                        ServiceMethod_Id = serviceMethod.Id
                    };
                    dbContext.Add(returnType);
                    dbContext.SaveChanges();

                    SaveReturnType(returnType, methodInfo.ReturnType.GetTypeInfo(), serviceMethod.Name);
                }

                if (data.CustomSoapHeaderName != null)
                {
                    ServiceMessageHeader messageHeader = new ServiceMessageHeader()
                    {
                        ServiceMethod_Id = serviceMethod.Id,
                        Order = 0,
                        TypeName = data.CustomSoapHeaderName,
                        AssemblyName = customMessageHeadersAssembly.FullName,
                    };

                    dbContext.Add(messageHeader);
                    dbContext.SaveChanges();

                    var q = (from t in customMessageHeadersAssembly.GetType(CUSTOMSOAPHEADERSNAMESPACE + "." + data.CustomSoapHeaderName).GetTypeInfo().DeclaredProperties
                             where t.GetCustomAttribute(typeof(HeaderDynamicProperty), false) != null
                             select new TypeData { Name = t.Name, Type = t.PropertyType.FullName });
                    List<TypeData> customSoapHeaderFieldsList = new List<TypeData>(q);


                    foreach (var p in customSoapHeaderFieldsList)
                    {
                        QualifiedName qualifiedName = new QualifiedName()
                        {
                            Name = data.CustomSoapHeaderName + "." + p.Name,
                        };
                        ServiceType serviceType = new ServiceType
                        {
                            ServiceMessageHeader_Id = messageHeader.Id,
                            IsCollectionType = false,
                            IsSystemType = true,
                            IsEnum = false,
                            Name = p.Name,
                            TypeName = p.Type,
                            QualifiedName_Id = qualifiedName.Id,
                        };
                        dbContext.Add(qualifiedName);
                        dbContext.Add(serviceType);
                        dbContext.SaveChanges();
                    }
                }
            }
        }

        private void SaveReturnType(ReturnType returnType, TypeInfo typeInfo, string methodName)
        {
            SaveSubtypes(null, returnType, typeInfo, String.Empty, methodName, null);
        }

        private void SaveInputParameterTypes(InputParamater inputParameter, ParameterInfo pInfo, string prefix, int? parentId)
        {

            if (pInfo.ParameterType.Namespace == "System")
            {
                SaveSubtypes(inputParameter, null, pInfo.ParameterType.GetTypeInfo(), String.Empty, pInfo.Name, parentId);
            }
            else if (pInfo.ParameterType.IsArray)
            {
                SaveSubtypes(inputParameter, null, pInfo.ParameterType.GetTypeInfo(), pInfo.Name, String.Empty, parentId);
            }
            else
            {
                SaveSubtypes(inputParameter, null, serviceProxy.ProxyType.Assembly.DefinedTypes.First(m => m.Name == pInfo.ParameterType.Name), String.Empty, pInfo.Name, parentId);
            }

        }
        
        private void SaveSubtypes(InputParamater inputParameter, ReturnType returnType, TypeInfo typeInfo, string prefix, string name, int? parentId)
        {
            using (AtiWrapperServicesModel dbContext = new AtiWrapperServicesModel())
            { 
                if (typeInfo.FullName.Equals("System.Runtime.Serialization.ExtensionDataObject"))
                {
                    return;
                }

                ServiceType serviceType = new ServiceType()
                {
                    ParentId = parentId,
                    Name = String.IsNullOrEmpty(name) ? prefix.Split('.').Last() : name,
                    TypeName = typeInfo.FullName,
                    IsSystemType = false,
                    IsCollectionType = false,
                    IsEnum = false,
                };
                if (inputParameter == null)
                {
                    serviceType.InputParamater_Id = null;
                }
                else
                {
                    serviceType.InputParamater_Id = inputParameter.Id;
                }
                if (returnType == null)
                {
                    serviceType.ReturnType_Id = null;
                }
                else
                {
                    serviceType.ReturnType_Id = returnType.Id;
                }
                

                if (typeInfo.Namespace == "System")
                {
                    serviceType.IsSystemType = true;
                }
                if (typeInfo.IsArray)
                {
                    serviceType.IsCollectionType = true;
                    // change this to a non-system type so that elements are used
                    serviceType.IsSystemType = false;
                }
                if (typeInfo.BaseType != null && typeInfo.BaseType.IsGenericType && typeInfo.BaseType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    serviceType.IsCollectionType = true;
                }
                if (typeInfo.IsByRef)
                {
                    serviceType.TypeName = typeInfo.FullName.TrimEnd('&');
                }
                if (typeInfo.BaseType != null && (typeInfo.BaseType.Equals(typeof(Enum))))
                {
                    serviceType.IsEnum = true;
                }

            
                QualifiedName qualifiedName = new QualifiedName();
                if (!String.IsNullOrEmpty(name))
                {
                    qualifiedName.Name = String.IsNullOrEmpty(prefix) ? name : prefix + "." + name;
                    prefix = String.IsNullOrEmpty(prefix) ? name : prefix + "." + name;
                }
                else
                {
                    qualifiedName.Name = prefix;
                }

                dbContext.Add(qualifiedName);
                dbContext.SaveChanges();
                serviceType.QualifiedName_Id = qualifiedName.Id;
                dbContext.Add(serviceType);
                dbContext.SaveChanges();
                dbContext.MakeDirty(serviceType, "Name");
                parentId = serviceType.Id;
                dbContext.SaveChanges();

                if (typeInfo.IsArray)
                {
                    SaveSubtypes(inputParameter, returnType, typeInfo.GetElementType().GetTypeInfo(), prefix, "Element", parentId);
                }
                else if (typeInfo.BaseType != null && typeInfo.BaseType.IsGenericType && typeInfo.BaseType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    SaveSubtypes(inputParameter, returnType, typeInfo.BaseType.GetGenericArguments().Single().GetTypeInfo(), prefix, "Element", parentId);
                }
                else if (typeInfo.Namespace != "System" && !serviceType.IsEnum )
                {

                    foreach (PropertyInfo prop in typeInfo.DeclaredProperties)
                    {
                        if (prop.PropertyType.FullName.Equals("System.Runtime.Serialization.ExtensionDataObject"))
                        {
                            continue;
                        }
                        if (null == prop.PropertyType.Namespace || !prop.PropertyType.Namespace.StartsWith("System"))
                        {
                            string newName;
                            if (prop.PropertyType.IsArray)
                            {
                                newName = prop.PropertyType.GetElementType().Name;
                                qualifiedName = new QualifiedName()
                                {
                                    Name = String.IsNullOrEmpty(prefix) ? prop.Name : prefix + "." + prop.Name,
                                };
                                dbContext.Add(qualifiedName);
                                dbContext.SaveChanges();

                                serviceType = new ServiceType()
                                {
                                    ParentId = parentId,
                                    Name = prop.Name,
                                    TypeName = prop.PropertyType.FullName,
                                    IsSystemType = false,
                                    IsCollectionType = true,
                                    IsEnum = false,
                                    QualifiedName_Id = qualifiedName.Id,
                                };
                                if (inputParameter == null)
                                {
                                    serviceType.InputParamater_Id = null;
                                }
                                else
                                {
                                    serviceType.InputParamater_Id = inputParameter.Id;
                                }
                                if (returnType == null)
                                {
                                    serviceType.ReturnType_Id = null;
                                }
                                else
                                {
                                    serviceType.ReturnType_Id = returnType.Id;
                                }
                                dbContext.Add(serviceType);
                                dbContext.SaveChanges();
                                dbContext.MakeDirty(serviceType, "Name");
                                parentId = serviceType.Id;
                                dbContext.SaveChanges();
                            }
                            else
                            {
                                newName = prop.PropertyType.Name;
                            }
                            SaveSubtypes(inputParameter, returnType, serviceProxy.ProxyType.Assembly.DefinedTypes.First(m => m.Name == newName), String.IsNullOrEmpty(prefix) ? prop.Name : prefix + "." + prop.Name, String.Empty, parentId);
                        }
                        else
                        {
                            qualifiedName = new QualifiedName()
                            {
                                Name = String.IsNullOrEmpty(prefix) ? prop.Name : prefix + "." + prop.Name,
                            };
                            dbContext.Add(qualifiedName);
                            dbContext.SaveChanges();

                            serviceType = new ServiceType()
                            {
                                ParentId = parentId,
                                Name = prop.Name,
                                TypeName = prop.PropertyType.FullName,
                                IsSystemType = true,
                                IsCollectionType = false,
                                IsEnum = false,
                                QualifiedName_Id = qualifiedName.Id,
                            };
                            if (inputParameter == null)
                            {
                                serviceType.InputParamater_Id = null;
                            }
                            else
                            {
                                serviceType.InputParamater_Id = inputParameter.Id;
                            }
                            if (returnType == null)
                            {
                                serviceType.ReturnType_Id = null;
                            }
                            else
                            {
                                serviceType.ReturnType_Id = returnType.Id;
                            }
                            dbContext.Add(serviceType);
                            dbContext.SaveChanges();
                        }

                    }

                    if (!typeInfo.BaseType.Equals(typeof(System.Object)))
                    {
                        SaveSubtypes(inputParameter, returnType, serviceProxy.ProxyType.Assembly.DefinedTypes.First(m => m.Name == typeInfo.BaseType.Name), String.IsNullOrEmpty(prefix) ? typeInfo.BaseType.Name : prefix + "." + typeInfo.BaseType.Name, String.Empty, parentId);
                    }
                }
            }

        }

        [HttpGet]
        public ActionResult TestServiceMethod()
        {
            serviceProxy = TempData["ServiceProxy"] as DynamicServiceProxy;
            TempData["ServiceProxy"] = serviceProxy;
            CustomerServiceUrlViewModel data = TempData["Data"] as CustomerServiceUrlViewModel;
            TempData["Data"] = data;
            
            ServiceMethodParameterViewModel smp = new ServiceMethodParameterViewModel();
            //TODO constructor?
            smp.Parameters = new List<ParameterViewModel>();
            smp.SoapHeaders = new List<ParameterViewModel>();
            foreach (var p in GetInputParamaterList(data))
            {
                ParameterViewModel parameter = new ParameterViewModel();
                parameter.Name = p.Name;
                parameter.TypeName = p.Type;
                if (p.Type == typeof(string).FullName)
                {
                    parameter.Value = parameter.Value as string;
                }
                else
                {
                    Type parameterType = Type.GetType(p.Type, false);
                    if (parameterType != null)
                    {
                        if (parameterType.IsArray)
                        {
                            parameter.Value = Activator.CreateInstance(parameterType.GetElementType());
                        }
                        else
                        {
                            parameter.Value = Activator.CreateInstance(parameterType);
                        }
                    }
                    else
                    {
                        Type assemblyType = serviceProxy.ProxyType.Assembly.GetType(p.Type);
                        if (assemblyType.IsArray)
                        {
                            parameter.Value = Activator.CreateInstance(assemblyType.GetElementType());
                        }
                        else if (!assemblyType.IsEnum)
                        {
                            parameter.Value = Activator.CreateInstance(assemblyType);
                        }
                        else
                        {
                            Type enumType = Enum.GetUnderlyingType(assemblyType);
                            parameter.Value = Activator.CreateInstance(enumType);
                        }
                    }
                }
                smp.Parameters.Add(parameter);
            }

            if (data.CustomSoapHeaderName != null)
            {
                smp.SoapHeaderTypeName = data.CustomSoapHeaderName;

                var q = (from t in customMessageHeadersAssembly.GetType(CUSTOMSOAPHEADERSNAMESPACE + "." + data.CustomSoapHeaderName).GetTypeInfo().DeclaredProperties
                         where t.GetCustomAttribute(typeof(HeaderDynamicProperty), false) != null
                         select new TypeData { Name = t.Name, Type = t.PropertyType.FullName });
                

                List<TypeData> customSoapHeaderFieldsList = new List<TypeData>(q);

                
                foreach (var p in customSoapHeaderFieldsList)
                {
                    ParameterViewModel parameter = new ParameterViewModel();
                    parameter.Name = p.Name;
                    parameter.TypeName = p.Type;
                    if (p.Type == typeof(string).FullName)
                    {
                        parameter.Value = parameter.Value as string;
                    }
                    else
                    {
                        parameter.Value = Activator.CreateInstance(Type.GetType(p.Type));
                    }
                    smp.SoapHeaders.Add(parameter);
                }
            }

            return View("ServiceMethodParameterView", smp);
        }

        

        [HttpPost]
        public ActionResult TestServiceMethod(ServiceMethodParameterViewModel model)
        {
            CustomerServiceUrlViewModel data = TempData["Data"] as CustomerServiceUrlViewModel;
            TempData["Data"] = data;
            serviceProxy = TempData["ServiceProxy"] as DynamicServiceProxy;
            TempData["ServiceProxy"] = serviceProxy;
            MethodResult mr = new MethodResult();
            StringBuilder request = new StringBuilder();
            request.Append(data.CustomerName + "_" + data.ServiceName + "." + data.ServiceName + "." + data.ServiceMethod + "( ");
            bool needComma = false;
            foreach (ParameterViewModel p in model.Parameters)
            {
                if( needComma )
                {
                    request.AppendFormat(", {0}", p.Value.ToString());
                }
                else 
                {
                    request.AppendFormat(" {0}", p.Value.ToString());
                }
                needComma = true;
            }
            request.Append(" )");
            mr.request = request.ToString();

            object[] parameters = new object[model.Parameters.Count];
            int j = 0;
            foreach(ParameterViewModel p in model.Parameters)
            {
                if (Type.GetType(p.TypeName, false) != null)
                {
                    if (Type.GetType(p.TypeName).IsArray)
                    {
                        //this needs to be dynamic since the type is not know until runtime
                        dynamic array = Activator.CreateInstance(Type.GetType(p.TypeName), new object[] { 1 });
                        
                        //special generic cast that llows the arry and the value to be of the same type
                        MethodInfo castMethod = this.GetType().GetMethod("Cast").MakeGenericMethod(Type.GetType(p.TypeName).GetElementType());
                        var test = castMethod.Invoke(null, new object[] { p.Value, array });
                        parameters[j] = array;
                    }
                    else
                    {
                        parameters[j] = p.Value;
                    }
                }
                else
                {
                    Type assemblyType = serviceProxy.ProxyType.Assembly.GetType(p.TypeName);
                    if (assemblyType.IsEnum)
                    {
                        parameters[j] = System.Enum.ToObject(assemblyType, p.Value);
                    }
                    else
                    {
                        if (assemblyType.BaseType != null && assemblyType.BaseType.IsGenericType && assemblyType.BaseType.GetGenericTypeDefinition() == typeof(List<>))
                        {

                            parameters[j] = Activator.CreateInstance(assemblyType);
                            assemblyType.GetMethod("Add").Invoke(parameters[j], new[] { p.Value });
                        }
                        else
                        {
                            parameters[j] = Convert.ChangeType(p.Value, assemblyType);
                        }
                    }
                }
                j++;
            }

            //TODO: testing
            MessageHeader proxyMessageHeader = null; ;
            if (model.SoapHeaders != null && model.SoapHeaders.Count > 0)
            {
                Type[] args = new Type[model.SoapHeaders.Count];
                for (int i = 0; i < model.SoapHeaders.Count; i++)
                {
                    ParameterViewModel p = model.SoapHeaders[i];
                    args[i] = Type.GetType(p.TypeName);
                }
                ConstructorInfo ctor = customMessageHeadersAssembly.GetTypes().First(m => m.Name == model.SoapHeaderTypeName).GetConstructor(args);
                //ConstructorInfo ctor = Assembly.GetExecutingAssembly().GetTypes().First(m => m.Name == model.SoapHeaderTypeName).GetConstructor(args);
                //MessageHeader proxyMessageHeader = ctor.Invoke(model.SoapHeaders.Select(h => h.Value).ToArray()) as MessageHeader;
                //TODO: testing
                proxyMessageHeader = ctor.Invoke(model.SoapHeaders.Select(h => h.Value).ToArray()) as MessageHeader;
                SoapHeaderInspector soapHeaderInspector = new SoapHeaderInspector(proxyMessageHeader);
                SoapEndpointBehavior soapEndpointBehavior = new SoapEndpointBehavior(soapHeaderInspector);
                ServiceEndpoint ep = (ServiceEndpoint)serviceProxy.GetProperty("Endpoint");
                ep.EndpointBehaviors.Add(soapEndpointBehavior);
            }

            object result = serviceProxy.CallMethod(data.ServiceMethod, parameters);

            XmlSerializer s = new XmlSerializer(result.GetType());
            StringWriter resultText = new StringWriter();
            s.Serialize(resultText, result);
            mr.result = resultText.ToString();


            ////TODO: testing Soap header parameter change
            //for (int i = 0; i < model.SoapHeaders.Count; i++)
            //    {
            //        ParameterViewModel p = model.SoapHeaders[i];
            //        proxyMessageHeader.GetType().InvokeMember(p.Name, C
            //            Type.DefaultBinder, proxyMessageHeader, new Object[] {"XXX"});    
            //    }
            ////proxyMessageHeader.GetType().InvokeMember("UserName", BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty,
            ////            Type.DefaultBinder, proxyMessageHeader, new Object[] { "XXX" });
            //result = serviceProxy.CallMethod(data.ServiceMethod, parameters);
            //s.Serialize(resultText, result);
            //mr.result += resultText.ToString();

            //mr.result = wsdlWebService.InvokeServiceMethod(data.ServiceName, data.ServiceMethod, model);
            return View("ServiceMethodResultView", mr);
        }

        public ActionResult ChangeEndpointAddress(string endpointAddress)
        {
            Trace.CorrelationManager.ActivityId = Guid.NewGuid();
            Trace.CorrelationManager.StartLogicalOperation("ChangeEndpointAddress");
            TraceSource.TraceEvent(TraceEventType.Start, 3000, string.Format("ChangeEndpointAddress : {0}", endpointAddress));

            DynamicServiceProxyFactoryOptions options = new DynamicServiceProxyFactoryOptions();
            options.EndpointAddress = DynamicServiceProxyFactoryOptions.EndpointAddressModifierOptions.MakeSameAsUrl;
            serviceFactory = new DynamicServiceProxyFactory(endpointAddress + "?wsdl", options);
            TempData["ServiceFactory"] = serviceFactory;
            TraceSource.TraceEvent(TraceEventType.Stop, 3999);
            return Content("Wsdl Address Changed");
        }

        //public ActionResult Simple(string serviceUrl)
        //{

        //    ViewBag.Simple = new List<SelectListItem>() { new SelectListItem { Text = "Item1", Value = "Item1" }, 
        //                                            new SelectListItem { Text = "Item2", Value = "Item2" }, 
        //                                            new SelectListItem { Text = "Item3", Value = "Item3"}};

        //    return Json(new { data = this.RenderPartialViewToString("Simple", this.myModel) });
        //}

        public JsonResult GetCustomers()
        {
            using (AtiWrapperServicesModel dbContext = new AtiWrapperServicesModel())
            {
                //var customerList = dbContext.Customers.Select(c => new { CustomerId = c.Id, CustomerName = c.Name });
                //return Json(customerList, JsonRequestBehavior.AllowGet);
                //var result = Json(dbContext.Customers, JsonRequestBehavior.AllowGet);

                var customerList = dbContext.Customers;
                List<SelectListItem> items = new List<SelectListItem>();
                foreach( var customer in customerList)
                {
                    items.Add(new SelectListItem { Value = customer.Id.ToString(), Text = customer.Name });
                }

                return Json(items.AsEnumerable(), JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetTestUrl(int? customerId)
        {
            using (AtiWrapperServicesModel dbContext = new AtiWrapperServicesModel())
            {
                if (customerId != null)
                {
                    var testUrlList = from t in dbContext.ServiceURLs 
                                      where customerId == t.Customer_Id
                                      select new { UrlID = t.Id, UrlName = t.TestURL };

                     List<SelectListItem> items = new List<SelectListItem>();

                    foreach (var testUrl in testUrlList)
                    {
                        items.Add(new SelectListItem { Value = testUrl.UrlID.ToString(), Text = testUrl.UrlName });
                    }

                    return Json(items.AsEnumerable(), JsonRequestBehavior.AllowGet);
                }
                return null;
            }
        }

        public JsonResult GetUseSourceEnums()
        {
            List<SelectListItem> enums = new List<SelectListItem>();
            int value = 0;
            foreach (var name in Enum.GetNames(typeof(UseSource)))
            {
                SelectListItem item = new SelectListItem();
                item.Text = name;
                item.Value = value.ToString();
                enums.Add(item);
                value++;
            }
            
            return Json(enums.AsEnumerable(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProductionUrl(int? customerId)
        {
            using (AtiWrapperServicesModel dbContext = new AtiWrapperServicesModel())
            {
                if (customerId != null)
                {
                    var productionUrlList = from t in dbContext.ServiceURLs
                                      where customerId == t.Customer_Id
                                      select new { UrlID = t.Id, UrlName = t.ProductionURL };

                    List<SelectListItem> items = new List<SelectListItem>();

                    foreach (var productionUrl in productionUrlList)
                    {
                        items.Add(new SelectListItem { Value = productionUrl.UrlID.ToString(), Text = productionUrl.UrlName });
                    }

                    return Json(items.AsEnumerable(), JsonRequestBehavior.AllowGet);
                }
                return null;
            }
        }

        public JsonResult GetServiceNames(string customerId, string serviceUrl)
        {
            using (AtiWrapperServicesModel dbContext = new AtiWrapperServicesModel())
            {
                int id = Convert.ToInt32(customerId);
                var serviceList = from c in dbContext.Customers
                                  join t in dbContext.ServiceURLs on c.Id equals t.Customer_Id
                                  join s in dbContext.ServiceNames on t.Id equals s.ServiceURL_Id
                                  where c.Id == id && s.ServiceType == 0
                                  select new { Text = s.Name, Value = s.Id };
                SelectList serviceNames;
                if (serviceList.Count() > 0)
                {
                    serviceNames = new SelectList(serviceList, "Text", "Value");
                    ViewBag.ServiceNames = serviceNames;
                    ViewBag.ServiceAddress = serviceUrl;
                    ViewBag.WsdlAddress = serviceUrl;
                    ViewBag.MakeSame = true;
                }
                else
                {
                    serviceNames = GetServiceNamesFromWsdl(serviceUrl);
                }

                return Json(serviceNames, JsonRequestBehavior.AllowGet);
            }
        }

        public SelectList GetServiceNamesFromWsdl(string serviceUrl)
        {
            Trace.CorrelationManager.ActivityId = Guid.NewGuid();
            Trace.CorrelationManager.StartLogicalOperation("ServiceNameList");
            TraceSource.TraceEvent(TraceEventType.Start, 4000, string.Format("ServiceNameList : {0}", serviceUrl));
            if (serviceFactory == null)
            {
                serviceFactory = new DynamicServiceProxyFactory(serviceUrl + "?wsdl");
            }
            TempData["ServiceFactory"] = serviceFactory;
            var a = (from c in serviceFactory.Contracts select new SelectListItem { Text = c.Name, Value = c.Name });
            SelectList serviceNames = new SelectList(a, "Value", "Text");

            foreach (ServiceEndpoint e in serviceFactory.Endpoints)
            {
                if (!serviceUrl.Equals(e.Address.ToString()))
                {
                    ViewBag.WsdlAddress = e.Address.ToString();
                    ViewBag.ServiceAddress = serviceUrl;
                    ViewBag.MakeSame = true;
                }
            }
            
            ViewBag.ServiceNames = serviceNames;
            TraceSource.TraceEvent(TraceEventType.Stop, 4999);
            return serviceNames;
        }

        public JsonResult GetServiceMethods(string serviceName)
        {
            serviceFactory = TempData["ServiceFactory"] as DynamicServiceProxyFactory;
            serviceProxy = serviceFactory.CreateProxy(serviceName);


            TempData["ServiceFactory"] = serviceFactory;
            TempData["ServiceProxy"] = serviceProxy;
            var selectServiceMethods = (from m in serviceProxy.ProxyType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                                        select new SelectListItem { Text = m.Name, Value = m.Name });
            SelectList serviceMethods = new SelectList(selectServiceMethods, "Value", "Text");
            ViewBag.ServiceMethods = serviceMethods;
            return Json(serviceMethods, JsonRequestBehavior.AllowGet);
            //return Json(new { data = this.RenderPartialViewToString("_ServiceMethod", this.myModel) });
        }

        public JsonResult GetSoapHeaders()
        {
            return Json(myModel.CustomSoapHeaderList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ServiceTypeTables(string serviceMethod)
        {
            serviceProxy = TempData["ServiceProxy"] as DynamicServiceProxy;
            TempData["ServiceProxy"] = serviceProxy;
            var b = (from m in serviceProxy.ProxyType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                     where m.Name == serviceMethod
                     select new TypeData { Name = m.ReturnType.Name, Type = m.ReturnType.ToString() });
            var c = (from m in serviceProxy.ProxyType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                     where m.Name == serviceMethod
                     from p in m.GetParameters()
                     select new TypeData { Name = p.Name, Type = p.ParameterType.ToString() });
            TypeData ret = b.First();
            List<TypeData> parameters = new List<TypeData>(c);
            ViewBag.ReturnType = ret;
            ViewBag.Parameters = parameters;
            return Json(new { data = this.RenderPartialViewToString("_TypeTables", this.myModel) });
        }

        protected string RenderPartialViewToString(string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.RouteData.GetRequiredString("action");

            ViewData.Model = model;

            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }

        //public ActionResult ParameterTypeTable()
        //{
            
        //    myModel = TempData["Data"] as CustomerServiceUrlViewModel;
        //    TempData["Data"] = myModel;
        //    return View(myModel);
        //}

        //public ActionResult ReturnTypeTable()
        //{
        //    myModel = TempData["Data"] as CustomerServiceUrlViewModel;
        //    TempData["Data"] = myModel;
        //    return View(myModel);
        //}
    }

 
}
