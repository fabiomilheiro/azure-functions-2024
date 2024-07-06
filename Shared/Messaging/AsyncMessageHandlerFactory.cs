using Microsoft.Extensions.DependencyInjection;

namespace Azf.Shared.Messaging;

public interface IAsyncMessageHandlerFactory
{
    AsyncMessageHandlerCreateResult Create(string messageTypeName);
}

public class AsyncMessageHandlerFactory : IAsyncMessageHandlerFactory
{

    private readonly IServiceProvider serviceProvider;

    public AsyncMessageHandlerFactory(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public AsyncMessageHandlerCreateResult Create(string messageTypeName)
    {
        if (!AsyncMessageMappings.ByMessageTypeName.TryGetValue(messageTypeName, out var data))
        {
            throw new BackendException(
                $"Could not find the message handler for message type '{messageTypeName}'." +
                $"{Environment.NewLine}Either correct the message type or implement the" +
                "respective handler.");
        }

        return new AsyncMessageHandlerCreateResult
        {
            MessageType = data.MessageType,
            Handler = (IAsyncMessageHandler)this.serviceProvider.GetRequiredService(data.HandlerType),
        };
    }
}

public class AsyncMessageHandlerCreateResult
{
    public IAsyncMessageHandler Handler { get; set; }

    public Type MessageType { get; set; }
}