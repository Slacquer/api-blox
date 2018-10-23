using System.Linq;
using APIBlox.AspNetCore.Contracts;
using APIBlox.NetCore.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace APIBlox.AspNetCore.Filters
{
    internal class EnsurePaginationResponseResultActionFilter : EnsureResponseResultActionFilter
    {
        private readonly IPaginationMetadataBuilder _paginationBuilder;

        public EnsurePaginationResponseResultActionFilter(
            ILoggerFactory loggerFactory,
            IPaginationMetadataBuilder paginationBuilder
        )
            : base(loggerFactory)
        {
            _paginationBuilder = paginationBuilder;
        }

        protected override void Handle(ActionExecutingContext context, ObjectResult result)
        {
            if (!ResultValueIsEnumerable || !ResultValueCount.HasValue)
                return;

            var value = result.Value;
            var type = value.GetType();
            var prop = type.GetProperties().First();
            var dynamicResult = new DynamicDataObject();
            var propValue = prop.GetValue(value);

            dynamicResult.AddProperty(prop.Name, propValue)
                .AddProperty("Pagination", _paginationBuilder.Build(ResultValueCount.Value, context));

            result.Value = dynamicResult;
        }
    }
}
