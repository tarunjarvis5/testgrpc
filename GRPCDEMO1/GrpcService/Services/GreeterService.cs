using Grpc.Core;
using GrpcService;
using System.Collections.Concurrent;

namespace GrpcService.Services
{

    public class MyServiceImpl : MyService.MyServiceBase
    {
        private readonly ConcurrentDictionary<string, ClientContext> clients = new ConcurrentDictionary<string, ClientContext>();
        public event EventHandler<ClientEventArgs> ClientAdded;
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        public event EventHandler<ClientEventArgs> ClientDisconnected;

        public override async Task Connect(IAsyncStreamReader<Message> requestStream, IServerStreamWriter<Message> responseStream, ServerCallContext context)
        {
            string clientId = Guid.NewGuid().ToString();
            var clientContext = new ClientContext(clientId, requestStream, responseStream, context);
            clients.TryAdd(clientId, clientContext);
            ClientAdded?.Invoke(this, new ClientEventArgs(clientId));
            await clientContext.HandleMessagesAsync(MessageReceived);
            clients.TryRemove(clientId, out _);
            ClientDisconnected?.Invoke(this, new ClientEventArgs(clientId));
        }

        public async Task SendToClientAsync(string clientId, Message message)
        {
            if (clients.TryGetValue(clientId, out ClientContext clientContext))
            {
                await clientContext.SendAsync(message);
            }
        }

        public async Task SendToAllClientsAsync(Message message)
        {
            foreach (var clientContext in clients.Values)
            {
                await clientContext.SendAsync(message);
            }
        }
    }

    public class ClientContext
    {
        public string ClientId { get; }
        private readonly IAsyncStreamReader<Message> requestStream;
        private readonly IServerStreamWriter<Message> responseStream;
        private readonly ServerCallContext context;

        public ClientContext(string clientId, IAsyncStreamReader<Message> requestStream, IServerStreamWriter<Message> responseStream, ServerCallContext context)
        {
            ClientId = clientId;
            this.requestStream = requestStream;
            this.responseStream = responseStream;
            this.context = context;
        }

        public async Task HandleMessagesAsync(EventHandler<MessageReceivedEventArgs> messageReceivedHandler)
        {
            try
            {
                await foreach (var message in requestStream.ReadAllAsync())
                {
                    messageReceivedHandler?.Invoke(this, new MessageReceivedEventArgs(ClientId, message));
                }
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
            {
                // Ignore cancelled exception
            }
        }

        public async Task SendAsync(Message message)
        {
            try
            {
                await responseStream.WriteAsync(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message to client {ClientId}: {ex.Message}");
            }
        }
    }

    public class ClientEventArgs : EventArgs
    {
        public string ClientId { get; }

        public ClientEventArgs(string clientId)
        {
            ClientId = clientId;
        }
    }

    public class MessageReceivedEventArgs : EventArgs
    {
        public string ClientId { get; }
        public Message Message { get; }

        public MessageReceivedEventArgs(string clientId, Message message)
        {
            ClientId = clientId;
            Message = message;
        }
    }



    #region greeter

    //private readonly ILogger<GreeterService> _logger;
    //public GreeterService(ILogger<GreeterService> logger)
    //{
    //    _logger = logger;
    //}

    //public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
    //{
    //    return Task.FromResult(new HelloReply
    //    {
    //        Message = "Hello " + request.Name
    //    });
    //}

    #endregion
}
