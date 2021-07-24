using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace HttpTriggerFunction
{
    public static class BlobStorage
    {
        [FunctionName("BlobStorage")]
        public static void Run([BlobTrigger("blobcontainersunaz/{name}", Connection = "queueconnection")]Stream myBlob, string name, ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
        }
    }
}
