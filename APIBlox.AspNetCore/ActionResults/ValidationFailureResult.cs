using System;
using System.Linq;
using APIBlox.AspNetCore.Errors;
using APIBlox.NetCore.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APIBlox.AspNetCore.ActionResults
{
    /// <inheritdoc />
    /// <summary>
    ///     Class ValidationFailureResult.
    /// </summary>
    /// <seealso cref="T:APIBlox.AspNetCore.ActionResults.ProblemResult" />
    public class ValidationFailureResult : ProblemResult
    {
        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:APIBlox.AspNetCore.ActionResults.ValidationFailureResult" /> class.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <param name="errorDocumentLocation">The error document location.</param>
        public ValidationFailureResult(
            int statusCode = StatusCodes.Status400BadRequest,
            string errorDocumentLocation = "about:blank"
        )
            : base(new RequestErrorObject
                {
                    Title = "One or more validation errors has occured.",
                    Detail = "Please refer to the errors property for additional details",
                    Type = errorDocumentLocation,
                    Status = statusCode
                }
            )
        {
            StatusCode = statusCode;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Called when just prior to
        ///     <see cref="M:APIBlox.AspNetCore.ActionResults.ProblemResult.OnFormatting(Microsoft.AspNetCore.Mvc.ActionContext)" />
        ///     completes.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="obj">The object.</param>
        protected override void OnBeforeFormattingComplete(ActionContext context, RequestErrorObject obj)
        {
            var ms = context.ModelState;

            if (ms.IsValid)
                throw new ArgumentException("The validation state is VALID, the action result " +
                                            $"{nameof(ValidationFailureResult)} is being inappropriately used."
                );

            var errors = context.ModelState.Keys
                .Where(k => !k.IsEmptyNullOrWhiteSpace())
                .SelectMany(key =>
                    ms[key].Errors.Select(x =>
                        {
                            dynamic ret = new DynamicErrorObject
                            {
                                Title = "Invalid Property Value",
                                Detail = x.ErrorMessage
                            };
                            ret.Property = key;

                            return ret;
                        }
                    )
                ).ToList();

            if (!errors.Any())
            {
                obj.Errors = null;
                obj.Detail = null;

                return;
            }

            foreach (var err in errors)
                obj.Errors.Add(err);
        }
    }
}
