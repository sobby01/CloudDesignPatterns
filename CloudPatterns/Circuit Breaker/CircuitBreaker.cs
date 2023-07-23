using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudPatterns.Circuit_Breaker
{
    public class CircuitBreaker
    {
        private readonly int failureThreshold;
        private readonly TimeSpan timeoutDuration;

        private int consecutiveFailures;
        private DateTime lastFailureTime;
        private readonly ICircuitBreakerStateStore stateStore; 
        private readonly string circuitId;

        public CircuitBreaker(string serviceName, int failureThreshold, TimeSpan timeoutDuration, ICircuitBreakerStateStore stateStore)
        {
            this.failureThreshold = failureThreshold;
            this.timeoutDuration = timeoutDuration;
            this.stateStore = stateStore;
            this.circuitId = serviceName;
        }
        public async Task<T> ExecuteAsync<T>(Func<Task<T>> action)
        {
            CircuitState state = stateStore.GetCircuitState(circuitId);

            switch (state)
            {
                case CircuitState.Closed:
                    try
                    {
                        T result = await action();
                        consecutiveFailures = 0;
                        return result;
                    }
                    catch (Exception)
                    {
                        consecutiveFailures++;
                        lastFailureTime = DateTime.UtcNow;
                        if (consecutiveFailures >= failureThreshold)
                        {
                            stateStore.SetCircuitState(circuitId, CircuitState.Open);
                            Console.WriteLine("Circuit Breaker is OPEN. Requests are blocked.");
                        }
                        throw;
                    }

                case CircuitState.Open:
                    if (DateTime.UtcNow >= lastFailureTime + timeoutDuration)
                    {
                        stateStore.SetCircuitState(circuitId, CircuitState.HalfOpen);
                        Console.WriteLine("Circuit Breaker is in HALF-OPEN state. Testing the connection.");
                        return await ExecuteAsync(action);
                    }
                    else
                    {
                        throw new CircuitBreakerException("Circuit Breaker is OPEN. Requests are blocked.");
                    }

                case CircuitState.HalfOpen:
                    try
                    {
                        T result = await action();
                        consecutiveFailures = 0;
                        stateStore.SetCircuitState(circuitId, CircuitState.Closed);
                        Console.WriteLine("Circuit Breaker is CLOSED. Requests are allowed.");
                        return result;
                    }
                    catch (Exception)
                    {
                        consecutiveFailures++;
                        lastFailureTime = DateTime.UtcNow;
                        if (consecutiveFailures >= failureThreshold)
                        {
                            stateStore.SetCircuitState(circuitId, CircuitState.Open);
                            Console.WriteLine("Circuit Breaker is OPEN. Requests are blocked.");
                        }
                        throw;
                    }

                default:
                    throw new InvalidOperationException("Invalid circuit state.");
            }
        }

    }
}
