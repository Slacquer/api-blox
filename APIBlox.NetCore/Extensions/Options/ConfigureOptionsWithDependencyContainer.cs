﻿// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.Options
{
    /// <inheritdoc />
    /// <summary>
    ///     Class ConfigureOptionsWithDependencyContainer.
    /// </summary>
    /// <typeparam name="TOptions">The type of the t options.</typeparam>
    /// <typeparam name="TDependent">The type of the t dependent.</typeparam>
    /// <seealso cref="T:Microsoft.Extensions.Options.IConfigureOptions`1" />
    /// <autogeneratedoc />
    public class ConfigureOptionsWithDependencyContainer<TOptions, TDependent> : IConfigureOptions<TOptions>
        where TOptions : class //, new()
    {
        /// <summary>
        ///     The configure options with dependency
        /// </summary>
        /// <autogeneratedoc />
        private readonly ConfigureOptionsWithDependency<TOptions, TDependent> _configureOptionsWithDependency;

        /// <summary>
        ///     The dependent
        /// </summary>
        /// <autogeneratedoc />
        private readonly TDependent _dependent;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConfigureOptionsWithDependencyContainer{TOptions, TDependent}" />
        ///     class.
        /// </summary>
        /// <param name="dependent">The dependent.</param>
        /// <param name="configureOptionsWithDependency">The configure options with dependency.</param>
        /// <autogeneratedoc />
        public ConfigureOptionsWithDependencyContainer(
            TDependent dependent,
            IOptions<ConfigureOptionsWithDependency<TOptions, TDependent>> configureOptionsWithDependency
        )
        {
            _configureOptionsWithDependency = configureOptionsWithDependency.Value;
            _dependent = dependent;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Invoked to configure a TOptions instance.
        /// </summary>
        /// <param name="options">The options instance to configure.</param>
        /// <autogeneratedoc />
        public void Configure(TOptions options)
        {
            _configureOptionsWithDependency.Action(options, _dependent);
        }
    }
}
