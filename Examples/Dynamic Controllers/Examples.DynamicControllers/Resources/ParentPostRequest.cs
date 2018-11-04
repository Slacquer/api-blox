using Microsoft.AspNetCore.Mvc;

namespace Examples.Resources
{
    
    public class ParentPostRequest
    {
        public int Age { get; set; }
        
        public string FirstName { get; set; }
        
        public string LastName { get; set; }

        // This is private so that things like swashbuckle won't try to
        // allow a user to fill it in, as we want it from the route.
        [FromRoute(Name ="someRouteValueWeNeed")]
        public int SomeRouteValueWeNeed { get;  set; }

        [FromQuery(Name ="someRouteValueWeNeed2")]
        public int SomeRouteValueWeNeed2 { get;  set; }
    }
}
