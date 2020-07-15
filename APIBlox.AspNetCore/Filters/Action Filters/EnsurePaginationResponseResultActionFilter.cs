using System;
using System.Collections.Generic;
using System.Linq;
using APIBlox.AspNetCore.Contracts;
using APIBlox.AspNetCore.Types;
using APIBlox.NetCore.Extensions;
using APIBlox.NetCore.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace APIBlox.AspNetCore.Filters
{
    internal class EnsurePaginationResponseResultActionFilter : EnsureResponseResultActionFilter
    {
        private readonly IPaginationMetadataBuilder _paginationBuilder;
        private readonly List<string> _onlyForThesePaths;
        private readonly bool _globalPaths;

        public EnsurePaginationResponseResultActionFilter(
            ILoggerFactory loggerFactory,
            IPaginationMetadataBuilder paginationBuilder,
            bool getsOnly,
            IEnumerable<string> onlyForThesePaths,
            Func<object, object> ensureResponseCompliesWithAction
        )
            : base(loggerFactory, getsOnly, ensureResponseCompliesWithAction)
        {

            _onlyForThesePaths = onlyForThesePaths is null ? new List<string>() : onlyForThesePaths.ToList();
            _globalPaths = !_onlyForThesePaths.Any();

            _paginationBuilder = paginationBuilder;
        }

        protected override void Handle(ActionExecutingContext context, ObjectResult result)
        {
            if (!ResultValueIsEnumerable || !ResultValueCount.HasValue)
                return;

            if (!_globalPaths && !_onlyForThesePaths.Any(p => p.EqualsEx(context.HttpContext.Request.Path)))
                return;

            var value = result.Value;
            var type = value.GetType();
            var prop = type.GetProperties().First();
            var dynamicResult = new DynamicDataObject();
            var propValue = prop.GetValue(value);

            dynamicResult.AddProperty(prop.Name, propValue).AddProperty(
                "Pagination",
                _paginationBuilder.Build(ResultValueCount.Value, context)
            );

            result.Value = dynamicResult;
        }
    }
}
