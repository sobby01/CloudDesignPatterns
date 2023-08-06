// See https://aka.ms/new-console-template for more information
using CloudPatterns.Circuit_Breaker;
using CloudPatterns.Consistent_Hashing;
using CloudPatterns.ID_Generator;
using CloudPatterns.Rate_Limiting;
using CloudPatterns.RetryPattern;

#region ConsistentHashing-KeyValueDataStore

string[] servers = { "Server-A", "Server-B", "Server-C" };
KeyValueDataStore kvStore = new KeyValueDataStore();
kvStore.Initialize(servers, 3);

// Store and fetch data for demonstration
kvStore.StoreData("data-1", "Value for data-1");
kvStore.StoreData("data-2", "Value for data-2");
kvStore.StoreData("data-3", "Value for data-3");
kvStore.StoreData("data-4", "Value for data-4");


kvStore.FetchData("data-1");
kvStore.FetchData("data-2");
kvStore.FetchData("data-3");
kvStore.FetchData("data-4");


kvStore.StoreData("data-5", "Value for data-5");
kvStore.StoreData("data-6", "Value for data-6");

// Fetch data after node failure
kvStore.FetchData("data-1");
kvStore.FetchData("data-2");
kvStore.FetchData("data-3");
kvStore.FetchData("data-4");
kvStore.FetchData("data-5");
kvStore.FetchData("data-6");

#endregion


#region HashKey

string key = "tenant_Key";
HashIDGenerator generator = new HashIDGenerator();
string dynamoIdentifier = generator.GenerateDynamoIdentifier(key);
Console.WriteLine("Dynamo Identifier: " + dynamoIdentifier);

#endregion

#region RateLimiting


#endregion
//SlidingWindowRateLimiting3();
//SlidingWindowRateLimiting2();
FixedWindowRateLimiting();
//SlidingWindowRateLimiting();
Console.ReadKey();
//RetryPattern();
//await CircuitBreakerOperation();

#region RateLimiting

void SlidingWindowRateLimiting2()
{
    // Create a Sliding Window Rate Limiter with a limit of 10 requests per window of 1 second
    var slidingWindowRateLimiter = new SlidingWindowRateLimiter_Version2(limit: 10, windowMilliSeconds: 1000);

    // Simulate requests
    for (int i = 1; i <= 30; i++)
    {
        if (slidingWindowRateLimiter.TryAcquire())
        {
            Console.WriteLine($"Request {i} - Acquired. Processing...");
        }
        else
        {
            Console.WriteLine($"Request {i} - Rejected. Rate limit exceeded.");
        }
        //Thread.Sleep(100); // Sleep between each request
    }

    // Display the total requests processed in the sliding window
    int totalRequestsInWindow = slidingWindowRateLimiter.GetTotalRequestsInWindow();
    Console.WriteLine($"Total Requests Processed in the Sliding Window: {totalRequestsInWindow}");
}
void Forward(object packet)
{
    Console.WriteLine("Packet Forwarded: " + packet.ToString());
}

void Drop(object packet)
{
    Console.WriteLine("Packet Dropped: " + packet.ToString());
}

void SlidingWindowRateLimiting3()
{
    // Create a Sliding Window Rate Limiter with a limit of 10 requests per window of 1 second

    SlidingWindow_Version3 throttle = new SlidingWindow_Version3(3, 1, Forward, Drop);

    int packet = 0;

    for (int i = 1; i <= 25; i++)
    {
        Thread.Sleep(300); // 100 milliseconds delay, equivalent to sleep(0.1) in Python
        throttle.Handle(packet);
        packet++;
    }


}

void FixedWindowRateLimiting()
{
    // Create a Fixed Window Rate Limiter with a limit of 5 requests per window of 1 second
    var rateLimiter = new FixedWindowRateLimiter(capacity: 10, windowDuration: TimeSpan.FromSeconds(1));

    // Simulate requests for the first interval 12:00:00 - 12:00:59
    // Simulate 10 requests
    //rateLimiter.stopwatch.Start();
    for (int i = 1; i <= 15; i++)
    {
        if (rateLimiter.TryAcquire())
        {
            Console.WriteLine($"Request {i}: Acquired. Processing... {DateTime.Now}");
            // Simulate request processing
            Console.WriteLine($"Request {i}: Processing completed.");
        }
        else
        {
            Console.WriteLine($"Request {i}: Rejected. Rate limit exceeded.");
        }
        Thread.Sleep(100);
    }
}

void SlidingWindowRateLimiting()
{
    var rateLimiter = new SlidingWindowRateLimiter(3, TimeSpan.FromSeconds(1));
    // Create a Sliding Window Rate Limiter with a limit of 5 requests per window of 1 second
    //var rateLimiter = new SlidingWindowRateLimiter(limit: 2, windowMilliSeconds: 1000);

    // Simulate 10 requests
    rateLimiter.TryAcquire();
    Thread.Sleep(600);
    for (int i = 1; i <= 15; i++)
    {
        if (rateLimiter.TryAcquire())
        {
            Console.WriteLine($"Request {i}: Acquired. Processing...");
            // Simulate request processing
            Thread.Sleep(100);
        }
        else
        {
            Console.WriteLine($"Request {i}: Rejected. Rate limit exceeded.");
        }
        Thread.Sleep(200); // Sleep between each request
    }
}

void TokenBuketRateLimiting()
{
    // Create a Token Bucket Rate Limiter with a bucket capacity of 5 tokens, refilling 2 tokens every 1 second
    var rateLimiter = new TokenBucketRateLimiter(capacity: 5, refillTokens: 2, refillIntervalSeconds: 1);

    // Simulate 10 requests
    for (int i = 1; i <= 10; i++)
    {
        if (rateLimiter.TryAcquire())
        {
            Console.WriteLine($"Request {i}: Acquired. Processing...");
            // Simulate request processing
            Thread.Sleep(200);
            Console.WriteLine($"Request {i}: Processing completed.");
        }
        else
        {
            Console.WriteLine($"Request {i}: Rejected. Rate limit exceeded.");
        }
        Thread.Sleep(200); // Sleep between each request
    }
}

#endregion

#region CircuitBreaker
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

#endregion

#region RetryPattern
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

#endregion
