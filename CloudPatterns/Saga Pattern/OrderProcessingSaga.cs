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
            // First step of the saga
            Console.WriteLine($"Order initiated for OrderId: {order.OrderId}");
            // Update the order status to "Created"
            order.Status = "Created";
            return new SagaStepResult { IsSuccess = true };
        }

        // Compensating action for Initial step step
        public async Task RevertInitiateOrderStepAsync(Order order)
        {
            // Release the Order
            await Task.Delay(1000);
            Console.WriteLine($"Order initiated for OrderId: {order.OrderId} needs to be released");
        }
    }
}
