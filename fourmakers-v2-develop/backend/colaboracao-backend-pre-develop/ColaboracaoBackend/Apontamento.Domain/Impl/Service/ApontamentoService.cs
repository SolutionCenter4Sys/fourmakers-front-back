using ApiClient.Domain;
using Apontamento.Domain.Enums;
using Apontamento.Domain.Interfaces;
using Apontamento.Domain.Interfaces.Service;
using Colaboracao.Core;
using Colaboracao.Helper;
using Colaboracao.Helper.Enum;
using Colaboracao.Helper.Util;
using Core.Domain.Apontamento;
using Core.Domain.TemplateEmail;
using Core.Domain.Usuario;
using Core.Domain.Usuario.Permissao;
using DataTransferObject.Domain.Apontamento;
using DataTransferObject.Domain.Apontamento.ApontamentoValidador.ValidaApontamentoInput;
using DataTransferObject.Domain.Apontamento.ColaboradorEApontamento;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Fourmakers;
using DataTransferObject.Domain.Usuario;
using Firebase.Domain.Interfaces.Services;
using Foursys.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colaboracao.Core.Interfaces;
using SRS.Infra.Constantes;
using TemplateOrg.Constantes;

using Logs.Infra.Attributes;

namespace Apontamento.Domain.Impl.Service
{
    [LogDomainClass]
    public class ApontamentoService : IApontamentoService
    {
        private readonly IApontamentoRepository _apontamentoRepository;
        private readonly IApontamentoCacheService _apontamentoCacheService;
        private readonly IFuncionalidadeSistemaRepository _funcionalidadeSistemaRepository;
        private readonly IPeriodoFechadoService _periodoFechadoService;
        private readonly INotificacaoService _notificacaoService;
        private readonly ITemplateRepository _templateRepository;
        private readonly IUsuarioColaboradorRepository _usuarioColaboradorRepository;
        private IStringLocalizer<ApontamentoMessage> _stringLocalizer;
        private IApontamentoValidadorService _apontamentoValidadorService;
        private IBuscaParametroConfiguracaoService _buscaParametroConfiguracaoService;
        private readonly IRestricaoDeAcessoService _restricaoDeAcessoService;

        private readonly IFolhaPontoRepository _folhaPontoRepository;
        private const int QUANTIDADE_MESES_ANTERIOR_VIGENCIA_RELATORIO_DE_APONTAMENTOS = 3;

        

        public const long VINTE_QUATRO_HORAS_EM_MINUTOS = 24 * 60;

        private enum RelatorioApontamentoAcessoEnum
        {
            PossuiAcessoAoRelatorioCompleto,
            PossuiAcessoAoRelatorioGestorProjeto
        }

        public ApontamentoService(
            IApontamentoRepository apontamentoRepository,
            IApontamentoCacheService apontamentoCacheService,
            IFuncionalidadeSistemaRepository funcionalidadeSistemaRepository,
            IPeriodoFechadoService periodoFechadoService,
            INotificacaoService notificacaoService,
            ITemplateRepository templateRepository,
            IStringLocalizer<ApontamentoMessage> stringLocalizer,
            IUsuarioColaboradorRepository usuarioColaboradorRepository,
            IApontamentoValidadorService apontamentoValidadorService, IBuscaParametroConfiguracaoService buscaParametroConfiguracaoService,
            IFolhaPontoRepository folhaPontoRepository, IRestricaoDeAcessoService restricaoDeAcessoService)
        {
            _apontamentoRepository = apontamentoRepository;
            _apontamentoCacheService = apontamentoCacheService;
            _funcionalidadeSistemaRepository = funcionalidadeSistemaRepository;
            _periodoFechadoService = periodoFechadoService;
            _notificacaoService = notificacaoService;
            _templateRepository = templateRepository;
            _stringLocalizer = stringLocalizer;
            _usuarioColaboradorRepository = usuarioColaboradorRepository;
            _apontamentoValidadorService = apontamentoValidadorService;
            _buscaParametroConfiguracaoService = buscaParametroConfiguracaoService;
            _folhaPontoRepository = folhaPontoRepository;
            _restricaoDeAcessoService = restricaoDeAcessoService;
        }

        public async Task<ApontarHorasResult> ApontarHorasEmLote(string cpfRequest, int orgId, string projetoId, string atividadeId, long horas, string dataInicio, string dataFim,
         int diaQuebraSemana, bool deveSomarApontamentoDia, bool incluirSabado, bool incluirDomingo, bool incluirFeriado, string cpfColaborador, string observacao)
        {
            try
            {

                var validaResult = await _apontamentoValidadorService.ValidaApontamentoEmLote(projetoId, atividadeId, horas, dataInicio, dataFim, cpfRequest, cpfColaborador, orgId);

                DateTime dataInicioDt = DateTime.Parse(dataInicio);
                DateTime dataFimDt = DateTime.Parse(dataFim);

                List<DateTime> listaDatasPeriodo = GerarListaDatasPeriodo(dataInicioDt, dataFimDt, incluirSabado, incluirDomingo, incluirFeriado, orgId);

                List<ColaboradorApontamentoDTO> apontamentosDiarioColaboradorPorProjetoEAtividade = await _apontamentoRepository
                    .GetApontamentosColaborador(validaResult.CpfUtilizado, orgId, projetoId, atividadeId, periodo: listaDatasPeriodo);

                List<ColaboradorApontamentoDTO> apontamentosDiarioColaborador = await _apontamentoRepository
                    .GetApontamentosColaborador(validaResult.CpfUtilizado, orgId, periodo: listaDatasPeriodo);

                var statusApontamentos = await ObterListaStatusComCache();

                string idStatusPendente = statusApontamentos
                                                .SingleOrDefault(x => x.cod_status_apontamento == (int)EnumStatusApontamento.PendenteGestorProjeto)
                                                .id_status_apontamento.ToString();
                string idStatusAprovado = statusApontamentos
                                  .SingleOrDefault(x => x.cod_status_apontamento == (int)EnumStatusApontamento.Aprovado)
                                  .id_status_apontamento.ToString();

                //string erroMensagem24horas = "A soma da hora lançadas com as horas já apontadas não pode exceder 24 horas";
                string erroMensagem24horas = _stringLocalizer.GetStringOuVazio("SUM_TRACKING_HOURS_CANNOT_BE_GREATER_THAN_24_HOURS");

                var colaboradorApontamentoUltimaAlteracao = new ColaboradorApontamentoDTO();

                foreach (DateTime dataRegistro in listaDatasPeriodo)
                {
                    bool usarLancamentoAtual = false;

                    var apontamentoAtual = ObterApontamento(validaResult.EhLancamentoParaOutroColaborador, apontamentosDiarioColaboradorPorProjetoEAtividade, dataRegistro);

                    bool jaExisteApontamentoNoDia = apontamentoAtual is not null;

                    long somaHorasLancadasParaValidacao = apontamentosDiarioColaborador
                                                  .Where(x => (apontamentoAtual is null || x.Id != apontamentoAtual.Id) && x.Data == dataRegistro).Sum(s => s.Horas);

                    long horasSeraoApontadas = horas;

                    // var mensagemAprovacao = "Aprovado via funcionalidade APONTAMENTO_HORAS_COLABORADOR";
                    var mensagemAprovacao = _stringLocalizer.GetStringOuVazio("APPROVED_BY_TIMESHEET_HOURS_COLLABORATOR");

                    string justificativa = validaResult.EhLancamentoParaOutroColaborador? mensagemAprovacao : null;

                    if (jaExisteApontamentoNoDia)
                    {
                        var apontamentoEstaAprovado = apontamentoAtual.StatusGrupoCodigo == (int)EnumStatusApontamentoGrupo.Aprovado;
                        var apontamentoEstaPendente = apontamentoAtual.StatusGrupoCodigo == (int)EnumStatusApontamentoGrupo.Pendente;
                        var apontamentoEstaReprovado = apontamentoAtual.StatusGrupoCodigo == (int)EnumStatusApontamentoGrupo.Reprovado;

                        if (((apontamentoEstaAprovado || apontamentoEstaReprovado) && validaResult.EhLancamentoParaOutroColaborador)
                            || ((apontamentoEstaPendente || apontamentoEstaReprovado) && !validaResult.EhLancamentoParaOutroColaborador))
                        {
                            usarLancamentoAtual = true;
                        }
                    }

                    if (usarLancamentoAtual)
                    {
                        long horasAnteriores = apontamentoAtual.Horas;

                        if (deveSomarApontamentoDia)
                        {
                            horasSeraoApontadas += horasAnteriores;
                        }

                        if (somaHorasLancadasParaValidacao + horasSeraoApontadas > VINTE_QUATRO_HORAS_EM_MINUTOS)
                        {
                            throw new ArgumentException(erroMensagem24horas);
                        }

                        apontamentoAtual.Horas = horasSeraoApontadas;
                        apontamentoAtual.StatusApontamentoId = validaResult.EhLancamentoParaOutroColaborador ? idStatusAprovado : idStatusPendente;
                        apontamentoAtual.CodigoColaboradorJustificativa = validaResult.EhLancamentoParaOutroColaborador ? cpfRequest : null;

                        var idioma = CultureUtil.GetCurrentCulture();

                        var apontamentoAtualizado = await _apontamentoRepository.EditarApontamento(apontamentoAtual, idioma, cpfRequest);
                        await InserirLogApontamento(apontamentoAtualizado.Id, apontamentoAtual.StatusApontamentoId, apontamentoAtualizado.StatusApontamentoId, justificativa, horasAnteriores, apontamentoAtualizado.Horas, null, apontamentoAtualizado.VigenciaId, apontamentoAtualizado.ProjetoCodigo, orgId, validaResult.CpfUtilizado, apontamentoAtualizado.NumeroSemana, apontamentoAtualizado.NumeroSemanaDia, apontamentoAtualizado.AtividadeId, cpfRequest);
                        colaboradorApontamentoUltimaAlteracao = apontamentoAtualizado;
                    }
                    else
                    {
                        if (somaHorasLancadasParaValidacao + horasSeraoApontadas > VINTE_QUATRO_HORAS_EM_MINUTOS)
                        {
                            throw new ArgumentException(erroMensagem24horas);
                        }

                        ColaboradorApontamentoDTO novoApontamentoDTO = new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Horas = horasSeraoApontadas,
                            Data = dataRegistro,
                            Justificativa = null,
                            NumeroSemana = ObterNumeroSemanaDia(dataRegistro, diaQuebraSemana, orgId),
                            NumeroSemanaDia = ObterNumeroDiaNaSemana(dataRegistro, diaQuebraSemana, orgId),
                            AtividadeId = atividadeId,
                            StatusApontamentoId = validaResult.EhLancamentoParaOutroColaborador ? idStatusAprovado : idStatusPendente,
                            TipoApontamento = EnumTipoApontamento.diario.ToString(),
                            VigenciaId = (await ObterVigenciaDia(dataRegistro)).Id,
                            ProjetoCodigo = projetoId,
                            OrgId = orgId,
                            ColaboradorCpf = validaResult.CpfUtilizado,
                            Observacao = observacao,
                            CodigoColaboradorJustificativa = validaResult.EhLancamentoParaOutroColaborador ? cpfRequest : null
                        };

                        var idioma = CultureUtil.GetCurrentCulture();

                        var apontamentoInserido = await _apontamentoRepository.InserirApontamento(novoApontamentoDTO, idioma, cpfRequest);
                        await InserirLogApontamento(apontamentoInserido.Id, null, apontamentoInserido.StatusApontamentoId, justificativa, null, apontamentoInserido.Horas, null, apontamentoInserido.VigenciaId, apontamentoInserido.ProjetoCodigo, orgId, validaResult.CpfUtilizado, apontamentoInserido.NumeroSemana, apontamentoInserido.NumeroSemanaDia, apontamentoInserido.AtividadeId, cpfRequest);
                        colaboradorApontamentoUltimaAlteracao = apontamentoInserido;
                    }
                }

                var result = new ApontarHorasResult
                {
                    Sucesso = true,
                    ColaboradorApontamentoDTOUltimaAlteracao = colaboradorApontamentoUltimaAlteracao
                };
                return result;
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
                System.Console.WriteLine("Error on method: ApontarHorasEmLote");
                throw;
            }
        }

