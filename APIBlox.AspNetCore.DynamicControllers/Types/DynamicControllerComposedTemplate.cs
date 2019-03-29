using System;
using System.Collections.Generic;
using System.Text;
using APIBlox.AspNetCore.Contracts;

namespace APIBlox.AspNetCore.Types
{
    public class DynamicControllerComposedTemplate : IComposedTemplate
    {
        public DynamicControllerComposedTemplate(string content)
        {
            Content = content;
        }
        public string Content { get; }
    }
}
