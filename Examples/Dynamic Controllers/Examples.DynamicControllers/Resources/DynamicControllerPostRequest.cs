using Microsoft.AspNetCore.Mvc;

namespace Examples.Resources
{
    internal class DynamicControllerPostRequest
    {
        public string SomethingFromBody { get; set; }

        [FromRoute(Name = "someId")] 
        public int SomeId { get; set; }
    }
}
