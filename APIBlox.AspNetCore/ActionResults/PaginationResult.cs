#region -    Using Statements    -

using System.Net;
using Microsoft.AspNetCore.Mvc;

#endregion

namespace APIBlox.AspNetCore.ActionResults
{
    /// <summary>
    ///     Class PaginationResult.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ObjectResult" />
    public class PaginationResult : ObjectResult
    {
        #region -    Constructors    -

        /// <summary>
        ///     Initializes a new instance of the <see cref="PaginationResult" /> class.
        /// </summary>
        /// <param name="data">The data.</param>
        public PaginationResult(object data)
            : base(data)
        {
            StatusCode = (int) HttpStatusCode.OK;
        }

        #endregion
    }
}
