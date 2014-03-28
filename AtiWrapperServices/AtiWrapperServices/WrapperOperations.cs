using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AtiWrapperServicesORM.OpenAccess;
using System.Xml;
using System.Xml.Serialization;
using System.Reflection;
using System.IO;
using System.Collections;
using System.Diagnostics;


namespace AtiWrapperServices
{
    /// <summary>
    /// AtiWrapperServices WrapperOperations Class
    /// </summary>
    public class WrapperOperations
    {
        /// <summary>
        /// This code creates an array of type T and casts object to the type and puts them in the array.
        /// </summary>
        /// <typeparam name="T">Type T</typeparam>
        /// <param name="objs">Array of objects to be added to the new array of type T</param>
        /// <returns>Array of type T and all object now cast to type T</returns>
        public static T[] CastToArray<T>(object[] objs)
        {
            dynamic array = Activator.CreateInstance(typeof(T[]), new object[] { objs.Length });
            for (int i = 0; i < objs.Length; i++)
            {
                array[i] = (T)objs[i];
            }
            return array;
        }

        /// <summary>
        /// This method checks that all required parameters have been provided to the Wrapper prior to making a call to the real service.
        /// </summary>
        /// <param name="wrapperName">AtiWrapperServices service wrapper name</param>
        /// <param name="customerId">AtiWrapperServices customer Id</param>
        /// <param name="qualifiedNames">AtiWrapperServices list of string of fully qualified parameter names</param>
        /// <param name="DbContext">AtiWrapperServices ServiceProxyDBContext to the ServiceData Database</param>
        /// <param name="wrapperResult">AtiWrapperServices WrapperResult</param>
        /// <returns>boolean</returns>
        public bool CheckRequiredParameters(string wrapperName, int customerId, List<string> qualifiedNames, AtiWrapperServicesModel dbContext, WrapperResult wrapperResult)
        {
            var requiredWrapperParameters = (from w in dbContext.WrapperMethods
                                             join i in dbContext.WrapperInputParameters on w.Id equals i.WrapperMethod.Id
                                             join ip in dbContext.WrapperValueProperties on i.Id equals ip.WrapperInputParameter.Id
                                             join iq in dbContext.QualifiedNames on ip.QualifiedName.Id equals iq.Id
                                             join sm in dbContext.ServiceMethods on w.ServiceMethod.Id equals sm.Id
                                             join sn in dbContext.ServiceNames on sm.ServiceName.Id equals sn.Id
                                             join su in dbContext.ServiceURLs on sn.ServiceURL.Id equals su.Id
                                             join c in dbContext.Customers on su.Customer.Id equals c.Id
                                             where w.Name == wrapperName && c.Id == customerId && ip.Required == true
                                             select iq.Name).ToList();
            var requiredHeaderParamaters = (from w in dbContext.WrapperMethods
                                            join h in dbContext.WrapperMessageHeaders on w.Id equals h.WrapperMethod.Id
                                            join hp in dbContext.WrapperValueProperties on h.Id equals hp.WrapperMessageHeader.Id
                                            join hq in dbContext.QualifiedNames on hp.QualifiedName.Id equals hq.Id
                                            join sm in dbContext.ServiceMethods on w.ServiceMethod.Id equals sm.Id
                                            join sn in dbContext.ServiceNames on sm.ServiceName.Id equals sn.Id
                                            join su in dbContext.ServiceURLs on sn.ServiceURL.Id equals su.Id
                                            join c in dbContext.Customers on su.Customer.Id equals c.Id
                                            where w.Name == wrapperName && c.Id == customerId && hp.Required == true
                                            select hq.Name).ToList();

            requiredWrapperParameters.AddRange(requiredHeaderParamaters);

            var test = qualifiedNames.Distinct().ToList().Count;
            var test2 = requiredWrapperParameters.Intersect(qualifiedNames.Distinct().ToList(), StringComparer.OrdinalIgnoreCase);

            if (requiredWrapperParameters.Intersect(qualifiedNames.Distinct().ToList(), StringComparer.OrdinalIgnoreCase).Count() == requiredWrapperParameters.Count())
                return true;
            else
            {
                wrapperResult.ResultStatus = WrapperResult.ATIWrapperServiceStatusEnum.MissingPararmeter;
                foreach (string name in requiredWrapperParameters.Except(qualifiedNames))
                {
                    wrapperResult.FailureReason += string.Format(" Missing Required Parameter Qualified name: {0}", name);
                }
                return false;
            }
        }
        /// <summary>
        /// This method creates a dictionary of the parameters and their xml tag names. This is built during start up from the database.
        /// It is kept in memory to improve performance.
        /// </summary>
        /// <param name="parameters">AtiWrapperServices string of parameter Names</param>
        /// <returns>ParameterDictionary</returns>
        private ParameterDictionary CreateParameterDictionary(string parameters)
        {
            ParameterDictionary parameterDictionary = new ParameterDictionary();

            if (!String.IsNullOrEmpty(parameters))
            {
               
                using (XmlTextReader reader = new XmlTextReader(parameters, XmlNodeType.Element, null))
                {
                    int i = 0;
                    while (reader.Read())
                    {
                        ParamTagName param = new ParamTagName();
                        param.Tag = reader.Name;
                        param.Name = reader.ReadString();
                        if (string.IsNullOrEmpty(param.Tag))
                        {
                            throw new Exception(string.Format("Parameter Tags Invalid, no tag for: {0}", param.Name));
                        }
                        parameterDictionary.Parameters.Add(i, param);
                        i++;
                    }
                }
            }
            return parameterDictionary;
        }

