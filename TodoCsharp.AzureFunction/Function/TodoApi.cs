using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using System.IO;
using System.Threading.Tasks;
using TodoCsharp.Commun.Models;
using Newtonsoft.Json;
using TodoCsharp.Commun.Response;
using TodoCsharp.AzureFunction.Entities;
using System;

namespace TodoCsharp.AzureFunction.Function
{
    public static class TodoApi
    {
        [FunctionName(nameof(CreateTodo))]
        public static async Task<IActionResult> CreateTodo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            [Table("todo", Connection = "AzureWebJobsStorage")] CloudTable todoTable,
            ILogger log)
        {
            log.LogInformation("your request was successfulled.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Todo todo = JsonConvert.DeserializeObject<Todo>(requestBody);

            if(string.IsNullOrEmpty(todo?.TaskDescription))
            {
                return new BadRequestObjectResult(new Response
                {
                    isSuccess = false,
                    Mesages = "Request must have a  TaskDescription"
                });             
            }

            TodoEntity todoEntity = new TodoEntity
            {
                CreatedTime = DateTime.UtcNow,
                ETag = "*",
                isCompleted = false,
                PartitionKey = "TODO",
                RowKey = Guid.NewGuid().ToString(),
                TaskDescription = todo.TaskDescription
            };

            TableOperation AddTableOperation = TableOperation.Insert(todoEntity);
            await todoTable.ExecuteAsync(AddTableOperation);

            string message = "Todo was storaged inside the table";
            log.LogInformation(message);

            return new OkObjectResult(new Response { 
            
                isSuccess = true,
                Mesages = message,
                Result = todoEntity
            
            });
        }
    }
}
