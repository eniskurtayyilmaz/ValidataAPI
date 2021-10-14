
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using ValidataAPI.Utils.Services;

namespace ValidataAPI.Utils.Tests.Services
{
    public class HttpContextServiceTest
    {
        [Test]
        public void It_Should_Return_Context_Item_Of_Given_Key()
        {
            const string key = "key";
            const string value = "value";
            var mockIHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var context = new DefaultHttpContext {Items = {[key] = value}};
            mockIHttpContextAccessor.Setup(ca => ca.HttpContext).Returns(context);
            var httpContextService = new HttpContextService(mockIHttpContextAccessor.Object);
            var excepted = httpContextService.GetContextItemValue(key);
            Assert.AreEqual(excepted, value);
        }

        [Test]
        public void It_Should_Return_Empty_Value_Of_Given_Key_When_Key_Is_Not_Present()
        {
            const string key = "key";
            var mockIHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var context = new DefaultHttpContext();
            mockIHttpContextAccessor.Setup(ca => ca.HttpContext).Returns(context);
            var httpContextService = new HttpContextService(mockIHttpContextAccessor.Object);
            var excepted = httpContextService.GetContextItemValue(key);
            Assert.IsNull(excepted);
        }

        [Test]
        public void It_Should_Return_Empty_Value_Of_Given_Key_When_Context_Is_Not_Present()
        {
            const string key = "key";
            var mockIHttpContextAccessor = new Mock<IHttpContextAccessor>();
            mockIHttpContextAccessor.Setup(ca => ca.HttpContext).Returns((HttpContext) null);
            var httpContextService = new HttpContextService(mockIHttpContextAccessor.Object);
            var excepted = httpContextService.GetContextItemValue(key);
            Assert.IsNull(excepted);
        }
    }
}