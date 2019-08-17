using System;
using System.Linq;
using System.Threading.Tasks;
using APIBlox.AspNetCore.Attributes;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace APIBlox.AspNetCore.ModelBinders
{
    internal class FromQueryAlternateNamesBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
                throw new ArgumentNullException(nameof(bindingContext));

            var modelName = bindingContext.ModelName;

            var metadata = (DefaultModelMetadata) bindingContext.ModelMetadata;

            var alternate = FindAlternateValue(bindingContext, metadata);

            if (alternate == ValueProviderResult.None)
                return Task.CompletedTask;

            try
            {
                var converted = Convert.ChangeType(alternate.FirstValue, metadata.UnderlyingOrModelType);

                bindingContext.ModelState.SetModelValue(modelName, alternate);

                bindingContext.Result = ModelBindingResult.Success(converted);
            }
            catch (Exception e) when (e is InvalidCastException || e is FormatException)
            {
                bindingContext.ModelState.TryAddModelError(
                    modelName,
                    $"Incorrect dataType value, needs to be type {metadata.UnderlyingOrModelType}."
                );
            }

            return Task.CompletedTask;
        }

        private static ValueProviderResult FindAlternateValue(ModelBindingContext bc, DefaultModelMetadata metadata)
        {
            var attrs = metadata.Attributes.PropertyAttributes
                .Where(a => a.GetType() == typeof(FromQueryWithAlternateNamesAttribute))
                .Cast<FromQueryWithAlternateNamesAttribute>()
                .FirstOrDefault();

            if (attrs is null)
                return ValueProviderResult.None;

            foreach (var an in attrs.AlternateNames)
            {
                var vr = bc.ValueProvider.GetValue(an);

                if (vr != ValueProviderResult.None)
                    return vr;
            }

            return ValueProviderResult.None;
        }
    }
}
