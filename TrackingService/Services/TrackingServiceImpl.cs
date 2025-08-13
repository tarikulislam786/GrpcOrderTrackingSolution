using System;
using System.Threading.Tasks;
using Grpc.Core;
using Shared.Protos;

public class TrackingServiceImpl : Tracking.TrackingBase
{
    // Simulated status sequence
    private static readonly string[] StatusSequence = new[]
    {
        "Accepted", "Preparing", "Ready for pickup", "Out for delivery", "Delivered"
    };

    public override async Task TrackOrder(OrderRequest request, IServerStreamWriter<OrderStatus> responseStream, ServerCallContext context)
    {
        Console.WriteLine($"[TrackingService] Client subscribed to order {request.OrderId}");

        // Simulate streaming updates every 2 seconds
        foreach (var status in StatusSequence)
        {
            if (context.CancellationToken.IsCancellationRequested)
            {
                Console.WriteLine($"[TrackingService] Stream canceled for order {request.OrderId}");
                break;
            }

            var update = new OrderStatus
            {
                OrderId = request.OrderId,
                Status = status,
                Timestamp = DateTime.UtcNow.ToString("o")
            };

            await responseStream.WriteAsync(update);
            Console.WriteLine($"[TrackingService] Sent status '{status}' for order {request.OrderId}");

            // simulate delay between status changes
            await Task.Delay(TimeSpan.FromSeconds(2), context.CancellationToken);
        }

        Console.WriteLine($"[TrackingService] Finished streaming for order {request.OrderId}");
    }
}
