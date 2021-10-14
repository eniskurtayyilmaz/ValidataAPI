using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using ValidataAPI.Utils.Common;
using ValidataAPI.Api.Middleware;

namespace ValidataAPI.Api.Tests.Middleware
{
    public class HeaderToContextMiddlewareTest
    {
        [Test]
        public async Task It_Should_Copy_User_And_Trace_Id_Header_To_Http_Context_Item()
        {
            var mockRequestDelegate = new Mock<RequestDelegate>();
            const string traceId = "traceId";
            const string userId = "userId";
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers[Constants.TraceIdHeaderName] = traceId;
            httpContext.Request.Headers[Constants.UserIdHeaderName] = userId;

            var headerToContextMiddleware = new HeaderToContextMiddleware(mockRequestDelegate.Object);
            await headerToContextMiddleware.InvokeAsync(httpContext);

            Assert.AreEqual( httpContext.Items[Constants.TraceIdHeaderName]?.ToString(), traceId);
            Assert.AreEqual(  httpContext.Items[Constants.UserIdHeaderName]?.ToString(), userId);
            Assert.AreEqual(            httpContext.Response.Headers[Constants.TraceIdHeaderName].ToString(), traceId);
            mockRequestDelegate.Verify(rd => rd.Invoke(httpContext), Times.Once);
        }
    }
}