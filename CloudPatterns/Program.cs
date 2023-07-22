// See https://aka.ms/new-console-template for more information
using CloudPatterns.Circuit_Breaker;
using CloudPatterns.RetryPattern;

RetryPattern();
await CircuitBreakerOperation();

async Task<OperationResult<string>> CircuitBreakerOperation()
{
    // Create an instance of the state store
    ICircuitBreakerStateStore stateStore = new CircuitBreakerStateStore();

    // Circuit Breaker setup for a specific service
    var circuitBreaker = new CircuitBreaker("service1", failureThreshold: 5, timeoutDuration: TimeSpan.FromSeconds(30), stateStore);

    try
    {
        // Execute the API logic
        var result = await circuitBreaker.ExecuteAsync(async () =>
        {
            // Your API logic here...
            // For example, making a call to a database or external service
            return await SomeAsyncOperation(); // Replace this with your actual async operation
        });

        return new OperationResult<string> { IsSuccess = true, Data = "Success" };
        //return Ok(/* Success response */);
    }
    catch (CircuitBreakerException ex)
    {
        // Handle Circuit Breaker open state, maybe return a fallback response or retry later
        //return StatusCode(503, "Service is temporarily unavailable. Please try again later.");
        return new OperationResult<string> { IsSuccess = false, Data = "Failure", Message = "Error: " + ex.Message };
    }
    catch (Exception ex)
    {
        // Handle other exceptions or return an error response
        //return StatusCode(500, "An error occurred while processing your request.");
        return new OperationResult<string> { IsSuccess = false, Data = "Failure", Message = "Error: " + ex.Message };
    }
}

async Task<string> SomeAsyncOperation()
{
    await Task.Delay(1000);
    return "Success";
}

void RetryPattern()
{
    int maxRetryAttempts = 3;
    TimeSpan retryDelay = TimeSpan.FromSeconds(2);
    var retryHandler = new RetryPattern(maxRetryAttempts, retryDelay);

    // Call RetryAsync with the asynchronous operation to retry
    retryHandler.RetryAsync(AsyncOperation).Wait();

    Console.ReadLine();
}


async Task AsyncOperation()
{
    // Simulate a transient failure
    if (new Random().Next(0, 10) < 8)
    {
        throw new TransientException("Transient failure occurred.");
    }

    await Task.Delay(100); // Simulate some asynchronous work
    Console.WriteLine("Async operation succeeded!");
}
