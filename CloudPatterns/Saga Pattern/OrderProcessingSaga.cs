using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudPatterns.Saga_Pattern
{
    public class OrderProcessingSaga
    {
        public SagaStepResult InitiateOrderStep(Order order)
        {
            // Perform any necessary actions for the first step of the saga
            Console.WriteLine($"Order initiated for OrderId: {order.OrderId}");
            // Update the order status to "Created" or any appropriate initial status
            order.Status = "Created";
            return new SagaStepResult { IsSuccess = true };
        }

        // Compensating action for Initial step step
        public async Task CompensateInitiateOrderStepAsync(Order order)
        {
            // Perform the compensation logic, e.g., release the Order
            await Task.Delay(1000); // Simulate async operation
            Console.WriteLine($"Order initiated for OrderId: {order.OrderId} needs to be released");
        }
    }
}
