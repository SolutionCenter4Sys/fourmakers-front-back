using Colaboracao.Core.Interfaces;
using Core.Domain.Colaborador;
using Core.Domain.GestaoPessoa.Pdi;
using Core.Domain.Usuario.Permissao;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.GestaoPessoa.Pdi;
using DataTransferObject.Domain.Usuario;
using GestaoPessoa.Domain.Interfaces.Services.Pdi;
using Logs.Infra.Attributes;
using SRS.Infra.Constantes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoPessoa.Domain.Impl.Services.Pdi
{
    [LogDomainClass]
    public class PdisRhService : IPdisRhService
    {
        private const int TamanhoPaginaMaximo = 100;

        private readonly IPdiRepository _pdiRepository;
        private readonly IFuncionalidadeSistemaRepository _funcionalidadeSistemaRepository;
        private readonly IRestricaoDeAcessoService _restricaoDeAcessoService;
        private readonly IBuscaColaboradorRepository _buscaColaboradorRepository;

        public PdisRhService(
            IPdiRepository pdiRepository,
            IFuncionalidadeSistemaRepository funcionalidadeSistemaRepository,
            IRestricaoDeAcessoService restricaoDeAcessoService,
            IBuscaColaboradorRepository buscaColaboradorRepository)
        {
            _pdiRepository = pdiRepository;
            _funcionalidadeSistemaRepository = funcionalidadeSistemaRepository;
            _restricaoDeAcessoService = restricaoDeAcessoService;
            _buscaColaboradorRepository = buscaColaboradorRepository;
        }

        public async Task<ApiGenericResult<PdiListagemRhResult>> ListarPdisRhAsync(
            string codigoInternoUsuario,
            int orgIdUsuarioLogado,
            int pagina,
            int tamanhoPagina,
            string filtroColaboradorId,
            string filtroGestorCodigoInterno)
        {
            var result = new ApiGenericResult<PdiListagemRhResult>();
            try
            {
                var (ok, msg, codigosEscopo) = await ResolverEscopoColaboradoresRhAsync(
                    codigoInternoUsuario, orgIdUsuarioLogado, filtroColaboradorId, filtroGestorCodigoInterno);
                if (!ok)
                {
                    result.Sucesso = false;
                    result.Mensagem = msg;
                    return result;
                }

                if (codigosEscopo != null && codigosEscopo.Count == 0)
                {
                    var tNorm = Math.Max(1, Math.Min(TamanhoPaginaMaximo, tamanhoPagina));
                    result.Retorno = new PdiListagemRhResult
                    {
                        Items = Array.Empty<PdiResumoRhDTO>(),
                        TotalCount = 0,
                        Pagina = Math.Max(1, pagina),
                        TamanhoPagina = tNorm,
                        TotalPaginas = 0
                    };
                    result.Sucesso = true;
                    return result;
                }

                IReadOnlyList<string> codigosFiltro = codigosEscopo;

                var paginaNorm = Math.Max(1, pagina);
                var tamanhoNorm = Math.Max(1, Math.Min(TamanhoPaginaMaximo, tamanhoPagina));

                var (items, totalCount) = await _pdiRepository.ListarPdisRhPorOrgAsync(
                    orgIdUsuarioLogado,
                    codigosFiltro,
                    paginaNorm,
                    tamanhoNorm);

                var totalPaginas = totalCount <= 0 ? 0 : (int)Math.Ceiling((double)totalCount / tamanhoNorm);
                result.Retorno = new PdiListagemRhResult
                {
                    Items = items?.ToList() ?? new List<PdiResumoRhDTO>(),
                    TotalCount = totalCount,
                    Pagina = paginaNorm,
                    TamanhoPagina = tamanhoNorm,
                    TotalPaginas = totalPaginas
                };
                result.Sucesso = true;
            }
            catch (Exception)
            {
                result.Sucesso = false;
                result.Mensagem = "Erro ao listar PDIs da organização (RH).";
            }

            return result;
        }

        public async Task<ApiGenericResult<PdiRhBigNumbersResultDTO>> ObterBigNumbersAsync(
            string codigoInternoUsuario,
            int orgIdUsuarioLogado,
            string filtroColaboradorId,
            string filtroGestorCodigoInterno)
        {
            var result = new ApiGenericResult<PdiRhBigNumbersResultDTO>();
            try
            {
                var (ok, msg, codigosEscopo) = await ResolverEscopoColaboradoresRhAsync(
                    codigoInternoUsuario, orgIdUsuarioLogado, filtroColaboradorId, filtroGestorCodigoInterno);
                if (!ok)
                {
                    result.Sucesso = false;
                    result.Mensagem = msg;
                    return result;
                }

                if (codigosEscopo != null && codigosEscopo.Count == 0)
                {
                    result.Retorno = new PdiRhBigNumbersResultDTO();
                    result.Sucesso = true;
                    return result;
                }

                var contagens = await _pdiRepository.ObterContagensPdisRhAsync(orgIdUsuarioLogado, codigosEscopo);
                result.Retorno = new PdiRhBigNumbersResultDTO
                {
                    NaoIniciado = contagens.NaoIniciado,
                    EmAnalise = contagens.EmAnalise,
                    EmAndamento = contagens.EmAndamento,
                    Finalizados = contagens.Finalizados,
                    Cancelados = contagens.Cancelados
                };
                result.Sucesso = true;
            }
            catch (Exception)
            {
                result.Sucesso = false;
                result.Mensagem = "Erro ao obter contagens de PDIs (RH).";
            }

            return result;
        }

        /// <summary>
        /// <c>null</c> = sem filtro IN (toda a org). Lista vazia = escopo sem colaboradores. Erro = (false, mensagem, null).
        /// </summary>
        private async Task<(bool Ok, string Mensagem, List<string> CodigosOuNull)> ResolverEscopoColaboradoresRhAsync(
            string codigoInternoUsuario,
            int orgIdUsuarioLogado,
            string filtroColaboradorId,
            string filtroGestorCodigoInterno)
        {
            if (string.IsNullOrWhiteSpace(codigoInternoUsuario))
                return (false, "Usuário não identificado.", null);

            var usuario = codigoInternoUsuario.Trim();

            var temRh = await _funcionalidadeSistemaRepository.ValidaAcessoFuncionalidade(
                usuario,
                orgIdUsuarioLogado,
                FuncionalidadeSistemaEnum.GESTAO_DESEMPENHO_RH);
            var temMinhaEquipe = await _funcionalidadeSistemaRepository.ValidaAcessoFuncionalidade(
                usuario,
                orgIdUsuarioLogado,
                FuncionalidadeSistemaEnum.MINHA_EQUIPE);
            if (!temRh && !temMinhaEquipe)
                return (false, "Acesso negado: é necessária a funcionalidade GESTAO_DESEMPENHO_RH ou MINHA_EQUIPE para a organização do usuário logado.", null);

            var restricaoDiretorias = await _restricaoDeAcessoService.ListarMinhasRestricoesDeAcessoPorCodigoInternoETipo(
                usuario,
                orgIdUsuarioLogado,
                RestricaoDeAcessoTipoConstants.DIRETORIA);

            List<string> codigosEscopo = null;
            if (temRh)
            {
                if (restricaoDiretorias != null && restricaoDiretorias.Count > 0)
                {
                    var colaboradores = _buscaColaboradorRepository.ListaColaboradoresOrg(
                        orgIdUsuarioLogado, 0, 20_000, false, "", "", restricaoDiretorias);
                    codigosEscopo = colaboradores
                        .Select(c => c.Cpf)
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToList();
                }
            }
            else
            {
                var subordinados = _buscaColaboradorRepository.ListarCodigosInternosHierarquiaTimeIncluindoGestor(usuario, orgIdUsuarioLogado);
                subordinados.Remove(usuario);
                codigosEscopo = subordinados.ToList();
                if (restricaoDiretorias != null && restricaoDiretorias.Count > 0)
                {
                    var colaboradores = _buscaColaboradorRepository.ListaColaboradoresOrg(
                        orgIdUsuarioLogado, 0, 20_000, false, "", "", restricaoDiretorias);
                    var permitidosDir = new HashSet<string>(
                        colaboradores.Select(c => c.Cpf).Where(x => !string.IsNullOrWhiteSpace(x)),
                        StringComparer.OrdinalIgnoreCase);
                    codigosEscopo = codigosEscopo.Where(c => permitidosDir.Contains(c)).ToList();
                }
            }

            if (!string.IsNullOrWhiteSpace(filtroGestorCodigoInterno))
            {
                var gestor = filtroGestorCodigoInterno.Trim();
                var subordinados = _buscaColaboradorRepository.ListarCodigosInternosHierarquiaTimeIncluindoGestor(gestor, orgIdUsuarioLogado);
                subordinados.Remove(gestor);

                if (codigosEscopo == null)
                    codigosEscopo = subordinados.ToList();
                else
                    codigosEscopo = codigosEscopo.Where(c => subordinados.Contains(c)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(filtroColaboradorId))
            {
                var c = filtroColaboradorId.Trim();
                if (codigosEscopo != null)
                {
                    var permitido = codigosEscopo.Any(x => string.Equals(x, c, StringComparison.OrdinalIgnoreCase));
                    if (!permitido)
                        return (false, "Colaborador informado está fora do seu escopo de acesso (diretoria ou gestor).", null);
                }
                codigosEscopo = new List<string> { c };
            }

            return (true, null, codigosEscopo);
        }
    }
}
