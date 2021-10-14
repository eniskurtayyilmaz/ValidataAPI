using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using ValidataAPI.Utils.Common;
using ValidataAPI.Api.Middleware;
using ValidataAPI.Utils.Services;
using ValidataAPI.Utils.Exceptions;
using ValidataAPI.Utils.Models;

namespace ValidataAPI.Api.Tests.Middleware
{
    public class ExceptionHandlingMiddlewareTest
    {
        [Test]
        public void It_Should_Move_Next_When_No_Exception_Thrown()
        {
            var mockRequestDelegate = new Mock<RequestDelegate>();
            var mockJsonSerializerService = new Mock<IJsonSerializerService>();
            var mockHttpContextService = new Mock<IHttpContextService>();
            var mockLogger = new Mock<ILogger<ExceptionHandlingMiddleware>>();
            var httpContext = new DefaultHttpContext();
            var exceptionHandlingMiddleware = new ExceptionHandlingMiddleware(mockRequestDelegate.Object
                , mockJsonSerializerService.Object, mockHttpContextService.Object, mockLogger.Object);
            exceptionHandlingMiddleware.InvokeAsync(httpContext);
            mockRequestDelegate.Verify(rd => rd.Invoke(httpContext), Times.Once);
        }

        [Test]
        public async Task It_Should_Return_Error_Response_With_404_Status_When_Domain_Not_Found_Exception_Thrown()
        {
            const string domain = "domain";
            const string id = "id";
            const string traceId = "traceId";
            var mockRequestDelegate = new Mock<RequestDelegate>();
            var mockJsonSerializerService = new Mock<IJsonSerializerService>();
            var mockHttpContextService = new Mock<IHttpContextService>();
            var mockLogger = new Mock<ILogger<ExceptionHandlingMiddleware>>();
            var httpContext = new DefaultHttpContext();
            var domainNotFoundException = new DomainNotFoundException(domain, id);
            mockRequestDelegate.Setup(rd => rd.Invoke(httpContext))
                .Throws(domainNotFoundException);

            mockHttpContextService.Setup(hs =>
                hs.GetContextItemValue(Constants.TraceIdHeaderName)).Returns(traceId);

            mockJsonSerializerService.Setup(j => j.Serialize(It.IsAny<ErrorResponse>()))
                .Returns("{}");

            var exceptionHandlingMiddleware = new ExceptionHandlingMiddleware(mockRequestDelegate.Object
                , mockJsonSerializerService.Object, mockHttpContextService.Object, mockLogger.Object);
            await exceptionHandlingMiddleware.InvokeAsync(httpContext);

            Assert.AreEqual(httpContext.Response.ContentType, Constants.ContentTypeApplicationJson);
            Assert.AreEqual(httpContext.Response.StatusCode, 404);
        }

        [Test]
        public async Task It_Should_Return_Error_Response_With_500_Status_When_Unhandled_Exception_Thrown()
        {
            const string traceId = "traceId";
            var mockRequestDelegate = new Mock<RequestDelegate>();
            var mockJsonSerializerService = new Mock<IJsonSerializerService>();
            var mockHttpContextService = new Mock<IHttpContextService>();
            var mockLogger = new Mock<ILogger<ExceptionHandlingMiddleware>>();
            var httpContext = new DefaultHttpContext();
            var unhandledException = new Exception();
            mockRequestDelegate.Setup(rd => rd.Invoke(httpContext))
                .Throws(unhandledException);

            mockHttpContextService.Setup(hs =>
                hs.GetContextItemValue(Constants.TraceIdHeaderName)).Returns(traceId);
            mockJsonSerializerService.Setup(j => j.Serialize(It.IsAny<ErrorResponse>()))
                .Returns("{}");
            var exceptionHandlingMiddleware = new ExceptionHandlingMiddleware(mockRequestDelegate.Object
                , mockJsonSerializerService.Object, mockHttpContextService.Object, mockLogger.Object);
            await exceptionHandlingMiddleware.InvokeAsync(httpContext);
            
            
            Assert.AreEqual(httpContext.Response.ContentType, Constants.ContentTypeApplicationJson);
            Assert.AreEqual(httpContext.Response.StatusCode, 500);
        }

