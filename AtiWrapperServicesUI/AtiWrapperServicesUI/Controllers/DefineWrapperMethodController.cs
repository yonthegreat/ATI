using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AtiWrapperServicesUI.Models;
using System.Xml.Linq;
using AtiWrapperServicesORM.OpenAccess;
using DynamicServiceProxyNamespace;
using System.Reflection;
using System.Diagnostics;

namespace AtiWrapperServicesUI.Controllers
{
    public class DefineWrapperMethodController : Controller
    {
        static public TraceSource TraceSource = new TraceSource("AtiServiceDataUI");
        //
        // GET: /WrapperMethod/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /WrapperMethod/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /WrapperMethod/Create

        public ActionResult Create()
        {
            WrapperMethodViewModel model = new WrapperMethodViewModel();
            using (AtiWrapperServicesModel dbContext = new AtiWrapperServicesModel())
            {
                var customerQuery = from c in dbContext.Customers
                                    select new SelectListItem { Text = c.Name, Value = c.Name };
                model.CustomerNames = new List<SelectListItem>(customerQuery);
                var methodQuery = from m in dbContext.ServiceMethods
                                  select new SelectListItem { Text = m.Name, Value = m.Name };
                model.ServiceMethods = new List<SelectListItem>(methodQuery);

                return View(model);
            }
        }

        //
        // POST: /WrapperMethod/Create

        [HttpPost]
        public ActionResult Create(WrapperMethodViewModel model, string command)
        {
            Trace.CorrelationManager.ActivityId = Guid.NewGuid();
            Trace.CorrelationManager.StartLogicalOperation("WrapperMethodController");
            TraceSource.TraceEvent(TraceEventType.Start, 1000, "WrapperMethodController Create Post");
            using (AtiWrapperServicesModel dbContext = new AtiWrapperServicesModel())
            {
                if (command == "InputParamterDefaults")
                {
                    var proxyCreateData = (from n in dbContext.ServiceNames
                                           join u in dbContext.ServiceURLs on n.ServiceURL.Id equals u.Id
                                           join m in dbContext.ServiceMethods on n.Id equals m.ServiceName.Id
                                           join c in dbContext.Customers on u.Customer.Id equals c.Id
                                           where model.ServiceMethod == m.Name && model.CustomerName == c.Name
                                           select new { Id = n.Id, Name = n.Name, TestUrl = u.TestURL, ProductionUrl = u.ProductionURL, EndpointModifier = m.EndpointAddressModifierOption, XmlSignatureType = m.XmlTypeSignature, ProxyServiceType = n.ServiceType }).FirstOrDefault();

                    if (proxyCreateData != null)
                    {
                        DynamicServiceProxyFactory serviceFactory;
                        DynamicServiceProxy proxy;
                        DynamicServiceProxyFactoryOptions options = new DynamicServiceProxyFactoryOptions();
                        options.ServiceProxyType = (DynamicServiceProxyFactoryOptions.ServiceProxyTypes)proxyCreateData.ProxyServiceType;

                        if (options.ServiceProxyType.Equals(DynamicServiceProxyFactoryOptions.ServiceProxyTypes.Wsdl))
                        {

                            options.EndpointAddress = (DynamicServiceProxyFactoryOptions.EndpointAddressModifierOptions)proxyCreateData.EndpointModifier;
                            serviceFactory = new DynamicServiceProxyFactory(proxyCreateData.TestUrl + "?wsdl", options);
                            proxy = serviceFactory.CreateProxy(proxyCreateData.Name);
                        }
                        else //if( options.ServiceProxyType.Equals(DynamicServiceProxyFactoryOptions.ServiceProxyTypes.Xml))
                        {
                            List<XmlServiceParameters> inputParamters = (from n in dbContext.ServiceNames
                                                                         join u in dbContext.ServiceURLs on n.ServiceURL.Id equals u.Id
                                                                         join m in dbContext.ServiceMethods on n.Id equals m.ServiceName.Id
                                                                         join c in dbContext.Customers on u.Customer.Id equals c.Id
                                                                         join i in dbContext.InputParamaters on m.Id equals i.ServiceMethod.Id
                                                                         join t in dbContext.ServiceTypes on i.Id equals t.InputParamater.Id
                                                                         join q in dbContext.QualifiedNames on t.QualifiedName.Id equals q.Id
                                                                         orderby i.Order
                                                                         where model.ServiceMethod == m.Name && model.CustomerName == c.Name
                                                                         select new XmlServiceParameters { Name = q.Name, TypeName = t.TypeName }).ToList();

                            serviceFactory = new DynamicServiceProxyFactory(proxyCreateData.TestUrl, proxyCreateData.Name, model.ServiceMethod, proxyCreateData.XmlSignatureType, options, inputParamters, Assembly.GetExecutingAssembly().ToString());
                            proxy = serviceFactory.CreateProxy(proxyCreateData.Name + "_" + model.ServiceMethod);
                        }
                        TraceSource.TraceEvent(TraceEventType.Stop, 2998);
                        TempData["ServiceProxy"] = proxy;
                        TempData["WrapperModel"] = model;
                        return RedirectToAction("InputParameterDefaults", model);
                    }
                }
                TraceSource.TraceEvent(TraceEventType.Stop, 2999);
                return View(model);
            }
        }

