using TrackingService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.MapGet("/", () => "TrackingService running. Communication with gRPC endpoints must be made through a gRPC client.");

// Map the tracking gRPC service
app.MapGrpcService<TrackingServiceImpl>();
app.Run();

