using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudPatterns.RetryPattern
{
    public class RetryPattern
    {
        private readonly int maxRetryAttempts;
        private readonly TimeSpan retryDelay;

        public RetryPattern(int maxRetryAttempts, TimeSpan retryDelay)
        {
            this.maxRetryAttempts = maxRetryAttempts;
            this.retryDelay = retryDelay;
        }

        public async Task RetryAsync(Func<Task> asyncAction)
        {
            int retryCount = 0;

            while (retryCount < maxRetryAttempts)
            {
                try
                {
                    await asyncAction(); // Execute the asynchronous operation
                    return;              // If successful, exit the method
                }
                catch (TransientException ex)
                {
                    Console.WriteLine($"Transient Exception: {ex.Message}");

                    // Increment the retry count
                    retryCount++;

                    // Wait for the specified delay before retrying
                    await Task.Delay(retryDelay);
                }
                catch (Exception ex)
                {
                    // Handle other non-transient exceptions, if necessary
                    Console.WriteLine($"Non-Transient Exception: {ex.Message}");
                    break;
                }
            }

            Console.WriteLine("Maximum retry attempts reached. The operation failed.");
        }
    }
}
