using Azf.Shared.Messaging;

namespace Azf.UserService.Messaging
{
    public class ExampleAsyncMessage : AsyncMessage
    {
        public required int TestValue { get; set; }
    }

    public class ExampleAsyncMessageHandler
    {
    }
}
