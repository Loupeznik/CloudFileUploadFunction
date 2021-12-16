using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Azure.Storage.Blobs;

namespace CloudFileUploadFunction
{
    public static class CloudFileUploadFunction
    {
        [FunctionName("CloudFileUploadFunction")]
        public static async Task<ActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "upload")] HttpRequest req)
        {

            try
            {
                var formdata = await req.ReadFormAsync();
                var file = req.Form.Files["file"];

                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);

                memoryStream.Position = 0;

                var blobClient = new BlobClient(
                    Environment.GetEnvironmentVariable("UploadEndpoint"),
                    "uploads",
                    file.FileName);

                var result = await blobClient.UploadAsync(memoryStream);

                return new OkObjectResult(result.Value);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex);
            }
        }
    }
}
