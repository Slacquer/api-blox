using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using APIBlox.NetCore.Extensions;
using APIBlox.NetCore.Types;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace SlnTests.APIBlox.NetCore
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    public class ServiceCollectionExtensionsNetCoreOtherTests
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly List<string> _paths = new List<string>();

        public ServiceCollectionExtensionsNetCoreOtherTests()
        {
            _loggerFactory = new LoggerFactory();
            
            var path = Path.GetTempPath();
            var root = Path.Combine(path, @"a\b\c\d\e\");

            _paths.Add(root);
            _paths.Add(Path.Combine(path, root, @"f\g\findMe"));
            _paths.Add(Path.Combine(path, root, @"h\i\findMe"));
            _paths.Add(Path.Combine(path, root, @"h\bin\findMe"));

            for (var i = 0; i < 10; i++)
                _paths.Add(Path.Combine(root, i.ToString()));

            _paths.Add(Path.Combine(_paths.Last(), "findMeToo"));

            foreach (var p in _paths)
            {
                if (!Directory.Exists(p))
                    Directory.CreateDirectory(p);
            }

            for (var i = 0; i < 10; i++)
                _paths.Add(Path.Combine(i % 2 == 0 ? "!" + root : root, i.ToString()));
        }

        [Fact]
        public void LoggerOutputShouldComplainAboutAsterisksForInvalidPath()
        {
            var sc = new ServiceCollection();

            var names = new[] { "SlnTests" };
            var paths = new List<string>(PathParser.FindAllSubDirectories(@"..\..\..\..\**\obj\").Select(di => $"!{di.FullName}"))
            {
                @"..\..\..\..\",
                @"C\Program Files\**"
            }.ToArray();


            _loggerFactory.AddProvider(new AssertLoggerProvider(msg =>
                    {
                        if (!msg.Contains("**"))
                            Assert.False(msg.IsEmptyNullOrWhiteSpace());
                    }
                )
            );

            sc.AddInjectableServices(_loggerFactory, names, paths);
        }

        //[Fact]
        //public async Task ShouldThrowArgumentExceptionForAssemblyPathsSinceNoAssembliesFound()
        //{
        //    var sc = new ServiceCollection();

        //    var names = new[] { "SlnTests" };
        //    var paths = new List<string>(_paths)
        //    {
        //        @"..\..\**"
        //    }.ToArray();

        //    _loggerFactory.AddProvider(new AssertLoggerProvider(msg =>
        //            {
        //                if (!msg.Contains("**"))
        //                    Assert.False(msg.IsEmptyNullOrWhiteSpace());
        //            }
        //        )
        //    );

        //    var ex = await Assert.ThrowsAsync<ArgumentException>(() => sc.AddInjectableServices(_loggerFactory, names, paths));

        //    Assert.Contains("NO assemblies", ex.Message);
        //}
    }

    public class AssertLoggerProvider : ILoggerProvider
    {
        private readonly Action<string> _action;

        public AssertLoggerProvider(Action<string> action)
        {
            _action = action;
        }

        public void Dispose()
        {
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new AssertLogger(_action);
        }
    }

    public class AssertLogger : ILogger
    {
        private readonly Action<string> _action;

        public AssertLogger(Action<string> action)
        {
            _action = action;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            _action?.Invoke(state.ToString());
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return new AssertLoggerHandler();
        }
    }

    public class AssertLoggerHandler : IDisposable
    {
        public void Dispose()
        {
        }
    }
}
