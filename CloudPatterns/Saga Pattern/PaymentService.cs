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
                // Simulate a payment failure
                await Task.Delay(1000);
                Console.WriteLine($"Processing payment for OrderId: {order.OrderId}");
                // Update the order status to "PaymentProcessed"
                order.Status = "PaymentProcessed";
                return new SagaStepResult { IsSuccess = false, ErrorMessage = "Payment failed" };
            }
            catch (Exception ex)
            {
                return new SagaStepResult { IsSuccess = false, ErrorMessage = ex.Message };
            }
        }

        public async Task RevertProcessPaymentStepAsync(Order order)
        {
            // Refund the payment
            await Task.Delay(1000);
            Console.WriteLine($"Payment for OrderId: {order.OrderId} needs to be refunded");
        }
    }
}
