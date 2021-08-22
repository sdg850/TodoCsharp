﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TodoCsharp.AzureFunction.Entities;
using TodoCsharp.Commun.Models;

namespace TodoCsharp.Test.Helpers
{
    public class TestFactory
    {
        public static TodoEntity GetTodoEntity()
        {
            return new TodoEntity
            {
                ETag = "*",
                PartitionKey = "TODO",
                RowKey = Guid.NewGuid().ToString(),
                CreatedTime = DateTime.UtcNow,
                isCompleted = false,
                TaskDescription = "Task: kill the humans."
                
            };
        }

        public static DefaultHttpRequest CreateHttpRequest(Guid Id, Todo Request)
        {
            string request = JsonConvert.SerializeObject(Request);
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = GenerateStreamFromString(request),
                Path = $"/{Id}"
            };
        }

        public static DefaultHttpRequest CreateHttpRequest(Guid Id)
        {
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Path = $"/{Id}"
            };
        }

        public static DefaultHttpRequest CreateHttpRequest(Todo Request)
        {
            string request = JsonConvert.SerializeObject(Request);
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = GenerateStreamFromString(request)
            };
        }

        public static DefaultHttpRequest CreateHttpRequest()
        {
            return new DefaultHttpRequest(new DefaultHttpContext());
        }

        public static Todo GetRequest()
        {
            return new Todo
            {
                CreatedTime = DateTime.UtcNow,
                isCompleted = false,
                TaskDescription = "Try to conquer the world."
            };
        }

        public static Stream GenerateStreamFromString(string stringToConvert)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(stringToConvert);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public static ILogger CreateLogger(LoggerTypes type = LoggerTypes.Null)
        {
            ILogger logger;
            if (type == LoggerTypes.List)
            {
                logger = new ListLogger();
            }
            else
            {
                logger = NullLoggerFactory.Instance.CreateLogger("Null Logger");
            }

            return logger;
        }
    }
}
