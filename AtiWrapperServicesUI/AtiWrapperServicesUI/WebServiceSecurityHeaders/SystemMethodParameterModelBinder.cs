using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AtiWrapperServicesUI.Models;

namespace AtiWrapperServicesUI
{
    public class SystemMethodParameterModelBinder : DefaultModelBinder
    {
        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType)
        {
            var type = bindingContext.ValueProvider.GetValue(bindingContext.ModelName + ".TypeName");
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName + ".Value");
            var name = bindingContext.ValueProvider.GetValue(bindingContext.ModelName + ".Name");
            ParameterViewModel model = new ParameterViewModel();
            model.TypeName = type.AttemptedValue;
            model.Name = name.AttemptedValue;
            switch (type.AttemptedValue)
            {
                case "System.String":
                    {
                        model.Value = Convert.ToString(value.AttemptedValue);
                        break;
                    }
                case "System.DateTime":
                    {
                        model.Value = Convert.ToDateTime(value.AttemptedValue);
                        break;
                    }
                case "System.Int":
                    {
                        model.Value = Convert.ToInt32(value.AttemptedValue);
                        break;
                    }
                default:
                    {
                        throw new NotImplementedException();
                    }
            }
            bindingContext.ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, model.GetType());
            return model;
        }
    }

    public class ParameterModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var type = bindingContext.ValueProvider.GetValue(bindingContext.ModelName + ".TypeName");
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName + ".Value");
            var name = bindingContext.ValueProvider.GetValue(bindingContext.ModelName + ".Name");
            ParameterViewModel model = new ParameterViewModel();
            model.TypeName = type.AttemptedValue;
            model.Name = name.AttemptedValue;
            Type typeType = Type.GetType(type.AttemptedValue, false);
            if (typeType != null)
            {
                if (typeType.IsArray)
                {
                    //dynamic array = Activator.CreateInstance(typeType, new object[] { 1 });
                    if (typeType.GetElementType().Equals("System.String"))
                    {
                       model.Value = Convert.ToString(value.AttemptedValue).TrimStart();
                    }
                    else
                    {
                        model.Value = Convert.ChangeType(value.AttemptedValue, typeType.GetElementType());
                    }
                }
                else
                {
                    if (type.AttemptedValue.Equals("System.String"))
                    {
                        model.Value = Convert.ToString(value.AttemptedValue).TrimStart();
                    }
                    else
                    {
                        model.Value = Convert.ChangeType(value.AttemptedValue, typeType);
                    }
                }
            }
            else
            {
                model.Value = Convert.ToInt32(value.AttemptedValue);
            }
            return model;

        }
    }

    public class XmlParameterModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var type = bindingContext.ValueProvider.GetValue(bindingContext.ModelName + ".TypeName");
            var include = bindingContext.ValueProvider.GetValue(bindingContext.ModelName + ".Include");
            var name = bindingContext.ValueProvider.GetValue(bindingContext.ModelName + ".Name");
            var isSystemType = bindingContext.ValueProvider.GetValue(bindingContext.ModelName + ".IsSystemType");
            XmlServiceMethodTypeViewModel model = new XmlServiceMethodTypeViewModel();
            model.TypeName = type.AttemptedValue;
            model.Name = name.AttemptedValue;
            // The checkbox helper added a hidden value so include has to be split
            model.Include = Convert.ToBoolean(include.AttemptedValue.Split(',').First());
            model.IsSystemType = Convert.ToBoolean(isSystemType.AttemptedValue);
            return model;

        }
    }
}