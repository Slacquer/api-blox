using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using APIBlox.NetCore.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace APIBlox.NetCore.Types
{
    /// <inheritdoc />
    /// <summary>
    ///     Class DynamicDataObject.
    ///     <para>About as close as you can get to a javaScript object.</para>
    /// </summary>
    /// <seealso cref="T:System.Dynamic.DynamicObject" />
    [DebuggerStepThrough]
    public class DynamicDataObject : DynamicObject
    {
        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:APIBlox.NetCore.Types.DynamicDataObject" /> class.
        /// </summary>
        public DynamicDataObject()
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            };
            settings.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy(), false));
            Settings = settings;
        }

        /// <summary>
        ///     The properties collection
        /// </summary>
        protected Dictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();

        /// <summary>
        ///     Gets or sets the settings.
        /// </summary>
        /// <value>The settings.</value>
        [JsonIgnore]
        protected JsonSerializerSettings Settings { get; set; }

        /// <summary>
        ///     Adds the property.
        /// </summary>
        /// <typeparam name="TModel">The type of the t model.</typeparam>
        /// <typeparam name="TType">The type of the t type.</typeparam>
        /// <param name="func">The function.</param>
        /// <param name="value">The value.</param>
        public DynamicDataObject AddProperty<TModel, TType>(Expression<Func<TModel, TType>> func, TType value)
        {
            var expression = (MemberExpression) func.Body;
            var pn = expression.Member.Name;

            if (Properties.Any(p => p.Key.EqualsEx(pn)))
                return this;

            Properties.Add(expression.Member.Name, value);

            return this;
        }

        /// <summary>
        ///     Removes the property.
        /// </summary>
        /// <typeparam name="TModel">The type of the t model.</typeparam>
        /// <typeparam name="TType">The type of the t type.</typeparam>
        /// <param name="func">The function.</param>
        /// <returns>DynamicDataObject.</returns>
        public DynamicDataObject RemoveProperty<TModel, TType>(Expression<Func<TModel, TType>> func)
        {
            var expression = (MemberExpression) func.Body;
            var pn = expression.Member.Name;

            if (!Properties.Any(p => p.Key.EqualsEx(pn)))
                return this;

            Properties.Remove(expression.Member.Name);

            return this;
        }

        /// <summary>
        ///     Adds a property using a string.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        public DynamicDataObject AddProperty(string propertyName, object value)
        {
            if (Properties.Any(p => p.Key.EqualsEx(propertyName)))
                return this;

            Properties.Add(propertyName, value);

            return this;
        }

        /// <summary>
        ///     Removes the property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>DynamicDataObject.</returns>
        public DynamicDataObject RemoveProperty(string propertyName)
        {
            if (!Properties.Any(p => p.Key.EqualsEx(propertyName)))
                return this;

            Properties.Remove(propertyName);

            return this;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Returns the enumeration of all dynamic member names.
        /// </summary>
        /// <returns>A sequence that contains dynamic member names.</returns>
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return Properties.Keys.OrderBy(k => k);
        }

        /// <summary>
        ///     Gets a property using an expression.
        /// </summary>
        /// <typeparam name="TModel">The type of the t model.</typeparam>
        /// <typeparam name="TType">The type of the t type.</typeparam>
        /// <param name="func">The Expression.</param>
        /// <returns>TType when found, otherwise default(TType)</returns>
        public TType GetPropertyValue<TModel, TType>(Expression<Func<TModel, TType>> func)
        {
            var expression = (MemberExpression) func.Body;
            var pn = expression.Member.Name;

            var kvp = Properties.FirstOrDefault(p =>
                p.Key.EqualsEx(pn)
            );

            if (kvp.Key is null)
                return default;

            var success = Properties.TryGetValue(kvp.Key, out var value);

            return (TType) (success
                ? Convert.ChangeType(value.ToString(), typeof(TType))
                : default(TType));
        }

        /// <summary>
        ///     Gets a property using a string.
        /// </summary>
        /// <typeparam name="TType">The type of the t type.</typeparam>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>TType when found, otherwise default(TType)</returns>
        public TType GetPropertyValue<TType>(string propertyName)
        {
            var kvp = Properties.FirstOrDefault(p =>
                p.Key.EqualsEx(propertyName)
            );

            if (kvp.Key is null)
                return default;

            var success = Properties.TryGetValue(kvp.Key, out var value);

            return (TType) (success
                ? Convert.ChangeType(value.ToString(), typeof(TType))
                : default(TType));
        }

        /// <summary>
        ///     Determines whether the specified property exists.  This is useful prior to calling
        ///     <see cref="GetPropertyValue{TModel, TType}(Expression{Func{TModel, TType}})" /> or
        ///     <see cref="GetPropertyValue{TType}(string)" /> on value types.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns><c>true</c> if the specified property name has property; otherwise, <c>false</c>.</returns>
        public bool HasProperty(string propertyName)
        {
            return Properties.Any(p => p.Key.EqualsEx(propertyName));
        }

        /// <summary>
        ///     Determines whether the specified property exists.  This is useful prior to calling
        ///     <see cref="GetPropertyValue{TModel, TType}(Expression{Func{TModel, TType}})" /> or
        ///     <see cref="GetPropertyValue{TType}(string)" /> on value types.
        /// </summary>
        /// <typeparam name="TModel">Type of model</typeparam>
        /// <typeparam name="TType">Type of property</typeparam>
        /// <param name="action">The action.</param>
        /// <returns><c>true</c> if the specified action has property; otherwise, <c>false</c>.</returns>
        public bool HasProperty<TModel, TType>(Expression<Func<TModel, TType>> action)
        {
            var expression = (MemberExpression) action.Body;
            var pn = expression.Member.Name;

            return Properties.Any(p => p.Key.EqualsEx(pn));
        }

        /// <summary>
        ///     Performs an implicit conversion from <see cref="DynamicDataObject" /> to <see cref="System.Boolean" />.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator bool(DynamicDataObject obj)
        {
            return !(obj is null);
        }

        /// <summary>
        ///     Serializes this instance using <see cref="JsonConvert" /> with the current <see cref="Settings" />.
        /// </summary>
        /// <returns>System.String.</returns>
        public virtual string Serialize()
        {
            return JsonConvert.SerializeObject(this, Settings);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Provides the implementation for operations that get member values. Classes derived from the
        ///     <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic behavior for
        ///     operations such as getting a value for a property.
        /// </summary>
        /// <param name="binder">
        ///     Provides information about the object that called the dynamic operation. The binder.Name property
        ///     provides the name of the member on which the dynamic operation is performed. For example, for the
        ///     Console.WriteLine(sampleObject.SampleProperty) statement, where sampleObject is an instance of the class derived
        ///     from the <see cref="T:System.Dynamic.DynamicObject" /> class, binder.Name returns "SampleProperty". The
        ///     binder.IgnoreCase property specifies whether the member name is case-sensitive.
        /// </param>
        /// <param name="result">
        ///     The result of the get operation. For example, if the method is called for a property, you can
        ///     assign the property value to <paramref name="result" />.
        /// </param>
        /// <returns>
        ///     true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of
        ///     the language determines the behavior. (In most cases, a run-time exception is thrown.)
        /// </returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var name = binder.Name;

            return Properties.TryGetValue(name, out result);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Provides the implementation for operations that set member values. Classes derived from the
        ///     <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic behavior for
        ///     operations such as setting a value for a property.
        /// </summary>
        /// <param name="binder">
        ///     Provides information about the object that called the dynamic operation. The binder.Name property
        ///     provides the name of the member to which the value is being assigned. For example, for the statement
        ///     sampleObject.SampleProperty = "Test", where sampleObject is an instance of the class derived from the
        ///     <see cref="T:System.Dynamic.DynamicObject" /> class, binder.Name returns "SampleProperty". The binder.IgnoreCase
        ///     property specifies whether the member name is case-sensitive.
        /// </param>
        /// <param name="value">
        ///     The value to set to the member. For example, for sampleObject.SampleProperty = "Test", where
        ///     sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, the
        ///     <paramref name="value" /> is "Test".
        /// </param>
        /// <returns>
        ///     true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of
        ///     the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.)
        /// </returns>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            // Converting the property name to lowercase so that property names become case-insensitive.
            Properties[binder.Name] = value;

            // You can always add a value to a dictionary, so this method always returns true.
            return true;
        }
    }
}
