using Azf.Shared;
using Azf.Shared.Sql.Outbox;
using Azf.UserService.Helpers;
using Azf.UserService.Messaging;
using Azf.UserService.Sql;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Azf.UserService
{
    public class UserFunctions
    {
        private readonly IExampleService exampleService;
        private readonly UserServiceSettings settings;
        private readonly UserServiceSettings settings2;
        private readonly UserSqlDbContext db;

        public UserFunctions(
            IExampleService exampleService,
            IOptions<UserServiceSettings> settings,
            UserServiceSettings settings2,
            UserSqlDbContext db
            )
        {
            this.exampleService = exampleService;
            this.settings = settings.Value;
            this.settings2 = settings2;
            this.db = db;
        }

        [FunctionName("ListUsers")]
        public async Task<IActionResult> ListUsers(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            var result = exampleService.Execute();

            //for (var i = 0; i < 3_000; i++)
            //{
            //    var id = Guid.NewGuid();

            //    this.db.Users.Add(new Sql.Models.User
            //    {
            //        CreatedAt = DateTime.Now,
            //        Name = $"User {id}",
            //        Email = $"user{id}@mailinator.com",
            //        Id = Guid.NewGuid(),
            //        UpdatedAt = DateTime.Now,
            //    });
            //}

            //this.db.SaveChanges();

            this.db.AddOutboxQueueMessage(
                Shared.Messaging.QueueName.User,
                new ExampleAsyncMessage
                {
                    TestValue = 2000
                });
            
            await this.db.SaveChangesAsync();

            var users = await this.db.Users.ToListAsync();
            var names = string.Join(',', users.Select(u => u.Name).ToArray());
            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name} (result={result}, setting={settings.ExampleSetting}). This HTTP triggered function executed successfully." +
                $" users = {names} ({users.Count})";



            return new OkObjectResult(responseMessage);
        }
    }
}
