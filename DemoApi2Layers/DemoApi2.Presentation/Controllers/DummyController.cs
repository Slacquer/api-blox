//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using APIBlox.AspNetCore;
//using APIBlox.AspNetCore.Attributes;
//using Microsoft.AspNetCore.Mvc;

//namespace DemoApi2.Presentation.Controllers
//{
//    [Route("api/[controller]/{someRouteId:int}/foo/bar")]
//    [ApiController]
//    public class DummyController : ControllerBase
//    {
//        [HttpPost]
//        public IActionResult Post([Populate] Dummy dummy)
//        {
//            return Ok();
//        }
//    }

//    public class Dummy
//    {
//        public string Name { get; set; }

//        public int SomeRouteId { get; private set; }
//    }
//}


