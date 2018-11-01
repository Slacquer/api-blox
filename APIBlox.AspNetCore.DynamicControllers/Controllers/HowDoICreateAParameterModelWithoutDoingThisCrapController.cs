//using System;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.ModelBinding;

//// ReSharper disable once CheckNamespace
//namespace APIBlox.AspNetCore
//{
//    internal class HowDoICreateAParameterModelWithoutDoingThisCrapController : ControllerBase
//    {
//        //
//        // If your looking at this and thinking "WTF is wrong with this guy, it's so EASY to do" then
//        // please for the love of all things good and holy, enlighten me with a pull-request with a solution.
//        // I will immediately put you on my Christmas card list...
//        //
//        // Also note that once this gets used to create params, I remove it from
//        // the controllers collection, so hey at least I cleaned up a bit...
//        //
//        public ActionResult Dummy(
//            uint v0, 
//            int v1,
//            long v2, 
//            double v3,
//            short v4, 
//            Guid v5,
//            ushort v6,
//            uint v7, 
//            ulong v8, 
//            short v9, 
//            int v10, 
//            long v11,
//            decimal v12, 
//            char v13,
//            HashCode v14,
//            string v15,
//            int? v16,
//            long? v17,
//            double? v18,
//            decimal? v19
//        )
//        {
//            return null;
//        }

//        public ActionResult Dummy2(
//            [BindRequired, FromQuery] uint v0,
//            [BindRequired, FromQuery] int v1,
//            [BindRequired, FromQuery] long v2,
//            [BindRequired, FromQuery] double v3,
//            [BindRequired, FromQuery] short v4,
//            [BindRequired, FromQuery] Guid v5,
//            [BindRequired, FromQuery] ushort v6,
//            [BindRequired, FromQuery] uint v7,
//            [BindRequired, FromQuery]  ulong v8,
//            [BindRequired, FromQuery]  short v9,
//            [BindRequired, FromQuery]  int v10,
//            [BindRequired, FromQuery] long v11,
//            [BindRequired, FromQuery] decimal v12,
//            [BindRequired, FromQuery]  char v13,
//            [BindRequired, FromQuery] HashCode v14,
//            [BindRequired, FromQuery] string v15,
//            [BindRequired, FromQuery] int? v16,
//            [BindRequired, FromQuery] long? v17,
//            [BindRequired, FromQuery] double? v18,
//            [BindRequired, FromQuery] decimal? v19
//        )
//        {
//            return null;
//        }
//    }
//}
