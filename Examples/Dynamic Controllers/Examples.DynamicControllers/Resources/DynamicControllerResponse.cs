using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIBlox.AspNetCore.Contracts;

namespace Examples.Resources
{
    internal class DynamicControllerResponse : IResource<int>
    {
        public int Id { get; set; }

        public string SomeValueThatHadToBeThreeCharacters { get; set; }
    }
}
