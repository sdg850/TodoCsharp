using System;

namespace TodoCsharp.common.Models
{
    public internal class Todo
    {
        public DateTime CreatedTime { get; set; }
        public string TaskDescription { get; set; }

        public bool isCompleted { get; set; }
    }
}
