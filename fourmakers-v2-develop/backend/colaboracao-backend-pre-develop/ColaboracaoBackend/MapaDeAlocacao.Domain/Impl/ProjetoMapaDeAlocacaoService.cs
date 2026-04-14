using Colaboracao.Core;
using Colaboracao.Helper;
using Core.Domain;
using Core.Domain.MapaAlocacao;
using DataTransferObject.Domain.MapaDeAlocacao;
using DataTransferObject.Domain.MapaDeAlocacao.Projetos;
using DataTransferObject.Domain.Projeto;
using MapaDeAlocacao.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Logs.Infra.Attributes;

namespace MapaDeAlocacao.Domain.Impl
{
    [LogDomainClass]
    public class ProjetoMapaDeAlocacaoService : IProjetoMapaDeAlocacaoService
    {
        private IMapaDeAlocacaoValidadorService _validadorService;
        private IProjetoMapaDeAlocacaoRepository _projetoMapaAlocacaoRepository;
        private ILogCore _logCore;
        private IUnitOfWork _unitOfWork;
        private IMapaAlocacaoRepository _mapaDeAlocacaoRepository;

        public ProjetoMapaDeAlocacaoService(IMapaDeAlocacaoValidadorService validadorService,
            IProjetoMapaDeAlocacaoRepository projetoMapaAlocacaoRepository,
            ILogCore logCore,
            IUnitOfWork unitOfWork,
            IMapaAlocacaoRepository mapaAlocacaoRepository)
        {
            _validadorService = validadorService;
            _projetoMapaAlocacaoRepository = projetoMapaAlocacaoRepository;
            _logCore = logCore;
            _unitOfWork = unitOfWork;
            _mapaDeAlocacaoRepository = mapaAlocacaoRepository;
        }
        public List<BuscaProjetoHorasDTO> ListarProjetoHoras(string busca, int cursor, int limite, string cpfSolicitante, int orgId)
        {
            try
            {
                _validadorService.ValidaAcesso(cpfSolicitante, orgId);

                var ret = new List<BuscaProjetoHorasDTO>();

                ret = _projetoMapaAlocacaoRepository.ListarProjetoHoras(busca, cursor, limite, orgId);

                return ret;
            }
            catch
            {
                throw;
            }
        }

