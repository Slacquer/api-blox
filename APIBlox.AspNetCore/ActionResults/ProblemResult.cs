using APIBlox.AspNetCore.Types;
using Microsoft.AspNetCore.Mvc;

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
        private readonly RequestErrorObject _error;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProblemResult" /> class.  I will set the following:
        ///     <para>
        ///         <see cref="RequestErrorObject.Instance" /> value to the ActionContext.HttpContext.Request.Path
        ///     </para>
        ///     <para>
        ///         <see cref="ObjectResult.StatusCode" /> value to the <see cref="RequestErrorObject.Status" />
        ///     </para>
        ///     <para>
        ///         I will clear all content types and add "application/problem+json"
        ///     </para>
        /// </summary>
        /// <param name="error">The error.</param>
        public ProblemResult(RequestErrorObject error)
            : base(error)
        {
            _error = error;
            StatusCode = _error.Status;
            ContentTypes.Clear();
            ContentTypes.Add(Constants.ResponseTypes.ErrorResponseContentType);
        }

        /// <inheritdoc />
        /// <summary>
        ///     This method is called before the formatter writes to the output stream.
        /// </summary>
        /// <param name="context">The context.</param>
        public override void OnFormatting(ActionContext context)
        {
            _error.Instance = context.HttpContext.Request.Path;

            if (_error.Status.HasValue)
                context.HttpContext.Response.StatusCode = _error.Status.Value;
            else
                _error.Status = StatusCode;

            OnBeforeFormattingComplete(context, _error);

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
