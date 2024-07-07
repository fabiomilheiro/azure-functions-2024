using Azf.Shared;
using Azf.UserService.Helpers;
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
    public class ListUsers
    {
        private readonly IExampleService exampleService;
        private readonly UserServiceSettings settings;

        public ListUsers(IExampleService exampleService, IOptions<UserServiceSettings> settings)
        {
            this.exampleService = exampleService;
            this.settings = settings.Value;
        }

        [FunctionName("Function1")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
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
