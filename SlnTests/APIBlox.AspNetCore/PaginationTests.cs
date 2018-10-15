using System;
using System.Collections.Generic;
using APIBlox.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Moq;
using Xunit;

namespace SlnTests.APIBlox.AspNetCore
{
    public class PaginationTests
    {
        public PaginationTests()
        {
            var c = new DefaultHttpContext();

            c.Request.Scheme = "http";
            c.Request.Host = new HostString("dummy.com");
            c.Request.PathBase = "/";
            c.Request.Path = "/tests";

            _actionContext = new ActionContext(
                c,
                new Mock<RouteData>().Object,
                new Mock<ActionDescriptor>().Object
            );
        }

        private readonly ActionContext _actionContext;

        private ActionExecutingContext GetActionExecutingContext()
        {
            return new ActionExecutingContext(
                _actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                new Mock<Controller>().Object
            );
        }

        [Fact]
        public void IncomingTopButNoSkipAndNoRcSoFirstCallShouldHaveNextButNoPrevious()
        {
            var ctx = GetActionExecutingContext();
            ctx.HttpContext.Request.QueryString = new QueryString("?top=10");

            var builder = new PaginationMetadataBuilder(100);
            var ret = builder.Build(10, ctx);

            Assert.Null(ret.Previous);
            Assert.NotNull(ret.Next);
            Assert.Contains("skip=10", ret.Next);
            Assert.Contains("top=10", ret.Next);
            Assert.Contains("rc=10", ret.Next);
        }

        [Fact]
        public void IncomingTopSkipAndRcSoShouldHaveNextAndPreviousTopShouldNeverBeMoreThanMax()
        {
            var ctx = GetActionExecutingContext();
            ctx.HttpContext.Request.QueryString = new QueryString("?top=10&skip=10&rc=10");

            var builder = new PaginationMetadataBuilder(100);
            var ret = builder.Build(10, ctx);

            Assert.NotNull(ret.Previous);
            Assert.NotNull(ret.Next);

            Assert.Contains("skip=20", ret.Next);
            Assert.Contains("top=10", ret.Next);
            Assert.Contains("rc=20", ret.Next);

            // RC should be gone
            Assert.Contains("top=10", ret.Previous);
            Assert.DoesNotContain("skip=", ret.Previous);
            Assert.DoesNotContain("rc=", ret.Previous);
        }

        [Fact]
        public void NoInputsShouldHaveNextWithTopBeingTheMaxAndSkipBeingCountAndRcBeing50NoPreviousRegardlessOfMaxCount()
        {
            var ctx = GetActionExecutingContext();
            var builder = new PaginationMetadataBuilder(100);
            var ret = builder.Build(50, ctx);

            Assert.NotNull(ret);
            Assert.NotNull(ret.Next);

            Assert.Contains("top=100", ret.Next);
            Assert.Contains("skip=50", ret.Next);
            Assert.Contains("rc=50", ret.Next);

            Assert.Null(ret.Previous);

            ctx = GetActionExecutingContext();
            builder = new PaginationMetadataBuilder(10);
            ret = builder.Build(10, ctx);

            Assert.NotNull(ret);
            Assert.NotNull(ret.Next);

            Assert.Contains("top=10", ret.Next);
            Assert.Contains("skip=10", ret.Next);
            Assert.Contains("rc=10", ret.Next);

            Assert.Null(ret.Previous);
        }

        [Fact]
        public void NoInputsShouldHaveNextWithTopBeingTheMaxSkipBeingCountNoPrevious()
        {
            var ctx = GetActionExecutingContext();
            var builder = new PaginationMetadataBuilder(100);
            var ret = builder.Build(50, ctx);

            Assert.NotNull(ret);
            Assert.NotNull(ret.Next);

            Assert.Contains("top=100", ret.Next);
            Assert.Contains("skip=50", ret.Next);
            Assert.Contains("rc=50", ret.Next);

            Assert.Null(ret.Previous);
        }

        [Fact]
        public void ShouldContainExtraStuffSentInQuery()
        {
            var ctx = GetActionExecutingContext();
            ctx.HttpContext.Request.QueryString = new QueryString("?top=10&myExtraData=999");

            var builder = new PaginationMetadataBuilder(100);
            var ret = builder.Build(50, ctx);

            Assert.NotNull(ret);
            Assert.NotNull(ret.Next);

            Assert.Contains("top=10", ret.Next);
            Assert.Contains("skip=50", ret.Next);
            Assert.Contains("rc=50", ret.Next);

            Assert.Contains("myExtraData=999", ret.Next);

            Assert.Null(ret.Previous);
        }

        [Fact]
        public void ShouldThrowSinceResultSetIsLargerThanMaxDefinedPageSize()
        {
            var ctx = GetActionExecutingContext();
            ctx.HttpContext.Request.QueryString = new QueryString("?top=10&skip=10&rc=10");
            var builder = new PaginationMetadataBuilder(5);

            var ex = Assert.Throws<IndexOutOfRangeException>(() => builder.Build(10, ctx));

            Assert.Contains("The result set is larger", ex.Message);
        }

        [Fact]
        public void ShouldTreatZerosAsNoInputsSoShouldHaveNextNoPrevious()
        {
            var ctx = GetActionExecutingContext();
            ctx.HttpContext.Request.QueryString = new QueryString("?top=0&skip=0&rc=0");

            var builder = new PaginationMetadataBuilder(100);
            var ret = builder.Build(50, ctx);

            Assert.NotNull(ret);
            Assert.NotNull(ret.Next);

            Assert.Contains("top=100", ret.Next);
            Assert.Contains("skip=50", ret.Next);
            Assert.Contains("rc=50", ret.Next);
            Assert.Null(ret.Previous);



            ctx = GetActionExecutingContext();
            ctx.HttpContext.Request.QueryString = new QueryString("?top=100&skip=0&rc=0");

            builder = new PaginationMetadataBuilder(100);
            ret = builder.Build(50, ctx);

            Assert.NotNull(ret);
            Assert.NotNull(ret.Next);

            Assert.Contains("top=100", ret.Next);
            Assert.Contains("skip=50", ret.Next);
            Assert.Contains("rc=50", ret.Next);
            Assert.Null(ret.Previous);
            


            ctx = GetActionExecutingContext();
            ctx.HttpContext.Request.QueryString = new QueryString("?skip=0");

            builder = new PaginationMetadataBuilder(100);
            ret = builder.Build(50, ctx);

            Assert.NotNull(ret);
            Assert.NotNull(ret.Next);

            Assert.Contains("top=100", ret.Next);
            Assert.Contains("skip=50", ret.Next);
            Assert.Contains("rc=50", ret.Next);

            Assert.Null(ret.Previous);
        }

        [Fact]
        public void WhenTopOrSkipAreDisregardedOrMissingShouldRcShouldBeDisregardedAsWell()
        {
            var ctx = GetActionExecutingContext();
            ctx.HttpContext.Request.QueryString = new QueryString("?skip=0&rc=10");

            var builder = new PaginationMetadataBuilder(100);
            var ret = builder.Build(50, ctx);

            Assert.NotNull(ret);
            Assert.NotNull(ret.Next);

            Assert.Contains("top=100", ret.Next);
            Assert.Contains("skip=50", ret.Next);
            Assert.Contains("rc=50", ret.Next);
            Assert.Null(ret.Previous);
        }
    }
}
