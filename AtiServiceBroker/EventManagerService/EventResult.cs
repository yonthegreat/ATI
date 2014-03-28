using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagerService
{
    public class EventResult
    {
        public enum ResultStatus { Success, Initialize, Stopped, ReadyToStart, ParameterMismatch }

        public ResultStatus Status;

        public object Result;

        public EventResult()
        {
            this.Status = ResultStatus.Initialize;
            Result = null as Object;
        }
    }
}
