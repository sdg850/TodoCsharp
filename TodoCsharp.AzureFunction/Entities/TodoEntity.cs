using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace TodoCsharp.AzureFunction.Entities
{
    public class TodoEntity : TableEntity 
    {
        public DateTime CreatedTime { get; set; }
        public string TaskDescription { get; set; }

        public bool isCompleted { get; set; }


    }
}
