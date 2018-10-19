using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using APIBlox.NetCore.Extensions;
using APIBlox.NetCore.Types;
using Xunit;

namespace SlnTests.APIBlox.NetCore
{
    public class PathParserTests : IDisposable
    {
        public PathParserTests()
        {
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
        }

        private readonly List<string> _paths = new List<string>();

        [Fact]
        public void ShouldHavePathsFromSingleDoubleAsterisks()
        {
            var parser = PathParser.FindAll(_paths[1].Replace(@"\g", @"\**")).ToList();

            Assert.NotNull(parser);
            Assert.True(parser.Count > 0);

            var hasRelease = parser.Any(d => d.FullName.EndsWithEx("findme"));

            Assert.True(hasRelease);
        }

        [Fact]
        public void ShouldHavePathsFromSingleDoubleAsterisksOnlyBin()
        {
            var parser = PathParser.FindAll(_paths[3].Replace(@"\a", @"\**")).ToList();

            Assert.NotNull(parser);
            Assert.True(parser.Count > 0);

            var hasRelease = parser.Any(d => d.FullName.EndsWithEx("findme"));

            Assert.True(hasRelease);
        }

        [Fact]
        public void ShouldHaveSinglePathAsNoAsterisksWereProvided()
        {
            var parser = PathParser.FindAll(_paths[2]).ToList();

            Assert.NotNull(parser);
            Assert.True(parser.Count == 1);
        }

        public void Dispose()
        {
            Directory.Delete(_paths[0], true);
        }
    }
}
