using Colaboracao.Helper.Util;
using DataTransferObject.Domain.MapaDeAlocacao;
using DataTransferObject.Domain.MapaDeAlocacao.EditarAlocacao;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Colaboracao.Helper
{
    public class MapaUtil
    {
        private static readonly Dictionary<string, int> horasPrevistasCache = new Dictionary<string, int>();

        public static int GetHorasPrevistasMesStaticCache(int mes, int ano, DateTime[] feriados, bool forcarRecalculoHorasPrevistas = false)
        {
            var cacheKey = $"{mes}-{ano}";

            if (horasPrevistasCache.TryGetValue(cacheKey, out var cachedResult))
            {
                if (!forcarRecalculoHorasPrevistas)
                {
                    return cachedResult;
                }
            }

            var quantidadeHorasDia = HorasUtil.QuantidadeHorasDia;

            var inicio = new DateTime(ano, mes, 1);
            var fim = new DateTime(ano, mes, DateTime.DaysInMonth(ano, mes));
            var diasUteis = DateTimeUtil.CountBusinessDays(inicio, fim, false, feriados);

            var horasPrevistas = diasUteis * quantidadeHorasDia;

            horasPrevistasCache[cacheKey] = horasPrevistas;

            return horasPrevistas;
        }

        public static double GetHorasMesPeriodo(DateTime dataInicio, DateTime dataFinal, double horas, bool incluiFinalDeSemana, DateTime[] feriados)
        {
            var quantidadeHorasDia = horas;

            var horasTotal = 0.0;

            int quantidadeDias = DateTimeUtil.CountBusinessDays(dataInicio, dataFinal, incluiFinalDeSemana, feriados);

            horasTotal = quantidadeDias * quantidadeHorasDia;

            return horasTotal;
        }

        public static async Task<List<HorasMensalDTO>> GetQuantidadeHorasTotalPrevistas(DateTime dataInicio, DateTime dataFim, DateTime[] feriados)
        {
            var ret = new List<HorasMensalDTO>();
            var mesAux = dataInicio.Month;
            var anoAux = dataInicio.Year;

            var tasks = new List<Task<int>>();

            DateTime dataAtual = dataInicio;

            while (dataAtual <= dataFim)
            {
                var task = Task.Run(() => MapaUtil.GetHorasPrevistasMesStaticCache(mesAux, anoAux, feriados));
                tasks.Add(task);

                ret.Add(new HorasMensalDTO
                {
                    Horas = await task, // aguarda a conclusão da tarefa
                    Mes = mesAux,
                    Ano = anoAux
                });

                dataAtual = dataAtual.AddMonths(1);
                mesAux = dataAtual.Month;
                anoAux = dataAtual.Year;
            }

            await Task.WhenAll(tasks);

            return ret;
        }

        public static List<HorasMensalDTO> GetHorasPrevistasMes(DateTime? dataInicio, DateTime? dataFim, DateTime[] feriados)
        {
            var ret = new List<HorasMensalDTO>();

            if (dataInicio.HasValue && dataFim.HasValue)
            {
                var mesAux = dataInicio.Value.Month;
                var anoAux = dataInicio.Value.Year;
                var dataAtual = dataInicio.Value;

                while (dataAtual <= dataFim.Value)
                {
                    ret.Add(new HorasMensalDTO
                    {
                        Horas = MapaUtil.GetHorasPrevistasMesStaticCache(mesAux, anoAux, feriados),
                        Mes = mesAux,
                        Ano = anoAux
                    });

                    if (mesAux >= 12)
                    {
                        mesAux = 1;
                        anoAux++;
                    }
                    else
                    {
                        mesAux++;
                    }

                    dataAtual = new DateTime(anoAux, mesAux, 1);
                }
            }
            else
            {
                var mesAtual = DateTime.Now.Month;
                var anoAtual = DateTime.Now.Year;

                for (var i = 0; i < 12; i++)
                {
                    ret.Add(new HorasMensalDTO
                    {
                        Horas = MapaUtil.GetHorasPrevistasMesStaticCache(mesAtual, anoAtual, feriados),
                        Mes = mesAtual,
                        Ano = anoAtual
                    });

                    if (mesAtual >= 12)
                    {
                        mesAtual = 1;
                        anoAtual++;
                    }
                    else
                    {
                        mesAtual++;
                    }
                }
            }

            return ret;
        }

        public static CadastroMapaAlocacaoDTO CriarNovaAlocacao(EditarHoraDiaDTO dbrow, DateTime dataInicio, DateTime dataFim, double quantidadeDeHoras, string observacao, string oportunidade, double? percentual, sbyte? prioritario)
        {
            return new CadastroMapaAlocacaoDTO
            {
                CodigoProjeto = dbrow.CodigoProjeto.ToString(),
                CpfColaborador = dbrow.CpfColaborador,
                DataInicio = dataInicio,
                DataFim = dataFim,
                IncluiFimDeSemana = dbrow.IncluiFimDeSemana,
                QuantidadeHoras = quantidadeDeHoras,
                Observacao = observacao,
                Oportunidade = oportunidade,
                Percentual = percentual,
                Prioritario = (sbyte)prioritario
            };
        }

        public static bool EhFinalDeSemana(DateTime data) => data.DayOfWeek == DayOfWeek.Saturday || data.DayOfWeek == DayOfWeek.Sunday;

        public static int ObterNumeroDoMes(string mesString)
        {
            // Mapeie o nome do mês para um número de mês
            switch (mesString.ToLower())
            {
                case "jan": return 1;
                case "fev": return 2;
                case "mar": return 3;
                case "abr": return 4;
                case "mai": return 5;
                case "jun": return 6;
                case "jul": return 7;
                case "ago": return 8;
                case "set": return 9;
                case "out": return 10;
                case "nov": return 11;
                case "dez": return 12;
                case "feb": return 2;
                case "apr": return 4;
                case "may": return 5;
                case "aug": return 8;
                case "sep": return 9;
                case "oct": return 10;
                case "dec": return 12;

                default: return -1; // Retorna -1 para indicar erro
            }
        }

        public static string ObterNomeDoMes(int mesNumero)
        {
            // Mapeie o número do mês para o nome do mês em português
            switch (mesNumero)
            {
                case 1: return "jan";
                case 2: return "fev";
                case 3: return "mar";
                case 4: return "abr";
                case 5: return "mai";
                case 6: return "jun";
                case 7: return "jul";
                case 8: return "ago";
                case 9: return "set";
                case 10: return "out";
                case 11: return "nov";
                case 12: return "dez";
                default: throw new ArgumentOutOfRangeException("mesNumero", "Número do mês deve estar entre 1 e 12");
            }
        }

        public static bool StringSemNumerosOuCaracteresEspeciais(string input)
        {
            if (input == null) return true;
            Regex regex = new Regex("^[a-zA-Z ]+$");

            return regex.IsMatch(input);
        }
    }
}