        /// <summary>
        /// This method maps the parameterized values and defaulted values for the MessageHeader portion of WSDL based services
        /// </summary>
        /// <param name="IsTest">boolean</param>
        /// <param name="serviceMethodId">AtiWrapperServices ServiceMethod Id</param>
        /// <param name="wrapperName">AtiWrapperServices Service Wrapper Name</param>
        /// <param name="customerId">AtiWrapperServices Customer Id</param>
        /// <param name="pararmeterDictionary">AtiWrapperServices Dictionary of parameters</param>
        /// <param name="DbContext">AtiWrapperServices ServiceProxyDBContext to the ServiceData Database</param>
        public void MapMessageHeaderValuesAndDefaults(AtiWrapperServicesEnums.ServiceMode serviceMode, int serviceMethodId, string wrapperName, int customerId, ParameterDictionary pararmeterDictionary, AtiWrapperServicesModel dbContext)
        {
            ProxyInformation wrapperProxyInfo;
            if (serviceMode.Equals(AtiWrapperServicesEnums.ServiceMode.Production))
            {
                wrapperProxyInfo = ServiceProxies.ProductionProxies[serviceMethodId];
            }
            else
            {
                wrapperProxyInfo = ServiceProxies.TestProxies[serviceMethodId];
            }
            
            var serviceHeaderParameters = (from s in dbContext.ServiceMethods
                                           join h in dbContext.ServiceMessageHeaders on s.Id equals h.ServiceMethod_Id
                                           join t in dbContext.ServiceTypes on h.Id equals t.ServiceMessageHeader.Id
                                           join q in dbContext.QualifiedNames on t.QualifiedName.Id equals q.Id
                                           join n in dbContext.ServiceNames on s.ServiceName.Id equals n.Id
                                           join u in dbContext.ServiceURLs on n.ServiceURL.Id equals u.Id
                                           join c in dbContext.Customers on u.Customer.Id equals c.Id
                                           orderby h.Order
                                           where s.Id == serviceMethodId && c.Id == customerId && (t.IsSystemType == true || t.IsEnum == true)
                                           select new { QualifiedName = q.Name, QualifiedId = q.Id, TypeName = t.TypeName, HeaderId = h.Id, IsEnum = t.IsEnum }).ToList();

            foreach (var shp in serviceHeaderParameters)
            {
                Object value;
                string parameterSpecified = (from t in pararmeterDictionary.Parameters.Values
                                            where shp.QualifiedName == t.Tag
                                            select t.Name).FirstOrDefault();
                if (pararmeterDictionary.Parameters.Count == 0 || string.IsNullOrEmpty(parameterSpecified))
                {
                    var defaultValue = (from w in dbContext.WrapperMethods
                                        join h in dbContext.WrapperMessageHeaders on w.Id equals h.WrapperMethod.Id
                                        join p in dbContext.WrapperValueProperties on h.Id equals p.WrapperMessageHeader.Id
                                        join v in dbContext.WrapperValues on p.Id equals v.DefaultWrapperValueProperty_Id
                                        join q in dbContext.QualifiedNames on p.QualifiedName.Id equals q.Id
                                        join s in dbContext.ServiceMethods on w.ServiceMethod.Id equals s.Id
                                        join n in dbContext.ServiceNames on s.ServiceName.Id equals n.Id
                                        join u in dbContext.ServiceURLs on n.ServiceURL.Id equals u.Id
                                        join c in dbContext.Customers on u.Customer.Id equals c.Id
                                        where w.Name == wrapperName && c.Id == customerId && shp.QualifiedId == q.Id
                                        select v.Value).First();
                    value = Convert.ChangeType(defaultValue, Type.GetType(shp.TypeName));
                }
                else
                {
                    value = Convert.ChangeType(parameterSpecified, Type.GetType(shp.TypeName));
                }

                wrapperProxyInfo.proxyHeaders[shp.HeaderId].GetType().InvokeMember(shp.QualifiedName.Split('.').Last(),
                                                                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty,
                                                                        Type.DefaultBinder,
                                                                        wrapperProxyInfo.proxyHeaders[shp.HeaderId],
                                                                        new Object[] { value });
            }
        }

