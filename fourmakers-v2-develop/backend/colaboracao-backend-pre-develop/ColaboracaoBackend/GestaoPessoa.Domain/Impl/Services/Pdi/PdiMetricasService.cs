using Colaboracao.Core.Interfaces;
using Core.Domain.Colaborador;
using Core.Domain.GestaoPessoa.Pdi;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.GestaoPessoa.Pdi;
using GestaoPessoa.Domain.Interfaces.Services.Pdi;
using Logs.Infra.Attributes;
using SRS.Infra.Constantes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoPessoa.Domain.Impl.Services.Pdi
{
    [LogDomainClass]
    public class PdiMetricasService : IPdiMetricasService
    {
        private static readonly string[] StatusAtivos = { "IN_ANALYSIS", "IN_PROGRESS", "NOT_STARTED" };
        private static readonly string[] StatusHistoricos = { "COMPLETED", "CANCELLED" };
        private const int PaginaPadrao = 1;
        private const int TamanhoPaginaPadrao = 25;
        private const int TamanhoPaginaMaximo = 200;

        private readonly IPdiRepository _pdiRepository;
        private readonly IBuscaColaboradorRepository _buscaColaboradorRepository;
        private readonly IRestricaoDeAcessoService _restricaoDeAcessoService;

        public PdiMetricasService(
            IPdiRepository pdiRepository,
            IBuscaColaboradorRepository buscaColaboradorRepository,
            IRestricaoDeAcessoService restricaoDeAcessoService)
        {
            _pdiRepository = pdiRepository;
            _buscaColaboradorRepository = buscaColaboradorRepository;
            _restricaoDeAcessoService = restricaoDeAcessoService;
        }

        public async Task<ApiGenericResult<PdiMetricasResultDTO>> ObterMetricasPorOrgAsync(string cpfUsuario, int orgId, PdiMetricasFiltroRequestDTO filtro = null)
        {
            var result = new ApiGenericResult<PdiMetricasResultDTO>();
            try
            {
                filtro ??= new PdiMetricasFiltroRequestDTO();
                var (diretorias, erroDir) = await ResolverDiretoriasEfetivasAsync(cpfUsuario, orgId, filtro);
                if (erroDir != null)
                {
                    result.Sucesso = false;
                    result.Mensagem = erroDir;
                    return result;
                }
                var cpfsFiltro = ResolverCodigosColaboradorFiltro(orgId, diretorias, filtro);
                if (cpfsFiltro != null && cpfsFiltro.Count == 0)
                {
                    result.Retorno = CriarResultadoVazio();
                    result.Sucesso = true;
                    return result;
                }

                var statusesListagem = ParseStatuses(filtro.Statuses);
                var queryBase = new PdiMetricasQueryDTO
                {
                    OrgId = orgId,
                    CodDiretorias = diretorias,
                    CodigosColaborador = cpfsFiltro,
                    DataCriacaoMin = filtro.DataInicio,
                    DataDeadlineMax = filtro.DataConclusao,
                    StatusesListagem = null
                };

                var queryLista = new PdiMetricasQueryDTO
                {
                    OrgId = orgId,
                    CodDiretorias = diretorias,
                    CodigosColaborador = cpfsFiltro,
                    DataCriacaoMin = filtro.DataInicio,
                    DataDeadlineMax = filtro.DataConclusao,
                    StatusesListagem = statusesListagem.Count > 0 ? statusesListagem : null
                };

                var bigNumbers = await _pdiRepository.ObterContagensMetricasFiltradasAsync(queryBase);
                var (pagina, tamPag) = NormalizarPaginacao(filtro.Pagina, filtro.TamanhoPagina);
                var totalLista = await _pdiRepository.ContarPdisMetricasFiltradasAsync(queryLista);
                queryLista.Skip = (pagina - 1) * tamPag;
                queryLista.Take = tamPag;
                var itens = (await _pdiRepository.ListarPdisMetricasFiltradasAsync(queryLista)).ToList();
                var metaLista = new PdiMetricasListaPaginacaoDTO
                {
                    Pagina = pagina,
                    TamanhoPagina = tamPag,
                    TotalItens = totalLista,
                    TotalPaginas = totalLista == 0 ? 0 : (int)Math.Ceiling(totalLista / (double)tamPag)
                };
                result.Retorno = MontarResultadoComItens(bigNumbers, itens, metaLista);
                result.Sucesso = true;
            }
            catch (Exception)
            {
                result.Sucesso = false;
                result.Mensagem = "Erro ao obter métricas de PDI.";
            }
            return result;
        }

        public async Task<ApiGenericResult<PdiMetricasListagemPaginadaDTO<PdiGestorItemDTO>>> ListarGestoresOrgAsync(
            string cpfUsuario, int orgId, int? pagina, int? tamanhoPagina)
        {
            var result = new ApiGenericResult<PdiMetricasListagemPaginadaDTO<PdiGestorItemDTO>>();
            try
            {
                var (diretorias, erroDir) = await ResolverDiretoriasEfetivasAsync(cpfUsuario, orgId, new PdiMetricasFiltroRequestDTO());
                if (erroDir != null)
                {
                    result.Sucesso = false;
                    result.Mensagem = erroDir;
                    return result;
                }
                var (pag, tam) = NormalizarPaginacao(pagina, tamanhoPagina);
                var skip = (pag - 1) * tam;
                var (itens, total) = await _pdiRepository.ListarGestoresPorDiretoriasPaginadoAsync(orgId, diretorias, skip, tam);
                result.Retorno = MontarListagemPaginada(itens.ToList(), pag, tam, total);
                result.Sucesso = true;
            }
            catch (Exception)
            {
                result.Sucesso = false;
                result.Mensagem = "Erro ao listar gestores.";
            }
            return result;
        }

        public async Task<ApiGenericResult<PdiMetricasListagemPaginadaDTO<PdiUnidadeItemDTO>>> ListarUnidadesMetricasAsync(
            string cpfUsuario, int orgId, int? pagina, int? tamanhoPagina)
        {
            var result = new ApiGenericResult<PdiMetricasListagemPaginadaDTO<PdiUnidadeItemDTO>>();
            try
            {
                List<PdiUnidadeItemDTO> lista;
                var restricao = await _restricaoDeAcessoService.ListarMinhasRestricoesDeAcessoPorCodigoInternoETipo(
                    cpfUsuario, orgId, RestricaoDeAcessoTipoConstants.DIRETORIA);
                if (restricao != null && restricao.Count > 0)
                {
                    var todas = await _buscaColaboradorRepository.ListarDiretoriasDistintasPorOrgAsync(orgId);
                    var map = todas
                        .GroupBy(x => x.Cod ?? "", StringComparer.OrdinalIgnoreCase)
                        .ToDictionary(g => g.Key, g => g.First().Diretoria ?? g.Key, StringComparer.OrdinalIgnoreCase);
                    lista = restricao
                        .Select(cod => new PdiUnidadeItemDTO
                        {
                            CodDiretoria = cod,
                            Descricao = map.TryGetValue(cod, out var d) ? d : cod
                        })
                        .OrderBy(x => x.Descricao)
                        .ToList();
                }
                else
                {
                    var todas = await _buscaColaboradorRepository.ListarDiretoriasDistintasPorOrgAsync(orgId);
                    lista = todas
                        .Select(x => new PdiUnidadeItemDTO { CodDiretoria = x.Cod, Descricao = x.Diretoria ?? x.Cod })
                        .OrderBy(x => x.Descricao)
                        .ToList();
                }

                var (pag, tam) = NormalizarPaginacao(pagina, tamanhoPagina);
                var total = lista.Count;
                var fatia = lista.Skip((pag - 1) * tam).Take(tam).ToList();
                result.Retorno = MontarListagemPaginada(fatia, pag, tam, total);
                result.Sucesso = true;
            }
            catch (Exception)
            {
                result.Sucesso = false;
                result.Mensagem = "Erro ao listar unidades.";
            }
            return result;
        }

        public async Task<ApiGenericResult<PdiMetricasListagemPaginadaDTO<PdiColaboradorMetricaItemDTO>>> ListarColaboradoresMetricasAsync(
            string cpfUsuario, int orgId, string? cpfGestor, string? codDiretoria, int? pagina, int? tamanhoPagina)
        {
            var result = new ApiGenericResult<PdiMetricasListagemPaginadaDTO<PdiColaboradorMetricaItemDTO>>();
            try
            {
                var filtro = new PdiMetricasFiltroRequestDTO { CodDiretoria = codDiretoria, CpfGestor = cpfGestor };
                var (diretorias, erroDir) = await ResolverDiretoriasEfetivasAsync(cpfUsuario, orgId, filtro);
                if (erroDir != null)
                {
                    result.Sucesso = false;
                    result.Mensagem = erroDir;
                    return result;
                }
                var dirsParam = diretorias == null ? null : diretorias.ToList();
                var colaboradores = _buscaColaboradorRepository.ListaColaboradoresOrg(orgId, 0, 20_000, false, "", "", dirsParam);
                IEnumerable<string> cpfsPermitidos = colaboradores.Select(c => c.Cpf).Where(x => !string.IsNullOrEmpty(x)).Distinct(StringComparer.OrdinalIgnoreCase);
                if (!string.IsNullOrWhiteSpace(cpfGestor))
                {
                    var time = ObterCodigosPermitidos(cpfGestor.Trim(), orgId);
                    cpfsPermitidos = cpfsPermitidos.Where(c => time.Contains(c));
                }
                var set = new HashSet<string>(cpfsPermitidos, StringComparer.OrdinalIgnoreCase);
                var lista = colaboradores
                    .Where(c => set.Contains(c.Cpf))
                    .Select(c => new PdiColaboradorMetricaItemDTO { Cpf = c.Cpf, NomeCompleto = c.Nome })
                    .OrderBy(x => x.NomeCompleto)
                    .ToList();
                var (pag, tam) = NormalizarPaginacao(pagina, tamanhoPagina);
                var total = lista.Count;
                var fatia = lista.Skip((pag - 1) * tam).Take(tam).ToList();
                result.Retorno = MontarListagemPaginada(fatia, pag, tam, total);
                result.Sucesso = true;
            }
            catch (Exception)
            {
                result.Sucesso = false;
                result.Mensagem = "Erro ao listar colaboradores.";
            }
            return result;
        }

        public async Task<ApiGenericResult<PdiMetricasCsvExportResultDTO>> ExportarMetricasCsvAsync(
            string cpfUsuario, int orgId, PdiMetricasExportRequestDTO request)
        {
            var result = new ApiGenericResult<PdiMetricasCsvExportResultDTO>();
            try
            {
                if (request?.PdiIds == null || request.PdiIds.Count == 0)
                {
                    result.Sucesso = false;
                    result.Mensagem = "Informe ao menos um PDI para exportar.";
                    return result;
                }
                var filtro = request.Filtro ?? new PdiMetricasFiltroRequestDTO();
                var (diretorias, erroDir) = await ResolverDiretoriasEfetivasAsync(cpfUsuario, orgId, filtro);
                if (erroDir != null)
                {
                    result.Sucesso = false;
                    result.Mensagem = erroDir;
                    return result;
                }
                var cpfsFiltro = ResolverCodigosColaboradorFiltro(orgId, diretorias, filtro);
                if (cpfsFiltro != null && cpfsFiltro.Count == 0)
                {
                    result.Sucesso = false;
                    result.Mensagem = "Nenhum colaborador no escopo do filtro.";
                    return result;
                }
                var escopo = new PdiMetricasQueryDTO
                {
                    OrgId = orgId,
                    CodDiretorias = diretorias,
                    CodigosColaborador = cpfsFiltro,
                    DataCriacaoMin = filtro.DataInicio,
                    DataDeadlineMax = filtro.DataConclusao,
                    StatusesListagem = null
                };
                var linhas = (await _pdiRepository.ListarPdisMetricasExportPorIdsAsync(request.PdiIds, escopo)).ToList();
                var idsPedidos = request.PdiIds.Distinct().ToList();
                if (linhas.Count != idsPedidos.Count)
                {
                    result.Sucesso = false;
                    result.Mensagem = "Um ou mais PDIs não pertencem ao seu escopo de acesso ou aos filtros informados.";
                    return result;
                }
                var csv = MontarCsv(linhas);
                result.Retorno = new PdiMetricasCsvExportResultDTO
                {
                    ConteudoCsv = csv,
                    NomeArquivo = $"metricas_pdi_{DateTime.UtcNow:yyyyMMddHHmmss}.csv"
                };
                result.Sucesso = true;
            }
            catch (Exception)
            {
                result.Sucesso = false;
                result.Mensagem = "Erro ao exportar métricas.";
            }
            return result;
        }

        private static string MontarCsv(List<PdiMetricaExportLinhaDTO> linhas)
        {
            var sb = new StringBuilder();
            sb.AppendLine("PdiId;Titulo;ColaboradorId;Status;DataCriacao;DeadLine");
            foreach (var l in linhas)
            {
                sb.Append(EscaparCsv(l.PdiId.ToString())).Append(';');
                sb.Append(EscaparCsv(l.Titulo)).Append(';');
                sb.Append(EscaparCsv(l.ColaboradorId)).Append(';');
                sb.Append(EscaparCsv(l.Status)).Append(';');
                sb.Append(EscaparCsv(l.DataCriacao?.ToString("o", CultureInfo.InvariantCulture))).Append(';');
                sb.Append(EscaparCsv(l.DeadLine?.ToString("o", CultureInfo.InvariantCulture)));
                sb.AppendLine();
            }
            return sb.ToString();
        }

        private static string EscaparCsv(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            if (s.Contains(';') || s.Contains('"') || s.Contains('\n'))
                return "\"" + s.Replace("\"", "\"\"") + "\"";
            return s;
        }

        public async Task<ApiGenericResult<PdiMetricasResultDTO>> ObterMetricasColaboradorAsync(string cpf, int orgId)
        {
            var result = new ApiGenericResult<PdiMetricasResultDTO>();
            try
            {
                var codigos = string.IsNullOrEmpty(cpf) ? new List<string>() : new List<string> { cpf };
                if (codigos.Count == 0)
                {
                    result.Retorno = CriarResultadoVazio();
                    result.Sucesso = true;
                    return result;
                }
                result.Retorno = await MontarMetricasAsync(codigos, orgId);
                result.Sucesso = true;
            }
            catch (Exception)
            {
                result.Sucesso = false;
                result.Mensagem = "Erro ao obter métricas de PDI.";
            }
            return result;
        }

        public async Task<ApiGenericResult<PdiMetricasResultDTO>> ObterMetricasGestorAsync(string cpfGestor, int orgId)
        {
            var result = new ApiGenericResult<PdiMetricasResultDTO>();
            try
            {
                var codigos = ObterCodigosPermitidos(cpfGestor, orgId).ToList();
                if (codigos.Count == 0)
                {
                    result.Retorno = CriarResultadoVazio();
                    result.Sucesso = true;
                    return result;
                }
                result.Retorno = await MontarMetricasAsync(codigos, orgId);
                result.Sucesso = true;
            }
            catch (Exception)
            {
                result.Sucesso = false;
                result.Mensagem = "Erro ao obter métricas do time.";
            }
            return result;
        }

        public async Task<ApiGenericResult<PdiMetricasResultDTO>> ObterMetricasGestorPorColaboradorAsync(string colaboradorId, string cpfGestor, int orgId)
        {
            var result = new ApiGenericResult<PdiMetricasResultDTO>();
            try
            {
                var codigosPermitidos = ObterCodigosPermitidos(cpfGestor, orgId);
                if (!codigosPermitidos.Contains(colaboradorId ?? string.Empty))
                {
                    result.Sucesso = false;
                    result.Mensagem = "Colaborador não pertence ao seu time.";
                    return result;
                }
                var codigos = new List<string> { colaboradorId };
                result.Retorno = await MontarMetricasAsync(codigos, orgId);
                result.Sucesso = true;
            }
            catch (Exception)
            {
                result.Sucesso = false;
                result.Mensagem = "Erro ao obter métricas do colaborador.";
            }
            return result;
        }

        private async Task<(IReadOnlyList<string> Diretorias, string Erro)> ResolverDiretoriasEfetivasAsync(string cpfUsuario, int orgId, PdiMetricasFiltroRequestDTO filtro)
        {
            var restricao = await _restricaoDeAcessoService.ListarMinhasRestricoesDeAcessoPorCodigoInternoETipo(
                cpfUsuario, orgId, RestricaoDeAcessoTipoConstants.DIRETORIA);
            if (!string.IsNullOrWhiteSpace(filtro?.CodDiretoria))
            {
                var cod = filtro.CodDiretoria.Trim();
                if (restricao != null && restricao.Count > 0 && !restricao.Any(r => string.Equals(r, cod, StringComparison.OrdinalIgnoreCase)))
                    return (null, "Unidade não permitida para o seu usuário.");
                return (new List<string> { cod }, null);
            }
            if (restricao != null && restricao.Count > 0)
                return (restricao.ToList(), null);
            return (null, null);
        }

        private List<string> ResolverCodigosColaboradorFiltro(int orgId, IReadOnlyList<string> diretorias, PdiMetricasFiltroRequestDTO filtro)
        {
            filtro ??= new PdiMetricasFiltroRequestDTO();
            var dirsParam = diretorias == null ? null : diretorias.ToList();
            var temGestor = !string.IsNullOrWhiteSpace(filtro.CpfGestor);
            var temColaborador = !string.IsNullOrWhiteSpace(filtro.CpfColaborador);
            if (!temGestor && !temColaborador)
                return null;

            var colaboradores = _buscaColaboradorRepository.ListaColaboradoresOrg(orgId, 0, 20_000, false, "", "", dirsParam);
            var noEscopo = new HashSet<string>(
                colaboradores.Select(c => c.Cpf).Where(x => !string.IsNullOrEmpty(x)),
                StringComparer.OrdinalIgnoreCase);

            if (temGestor && temColaborador)
            {
                var time = ObterCodigosPermitidos(filtro.CpfGestor.Trim(), orgId);
                var c = filtro.CpfColaborador.Trim();
                if (!time.Contains(c) || !noEscopo.Contains(c))
                    return new List<string>();
                return new List<string> { c };
            }
            if (temColaborador)
            {
                var c = filtro.CpfColaborador.Trim();
                return noEscopo.Contains(c) ? new List<string> { c } : new List<string>();
            }
            var timeSó = ObterCodigosPermitidos(filtro.CpfGestor.Trim(), orgId);
            var inter = timeSó.Where(noEscopo.Contains).ToList();
            return inter;
        }

        private static IReadOnlyList<string> ParseStatuses(string statuses)
        {
            if (string.IsNullOrWhiteSpace(statuses))
                return Array.Empty<string>();
            return statuses.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(s => s.ToUpperInvariant())
                .Where(s => s.Length > 0)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private static (int Pagina, int Tamanho) NormalizarPaginacao(int? pagina, int? tamanhoPagina)
        {
            var p = pagina is >= 1 ? pagina.Value : PaginaPadrao;
            var rawT = tamanhoPagina is >= 1 ? tamanhoPagina.Value : TamanhoPaginaPadrao;
            var t = Math.Min(rawT, TamanhoPaginaMaximo);
            return (p, t);
        }

        private static PdiMetricasListagemPaginadaDTO<T> MontarListagemPaginada<T>(
            IReadOnlyList<T> itens, int pagina, int tamanho, int total)
        {
            return new PdiMetricasListagemPaginadaDTO<T>
            {
                Itens = itens,
                Paginacao = new PdiMetricasListaPaginacaoDTO
                {
                    Pagina = pagina,
                    TamanhoPagina = tamanho,
                    TotalItens = total,
                    TotalPaginas = total == 0 ? 0 : (int)Math.Ceiling(total / (double)tamanho)
                }
            };
        }

        private static PdiMetricasResultDTO MontarResultadoComItens(
            PdiMetricasBigNumbersDTO bigNumbers, List<PdiMetricaItemDTO> itens, PdiMetricasListaPaginacaoDTO listaPaginacao)
        {
            var ativos = itens.Where(i =>
            {
                var st = (i.Status ?? "").ToUpperInvariant();
                return st is "NOT_STARTED" or "IN_ANALYSIS" or "IN_PROGRESS";
            }).ToList();
            var historicos = itens.Where(i =>
            {
                var st = (i.Status ?? "").ToUpperInvariant();
                return st is "COMPLETED" or "CANCELLED";
            }).ToList();
            return new PdiMetricasResultDTO
            {
                BigNumbers = bigNumbers,
                ListaPaginacao = listaPaginacao,
                Itens = itens,
                Ativos = ativos,
                Historicos = historicos
            };
        }

        private async Task<PdiMetricasResultDTO> MontarMetricasAsync(List<string> codigos, int orgId)
        {
            var bigNumbers = await _pdiRepository.ObterContagensPorStatusAsync(codigos, orgId);
            var ativos = (await _pdiRepository.ListarPdisComPrevisaoPorStatusesAsync(codigos, orgId, StatusAtivos)).ToList();
            var historicos = (await _pdiRepository.ListarPdisComPrevisaoPorStatusesAsync(codigos, orgId, StatusHistoricos)).ToList();
            var itens = ativos.Concat(historicos).ToList();
            return new PdiMetricasResultDTO
            {
                BigNumbers = bigNumbers,
                ListaPaginacao = null,
                Itens = itens,
                Ativos = ativos,
                Historicos = historicos
            };
        }

        private static PdiMetricasResultDTO CriarResultadoVazio()
        {
            return new PdiMetricasResultDTO
            {
                BigNumbers = new PdiMetricasBigNumbersDTO(),
                ListaPaginacao = new PdiMetricasListaPaginacaoDTO
                {
                    Pagina = PaginaPadrao,
                    TamanhoPagina = TamanhoPaginaPadrao,
                    TotalItens = 0,
                    TotalPaginas = 0
                },
                Itens = new List<PdiMetricaItemDTO>(),
                Ativos = new List<PdiMetricaItemDTO>(),
                Historicos = new List<PdiMetricaItemDTO>()
            };
        }

        private HashSet<string> ObterCodigosPermitidos(string cpfGestor, int orgId)
        {
            var codigos = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (!string.IsNullOrEmpty(cpfGestor))
                codigos.Add(cpfGestor);
            var subordinados = _buscaColaboradorRepository.GetSubordinadosColaboradorOrg(cpfGestor, orgId);
            if (subordinados != null)
            {
                foreach (var s in subordinados)
                    if (!string.IsNullOrEmpty(s?.Cpf))
                        codigos.Add(s.Cpf);
            }
            return codigos;
        }
    }
}
