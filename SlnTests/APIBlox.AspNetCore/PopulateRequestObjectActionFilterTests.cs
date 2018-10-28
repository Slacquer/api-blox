//using System;
//using System.Collections.Generic;
//using System.Net.Http;
//using System.Text;
//using System.Threading.Tasks;
//using APIBlox.AspNetCore;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Abstractions;
//using Microsoft.AspNetCore.Mvc.Controllers;
//using Microsoft.AspNetCore.Mvc.Filters;
//using Microsoft.AspNetCore.Routing;
//using Microsoft.Extensions.Logging;
//using Moq;
//using Xunit;

//namespace SlnTests.APIBlox.AspNetCore
//{
//    public class PopulateRequestObjectActionFilterTests
//    {
//        private readonly ActionContext _actionContext;

//        private ActionExecutingContext GetActionExecutingContext()
//        {
//            return new ActionExecutingContext(
//                _actionContext,
//                new List<IFilterMetadata>(),
//                new Dictionary<string, object>(),
//                new Mock<Controller>().Object
//            );
//        }

//        public PopulateRequestObjectActionFilterTests()
//        {
//            var c = new DefaultHttpContext();

//            c.Request.Scheme = "http";
//            c.Request.Host = new HostString("dummy.com");
//            c.Request.PathBase = "/";
//            c.Request.Path = "/tests";

//            var des = new ActionDescriptor();
//            var p = new ControllerParameterDescriptor
//            {
//                ParameterType = typeof(int), 
//                Name = "fooInt",
//                ParameterInfo = new Mock<System.Reflection.ParameterInfo>().Object
//            };
//            des.Parameters = new List<ParameterDescriptor>{p};

//            _actionContext = new ActionContext(
//                c,
//                new Mock<RouteData>().Object,
//               des
//            );
//        }



//        [Fact]
//        public async Task ShouldWork()
//        {
//            var pop = new PopulateRequestObjectActionFilter(new Mock<LoggerFactory>().Object);

//            var del = new ActionExecutionDelegate(() => { return null; });
//            var ctx = GetActionExecutingContext();

//            await pop.OnActionExecutionAsync(ctx, del);


//        }
//    }
//}
