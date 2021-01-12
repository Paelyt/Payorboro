using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GloballendingViews.Classes
{
    public static class ReportUtilities
    {
        public static string Before(this string @this, string a)
        {
            try {
                var posA = @this.IndexOf(a, StringComparison.Ordinal);
                return posA == -1 ? "" : @this.Substring(0, posA);
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public static string After(this string @this, string a)
        {
            try
            {
                var posA = @this.LastIndexOf(a, StringComparison.Ordinal);
                if (posA == -1)
                {
                    return "";
                }
                var adjustedPosA = posA + a.Length;
                return adjustedPosA >= @this.Length ? "" : @this.Substring(adjustedPosA);
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

       
    }
}