using System.Collections;
using System.Collections.Generic;
using System.Linq;
using APIBlox.NetCore;
using APIBlox.AspNetCore.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace APIBlox.AspNetCore.Filters
{
    internal class EnsurePaginationResponseResultActionFilter : EnsureResponseResultActionFilter
    {
        private readonly IPaginationBuilder _paginationBuilder;


        public EnsurePaginationResponseResultActionFilter(
            ILoggerFactory loggerFactory,
            IPaginationBuilder paginationBuilder
        )
            : base(loggerFactory)
        {
            _paginationBuilder = paginationBuilder;
        }

        protected override void Handle(ActionExecutingContext context, ObjectResult result)
        {
            if (!ResultValueIsEnumerable)
                return;

            var value = result.Value;
            var type = value.GetType();
            var prop = type.GetProperties().First();
            var dynamicResult = new DynamicDataObject();
            var propValue = prop.GetValue(value);

            dynamicResult.AddProperty(prop.Name, propValue)
                .AddProperty("Pagination", _paginationBuilder.Build(value as IEnumerable<object>, context));

            result.Value = dynamicResult;
        }
    }
}
