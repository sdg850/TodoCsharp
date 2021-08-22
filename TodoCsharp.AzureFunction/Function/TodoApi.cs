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
        [FunctionName(nameof(CreateItem))]
        public static async Task<IActionResult> CreateItem(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "todo")] HttpRequest req,
            [Table("todo", Connection = "AzureWebJobsStorage")] CloudTable todoTable,
            ILogger log)
        {
            log.LogInformation("your request was successfulled.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Todo item = JsonConvert.DeserializeObject<Todo>(requestBody);

            if (string.IsNullOrEmpty(item?.TaskDescription))
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
                TaskDescription = item.TaskDescription
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

        [FunctionName(nameof(UpdateItem))]
        public static async Task<IActionResult> UpdateItem(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "todo/{id}")] HttpRequest req,
        [Table("todo", Connection = "AzureWebJobsStorage")] CloudTable todoTable,
        string id,
        ILogger log)
        {
            log.LogInformation($"your {id} was received.");
          
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Todo item = JsonConvert.DeserializeObject<Todo>(requestBody);

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
            todoEntity.isCompleted = item.isCompleted;

            if (!string.IsNullOrEmpty(item.TaskDescription))
            {
                todoEntity.TaskDescription = item.TaskDescription;
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

        [FunctionName(nameof(GetAllItems))]
        public static async Task<IActionResult> GetAllItems(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todo")] HttpRequest req,
        [Table("todo", Connection = "AzureWebJobsStorage")] CloudTable todoTable,
        ILogger log)
        {
            log.LogInformation($"Geting all the items...");

            TableQuery<TodoEntity> query = new TableQuery<TodoEntity>();
            TableQuerySegment<TodoEntity> items = await todoTable.ExecuteQuerySegmentedAsync(query, null);


            string message = $"Showing all items...";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {

                isSuccess = true,
                Mesages = message,
                Result = items

            });
        }

        [FunctionName(nameof(GetItemById))]
        public static  IActionResult GetItemById(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todo/{id}")] HttpRequest req,
        [Table("todo","TODO", "{id}", Connection = "AzureWebJobsStorage")] TodoEntity todoEntity,
        string id,
        ILogger log)
        {
            log.LogInformation($"Geting id {id} from the table...");


            if (todoEntity == null)
            {
                return new BadRequestObjectResult(new Response
                {
                    isSuccess = false,
                    Mesages = "¡Ups looks like somthing went wrong, Id no found. Try again!"

                });

            }


            string message = $"Showing id result...";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {

                isSuccess = true,
                Mesages = message,
                Result = todoEntity

            });
        }

        [FunctionName(nameof(DeleteItemById))]
        public static async Task<IActionResult> DeleteItemById(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "todo/{id}")] HttpRequest req,
        [Table("todo", "TODO", "{id}", Connection = "AzureWebJobsStorage")] TodoEntity todoEntity,
        [Table("todo", Connection = "AzureWebJobsStorage")] CloudTable todoTable,
        string id,
        ILogger log)
        {
            log.LogInformation($"Deleting id {id} from the table...");


            if (todoEntity == null)
            {
                return new BadRequestObjectResult(new Response
                {
                    isSuccess = false,
                    Mesages = "¡Ups looks like somthing went wrong, Id no found. Try again!"

                });

            }

            await todoTable.ExecuteAsync(TableOperation.Delete(todoEntity));


            string message = $"Showing id deleted...";
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
