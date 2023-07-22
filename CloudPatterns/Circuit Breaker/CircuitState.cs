using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudPatterns.Circuit_Breaker
{
    public enum CircuitState
    {
        Closed,
        Open,
        HalfOpen
    }
}
