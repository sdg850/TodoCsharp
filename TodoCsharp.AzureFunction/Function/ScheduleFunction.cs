using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using TodoCsharp.AzureFunction.Entities;

namespace TodoCsharp.AzureFunction.Function
{
    public static class ScheduleFunction
    {
        [FunctionName("ScheduleFunction")]
        public static async Task Run(
            [TimerTrigger("0 */1 * * * *")]TimerInfo myTimer,
            [Table("todo", Connection = "AzureWebJobsStorage")] CloudTable todoTable,
            ILogger log)
        {
            log.LogInformation($"isCompleted Timer trigger function executing...");

            string filter = TableQuery.GenerateFilterConditionForBool("isCompleted", QueryComparisons.Equal, true);
            TableQuery<TodoEntity> query = new TableQuery<TodoEntity>().Where(filter);
            TableQuerySegment<TodoEntity> completedItems = await todoTable.ExecuteQuerySegmentedAsync(query, null);

            int deletedItems = 0;

            foreach(TodoEntity completedItem in completedItems)
            {
                await todoTable.ExecuteAsync(TableOperation.Delete(completedItem));
                deletedItems++;
            }

            log.LogInformation($"isCompleted Timer trigger function  completed and executed at: {DateTime.Now} \n Number de items deleted {deletedItems}");
        }
    }
}
