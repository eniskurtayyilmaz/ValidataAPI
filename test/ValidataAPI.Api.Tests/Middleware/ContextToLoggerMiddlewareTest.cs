using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using ValidataAPI.Utils.Common;
using ValidataAPI.Api.Middleware;
using ValidataAPI.Utils.Services;

namespace ValidataAPI.Api.Tests.Middleware
{
    public class ContextToLoggerMiddlewareTest
    {
        [Test]
        public async Task It_Should_Set_TraceId_And_UserId_To_Logger_Scope()
        {
            var mockRequestDelegate = new Mock<RequestDelegate>();
            var mockLogger = new Mock<ILogger<ContextToLoggerMiddleware>>();
            var mockHttpContextService = new Mock<IHttpContextService>();
            var httpContext = new DefaultHttpContext();
            const string traceId = "traceId";
            const string userId = "userId";

            var contextToLoggerMiddleware = new ContextToLoggerMiddleware(mockRequestDelegate.Object,
                mockHttpContextService.Object, mockLogger.Object);
            mockHttpContextService.Setup(h => h.GetContextItemValue(Constants.TraceIdHeaderName))
                .Returns(traceId);

            mockHttpContextService.Setup(h => h.GetContextItemValue(Constants.UserIdHeaderName))
                .Returns(userId);

            mockLogger.Setup(l => l.BeginScope(It.IsAny<List<KeyValuePair<string, object>>>()))
                .Callback<List<KeyValuePair<string, object>>>(e =>
                    {
                        Assert.AreEqual(e[0].Key, Constants.TraceIdHeaderName);
                        Assert.AreEqual(e[0].Value, traceId);
                        Assert.AreEqual(e[1].Key, Constants.UserIdHeaderName);
                        Assert.AreEqual(e[1].Value, userId);
                    }
                );

            await contextToLoggerMiddleware.InvokeAsync(httpContext);
            mockRequestDelegate.Verify(r => r.Invoke(httpContext), Times.Once);
        }
    }
}