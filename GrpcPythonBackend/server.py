from concurrent import futures
import os
from pathlib import Path

import grpc

import greeting_pb2
import greeting_pb2_grpc


class GreeterService(greeting_pb2_grpc.GreeterServicer):
    def SayHello(self, request, context):
        name = request.name.strip() or "there"
        return greeting_pb2.HelloReply(message=f"Hello, {name}!")


def main():
    certificate_path = Path(os.environ["GRPC_TLS_CERTIFICATE"])
    private_key_path = Path(os.environ["GRPC_TLS_PRIVATE_KEY"])
    certificate = certificate_path.read_bytes()
    private_key = private_key_path.read_bytes()

    server = grpc.server(futures.ThreadPoolExecutor(max_workers=8))
    greeting_pb2_grpc.add_GreeterServicer_to_server(GreeterService(), server)
    credentials = grpc.ssl_server_credentials(((private_key, certificate),))
    address = os.environ.get("GRPC_BIND_ADDRESS", "[::]:7045")

    if server.add_secure_port(address, credentials) == 0:
        raise RuntimeError(f"Could not bind the gRPC server to {address}.")

    server.start()
    print(f"Python gRPC backend listening securely on {address}", flush=True)

    try:
        server.wait_for_termination()
    except KeyboardInterrupt:
        server.stop(grace=2).wait()


if __name__ == "__main__":
    main()
