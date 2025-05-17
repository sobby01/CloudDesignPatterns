using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudPatterns.Saga_Pattern
{
    public class InventoryService
    {
        public async Task<SagaStepResult> ReserveInventoryStepAsync(Order order)
        {
            try
            {
                // Reservation is successful
                await Task.Delay(1000);
                Console.WriteLine($"Inventory reserved for OrderId: {order.OrderId}");
                // Update the order status to "InventoryReserved" or any appropriate status
                order.Status = "InventoryReserved";
                return new SagaStepResult { IsSuccess = true };
            }
            catch (Exception ex)
            {
                return new SagaStepResult { IsSuccess = false, ErrorMessage = ex.Message };
            }
        }

        public async Task RevertReserveInventoryStepAsync(Order order)
        {
            // Release the reserved inventory
            await Task.Delay(1000);
            Console.WriteLine($"Inventory reservation for OrderId: {order.OrderId} needs to be released");
        }
    }
}
