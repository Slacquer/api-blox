using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace SlnTests.APIBlox.AspNetCore.OpenApi
{
    public class BuilderTests
    {

        [Fact]
        public void DocumentShouldNotBeEmpty()
        {
            var col = new ServiceCollection();

            col.AddOpenApiBits();
        }
    }
}
