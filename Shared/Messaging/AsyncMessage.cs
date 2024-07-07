namespace Azf.Shared.Messaging;

public abstract class AsyncMessage
{
    public string MessageId { get; set; }

    public string TypeName => this.GetType().Name;
}