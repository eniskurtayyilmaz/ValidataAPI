using Moq;
using NUnit.Framework;
using ValidataAPI.Utils.Common;
using ValidataAPI.Utils.Services;

namespace ValidataAPI.Utils.Tests.Services
{
    public class TraceIdResolverServiceTest
    {
        [Test]
        public void It_Should_Return_Trace_Id_From_Context_When_Request_Contains_Trace_Id()
        {
            const string traceId = "custom trace id";
            var mockIHttpContextService = new Mock<IHttpContextService>();
            mockIHttpContextService
                .Setup(contextAccessor => contextAccessor.GetContextItemValue(Constants.TraceIdHeaderName))
                .Returns(traceId);
            var traceIdResolverService = new TraceIdResolverService(mockIHttpContextService.Object);
            var expected = traceIdResolverService.GetTraceId();
            
            Assert.AreEqual(expected, traceId);
        }
    }
}