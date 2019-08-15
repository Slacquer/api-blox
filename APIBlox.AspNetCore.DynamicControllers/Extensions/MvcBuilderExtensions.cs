using APIBlox.AspNetCore;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    ///     Class MvcBuilderExtensions.
    /// </summary>
    public static class MvcBuilderExtensions
    {
        /// <summary>
        ///     Adds the post location header result filter.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns>IMvcBuilder.</returns>
        public static IMvcBuilder AddPostLocationHeaderResultFilter(
            this IMvcBuilder builder
        )
        {
            return builder.AddFilter<PostLocationHeaderResultFilter>(order: 100);
        }

        /// <summary>
        ///     Adds the post location header result filter.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns>IMvcCoreBuilder.</returns>
        public static IMvcCoreBuilder AddPostLocationHeaderResultFilter(
            this IMvcCoreBuilder builder
        )
        {
            return builder.AddFilter<PostLocationHeaderResultFilter>(order: 100);
        }
    }
}
