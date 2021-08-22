using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using TodoCsharp.AzureFunction.Entities;
using TodoCsharp.AzureFunction.Function;
using TodoCsharp.Commun.Models;
using TodoCsharp.Test.Helpers;
using Xunit;

namespace TodoCsharp.Test.Tests
{
    public class ApiTest
    {
        private readonly ILogger logger = TestFactory.CreateLogger();

        [Fact]
        public async void CreateItem_Should_Return_200()
        {
            // Arrenge
            MockCloudTableTodos mockTodos = new MockCloudTableTodos(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            Todo Request = TestFactory.GetRequest();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(Request);

            // Act
            IActionResult response = await TodoApi.CreateItem(request, mockTodos, logger);

            // Assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }



        [Fact]
        public void GetItemById_Should_Return_200()
        {
            // Arrenge
            MockCloudTableTodos mockTodos = new MockCloudTableTodos(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            logger.LogInformation($"mockTodos ------------>{mockTodos}");

            Todo Request = TestFactory.GetRequest();
            logger.LogInformation($"Request: {Request}");

            TodoEntity Entity = TestFactory.GetTodoEntity();
            logger.LogInformation($"Entity: {Entity}");

            Guid Id = Guid.NewGuid();
            logger.LogInformation($"Id: {Id}");

            DefaultHttpRequest request = TestFactory.CreateHttpRequest(Id, Request);
            logger.LogInformation($"request: {request}");

            // Act
            IActionResult response = TodoApi.GetItemById(request, Entity, Id.ToString(), logger);
            logger.LogInformation($"response: {response}");

            // Assert
            OkObjectResult result = (OkObjectResult)response;
            logger.LogInformation($"result: {result}");

            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            logger.LogInformation($" fin ----------------------------");
        }

        [Fact]
        public async void UpdateItemById_Should_Return_200()
        {
            // Arrenge
            MockCloudTableTodos mockTodos = new MockCloudTableTodos(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            Todo Request = TestFactory.GetRequest();
            Guid Id = Guid.NewGuid();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(Id, Request);

            // Act
            IActionResult response = await TodoApi.UpdateItem(request, mockTodos, Id.ToString(), logger);

            // Assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

        [Fact]
       public async void GetAllItems_Should_Return_200()
        {
         // Arrenge
          MockCloudTableTodos mockTodos = new MockCloudTableTodos(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
          Todo Request = TestFactory.GetRequest();
           DefaultHttpRequest request = TestFactory.CreateHttpRequest();

           // Act
           IActionResult response = await TodoApi.GetAllItems(request, mockTodos, logger);

           //Assert
           OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

        [Fact]
        public async void DeleteItemById_Should_Return_200()
        {
            // Arrenge
            MockCloudTableTodos mockTodos = new MockCloudTableTodos(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            Todo Request = TestFactory.GetRequest();
            TodoEntity Entity = TestFactory.GetTodoEntity();
            Guid Id = Guid.NewGuid();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(Id, Request);

            // Act
            IActionResult response = await TodoApi.DeleteItemById(request, Entity, mockTodos, Id.ToString(), logger);

            // Assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

    }
}
