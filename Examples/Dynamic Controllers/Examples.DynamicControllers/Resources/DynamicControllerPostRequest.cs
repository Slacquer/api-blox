using Microsoft.AspNetCore.Mvc;

namespace Examples.Resources
{
    public class DynamicControllerPostRequest
    {
        public string SomethingFromBody { get; set; }

        [FromRoute(Name = "someId")] 
        public int SomeId { get; private set; }
    }
}
