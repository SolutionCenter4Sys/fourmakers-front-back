using System;
using System.Collections.Generic;
using System.Linq;

namespace Colaboracao.Helper.Testes
{
    internal class DateTimeUtilTestsHelper
    {
        // Método para gerar data aleatória entre duas datas
        public static DateTime GerarDataAleatoria(DateTime dataInicio, DateTime dataFim)
        {
            Random random = new Random();
            int range = (dataFim - dataInicio).Days;
            return dataInicio.AddDays(random.Next(range));
        }

        // Método para gerar feriados aleatórios dentro de um intervalo de datas
        public static DateTime[] GerarFeriadosAleatorios(DateTime dataInicio, DateTime dataFim, int quantidade)
        {
            List<DateTime> feriados = new List<DateTime>();
            Random random = new Random();

            for (int i = 0; i < quantidade; i++)
            {
                int range = (dataFim - dataInicio).Days;
                DateTime feriado = dataInicio.AddDays(random.Next(range));
                feriados.Add(feriado);
            }

            return feriados.ToArray();
        }

        public static int ContarDiasUteis(DateTime dataInicio, DateTime dataFim, bool incluiFinalDeSemana, DateTime[] feriados)
        {
            int diasUteis = 0;
            HashSet<DateTime> feriadosSet = new HashSet<DateTime>(feriados.Select(f => f.Date));

            for (DateTime data = dataInicio.Date; data <= dataFim.Date; data = data.AddDays(1))
            {
                // verificacao se é feriado
                bool ehFeriado = feriadosSet.Contains(data.Date);

                // se não for feriado (não adiciona), verifica as demais condições
                if (!ehFeriado)
                {
                    // se inclui finais de semana ou o dia não é sábado nem domingo, conta como dia útil
                    if (incluiFinalDeSemana || (data.DayOfWeek != DayOfWeek.Saturday && data.DayOfWeek != DayOfWeek.Sunday))
                    {
                        diasUteis++;
                    }
                }
            }

            return diasUteis;
        }
    }
}