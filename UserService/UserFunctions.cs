using Azf.Shared;
using Azf.UserService.Helpers;
using Azf.UserService.Sql;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.IO;
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

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name} (result={result}, setting={settings.ExampleSetting}). This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }
    }
}
