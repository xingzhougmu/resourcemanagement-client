﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lithnet.ResourceManagement.Client
{
    /// <summary>
    /// Contains various helper extensions for use with the this library
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Converts a date time to the ISO 8601 date string required by the Resource Management Service
        /// </summary>
        /// <param name="dateTime">The date and time to convert</param>
        /// <returns>An ISO 8601 date format string</returns>
        public static string ToResourceManagementServiceDateFormat(this DateTime dateTime)
        {
            return Extensions.ToResourceManagementServiceDateFormat(dateTime, false);
        }

        /// <summary>
        /// Converts a date time to the ISO 8601 date string required by the Resource Management Service
        /// </summary>
        /// <param name="dateTime">The date and time to convert</param>
        /// <param name="zeroMilliseconds">A value indicating whether the millisecond component of the date should be zeroed to avoid rounding/round-trip issues</param>
        /// <returns>An ISO 8601 date format string</returns>
        public static string ToResourceManagementServiceDateFormat(this DateTime dateTime, bool zeroMilliseconds)
        {
            if (zeroMilliseconds)
            {
                return dateTime.ToString(TypeConverter.FimServiceDateFormatZeroedMilliseconds);
            }
            else
            {
                return dateTime.ToString(TypeConverter.FimServiceDateFormat);

            }
        }
    }
}