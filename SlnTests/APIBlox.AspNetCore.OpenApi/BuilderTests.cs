#region -    Using Statements    -

using Microsoft.Extensions.DependencyInjection;
using Xunit;

#endregion

namespace SlnTests.APIBlox.AspNetCore.OpenApi
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class BuilderTests
    {
        [Fact]
        public void DocumentShouldNotBeEmpty()
        {
            var col = new ServiceCollection();

            //col.AddOpenApiBits();
        }
    }
}
