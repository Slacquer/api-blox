﻿using System;
using System.Diagnostics;

namespace APIBlox.NetCore.Extensions
{
    /// <summary>
    ///     Class EnumExtensions.
    /// </summary>
    [DebuggerStepThrough]
    public static class EnumExtensions
    {
        /// <summary>
        ///     Gets the type of the attribute of.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumVal">The enum value.</param>
        /// <returns>T.</returns>
        public static T GetAttributeOfType<T>(this Enum enumVal)
            where T : Attribute
        {
            var type = enumVal.GetType();
            var memInfo = type.GetMember(enumVal.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);

            return attributes.Length > 0
                ? (T) attributes[0]
                : null;
        }
    }
}