        [Test]
        public async Task It_Should_Return_Error_Response_With_400_Status_When_Business_Rule_Exception_Thrown()
        {
            const string userMessage = "client mesajı";
            const string traceId = "traceId";
            var mockRequestDelegate = new Mock<RequestDelegate>();
            var mockJsonSerializerService = new Mock<IJsonSerializerService>();
            var mockHttpContextService = new Mock<IHttpContextService>();
            var mockLogger = new Mock<ILogger<ExceptionHandlingMiddleware>>();
            var httpContext = new DefaultHttpContext();
            var businessRuleException = new BusinessRuleException(userMessage);
            mockRequestDelegate.Setup(rd => rd.Invoke(httpContext))
                .Throws(businessRuleException);

            mockHttpContextService.Setup(hs =>
                hs.GetContextItemValue(Constants.TraceIdHeaderName)).Returns(traceId);

            mockJsonSerializerService.Setup(j => j.Serialize(It.IsAny<ErrorResponse>()))
                .Returns("{}");

            var exceptionHandlingMiddleware = new ExceptionHandlingMiddleware(mockRequestDelegate.Object
                , mockJsonSerializerService.Object, mockHttpContextService.Object, mockLogger.Object);
            await exceptionHandlingMiddleware.InvokeAsync(httpContext);

            Assert.AreEqual(httpContext.Response.ContentType, Constants.ContentTypeApplicationJson);
            Assert.AreEqual(httpContext.Response.StatusCode, 400);
        }
        
        [Test]
        public async Task It_Should_Return_Error_Messages_When_Given_Message()
        {
            const string userMessage = "client mesajı";
            const string traceId = "traceId";
            var errorResponse = new ErrorResponse()
            {
                StatusCode = 500,
                TraceId= traceId
            };

            errorResponse.Errors.Add(typeof(Exception).ToString(), new string[] { userMessage });
            
            var mockRequestDelegate = new Mock<RequestDelegate>();
            var mockJsonSerializerService = new Mock<IJsonSerializerService>();
            var mockHttpContextService = new Mock<IHttpContextService>();
            var mockLogger = new Mock<ILogger<ExceptionHandlingMiddleware>>();
            var httpContext = new DefaultHttpContext();
            httpContext.Response.Body = new MemoryStream();
            var unhandledException = new Exception();
            mockRequestDelegate.Setup(rd => rd.Invoke(httpContext))
                .Throws(unhandledException);

            mockHttpContextService.Setup(hs =>
                hs.GetContextItemValue(Constants.TraceIdHeaderName)).Returns(traceId);

            mockJsonSerializerService.Setup(j => j.Serialize(It.IsAny<ErrorResponse>()))
                .Returns(JsonConvert.SerializeObject(errorResponse));

            var exceptionHandlingMiddleware = new ExceptionHandlingMiddleware(mockRequestDelegate.Object
                , mockJsonSerializerService.Object, mockHttpContextService.Object, mockLogger.Object);
            await exceptionHandlingMiddleware.InvokeAsync(httpContext);


            httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            var body = new StreamReader(httpContext.Response.Body).ReadToEnd();


            var responseBody = JsonConvert.DeserializeObject<ErrorResponse>(body);

            Assert.AreEqual(httpContext.Response.ContentType, Constants.ContentTypeApplicationJson);
            Assert.AreEqual(httpContext.Response.StatusCode, 500);
            
            Assert.AreEqual(userMessage, responseBody.Errors[typeof(Exception).ToString()][0]);
            Assert.AreEqual(errorResponse.Errors.Keys.First(), responseBody.Errors.Keys.First());
        }
    }
}