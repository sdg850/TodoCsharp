using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using TodoCsharp.AzureFunction.Entities;
using TodoCsharp.Commun.Models;
using TodoCsharp.Commun.Response;

namespace TodoCsharp.AzureFunction.Function
{
    public static class TodoApi
    {
        [FunctionName(nameof(CreateTodo))]
        public static async Task<IActionResult> CreateTodo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "todo")] HttpRequest req,
            [Table("todo", Connection = "AzureWebJobsStorage")] CloudTable todoTable,
            ILogger log)
        {
            log.LogInformation("your request was successfulled.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Todo todo = JsonConvert.DeserializeObject<Todo>(requestBody);

            if (string.IsNullOrEmpty(todo?.TaskDescription))
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

            return new OkObjectResult(new Response
            {

                isSuccess = true,
                Mesages = message,
                Result = todoEntity

            });
        }

        [FunctionName(nameof(UpdateTodo))]
        public static async Task<IActionResult> UpdateTodo(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "todo/{id}")] HttpRequest req,
        [Table("todo", Connection = "AzureWebJobsStorage")] CloudTable todoTable,
        string id,
        ILogger log)
        {
            log.LogInformation($"your {id} was received.");
          
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Todo todo = JsonConvert.DeserializeObject<Todo>(requestBody);

            //Check todo id

            TableOperation findOperation = TableOperation.Retrieve<TodoEntity>("TODO", id);

            log.LogInformation($"FindOperation is {findOperation}.");

            TableResult findResult = await todoTable.ExecuteAsync(findOperation);

            log.LogInformation($"your Result is {findResult}.");


            if (findResult.Result == null)
            {
                return new BadRequestObjectResult(new Response
                {
                    isSuccess = false,
                    Mesages = "¡Ups looks like somthing went wrong, Id no found. Try again!"

                });

            }

            //Update Todo
            TodoEntity todoEntity = (TodoEntity)findResult.Result;
            todoEntity.isCompleted = todo.isCompleted;

            if (!string.IsNullOrEmpty(todo.TaskDescription))
            {
                todoEntity.TaskDescription = todo.TaskDescription;
            }


            TableOperation AddTableOperation = TableOperation.Replace(todoEntity);
            await todoTable.ExecuteAsync(AddTableOperation);

            string message = $"Todo: {id} was updated";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {

                isSuccess = true,
                Mesages = message,
                Result = todoEntity

            });
        }
    }
}