        [HttpGet]
        public ActionResult InputParameterDefaults(WrapperMethodViewModel model)
        {            
            WrapperMethodParameterViewModel wmp = new WrapperMethodParameterViewModel();
            wmp.ServiceMethodName = model.ServiceMethod;
            wmp.WrapperMethodName = model.WrapperMethod;
            wmp.CustomerName = model.CustomerName;
            //TODO constructor?
            wmp.Parameters = new List<ParameterDefaultViewModel>();
            wmp.SoapHeaders = new List<ParameterDefaultViewModel>();

            DynamicServiceProxy proxy = TempData["ServiceProxy"] as DynamicServiceProxy;

            using (AtiWrapperServicesModel dbContext = new AtiWrapperServicesModel())
            {
                var inputParameterQuery = from m in dbContext.ServiceMethods
                                          join i in dbContext.InputParamaters on m.Id equals i.ServiceMethod.Id
                                          join t in dbContext.ServiceTypes on i.Id equals t.InputParamater.Id
                                          join q in dbContext.QualifiedNames on t.QualifiedName.Id equals q.Id
                                          join n in dbContext.ServiceNames on m.ServiceName.Id equals n.Id
                                          join u in dbContext.ServiceURLs on n.ServiceURL.Id equals u.Id
                                          join c in dbContext.Customers on u.Customer.Id equals c.Id
                                          orderby i.Order
                                          where m.Name == model.ServiceMethod && c.Name == model.CustomerName && (t.IsSystemType == true || t.IsEnum == true)
                                          select new { Name = q.Name, Type = t.TypeName, IsEnum = t.IsEnum };

                int id = 0;
                foreach (var p in inputParameterQuery.ToList())
                {
                    ParameterDefaultViewModel parameter = new ParameterDefaultViewModel();
                    parameter.Name = p.Name;
                    parameter.TypeName = p.Type;
                    parameter.Required = false;
                    parameter.Id = id;
                    if (p.Type == typeof(string).FullName)
                    {
                        parameter.Value = parameter.Value as string;
                    }
                    else
                    {
                        if (p.IsEnum)
                        {
                            parameter.Value = (int)0;
                            Type enumType = proxy.ProxyType.Assembly.GetType(p.Type);
                            var enumValues = Enum.GetValues(enumType).Cast<Enum>().Select(e => new { Value = e.ToString(), Text = e.ToString() });
                            parameter.EnumList = new SelectList(enumValues, "Value", "Text", "");
                        }
                        else
                        {
                            parameter.Value = Activator.CreateInstance(Type.GetType(p.Type));
                        }
                    }
                    wmp.Parameters.Add(parameter);
                    id++;
                }
                
                var headerMessageQuery  = from m in dbContext.ServiceMethods
                                          join h in dbContext.ServiceMessageHeaders on m.Id equals h.ServiceMethod_Id
                                          join t in dbContext.ServiceTypes on h.Id equals t.ServiceMessageHeader.Id
                                          join q in dbContext.QualifiedNames on t.QualifiedName.Id equals q.Id
                                          orderby h.Order
                                          where m.Name == model.ServiceMethod && t.IsSystemType == true
                                          select new { Name = q.Name, Type = t.TypeName };


                id = wmp.Parameters.Count;

                foreach (var p in headerMessageQuery)
                {
                    ParameterDefaultViewModel parameter = new ParameterDefaultViewModel();
                    parameter.Name = p.Name;
                    parameter.TypeName = p.Type;
                    parameter.Required = false;
                    parameter.Id = id;
                    if (p.Type == typeof(string).FullName)
                    {
                        parameter.Value = parameter.Value as string;
                    }
                    else
                    {
                        parameter.Value = Activator.CreateInstance(Type.GetType(p.Type));
                    }
                    wmp.SoapHeaders.Add(parameter);
                    id++;
                }
            }

            return View("WrapperMethodParameterDefaultView", wmp);
        }

