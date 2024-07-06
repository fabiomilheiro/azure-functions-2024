namespace Azf.Shared.Messaging;

public interface IAsyncMessageHandler
{
    Task HandleAsync(AsyncMessage message);
}

public abstract class AsyncMessageHandlerBase<TAsyncMessage> : IAsyncMessageHandler
    where TAsyncMessage : AsyncMessage
{
    public Task HandleAsync(AsyncMessage message)
    {
        return this.HandleAsync((TAsyncMessage)message);
    }

    protected abstract Task HandleAsync(TAsyncMessage message);
}