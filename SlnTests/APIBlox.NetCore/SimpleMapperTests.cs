using APIBlox.NetCore.Types;
using APIBlox.NetCore.Types.JsonBits;
using Newtonsoft.Json;
using Xunit;

namespace SlnTests.APIBlox.NetCore
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    public class SimpleMapperTests
    {

        [Fact]
        public void Success()
        {
            var foo = new Foo { Name = "          " };

            var bar = foo.MapTo<Bar>(settings: new JsonSerializerSettings { Converters = new JsonConverter[] { new EmptyStringToNullConverter() }, ContractResolver = new PopulateNonPublicSettersContractResolver() });

            Assert.Null(bar.Name);

        }

        public class Foo
        {
            public string Name { get; set; }
        }

        public class Bar
        {
            public string Name { get; private set; }
        }
    }
}
