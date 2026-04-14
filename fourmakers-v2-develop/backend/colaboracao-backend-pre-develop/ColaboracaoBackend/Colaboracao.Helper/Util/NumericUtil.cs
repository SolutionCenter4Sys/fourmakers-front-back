using System;
using System.Text.RegularExpressions;

namespace Colaboracao.Helper
{
    public static class NumericUtil
    {
        public static object GetValidInteger(object pValue)
        {
            if (pValue == null)
            {
                return null;
            }
            try
            {
                return Convert.ToInt32(pValue);
            }
            catch
            {
                return null;
            }
        }

        public static object GetValidLong(object pValue)
        {
            if (pValue == null)
            {
                return null;
            }
            try
            {
                return Convert.ToInt64(pValue);
            }
            catch
            {
                return null;
            }
        }

        public static object GetValidDouble(object pValue)
        {
            if (pValue == null)
            {
                return null;
            }
            try
            {
                return Convert.ToDouble(pValue);
            }
            catch
            {
                return null;
            }
        }

        public static object GetValidDecimal(object pValue)
        {
            if (pValue == null)
            {
                return null;
            }
            try
            {
                return Convert.ToDecimal(pValue);
            }
            catch
            {
                return null;
            }
        }

        public static bool IsValidDouble(this object pValue)
        {
            if (!pValue.IsEmpty())
            {
                return GetValidDouble(pValue) != null;
            }
            return false;
        }

        public static bool IsValidInteger(this string pValue)
        {
            return GetValidInteger(pValue) is not null;
        }

        public static object GetValidSingle(object pValue)
        {
            if (pValue == null)
            {
                return null;
            }
            try
            {
                return Convert.ToSingle(pValue);
            }
            catch
            {
                return null;
            }
        }

        public static bool IsNumeric(this string pNumeric)
        {
            if (new Regex("[^0-9]").IsMatch(pNumeric))
            {
                return false;
            }
            return true;
        }
    }
}