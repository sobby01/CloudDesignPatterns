﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudPatterns.Saga_Pattern
{
    public class OrderProcessingSagaOrchestrator
    {
        private readonly OrderProcessingSaga _saga;
        private readonly InventoryService _inventoryService;
        private readonly PaymentService _paymentService;

        public OrderProcessingSagaOrchestrator()
        {
            _saga = new OrderProcessingSaga();
            _inventoryService = new InventoryService();
            _paymentService = new PaymentService();
        }

        public async Task<SagaStepResult> ProcessOrder(Order order)
        {
            try
            {
                // Step 1: Initiate Order
                var initiateOrderResult = _saga.InitiateOrderStep(order);
                if (!initiateOrderResult.IsSuccess)
                {
                    return initiateOrderResult;
                }

                // Step 2: Reserve Inventory
                var reserveInventoryResult = await _inventoryService.ReserveInventoryStepAsync(order);
                if (!reserveInventoryResult.IsSuccess)
                {
                    // Step 2 failed, revert actions for previous steps
                    await _saga.RevertInitiateOrderStepAsync(order);
                    return reserveInventoryResult;
                }

                // Step 3: Process Payment
                var processPaymentResult = await _paymentService.ProcessPaymentStepAsync(order);
                if (!processPaymentResult.IsSuccess)
                {
                    // Step 3 failed, revert actions for previous steps
                    await _inventoryService.RevertReserveInventoryStepAsync(order);
                    await _saga.RevertInitiateOrderStepAsync(order);
                }

                return processPaymentResult;

            }
            catch (Exception ex)
            {
                return new SagaStepResult { IsSuccess = false, ErrorMessage = ex.Message };
            }
        }
    }
}
