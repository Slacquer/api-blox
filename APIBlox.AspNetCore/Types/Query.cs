using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using APIBlox.NetCore.Extensions;
using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json;

namespace APIBlox.AspNetCore.Types
{
    /// <summary>
    ///     Class Query.
    /// </summary>
    public abstract class Query
    {
        /// <summary>
        ///     Sets the aliases and values.
        /// </summary>
        internal void SetAliasesAndValues(Dictionary<string, string> queryParams)
        {
            _props ??= GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            foreach (var qp in queryParams)
            {
                var found = false;

                foreach (var kvp in Map)
                {
                    if (!kvp.Key.EqualsEx(qp.Key) && !kvp.Value.Any(s => s.EqualsEx(qp.Key)))
                        continue;

                    var prop = _props.FirstOrDefault(p => p.Name.EqualsEx(kvp.Key));

                    prop?.SetNullableValue(this, qp.Value);

                    found = true;
                }

                if (!found)
                    Undefined.Add(qp);
            }
        }

        private PropertyInfo[] _props;

        /// <summary>
        ///     The in map for deciphering incoming query params.
        /// </summary>
        [JsonIgnore]
        protected Dictionary<string, string[]> Map { get; } = new();

        /// <summary>
        ///     Gets or sets the undefined parameters.
        /// </summary>
        [JsonIgnore]
        internal IDictionary<string, string> Undefined { get; set; } = new Dictionary<string, string>();

        /// <summary>
        ///     Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public override string ToString()
        {
            return BuildQuery().ToString();
        }

        /// <summary>
        ///     Builds the query.
        /// </summary>
        /// <returns>QueryBuilder.</returns>
        protected virtual QueryBuilder BuildQuery()
        {
            var qb = new QueryBuilder();

            if (Undefined is null)
                return qb;

            foreach (var p in Undefined)
                qb.Add(p.Key.ToCamelCase(), p.Value);

            return qb;
        }
    }
}
