using System;

namespace Colaboracao.Helper
{
    public static class ObjectExtension
    {
        public static int ToInt(this object pObject)
        {
            object validInteger = NumericUtil.GetValidInteger(pObject);
            if (validInteger != null)
            {
                return (int)validInteger;
            }
            return int.MinValue;
        }

        public static double ToDouble(this object pObject)
        {
            object validDouble = NumericUtil.GetValidDouble(pObject);
            if (validDouble != null)
            {
                return (double)validDouble;
            }
            return Double.MinValue;
        }

        public static long ToLong(this object pObject)
        {
            object validLong = NumericUtil.GetValidLong(pObject);
            if (validLong != null)
            {
                return (long)validLong;
            }
            return long.MinValue;
        }

        public static int? ToIntOuNull(this object pObject)
        {
            object validInteger = NumericUtil.GetValidInteger(pObject);
            if (validInteger != null)
            {
                return (int?)validInteger;
            }
            return null;
        }

        public static long? ToLongOuNull(this object pObject)
        {
            object validLong = NumericUtil.GetValidLong(pObject);
            if (validLong != null)
            {
                return (long?)validLong;
            }
            return null;
        }

        public static long ToLongOuZero(this object pObject)
        {
            object validLong = NumericUtil.GetValidLong(pObject);
            if (validLong != null)
            {
                return (long)validLong;
            }
            return 0L;
        }

        public static int ToIntOuZero(this object pObject)
        {
            object validInteger = NumericUtil.GetValidInteger(pObject);
            if (validInteger != null)
            {
                return (int)validInteger;
            }
            return 0;
        }

        public static decimal ToDecimalOuZero(this object pObject)
        {
            object validInteger = NumericUtil.GetValidDecimal(pObject);
            if (validInteger != null)
            {
                return (decimal)validInteger;
            }
            return 0;
        }

        public static DateTime ToDateTime(this object pObject)
        {
            object validDateTime = DateTimeUtil.ObterDateTimeValido(pObject);
            if (validDateTime != null)
            {
                return (DateTime)validDateTime;
            }
            return DateTime.MinValue;
        }

        public static DateTime? ToDateTimeOuNull(this object pObject)
        {
            object validDateTime = DateTimeUtil.ObterDateTimeValido(pObject);
            if (validDateTime != null)
            {
                return (DateTime?)validDateTime;
            }
            return null;
        }

        public static bool ToBool(this object pObject)
        {
            if (pObject == DBNull.Value)
            {
                return false;
            }
            if (pObject.ToStringOuVazio() == "0")
            {
                return false;
            }
            if (pObject.ToStringOuVazio() == "1")
            {
                return true;
            }
            return Convert.ToBoolean(pObject);
        }

        public static float ToSingle(this object pObject)
        {
            object validSingle = NumericUtil.GetValidSingle(pObject);
            if (validSingle != null)
            {
                return (float)validSingle;
            }
            return float.MinValue;
        }

        public static float? ToSingleOuNull(this object pObject)
        {
            object validSingle = NumericUtil.GetValidSingle(pObject);
            if (validSingle != null)
            {
                return (float?)validSingle;
            }
            return null;
        }

        public static string ToStringOuVazio(this object pObject)
        {
            if (pObject == null || pObject == DBNull.Value)
            {
                return string.Empty;
            }
            return pObject.ToString();
        }

        public static string ToStringOuParametro(this object pObject, string pParametro)
        {
            if (pObject == null || pObject == DBNull.Value)
            {
                return pParametro;
            }
            return pObject.ToString();
        }

        public static bool EhStringValidaEDiferenteDeZero(this object pObject)
        {
            return pObject.ToStringOuVazio().Replace("0", string.Empty) != string.Empty;
        }

        public static string ToStringOuNull(this object pObject)
        {
            if (pObject == null || pObject == DBNull.Value || pObject.IsEmpty())
            {
                return null;
            }
            return pObject.ToString();
        }

        public static string ToNullSeTextoNull(this object pObject)
        {
            if (pObject.ToStringOuVazio() == "null" || pObject.ToStringOuVazio() == "")
            {
                return null;
            }
            return pObject.ToString();
        }

        public static string ToNullSomenteSeTextoNull(this object pObject)
        {
            if (pObject.ToStringOuVazio() == "null")
            {
                return null;
            }
            return pObject.ToString();
        }

        public static string ToNullSeTextoNullOuZero(this object pObject)
        {
            if (pObject.ToStringOuVazio() == "null" || pObject.ToStringOuVazio() == "" || pObject.ToStringOuVazio() == "0")
            {
                return null;
            }
            return pObject.ToString();
        }

        public static bool IsEmpty(this object pObject)
        {
            return pObject.ToStringOuVazio().Trim() == "";
        }

        public static bool IsNotEmpty(this object pObject)
        {
            return !(pObject.ToStringOuVazio().Trim() == "");
        }

        public static bool HasValue(this object pObject)
        {
            return !pObject.IsEmpty();
        }

        public static bool IsNull(this object pObject)
        {
            return pObject is null;
        }

        public static bool IsNotNull(this object pObject)
        {
            return pObject is not null;
        }
    }
}