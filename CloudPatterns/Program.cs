// See https://aka.ms/new-console-template for more information
using CloudPatterns.RetryPattern;

RetryPattern();

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
