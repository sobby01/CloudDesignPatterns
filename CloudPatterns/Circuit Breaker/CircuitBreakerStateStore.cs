using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudPatterns.Circuit_Breaker
{
    public class CircuitBreakerStateStore : ICircuitBreakerStateStore
    {
        private readonly ConcurrentDictionary<string, CircuitState> circuitStates = new ConcurrentDictionary<string, CircuitState>();
        private readonly ConcurrentDictionary<string, int> failureCounts = new ConcurrentDictionary<string, int>();

        public CircuitState GetCircuitState(string circuitId)
        {
            circuitStates.TryGetValue(circuitId, out CircuitState state);
            return state;
        }

        public void SetCircuitState(string circuitId, CircuitState state)
        {
            circuitStates[circuitId] = state;
        }

        public void IncrementFailureCount(string circuitId)
        {
            failureCounts.AddOrUpdate(circuitId, 1, (key, count) => count + 1);
        }

        public void ResetFailureCount(string circuitId)
        {
            failureCounts.TryRemove(circuitId, out _);
        }
    }
}
