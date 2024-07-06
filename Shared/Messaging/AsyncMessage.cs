namespace Azf.Shared.Messaging;

public abstract class AsyncMessage
{
    public required string MessageId { get; set; }

    public string TypeName => this.GetType().Name;
}