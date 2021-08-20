using System;
using System.Collections.Generic;
using System.Text;

namespace TodoCsharp.Commun.Models
{
    public class Todo
    {
        public DateTime CreatedTime { get; set; }
        public string TaskDescription { get; set; }

        public bool isCompleted { get; set; }
    }
}
