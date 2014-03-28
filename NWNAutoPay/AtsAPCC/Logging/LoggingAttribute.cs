using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using NLog;

namespace AtsAPCC.Logging
{
    public class LoggingAttribute : ActionFilterAttribute
    {
        private const string NULL_VALUE = "{null}";
        private const string OBSCURED_VALUE = "********";
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //Log begin of method
            LogActionStart(filterContext);

            base.OnActionExecuting(filterContext);
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.Exception != null && !filterContext.ExceptionHandled)
            {
                LogException(filterContext.Exception);
            }

            //Log end of method
            LogActionEnd(filterContext);

            base.OnActionExecuted(filterContext);
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            LogResultStart(filterContext);
            base.OnResultExecuting(filterContext);
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            if (filterContext.Exception != null && !filterContext.ExceptionHandled)
            {
                string resultType = GetResultType(filterContext.Result);
                LogException(filterContext.Exception, resultType);
            }

            LogResultEnd(filterContext);
            base.OnResultExecuted(filterContext);
        }

        public static void LogException(Exception exception, string message = "")
        {
            if (_logger.IsErrorEnabled)
            {
                _logger.ErrorException(message, exception);
                if (exception.InnerException != null)
                {
                    _logger.ErrorException("Inner Exception", exception.InnerException);
                }
            }
        }

        private static void LogActionStart(ActionExecutingContext filterContext)
        {
            if (_logger.IsTraceEnabled)
            {
                string message = string.Format("{0}.{1}({2}) started",
                                               filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                                               filterContext.ActionDescriptor.ActionName,
                                               GetParameters(filterContext.ActionDescriptor, filterContext.ActionParameters));
                _logger.Trace(message);
            }
        }

        private static void LogActionEnd(ActionExecutedContext filterContext)
        {
            if (_logger.IsTraceEnabled)
            {
                string message = string.Format("{0}.{1} ended - returned {2}",
                                               filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                                               filterContext.ActionDescriptor.ActionName,
                                               filterContext.Result != null
                                                   ? filterContext.Result.ToString()
                                                   : NULL_VALUE);
                _logger.Trace(message);
            }
        }

        private static string GetParameters(ActionDescriptor action, IEnumerable<KeyValuePair<string, object>> actionParameters)
        {
            var result = new StringBuilder();
            List<ParameterDescriptor> parameters = action.GetParameters().ToList();
            foreach (KeyValuePair<string, object> keyValuePair in actionParameters)
            {
                KeyValuePair<string, object> pair = keyValuePair;
                ParameterDescriptor parameter = parameters.Find(x => x.ParameterName == pair.Key);
                string value = keyValuePair.Value != null ? keyValuePair.Value.ToString() : NULL_VALUE;
                if (parameter.GetCustomAttributes(typeof(LoggingObscureAttribute), false).Length > 0)
                {
                    value = OBSCURED_VALUE;
                }

                result.Append(parameter.ParameterName);
                result.Append(": ");
                result.Append(value);
                result.Append(", ");
            }
            if (result.Length > 0)
            {
                result.Remove(result.Length - 2, 2);
            }
            return result.ToString();
        }

        private static void LogResultStart(ResultExecutingContext filterContext)
        {
            if (_logger.IsTraceEnabled)
            {
                string resultType = GetResultType(filterContext.Result);
                string message = string.Format("ActionResult: {0} started",
                                               resultType);
                _logger.Trace(message);
            }
        }

        private static string GetResultType(ActionResult actionResult)
        {
            string resultType = actionResult.GetType().Name;
            if (typeof(ViewResultBase).IsAssignableFrom(actionResult.GetType()))
            {
                resultType += " " + ((ViewResultBase)actionResult).ViewName;
            }
            return resultType;
        }

        private static void LogResultEnd(ResultExecutedContext filterContext)
        {
            if (_logger.IsTraceEnabled)
            {
                string resultType = GetResultType(filterContext.Result);
                string message = string.Format("ActionResult: {0} ended",
                                               resultType);
                _logger.Trace(message);
            }
        }
    }
}