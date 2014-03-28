using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RulesEngine
{
    
    public class RulesResult
    {
        public enum ResultStatus { Success, Init };

        public ResultStatus Status;

        public object FailureReason;

        public object Result;

        public RulesResult()
        {
            this.Status = ResultStatus.Init;
        }
    }
}
