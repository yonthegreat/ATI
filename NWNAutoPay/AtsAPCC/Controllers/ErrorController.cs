using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AtsAPCC.Models;

namespace AtsAPCC.Controllers
{
    /// <summary>
    /// Controller for Errors and Maintenance
    /// </summary>
    public class ErrorController : Controller
    {
        /// <summary>
        /// Sends the used to the Error Page
        /// </summary>
        /// <param name="statusCode"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public ActionResult Index(string statusCode, Exception exception)
        {
            Response.StatusCode = Convert.ToInt32(statusCode);
            return View("Index");
        }

        /// <summary>
        /// Sends the user to the Maintenance Page
        /// </summary>
        /// <param name="statusCode"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public ActionResult Maintenance(string statusCode, Exception exception)
        {
            Response.StatusCode = Convert.ToInt32(statusCode);
            return View("MaintenancePage");
        }

    }
}
