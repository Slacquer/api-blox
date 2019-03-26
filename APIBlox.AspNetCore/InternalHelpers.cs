﻿using System;
using APIBlox.AspNetCore.Types.Errors;

namespace APIBlox.AspNetCore
{
    internal static class InternalHelpers
    {
        public static Action<RequestErrorObject> AlterRequestErrorObjectAction { get; set; }

        public static Func<object, object> EnsureResponseCompliesWithAction { get; set; } = objectResultValue => new { Data = objectResultValue };

        public static string ErrorResponseContentType { get; set; } = "application/problem+json";

    }
}
