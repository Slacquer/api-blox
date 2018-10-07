#region -    Using Statements    -

using APIBlox.AspNetCore.Errors;
using Microsoft.AspNetCore.Mvc;

#endregion

namespace APIBlox.AspNetCore.ActionResults
{
    /// <summary>
    ///     Class ProblemResult.
    ///     <para>Content-Type: application/problem+json</para>
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ObjectResult" />
    /// <inheritdoc />
    /// <seealso cref="ObjectResult" />
    /// <seealso cref="T:APIBlox.AspNetCore.Errors.RequestError" />
    /// <seealso cref="T:Microsoft.AspNetCore.Http.StatusCodes" />
    public class ProblemResult : ObjectResult
    {
        #region -    Fields    -

        /// <summary>
        ///     The error
        /// </summary>
        protected readonly RequestErrorObject Error;

        #endregion

        #region -    Constructors    -

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProblemResult" /> class.
        /// </summary>
        /// <param name="error">The error.</param>
        public ProblemResult(RequestErrorObject error)
            : base(error)
        {
            Error = error;
            StatusCode = Error.Status;
            ContentTypes.Clear();
            ContentTypes.Add("application/problem+json");
        }

        #endregion

        /// <inheritdoc />
        /// <summary>
        ///     This method is called before the formatter writes to the output stream.
        /// </summary>
        /// <param name="context">The context.</param>
        public override void OnFormatting(ActionContext context)
        {
            Error.Instance = context.HttpContext.Request.Path;

            if (Error.Status.HasValue)
                context.HttpContext.Response.StatusCode = Error.Status.Value;
            else
                Error.Status = StatusCode;

            OnBeforeFormattingComplete(context, Error);

            base.OnFormatting(context);
        }

        /// <summary>
        ///     Called just prior to <see cref="OnFormatting(ActionContext)" /> completes.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="error">The <see cref="RequestErrorObject" />.</param>
        protected virtual void OnBeforeFormattingComplete(ActionContext context, RequestErrorObject error)
        {
        }
    }
}
