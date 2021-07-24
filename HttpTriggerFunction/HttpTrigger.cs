using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace HttpTriggerFunction
{
    public static class HttpTrigger
    {
        // number of messages to be sent to the queue


        [FunctionName("HttpTrigger")]
        [return: Queue("sunqueue")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public static async Task<ValidationsComplete> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            SendArticleAsync(name).Wait(); // send message to queue
            ServiceBus.SendMessageToServiceBus().Wait(); // send message to service bus queue

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new ValidationsComplete() { FlightIds = new int[] { 1, 23, 34, 34, 56, 57 }, Successful = true };
            // return new OkObjectResult(responseMessage);
        }



        static async Task SendArticleAsync(string newsMessage)
        {
            string storageConnection = "DefaultEndpointsProtocol=https;AccountName=storageaccountsunaz;AccountKey=AHjrV1KgvRP6u+W9yuta9u/LMPJC9Gkv2j9+Vy+Q2z/LMHuuFKYFeEIVsseGbyTvAmVaOScUnH9crVZA3B81yA==;EndpointSuffix=core.windows.net";

            CloudStorageAccount account = CloudStorageAccount.Parse(storageConnection);

            CloudQueueClient client = account.CreateCloudQueueClient();

            CloudQueue queue = client.GetQueueReference("sunqueue");
            bool createdQueue = await queue.CreateIfNotExistsAsync();
            if (createdQueue)
            {

            }
            CloudQueueMessage articleMessage = new CloudQueueMessage(newsMessage);
            await queue.AddMessageAsync(articleMessage);
        }



    }
    public class ValidationsComplete
    {
        public bool Successful { get; set; }
        public IEnumerable<int> FlightIds { get; set; }
    }
}

