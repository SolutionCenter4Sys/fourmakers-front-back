using System;
using System.Globalization;

namespace Colaboracao.Helper
{
    public static class DateTimeExtension
    {
        public static bool EstaEntreDatas(this DateTime pDtReferencia, DateTime pDtInicio, DateTime pDtFim)
        {
            if (pDtInicio <= pDtReferencia)
            {
                return pDtReferencia <= pDtFim;
            }
            return false;
        }

        public static bool EstaEntreDatas(this DateTime pDtReferencia, DateTime? pDtInicio, DateTime? pDtFim)
        {
            if (pDtInicio.HasValue && pDtFim.HasValue && pDtInicio.Value <= pDtReferencia)
            {
                return pDtReferencia <= pDtFim.Value;
            }
            return false;
        }

        public static bool EstaEntreDatas(this DateTime? pDtReferencia, DateTime? pDtInicio, DateTime? pDtFim)
        {
            if (pDtReferencia.HasValue && pDtInicio.HasValue && pDtFim.HasValue && pDtInicio.Value <= pDtReferencia.Value)
            {
                return pDtReferencia.Value <= pDtFim.Value;
            }
            return false;
        }

        public static bool NaoEhFimDeSemana(this DateTime pDataHora)
        {
            return !pDataHora.EhFimDeSemana();
        }

        public static bool EhFimDeSemana(this DateTime pDataHora)
        {
            return pDataHora.DayOfWeek == DayOfWeek.Saturday || pDataHora.DayOfWeek == DayOfWeek.Sunday;
        }

        public static string ToLogFormat(this DateTime dateTime)
        {
            return dateTime.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
        }
        public static string ToLogFormat(this DateTime? dateTime)
        {
            return dateTime.HasValue
                ? dateTime.Value.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)
                : string.Empty;
        }
    }
}