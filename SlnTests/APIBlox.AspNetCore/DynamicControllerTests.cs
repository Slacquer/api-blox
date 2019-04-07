using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using APIBlox.AspNetCore;
using APIBlox.AspNetCore.Extensions;
using APIBlox.NetCore.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SlnTests.APIBlox.AspNetCore.SlnTests.APIBlox.AspNetCore.RequestObjects;
using SlnTests.APIBlox.AspNetCore.SlnTests.APIBlox.AspNetCore.ResponseObjects;
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
            var ret = DynamicControllerFactory.WriteInputParamsWithNamespaces(type);

            Assert.NotNull(ret);
        }



    }

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

    public class MyCoolAttribute : Attribute
    {
        public int[] Items { get; }

        public MyCoolAttribute(params int[] items)
        {
            Items = items;
        }
    }

    public class AnotherAttribute : Attribute
    {
        public string[] Fubars { get; set; }

        public AnotherAttribute()
        {
        }
    }




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
