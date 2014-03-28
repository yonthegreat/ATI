using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;

namespace DynamicServiceProxyNamespace
{

    /// <summary>
    /// DynamicServiceProxy XmlServiceTypeBuilder class contains the routines that build at ServiceType that can be used by
    /// the DynamicSeverviceProxyFactory to create a DynamicProxy for XML based Services
    /// </summary>
    class XmlServiceTypeBuilder
    {

        /// <summary>
        /// Globals
        /// </summary>
        public List<XmlServiceParameters> parameters;
        public string qualifiedObjectName;
        string xmlEnvelopeName;

        /// <summary>
        /// DynamicServiceProxy XmlServiceTypeBuilder constructor
        /// </summary>
        /// <param name="parameters">DynamicServiceProxy List of XmlServiceParameters (Name and TypeName)</param>
        /// <param name="qualifiedObjectName">DynamicServiceProxy fully qualified name</param>
        /// <param name="xmlEnvelopeName">DynamicServiceProxy Name of the XML envelope</param>
        public XmlServiceTypeBuilder(List<XmlServiceParameters> parameters, string qualifiedObjectName, string xmlEnvelopeName)
        {
            this.parameters = parameters;
            this.qualifiedObjectName = qualifiedObjectName;
            this.xmlEnvelopeName = xmlEnvelopeName;
        }

        /// <summary>
        /// DynamicServiceProxy CreateNewObject creates an instance of the object
        /// </summary>
        /// <returns>object</returns>
        public object CreateNewObject()
        {
            var myType = CompileResultType();
            return Activator.CreateInstance(myType);
        }

        /// <summary>
        /// DynamicServiceProxy CompileResultType builds a result type for the XML service
        /// </summary>
        /// <returns>Type</returns>
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

        /// <summary>
        /// DynamicServiceProxy GetTypeBuilder get a TypeBuilder
        /// </summary>
        /// <param name="typeSignature">DynamicServiceProxy return Type of the method</param>
        /// <param name="methodName">DynamicServiceProxy Name of the method</param>
        /// <returns>TypeBuilder</returns>
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

        /// <summary>
        /// DynamicServiceProxy CreateProperty creates properties for the Type needed by the XML Service
        /// NOTE: it uses IL to create the getter and setter code for the  property
        /// </summary>
        /// <param name="tb">DynamicServiceProxy TypeBuilder for the type</param>
        /// <param name="propertyName">DynamicServiceProxy Name of the property</param>
        /// <param name="propertyType">DynamicServiceProxy Type of the property</param>
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
