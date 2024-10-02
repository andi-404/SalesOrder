using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Globalization;

namespace SalesOrder.Extensions
{
    public static class FormatNumber
    {
        public static string ToNumberID(this decimal value)
        {
            return value.ToString("C2", new CultureInfo("id-ID")).Replace("Rp", "");
        }
    }
}
