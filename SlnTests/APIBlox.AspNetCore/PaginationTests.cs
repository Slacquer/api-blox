using System.Collections.Generic;
using APIBlox.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Moq;
using Xunit;

namespace SlnTests.APIBlox.AspNetCore
{
    public class PaginationTests
    {
        private readonly ActionExecutingContext _actionExecutingContext;

        public PaginationTests()
        {
            var c = new DefaultHttpContext();

            c.Request.Scheme = "http";
            c.Request.Host = new HostString("dummy.com");
            c.Request.PathBase = "/";
            c.Request.Path = "/tests";
            
            var actionContext = new ActionContext(
                c,
                new Mock<RouteData>().Object,
                new Mock<ActionDescriptor>().Object
            );

            _actionExecutingContext = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                new Mock<Controller>().Object
            );
        }

        [Fact]
        public void ShouldHaveNextWithTopAndSkip()
        {
            _actionExecutingContext.HttpContext.Request.QueryString = new QueryString("?top=1");
            
            var builder = new PaginationMetadataBuilder();
            var ret = builder.Build(100, _actionExecutingContext);

            Assert.NotNull(ret);
            Assert.True(ret.ResultCount == 100);
            Assert.True(ret.Next.Contains("top") && ret.Next.Contains("skip"));
            Assert.True(ret.Next != null);
        }

        [Fact]
        public void ShouldHaveNextAndPreviousWithTopAndSkip()
        {
            _actionExecutingContext.HttpContext.Request.QueryString = new QueryString("?skip=5&top=1");
            
            var builder = new PaginationMetadataBuilder();
            var ret = builder.Build(100, _actionExecutingContext);

            Assert.NotNull(ret);
            Assert.True(ret.ResultCount == 100);
            Assert.True(ret.Next.Contains("top") && ret.Next.Contains("skip"));
            Assert.True(ret.Next != null);
            Assert.True(ret.Previous != null);
        }
        
        [Fact]
        public void NextShouldBeNullSinceCountIsLessThanMaxSoMustBeOnLastPage()
        {
            _actionExecutingContext.HttpContext.Request.QueryString = new QueryString("?skip=25&top=25");

            var builder = new PaginationMetadataBuilder(50);
            var ret = builder.Build(3, _actionExecutingContext);

            Assert.NotNull(ret);
            Assert.True(ret.ResultCount == 3);
            Assert.Contains("top", ret.Previous);
            Assert.True(ret.Next == null);
            Assert.True(ret.Previous != null);
        }
    }
}
