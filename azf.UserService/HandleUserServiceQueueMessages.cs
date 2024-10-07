using System;
using System.Threading.Tasks;
using Azf.Shared.Messaging;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Azf.UserService
{
    public class HandleUserServiceQueueMessages
    {
        private readonly IMessageHandlingOrchestrator messageHandlingOrchestrator;

        public HandleUserServiceQueueMessages(IMessageHandlingOrchestrator messageHandlingOrchestrator)
        {
            this.messageHandlingOrchestrator = messageHandlingOrchestrator;
        }

        [FunctionName("HandleUserServiceQueueMessages")]
        public async Task Run(
            [ServiceBusTrigger("user", Connection = "ServiceBusConnectionString")] string message)
        {
            await this.messageHandlingOrchestrator.HandleAsync(message);
        }
    }
}