        /// <summary>
        /// This method maps the parameterized values and defaulted values for the service method 
        /// </summary>
        /// <param name="IsTest">boolean</param>
        /// <param name="serviceMethodId">AtiWrapperServices ServiceMethod Id</param>
        /// <param name="wrapperName">AtiWrapperServices Service Wrapper Name</param>
        /// <param name="customerId">AtiWrapperServices Customer Id</param>
        /// <param name="pararmeterDictionary">AtiWrapperServices Dictionary of parameters</param>
        /// <param name="DbContext">AtiWrapperServices ServiceProxyDBContext to the ServiceData Database</param>
        /// <returns>Array of Object</returns>
        public Object[] NewMapWrapperParameterValuesAndDefaults(AtiWrapperServicesEnums.ServiceMode serviceMode, int serviceMethodId, string wrapperName, int customerId, ParameterDictionary parameterDictionary, AtiWrapperServicesModel dbContext)
        {
            var serviceParameters = (from s in dbContext.ServiceMethods
                                     join i in dbContext.InputParamaters on s.Id equals i.ServiceMethod.Id
                                     join t in dbContext.ServiceTypes on i.Id equals t.InputParamater.Id
                                     join q in dbContext.QualifiedNames on t.QualifiedName.Id equals q.Id
                                     join n in dbContext.ServiceNames on s.ServiceName.Id equals n.Id
                                     join u in dbContext.ServiceURLs on n.ServiceURL.Id equals u.Id
                                     join c in dbContext.Customers on u.Customer.Id equals c.Id
                                     orderby i.Order
                                     where s.Id == serviceMethodId && c.Id == customerId && t.ParentId == null
                                     select new { QualifiedName = q.Name, QualifiedID = q.Id, TypeName = t.TypeName, ParentId = t.ParentId, IsEnum = t.IsEnum, TypeId = t.Id}).ToList();

            List<QualifiedObjects> arguments = new List<QualifiedObjects>();
            List<int> orderedParameterList = new List<int>();
            var methodName = (from s in dbContext.ServiceMethods
                              where s.Id == serviceMethodId
                              select s.Name).FirstOrDefault();
            System.Text.StringBuilder logParameters = new System.Text.StringBuilder();
            logParameters.Append(string.Format("Service Method: {0} Parameters: ", methodName));

            foreach (var sp in serviceParameters)
            {
                arguments.Add(AddChildren(serviceMode, orderedParameterList, sp.TypeId, serviceMethodId, wrapperName, customerId, parameterDictionary, dbContext, logParameters));
            }
            List<object> returnObjs = new List<object>();
            foreach (QualifiedObjects a in arguments)
            {
                foreach (object o in a.QualifiedObjectList)
                {
                    returnObjs.Add(o);
                }
            }
            ServiceProxies.WrapperTrace.TraceEvent(TraceEventType.Information, 100, logParameters.ToString());
            return returnObjs.ToArray();
        }

        /// <summary>
        /// This method obscures parameters that can not be logged. i.e. card number and ccv.
        /// </summary>
        /// <param name="logParameters">AtiWrapperServices Reference of Parameters to log</param>
        /// <param name="log">AtiWrapperServices log of 0 = Do Not Log</param>
        /// <param name="typeName">AtiWrapperServices Parameter Type name</param>
        /// <param name="name">AtiWrapperServices Parameter Name</param>
        /// <param name="value">AtiWrapperServices Parameter Value</param>
        private void AddToServiceMethodLog(ref System.Text.StringBuilder logParameters, int log, string typeName, string name, string value)
        {
            if (log == 0)
            {
                logParameters.Append(string.Format("{0} : {1} = {2}, ", typeName, name, value));
            }
            else
            {
                string blank = value;
                blank = System.Text.RegularExpressions.Regex.Replace(blank, ".", "#");
                logParameters.Append(string.Format("{0} : {1} = {2}, ", typeName, name, blank));
            }
        }

