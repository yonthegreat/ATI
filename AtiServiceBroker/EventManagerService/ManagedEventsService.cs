using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Ati.ServiceHost;
using Ati.ServiceHost.Web;
using System.Reflection;

namespace EventManagerService
{
    [ExportService("ManagedEventsService", typeof(ManagedEventsService))]
    public class ManagedEventsService : IEventManagerService
    {
        public EventResult CreateAllEvents()
        {
            EventResult result = new EventResult();
            return result;
        }

        public EventResult StartEventHandlerById(int customerId, int eventId)
        {
            EventResult result = new EventResult();
            return result;
        }

        public EventResult StartEventHandlerByName(string customerName, string eventName)
        {
            EventResult result = new EventResult();
            return result;
        }


        public EventResult StopEventHandlerById(int customerId, int eventId)
        {
            EventResult result = new EventResult();

            result.Status = EventResult.ResultStatus.Stopped;
            return result;
        }

        
        public EventResult StopEventHandlerByName(string customerName, string eventName)
        {
            EventResult result = new EventResult();

            result.Status = EventResult.ResultStatus.Stopped;
            return result;
        }


        public EventResult CreateEventValuesOnly(string typeName, IEnumerable<object> parameterValues)
        {
            EventResult result = new EventResult();
            Assembly assembly = Assembly.GetExecutingAssembly();
            Type eventType = assembly.GetType(typeName);
            object theEvent = Activator.CreateInstance(eventType, parameterValues.ToArray());
            if (theEvent != null)
            {
                result.Status = EventResult.ResultStatus.ReadyToStart;
                result.Result = theEvent;
            }
            return result;
        }


        public EventResult CreateEventNamesAndValues(string typeName, IEnumerable<string> parameterNames, IEnumerable<object> parameterValues)
        {
            EventResult result = new EventResult();
            if (parameterNames.Count() != parameterValues.Count())
            {
                result.Status = EventResult.ResultStatus.ParameterMismatch;
                return result;
            }
            Assembly assembly = Assembly.GetExecutingAssembly();
            Type eventType = assembly.GetType(typeName);
            object theEvent = Activator.CreateInstance(eventType);

            int index = 0;
            var values = parameterValues.ToArray();
            foreach (string parameterName in parameterNames)
            {
                PropertyInfo prop = theEvent.GetType().GetProperty(parameterName, BindingFlags.Public | BindingFlags.Instance);
                if (null != prop && prop.CanWrite)
                {
                    prop.SetValue(theEvent, values[index], null);
                }
            }
            if (theEvent != null)
            {
                result.Status = EventResult.ResultStatus.ReadyToStart;
                result.Result = theEvent;
            }
            return result;
        }
    }
}
