using System;
using System.Threading.Tasks;
using Grpc.Core;
using Shared.Protos;

public class OrderServiceImpl : Order.OrderBase
{
    public override Task<OrderStatus> CreateOrder(OrderRequest request, ServerCallContext context)
    {
        // Generate a created status
        var status = new OrderStatus
        {
            OrderId = string.IsNullOrWhiteSpace(request.OrderId) ? Guid.NewGuid().ToString("N") : request.OrderId,
            Status = "Order Created",
            Timestamp = DateTime.UtcNow.ToString("o")
        };

        // In a real system you'd persist the order and notify TrackingService (e.g., via message bus)
        Console.WriteLine($"[OrderService] Created order {status.OrderId} for {request.CustomerName}");

        return Task.FromResult(status);
    }
}
