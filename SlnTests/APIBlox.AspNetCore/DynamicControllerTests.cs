using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using APIBlox.AspNetCore;
using APIBlox.AspNetCore.Extensions;
using APIBlox.NetCore.Extensions;
using Microsoft.AspNetCore.Mvc;
using SlnTests.APIBlox.AspNetCore.SlnTests.APIBlox.AspNetCore.RequestObjects;
using SlnTests.APIBlox.AspNetCore.SlnTests.APIBlox.AspNetCore.ResponseObjects;
using Xunit;

#pragma warning disable 1591

namespace SlnTests.APIBlox.AspNetCore
{
    public class DynamicControllerTests
    {
        [Fact]
        internal void Foo()
        {
            var factory = new DynamicControllerFactory("fooAss", true);

            var foo = factory.ComposeGetAllController<TestControllerParameters, IEnumerable<TestResponseObject>>("MySUperCooolController");

            var (controllerTypes, errors) = factory.Compile(foo);

            Assert.NotNull(controllerTypes);
            Assert.Null(errors);

            var actions = controllerTypes.First().GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(m => m.Name.EqualsEx("GetAllTestResponseObject")).ToList();

            Assert.NotEmpty(actions);

            var parameters = actions.First().GetParameters();

            Assert.NotEmpty(parameters);

            var p1 = parameters.First().GetCustomAttributes(typeof(FromRouteAttribute), false);

            Assert.NotNull(p1);
        }
    }

    public class TestControllerParameters
    {
        [FromRoute(Name = "id")]
        public int Id { get; set; }

        [FromBody]
        public TestRequestObj Body { get; set; }
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


