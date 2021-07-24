using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace HttpTriggerFunction
{
    public static class QueueTriggerFn
    {
        [FunctionName("QueueTriggerFn")]
        public static void Run([QueueTrigger("sunqueue", Connection = "queueconnection")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
        }
    }
}
