using Azf.Shared.Configuration;
using Azf.Shared.Messaging;
using Azf.Shared.Sql.Outbox;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Sql;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Azf.UserService
{
    public class OutboxFunctions
    {
        private readonly IOutboxRelayer<QueueMessage> queueOutboxRelayer;
        private readonly IOutboxRelayer<TopicMessage> topicOutboxRelayer;

        public OutboxFunctions(
            IOutboxRelayer<QueueMessage> queueOutboxRelayer,
            IOutboxRelayer<TopicMessage> topicOutboxRelayer)
        {
            this.queueOutboxRelayer = queueOutboxRelayer;
            this.topicOutboxRelayer = topicOutboxRelayer;
        }

        // Visit https://aka.ms/sqltrigger to learn how to use this trigger binding
        [FunctionName("RelayOutboxQueueMessages")]
        public async Task RelayQueueMessages(
                [SqlTrigger("usersvc.QueueMessages", "SqlConnectionString")]
                IReadOnlyList<SqlChange<QueueMessage>> changes)
        {
            var queueOutboxMessages = changes
                .Where(c => c.Operation == SqlChangeOperation.Insert)
                .Select(c => c.Item)
                .ToArray();

            await this.queueOutboxRelayer.RelayMessageBatchAsync(queueOutboxMessages);
        }

        // Visit https://aka.ms/sqltrigger to learn how to use this trigger binding
        [FunctionName("RelayOutboxTopicMessages")]
        public async Task RelayTopicMessages(
                [SqlTrigger("usersvc.TopicMessages", "SqlConnectionString")]
                IReadOnlyList<SqlChange<TopicMessage>> changes)
        {
            var topicOutboxMessages = changes
                .Where(c => c.Operation == SqlChangeOperation.Insert)
                .Select(c => c.Item)
                .ToArray();

            await this.topicOutboxRelayer.RelayMessageBatchAsync(topicOutboxMessages);
        }
    }
}