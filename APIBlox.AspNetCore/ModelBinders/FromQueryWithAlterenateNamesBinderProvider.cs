using System;
using System.Linq;
using APIBlox.AspNetCore.Attributes;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace APIBlox.AspNetCore.ModelBinders
{
    internal class FromQueryWithAlternateNamesBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            return IsIQuery(context)
                ? new BinderTypeModelBinder(typeof(FromQueryAlternateNamesBinder))
                : null;
        }

        private static bool IsIQuery(ModelBinderProviderContext c)
        {
            var attrs = ((DefaultModelMetadata)c.Metadata).Attributes.PropertyAttributes;

            return !(attrs is null) && attrs.Any(pa => pa.GetType() == typeof(FromQueryWithAlternateNamesAttribute));
        }
    }
}
