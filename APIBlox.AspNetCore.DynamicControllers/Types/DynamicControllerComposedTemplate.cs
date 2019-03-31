using System;
using System.Collections.Generic;
using System.Text;
using APIBlox.AspNetCore.Contracts;

namespace APIBlox.AspNetCore.Types
{
    public class DynamicControllerComposedTemplate : IComposedTemplate
    {
        public DynamicControllerComposedTemplate(string content, string requiredHandler)
        {
            Content = content;
            RequiredHandler = requiredHandler;
        }
        public string Content { get; }

        public string RequiredHandler { get; }
    }
}
