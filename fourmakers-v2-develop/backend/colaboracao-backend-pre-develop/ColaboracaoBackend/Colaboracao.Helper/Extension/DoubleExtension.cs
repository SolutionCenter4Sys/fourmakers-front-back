using System;

namespace Colaboracao.Helper
{
    public static class DoubleExtension
    {
        public static string ToStringHoraFormatada(this double horas)
        {
            double horasInteiras = Math.Truncate(horas);
            double minutos = Math.Floor(60 * (horas - horasInteiras));
            return horasInteiras.ToString() + ":" + minutos.ToString("00");
        }
    }
}