        /// <summary>
        /// This method builds a flat list of System types for each Service Method. It uses recursion on complex types, arrays and lists. It can also deal with enumerated types.
        /// </summary>
        /// <param name="IsTest">boolean</param>
        /// <param name="orderedParameterList">AtiWrapperServices List of Wrapper parameter id(s) in call Parameter order</param>
        /// <param name="parameterId">AtiWrapperServices parameterId</param>
        /// <param name="serviceMethodId">AtiWrapperServices WrapperServiceId</param>
        /// <param name="wrapperName">AtiWrapperServices Wrapper Name</param>
        /// <param name="customerId">AtiWrapperServices Wrapper customerId</param>
        /// <param name="parameterDictionary">AtiWrapperServices ParameterDictionary</param>
        /// <param name="DbContext">AtiWrapperServices ServiceProxyDBContext to the ServiceData Database</param>
        /// <param name="logParameters">AtiWrapperServices StringBuilder LogParameters</param>
        /// <returns>QualifiedObjects</returns>
        private QualifiedObjects AddChildren(AtiWrapperServicesEnums.ServiceMode serviceMode, List<int> orderedParameterList, int parameterId, int serviceMethodId, string wrapperName, int customerId, ParameterDictionary parameterDictionary, AtiWrapperServicesModel dbContext, System.Text.StringBuilder logParameters)
        {
            List<QualifiedObjects> children = new List<QualifiedObjects>();
            var kids = (from t in dbContext.ServiceTypes 
                        join c in dbContext.ServiceTypes on t.Id equals c.ParentId
                        join i in dbContext.InputParamaters on c.InputParamater.Id equals i.Id
                        where t.Id == parameterId
                        orderby i.Order
                        select c.Id).ToList();
            foreach(int kid in kids)
            {
                children.Add(AddChildren(serviceMode, orderedParameterList, kid, serviceMethodId, wrapperName, customerId, parameterDictionary, dbContext, logParameters));
            }
            orderedParameterList.Add(parameterId);

            ProxyInformation wrapperProxyInfo;
            if (serviceMode.Equals(AtiWrapperServicesEnums.ServiceMode.Production))
            {
                wrapperProxyInfo = ServiceProxies.ProductionProxies[serviceMethodId];                
            }
            else
            {
                wrapperProxyInfo = ServiceProxies.TestProxies[serviceMethodId];
            }
            
            var sp = (from t in dbContext.ServiceTypes 
                        join q in dbContext.QualifiedNames on t.QualifiedName.Id equals q.Id
                        where t.Id == parameterId
                      select new { QualifiedName = q.Name, QualifiedID = q.Id, TypeName = t.TypeName, ParentId = t.ParentId, IsEnum = t.IsEnum, TypeId = t.Id, Log = t.LogParameter }).First();

            
            QualifiedObjects returnObjects = new QualifiedObjects(sp.QualifiedName);
            Type parameterType = Type.GetType(sp.TypeName);
            if ((parameterType != null && !parameterType.IsArray && !parameterType.IsGenericType) || sp.IsEnum)
            {

                List<string> parameterSpecified = (from t in parameterDictionary.Parameters.Values
                                             where sp.QualifiedName == t.Tag
                                             select t.Name).ToList();

                if (parameterSpecified.Count == 0 || parameterDictionary.Parameters.Count == 0)
                {
                    //ServiceProxies._trace.TraceEvent(TraceEventType.Information, 300000, string.Format("AddChildren: {0}", wrapperName));
                    int wid = (from w in dbContext.WrapperMethods where w.Name == wrapperName select w.Id).FirstOrDefault();
                    //ServiceProxies._trace.TraceEvent(TraceEventType.Information, 300001, string.Format("WrapperId: {0}", wid));
                    IQueryable<int> wips = (from i in dbContext.WrapperInputParameters where i.WrapperMethod_Id == wid select i.Id);
                    foreach (int wip in wips)
                    {
                        //ServiceProxies._trace.TraceEvent(TraceEventType.Information, 300002, string.Format("WrapperInputId: {0}", wip));
                        int wvpid = (from p in dbContext.WrapperValueProperties where p.WrapperInputParameter_Id == wip select p.Id).FirstOrDefault();
                        //ServiceProxies._trace.TraceEvent(TraceEventType.Information, 300003, string.Format("WrapperInputParameterId: {0}", wvpid));
                        //var wvidandvalue = (from v in DbContext.WrapperValues where v.DefaultWrapperValueProperty_Id == wvpid select new { id = v.Id, value = v.Value }).FirstOrDefault();
                        //ServiceProxies._trace.TraceEvent(TraceEventType.Information, 300004, string.Format("WrapperValueID: {0} {1}", wvidandvalue.id, wvidandvalue.value));
                    }


                    string defaultValue = (from w in dbContext.WrapperMethods
                                           join i in dbContext.WrapperInputParameters on w.Id equals i.WrapperMethod.Id
                                           join p in dbContext.WrapperValueProperties on i.Id equals p.WrapperInputParameter.Id
                                           join v in dbContext.WrapperValues on p.Id equals v.DefaultWrapperValueProperty_Id
                                           join q in dbContext.QualifiedNames on p.QualifiedName.Id equals q.Id
                                           join s in dbContext.ServiceMethods on w.ServiceMethod.Id equals s.Id
                                           join n in dbContext.ServiceNames on s.ServiceName.Id equals n.Id
                                           join u in dbContext.ServiceURLs on n.ServiceURL.Id equals u.Id
                                           join c in dbContext.Customers on u.Customer.Id equals c.Id
                                           where w.Name == wrapperName && c.Id == customerId && sp.QualifiedID == q.Id
                                           select v.Value).FirstOrDefault();
                    if (string.IsNullOrEmpty(defaultValue))
                    {
                        if (sp.TypeName == typeof(string).FullName)
                        {
                            returnObjects.QualifiedObjectList.Add(String.Empty);
                        }
                        else if (sp.IsEnum)
                        {
                            Type enumType = wrapperProxyInfo.proxy.ProxyType.Assembly.GetType(sp.TypeName);
                            returnObjects.QualifiedObjectList.Add(System.Enum.ToObject(enumType, 0));
                        }
                        
                        else
                        {
                            returnObjects.QualifiedObjectList.Add(Activator.CreateInstance(Type.GetType(sp.TypeName)));
                        }
                        AddToServiceMethodLog(ref logParameters, sp.Log, sp.TypeName, sp.QualifiedName, returnObjects.QualifiedObjectList.Last().ToString());
                    }
                    else
                    {
                        if (sp.TypeName == typeof(string).FullName)
                        {
                            returnObjects.QualifiedObjectList.Add(defaultValue);
                        }
                        
                        else if (sp.IsEnum)
                        {
                            Type enumType = wrapperProxyInfo.proxy.ProxyType.Assembly.GetType(sp.TypeName);
                            int intval;
                            if (Int32.TryParse(defaultValue, out intval))
                            {
                                returnObjects.QualifiedObjectList.Add(Enum.ToObject(enumType, intval));
                            }
                            else
                            {
                                returnObjects.QualifiedObjectList.Add(Enum.Parse(enumType, defaultValue));
                            }
                        }
                        else
                        {
                            returnObjects.QualifiedObjectList.Add(Convert.ChangeType(defaultValue, Type.GetType(sp.TypeName)));
                        }
                        AddToServiceMethodLog(ref logParameters, sp.Log, sp.TypeName, sp.QualifiedName, returnObjects.QualifiedObjectList.Last().ToString());
                    }
                }
                else
                {
                    foreach (string param in parameterSpecified)
                    {
                        if (sp.IsEnum)
                        {
                            Type enumType = wrapperProxyInfo.proxy.ProxyType.Assembly.GetType(sp.TypeName);
                            int intval;
                            if (Int32.TryParse(param, out intval))
                            {
                                returnObjects.QualifiedObjectList.Add(Enum.ToObject(enumType, intval));
                            }
                            else
                            {
                                returnObjects.QualifiedObjectList.Add(Enum.Parse(enumType, param));
                            }
                        }
                        else
                        {
                            returnObjects.QualifiedObjectList.Add(Convert.ChangeType(param, Type.GetType(sp.TypeName)));
                        }
                        AddToServiceMethodLog(ref logParameters, sp.Log, sp.TypeName, sp.QualifiedName, returnObjects.QualifiedObjectList.Last().ToString());
                    }
                }
                
            }
            else
            {
                if (parameterType == null)
                {
                    parameterType = wrapperProxyInfo.proxy.ProxyType.Assembly.GetType(sp.TypeName);
                }
                if (parameterType.IsArray)
                {
                    QualifiedObjects child = children[0];

                    MethodInfo castMethod = this.GetType().GetMethod("CastToArray").MakeGenericMethod(parameterType.GetElementType());
                    returnObjects.QualifiedObjectList.Add(castMethod.Invoke(null, new object[] { child.QualifiedObjectList.ToArray() }));
                    
                    var test = child.QualifiedObjectList.ToArray();
                    
                    AddToServiceMethodLog(ref logParameters, sp.Log, sp.TypeName, sp.QualifiedName, returnObjects.QualifiedObjectList.Last().ToString());
                }
                else if (parameterType.BaseType != null && parameterType.BaseType.IsGenericType && parameterType.BaseType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    QualifiedObjects child = children[0];
                    object list = Activator.CreateInstance(parameterType);
                    foreach (object obj in child.QualifiedObjectList)
                    {
                        parameterType.GetMethod("Add").Invoke(list, new[] { obj });
                    }
                    returnObjects.QualifiedObjectList.Add(list);
                    AddToServiceMethodLog(ref logParameters, sp.Log, sp.TypeName, sp.QualifiedName, returnObjects.QualifiedObjectList.Last().ToString());
                }
                else
                {
                    object parentObj = Activator.CreateInstance(parameterType);
                    foreach (QualifiedObjects child in children)
                    {
                        foreach (object obj in child.QualifiedObjectList)
                        {
                            string childTypeName = (from q in dbContext.QualifiedNames
                                                    join t in dbContext.ServiceTypes on q.Id equals t.QualifiedName_Id
                                                    join i in dbContext.InputParamaters on t.InputParamater.Id equals i.Id
                                                    join m in dbContext.ServiceMethods on i.ServiceMethod.Id equals m.Id
                                                    where q.Name == child.QualifiedName && m.Id == serviceMethodId
                                                    select t.TypeName).First();
                            Type childType = Type.GetType(childTypeName, false);
                            if (childType == null)
                            {
                                childType = wrapperProxyInfo.proxy.ProxyType.Assembly.GetType(childTypeName);
                            }
                            parameterType.GetMethod("set_" + child.QualifiedName.Split('.').Last()).Invoke(parentObj, new[] { Convert.ChangeType(obj, childType) });
                        }
                    }
                    returnObjects.QualifiedObjectList.Add(parentObj);
                    AddToServiceMethodLog(ref logParameters, sp.Log, sp.TypeName, sp.QualifiedName, returnObjects.QualifiedObjectList.Last().ToString());
                }
            }
            return returnObjects;
        }

        
        /// <summary>
        /// This method will make a service call using the default values specified in the wrapper for the service method.
        /// </summary>
        /// <param name="IsTest">boolean</param>
        /// <param name="customerId">AtiWrapperServices wrapper customerId</param>
        /// <param name="wrapperName">AtiWrapperServices wrapper Name</param>
        /// <returns>WrapperResult</returns>
        public WrapperResult MakeWrapperCallWithDefaults(AtiWrapperServicesEnums.ServiceMode serviceMode, int customerId, string wrapperName)
        {
            WrapperResult wrapperResult = new WrapperResult();

            using (AtiWrapperServicesModel dbContext = new AtiWrapperServicesModel())
            {
                var serviceProperties = (from w in dbContext.WrapperMethods
                                         join s in dbContext.ServiceMethods on w.ServiceMethod.Id equals s.Id
                                         join n in dbContext.ServiceNames on s.ServiceName.Id equals n.Id
                                         join u in dbContext.ServiceURLs on n.ServiceURL.Id equals u.Id
                                         join c in dbContext.Customers on u.Customer.Id equals c.Id
                                         where w.Name == wrapperName && c.Id == customerId
                                         select new { Id = s.Id, MethodName = s.Name, ServiceName = n.Name, ServiceProxyType = n.ServiceType }).FirstOrDefault();
                if (serviceProperties == null)
                {
                    wrapperResult.ResultStatus = WrapperResult.ATIWrapperServiceStatusEnum.WrapperNotFound;
                    wrapperResult.FailureReason = string.Format("No Wrapper: {0} for Customer: {1}", wrapperName, customerId);
                    return wrapperResult;
                }

                MapMessageHeaderValuesAndDefaults(serviceMode, serviceProperties.Id, wrapperName, customerId, new ParameterDictionary(), dbContext);
                Object[] args;
                try
                {
                    args = NewMapWrapperParameterValuesAndDefaults(serviceMode, serviceProperties.Id, wrapperName, customerId, new ParameterDictionary(), dbContext);
                }
                catch (Exception a)
                {
                    wrapperResult.ResultStatus = WrapperResult.ATIWrapperServiceStatusEnum.Other;
                    wrapperResult.FailureReason = String.Format("Exception: {0}", a.Message);
                    if (a.InnerException != null)
                    {
                        wrapperResult.FailureReason += String.Format(" Inner Exception: {0}", a.InnerException.Message);
                    }
                    return wrapperResult;
                }

                ProxyInformation wrapperProxyInfo;
                if (serviceMode.Equals(AtiWrapperServicesEnums.ServiceMode.Production))
                {
                    wrapperProxyInfo = ServiceProxies.ProductionProxies[serviceProperties.Id];  
                }
                else
                {
                    wrapperProxyInfo = ServiceProxies.TestProxies[serviceProperties.Id];
                }

                //In development Mode log all of the parameters
                if (serviceMode.Equals(AtiWrapperServicesEnums.ServiceMode.Development))
                {
                    int i = 0;
                    foreach (object arg in args)
                    {
                        i++;
                        ServiceProxies.WrapperTrace.TraceEvent(TraceEventType.Information, (1020 + i), string.Format("{0}", arg.ToString()));
                    }
                }
                object result = null;
                try
                {
                    if (serviceProperties.ServiceProxyType.Equals((int)DynamicServiceProxyNamespace.DynamicServiceProxyFactoryOptions.ServiceProxyTypes.Wsdl))
                    {
                        result = wrapperProxyInfo.proxy.CallMethod(serviceProperties.MethodName.Split('.').Last(), args);
                    }
                    else //if (serviceProperties.ServiceProxyType.Equals((int)DynamicServiceProxy.DynamicServiceProxyFactoryOptions.ServiceProxyTypes.Xml)))
                    {
                        result = wrapperProxyInfo.proxy.CallMethod(serviceProperties.ServiceName + "_" + serviceProperties.MethodName + "_Proxy", args);
                    }
                }
                catch (Exception e)
                {
                    wrapperResult.ResultStatus = WrapperResult.ATIWrapperServiceStatusEnum.Other;
                    wrapperResult.FailureReason = String.Format("Exception: {0}", e.Message);
                    if (e.InnerException != null)
                    {
                        wrapperResult.FailureReason += String.Format(" Inner Exception: {0}", e.InnerException.Message);
                    }
                }
                if (result != null)
                {
                    wrapperResult.ResultStatus = WrapperResult.ATIWrapperServiceStatusEnum.Success;
                    XmlSerializer rs = new XmlSerializer(result.GetType());
                    StringWriter resultText = new StringWriter();
                    rs.Serialize(resultText, result);
                    wrapperResult.Result = resultText.ToString();
                }
            }
            return wrapperResult;
        }

