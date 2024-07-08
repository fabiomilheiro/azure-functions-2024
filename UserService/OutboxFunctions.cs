using Azf.Shared.Messaging;
using Azf.Shared.Sql.Outbox;
using Azf.UserService.Sql;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Sql;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;


namespace Azf.UserService
{
    public static class OutboxFunctions
    {
        public class PartialOutboxMessage
        {
            public required OutboxMessageType Type { get; set; }
        }

        // Visit https://aka.ms/sqltrigger to learn how to use this trigger binding
        [FunctionName("RelayOutboxMessages")]
        public static void Run(
                [SqlTrigger("usersvc.OutboxMessages", "SqlConnectionString")]
                IReadOnlyList<SqlChange<PartialOutboxMessage>> changes,
                ILogger log)
        {
            // TODO: Must parse the outbox message as queue/topic type or simply separate tables.
            // Or parse the message type based on the PartialOutboxMessage.Type value.
            // Or just have 2 different types: OutboxQueueMessage and OutboxTopicMessage.
            // See: https://learn.microsoft.com/en-us/ef/core/modeling/inheritance
            log.LogInformation("SQL Changes: " + JsonConvert.SerializeObject(changes));

        }
    }

    public class ToDoItem
    {
        public string Id { get; set; }
        public int Priority { get; set; }
        public string Description { get; set; }
    }
}
