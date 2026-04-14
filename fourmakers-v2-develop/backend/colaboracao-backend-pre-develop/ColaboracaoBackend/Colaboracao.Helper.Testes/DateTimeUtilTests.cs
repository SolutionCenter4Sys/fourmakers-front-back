using System;
using System.Diagnostics;
using Xunit;

namespace Colaboracao.Helper.Testes
{
    public class DateTimeUtilTests
    {
        [Fact]
        public void TestarContagemCorretaDeDias()
        {
            int numeroDeTestes = 100000;

            bool incluiFinalDeSemana = false;

            for (int i = 0; i < numeroDeTestes; i++)
            {
                DateTime dataInicio = DateTimeUtilTestsHelper.GerarDataAleatoria(new DateTime(2020, 1, 1), new DateTime(2030, 1, 1));
                DateTime dataFim = DateTimeUtilTestsHelper.GerarDataAleatoria(dataInicio, new DateTime(2030, 12, 31));

                DateTime[] feriados = DateTimeUtilTestsHelper.GerarFeriadosAleatorios(dataInicio, dataFim, 5); // Exemplo com até 5 feriados aleatórios

                incluiFinalDeSemana = !incluiFinalDeSemana;

                // Calculando dias úteis usando os dois métodos
                var testeContarDias = DateTimeUtilTestsHelper.ContarDiasUteis(dataInicio, dataFim, incluiFinalDeSemana, feriados);
                var testeContarDias2 = DateTimeUtil.CountBusinessDays(dataInicio, dataFim, incluiFinalDeSemana, feriados);

                if (testeContarDias != testeContarDias2)
                {
                    Assert.True(false, "BusinessDaysUntil não está 'contando' os dias corretamente.");
                }
            }

            Assert.True(true);
        }
        [Fact]
        public void ComparePerformanceBetweenMethods()
        {
            int numeroDeTestes = 100000;

            bool incluiFinalDeSemana = false;

            Stopwatch stopwatch = new Stopwatch();

            // Testando ContarDiasUteis
            stopwatch.Start();
            for (int i = 0; i < numeroDeTestes; i++)
            {
                // Gerando datas de início e fim aleatórias dentro de um intervalo razoável
                DateTime dataInicio = DateTimeUtilTestsHelper.GerarDataAleatoria(new DateTime(2020, 1, 1), new DateTime(2030, 1, 1));
                DateTime dataFim = DateTimeUtilTestsHelper.GerarDataAleatoria(dataInicio, new DateTime(2030, 12, 31));

                // Gerando feriados aleatórios
                DateTime[] feriados = DateTimeUtilTestsHelper.GerarFeriadosAleatorios(dataInicio, dataFim, 5); // Exemplo com até 5 feriados aleatórios

                incluiFinalDeSemana = !incluiFinalDeSemana;

                var testeContarDias = DateTimeUtilTestsHelper.ContarDiasUteis(dataInicio, dataFim, incluiFinalDeSemana, feriados);
            }
            stopwatch.Stop();
            long contarDiasUteisElapsedTime = stopwatch.ElapsedMilliseconds;
            Console.WriteLine($"ContarDiasUteis: {contarDiasUteisElapsedTime} ms");

            stopwatch.Reset();
            stopwatch.Start();
            for (int i = 0; i < numeroDeTestes; i++)
            {
                DateTime dataInicio = DateTimeUtilTestsHelper.GerarDataAleatoria(new DateTime(2020, 1, 1), new DateTime(2030, 1, 1));
                DateTime dataFim = DateTimeUtilTestsHelper.GerarDataAleatoria(dataInicio, new DateTime(2030, 12, 31));

                DateTime[] feriados = DateTimeUtilTestsHelper.GerarFeriadosAleatorios(dataInicio, dataFim, 5);

                incluiFinalDeSemana = !incluiFinalDeSemana;

                var testeContarDias2 = DateTimeUtil.CountBusinessDays(dataInicio, dataFim, incluiFinalDeSemana, feriados);
            }
            stopwatch.Stop();
            long businessDaysUntilElapsedTime = stopwatch.ElapsedMilliseconds;
            Console.WriteLine($"BusinessDaysUntil: {businessDaysUntilElapsedTime} ms");

            Assert.True(businessDaysUntilElapsedTime < contarDiasUteisElapsedTime,
                        $"BusinessDaysUntil should be faster. ContarDiasUteis: {contarDiasUteisElapsedTime} ms, BusinessDaysUntil: {businessDaysUntilElapsedTime} ms");
        }
    }
}