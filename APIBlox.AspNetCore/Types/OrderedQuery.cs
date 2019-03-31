using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using APIBlox.AspNetCore.Contracts;
using APIBlox.NetCore.Extensions;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace APIBlox.AspNetCore.Types
{
    /// <inheritdoc />
    /// <summary>
    ///     Class OrderedQuery.
    /// </summary>
    public class OrderedQuery : IOrderedQuery
    {
        private PropertyInfo[] _props;

        /// <summary>
        ///     Initializes a new instance of the <see cref="OrderedQuery" /> class.
        /// </summary>
        public OrderedQuery()
        {
            Map.TryAdd("OrderBy", new[] { "$OrderBy", "SortBy", "$SortBy", "Sort", "$Sort" });
        }

        /// <summary>
        ///     The in map for deciphering incoming query params.
        /// </summary>
        [JsonIgnore]
        internal Dictionary<string, string[]> Map { get; } = new Dictionary<string, string[]>();

        /// <summary>
        ///     Gets or sets the undefined parameters.
        /// </summary>
        /// <value>The other.</value>
        [JsonIgnore]
        internal IDictionary<string, string> Undefined { get; set; } = new Dictionary<string, string>();

        /// <inheritdoc />
        /// <summary>
        ///     Sets the order by.  Usage is determined by the API itself. 
        /// </summary>
        /// <value>The order by.</value>
        [FromQuery(Name = "orderBy")]
        public string OrderBy { get; set; }

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
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

            if (!OrderBy.IsEmptyNullOrWhiteSpace())
                qb.Add("orderBy", OrderBy);

            if (Undefined is null)
                return qb;

            foreach (var p in Undefined)
                qb.Add(p.Key.ToCamelCase(), p.Value);

            return qb;
        }

        /// <summary>
        ///     Sets the aliases and values.
        /// </summary>
        internal void SetAliasesAndValues(Dictionary<string, string> queryParams)
        {
            if (_props is null)
                _props = GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

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
    }
}
