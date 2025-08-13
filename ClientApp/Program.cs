using System;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using Shared.Protos;

Console.WriteLine("Client starting...");

// Create channels for both services
var orderSvcUrl = "https://localhost:5001";
var trackingSvcUrl = "https://localhost:6001";

// Note: ensure 'dotnet dev-certs https --trust' was run earlier
using var orderChannel = GrpcChannel.ForAddress(orderSvcUrl);
using var trackingChannel = GrpcChannel.ForAddress(trackingSvcUrl);

var orderClient = new Order.OrderClient(orderChannel);
var trackingClient = new Tracking.TrackingClient(trackingChannel);

// Create an order
var newOrderReq = new OrderRequest
{
    OrderId = Guid.NewGuid().ToString("N"),
    CustomerName = "Tarikul",
    Address = "Dhaka, Bangladesh"
};

Console.WriteLine($"Creating order {newOrderReq.OrderId}...");

var created = await orderClient.CreateOrderAsync(newOrderReq);
Console.WriteLine($"Order created: {created.OrderId}, Status: {created.Status}, Time: {created.Timestamp}");

// Subscribe to tracking stream (server-side streaming)
Console.WriteLine($"Subscribing to tracking updates for order {newOrderReq.OrderId}...");
using var streamingCall = trackingClient.TrackOrder(newOrderReq);

// Read updates from the response stream
await foreach (var update in streamingCall.ResponseStream.ReadAllAsync())
{
    Console.WriteLine($"[Update] {update.Timestamp} - {update.Status}");
}

Console.WriteLine("Tracking stream ended. Press any key to exit.");
Console.ReadKey();
