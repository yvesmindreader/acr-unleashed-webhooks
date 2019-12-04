using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.Documents;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AcrUnleashed.Webhooks.Models;
namespace AcrUnleashed.Webhooks
{
    public static class OnImagePushed
    {
        [FunctionName("OnImagePushed")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "images/push")] HttpRequest req,
            [CosmosDB(databaseName: "acr", collectionName: "pushes", ConnectionStringSetting = "CosmosDbConStr")]IAsyncCollector<ImagePush> items,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            log.LogInformation(requestBody);
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            if (data != null && "ping".Equals(data.action.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                return new StatusCodeResult(204);
            }
            if (data != null && data.request != null && data.target != null)
            {
                await items.AddAsync(new ImagePush
                {
                    Id = data.request.id,
                    LoginServer = data.request.host,
                    Action = data.action,
                    TimeStamp = DateTime.UtcNow,
                    Image = data.target.repository,
                    Tag = data.target.tag
                });
                return new OkResult();
            }
            return new BadRequestObjectResult("Invalid payload received");
        }
    }
}
