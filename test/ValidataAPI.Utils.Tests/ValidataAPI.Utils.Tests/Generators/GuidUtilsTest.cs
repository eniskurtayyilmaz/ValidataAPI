using System;
using NUnit.Framework;
using ValidataAPI.Utils.Generators;

namespace ValidataAPI.Utils.Tests.Generators
{
    public class GuidUtilsTest
    {
        [Test]
        public void It_Should_Return_Set_Guid_When_Clock_Is_Frozen()
        {
            var guidToSet = Guid.Parse("a9a1f713-291e-454e-b455-ce36eb390259");
            GuidUtils.Freeze(guidToSet);
            var expected = GuidUtils.New();
            Assert.AreEqual(expected, guidToSet);
        }


        [Test]
        public void It_Should_Return_Different_Guid_Instance_When_Clock_Is_Frozen()
        {
            var guid1 = GuidUtils.New();
            var guid2 = GuidUtils.New();
            Assert.AreNotEqual(guid1, guid2);
        }
    }
}