        public async Task<ColaboradorApontamentoDTO> ApontarHorasCarga(string cpfRequest, int orgId, string projetoId, string atividadeId, long horas, string dataRegistro,
            int diaQuebraSemana, bool deveSomarApontamentoDia, string cpfColaborador, string observacao)

        {
            var input = new ValidaApontamentoInput() { CpfColaborador = cpfColaborador, CpfRequest = cpfRequest, Horas = horas, DataRegistro = dataRegistro, ProjetoId = projetoId, HoraZerada = true};
            var validaResult = await _apontamentoValidadorService.ValidaApontamento(input, CRUDEnum.Create, orgId);

            var retorno = await this.ApontarHorasEmLote(cpfRequest, orgId, projetoId, atividadeId, horas, dataRegistro, dataRegistro, diaQuebraSemana, deveSomarApontamentoDia, true, true, true, cpfColaborador, observacao);

            return retorno.ColaboradorApontamentoDTOUltimaAlteracao;
        }

        
        public async Task<ColaboradorApontamentoDTO> ApontarHoras(string cpfRequest, int orgId, string projetoId, string atividadeId, long horas, string dataRegistro,
            int diaQuebraSemana, bool deveSomarApontamentoDia, string cpfColaborador, string observacao)

        {
            var input = new ValidaApontamentoInput() { CpfColaborador = cpfColaborador, CpfRequest = cpfRequest, Horas = horas, DataRegistro = dataRegistro, ProjetoId = projetoId};
            var validaResult = await _apontamentoValidadorService.ValidaApontamento(input, CRUDEnum.Create, orgId);

            var retorno = await this.ApontarHorasEmLote(cpfRequest, orgId, projetoId, atividadeId, horas, dataRegistro, dataRegistro, diaQuebraSemana, deveSomarApontamentoDia, true, true, true, cpfColaborador, observacao);

            return retorno.ColaboradorApontamentoDTOUltimaAlteracao;
        }


        public async Task<ColaboradorApontamentoDTO> EditarApontamento(EditarApontamentoDTO editarApontamentoDTO, string cpfRequest, int orgId)

        {

            var input = new ValidaApontamentoInput() { CpfRequest = cpfRequest, Horas = editarApontamentoDTO.Horas, DataRegistro = editarApontamentoDTO.DataRegistro, ColaboradorApontamentoId = editarApontamentoDTO.ApontamentoId, DataColetaDeDados = editarApontamentoDTO .DataColetaDeDados, AtividadeId = editarApontamentoDTO.AtividadeId};
            var validaResult = await _apontamentoValidadorService.ValidaApontamento(input, CRUDEnum.Update, orgId);

            var apontamentoAtual = validaResult.Apontamento;

            var statusApontamentos = await ObterListaStatusComCache();

            string idStatusPendente = statusApontamentos
                                            .SingleOrDefault(x => x.cod_status_apontamento == (int)EnumStatusApontamento.PendenteGestorProjeto)
                                            .id_status_apontamento.ToString();
            string idStatusAprovado = statusApontamentos
                                .SingleOrDefault(x => x.cod_status_apontamento == (int)EnumStatusApontamento.Aprovado)
                                .id_status_apontamento.ToString();

            var mensagemAprovacao = _stringLocalizer.GetStringOuVazio("APPROVED_BY_TIMESHEET_HOURS_COLLABORATOR");
            string justificativa = validaResult.EhLancamentoParaOutroColaborador ? mensagemAprovacao : null;
            string codigoColaboradorJustificativa = validaResult.EhLancamentoParaOutroColaborador ? cpfRequest : null;
            long horasAnteriores = apontamentoAtual.Horas;
            var dataRegistro = DateTime.Parse(editarApontamentoDTO.DataRegistro);
            apontamentoAtual.AtividadeId = editarApontamentoDTO.AtividadeId;
            apontamentoAtual.ProjetoCodigo = editarApontamentoDTO.ProjetoId;
            apontamentoAtual.Data = dataRegistro;
            apontamentoAtual.Observacao = editarApontamentoDTO.Observacao;
            apontamentoAtual.Horas = editarApontamentoDTO.Horas;
            apontamentoAtual.StatusApontamentoId = validaResult.EhLancamentoParaOutroColaborador ? idStatusAprovado : idStatusPendente;
            apontamentoAtual.CodigoColaboradorJustificativa = codigoColaboradorJustificativa;
            
            var obterNumeroSemana =  ObterNumeroSemanaDia(dataRegistro, 0, orgId);
            var obterNumeroSemanaDia =  ObterNumeroDiaNaSemana(dataRegistro, 0, orgId);
            
            apontamentoAtual.NumeroSemanaDia = obterNumeroSemanaDia;
            apontamentoAtual.NumeroSemana = obterNumeroSemana;

            var vigencias = await _apontamentoRepository.ListarVigencia();

            var novaVigencia = vigencias.Where((x => x.Mes == apontamentoAtual.Data.Month && x.Ano == apontamentoAtual.Data.Year)).FirstOrDefault();
            if (novaVigencia == null)
            {
                throw new ArgumentException("ATENÇÃO: Não foi possível realizar a edição, pois a Vigência não foi encontrada.");
            }

            apontamentoAtual.VigenciaId = novaVigencia.Id;

            var apontamentoAtualizado = await _apontamentoRepository.EditarApontamento(apontamentoAtual, CultureUtil.GetCurrentCulture(), cpfRequest);
            await InserirLogApontamento(apontamentoAtualizado.Id, apontamentoAtual.StatusApontamentoId, apontamentoAtualizado.StatusApontamentoId, justificativa, horasAnteriores, apontamentoAtualizado.Horas, null, apontamentoAtualizado.VigenciaId, apontamentoAtualizado.ProjetoCodigo, orgId, validaResult.CpfUtilizado, apontamentoAtualizado.NumeroSemana, apontamentoAtualizado.NumeroSemanaDia, apontamentoAtualizado.AtividadeId, cpfRequest);
            return apontamentoAtualizado;
        }

        private List<DateTime> GerarListaDatasPeriodo(DateTime dataInicio, DateTime dataFim, bool incluirSabado, bool incluirDomingo, bool incluirFeriado, int orgId)
        {
            var listaDatasPeriodo = new List<DateTime>();
            var feriados = ListarFeriados(orgId);

            for (var data = dataInicio; data <= dataFim; data = data.AddDays(1))
            {
                if (data.DayOfWeek == DayOfWeek.Saturday && incluirSabado ||
                    data.DayOfWeek == DayOfWeek.Sunday && incluirDomingo ||
                    feriados.Any(x => x.Data == data) && incluirFeriado ||
                    data.NaoEhFimDeSemana() && !feriados.Any(x => x.Data == data))
                {
                    listaDatasPeriodo.Add(data);
                }
            }
            return listaDatasPeriodo;
        }

        private ColaboradorApontamentoDTO ObterApontamento(bool ehLancamentoParaOutroColaborador, List<ColaboradorApontamentoDTO> apontamentosProjetoAtividade, DateTime dataRegistro)
        {
            var statusCodigos = new List<int>
            {
                (int)EnumStatusApontamentoGrupo.Pendente,
                (int)EnumStatusApontamentoGrupo.Reprovado
            };

            if (ehLancamentoParaOutroColaborador)
            {
                statusCodigos.Add((int)EnumStatusApontamentoGrupo.Aprovado);
            }

            return apontamentosProjetoAtividade
                   .FirstOrDefault(x => statusCodigos.Contains(x.StatusGrupoCodigo) && x.Data == dataRegistro);
        }

        private async Task<List<StatusApontamentoResult>> ObterListaStatusComCache()
        {
            try
            {
                var idioma = CultureUtil.GetCurrentCulture();
                var listaStatusCache = _apontamentoCacheService.ObterListaStatusCache(idioma);
                if (listaStatusCache != null)
                    return listaStatusCache;

                var resultado = await _apontamentoRepository.ListarStatus(idioma);
                _apontamentoCacheService.AtualizarListaStatusCache(resultado, idioma);

                return resultado;
            }
            catch (Exception e)
            {
                //throw new Exception("Erro ao obter a lista de status.", e);
                throw new Exception(_stringLocalizer.GetStringOuVazio("ERROR_GETTING_STATUS_LIST"), e);
            }
        }

        public async Task<List<StatusApontamentoGrupoDTO>> ObterListaStatusGrupo()
        {
            try
            {
                var listaStatus = await ObterListaStatusComCache();

                return listaStatus
                    .GroupBy(g => new { g.id_status_apontamento_grupo, g.cod_status_grupo, g.descricao_grupo })
                    .Select(s => new StatusApontamentoGrupoDTO()
                    {
                        id = s.Key.id_status_apontamento_grupo.ToString(),
                        codStatusGrupo = s.Key.cod_status_grupo,
                        descricao = s.Key.descricao_grupo
                    })
                    .Where(w => w.codStatusGrupo != (int)EnumStatusApontamentoGrupo.Deletado)
                    .OrderBy(item => item.codStatusGrupo)
                    .ToList();
            }
            catch (Exception e)
            {
                //throw new Exception("Erro ao obter a lista de grupos de status.", e);
                throw new Exception(_stringLocalizer.GetStringOuVazio("ERROR_GETTING_STATUS_GROUP_LIST"), e);
            }
        }

