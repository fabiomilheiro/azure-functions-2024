using Azf.Shared.Messaging;
using System;
using System.Threading.Tasks;

namespace Azf.UserService.Messaging
{
    public class ExampleAsyncMessage : AsyncMessage
    {
        public required int TestValue { get; set; }
    }

    public class ExampleAsyncMessageHandler : AsyncMessageHandlerBase<ExampleAsyncMessage>
    {
        protected override Task HandleAsync(ExampleAsyncMessage message)
        {
            Console.WriteLine($"Value: {message.TestValue}");
            
            return Task.CompletedTask;
        }
    }
}
