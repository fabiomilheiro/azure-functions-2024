using System.Collections.Generic;
using Azf.UserService.Sql;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Sql;

using Microsoft.Extensions.Logging;
using Newtonsoft.Json;


namespace Azf.UserService
{
    public static class OutboxFunctions
    {
        // Visit https://aka.ms/sqltrigger to learn how to use this trigger binding
        [FunctionName("RelayOutboxMessages")]
        public static void Run(
                [SqlTrigger($"{UserSqlDbContext.Schema}.OutboxMessage", "SqlConnectionString")] IReadOnlyList<SqlChange<ToDoItem>> changes,
                ILogger log)
        {
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
