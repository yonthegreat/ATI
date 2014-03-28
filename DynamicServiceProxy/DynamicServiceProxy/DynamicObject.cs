using System;
using System.Reflection;

namespace DynamicServiceProxyNamespace
{
    /// <summary>
    /// A Dynamic Object Class that is used to create objects that will be turned into proxies
    /// </summary>
    public class DynamicObject
    {
        /// <summary>
        /// private types
        /// </summary>
        private Type objType;
        private object obj;

        private BindingFlags CommonBindingFlags =
            BindingFlags.Instance |
            BindingFlags.Public;

        /// <summary>
        /// Dynamic Object object setter
        /// </summary>
        /// <param name="obj">object that will be turned into a Dynamic Object</param>
        public DynamicObject(Object obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            this.obj = obj;
            this.objType = obj.GetType();
        }
        /// <summary>
        /// Dynamic Object type setter
        /// </summary>
        /// <param name="objType">type</param>
        public DynamicObject(Type objType)
        {
            if (objType == null)
                throw new ArgumentNullException("objType");

            this.objType = objType;
        }

        /// <summary>
        /// Constructs a new instance of the Dynamic Object without parameters
        /// </summary>
        public void CallConstructor()
        {
            CallConstructor(new Type[0], new object[0]);
        }

        /// <summary>
        /// Constructs an new instance of the Dynamic Object using an array of parameter Types and an array of parameter Values
        /// </summary>
        /// <param name="paramTypes">Array of parameter Types</param>
        /// <param name="paramValues">Array of Parameter Values</param>
        public void CallConstructor(Type[] paramTypes, object[] paramValues)
        {
            ConstructorInfo ctor = this.objType.GetConstructor(paramTypes);
            //Hack
            var x = this.objType.GetInterfaces();

            if (ctor == null)
            {
                throw new DynamicServiceProxyException(
                        Constants.ErrorMessages.ProxyCtorNotFound);
            }

            this.obj = ctor.Invoke(paramValues);
        }

        /// <summary>
        /// Dynamic Object property getter
        /// </summary>
        /// <param name="property">name of dynamic Object property to get</param>
        /// <returns>property object</returns>
        public object GetProperty(string property)
        {
            object retval = this.objType.InvokeMember(
                property,
                BindingFlags.GetProperty | CommonBindingFlags,
                null /* Binder */,
                this.obj,
                null /* args */);

            return retval;
        }

        /// <summary>
        /// Dynamic Object property setter
        /// </summary>
        /// <param name="property">Name of Dynamic Object property to set</param>
        /// <param name="value">Value of Dynamic Object property to set</param>
        /// <returns>Dynamic Object property</returns>
        public object SetProperty(string property, object value)
        {
            object retval = this.objType.InvokeMember(
                property,
                BindingFlags.SetProperty | CommonBindingFlags,
                null /* Binder */,
                this.obj,
                new object[] { value });

            return retval;
        }

        /// <summary>
        /// Dynamic Object field getter
        /// </summary>
        /// <param name="field">Name of Dynamic Object field to get</param>
        /// <returns>Dynamic Object field</returns>
        public object GetField(string field)
        {
            object retval = this.objType.InvokeMember(
                field,
                BindingFlags.GetField | CommonBindingFlags,
                null /* Binder */,
                this.obj,
                null /* args */);

            return retval;
        }

        /// <summary>
        /// Dynamic Object field setter
        /// </summary>
        /// <param name="field">Name of Dynamic Object field to set</param>
        /// <param name="value">Value of Dynamic Object field to set</param>
        /// <returns>Dynamic Object field</returns>
        public object SetField(string field, object value)
        {
            object retval = this.objType.InvokeMember(
                field,
                BindingFlags.SetField | CommonBindingFlags,
                null /* Binder */,
                this.obj,
                new object[] { value });

            return retval;
        }

        /// <summary>
        /// Dynamic Object method Invoker with parameterized Array of parameter objects
        /// </summary>
        /// <param name="method">Name of Dynamic Object method to invoke</param>
        /// <param name="parameters">parameterized Array of parameter objects of the Dynamic Object method being Invoked</param>
        /// <returns></returns>
        public object CallMethod(string method, params object[] parameters)
        {
            object retval;
            try
            {
                retval = this.objType.InvokeMember(
                    method,
                    BindingFlags.InvokeMethod | CommonBindingFlags,
                    null /* Binder */,
                    this.obj,
                    parameters /* args */);
            }
            catch (Exception e)
            {
                throw e;
            }

            return retval;
        }

        /// <summary>
        /// Dynamic Object method Invoker with parameter types and parameter values
        /// </summary>
        /// <param name="method">Name of Dynamic Object method to invoke</param>
        /// <param name="types">Array of parameter Types of the Dynamic Object method being Invoked</param>
        /// <param name="parameters">Array of parameter Values of the Dynamic Object method being Invoked</param>
        /// <returns></returns>
        public object CallMethod(string method, Type[] types,
            object[] parameters)
        {
            if (types.Length != parameters.Length)
                throw new ArgumentException(
                    Constants.ErrorMessages.ParameterValueMistmatch);

            MethodInfo mi = this.objType.GetMethod(method, types);
            if (mi == null)
                throw new ApplicationException(string.Format(
                    Constants.ErrorMessages.MethodNotFound, method));

            object retval = mi.Invoke(this.obj, CommonBindingFlags, null,
                parameters, null);

            return retval;
        }

        /// <summary>
        /// Dynamic Object Type getter
        /// </summary>
        public Type ObjectType
        {
            get
            {
                return this.objType;
            }
        }

        /// <summary>
        /// Dynamic Object Instance getter
        /// </summary>
        public object ObjectInstance
        {
            get
            {
                return this.obj;
            }
        }

        /// <summary>
        /// Dynamic Object BindingFalgs setter and getter methods
        /// </summary>
        public BindingFlags BindingFlags
        {
            get
            {
                return this.CommonBindingFlags;
            }

            set
            {
                this.CommonBindingFlags = value;
            }
        }
    }
}