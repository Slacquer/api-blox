#region -    Using Statements    -

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using APIBlox.NetCore.Extensions;
using APIBlox.NetCore.Types;
using Xunit;

#endregion

namespace SlnTests.APIBlox.NetCore
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class PathParserTests : IDisposable
    {
        private readonly List<string> _paths = new List<string>();

        #region Setup/Teardown

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

        public void Dispose()
        {
            Directory.Delete(_paths[0], true);
        }

        #endregion

        [Fact]
        public void ShouldHavePathsFromSingleDoubleAsterisks()
        {
            var parser = PathParser.FindAllSubDirectories(_paths[1].Replace(@"\g", @"\**")).ToList();

            Assert.NotNull(parser);
            Assert.True(parser.Count > 0);

            var hasRelease = parser.Any(d => d.FullName.EndsWithEx("findme"));

            Assert.True(hasRelease);
        }

        [Fact]
        public void ShouldHavePathsFromSingleDoubleAsterisksOnlyBin()
        {
            var parser = PathParser.FindAllSubDirectories(_paths[3].Replace(@"\a", @"\**")).ToList();

            Assert.NotNull(parser);
            Assert.True(parser.Count > 0);

            var hasRelease = parser.Any(d => d.FullName.EndsWithEx("findme"));

            Assert.True(hasRelease);
        }

        [Fact]
        public void ShouldHaveSinglePathAsNoAsterisksWereProvided()
        {
            // root folder will never be counted unless we say so, so we use -2
            var parser = PathParser.FindAllSubDirectories(_paths[_paths.Count - 2]).ToList();

            Assert.NotNull(parser);
            Assert.True(parser.Count == 1);
        }
    }
}