        /// <summary>
        /// This method will make a service call using the wrapper and any parameters specified in the xml format of the parameters argument.
        /// </summary>
        /// <param name="IsTest">boolean</param>
        /// <param name="customerId">AtiWrapperServices wrapper customerId</param>
        /// <param name="wrapperName">AtiWrapperServices wrapper Name</param>
        /// <param name="parameters">AtiWrapperServices string parameters</param>
        /// <returns>WrapperResult</returns>
        public WrapperResult MakeWrapperCallWithParameters(AtiWrapperServicesEnums.ServiceMode serviceMode, int customerId, string wrapperName, string parameters)
        {
            ServiceProxies.WrapperTrace.TraceEvent(TraceEventType.Information, 3000, string.Format("MakeWrapperCallWithParameters start: {0}", wrapperName));
            WrapperResult wrapperResult = new WrapperResult();

            using (AtiWrapperServicesModel dbContext = new AtiWrapperServicesModel())
            {
                var serviceProperties = (from w in dbContext.WrapperMethods
                                         join r in dbContext.WrapperReturnTypes on w.Id equals r.WrapperMethod.Id
                                         join s in dbContext.ServiceMethods on w.ServiceMethod.Id equals s.Id
                                         join n in dbContext.ServiceNames on s.ServiceName.Id equals n.Id
                                         join u in dbContext.ServiceURLs on n.ServiceURL.Id equals u.Id
                                         join c in dbContext.Customers on u.Customer.Id equals c.Id
                                         where w.Name == wrapperName && c.Id == customerId
                                         select new { Id = s.Id, MethodName = s.Name, ServiceName = n.Name, ServiceProxyType = n.ServiceType, ReturnEncoding = r.ReturnEncoding, TestUrl = u.TestURL, ProductionUrl = u.ProductionURL }).FirstOrDefault();

                if (serviceProperties == null)
                {
                    ServiceProxies.WrapperTrace.TraceEvent(TraceEventType.Error, 3050, string.Format("MakeWrapperCallWithParameters No Wrapper: {0} for Customer: {1}", wrapperName, customerId));
                    wrapperResult.ResultStatus = WrapperResult.ATIWrapperServiceStatusEnum.WrapperNotFound;
                    wrapperResult.FailureReason = string.Format("No Wrapper: {0} for Customer: {1}", wrapperName, customerId);
                    return wrapperResult;
                }

                string url = string.Empty;
                if (ServiceProxies.AppSettings["WrapperServiceMode"] == "Test" || ServiceProxies.AppSettings["WrapperServiceMode"] == "Development")
                {
                    url = serviceProperties.TestUrl;
                }
                else if (ServiceProxies.AppSettings["WrapperServiceMode"] == "Production")
                {
                    url = serviceProperties.ProductionUrl;
                }

                if (url == string.Empty)
                {
                    ServiceProxies.WrapperTrace.TraceEvent(TraceEventType.Critical, 3060, "url is not defined");
                    WrapperResult error = new WrapperResult();
                    error.FailureReason = "url is not defined";
                    error.ResultStatus = WrapperResult.ATIWrapperServiceStatusEnum.Other;
                    return error;
                }
                ServiceProxies.WrapperTrace.TraceEvent(TraceEventType.Information, 3070, string.Format("DBServer: {0} using url: {1}", ServiceProxies.DBServer, url));
                ParameterDictionary parameterDictionary;
                try
                {
                    //TODO need to obscure entires just like log 3100
                    //ServiceProxies._trace.TraceEvent(TraceEventType.Information, 3075, string.Format("MakeWrapperCallWithParameters ParameterDictionary {0}", parameters));
                    parameterDictionary = CreateParameterDictionary(parameters);
                }
                catch (Exception pde)
                {
                    ServiceProxies.WrapperTrace.TraceEvent(TraceEventType.Error, 3150, string.Format("MakeWrapperCallWithParameters Parameter Dictionary failed: {0} ", wrapperName));
                    wrapperResult.ResultStatus = WrapperResult.ATIWrapperServiceStatusEnum.Other;
                    wrapperResult.FailureReason = String.Format("Exception: {0}", pde.Message);
                    return wrapperResult;
                }

                System.Text.StringBuilder logParameters = new System.Text.StringBuilder();
                logParameters.Append(string.Format("Wrapper Method: {0} Parameters: ", wrapperName));
                //log the parameters that can be logged
                foreach (ParamTagName p in parameterDictionary.Parameters.Values)
                {
                    int Log = (from t in dbContext.ServiceTypes
                               join q in dbContext.QualifiedNames on t.QualifiedName.Id equals q.Id
                               join i in dbContext.InputParamaters on t.InputParamater.Id equals i.Id
                               join m in dbContext.ServiceMethods on i.ServiceMethod.Id equals m.Id
                               join w in dbContext.WrapperMethods on m.Id equals w.ServiceMethod.Id
                               where w.Name == wrapperName && q.Name == p.Tag
                               select t.LogParameter).FirstOrDefault();

                    // In Development mode show all of the parameters
                    if (Log == 0 || ServiceProxies.AppSettings["WrapperServiceMode"] == "Development")
                    {
                        // This need to move down into addChildern. need to add some comments!!!
                        logParameters.Append(string.Format("{0} = {1}, ", p.Tag, p.Name));
                    }
                    else
                    {
                        string blank = p.Name;
                        blank = System.Text.RegularExpressions.Regex.Replace(blank, ".", "#");
                        logParameters.Append(string.Format("{0} = {1}, ", p.Tag, blank));
                    }
                }

                ServiceProxies.WrapperTrace.TraceEvent(TraceEventType.Information, 3100, logParameters.ToString());

                var paramTags = (from t in parameterDictionary.Parameters.Values
                                 select t.Tag).ToList();
                if (CheckRequiredParameters(wrapperName, customerId, paramTags, dbContext, wrapperResult))
                {

                    MapMessageHeaderValuesAndDefaults(serviceMode, serviceProperties.Id, wrapperName, customerId, parameterDictionary, dbContext);

                    Object[] args;
                    try
                    {
                        args = NewMapWrapperParameterValuesAndDefaults(serviceMode, serviceProperties.Id, wrapperName, customerId, parameterDictionary, dbContext);

                        //TODO: Uncomment this code if you need to see all of the parameter values
                        //if (ServiceProxies.appSettings["WrapperServiceMode"] == "Development")
                        //{
                        //    foreach (object a in args)
                        //    {
                        //        ServiceProxies._trace.TraceEvent(TraceEventType.Information, 3101, a.ToString());
                        //    }
                        //}

                    }
                    catch (Exception a)
                    {
                        wrapperResult.ResultStatus = WrapperResult.ATIWrapperServiceStatusEnum.Other;
                        wrapperResult.FailureReason = String.Format("Exception: {0}", a.Message);
                        if (a.InnerException != null)
                        {
                            wrapperResult.FailureReason += String.Format(" Inner Exception: {0}", a.InnerException.Message);
                        }
                        return wrapperResult;
                    }



                    ProxyInformation wrapperProxyInfo;
                    if (serviceMode.Equals(AtiWrapperServicesEnums.ServiceMode.Production))
                    {
                        wrapperProxyInfo = ServiceProxies.ProductionProxies[serviceProperties.Id];
                       
                    }
                    else
                    {
                        wrapperProxyInfo = ServiceProxies.TestProxies[serviceProperties.Id];
                    }
                    object result = null;
                    if (ServiceProxies.CardTraceFlag == 1)
                    {
                        ServiceProxies.CardTrace.TraceEvent(TraceEventType.Start, 300000, "Start Card Log Event");
                        ServiceProxies.CardTrace.TraceEvent(TraceEventType.Information, 310000, string.Format("Arguments: {0}", String.Join(", ", args.Select(o => o.ToString()))));
                        ServiceProxies.CardTrace.TraceEvent(TraceEventType.Stop, 399999, "Start Card Log Event");
                    }
                    ServiceProxies.WrapperTrace.TraceEvent(TraceEventType.Information, 10001, string.Format("ServiceId: {1}, ProxyName: {0}", serviceProperties.Id, wrapperProxyInfo.proxy.ObjectType.AssemblyQualifiedName));
                    ServiceProxies.WrapperTrace.TraceEvent(TraceEventType.Information, 10002, string.Format("Type: {0}", wrapperProxyInfo.proxy.GetType().ToString()));
                    try
                    {
                        if (ServiceProxies.AppSettings["WrapperServiceMode"] == "Development")
                        {
                            ServiceProxies.WrapperTrace.TraceEvent(TraceEventType.Information, 99999, string.Format("Arguments: {0} ", args.ToString()));
                        }

                        if (serviceProperties.ServiceProxyType.Equals((int)DynamicServiceProxyNamespace.DynamicServiceProxyFactoryOptions.ServiceProxyTypes.Wsdl))
                        {
                            ServiceProxies.WrapperTrace.TraceEvent(TraceEventType.Information, 10010, string.Format("ProxyCallMethod: {0} ", serviceProperties.MethodName.Split('.').Last()));
                            result = wrapperProxyInfo.proxy.CallMethod(serviceProperties.MethodName.Split('.').Last(), args);
                            ServiceProxies.WrapperTrace.TraceEvent(TraceEventType.Information, 10011, string.Format("ProxyCallMethod: {0} Result: {1}", serviceProperties.MethodName.Split('.').Last(), result.ToString()));
                        }
                        else //if (serviceProperties.ServiceProxyType.Equals((int)DynamicServiceProxy.DynamicServiceProxyFactoryOptions.ServiceProxyTypes.Xml)))
                        {
                            ServiceProxies.WrapperTrace.TraceEvent(TraceEventType.Information, 10012, string.Format("ProxyCallMethod: {0}", serviceProperties.ServiceName + "_" + serviceProperties.MethodName + "_Proxy"));
                            result = wrapperProxyInfo.proxy.CallMethod(serviceProperties.ServiceName + "_" + serviceProperties.MethodName + "_Proxy", args);
                            ServiceProxies.WrapperTrace.TraceEvent(TraceEventType.Information, 10013, string.Format("ProxyCallMethod: {0} Result: {1}", serviceProperties.MethodName.Split('.').Last(), result.ToString()));
                        }
                    }
                    catch (Exception e)
                    {
                        wrapperResult.ResultStatus = WrapperResult.ATIWrapperServiceStatusEnum.Other;
                        wrapperResult.FailureReason = String.Format("Exception: {0}", e.Message);
                        if (e.InnerException != null)
                        {
                            wrapperResult.FailureReason += String.Format(" Inner Exception: {0}", e.InnerException.Message);
                        }
                    }
                    if (result != null)
                    {
                        wrapperResult.ResultStatus = WrapperResult.ATIWrapperServiceStatusEnum.Success;
                        if (serviceProperties.ServiceProxyType.Equals((int)DynamicServiceProxyNamespace.DynamicServiceProxyFactoryOptions.ServiceProxyTypes.Wsdl))
                        {
                            ServiceProxies.WrapperTrace.TraceEvent(TraceEventType.Information, 4000, string.Format("MakeWrapperCallWithParameters Return Encoding: {0}", serviceProperties.ReturnEncoding));
                            if (serviceProperties.ReturnEncoding.Equals((int)WrapperResult.ReturnEncodingEnum.Xml))
                            {
                                XmlSerializer rs = new XmlSerializer(result.GetType());
                                using (StringWriter resultText = new StringWriter())
                                {
                                    rs.Serialize(resultText, result);
                                    wrapperResult.Result = resultText.ToString();
                                }
                            }
                            else if (serviceProperties.ReturnEncoding.Equals((int)WrapperResult.ReturnEncodingEnum.Obj))
                            {
                                wrapperResult.Result = result;
                            }
                            else
                            {
                                wrapperResult.ResultStatus = WrapperResult.ATIWrapperServiceStatusEnum.Other;
                                wrapperResult.FailureReason = "Return Encoding not specified";
                                wrapperResult.Result = result;
                            }
                        }
                        else //if (serviceProperties.ServiceProxyType.Equals((int)DynamicServiceProxy.DynamicServiceProxyFactoryOptions.ServiceProxyTypes.Xml)))
                        {
                            if (serviceProperties.ReturnEncoding.Equals((int)WrapperResult.ReturnEncodingEnum.Xml))
                            {
                                wrapperResult.Result = result.ToString();
                            }
                            else if (serviceProperties.ReturnEncoding.Equals((int)WrapperResult.ReturnEncodingEnum.Obj))
                            {
                                wrapperResult.Result = result;
                            }
                            else
                            {
                                wrapperResult.ResultStatus = WrapperResult.ATIWrapperServiceStatusEnum.Other;
                                wrapperResult.FailureReason = "Return Encoding not specified";
                                wrapperResult.Result = result;
                            }
                        }
                    }
                }
            }
            return wrapperResult;
        }
    }

    /// <summary>
    /// simple storage class used in the parameter dictionary
    /// </summary>
    public class ParamTagName
    {
        public string Tag {get; set;}
        public string Name {get; set;}
    }

    /// <summary>
    /// Dictionary class used for parameters
    /// </summary>
    public class ParameterDictionary
    {
        public Dictionary<int, ParamTagName> Parameters = new Dictionary<int, ParamTagName>();
    }

}