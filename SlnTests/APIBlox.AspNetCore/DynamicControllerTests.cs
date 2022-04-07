using System;
using APIBlox.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Xunit;

#pragma warning disable 1591

namespace SlnTests.APIBlox.AspNetCore
{
    public class DynamicControllerTests
    {
        [Fact]
        public void ShouldBuildInputParams()
        {
            var type = typeof(TestControllerParameters);
            var (item1, strings) = DynamicControllerFactory.WriteInputParamsWithNamespaces(type);

            Assert.NotNull(item1);
            Assert.NotNull(strings);
        }
    }

    /// <summary>
    ///     Class TestControllerParameters.
    /// </summary>
    public class TestControllerParameters
    {
        //[Another(Fubars = new[] { "a" })]
        [FromRoute(Name = "id")]

        // [MyCool(1, 2, 3)]
        public int Id { get; set; }

        //[Required(AllowEmptyStrings = false, ErrorMessage = "this is a better way to go?")]
        //[RegularExpression("some pattern", MatchTimeoutInMilliseconds = 5000)]
        //[FromBody]
        //public TestRequestObj Body { get; set; }

        //public IEnumerable<int?> BunchaNullableInts { get; set; }
    }

    //public class MyCoolAttribute : Attribute
    //{
    //    public MyCoolAttribute(params int[] items)
    //    {
    //        Items = items;
    //    }

    //    public int[] Items { get; }
    //}

    //public class AnotherAttribute : Attribute
    //{
    //    public string[] Fubars { get; set; }
    //}

    namespace SlnTests.APIBlox.AspNetCore.RequestObjects
    {
        public class TestRequestObj
        {
            public string Foo { get; set; }
            public string Bar { get; set; }
        }
    }

    namespace SlnTests.APIBlox.AspNetCore.ResponseObjects
    {
        public class TestResponseObject
        {
            public string Name { get; set; }
        }
    }
}
