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
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class PaginationTests
    {
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

        [Fact]
        public void IncomingAliasTests()
        {
            var ctx = GetActionExecutingContext();
            ctx.HttpContext.Request.QueryString = new QueryString("?take=2&offSet=5&rc=7&$SORTBY=foo.bar");

            var builder = new PaginationMetadataBuilder(10);
            var ret = builder.Build(10, ctx);

            Assert.NotNull(ret.Previous);
            Assert.NotNull(ret.Next);
            Assert.Contains("skip=7", ret.Next);
            Assert.Contains("top=2", ret.Next);
            Assert.Contains("runningCount=17", ret.Next);
            Assert.Contains("orderBy=foo.bar", ret.Next);
        }
        
        [Fact]
        public void NoResultsPaginationShouldBeEmpty()
        {
            var ctx = GetActionExecutingContext();
            ctx.HttpContext.Request.QueryString = new QueryString("?$top=2&$skip=5&rc=7");

            var builder = new PaginationMetadataBuilder(10);
            var ret = builder.Build(0, ctx);

            Assert.Null(ret.Previous);
            Assert.Null(ret.Next);
            Assert.True(ret.ResultCount == 0);
        }

        [Fact]
        public void IncomingTopIsLessThanSkipAndRcEqualsSkipSoNextValuesShouldBeCorrect()
        {
            var ctx = GetActionExecutingContext();
            ctx.HttpContext.Request.QueryString = new QueryString("?top=2&skip=5&rc=5");

            var builder = new PaginationMetadataBuilder(10);
            var ret = builder.Build(10, ctx);

            Assert.NotNull(ret.Previous);
            Assert.NotNull(ret.Next);
            Assert.Contains("skip=7", ret.Next);
            Assert.Contains("top=2", ret.Next);
            Assert.Contains("runningCount=15", ret.Next);
        }

        [Fact]
        public void ResultsLessThanMaxSoNextShouldBeNull()
        {
            var ctx = GetActionExecutingContext();
            ctx.HttpContext.Request.QueryString = new QueryString("");

            var builder = new PaginationMetadataBuilder(10);
            var ret = builder.Build(9, ctx);

            Assert.Null(ret.Previous);
            Assert.Null(ret.Next);
        }

        [Fact]
        public void IncomingTopIsMoreThanSkipAndRcEqualsSkipSoNextValuesShouldBeCorrect()
        {
            var ctx = GetActionExecutingContext();
            ctx.HttpContext.Request.QueryString = new QueryString("?top=5&skip=3&rc=3");

            var builder = new PaginationMetadataBuilder(10);
            var ret = builder.Build(10, ctx);

            Assert.NotNull(ret.Previous);
            Assert.NotNull(ret.Next);
            Assert.Contains("skip=8", ret.Next);
            Assert.Contains("top=5", ret.Next);
            Assert.Contains("runningCount=13", ret.Next);
        }

        [Fact]
        public void IncomingTopButNoSkipAndNoRcSoFirstCallShouldHaveNextButNoPrevious()
        {
            var ctx = GetActionExecutingContext();
            ctx.HttpContext.Request.QueryString = new QueryString("?top=10");

            var builder = new PaginationMetadataBuilder(100);
            var ret = builder.Build(100, ctx);

            Assert.Null(ret.Previous);
            Assert.NotNull(ret.Next);
            Assert.Contains("skip=10", ret.Next);
            Assert.Contains("top=10", ret.Next);
            Assert.Contains("runningCount=100", ret.Next);
        }

        [Fact]
        public void IncomingTopSkipAndRcSoShouldHaveNextAndPreviousTopShouldNeverBeMoreThanMax()
        {
            var ctx = GetActionExecutingContext();
            ctx.HttpContext.Request.QueryString = new QueryString("?top=10&skip=10&runningCount=10");

            var builder = new PaginationMetadataBuilder(100);
            var ret = builder.Build(100, ctx);

            Assert.NotNull(ret.Previous);
            Assert.NotNull(ret.Next);

            Assert.Contains("skip=20", ret.Next);
            Assert.Contains("top=10", ret.Next);
            Assert.Contains("runningCount=110", ret.Next);

            // RC should be gone
            Assert.Contains("top=10", ret.Previous);
            Assert.DoesNotContain("skip=", ret.Previous);
            Assert.DoesNotContain("runningCount=", ret.Previous);
        }

        [Fact]
        public void NoInputsShouldHaveNextWithTopBeingTheMaxAndSkipBeingCountAndRcBeing50NoPreviousRegardlessOfMaxCount()
        {
            var ctx = GetActionExecutingContext();
            var builder = new PaginationMetadataBuilder(100);
            var ret = builder.Build(100, ctx);

            Assert.NotNull(ret);
            Assert.NotNull(ret.Next);

            Assert.Contains("top=100", ret.Next);
            Assert.Contains("skip=100", ret.Next);
            Assert.Contains("runningCount=100", ret.Next);

            Assert.Null(ret.Previous);

            ctx = GetActionExecutingContext();
            builder = new PaginationMetadataBuilder(10);
            ret = builder.Build(10, ctx);

            Assert.NotNull(ret);
            Assert.NotNull(ret.Next);

            Assert.Contains("top=10", ret.Next);
            Assert.Contains("skip=10", ret.Next);
            Assert.Contains("runningCount=10", ret.Next);

            Assert.Null(ret.Previous);
        }
        
        [Fact]
        public void ShouldContainExtraStuffSentInQuery()
        {
            var ctx = GetActionExecutingContext();
            ctx.HttpContext.Request.QueryString = new QueryString("?top=10&myExtraData=999");

            var builder = new PaginationMetadataBuilder(100);
            var ret = builder.Build(100, ctx);

            Assert.NotNull(ret);
            Assert.NotNull(ret.Next);

            Assert.Contains("top=10", ret.Next);
            Assert.Contains("skip=100", ret.Next);
            Assert.Contains("runningCount=100", ret.Next);

            Assert.Contains("myExtraData=999", ret.Next);

            Assert.Null(ret.Previous);
        }

        [Fact]
        public void ShouldThrowSinceResultSetIsLargerThanMaxDefinedPageSize()
        {
            var ctx = GetActionExecutingContext();
            ctx.HttpContext.Request.QueryString = new QueryString("?top=10&skip=10&runningCount=10");
            var builder = new PaginationMetadataBuilder(5);

            var ex = Assert.Throws<IndexOutOfRangeException>(() => builder.Build(10, ctx));

            Assert.Equal("The result set of 10 is larger than what has been defined as the Max page size of 5.", ex.Message);
        }

        [Fact]
        public void ShouldTreatZerosAsNoInputsSoShouldHaveNextNoPrevious()
        {
            var ctx = GetActionExecutingContext();
            ctx.HttpContext.Request.QueryString = new QueryString("?top=0&skip=0&runningCount=0");

            var builder = new PaginationMetadataBuilder(100);
            var ret = builder.Build(100, ctx);

            Assert.NotNull(ret);
            Assert.NotNull(ret.Next);

            Assert.Contains("top=100", ret.Next);
            Assert.Contains("skip=100", ret.Next);
            Assert.Contains("runningCount=100", ret.Next);
            Assert.Null(ret.Previous);

            ctx = GetActionExecutingContext();
            ctx.HttpContext.Request.QueryString = new QueryString("?top=100&skip=0&runningCount=0");

            builder = new PaginationMetadataBuilder(100);
            ret = builder.Build(100, ctx);

            Assert.NotNull(ret);
            Assert.NotNull(ret.Next);

            Assert.Contains("top=100", ret.Next);
            Assert.Contains("skip=100", ret.Next);
            Assert.Contains("runningCount=100", ret.Next);
            Assert.Null(ret.Previous);

            ctx = GetActionExecutingContext();
            ctx.HttpContext.Request.QueryString = new QueryString("?$skip=0");

            builder = new PaginationMetadataBuilder(100);
            ret = builder.Build(100, ctx);

            Assert.NotNull(ret);
            Assert.NotNull(ret.Next);

            Assert.Contains("top=100", ret.Next);
            Assert.Contains("skip=100", ret.Next);
            Assert.Contains("runningCount=100", ret.Next);

            Assert.Null(ret.Previous);
        }

        [Fact]
        public void WhenTopOrSkipAreDisregardedOrMissingRcShouldBeDisregardedAsWell()
        {
            var ctx = GetActionExecutingContext();
            ctx.HttpContext.Request.QueryString = new QueryString("?skip=0&runningCount=10");

            var builder = new PaginationMetadataBuilder(100);
            var ret = builder.Build(100, ctx);

            Assert.NotNull(ret);
            Assert.NotNull(ret.Next);

            Assert.Contains("top=100", ret.Next);
            Assert.Contains("skip=100", ret.Next);
            Assert.Contains("runningCount=100", ret.Next);
            Assert.Null(ret.Previous);
        }
    }
}
