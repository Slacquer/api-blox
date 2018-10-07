#region -    Using Statements    -

using System;
using Microsoft.AspNetCore.Mvc;

#endregion

// ReSharper disable once CheckNamespace
namespace APIBlox.AspNetCore
{
    internal class HowDoICreateAParameterModelWithoutDoingThisCrapController : ControllerBase
    {
        //
        // If your looking at this and thinking "WTF is wrong with this guy, it's so EASY to do" then
        // please for the love of all things good and holy, enlighten me with a pull-request with a solution.
        // I will immediately put you on my Christmas card list...
        //
        // Also note that once this gets used to create params, I remove it from
        // the controllers collection, so hey at least I cleaned up a bit...
        //
        public ActionResult Dummy(
            uint v0, int v1, long v2, double v3, short v4, Guid v5,
            ushort v6, uint v7, ulong v8, short v9, int v10, long v11, decimal v12, char v13,
             HashCode v14, string v15
        )
        {
            return null;
        }
    }
}
