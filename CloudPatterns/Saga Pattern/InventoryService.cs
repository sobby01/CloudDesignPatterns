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
                // For simplicity, we'll assume the reservation is successful
                await Task.Delay(1000); // Simulate async operation
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

        // Compensating action for ReserveInventory step
        public async Task CompensateReserveInventoryStepAsync(Order order)
        {
            // Perform the compensation logic, e.g., release the reserved inventory
            await Task.Delay(1000); // Simulate async operation
            Console.WriteLine($"Inventory reservation for OrderId: {order.OrderId} needs to be released");
        }
    }
}