        [HttpPost]
        public ActionResult InputParameterDefaults(WrapperMethodParameterViewModel model)
        {
            using (AtiWrapperServicesModel dbContext = new AtiWrapperServicesModel())
            {
                ServiceMethod serviceMethod = (from s in dbContext.ServiceMethods
                                               join n in dbContext.ServiceNames on s.ServiceName.Id equals n.Id
                                               join u in dbContext.ServiceURLs on n.ServiceURL.Id equals u.Id
                                               join c in dbContext.Customers on u.Customer.Id equals c.Id
                                               where s.Name == model.ServiceMethodName && c.Name == model.CustomerName
                                               select s).First();

                var wrapperMethod = new WrapperMethod
                {
                    ServiceMethod = serviceMethod,
                    Name = model.WrapperMethodName,
                };

                dbContext.Add(wrapperMethod);
                dbContext.SaveChanges();

                var wrapperReturnType = new WrapperReturnType
                {
                    WrapperMethod = wrapperMethod,
                    ReturnEncoding = 0,
                };
                dbContext.Add(wrapperReturnType);
                dbContext.SaveChanges();

                var inputParametersQuery = from i in dbContext.InputParamaters
                                           where i.ServiceMethod.Id == serviceMethod.Id
                                           orderby i.Order
                                           select i;

                foreach (InputParamater i in inputParametersQuery.ToList())
                {
                    var wrapperInputParamater = new WrapperInputParameter
                    {
                        WrapperMethod = wrapperMethod,
                    };
                    dbContext.Add(wrapperInputParamater);
                    dbContext.SaveChanges();

                    var inputTypesQuery = from t in dbContext.ServiceTypes
                                          join q in dbContext.QualifiedNames on t.QualifiedName.Id equals q.Id
                                          where t.InputParamater.Id == i.Id && (t.IsSystemType == true || t.IsEnum == true)
                                          select q;

                    foreach (QualifiedName q in inputTypesQuery.ToList())
                    {
                        ParameterDefaultViewModel parameterData = model.Parameters.First(x => x.Name == q.Name);
                        var wrapperValueProperty = new WrapperValueProperty
                        {
                            QualifiedName = q,
                            WrapperInputParameter = wrapperInputParamater,
                            Required = parameterData.Required,
                        };
                        dbContext.Add(wrapperValueProperty);

                        if (parameterData.Required == false)
                        {
                            WrapperValue wrapperValue = new WrapperValue
                            {
                                DefaultWrapperValueProperty_Id = wrapperValueProperty.Id,
                            };
                            if (parameterData.Value.GetType() == typeof(System.String[]))
                            {
                                string[] value = (string[])parameterData.Value;
                                wrapperValue.Value = value[0];
                            }
                            dbContext.Add(wrapperValue);
                        }
                        dbContext.SaveChanges();
                    }
                }
                
                var headerMessageQuery = from h in dbContext.ServiceMessageHeaders where h.ServiceMethod_Id == serviceMethod.Id orderby h.Order select h;

                foreach (ServiceMessageHeader h in headerMessageQuery.ToList())
                {
                    var wrapperMessageHeader = new WrapperMessageHeader
                    {
                        WrapperMethod = wrapperMethod,
                    };
                    dbContext.Add(wrapperMessageHeader);
                    dbContext.SaveChanges();

                    var messageHeaderTypesQuery = from t in dbContext.ServiceTypes
                                                  join q in dbContext.QualifiedNames on t.QualifiedName.Id equals q.Id
                                                  where t.ServiceMessageHeader.Id == h.Id && t.IsSystemType == true
                                                  select q;

                    foreach (QualifiedName q in messageHeaderTypesQuery.ToList())
                    {
                        ParameterDefaultViewModel parameterData = model.SoapHeaders.First(x => x.Name == q.Name);
                        WrapperValueProperty wrapperValueProperty = new WrapperValueProperty
                        {
                            QualifiedName = q,
                            WrapperMessageHeader = wrapperMessageHeader,
                            Required = parameterData.Required,
                        };
                        dbContext.Add(wrapperValueProperty);

                        if (parameterData.Required == false)
                        {
                            WrapperValue wrapperValue = new WrapperValue
                            {
                                DefaultWrapperValueProperty_Id = wrapperValueProperty.Id,
                            };
                            if (parameterData.Value.GetType() == typeof(System.String[]))
                            {
                                string[] value = (string[])parameterData.Value;
                                wrapperValue.Value = value[0];
                            }
                            dbContext.Add(wrapperValue);
                        }
                        dbContext.SaveChanges();
                    }
                }
            }
            return View("WrapperMethodParameterDefaultView", model);
        }

        //
        // GET: /WrapperMethod/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /WrapperMethod/Edit/5

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
        // GET: /WrapperMethod/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /WrapperMethod/Delete/5

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
    }
}
