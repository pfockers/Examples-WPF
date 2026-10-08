using GrpcBackend.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGrpc();

var app = builder.Build();
app.MapGrpcService<GreeterService>();
app.MapGet("/", () => "This is a gRPC server. Call it using a gRPC client.");

app.Run();
