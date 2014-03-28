using System;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using AtsAPCC.StructureMap;
using System.Web;
using System.Diagnostics;


using AtsAPCC.Controllers;namespace AtsAPCC
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        /// <summary>
        /// setup for starting the application
        /// </summary>
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            ControllerBuilder.Current.SetControllerFactory(new StructureMapControllerFactory());
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        /// <summary>
        /// Initialize the structureMap
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
            StuctureMapInitializer.Initialize();
        }


        /// <summary>
        /// Used to re-direct the user to the appropriate Error or Maintenance page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            Server.ClearError();

            RouteData routeData = new RouteData();
            

            string code = "500";
            if (exception.GetType() == typeof(HttpException))
            {
                code = ((HttpException)exception).GetHttpCode().ToString();

                // This is a Maintenance code so re-direct to the maintenance view
                if (code == "999999")
                {
                    routeData.Values.Add("controller", "Error");
                    routeData.Values.Add("action", "Maintenance");
                }
                else if (code == "999998")
                {
                    routeData.Values.Add("controller", "Error");
                    routeData.Values.Add("action", "Index");
                    //routeData.Values.Add("exception", exception);
                    //routeData.Values.Add("statusCode", "500");
                }
                else
                {
                    routeData.Values.Add("controller", "Error");
                    routeData.Values.Add("action", "Index");
                    //routeData.Values.Add("exception", exception);
                    //routeData.Values.Add("statusCode", code);
                }

                
            }
            else
            {
                routeData.Values.Add("controller", "Error");
                routeData.Values.Add("action", "Index");
                //routeData.Values.Add("exception", exception);
                //routeData.Values.Add("statusCode", code);
            }
            AutoPayCreditController._errorTrace.TraceEvent(TraceEventType.Error, 300100, string.Format("NNWAutoPay HttpCode: {0} Error: {1}", code, exception.Message));
            AutoPayCreditController._errorTrace.TraceEvent(TraceEventType.Stop, 300200);
            IController controller = new ErrorController();
            controller.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));
        }
        
    }
}