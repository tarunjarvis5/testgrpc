using Grpc.Core;
using Grpc.Net.Client;
using GrpcService;
using GrpcService.Services;
using System;
using System.Threading.Tasks;
using static MyService.MyService;

namespace ConsoleAppClientGRPC
{
    internal class ClientClass
    {
        public ClientClass()
        {
            


        }


        public static async Task start()
        {
            // Create a gRPC channel and client
            using var channel = GrpcChannel.ForAddress("http://localhost:5000");
            var client = new MyService.MyServiceClient(channel);

            // Start a bi-directional streaming call to the server
            using var call = client.Connect();

            // Send a Connect message to the server to establish a new client connection
            var connectMessage = new Message { Message_ = "Connect" };
            await call.RequestStream.WriteAsync(connectMessage);

            // Start a new thread to read messages from the server in a loop
            var readTask = Task.Run(async () =>
            {
                try
                {
                    await foreach (var message in call.ResponseStream.ReadAllAsync())
                    {
                        Console.WriteLine($"Received message from server: {message}");
                    }
                }
                catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
                {
                    // Ignore cancelled exception
                }
            });

            // Send messages to the server in a loop
            for (int i = 0; i < 10; i++)
            {
                var message = new Message { Message_ = $"Message {i}" };
                await call.RequestStream.WriteAsync(message);
                Console.WriteLine($"Sent message to server: {message.Message_}");
                await Task.Delay(1000);
            }

            // Send a Disconnect message to the server to terminate the client connection
            var disconnectMessage = new Message { Message_ = "Disconnect" };
            await call.RequestStream.WriteAsync(disconnectMessage);

            // Wait for the read task to complete
            await readTask;
        }

    }



    }
}
