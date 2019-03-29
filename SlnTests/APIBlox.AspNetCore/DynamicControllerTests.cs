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
        internal void SuccessfullyCompileQueryAllController()
        {
            var factory = new DynamicControllerFactory("fooAss", true);

            var foo = factory.ComposeQueryAllController<TestControllerParameters, IEnumerable<TestResponseObject>>("SuccessfullyCompileQueryAllController");

            var controllerTypes = factory.Compile(foo);

            Assert.NotNull(controllerTypes);
            Assert.Null(factory.CompilationErrors);

            var actions = controllerTypes.First().GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(m => m.Name.EqualsEx("QueryAll")).ToList();

            Assert.NotEmpty(actions);

            var parameters = actions.First().GetParameters();

            Assert.NotEmpty(parameters);

            var p1 = parameters.First().GetCustomAttributes(typeof(FromRouteAttribute), false);

            Assert.NotNull(p1);
        }

        [Fact]
        internal void SuccessfullyCompileQueryByController()
        {
            var factory = new DynamicControllerFactory("MyAss", false);

            var foo = factory.ComposeQueryByController<TestControllerParameters, IEnumerable<TestResponseObject>>("SuccessfullyCompileQueryByController");

            var controllerTypes = factory.Compile(foo);

            Assert.NotNull(controllerTypes);
            Assert.Null(factory.CompilationErrors);

            var actions = controllerTypes.First().GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(m => m.Name.EqualsEx("QueryBy")).ToList();

            Assert.NotEmpty(actions);

            var parameters = actions.First().GetParameters();

            Assert.NotEmpty(parameters);

            var p1 = parameters.First().GetCustomAttributes(typeof(FromRouteAttribute), false);

            Assert.NotNull(p1);
        }

        [Fact]
        internal void SuccessfullyCompileMultipleControllersAndAssemblyExists()
        {
            var factory = new DynamicControllerFactory("OutputFile", false);

            var c1 = factory.ComposeQueryByController<TestControllerParameters, IEnumerable<TestResponseObject>>(
                "SuccessfullyCompileMultipleControllersAndAssemblyExists1"
            );
            var c2 = factory.ComposeQueryAllController<TestControllerParameters, IEnumerable<TestResponseObject>>(
                "SuccessfullyCompileMultipleControllersAndAssemblyExists2"
            );

            var outfile = @".\SuccessfullyCompileMultipleControllersAndAssemblyExists";
            var fi = factory.Compile(outfile, c1, c2);

            Assert.NotNull(fi);

            Assert.True(fi.Exists);
            Assert.True(fi.Length > 0);
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


