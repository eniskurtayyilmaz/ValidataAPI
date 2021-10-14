using NUnit.Framework;
using ValidataAPI.Utils.Services;

namespace ValidataAPI.Utils.Tests.Services
{
    public class JsonSerializerServiceTest
    {
        [Test]
        public void It_Should_Apply_Camel_Case_Naming_Policy()
        {
            var jsonSerializerService = new JsonSerializerService();
            var expected = jsonSerializerService.Serialize(new JsonTest() {Id = "Id", Name = "Name"});
            
            
            Assert.AreEqual(expected, "{\"id\":\"Id\",\"name\":\"Name\"}");
        }
    }
    
    

    public class JsonTest
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}