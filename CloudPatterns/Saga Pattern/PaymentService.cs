using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudPatterns.Saga_Pattern
{
    public class PaymentService
    {
        public async Task<SagaStepResult> ProcessPaymentStepAsync(Order order)
        {
            try
            {
                // For simplicity, we'll simulate a payment failure
                await Task.Delay(1000); // Simulate async operation
                Console.WriteLine($"Processing payment for OrderId: {order.OrderId}");
                // Update the order status to "PaymentProcessed" or any appropriate status
                order.Status = "PaymentProcessed";
                // For this example, we'll return a failure result to simulate a payment failure
                return new SagaStepResult { IsSuccess = false, ErrorMessage = "Payment failed" };
            }
            catch (Exception ex)
            {
                return new SagaStepResult { IsSuccess = false, ErrorMessage = ex.Message };
            }
        }

        // Compensating action for ProcessPayment step
        public async Task CompensateProcessPaymentStepAsync(Order order)
        {
            // Perform the compensation logic, e.g., refund the payment
            await Task.Delay(1000); // Simulate async operation
            Console.WriteLine($"Payment for OrderId: {order.OrderId} needs to be refunded");
        }
    }
}