        public List<StatusProjetosDTO> ListarStatusProjetos(string busca, int cursor, int limite, string cpfSolicitante, int orgId)
        {
            try
            {
                _validadorService.ValidaAcesso(cpfSolicitante, orgId);

                var ret = new List<StatusProjetosDTO>();

                ret = _projetoMapaAlocacaoRepository.ListarStatusProjetos(busca, cursor, limite, orgId);

                return ret;
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<ConsultaProjetoHorasDTO>> ConsultaProjetoHoras(int cursor, int limite, string cdProjeto, string? nomeProjeto, DisponibilidadeHorarioEnum? disponibilidadeHorario, int? cdStatusProjeto, int? codDiretoria, string cdCliente, string cliente, string gestorProjeto, string cpfSolicitante, int orgId)
        {
            try
            {
                _validadorService.ValidaAcesso(cpfSolicitante, orgId);

                var projetos = new List<ConsultaProjetoHorasDTO>();

                projetos = await _projetoMapaAlocacaoRepository.BuscarProjetosHoras(cursor, limite, cdProjeto, nomeProjeto, cdStatusProjeto, cdCliente, cliente, gestorProjeto, codDiretoria, orgId);

                return projetos;
            }
            catch
            {
                throw;
            }
        }

        private ResumoConsultarColaboradorProjetoDTO ConsultaProjetoHorasAlocadas(string cdProjeto, DateTime inicioProjeto, DateTime fimProjeto, string cpf, int orgId)
        {
            try
            {
                var colaboradoresNoProjeto = _mapaDeAlocacaoRepository.ColaboradoresAlocadosNoProjeto(cdProjeto, orgId);
                var projeto = new ResumoConsultarColaboradorProjetoDTO();
                projeto.HorasAlocadas = 0;
                projeto.HorasDisponiveis = 0;
                var baseMesesMensal = new List<HorasMensalDTO>();
                var baseMeses = new List<ForcaMensalColaboradorProjetoDTO>();
                try
                {
                    baseMeses = CalcularForcaMensalColaborador(inicioProjeto, fimProjeto, cdProjeto, orgId);
                }
                catch
                {
                    baseMeses = null;
                }

                if (baseMeses != null)
                {
                    foreach (var item in baseMeses)
                    {
                        var listaMesAno = item.MesAno[0].NomeMesAtual.Split('/');
                        var numeroMes = MapaUtil.ObterNumeroDoMes(listaMesAno[0]);
                        baseMesesMensal.Add(new HorasMensalDTO()
                        {
                            Mes = numeroMes,
                            Ano = int.Parse(listaMesAno[1]),
                            Horas = 0
                        });
                    }
                    var feriados = _projetoMapaAlocacaoRepository.GetFeriadosPorOrgId(orgId).Result;

                    var mesAnoHorasAlocadas = _mapaDeAlocacaoRepository.GetHorasTotalProjeto(baseMesesMensal, cdProjeto.ToString(), feriados, orgId).Sum(x => x.Horas);

                    if (!String.IsNullOrEmpty(cdProjeto))
                    {
                        projeto.NomeProjeto = _mapaDeAlocacaoRepository.BuscaNomeProjeto(cdProjeto, orgId);
                    }

                    projeto.HorasComerciais = _projetoMapaAlocacaoRepository.ResumoHorasComerciais(cdProjeto, orgId).HorasComerciais ?? 0;
                    projeto.HorasAlocadas = (long)mesAnoHorasAlocadas;
                    projeto.HorasDisponiveis = projeto.HorasComerciais > 0 ? (projeto.HorasComerciais - projeto.HorasAlocadas) : 0;
                }
                return projeto;
            }
            catch
            {
                throw;
            }
        }
        //private async Task<double> CalculaHorasPeriodoAlocado(List<PeriodoDTO> periodos)
        //{
        //    double quantidadeHoras = 0;

        //    await Task.Run(() =>
        //    {
        //        foreach (var periodo in periodos)
        //        {
        //            for (DateTime data = periodo.DataInicio; data.Date <= periodo.DataFim; data = data.AddDays(1))
        //            {
        //                if (periodo.IncluiFimDeSemana == false && data.DayOfWeek != DayOfWeek.Saturday && data.DayOfWeek != DayOfWeek.Sunday)
        //                {
        //                    quantidadeHoras += periodo.QuantidadeHoras;
        //                }
        //                else if (periodo.IncluiFimDeSemana == true)
        //                {
        //                    quantidadeHoras += periodo.QuantidadeHoras;
        //                }
        //            }
        //        }
        //    });

        //    return quantidadeHoras;
        //}

        private List<ColaboradorAlocadoProjetoDTO> ColaboradorAlocadoProjeto(string cdProjeto, string? cpfBusca, int? tbdBusca, DateTime inicioProjeto, DateTime fimProjeto, string cpfSolicitante, int orgId, bool exibirColabComAlocacoesAtuaisEFuturas)
        {
            try
            {
                _validadorService.ValidaAcesso(cpfSolicitante, orgId);

                var colaboradoresNoProjeto = new List<ColaboradorAlocadoDTO>();
                var ret = new List<ColaboradorAlocadoProjetoDTO>();

                if (exibirColabComAlocacoesAtuaisEFuturas)
                {
                    colaboradoresNoProjeto = _mapaDeAlocacaoRepository.ColaboradoresAlocadosNoProjetoFiltradoPorData(cdProjeto, orgId, inicioProjeto, fimProjeto);
                }
                else
                {
                    colaboradoresNoProjeto = _mapaDeAlocacaoRepository.ColaboradoresAlocadosNoProjeto(cdProjeto, orgId);
                }

                var colab = new ColaboradorAlocadoDTO();

                if (cpfBusca.HasValue())
                {
                    colaboradoresNoProjeto = colaboradoresNoProjeto.Where(x => x.CpfColaborador == cpfBusca).ToList();
                }
                if (tbdBusca.ToIntOuZero() > 0)
                {
                    colaboradoresNoProjeto = colaboradoresNoProjeto.Where(x => x.Codigo_tbd_Alocado == tbdBusca).ToList();
                }

                var feriados = _projetoMapaAlocacaoRepository.GetFeriadosPorOrgId(orgId).Result;

                foreach (var colaborador in colaboradoresNoProjeto)
                {
                    var forcaMensal = CalcularForcaMensalColaborador(inicioProjeto, fimProjeto, cdProjeto, orgId);

                    foreach (var item in forcaMensal)
                    {
                        List<PeriodoDTO> periodos = _projetoMapaAlocacaoRepository.ColaboradorPeriodoPorMesAnoProjeto(item.MesAno.FirstOrDefault().NomeMesAtual, colaborador.CpfColaborador, colaborador.Codigo_tbd_Alocado, cdProjeto, orgId);
                        item.HorasAlocadasNoMes = 0;

                        foreach (var periodo in periodos)
                        {
                            var mesAno = item.MesAno[0].NomeMesAtual.Split("/");
                            var ano = int.Parse(mesAno[1]);
                            var numeroMes = MapaUtil.ObterNumeroDoMes(mesAno[0]);

                            var dataInicio = periodo.DataInicio.Month == numeroMes && periodo.DataInicio.Year == ano
                                ? periodo.DataInicio
                                : new DateTime(ano, numeroMes, 1);

                            var ultimoDiaMes = DateTime.DaysInMonth(ano, numeroMes);
                            var dataFim = periodo.DataFim.Month == numeroMes && periodo.DataFim.Year == ano
                                ? periodo.DataFim
                                : new DateTime(ano, numeroMes, ultimoDiaMes);

                            var horas = MapaUtil.GetHorasMesPeriodo(dataInicio, dataFim, periodo.QuantidadeHoras, periodo.IncluiFimDeSemana, feriados.Select(x => x.Data).ToArray());

                            item.HorasAlocadasNoMes += horas;
                        }
                    }

                    ret.Add(new ColaboradorAlocadoProjetoDTO()
                    {
                        Cpf = colaborador.CpfColaborador,
                        CodigoTBD = colaborador.Codigo_tbd_Alocado,
                        NomeColaborador = colaborador.NomeColaborador,
                        forcaMensalColaboradorProjetoDTOs = forcaMensal
                    });
                }

                return ret;
            }
            catch
            {
                throw;
            }
        }

        private List<ForcaMensalColaboradorProjetoDTO> CalcularForcaMensalColaborador(DateTime? dataInicio, DateTime? dataFim, string codigoProjeto, int orgId)
        {
            var inicio = new DateTime();
            var fim = dataFim;
            var dataAtual = DateTime.Now;
            var listaResumo = new List<ForcaMensalColaboradorProjetoDTO>();
            var horasRangeTotal = 0.0;
            var feriados = _projetoMapaAlocacaoRepository.GetFeriadosPorOrgId(orgId).Result;

            if (dataInicio.Equals(DateTime.MinValue))
            {
                inicio = new DateTime(dataAtual.Year, dataAtual.Month, 1);
                dataInicio = inicio;
            }

            if (dataFim.Equals(DateTime.MinValue))
            {
                fim = inicio.AddMonths(12);
                dataFim = fim;
            }

            var listHorasPrevistasDTOs = MapaUtil.GetHorasPrevistasMes(dataInicio, dataFim, feriados.Select(x => x.Data).ToArray());

            foreach (var item in listHorasPrevistasDTOs)
            {
                horasRangeTotal += item.Horas;

                var mesesRetornoResumo = new List<MesesColaboradorDTO>();
                var horasTotaisAlocadasTotal = horasRangeTotal;
                int numeroMeses = listHorasPrevistasDTOs.Count();

                mesesRetornoResumo.Add(new MesesColaboradorDTO
                {
                    NomeMesAtual = new DateTime(item.Ano, item.Mes, 1).ToString("MMM/yyyy")
                });

                var resumo = new ForcaMensalColaboradorProjetoDTO
                {
                    HorasAlocadasNoMes = item.Horas,
                    MesAno = mesesRetornoResumo
                };

                listaResumo.Add(resumo);
            }
            return listaResumo;
        }

        private List<ForcaMensalProjetoDTO> ColaboradorForcaMensal(string cdProjeto, DateTime inicioProjeto, DateTime fimProjeto, string cpfSolicitante, int orgId)
        {
            var ret = new List<ForcaMensalProjetoDTO>();
            var colaboradoresNoProjeto = _mapaDeAlocacaoRepository.ColaboradoresAlocadosNoProjeto(cdProjeto, orgId);

            var forca = CalcularForcaMensalColaborador(inicioProjeto, fimProjeto, cdProjeto, orgId);

            foreach (var item in forca)
            {
                item.HorasAlocadasNoMes = item.HorasAlocadasNoMes / colaboradoresNoProjeto.Count;

                var mesAno = item.MesAno[0].NomeMesAtual.Split("/");
                var mes = MapaUtil.ObterNumeroDoMes(mesAno[0]);
                var mesAnoData = new DateTime(int.Parse(mesAno[1]), mes, 1);

                ret.Add(new ForcaMensalProjetoDTO()
                {
                    Forca = (long)item.HorasAlocadasNoMes,
                    MesAno = mesAnoData
                });
            }

            return ret;
        }

        public ConsultarColaboradorProjetoResult ConsultarColaboradoresProjeto(string cdProjeto, string cpf, int? tbdBusca, DateTime? de, DateTime? ate, string cpfSolicitante, int orgId, bool exibirAlocacoesAtuaisEFuturas)
        {
            var ret = new ConsultarColaboradorProjetoResult();

            try
            {
                _validadorService.ValidaAcesso(cpfSolicitante, orgId);
                if (!string.IsNullOrEmpty(cpf) && tbdBusca > 0)
                {
                    throw new Exception("Apenas um dos campos entre CPF e TBD ID devem ser preenchidos.");
                }

                var mesAtualDiaPrimeiro = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                de = de ?? mesAtualDiaPrimeiro;
                ate = ate ?? de.Value.AddMonths(12);

                if (String.IsNullOrEmpty(cdProjeto))
                {
                    throw new Exception("Informar código do Projeto!");
                }

                if (!_validadorService.ValidaSeExisteCodigoProjetoParaOrgId(cdProjeto, orgId))
                {
                    throw new Exception($"Código de Projeto: [{cdProjeto}] não encontrado para OrgId: [{orgId}]!");
                }

                var retServieColabAlocadoProj = this.ColaboradorAlocadoProjeto(cdProjeto, cpf, tbdBusca, de.Value, ate.Value, cpfSolicitante, orgId, exibirAlocacoesAtuaisEFuturas);
                var retServiceResumoConsultarColaboradorProjeto = this.ConsultaProjetoHorasAlocadas(cdProjeto, de.Value, ate.Value, cpfSolicitante, orgId);
                var retServiceForcaMensalProjeto = this.ColaboradorForcaMensal(cdProjeto, de.Value, ate.Value, cpfSolicitante, orgId);
                var retService = new List<ConsultarColaboradorProjetoDTO>
            {
                new ConsultarColaboradorProjetoDTO()
                {
                    colaboradorAlocadoProjeto = retServieColabAlocadoProj.OrderBy(colab => colab.NomeColaborador).ToList(),
                    forcaMensalProjeto = retServiceForcaMensalProjeto,
                    ResumoConsultarColaboradorProjeto = retServiceResumoConsultarColaboradorProjeto
                }
            };

                if (exibirAlocacoesAtuaisEFuturas)
                {
                    // retService.Where(x => x.ResumoConsultarColaboradorProjeto.HorasAlocadas == 0);
                }

                ret.ConsultarColaboradorProjetoDTO = retService;
                ret.Sucesso = true;
                return ret;
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return ret;
            }
        }
    }
}