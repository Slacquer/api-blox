#region -    Using Statements    -

using Microsoft.Extensions.DependencyInjection;
using Xunit;

#endregion

namespace SlnTests.APIBlox.AspNetCore.OpenApi
{
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
