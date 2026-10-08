using Grpc.Core;
using GrpcExample;

namespace GrpcBackend.Services;

public sealed class GreeterService : Greeter.GreeterBase
{
    public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
    {
        var name = string.IsNullOrWhiteSpace(request.Name) ? "there" : request.Name.Trim();
        return Task.FromResult(new HelloReply { Message = $"Hello, {name}!" });
    }
}
