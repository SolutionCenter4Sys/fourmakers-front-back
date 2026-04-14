using ApiClient.Domain;
using ApiClient.Domain.Interfaces;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Colaboracao.Helper.Enum;
using Colaboracao.Helper.Util;
using Core.Domain.Marketing.Comunicacao.Comunidade;
using Core.Domain.Marketing.Comunicacao.Grupo;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Marketing.Comunicacao.Comunidade;
using Marketing.Domain.Interfaces.Comunicacao.Comunidade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Marketing.Domain.Impl.Comunicacao.Comunidade
{
    public class ComunicacaoComunidadeService : IComunicacaoComunidadeService
    {
        private const string DESCRICAO_ENTIDADE = "Marketing - Comunicacao";

        private readonly IComunicacaoComunidadeRepository _repository;
        private readonly IComunicacaoGrupoRepository _grupoRepository;
        private readonly IUploadFilesClient _uploadFilesClient;
        private readonly IDBConnectionUnitOfWork _dbConnectionUnitOfWork;

        public ComunicacaoComunidadeService(
            IComunicacaoComunidadeRepository repository,
            IComunicacaoGrupoRepository grupoRepository,
            IUploadFilesClient uploadFilesClient,
            IDBConnectionUnitOfWork dbConnectionUnitOfWork)
        {
            _repository = repository;
            _grupoRepository = grupoRepository;
            _uploadFilesClient = uploadFilesClient;
            _dbConnectionUnitOfWork = dbConnectionUnitOfWork;
        }

        public async Task<ApiGenericResult<ComunidadesResumoResponseDTO>> ObterListarComunidadesResumoAsync(int orgId, string codigoInternoColaborador)
        {
            var result = new ApiGenericResult<ComunidadesResumoResponseDTO>();
            try
            {
                result.Retorno = await _repository.ObterListaComunidadesResumoAsync(orgId, codigoInternoColaborador);
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, DESCRICAO_ENTIDADE);
            }
            return result;
        }

        public async Task<ApiGenericResult<ComunidadeDetalheDTO>> ObterComunidadePorIdAsync(Guid id, int orgId, string codigoInternoColaborador)
        {
            var result = new ApiGenericResult<ComunidadeDetalheDTO>();
            try
            {
                result.Retorno = await _repository.ObterComunidadePorIdAsync(id, orgId, codigoInternoColaborador);
                if (result.Retorno == null)
                    result.Mensagem = "Comunidade não encontrada.";
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, DESCRICAO_ENTIDADE);
            }
            return result;
        }

        public async Task<ApiGenericResult<ComunidadeResumoDTO>> InserirComunidadeAsync(string codigoInternoColaborador, int orgId, InserirComunidadeRequestDTO request, byte[] capaImagem)
        {
            var result = new ApiGenericResult<ComunidadeResumoDTO>();
            _dbConnectionUnitOfWork.BeginTransaction();
            try
            {
                if (request == null) throw new ArgumentNullException(nameof(request));
                if (string.IsNullOrWhiteSpace(request.Nome)) throw new ArgumentException("Nome da comunidade obrigatório.");

                var possuiPermissao = await _grupoRepository.UsuarioPossuiPermissaoCriarComunidadeAsync(codigoInternoColaborador, orgId);
                if (!possuiPermissao)
                    throw new UnauthorizedAccessException("Você não possui permissão para criar comunidade. É necessário pertencer a um grupo com permissão de criação de comunidade.");

                if (!Enum.TryParse<TipoComunidadeEnum>(request.Tipo, true, out var tipoEnum))
                    throw new ArgumentException("Tipo da comunidade deve ser 'pública' ou 'privada'.");
                request.Tipo = tipoEnum.ToString();

                var politica = ComunidadeEnumExtensions.ParsePolitica(request.PublicacaoConfiguracaoPolitica);
                request.PublicacaoConfiguracaoPolitica = politica.ToDbValue();

                if (tipoEnum != TipoComunidadeEnum.privada)
                {
                    if (request.GruposComunidade?.Count > 0)
                        throw new ArgumentException("Grupos só podem ser associados a comunidades privadas.");
                }

                GarantirCriadorComoModeradorEParticipanteAoInserir(codigoInternoColaborador, request);
                GarantirModeradoresComoParticipantesAoInserir(request);
                ValidarModeradoresEParticipantesInserir(request);

                // Capa é opcional: só faz upload e preenche CapaUrl quando o arquivo for enviado
                if (capaImagem != null && capaImagem.Length > 0)
                {
                    request.CapaUrl = await FazerUploadCapaAsync(capaImagem);
                }

                var comunidadeId = await _repository.InserirComunidadeAsync(codigoInternoColaborador, orgId, request);
                var dataRefCriacao = DateTime.Now;

                result.Retorno = new ComunidadeResumoDTO
                {
                    Id = Guid.Parse(comunidadeId),
                    Nome = request.Nome,
                    Descricao = request.Descricao,
                    CapaUrl = request.CapaUrl,
                    Tipo = request.Tipo,
                    PermitePostagemMembro = request.PermitePostagemMembro,
                    PermiteSair = request.PermiteSair,
                    PublicacaoConfiguracaoPolitica = request.PublicacaoConfiguracaoPolitica,
                    PublicacaoPermiteComentario = request.PublicacaoPermiteComentario,
                    PublicacaoPermiteLikeHabilitado = request.PublicacaoPermiteLikeHabilitado,
                    TotalPublicacoes = 0,
                    TotalMembros = request.CodigoInternoColaboradoresParticipantes?.Count ?? 0,
                    CodigoInternoColaboradorCriacao = codigoInternoColaborador,
                    DataCriacao = dataRefCriacao,
                    CodigoInternoColaboradorUltimaAlteracao = codigoInternoColaborador,
                    DataUltimaAlteracao = dataRefCriacao
                };
                await _repository.EnriquecerColaboradoresResumoComunidadeAsync(result.Retorno, orgId);
                result.Mensagem = "Comunidade criada com sucesso.";

                _dbConnectionUnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                _dbConnectionUnitOfWork.Rollback();
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Create, DESCRICAO_ENTIDADE);
            }
            return result;
        }
        public async Task<ApiGenericResult> AtualizarComunidadeAsync(Guid id, string codigoInternoColaborador, int orgId, AtualizarComunidadeRequestDTO request, byte[] capaImagem)
        {
            var result = new ApiGenericResult();
            _dbConnectionUnitOfWork.BeginTransaction();
            try
            {
                if (request == null) throw new ArgumentNullException(nameof(request));
                if (string.IsNullOrWhiteSpace(request.Nome)) throw new ArgumentException("Nome da comunidade obrigatório.");

                var ehModerador = await _repository.ColaboradorEModeradorDaComunidadeAsync(id, codigoInternoColaborador, orgId);
                if (!ehModerador)
                    throw new ApplicationException("Você não possui permissão para editar esta comunidade. Somente moderadores da comunidade podem editar.");

                if (!Enum.TryParse<TipoComunidadeEnum>(request.Tipo, true, out var tipoEnum))
                    throw new ArgumentException("Tipo da comunidade deve ser 'pública' ou 'privada'.");
                request.Tipo = tipoEnum.ToString();

                var politica = ComunidadeEnumExtensions.ParsePolitica(request.PublicacaoConfiguracaoPolitica);
                request.PublicacaoConfiguracaoPolitica = politica.ToDbValue();

                if (tipoEnum != TipoComunidadeEnum.privada)
                {
                    if (request.GruposComunidade?.Count > 0)
                        throw new ArgumentException("Grupos só podem ser associados a comunidades privadas.");
                }

                if (request.Ativo)
                {
                    if (request.CodigosInternoColaboradoresModeradores != null)
                        await GarantirModeradorNaoCriadorNaoRemoveCriadorModeradorAsync(id, orgId, codigoInternoColaborador, request);

                    await GarantirModeradoresComoParticipantesAoAtualizarAsync(id, orgId, request);
                    ValidarModeradoresEParticipantesAtualizar(request);
                }

                var atualizarCapaUrl = capaImagem != null && capaImagem.Length > 0;
                if (atualizarCapaUrl)
                {
                    request.CapaUrl = await FazerUploadCapaAsync(capaImagem);
                }

                var sucesso = await _repository.AtualizarComunidadeAsync(id, orgId, request, codigoInternoColaborador, DateTime.Now, atualizarCapaUrl);
                if (!sucesso)
                    throw new ApplicationException("Comunidade não encontrada ou não pertence à organização.");

                result.Mensagem = "Comunidade atualizada com sucesso.";
                _dbConnectionUnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                _dbConnectionUnitOfWork.Rollback();
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Update, DESCRICAO_ENTIDADE);
            }
            return result;
        }

        private static HashSet<string> CriarConjuntoCodigosColaborador(IEnumerable<string> codigos)
        {
            var set = new HashSet<string>(StringComparer.Ordinal);
            if (codigos == null)
                return set;
            foreach (var c in codigos)
            {
                if (string.IsNullOrWhiteSpace(c))
                    continue;
                set.Add(c.Trim());
            }
            return set;
        }

        private static List<string> NormalizarListaModeradores(IReadOnlyList<string> moderadores)
        {
            if (moderadores == null || moderadores.Count == 0)
                return new List<string>();
            return moderadores
                .Where(m => !string.IsNullOrWhiteSpace(m))
                .Select(m => m.Trim())
                .Distinct()
                .ToList();
        }

        /// <summary>
        /// O criador da comunidade entra sempre como moderador e como participante (inclui nas listas do request se ainda não estiver).
        /// </summary>
        private static void GarantirCriadorComoModeradorEParticipanteAoInserir(string codigoInternoColaborador, InserirComunidadeRequestDTO request)
        {
            if (string.IsNullOrWhiteSpace(codigoInternoColaborador))
                throw new ArgumentException("Código do colaborador criador inválido.");

            var criador = codigoInternoColaborador.Trim();

            request.CodigosInternoColaboradoresModeradores ??= new List<string>();
            if (!ListaContemCodigoColaborador(request.CodigosInternoColaboradoresModeradores, criador))
                request.CodigosInternoColaboradoresModeradores.Add(criador);

            request.CodigoInternoColaboradoresParticipantes ??= new List<string>();
            if (!ListaContemCodigoColaborador(request.CodigoInternoColaboradoresParticipantes, criador))
                request.CodigoInternoColaboradoresParticipantes.Add(criador);
        }

        /// <summary>
        /// Mesma regra de <see cref="GarantirModeradoresComoParticipantesAoAtualizarAsync"/>: participantes atuais do request + todos os moderadores (sem buscar no banco — comunidade ainda não existe).
        /// </summary>
        private static void GarantirModeradoresComoParticipantesAoInserir(InserirComunidadeRequestDTO request)
        {
            var participantes = request.CodigoInternoColaboradoresParticipantes;
            MergeModeradoresInformadosNaListaParticipantes(request.CodigosInternoColaboradoresModeradores, ref participantes);
            request.CodigoInternoColaboradoresParticipantes = participantes;
        }

        private static bool ListaContemCodigoColaborador(IReadOnlyList<string> lista, string codigoNormalizado)
        {
            if (lista == null || lista.Count == 0)
                return false;
            foreach (var item in lista)
            {
                if (string.IsNullOrWhiteSpace(item))
                    continue;
                if (string.Equals(item.Trim(), codigoNormalizado, StringComparison.Ordinal))
                    return true;
            }
            return false;
        }

        private static void ValidarModeradoresEParticipantesInserir(InserirComunidadeRequestDTO request)
        {
            var participantes = CriarConjuntoCodigosColaborador(request.CodigoInternoColaboradoresParticipantes);
            var moderadoresInformados = NormalizarListaModeradores(request.CodigosInternoColaboradoresModeradores);

            if (moderadoresInformados.Count == 0)
                throw new ArgumentException("É necessário informar pelo menos um moderador da comunidade.");

            foreach (var mod in moderadoresInformados)
            {
                if (!participantes.Contains(mod))
                    throw new ArgumentException($"Todo moderador deve constar na lista de participantes. Moderador não incluído entre participantes: {mod}.");
            }
        }

        private async Task GarantirModeradorNaoCriadorNaoRemoveCriadorModeradorAsync(
            Guid comunidadeId,
            int orgId,
            string codigoInternoColaborador,
            AtualizarComunidadeRequestDTO request)
        {
            var codigoCriador = await _repository.ObterCodigoInternoColaboradorCriacaoComunidadeAsync(comunidadeId, orgId);
            if (string.IsNullOrWhiteSpace(codigoCriador))
                return;

            codigoCriador = codigoCriador.Trim();
            var codigoLogado = codigoInternoColaborador?.Trim() ?? string.Empty;
            if (string.Equals(codigoLogado, codigoCriador, StringComparison.Ordinal))
                return;

            var criadorAindaModerador = await _repository.ColaboradorEModeradorDaComunidadeAsync(comunidadeId, codigoCriador, orgId);
            if (!criadorAindaModerador)
                return;

            var moderadoresNovos = NormalizarListaModeradores(request.CodigosInternoColaboradoresModeradores);
            if (ListaContemCodigoColaborador(moderadoresNovos, codigoCriador))
                return;

            throw new ApplicationException("Não é permitido remover o criador da comunidade da lista de moderadores. Somente o próprio criador pode fazer essa alteração.");
        }

        private static void ValidarModeradoresEParticipantesAtualizar(AtualizarComunidadeRequestDTO request)
        {
            var moderadoresInformados = request.CodigosInternoColaboradoresModeradores != null;
            var participantesInformados = request.CodigoInternoColaboradoresParticipantes != null;

            if (!moderadoresInformados && !participantesInformados)
                return;

            if (moderadoresInformados != participantesInformados)
                throw new ArgumentException("Para alterar membros da comunidade, é necessário informar participantes e moderadores juntos.");

            var participantes = CriarConjuntoCodigosColaborador(request.CodigoInternoColaboradoresParticipantes);
            var moderadores = NormalizarListaModeradores(request.CodigosInternoColaboradoresModeradores);

            if (moderadores.Count == 0)
                throw new ArgumentException("É necessário informar pelo menos um moderador da comunidade.");

            foreach (var mod in moderadores)
            {
                if (!participantes.Contains(mod))
                    throw new ArgumentException($"Todo moderador deve constar na lista de participantes. Moderador não incluído entre participantes: {mod}.");
            }
        }

        private async Task GarantirModeradoresComoParticipantesAoAtualizarAsync(Guid comunidadeId, int orgId, AtualizarComunidadeRequestDTO request)
        {
            if (request?.CodigosInternoColaboradoresModeradores == null)
                return;

            var moderadores = NormalizarListaModeradores(request.CodigosInternoColaboradoresModeradores);
            if (moderadores.Count == 0)
                return;

            if (request.CodigoInternoColaboradoresParticipantes == null)
            {
                var participantesAtuais = await _repository.ObterCodigosParticipantesComunidadeAsync(comunidadeId, orgId);
                request.CodigoInternoColaboradoresParticipantes = participantesAtuais ?? new List<string>();
            }

            var participantes = request.CodigoInternoColaboradoresParticipantes;
            MergeModeradoresInformadosNaListaParticipantes(request.CodigosInternoColaboradoresModeradores, ref participantes);
            request.CodigoInternoColaboradoresParticipantes = participantes;
        }

        /// <summary>Núcleo compartilhado entre criar e editar: unir codigos moderadores normalizados ao conjunto de participantes.</summary>
        private static void MergeModeradoresInformadosNaListaParticipantes(IReadOnlyList<string> codigosModeradoresBrutos, ref List<string> participantes)
        {
            if (codigosModeradoresBrutos == null)
                return;

            var moderadores = NormalizarListaModeradores(codigosModeradoresBrutos);
            if (moderadores.Count == 0)
                return;

            var set = CriarConjuntoCodigosColaborador(participantes);
            foreach (var moderador in moderadores)
                set.Add(moderador);

            participantes = set.ToList();
        }

        private async Task<string> FazerUploadCapaAsync(byte[] capaImagem)
        {
            var data = DateTime.Now.ToString("yyyyMMddHHmmssFFF");
            var pathArquivoS3 = $"marketing/comunicacao/capas/capa_comunidade_{data}.png";

            await _uploadFilesClient.UploadFile(pathArquivoS3, capaImagem);

            return $"{VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.SERVICE_MIDIA)}{pathArquivoS3}";
        }

        public async Task<ApiGenericResult> ParticiparComunidadeAsync(Guid comunidadeId, string codigoInternoColaborador, int orgId)
        {
            var result = new ApiGenericResult();
            try
            {
                var sucesso = await _repository.ParticiparComunidadeAsync(comunidadeId, codigoInternoColaborador, orgId);
                if (!sucesso)
                {
                    result.Mensagem = "Comunidade não encontrada ou você não tem permissão para participar (comunidades públicas; privadas em que você está em algum grupo vinculado ou na lista de participantes).";
                    return result;
                }
                result.Mensagem = "Participação na comunidade realizada com sucesso.";
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Create, DESCRICAO_ENTIDADE);
            }
            return result;
        }

        public async Task<ApiGenericResult> SairComunidadeAsync(Guid comunidadeId, string codigoInternoColaborador, int orgId)
        {
            var result = new ApiGenericResult();
            try
            {
                var sucesso = await _repository.SairComunidadeAsync(comunidadeId, codigoInternoColaborador, orgId);
                if (!sucesso)
                {
                    result.Mensagem = "Comunidade não encontrada, esta comunidade não permite sair ou você não é membro.";
                    return result;
                }
                result.Mensagem = "Você saiu da comunidade com sucesso.";
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Update, DESCRICAO_ENTIDADE);
            }
            return result;
        }
    }
}
