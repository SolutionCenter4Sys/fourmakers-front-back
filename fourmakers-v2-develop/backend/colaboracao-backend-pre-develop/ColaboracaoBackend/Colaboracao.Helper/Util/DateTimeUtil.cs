using System;
using System.Collections.Generic;
using System.Globalization;

namespace Colaboracao.Helper
{
    public static class DateTimeUtil
    {
        /// <summary>
        /// Normaliza para <see cref="DateTimeKind.Utc"/> antes de persistir, alinhado a <see cref="DateTime.UtcNow"/> / rotinas.
        /// <see cref="DateTimeKind.Unspecified"/> é tratado como instante já em UTC (ex.: front em fuso UTC-3 que envia o instante correto,
        /// ou payload ISO em UTC interpretado sem Kind pelo binder).
        /// <see cref="DateTimeKind.Local"/> converte a partir do fuso local do servidor.
        /// </summary>
        public static DateTime? ParaUtcPreservandoInstante(DateTime? valor)
        {
            if (!valor.HasValue)
                return null;

            var dt = valor.Value;
            if (dt.Kind == DateTimeKind.Utc)
                return dt;
            if (dt.Kind == DateTimeKind.Local)
                return TimeZoneInfo.ConvertTimeToUtc(dt, TimeZoneInfo.Local);

            return DateTime.SpecifyKind(dt, DateTimeKind.Utc);
        }

        public static object ObterDateTimeValido(object pValor)
        {
            if (pValor == null || pValor == string.Empty)
            {
                return null;
            }
            try
            {
                return Convert.ToDateTime(pValor);
            }
            catch
            {
                return null;
            }
        }

        public static int? ObterIdade(DateTime? pData)
        {
            if (!pData.HasValue)
            {
                return null;
            }
            int idade = DateTime.Now.Year - Convert.ToDateTime(pData).Year;
            if (DateTime.Now.Month < Convert.ToDateTime(pData).Month || (DateTime.Now.Month == Convert.ToDateTime(pData).Month && DateTime.Now.Day < Convert.ToDateTime(pData).Day))
            {
                idade--;
            }
            return idade;
        }

        public static string ObterNomeDiaPorIdioma(DayOfWeek dayOfWeek, string cultureName)
        {
            CultureInfo cultura = new CultureInfo(cultureName);
            string nomeDia = cultura.DateTimeFormat.GetDayName(dayOfWeek);
            return char.ToUpper(nomeDia[0]) + nomeDia.Substring(1);
        }
        public static string ObterLabelMesAnoPorIdioma(int mes, int ano, string cultureName)
        {
            DateTime data = new DateTime(ano, mes, 1);
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(data.ToString("MMM/yyyy", new CultureInfo(cultureName))).Replace(".", "");
        }

        public static string ObterDataComUnderscoreParaNomeArquivo(DateTime data)
        {
            return $"{data:yyyy_MM_dd}";
        }

        public static string ObterDataEHoraComUnderscoreParaNomeArquivo(DateTime data)
        {
            return $"{data:yyyy_MM_dd_HH_mm_ss}";
        }

        public static int ObterDiferencaEmSegundos(DateTime pInicio, DateTime pFim)
        {
            return Convert.ToInt32(pFim.Subtract(pInicio).TotalSeconds);
        }

        public static int ObterDiferencaEmDias(DateTime pInicio, DateTime pFim)
        {
            return Convert.ToInt32(pFim.Subtract(pInicio).TotalDays);
        }

        public static int ObterDiferencaEmSegundos(TimeSpan pInicio, TimeSpan pFim)
        {
            return Convert.ToInt32(pInicio.Subtract(pFim).TotalSeconds);
        }

        public static string ObterSegundosFormatados(int pQuantidadeSegundos)
        {
            if (pQuantidadeSegundos <= 0)
            {
                pQuantidadeSegundos = 0;
            }
            return new DateTime(TimeSpan.FromSeconds(pQuantidadeSegundos).Ticks).ToString("HH:mm:ss");
        }

        public static bool VerificarSeDataFimEhMaiorOuIgualQueDataInicio(DateTime pInicio, DateTime pFim)
        {
            return pFim >= pInicio;
        }

        public static bool VerificarSeDataFimEhMaiorQueDataInicio(DateTime pInicio, DateTime pFim)
        {
            return pFim > pInicio;
        }

        public static int CountBusinessDays(DateTime firstDay, DateTime lastDay, bool includeWeekends, DateTime[]? bankHolidays = null)
        {
            firstDay = firstDay.Date;
            lastDay = lastDay.Date;

            if (firstDay > lastDay)
                throw new ArgumentException("Incorrect last day " + lastDay);

            TimeSpan span = lastDay - firstDay;
            int totalDays = span.Days + 1;
            int fullWeekCount = totalDays / 7;
            int extraDays = totalDays % 7;

            int businessDays = totalDays;

            if (!includeWeekends)
            {
                // Subtrair finais de semana completos
                businessDays -= fullWeekCount * 2;

                // Encontrar finais de semana nos dias restantes
                DateTime extraDaysStart = firstDay.AddDays(fullWeekCount * 7);
                for (int i = 0; i < extraDays; i++)
                {
                    if (extraDaysStart.AddDays(i).DayOfWeek == DayOfWeek.Saturday ||
                        extraDaysStart.AddDays(i).DayOfWeek == DayOfWeek.Sunday)
                    {
                        businessDays--;
                    }
                }
            }

            if (bankHolidays is not null)
            {
                HashSet<DateTime> holidaysAccounted = new HashSet<DateTime>();
                // Subtrair feriados
                foreach (DateTime bankHoliday in bankHolidays)
                {
                    DateTime bh = bankHoliday.Date;

                    if (firstDay <= bh && bh <= lastDay)
                    {
                        if (!includeWeekends && (bh.DayOfWeek == DayOfWeek.Sunday || bh.DayOfWeek == DayOfWeek.Saturday))
                            continue;

                        if (!holidaysAccounted.Contains(bh))
                        {
                            --businessDays;
                            holidaysAccounted.Add(bh);
                        }
                    }
                }
            }

            return businessDays;
        }

        public static double ConverteDiasUteisParaHoras(DateTime firstDay, DateTime lastDay, int horasAlocacao, bool includeWeekends, DateTime[]? bankHolidays = null)
        {
            var diasUteis = CountBusinessDays(firstDay, lastDay, includeWeekends, bankHolidays);

            var horasUteis = horasAlocacao * diasUteis;

            return horasUteis;
        }

        public static string ConverterHorasParaHorasMinutosFormatados(this double horas)
        {
            int horasInteiras = (int)horas;
            int minutos = (int)((horas - horasInteiras) * 60);

            if (minutos > 0)
            {
                return $"{horasInteiras}h {minutos}m";
            }
            else
            {
                return $"{horasInteiras}h";
            }
        }
        
        public static TimeSpan ParseHorasTotais(string valor)
        {
            var partes = valor.Split(':');
            int horas = int.Parse(partes[0]);
            int minutos = int.Parse(partes[1]);
            return new TimeSpan(horas, minutos, 0);
        }
    }
}