        public async Task<bool> DeletarApontamentoColaborador(string colaboradorApontamentoId, string cpfRequest, int orgId, DateTime dataColetaDeDados, bool HoraZerada = false)
        {
            try
            {
                var input = new ValidaApontamentoInput()
                {
                    //Apontamento = colaboradorApontamentoDTO,
                    ColaboradorApontamentoId = colaboradorApontamentoId,
                    //CpfColaborador = colaboradorApontamentoDTO.ColaboradorCpf == cpfRequest ? string.Empty : colaboradorApontamentoDTO.ColaboradorCpf,
                    CpfRequest = cpfRequest,
                    DataColetaDeDados = dataColetaDeDados,
                    HoraZerada = HoraZerada
                    
                };
                var validaResult = await _apontamentoValidadorService.ValidaApontamento(input, CRUDEnum.Delete, orgId);

                var colaboradorApontamentoDTO = validaResult.Apontamento;

                var statusApontamentos = await ObterListaStatusComCache();

                string id_status_deletado = statusApontamentos
                    .FirstOrDefault(x => x.cod_status_grupo == (int)EnumStatusApontamentoGrupo.Deletado)
                    .id_status_apontamento.ToString();

                var apontamentoDeletado = _apontamentoRepository.DeletarApontamento(colaboradorApontamentoId);

                await InserirLogApontamento(colaboradorApontamentoDTO.Id, colaboradorApontamentoDTO.StatusApontamentoId, id_status_deletado, null, colaboradorApontamentoDTO.Horas, null, null, colaboradorApontamentoDTO.VigenciaId, colaboradorApontamentoDTO.ProjetoCodigo, orgId, colaboradorApontamentoDTO.ColaboradorCpf, colaboradorApontamentoDTO.NumeroSemana, colaboradorApontamentoDTO.NumeroSemanaDia, colaboradorApontamentoDTO.AtividadeId, cpfRequest);
                return apontamentoDeletado;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private TotalizadorApontamentosDTO RetornarTotalizadorVigencia(List<ApontamentoMensalDTO> apontamentosMensais)
        {
            return new TotalizadorApontamentosDTO
            {
                QuantidadeTotalProjetos = apontamentosMensais.Select(x => x.Projeto.CodProjeto).Distinct().Count(),
                SomaHoras = apontamentosMensais.Sum(x => x.Horas)
            };
        }

        private TotalizadorApontamentosBigNumbersDTO RetornarTotalizadorBigNumbers(List<ApontamentoMensalDTO> apontamentosMensais)
        {
            return new TotalizadorApontamentosBigNumbersDTO
            {
                QuantidadeTotalColaboradores = 1,
                QuantidadeTotalNaoApontado = apontamentosMensais.Count > 0 ? "0/1" : "1/1",
                SomaHorasAprovadas = apontamentosMensais.Where(x => x.CodStatusGrupoMensal == (int)EnumStatusApontamentoGrupo.Aprovado).Sum(x => x.Horas).ToDecimalOuZero(),
                SomaHorasLancadas = apontamentosMensais.Where(x => x.CodStatusGrupoMensal == (int)EnumStatusApontamentoGrupo.Aprovado || x.CodStatusGrupoMensal == (int)EnumStatusApontamentoGrupo.Pendente)
                                    .Sum(x => x.Horas).ToDecimalOuZero(),
                SomaHorasPendentes = apontamentosMensais.Where(x => x.CodStatusGrupoMensal == (int)EnumStatusApontamentoGrupo.Pendente).Sum(x => x.Horas).ToDecimalOuZero(),
                SomaHorasReprovdas = apontamentosMensais.Where(x => x.CodStatusGrupoMensal == (int)EnumStatusApontamentoGrupo.Reprovado).Sum(x => x.Horas).ToDecimalOuZero()
            };
        }

        public async Task<ApontamentosPorVigenciaDTO> ListarApontamentosPorVigencia(int mes, int ano, string cpfRequest, string codColaborador, int orgId, string cpfColaborador)
        {
            var input = new ValidaApontamentoInput() { CpfColaborador = cpfColaborador, CpfRequest = cpfRequest };
            var validaResult = await _apontamentoValidadorService.ValidaApontamento(input, CRUDEnum.Read, orgId);

            if (codColaborador.IsEmpty())
            {
                var colaboradorDTO = await _apontamentoRepository.GetColaboradorPorCpfEOrgId(validaResult.CpfUtilizado, orgId);
                if (colaboradorDTO is null)
                {
                    //throw new Exception("Não foi possível encontrar esse colaborador para a Org especificada.");
                    throw new Exception(_stringLocalizer.GetStringOuVazio("COLLABORATOR_COULD_NOT_BE_FOUND_FOR_THE_SPECIFIED_ORG"));
                }
                codColaborador = colaboradorDTO.CodColaborador;
            }

            var result = new ApontamentosPorVigenciaDTO();

            bool soProjetosDesteGerente = false;

            var apontamentosMensais = await _apontamentoRepository.ListarApontamentosMensaisPorVigenciaColaborador(
                mes,
                ano,
                validaResult.CpfUtilizado,
                codColaborador,
                orgId,
                "",
                soProjetosDesteGerente);

            foreach (var resumoMensal in apontamentosMensais)
            {
                resumoMensal.Aprovadores = await _apontamentoRepository.ListarAprovadoresApontamentosMensaisColaborador(mes, ano, validaResult.CpfUtilizado, resumoMensal.Projeto.CodProjeto, orgId);
            }

            var apontamentosColaborador = await _apontamentoRepository.ListarApontamentosPorVigenciaColaborador(
                mes,
                ano,
                validaResult.CpfUtilizado,
                codColaborador,
                orgId,
                "",
                soProjetosDesteGerente);

            var ocultaTimesheet = false;
            
            if (!string.IsNullOrEmpty(codColaborador))
            {
                var configuracaoAcessoTimesheet = _buscaParametroConfiguracaoService.GetParametroConfiguracao<string>(ParametroOrgCodigoEnum.OCULTAR_MODULO_TIMESHEET_MODELO_CONTRATACAO, orgId, codColaborador);
                if (!string.IsNullOrEmpty(configuracaoAcessoTimesheet))
                {
                    if (configuracaoAcessoTimesheet == "Todos")
                    {
                        ocultaTimesheet = true;
                    }
                    else
                    {
                        var podeApontar = await _apontamentoRepository.VerificaSeColaboradorPodeApontarPorModeloDeTrabalho(orgId, codColaborador);
                        ocultaTimesheet = !podeApontar;
                    }
                }
            }
            
            var folhaPonto = await _folhaPontoRepository.BuscarFolhasPontoPorColaboradorAsync(cpfColaborador, mes.ToString("00") + "/" + ano.ToString(), orgId);
            if (folhaPonto.Count > 0)
            {
                if (!string.IsNullOrEmpty(folhaPonto.FirstOrDefault()?.FolhaPdf))
                {
                    result.PdfFolhaPontoUrl = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente("SERVICE_MEDIA_BASE_URL") + folhaPonto.FirstOrDefault()?.FolhaPdf;
                }
                //result.PdfHoleriteUrl = folhaPonto.FirstOrDefault()?.FolhaPdf;
            }

            result.ApontamentosMensais = apontamentosMensais;
            result.ColaboradorApontamentos = apontamentosColaborador;
            result.TotalizadorApontamentosMensais = RetornarTotalizadorVigencia(apontamentosMensais);
            result.TotalizadorApontamentosBigNumbers = RetornarTotalizadorBigNumbers(apontamentosMensais);
            result.OcultaTimeSheet = ocultaTimesheet;

            result.Vigencia = new VigenciaDTO()
            {
                Mes = mes,
                Ano = ano,
                Horas_trabalhadas = apontamentosMensais?.Sum(a => a.Horas) ?? 0,
                // Status = DefineStatusDaVigencia(apontamentosMensais)
            };
            
            result.DataColetaDeDados = await _apontamentoRepository.BuscarHorasExataBancoDeDados();

            return result;
        }

        public async Task<ApontamentosPorVigenciaDTO> ListarApontamentosVigenciaPorGerente(int mes, int ano, string cpf, string codColaborador, int orgId, string cpfGerente)
        {
            var result = new ApontamentosPorVigenciaDTO();
            var souGestorHierarquicoDesteColaborador = await _apontamentoRepository.VerificaSeEhGestorHierarquicoDeUmAprovador(orgId, codColaborador, cpfGerente);
            
            bool soProjetosDesteGerente = !souGestorHierarquicoDesteColaborador;

            var apontamentosMensais = await _apontamentoRepository.ListarApontamentosMensaisPorVigenciaColaborador(
                mes,
                ano,
                cpf,
                codColaborador,
                orgId,
                cpfGerente,
                soProjetosDesteGerente);

            foreach (var apontamentoMensal in apontamentosMensais)
            {
                apontamentoMensal.Aprovadores = await _apontamentoRepository.ListarAprovadoresApontamentosMensaisColaborador(mes, ano, cpf, apontamentoMensal.Projeto.CodProjeto, orgId);
            }

            var apontamentosColaborador = await _apontamentoRepository.ListarApontamentosPorVigenciaColaborador(
                mes,
                ano,
                cpf,
                codColaborador,
                orgId,
                cpfGerente,
                soProjetosDesteGerente);

            var folhaPonto = await _folhaPontoRepository.BuscarFolhasPontoPorColaboradorAsync(cpf, mes.ToString("00") + '/' + ano.ToString(), orgId);
            if (folhaPonto.Count > 0)
            {
                if (!string.IsNullOrEmpty(folhaPonto.FirstOrDefault()?.FolhaPdf))
                {
                    result.PdfFolhaPontoUrl = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente("SERVICE_MEDIA_BASE_URL") + folhaPonto.FirstOrDefault()?.FolhaPdf;
                }
                //result.PdfHoleriteUrl = folhaPonto.FirstOrDefault()?.FolhaPdf;
            }

            result.ApontamentosMensais = apontamentosMensais;
            result.ColaboradorApontamentos = apontamentosColaborador;
            result.TotalizadorApontamentosMensais = RetornarTotalizadorVigencia(apontamentosMensais);
            result.TotalizadorApontamentosBigNumbers = RetornarTotalizadorBigNumbers(apontamentosMensais);

            result.Vigencia = new VigenciaDTO()
            {
                Mes = mes,
                Ano = ano,
                Horas_trabalhadas = apontamentosMensais?.Sum(a => a.Horas) ?? 0,
                // Status = DefineStatusDaVigencia(apontamentosMensais)
            };

            result.DataColetaDeDados = await _apontamentoRepository.BuscarHorasExataBancoDeDados();

            return result;
        }

        //private string DefineStatusDaVigencia(List<ApontamentoMensalDTO> apontamentosMensais)
        //{
        //    if (apontamentosMensais.Count == 0)
        //    {
        //        //return "Aberto";
        //        return _stringLocalizer.GetStringOuVazio("STATUS_OPEN");
        //    }
        //    return apontamentosMensais.OrderBy(x => x.PrioridadeStatusApontamentoGrupo).First().DescricaoStatusMensal;
        //}

        public async Task<IEnumerable<ProjetoAtividadeDTO>> ListarProjetosComAtividadesPorColaborador(string cpfRequest, int orgId, string cpfColaborador)
        {
            var input = new ValidaApontamentoInput() { CpfColaborador = cpfColaborador, CpfRequest = cpfRequest };
            var validaResult = await _apontamentoValidadorService.ValidaApontamento(input, CRUDEnum.Read, orgId);

            return await _apontamentoRepository.ListarProjetosComAtividadesPorCPF(validaResult.CpfUtilizado, orgId, validaResult.EhLancamentoParaOutroColaborador);
        }

        public int ObterNumeroDiaNaSemana(DateTime dia, int diaQuebraSemana, int orgId)
        {
            var template = ObterTemplateSemanaVigencia(dia.Month, dia.Year, diaQuebraSemana, orgId);
            foreach (SemanaDTO semana in template.semanas)
            {
                foreach (DiaDTO diaDto in semana.Dias)
                {
                    if (diaDto.Data.Date == dia.Date)
                    {
                        return diaDto.NumeroSemanaDia;
                    }
                }
            }
            return -1;
        }

        public int ObterNumeroSemanaDia(DateTime dia, int diaQuebraSemana, int orgId)
        {
            var template = ObterTemplateSemanaVigencia(dia.Month, dia.Year, diaQuebraSemana, orgId);
            foreach (SemanaDTO semana in template.semanas)
            {
                foreach (DiaDTO diaDto in semana.Dias)
                {
                    if (diaDto.Data.Date == dia.Date)
                    {
                        return semana.NumeroSemana;
                    }
                }
            }
            return -1;
        }

        public TemplateSemanaVigenciaResult ObterTemplateSemanaVigencia(int mes, int ano, int diaQuebraSemana, int orgId)
        {
            try
            {
                var idioma = CultureUtil.GetCurrentCulture();
                var templateSemanaVigenciaCache = _apontamentoCacheService.ObterTemplateSemanaVigenciaCache(mes, ano, diaQuebraSemana, idioma, orgId);
                if (templateSemanaVigenciaCache != null)
                    return templateSemanaVigenciaCache;

                var resultado = CalcularTemplateSemanaVigencia(mes, ano, diaQuebraSemana, idioma, orgId);
                _apontamentoCacheService.AtualizarTemplateSemanaVigenciaCache(mes, ano, diaQuebraSemana, resultado, idioma, orgId);

                return resultado;
            }
            catch (Exception e)
            {
                //throw new Exception("Erro ao obter o template da semana da vigência.", e);
                throw new Exception(_stringLocalizer.GetStringOuVazio("ERROR_GETTING_CURRENT_WEEK_TEMPLATE"), e);
            }
        }

        private TemplateSemanaVigenciaResult CalcularTemplateSemanaVigencia(int mes, int ano, int diaQuebraSemana, string idioma, int orgId)
        {
            TemplateSemanaVigenciaResult resultado = new TemplateSemanaVigenciaResult
            {
                semanas = new List<SemanaDTO>()
            };

            DateTime primeiroDiaDoMes = new DateTime(ano, mes, 1);
            DateTime ultimoDiaDoMes = primeiroDiaDoMes.AddMonths(1).AddDays(-1);
            List<FeriadoDTO> feriados = ListarFeriados(orgId);

            DayOfWeek diaQuebraSemanaDay = (DayOfWeek)diaQuebraSemana;

            int semanasCount = 0;
            int diasCount = 1;

            for (DateTime dataAtual = primeiroDiaDoMes; dataAtual <= ultimoDiaDoMes; dataAtual = dataAtual.AddDays(1))
            {
                if (dataAtual.DayOfWeek == diaQuebraSemanaDay || resultado.semanas.Count == 0)
                {
                    diasCount = 1;

                    if (resultado.semanas.Count > 0)
                    {
                        resultado.semanas.Last().UltimoDiaSemana = dataAtual.Day - 1;
                    }

                    SemanaDTO novaSemana = new SemanaDTO
                    {
                        NumeroSemana = semanasCount + 1,
                        PrimeiroDiaSemana = dataAtual.Day,
                        Dias = new List<DiaDTO>()
                    };

                    resultado.semanas.Add(novaSemana);
                    semanasCount++;
                }

                DiaDTO novoDia = new DiaDTO
                {
                    Data = dataAtual,
                    NumeroSemanaDia = diasCount,
                    Label = DateTimeUtil.ObterNomeDiaPorIdioma(dataAtual.DayOfWeek, idioma),
                    NumeroSemana = semanasCount,
                    MesAtivo = dataAtual.Month == mes,
                    Feriado = feriados.Any(x => x.Data == dataAtual),
                };

                diasCount++;

                resultado.semanas.Last().Dias.Add(novoDia);

                if (dataAtual == ultimoDiaDoMes)
                {
                    resultado.semanas[^1].UltimoDiaSemana = dataAtual.Day;
                }
            }

            // Adicionando dias quebrados inicio do mes
            int diferencaDiasPrimeiraSemana = 7 - resultado.semanas[0].Dias.Count;
            if (diferencaDiasPrimeiraSemana != 0)
            {
                for (var i = 1; i <= diferencaDiasPrimeiraSemana; i++)
                {
                    var dateTimeDia = primeiroDiaDoMes.AddDays(-i);
                    DiaDTO novoDia = new DiaDTO
                    {
                        Data = dateTimeDia,
                        Label = DateTimeUtil.ObterNomeDiaPorIdioma(dateTimeDia.DayOfWeek, idioma),
                        MesAtivo = dateTimeDia.Month == mes,
                        Feriado = feriados.Any(x => x.Data == dateTimeDia)
                    };
                    resultado.semanas[0].Dias.Add(novoDia);
                    resultado.semanas[0].Dias = resultado.semanas[0].Dias.OrderBy(x => x.Data).ToList();
                }
            }

            // Adicionando dias quebrados fim do mês
            int diferencaDiasUltimaSemana = 7 - resultado.semanas.Last().Dias.Count;
            if (diferencaDiasUltimaSemana != 0)
            {
                for (var i = 1; i <= diferencaDiasUltimaSemana; i++)
                {
                    var dateTimeDia = ultimoDiaDoMes.AddDays(+i);
                    DiaDTO novoDia = new DiaDTO
                    {
                        Data = dateTimeDia,
                        Label = DateTimeUtil.ObterNomeDiaPorIdioma(dateTimeDia.DayOfWeek, idioma),
                        MesAtivo = dateTimeDia.Month == mes,
                        Feriado = feriados.Any(x => x.Data == dateTimeDia)
                    };
                    resultado.semanas.Last().Dias.Add(novoDia);
                    resultado.semanas.Last().Dias = resultado.semanas.Last().Dias.OrderBy(x => x.Data).ToList();
                }
            }

            return resultado;
        }

        public async Task<VigenciaDTO> ObterVigenciaDia(DateTime dia)
        {
            var vigencias = await _apontamentoRepository.ListarVigencia();
            return vigencias.FirstOrDefault(x => x.Mes == dia.Month && x.Ano == dia.Year);
        }

        private async Task<bool> InserirLogApontamento(string colaboradorApontamentoId, string tbStatusApontamentoAnteriorId, string tbStatusApontamentoNovoId,
            string justificativa, long? horasAnterior, long? horasNovo, long? horasReprovadas, string tbVigenciaId, string tbProjetoOrgCodProjeto,
            int tbOrgId, string tbColaboradorOrgTbColaboradorCpf, int? numeroSemana, int? numeroSemanaDia, string tbAtividadeId, string cpfRequest)
        {
            var colaboradorApontamentoLog = new ColaboradorApontamentoLogDTO()
            {
                Id = Guid.NewGuid().ToString(),
                DataCriacao = DateTime.Now,
                ColaboradorApontamentoId = colaboradorApontamentoId,
                TbStatusApontamentoAnteriorId = tbStatusApontamentoAnteriorId,
                TbStatusApontamentoNovoId = tbStatusApontamentoNovoId,
                Justificativa = String.IsNullOrEmpty(justificativa) ? null : justificativa,
                HorasAnterior = horasAnterior,
                HorasNovo = horasNovo,
                HorasReprovadas = horasReprovadas,
                TbVigenciaId = tbVigenciaId,
                TbProjetoOrgCodProjeto = tbProjetoOrgCodProjeto,
                TbOrgId = tbOrgId,
                TbColaboradorOrgTbColaboradorCpf = tbColaboradorOrgTbColaboradorCpf,
                NumeroSemana = numeroSemana,
                NumeroSemanaDia = numeroSemanaDia,
                TbAtividadeId = tbAtividadeId,
                TbColaboradorCpfCriacao = cpfRequest
            };

            return await _apontamentoRepository.InserirApontamentoLog(colaboradorApontamentoLog);
        }

        public async Task<List<ProjetoGerenteResult>> ListarProjetosGerenteDeProjetos(string cpf, int orgId)
        {
            try
            {
                var projetosGerente = await _apontamentoRepository.ListarProjetosGerenteDeProjetos(cpf, orgId);
                return projetosGerente;
            }
            catch (Exception e)
            {
                //throw new Exception("Erro ao obter os projetos do gerente.", e);
                throw new Exception(_stringLocalizer.GetStringOuVazio("ERROR_GETTING_PROJECTS_MANAGER"), e);
            }
        }

        public async Task<ListarVigenciaResult> ListarVigenciasProjetosGerente(string cpfGerente, int orgId)
        {
            try
            {
                var listaVigenciaSimples = await _apontamentoRepository.GetVigenciaApontamentoGerenteDeProjetos(cpfGerente, orgId);

                var idioma = CultureUtil.GetCurrentCulture();

                VigenciaSimplesDTO mesVigente = new VigenciaSimplesDTO()
                {
                    Mes = DateTime.Now.Month,
                    Ano = DateTime.Now.Year,
                    Label = DateTimeUtil.ObterLabelMesAnoPorIdioma(DateTime.Now.Month, DateTime.Now.Year, idioma)
                };

                if (!listaVigenciaSimples.Any(x => x.Mes == mesVigente.Mes && x.Ano == mesVigente.Ano))
                {
                    listaVigenciaSimples.Add(mesVigente);
                }

                listaVigenciaSimples.ForEach(f => f.Label = DateTimeUtil.ObterLabelMesAnoPorIdioma(f.Mes, f.Ano, idioma));

                ListarVigenciaResult listarVigenciaResult = new ListarVigenciaResult();
                listarVigenciaResult.mesVigente = mesVigente;
                listarVigenciaResult.meses = listaVigenciaSimples.OrderByDescending(x => x.Ano).ThenByDescending(x => x.Mes).ToList();

                return listarVigenciaResult;
            }
            catch (Exception e)
            {
                //throw new Exception("Erro ao obter as vigencias do gerente.", e);
                throw new Exception(_stringLocalizer.GetStringOuVazio("ERROR_GETTING_TERMS_MANAGER"), e);
            }
        }

        public async Task<ListarVigenciaResult> ListarVigenciasColaborador(string cpfRequest, int orgId, string cpfColaborador)
        {
            var input = new ValidaApontamentoInput() { CpfColaborador = cpfColaborador, CpfRequest = cpfRequest };
            var validaResult = await _apontamentoValidadorService.ValidaApontamento(input, CRUDEnum.Read, orgId);

            var listaVigenciaSimples = await _apontamentoRepository.GetVigenciaColaborador(validaResult.CpfUtilizado, orgId);
            
            //Trazendo vigrencias para o usuario que possui apenas Acesso a funcionalidade RELATORIO DE APONTAMENTOS Caso ele nao possua apontamentos
            if (await _funcionalidadeSistemaRepository.ValidaAcessoFuncionalidade(cpfRequest, orgId,
                    FuncionalidadeSistemaEnum.RELATORIO_DE_APONTAMENTO))
            {
                var listarVigenciaRelatorio = await _apontamentoRepository.ListarVigenciasMesEQuantidade(QUANTIDADE_MESES_ANTERIOR_VIGENCIA_RELATORIO_DE_APONTAMENTOS, DateTime.Now);
                foreach (var vigencia in listarVigenciaRelatorio)
                {
                    var buscaMesEAno = listaVigenciaSimples.Where((x => x.Mes == vigencia.Mes && x.Ano == vigencia.Ano))
                        .FirstOrDefault();
                    if (buscaMesEAno == null)
                    {
                        listaVigenciaSimples.Add(new()
                        {
                            Mes = vigencia.Mes,
                            Ano = vigencia.Ano,
                            Label = vigencia.Label
                        });
                    }
                }
            }

            var idioma = CultureUtil.GetCurrentCulture();

            VigenciaSimplesDTO mesVigente = new VigenciaSimplesDTO()
            {
                Mes = DateTime.Now.Month,
                Ano = DateTime.Now.Year,
                Label = DateTimeUtil.ObterLabelMesAnoPorIdioma(DateTime.Now.Month, DateTime.Now.Year, idioma)
            };

            if (!listaVigenciaSimples.Any(x => x.Mes == mesVigente.Mes && x.Ano == mesVigente.Ano))
            {
                listaVigenciaSimples.Add(mesVigente);
            }

            listaVigenciaSimples.ForEach(f => f.Label = DateTimeUtil.ObterLabelMesAnoPorIdioma(f.Mes, f.Ano, idioma));

            ListarVigenciaResult listarVigenciaResult = new ListarVigenciaResult();
            listarVigenciaResult.mesVigente = mesVigente;
            listarVigenciaResult.meses = listaVigenciaSimples.OrderByDescending(x => x.Ano).ThenByDescending(x => x.Mes).ToList();

            return listarVigenciaResult;
        }

        private async Task<bool> ValidarMesmaGerenciaAprovadorEApontamentos(List<string> ids, string cpf_gerente, int orgId)
        {
            var apontamentosProjetosEGerentes = await _apontamentoRepository.GetCodProjetoECpfGerenteByIdsApontamentos(ids);

            var idsFiltrados = apontamentosProjetosEGerentes
                .Where(x => (x.cpf_gerente == cpf_gerente || x.cod_gestor_hierarquico == cpf_gerente) && x.tb_org_id == orgId)
                .Select(x => x.id_apontamento)
                .ToList();

            var idsInvalidos = ids.Except(idsFiltrados.Select(id => id.ToString())).ToList();

            if (idsInvalidos.Any())
            {
                //throw new ArgumentException("Os seguintes IDs de apontamentos não correspondem à gerência informada: " + string.Join(", ", idsInvalidos));
                var errorMessage = _stringLocalizer.GetStringOuVazio("IDS_NOT_BELONG_TO_MANAGEMENT");
                errorMessage = errorMessage.Replace("{{INVALID_IDS}}", string.Join(", ", idsInvalidos));
                throw new ArgumentException(errorMessage);
            }

            return true;
        }

        private async Task ValidaSeExisteAlteracoesDuranteOperacoesEmApontamentos(List<String> ids, DateTime? dataColetaDeDados)
        {
            var existeAlteracoes = await _apontamentoRepository.ExisteApontamentosAlteradosDuranteAExecucaoPorIds(ids, dataColetaDeDados);
            if (existeAlteracoes)
            {
                throw new ArgumentException(_stringLocalizer.GetStringOuVazio("MODIFIED_DURING_CHANGES"));
            }
        }

        public async Task<int> AprovarHorasGestorProjetoEmLote(AprovarHorasGestorProjetoEmLoteDTO aprovarHorasDTO, string cpfRequest, int orgId, DateTime? dataColetaDeDados)
        {
            await ValidarMesmaGerenciaAprovadorEApontamentos(aprovarHorasDTO.Ids, cpfRequest, orgId);
            if(dataColetaDeDados != null)
            {
                await ValidaSeExisteAlteracoesDuranteOperacoesEmApontamentos(aprovarHorasDTO.Ids, dataColetaDeDados);
            }
            List<ApontamentoReduzidoDTO> idsPendentes = await VerificaSeTodosOsApontamentosEstaoPendentesENaoEstaoNoPeriodoFechado(aprovarHorasDTO.Ids, orgId);
            
            var aprovarMeusLancamentos =  _buscaParametroConfiguracaoService.GetParametroConfiguracao<bool>(ParametroOrgCodigoEnum.PERMITIR_APROVADOR_APONTAR_SEUS_LANCAMENTOS, orgId, cpfRequest);

            if (!aprovarMeusLancamentos)
            {
                foreach(var id in idsPendentes)
                {
                    await _apontamentoValidadorService.ValidaSeEhMeuLancamento(orgId, id.Id, cpfRequest);
                }
            }
            //TODO: vamos precisar fazer aqui a regra para > 8 hrs deixar pendente gestor adm
            int qtdApontamentosAprovados = _apontamentoRepository.AtualizarStatusApontamentoAprovadoOuReprovadoPorIds(idsPendentes.Select(x => x.Id).ToList(), (int)EnumStatusApontamento.Aprovado, String.IsNullOrEmpty(aprovarHorasDTO.Justificativa) ? null : aprovarHorasDTO.Justificativa, cpfRequest);
            await _apontamentoRepository.InserirLogsAprovacaoOuReprovacaoApontamentosGerenteDeProjeto(idsPendentes.Select(x => x.Id).ToList(), String.IsNullOrEmpty(aprovarHorasDTO.Justificativa) ? null : aprovarHorasDTO.Justificativa, (int)EnumStatusApontamento.Aprovado, cpfRequest);
            return qtdApontamentosAprovados;
        }

        public async Task<int> ReprovarHorasGestorProjetoEmLote(ReprovarHorasGestorProjetoEmLoteDTO reprovarHorasDTO, string cpfRequest, int orgId)
        {
            await ValidarMesmaGerenciaAprovadorEApontamentos(reprovarHorasDTO.Ids, cpfRequest, orgId);
            var listaApontamentosPendentes = await VerificaSeTodosOsApontamentosEstaoPendentesENaoEstaoNoPeriodoFechado(reprovarHorasDTO.Ids, orgId);
            int qtdApontamentosReprovados = _apontamentoRepository.AtualizarStatusApontamentoAprovadoOuReprovadoPorIds(listaApontamentosPendentes.Select(x => x.Id).ToList(), (int)EnumStatusApontamento.ReprovadoGestorProjeto, String.IsNullOrEmpty(reprovarHorasDTO.Justificativa) ? null : reprovarHorasDTO.Justificativa, cpfRequest);
            await _apontamentoRepository.InserirLogsAprovacaoOuReprovacaoApontamentosGerenteDeProjeto(listaApontamentosPendentes.Select(x => x.Id).ToList(), String.IsNullOrEmpty(reprovarHorasDTO.Justificativa) ? null : reprovarHorasDTO.Justificativa, (int)EnumStatusApontamento.ReprovadoGestorProjeto, cpfRequest);

            await EnviarNotificacoesDeReprovacao(listaApontamentosPendentes, reprovarHorasDTO.Justificativa, orgId);

            // registra o email assicronamente sem travar a execução do método em caso de não sucesso
            await RegistrarEmailReprovacaoParaEnvio(listaApontamentosPendentes, reprovarHorasDTO.Justificativa, orgId);

            return qtdApontamentosReprovados;
        }

        private async Task RegistrarEmailReprovacaoParaEnvio(List<ApontamentoReduzidoDTO> listaApontamentosPendentes, string justificativaReprovacao, int orgId)
        {
            await Task.Run(async () =>
            {
                try
                {
                    string templateParametrizado = "";

                    var retTemplateService = _templateRepository.BuscaTemplateEmail(TemplateOrgParametroEnum.REPROVACAO_APONTAMENTO_HORAS, CultureUtil.GetCurrentCulture());
                    var primeiroLancamento = listaApontamentosPendentes.FirstOrDefault();

                    if (retTemplateService == null)
                    {
                        _stringLocalizer.GetStringOuVazio("ERROR_SENDING_REJECTION_EMAIL_TEMPLATE_NOT_REGISTERED");
                    }

                    if (primeiroLancamento == null)
                    {
                        _stringLocalizer.GetStringOuVazio("ERROR_SENDING_REJECTION_EMAIL_EMPTY_LIST");
                    }

                    templateParametrizado = retTemplateService.Template;
                    templateParametrizado = templateParametrizado.Replace("${NOME}", primeiroLancamento.NomeCompletoColaborador);
                    templateParametrizado = templateParametrizado.Replace("${JUSTIFICATIVA_REPROVACAO}", justificativaReprovacao);

                    var linhaReprovacaoTemplate = @"   <tr>
                                                <td style=""border:1px solid #ccc;padding:6px;text-align:left"">${DATA_LANCAMENTO}</td>
                                                <td style=""border:1px solid #ccc;padding:6px;text-align:left"">${PROJETO}</td>
                                                <td style=""border:1px solid #ccc;padding:6px;text-align:left"">${ATIVIDADE}</td>
                                          </tr>";

                    var linhaReprovacaoParametrizada = "";

                    foreach (var apontamentoPendente in listaApontamentosPendentes.OrderByDescending(x => x.Data))
                    {
                        var nomeProjeto = apontamentoPendente.NomeProjeto;
                        var codProjeto = apontamentoPendente.CodProjeto;
                        var projeto = (nomeProjeto ?? "-") == "-" ? codProjeto : $"{codProjeto} - {nomeProjeto}";

                        linhaReprovacaoParametrizada += linhaReprovacaoTemplate.Replace("${PROJETO}", projeto)
                                                                               .Replace("${ATIVIDADE}", apontamentoPendente.AtividadeDescricao)
                                                                               .Replace("${DATA_LANCAMENTO}", apontamentoPendente.Data.ToString("dd/MM/yyyy"));
                    }

                    templateParametrizado = templateParametrizado.Replace("${LINHA_REPROVACAO}", linhaReprovacaoParametrizada);

                    _templateRepository.RegistraTemplateEmail(orgId, primeiroLancamento.EmailColaborador, _stringLocalizer.GetStringOuVazio("SUBJECT_TIMESHEET_HOURS_REJECTED"), templateParametrizado);
                }
                catch (Exception)
                {
                    // colocar log aqui posteriormente, por enquanto, o sistema não vai travar por não conseguir mandar o e-mail
                }
            });
        }

        private async Task EnviarNotificacoesDeReprovacao(List<ApontamentoReduzidoDTO> idsPendentes, string justificativa, int orgId)
        {
            var cpfs = idsPendentes.Select(x => x.Cpf).Distinct().ToList();
            var projetos = idsPendentes.Select(x => x.NomeProjeto).Distinct().ToList();
            var projetosJoined = String.Join(", ", projetos);

            string tituloNotificacao = _stringLocalizer.GetStringOuVazio("REJECTED_HOURS");
            string mensagemNotificacao = ConstruirMensagemDeNotificacao(projetos, projetosJoined, justificativa);
            foreach (var cpf in cpfs)
            {
                await _notificacaoService.EnviarNotificacaoColaborador(cpf, orgId, tituloNotificacao, mensagemNotificacao, null, FuncionalidadeSistemaEnum.APONTAMENTO_HORAS_COLABORADOR);
            }
        }

        private string ConstruirMensagemDeNotificacao(List<string> projetos, string projetosJoined, string justificativa)
        {
            string plural = projetos.Count() > 1 ? "s" : "";

            string mensagemNotificacao = _stringLocalizer.GetStringOuVazio("NOTIFICATION_REJECTED_PROJECT");
            mensagemNotificacao = mensagemNotificacao.Replace("{{PLURAL}}", plural);
            mensagemNotificacao = mensagemNotificacao.Replace("{{PROJECTS_JOINED}}", projetosJoined);

            if (!String.IsNullOrEmpty(justificativa))
            {
                mensagemNotificacao += Environment.NewLine;
                mensagemNotificacao += _stringLocalizer.GetStringOuVazio("NOTIFICATION_REJECTED_PROJECT_APPROVE_SAY");
                mensagemNotificacao = mensagemNotificacao.Replace("{{JUSTIFICATION}}", $"{justificativa}. ");
            }

            mensagemNotificacao += _stringLocalizer.GetStringOuVazio("NOTIFICATION_REJECTED_PROJECT_ACCESS_MORE_DETAILS");

            return mensagemNotificacao;
        }

        private async Task<List<ApontamentoReduzidoDTO>> VerificaSeTodosOsApontamentosEstaoPendentesENaoEstaoNoPeriodoFechado(List<string> ids, int orgId)
        {
            List<ApontamentoReduzidoDTO> apontamentos = await _apontamentoRepository.GetIdsApontamentosPorCodStatusApontamento(ids, (int)EnumStatusApontamento.PendenteGestorProjeto, orgId);

            if (apontamentos.Count != ids.Count)
            {
                //throw new ArgumentException("Status dos apontamentos não podem ser alterados pois estão com status diferente de Pendente");
                throw new ArgumentException(_stringLocalizer.GetStringOuVazio("STATUS_CHANGE_RESTRICTED_FOR_NON_PENDING"));
            }

            await _periodoFechadoService.ValidaSeEstaNoPeriodoFechado(apontamentos.Select(x => x.Data.ToString()).ToList(), orgId);

            return apontamentos;
        }

        public async Task<IEnumerable<ColaboradoresVinculadosGerenteDTO>> ListarColaboradoresVinculadosGerenteDeProjetoComApontamentos(int mes, int ano, string codProjeto, string cpf, int orgId)
        {
            if ((mes != 0 || ano != 0)
                && !DateTime.TryParseExact($"{mes:D2}/{ano}", "MM/yyyy", null, System.Globalization.DateTimeStyles.None, out _))
            {
                //throw new ArgumentException($"Os parâmetros mes: '{mes}' e ano: '{ano}' foram informados incorretamente.");
                var errorMessage = _stringLocalizer.GetStringOuVazio("INVALID_MONTH_AND_YEAR_PARAMETERS");
                errorMessage = errorMessage.Replace("{{MOUNTH}}", mes.ToStringOuVazio());
                errorMessage = errorMessage.Replace("{{YEAR}}", ano.ToStringOuVazio());
                throw new ArgumentException(errorMessage);
            }
            
            var restricaoDiretorias = await _restricaoDeAcessoService.ListarMinhasRestricoesDeAcessoPorCodigoInternoETipo(cpf, orgId, RestricaoDeAcessoTipoConstants.DIRETORIA);
            return await _apontamentoRepository.ListarColaboradoresVinculadosGerenteDeProjetoComApontamentos(mes, ano, codProjeto, cpf, orgId, restricaoDiretorias);
        }

        public async Task<IEnumerable<ColaboradoresVinculadosGerenteDTO>> ListarGerentesAdministrativosDosColaboradoresVinculadosGerenteProjeto(string cpf, int orgId)
        {
            try
            {
                var restricaoDiretorias = await _restricaoDeAcessoService.ListarMinhasRestricoesDeAcessoPorCodigoInternoETipo(cpf, orgId, RestricaoDeAcessoTipoConstants.DIRETORIA);
                var gerentesAdm = await _apontamentoRepository.ListarGerentesAdministrativosDosColaboradoresVinculadosGerenteProjeto(cpf, orgId, restricaoDiretorias);
                return gerentesAdm;
            }
            catch (Exception e)
            {
                //throw new Exception("Erro ao obter os gerentes administrativos.", e);
                var errorMessage = _stringLocalizer.GetStringOuVazio("ERROR_GETTING_ADMIN_MANAGERS");
                throw new Exception(errorMessage, e);
            }
        }

        public async Task<ListaComTotalizadorHorasResult<ProjetoVisaoGerenteDTO>> ListarProjetosVisaoGerenteDeProjeto(string codProjeto, int mes, int ano, string cpfColaborador, int codStatusGrupo, string cpfRequest, int orgId, string cpfGerenteAdm)
        {
            var restricaoDiretorias = await _restricaoDeAcessoService.ListarMinhasRestricoesDeAcessoPorCodigoInternoETipo(cpfRequest, orgId, RestricaoDeAcessoTipoConstants.DIRETORIA);
            var listaProjetosGerente = await _apontamentoRepository.ListarProjetosVisaoGerenteDeProjeto(cpfRequest, orgId, codProjeto, mes, ano, cpfColaborador, codStatusGrupo, cpfGerenteAdm, restricaoDiretorias);
            listaProjetosGerente.DataColetaDeDados = await _apontamentoRepository.BuscarHorasExataBancoDeDados();;

            return listaProjetosGerente;
        }

        private List<FeriadoDTO> ListarFeriados(int orgId)
        {
            try
            {
                var listaFeriadoCache = _apontamentoCacheService.ObterListaFeriadoCache(orgId);
                if (listaFeriadoCache != null)
                {
                    return listaFeriadoCache;
                }
                var resultado = _apontamentoRepository.ListarFeriados(orgId);
                _apontamentoCacheService.AtualizarListaFeriadoCache(resultado, orgId);
                return resultado;
            }
            catch (Exception e)
            {
                //throw new Exception("Erro ao obter a lista de feriados.", e);
                var errorMessage = _stringLocalizer.GetStringOuVazio("ERROR_GETTING_HOLIDAY_LIST");
                throw new Exception(errorMessage, e);
            }
        }

        public async Task<ApiGenericResult<FileContentResult>> RelatorioApontamentos(int mes, int ano, string cpfRequest, int orgId)
        {
            return await GerarRelatorioApontamentos(mes, ano, cpfRequest, orgId, ehSimplificado: false);
        }

        public async Task<ApiGenericResult<FileContentResult>> RelatorioApontamentosSimplificado(int mes, int ano, string cpfRequest, int orgId)
        {
            return await GerarRelatorioApontamentos(mes, ano, cpfRequest, orgId, ehSimplificado: true);
        }

        private async Task<ApiGenericResult<FileContentResult>> GerarRelatorioApontamentos(int mes, int ano, string cpfRequest, int orgId, bool ehSimplificado)
        {
            FuncionalidadeSistemaEnum funcionalidadeSistemaEnum = ehSimplificado ? FuncionalidadeSistemaEnum.RELATORIO_DE_APONTAMENTO_SIMPLIFICADO : FuncionalidadeSistemaEnum.RELATORIO_DE_APONTAMENTO;

            var relatorioApontamentoAcessoEnum = ValidarAcessoAoRelatorioApontamentos(cpfRequest, orgId, funcionalidadeSistemaEnum);
            var considerarApenasAtivos = _buscaParametroConfiguracaoService.GetParametroConfiguracao<bool>(ParametroOrgCodigoEnum.CONSIDERAR_APENAS_ATIVOS_RELATORIO_APONTAMENTO_SIMPLIFICADO, orgId, cpfRequest);

            var ret = new ApiGenericResult<FileContentResult>();
            try
            {
                if (mes > 12)
                {
                    //ret.Mensagem = "Mês informado inválido!";
                    ret.Mensagem = _stringLocalizer.GetStringOuVazio("INVALID_MONTH");
                    ret.Sucesso = false;
                    return ret;
                }

                if (ano.ToString().Length < 4 || ano > DateTime.Now.Year)
                {
                    ret.Mensagem = "Ano informado inválido!";
                    ret.Mensagem = _stringLocalizer.GetStringOuVazio("INVALID_YEAR");
                    ret.Sucesso = false;
                    return ret;
                }
                
                var restricaoDiretorias = await _restricaoDeAcessoService.ListarMinhasRestricoesDeAcessoPorCodigoInternoETipo(cpfRequest, orgId, RestricaoDeAcessoTipoConstants.DIRETORIA);
                var apontamentosResult = new List<dynamic>();

                if (relatorioApontamentoAcessoEnum == RelatorioApontamentoAcessoEnum.PossuiAcessoAoRelatorioCompleto)
                {
                    apontamentosResult = ehSimplificado
                        ? await _apontamentoRepository.RelatorioApontamentoSimplificado(mes, ano, orgId, restricaoDiretorias, considerarApenasAtivos)
                        : await _apontamentoRepository.RelatorioApontamento(mes, ano, orgId, null, restricaoDiretorias);
                }
                else if (relatorioApontamentoAcessoEnum == RelatorioApontamentoAcessoEnum.PossuiAcessoAoRelatorioGestorProjeto)
                {
                    if (ehSimplificado)
                    {
                        //throw new Exception("Não é possível extrair um relatório com Visão Gerente");
                        throw new Exception(_stringLocalizer.GetStringOuVazio("CANNOT_EXTRACT_MANAGER_VIEW_REPORT"));
                    }
                    apontamentosResult = await _apontamentoRepository.RelatorioApontamento(mes, ano, orgId, cpfRequest, restricaoDiretorias);
                }
                else
                {
                    //throw new Exception("Erro não identificado ao recuperar RelatorioApontamentoEnum.");
                    throw new Exception(_stringLocalizer.GetStringOuVazio("UNIDENTIFIED_ERROR_REPORT_ACCESS_ENUM"));
                }

                if (!apontamentosResult.Any())
                {
                    //ret.Mensagem = "Não existem apontamentos para o período informado.";
                    ret.Mensagem = _stringLocalizer.GetStringOuVazio("NO_ENTRIES_FOR_PERIOD");
                    ret.Sucesso = false;
                    return ret;
                }

                var filePrefix = ehSimplificado ? "nova_cooperativa_prod" : "Relatorio_Apontamento";

                var fileBytes = ehSimplificado ? ExcelFileUtil.CreateExcelFile(apontamentosResult, "report", mustFormatWithTable: false) : ExcelFileUtil.CreateExcelFile(apontamentosResult);

                var fileName = ehSimplificado ? $"{filePrefix} {new DateTime(ano, mes, 1):dd-MM-yy} - {new DateTime(ano, mes, DateTime.DaysInMonth(ano, mes)):dd-MM-yy}-Relatório de cliente.xlsx"
                                               : $"{filePrefix}_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.xlsx";

                ret.Retorno = new FileContentResult(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = fileName
                };
            }
            catch (Exception ex)
            {
                ret.Sucesso = false;
                ret.Mensagem = ex.Message;
            }

            return ret;
        }

        private RelatorioApontamentoAcessoEnum ValidarAcessoAoRelatorioApontamentos(string cpf, int orgId, FuncionalidadeSistemaEnum funcionalidadeSistema)
        {
            var funcionalidadesPermitidas = new[]
            {
                FuncionalidadeSistemaEnum.RELATORIO_DE_APONTAMENTO,
                FuncionalidadeSistemaEnum.RELATORIO_DE_APONTAMENTO_SIMPLIFICADO
            };

            if (!funcionalidadesPermitidas.Contains(funcionalidadeSistema))
            {
                //throw new Exception("Erro ao validar relatório, parâmetro de funcionalidade do sistema informado não é válido para este contexto.");
                throw new Exception(_stringLocalizer.GetStringOuVazio("ERROR_VALIDATE_REPORT_INVALID_PARAM"));
            }

            var isValid = _funcionalidadeSistemaRepository.ValidaAcessoFuncionalidade(cpf, orgId, funcionalidadeSistema).Result;

            if (isValid)
            {
                return RelatorioApontamentoAcessoEnum.PossuiAcessoAoRelatorioCompleto;
            }

            if (funcionalidadeSistema == FuncionalidadeSistemaEnum.RELATORIO_DE_APONTAMENTO)
            {
                var listaprojetosgerente = ListarProjetosGerenteDeProjetos(cpf, orgId).Result;

                if (listaprojetosgerente.Any())
                {
                    return RelatorioApontamentoAcessoEnum.PossuiAcessoAoRelatorioGestorProjeto;
                }
            }

            //throw new UnauthorizedAccessException($"Acesso negado: Relatório de Apontamentos{(funcionalidadeSistema == FuncionalidadeSistemaEnum.RELATORIO_DE_APONTAMENTO_SIMPLIFICADO ? " Simplificado" : "")}.");
            var errorMessage = (funcionalidadeSistema == FuncionalidadeSistemaEnum.RELATORIO_DE_APONTAMENTO_SIMPLIFICADO ? _stringLocalizer.GetStringOuVazio("ACCESS_DENIED_TIMESHEET_SIMPLIFIED") : _stringLocalizer.GetStringOuVazio("ACCESS_DENIED_TIMESHEET"));

            throw new UnauthorizedAccessException(errorMessage);
        }

        private bool ValidarAcessoSomenteAoRelatorioApontamento(string cpf, int orgId)
        {
            var funcionalidadeRelatorioApontamento = FuncionalidadeSistemaEnum.RELATORIO_DE_APONTAMENTO;

            var isValid = _funcionalidadeSistemaRepository.ValidaAcessoFuncionalidade(cpf, orgId, funcionalidadeRelatorioApontamento).Result;

            if (isValid)
            {
                return true;
            }

            //throw new UnauthorizedAccessException($"Acesso negado: Relatório de Apontamentos.");
            throw new UnauthorizedAccessException(_stringLocalizer.GetStringOuVazio("ACCESS_DENIED_TIMESHEET"));
        }

        private RelatorioApontamentoAcessoEnum ValidarAcessoAoRelatorioApontamentoSimplificado(string cpf, int orgId)
        {
            var isValid = _funcionalidadeSistemaRepository.ValidaAcessoFuncionalidade(cpf, orgId, FuncionalidadeSistemaEnum.RELATORIO_DE_APONTAMENTO_SIMPLIFICADO).Result;

            if (isValid)
            {
                return RelatorioApontamentoAcessoEnum.PossuiAcessoAoRelatorioCompleto;
            }

            var listaprojetosgerente = ListarProjetosGerenteDeProjetos(cpf, orgId).Result;

            if (listaprojetosgerente.Any())
            {
                return RelatorioApontamentoAcessoEnum.PossuiAcessoAoRelatorioGestorProjeto;
            }

            //throw new UnauthorizedAccessException("Acesso negado: Relatório de Apontamentos Simplificado.");
            throw new UnauthorizedAccessException(_stringLocalizer.GetStringOuVazio("ACCESS_DENIED_TIMESHEET_SIMPLIFIED"));
        }

        public async Task<ListaComTotalizadorHorasResult<ColaboradorEApontamentoResult>> ListarColaboradoresEApontamentosPorGestor(string nomeColaborador, string codigoGerente, string codigoStatus, int mesVigencia, int anoVigencia, int orgId, string codProjeto, string codColaboradorExternoAprovador, int cursor, int limite, string cpfRequset)
        {
            mesVigencia = mesVigencia.ToIntOuZero();
            anoVigencia = anoVigencia.ToIntOuZero();
            limite = limite.ToIntOuZero();

            ValidacaoUtil.ObrigaCursorLimite(cursor, limite);
            ValidacaoUtil.ObrigaMesAno(mesVigencia, anoVigencia);
            ValidacaoUtil.ValidaMesAno(mesVigencia, anoVigencia);
            
            var restricaoDiretorias = await _restricaoDeAcessoService.ListarMinhasRestricoesDeAcessoPorCodigoInternoETipo(cpfRequset, orgId, RestricaoDeAcessoTipoConstants.DIRETORIA);
            var considerarApenasAtivos = _buscaParametroConfiguracaoService.GetParametroConfiguracao<bool>(ParametroOrgCodigoEnum.CONSIDERAR_APENAS_ATIVOS_RELATORIO_APONTAMENTO_SIMPLIFICADO, orgId, cpfRequset);
            var colaboradores = await _apontamentoRepository.ListarColaboradoresEApontamentosPorGestor(nomeColaborador, codigoGerente, codigoStatus, mesVigencia, anoVigencia, orgId, codProjeto, codColaboradorExternoAprovador, cursor, limite, restricaoDiretorias, considerarApenasAtivos);

            return colaboradores;
        }

        public async Task<List<StatusApontamentoGrupoResult>> ListaStatusApontamentoGerenteProjeto(int orgId)
        {
            try
            {
                List<StatusApontamentoGrupoResult> ret = await _apontamentoRepository.ListaStatusApontamentoGerenteProjeto(orgId, CultureUtil.GetCurrentCulture());

                return ret;
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<GestoresApontamentoResult>> ListarGestoresApontamento(int orgId)
        {
            try
            {
                var ret = new List<GestoresApontamentoResult>();

                var result = await _apontamentoRepository.ListarGestoresApontamento(orgId);

                foreach (var gestor in result)
                {
                    ret.Add(gestor);
                }

                return ret;
            }
            catch
            {
                throw;
            }
        }

        private MensagemAprovarProjetoVigenciaResult BuildMensagemAprovarProjetoVigencia(string cpf, string codigoProjeto, bool aprovado, string mensagem)
        {
            return new MensagemAprovarProjetoVigenciaResult()
            {
                CodProjeto = codigoProjeto,
                Cpf = cpf,
                ProjetoAprovado = aprovado,
                Mensagem = mensagem
            };
        }

        public async Task<ApiGenericResult<AprovarProjetoVigenciaEmLoteResult>> AprovarProjetoVigenciaEmLote(List<AprovarProjetoVigenciaProjetoDTO> projetos, int mes, int ano, string justificativa, string cpfRequest, int orgId, DateTime dataColetaDeDados)
        {
            var ret = new ApiGenericResult<AprovarProjetoVigenciaEmLoteResult>();
            var aprovarResult = new AprovarProjetoVigenciaEmLoteResult();
            List<string> erros = new List<string>();
            List<MensagemAprovarProjetoVigenciaResult> mensagens = new List<MensagemAprovarProjetoVigenciaResult>();
            int qtdProjetosAprovados = 0;
            int qtdProjetosPulados = 0;
            if (projetos.Any(x => String.IsNullOrEmpty(x.CodProjeto)))
            {
                //erros.Add("O código do projeto é obrigatório.");
                erros.Add(_stringLocalizer.GetStringOuVazio("REQUIRED_PROJECT_CODE"));
            }
            if (projetos.Any(x => String.IsNullOrEmpty(x.Cpf)))
            {
                //erros.Add("O CPF é obrigatório.");
                erros.Add(_stringLocalizer.GetStringOuVazio("REQUIRED_CPF"));
            }
            if (erros.Any())
            {
                ret.Sucesso = false;
                ret.Erros = erros;
                return ret;
            }
            foreach (var projeto in projetos)
            {
                var ap = await _apontamentoRepository.GetApontamentosColaborador(projeto.Cpf, orgId, projeto.CodProjeto, mes: mes, ano: ano);
                if (!ap.Any())
                {
                    //var errorNotTimesheet = $"Não há apontamentos.";
                    var errorNotTimesheet = _stringLocalizer.GetStringOuVazio("NOT_TIMESHEET");
                    mensagens.Add(BuildMensagemAprovarProjetoVigencia(projeto.Cpf, projeto.CodProjeto, false, errorNotTimesheet));
                    qtdProjetosPulados++;
                    continue;
                }
                if(ap.Any(x => x.DataAlteracao > dataColetaDeDados))
                {
                    var errorNotTimesheet = _stringLocalizer.GetStringOuVazio("MODIFIED_DURING_CHANGES");
                    mensagens.Add(BuildMensagemAprovarProjetoVigencia(projeto.Cpf, projeto.CodProjeto, false, errorNotTimesheet));
                    qtdProjetosPulados++;
                    continue;
                }
                if (!ap.Any(x => x.StatusGrupoCodigo == (int)EnumStatusApontamentoGrupo.Pendente))
                {
                    //var alreadyApprovedOrDisapprove = $"Não há aprovações pendentes neste projeto.";
                    var alreadyApprovedOrDisapprove = _stringLocalizer.GetStringOuVazio("NO_PENDING_APPROVALS");
                    mensagens.Add(BuildMensagemAprovarProjetoVigencia(projeto.Cpf, projeto.CodProjeto, false, alreadyApprovedOrDisapprove));
                    qtdProjetosPulados++;
                    continue;
                }

                var aprovarDTO = new AprovarHorasGestorProjetoEmLoteDTO()
                {
                    Ids =  ap
                        .Where(x => 
                            x.StatusGrupoCodigo == (int)EnumStatusApontamentoGrupo.Pendente &&
                            (x.DataAlteracao == null || x.DataAlteracao < dataColetaDeDados)
                        )
                        .Select(x => x.Id)
                        .ToList(),
                    Justificativa = justificativa,
                };
                try
                {
                    await this.AprovarHorasGestorProjetoEmLote(aprovarDTO, cpfRequest, orgId, null);

                    //var approved = "Aprovado";
                    var approved = _stringLocalizer.GetStringOuVazio("STATUS_APPROVED");

                    mensagens.Add(BuildMensagemAprovarProjetoVigencia(projeto.Cpf, projeto.CodProjeto, true, approved));
                    qtdProjetosAprovados++;
                }
                catch (ArgumentException ae)
                {
                    mensagens.Add(BuildMensagemAprovarProjetoVigencia(projeto.Cpf, projeto.CodProjeto, false, ae.Message));
                    qtdProjetosPulados++;
                }
                catch (Exception e)
                {
                    //var errorWhenApprovedProjectAndCollaborator = $"Erro ao aprovar o projeto com código {projeto.CodProjeto} e colaborador {projeto.Cpf}: {e.Message}";
                    var errorWhenApprovedProjectAndCollaborator = _stringLocalizer.GetStringOuVazio("ERROR_WHEN_APPROVED_PROJECT_AND_COLLABORATOR");
                    errorWhenApprovedProjectAndCollaborator = errorWhenApprovedProjectAndCollaborator.Replace("{{COD_PROJETO}}", projeto.CodProjeto);
                    errorWhenApprovedProjectAndCollaborator = errorWhenApprovedProjectAndCollaborator.Replace("{{CPF}}", projeto.Cpf);
                    errorWhenApprovedProjectAndCollaborator = errorWhenApprovedProjectAndCollaborator.Replace("{{MESSAGE}}", e.Message);

                    erros.Add(errorWhenApprovedProjectAndCollaborator);
                    qtdProjetosPulados++;
                }
            }
            ;
            aprovarResult.Mensagens = mensagens;
            aprovarResult.QuantidadeProjetosAprovados = qtdProjetosAprovados;
            aprovarResult.QuantidadeProjetosPulados = qtdProjetosPulados;
            ret.Retorno = aprovarResult;
            ret.Erros = erros;
            ret.Sucesso = !(ret.Erros.Any() && ret.Retorno.QuantidadeProjetosAprovados == 0);
            return ret;
        }

        public async Task<ApiGenericResult<FileContentResult>> RelatorioColaboradoresQueNaoApontaram(int orgId, string cpf, int mes, int ano)
        {
            var ret = new ApiGenericResult<FileContentResult>();
            try
            {
                ValidarAcessoSomenteAoRelatorioApontamento(cpf, orgId);

                var relatorio = await _apontamentoRepository.RelatorioColaboradoresQueNaoApontaram(orgId, mes, ano);

                if (!relatorio.Any())
                {
                    //ret.Mensagem = "Não existem colaboradores para gerar o arquivo.";
                    ret.Mensagem = _stringLocalizer.GetStringOuVazio("NO_COLLABORATORS_TO_GENERATE_FILE");
                    return ret;
                }

                var fileBytes = ExcelFileUtil.CreateExcelFile(relatorio);
                var fileName = "Exportacao_Colaboradores_Nao_Apontados_" + mes + "_" + ano + ".xlsx";

                ret.Retorno = new FileContentResult(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                ret.Retorno.FileDownloadName = fileName;
            }
            catch (Exception ex)
            {
                ret.Sucesso = false;
                ret.Mensagem = ex.Message;
            }

            return ret;
        }

        public async Task<ApiGenericResult<string>> EnviaEmailNotificacaoAprovadoresStatusPendentes(string nomeColaborador, string codigoGerente, string codigoStatus, int mesVigencia, int anoVigencia, string codProjeto, string codColaboradorExternoAprovador, int cursor, int limite, UsuarioLogadoDTO usuarioLogado)
        {
            var resultado = new ApiGenericResult<string>();
            
            var apontamentos = await ListarColaboradoresEApontamentosPorGestor(nomeColaborador, codigoGerente, ((int)EnumStatusApontamentoGrupo.Pendente).ToString(), mesVigencia, anoVigencia, usuarioLogado.OrgId, codProjeto, codColaboradorExternoAprovador, cursor, limite, usuarioLogado.Cpf);

            if (apontamentos.Lista == null || apontamentos.Lista.Count() == 0)
            {
                resultado.Sucesso = false;
                resultado.Mensagem = "Não há apontamentos Pendentes.";

                return resultado;
            }

            await EnviaEmailAprovadores(apontamentos, usuarioLogado);

            await EnviaEmailUsuarioLogado(apontamentos, usuarioLogado);

            resultado.Mensagem = "Notificação enviada com sucesso.";

            return resultado;
        }

        public async Task EnviaEmailAprovadores(ListaComTotalizadorHorasResult<ColaboradorEApontamentoResult> apontamentos, UsuarioLogadoDTO usuarioLogado)
        {
            string baseUrl = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.URL_BASE)?.TrimEnd('/');
            string colaboracaoUrlAprovacao = $"{baseUrl}/timesheet/gestor";

            var retTemplateService = _templateRepository.BuscaTemplateEmail(TemplateOrgParametroEnum.NOTIFICACAO_APONTAMENTOS_STATUS_PENDENTE);

            //Buscando todos os aprovadores sem haver duplicações.
            var aprovadoresUnificados = apontamentos.Lista
                .SelectMany(x => x.Aprovadores)
                .GroupBy(a => a.CodigoInternoAprovador)
                .Select(g => g.First())
                .ToList();

            foreach (var aprovador in aprovadoresUnificados)
            {
                //procurando apontamentos que contenha o aprovador na lista
                var apontamentosDoAprovador = apontamentos.Lista
                .Where(x => x.Aprovadores.Any(a => a.CodigoInternoAprovador == aprovador.CodigoInternoAprovador))
                .ToList();

                var templateFinal = retTemplateService.Template;

                if (retTemplateService == null)
                    throw new ApplicationException(_stringLocalizer.GetStringOuVazio("ERROR_SENDING_REJECTION_EMAIL_TEMPLATE_NOT_REGISTERED"));

                var linhasTabela = string.Empty;

                foreach (var apontamentoPendente in apontamentosDoAprovador)
                {
                    var horasProjeto = (double)apontamentoPendente.SomaHoras / 60;
                    var horasFormatadas = DateTimeUtil.ConverterHorasParaHorasMinutosFormatados(horasProjeto);

                    string linha = string.Format("<tr>" +
                             "<td style=\"padding: 12px 24px;\">{0}</td>\r\n" +
                             "<td style=\"padding: 12px 24px;\">{1}</td>\r\n" +
                             "</tr>", apontamentoPendente.Projeto, horasFormatadas);

                    linhasTabela += linha;
                }

                var colaborador = _usuarioColaboradorRepository.BuscaColaboradorPorCodigo(aprovador.CodigoInternoAprovador, usuarioLogado.OrgId);
                templateFinal = templateFinal.Replace("${ITENS_DA_TABELA}", linhasTabela);
                templateFinal = templateFinal.Replace("${COLABORACAO_URL_APROVACAO}", colaboracaoUrlAprovacao);
                templateFinal = templateFinal.Replace("${NOME}", colaborador.NomeColaborador);
                await _templateRepository.RegistraTemplateEmailAsync(usuarioLogado.OrgId, colaborador.Email, _stringLocalizer.GetStringOuVazio("SUBJECT_TIMESHEET_HOURS_PEDING"), templateFinal);
            }
        }

        public async Task EnviaEmailUsuarioLogado(ListaComTotalizadorHorasResult<ColaboradorEApontamentoResult> apontamentos, UsuarioLogadoDTO usuarioLogado)
        {
            var templateGestor = _templateRepository.BuscaTemplateEmail(TemplateOrgParametroEnum.NOTIFICA_GESTOR_APONTAMENTOS_STATUS_PENDENTE);

            if (templateGestor == null)
                throw new ApplicationException(_stringLocalizer.GetStringOuVazio("ERROR_SENDING_REJECTION_EMAIL_TEMPLATE_NOT_REGISTERED"));

            string linhasProjetos = string.Empty;

            foreach (var apontamentoPendente in apontamentos.Lista)
            {
                var emailDosAprovadores = string.Empty;
                foreach (var aprovador in apontamentoPendente.Aprovadores)
                {
                    var colaborador = _usuarioColaboradorRepository.BuscaColaboradorPorCodigo(aprovador.CodigoInternoAprovador, usuarioLogado.OrgId);

                    var emailDoAprovador = emailDosAprovadores != string.Empty
                        ? $"<br>{colaborador.Email}"
                        : colaborador.Email;

                    emailDosAprovadores += emailDoAprovador;
                }

                var nomeProjeto = apontamentoPendente.Projeto;
                var horasProjeto = (double)apontamentoPendente.SomaHoras / 60;
                var horasFormatadas = DateTimeUtil.ConverterHorasParaHorasMinutosFormatados(horasProjeto);

                string linha = string.Format("<tr>" +
                            "<td style=\"padding: 12px 24px;\">{0}</td>\r\n" +
                            "<td style=\"padding: 12px 24px;\">{1}</td>\r\n" +
                            "<td style=\"padding: 12px 24px;\">{2}</td>\r\n" +
                            "</tr>", emailDosAprovadores, nomeProjeto, horasFormatadas);

                linhasProjetos += linha;
            }

            var usuario = _usuarioColaboradorRepository.GetUserByCPFEOrgId(usuarioLogado.Cpf, usuarioLogado.OrgId);

            var templateFinal = templateGestor.Template
                                                  .Replace("${ITENS_DA_TABELA}", linhasProjetos)
                                                  .Replace("${NOME}", usuario.NomeColaborador);

            await _templateRepository.RegistraTemplateEmailAsync(usuarioLogado.OrgId, usuarioLogado.Email, _stringLocalizer.GetStringOuVazio("SUBJECT_TIMESHEET_HOURS_PEDING"), templateFinal);
        }
    }
}