using System;
using System.Diagnostics;
using System.Web.Mvc;
using StructureMap;

namespace AtsAPCC.StructureMap
{
	public class StructureMapControllerFactory : DefaultControllerFactory
	{
        protected override IController GetControllerInstance(System.Web.Routing.RequestContext requestContext, Type controllerType)
        {
            if (controllerType == null) return base.GetControllerInstance(requestContext, controllerType);

            IController result = null;
            try
            {
                result = ObjectFactory.GetInstance(controllerType) as Controller;
            }
            catch (StructureMapException)
            {
                Debug.WriteLine(ObjectFactory.WhatDoIHave());
                throw;
            }
            return result;
        }
	}
}