using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudPatterns.Circuit_Breaker
{
    public interface ICircuitBreakerStateStore
    {
        CircuitState GetCircuitState(string circuitId);
        void SetCircuitState(string circuitId, CircuitState state);
        void IncrementFailureCount(string circuitId);
        void ResetFailureCount(string circuitId);
    }
}
