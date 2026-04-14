using Colaboracao.Core.Interfaces;
using Colaboracao.Helper.Enum;
using Colaboracao.Helper.Util;
using Core.Domain.Marketing.Comunicacao.Grupo;
using Core.Domain.Marketing.Comunicacao.Profissionais;
using Core.Domain.Marketing.Comunicacao.Publicacao;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Marketing.Comunicacao.Profissionais;
using DataTransferObject.Domain.Marketing.Comunicacao.Publicacao;
using Marketing.Domain.Interfaces.Comunicacao.Publicacao;
using ApiClient.Domain.Interfaces;
using Colaboracao.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ApiClient.Domain;

namespace Marketing.Domain.Impl.Comunicacao.Publicacao
{
    public class ComunicacaoPublicacaoService : IComunicacaoPublicacaoService
    {
        private const string DESCRICAO_ENTIDADE = "Marketing - Comunicacao";
        private const long TAMANHO_MAXIMO_ANEXO_BYTES = 50L * 1024L * 1024L;
        private const string PASTA_ANEXOS_PUBLICACAO = "marketing/comunicacao/publicacoes/anexos";
        private readonly IComunicacaoPublicacaoRepository _repository;
        private readonly IComunicacaoGrupoRepository _grupoRepository;
        private readonly IComunicacaoProfissionaisRepository _profissionaisRepository;
        private readonly IDBConnectionUnitOfWork _dbConnectionUnitOfWork;
        private readonly IUploadFilesClient _uploadFilesClient;

        public ComunicacaoPublicacaoService(
            IComunicacaoPublicacaoRepository repository,
            IComunicacaoGrupoRepository grupoRepository,
            IComunicacaoProfissionaisRepository profissionaisRepository,
            IDBConnectionUnitOfWork dbConnectionUnitOfWork,
            IUploadFilesClient uploadFilesClient)
        {
            _repository = repository;
            _grupoRepository = grupoRepository;
            _profissionaisRepository = profissionaisRepository;
            _dbConnectionUnitOfWork = dbConnectionUnitOfWork;
            _uploadFilesClient = uploadFilesClient;
        }

        public async Task<ApiGenericResult<PublicacaoFeedResponseDTO>> ObterListaPublicacaoGeralAsync(string codigoInternoColaborador, int orgId, bool somenteLeituraObrigatoria, string tipo, List<string> labels = null, List<string> tags = null, List<string> status = null, List<string> statusAprovacao = null, string comunidadeId = null, bool somenteNaoOcultoNoFeed = false, bool somentePublicacaoOficial = false)
        {
            var result = new ApiGenericResult<PublicacaoFeedResponseDTO>();
            try
            {
                if (string.IsNullOrWhiteSpace(codigoInternoColaborador))
                    throw new ArgumentException("Código interno do colaborador é obrigatório para listar publicações.");
                await _repository.CriarOuAtualizarInteracoesParaFeedGeralAsync(codigoInternoColaborador, orgId, somenteLeituraObrigatoria, tipo, labels, tags, status, statusAprovacao, comunidadeId, somenteNaoOcultoNoFeed, somentePublicacaoOficial);
                result.Retorno = await _repository.ObterListaPublicacaoGeralAsync(codigoInternoColaborador, orgId, somenteLeituraObrigatoria, tipo, labels, tags, status, statusAprovacao, comunidadeId, somenteNaoOcultoNoFeed, somentePublicacaoOficial);
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, DESCRICAO_ENTIDADE);
            }
            return result;
        }

        public async Task<ApiGenericResult<PublicacaoPastasSugestaoResponseDTO>> ObterSugestoesPastasAsync(string codigoInternoColaborador, int orgId)
        {
            var result = new ApiGenericResult<PublicacaoPastasSugestaoResponseDTO>();
            try
            {
                if (string.IsNullOrWhiteSpace(codigoInternoColaborador))
                    throw new ArgumentException("Código interno do colaborador é obrigatório.");
                var nomes = await _repository.ObterNomesPastasSugestaoAsync(orgId);
                result.Retorno = new PublicacaoPastasSugestaoResponseDTO { Pastas = nomes };
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, DESCRICAO_ENTIDADE);
            }
            return result;
        }

        public async Task<ApiGenericResult> ConfirmarLeituraObrigatoriaAsync(string codigoInternoColaborador, int orgId, ConfirmarLeituraObrigatoriaRequestDTO request)
        {
            var result = new ApiGenericResult();
            try
            {
                if (request == null || request?.PublicacaoId == null)
                {
                    throw new ArgumentException("PublicacaoId obrigatório.");
                }
                var sucesso = await _repository.ConfirmarLeituraObrigatoriaAsync(request.PublicacaoId, codigoInternoColaborador, orgId);
                if (!sucesso)
                {
                    throw new ApplicationException("Não foi possível confirmar a leitura obrigatória.");
                }
                result.Mensagem = "Leitura obrigatória confirmada com sucesso.";
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Update, DESCRICAO_ENTIDADE);
            }
            return result;
        }

        public async Task<ApiGenericResult<List<ProfissionalDTO>>> ObterColaboradoresQueConfirmaramLeituraAsync(string publicacaoId, string codigoInternoColaborador, int orgId)
        {
            var result = new ApiGenericResult<List<ProfissionalDTO>>();
            try
            {
                if (string.IsNullOrWhiteSpace(publicacaoId) || !Guid.TryParse(publicacaoId, out var publicacaoIdGuid))
                {
                    throw new ArgumentException("PublicacaoId inválido ou não informado.");
                }
                var codigosOrdenados = await _repository.ObterCodigosColaboradoresQueConfirmaramLeituraAsync(publicacaoIdGuid, orgId);
                if (codigosOrdenados.Count == 0)
                {
                    result.Retorno = new List<ProfissionalDTO>();
                    return result;
                }
                var profissionais = await _profissionaisRepository.ObterProfissionaisPorCodigosAsync(orgId, codigosOrdenados, codigoInternoColaborador);
                var ordemPorCodigo = codigosOrdenados.Select((cod, idx) => (cod, idx)).ToDictionary(x => x.cod, x => x.idx);
                result.Retorno = profissionais
                    .OrderBy(p => ordemPorCodigo.TryGetValue(p.CodigoColaboradorInterno, out var idx) ? idx : int.MaxValue)
                    .ToList();
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, DESCRICAO_ENTIDADE);
            }
            return result;
        }

        public async Task<ApiGenericResult<PublicacaoDetalheDTO>> InserirPublicacaoAsync(string codigoInternoColaborador, int orgId, InserirPublicacaoRequestDTO request)
            => await InserirPublicacaoAsync(codigoInternoColaborador, orgId, request, null);

        public async Task<ApiGenericResult<PublicacaoDetalheDTO>> InserirPublicacaoAsync(string codigoInternoColaborador, int orgId, InserirPublicacaoRequestDTO request, List<PublicacaoAnexoUploadDTO> anexosUpload)
        {
            var result = new ApiGenericResult<PublicacaoDetalheDTO>();
            var uploadsEfetuados = new List<string>();
            _dbConnectionUnitOfWork.BeginTransaction();
            try
            {
                if (request == null) throw new ArgumentNullException(nameof(request));
                if (string.IsNullOrWhiteSpace(request.Titulo)) throw new ArgumentException("Título obrigatório.");
                if (string.IsNullOrWhiteSpace(request.Tipo)) throw new ArgumentException("Tipo da publicação obrigatório (informativo ou documento).");
                
                var tipoValido = string.Equals(request.Tipo, "informativo", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(request.Tipo, "documento", StringComparison.OrdinalIgnoreCase);
                
                if (!tipoValido)
                    throw new ArgumentException("Tipo da publicação deve ser 'informativo' ou 'documento'.");

                if (!string.IsNullOrWhiteSpace(request.ComunidadeId) && request.Grupos?.Count > 0)
                    throw new ArgumentException("Quando ComunidadeId for informado, não é permitido informar Grupos; comunidades já possuem grupos relacionados.");
                
                if (!string.IsNullOrWhiteSpace(request.ComunidadeId))
                    request.Grupos = null;

                // Publicações de comunidade não devem validar os bits de permissão de grupos (oficial/informativo/aprovação).
                // Para publicação oficial (sem ComunidadeId), é obrigatório estar em um grupo com o bit de criação de publicação oficial.
                if (string.IsNullOrWhiteSpace(request.ComunidadeId))
                {
                    var podeCriar = await _grupoRepository.UsuarioPodeCriarPublicacaoOficialAsync(codigoInternoColaborador, orgId);
                    if (!podeCriar)
                        throw new ApplicationException("Você não possui permissão para criar publicação oficial. É necessário estar em um grupo que permita criar publicação oficial.");
                }

                NormalizarLabelsETagsParaTipo(request.Tipo, request);
                await ValidarGruposAsync(request.Grupos, orgId);

                if (anexosUpload?.Count > 0)
                {
                    request.Anexos = await FazerUploadAnexosAsync(anexosUpload, request.Anexos, uploadsEfetuados);
                }

                request.DataAgendamentoPublicacao = DateTimeUtil.ParaUtcPreservandoInstante(request.DataAgendamentoPublicacao);
                request.DataValidadePublicacao = DateTimeUtil.ParaUtcPreservandoInstante(request.DataValidadePublicacao);

                string aprovacaoStatus;
                string publicacaoStatus;
                DateTime? dataPublicacao;

                if (!string.IsNullOrWhiteSpace(request.ComunidadeId))
                {
                    aprovacaoStatus = "nao_requer";
                    if (request.DataAgendamentoPublicacao.HasValue)
                    {
                        publicacaoStatus = "agendada";
                        dataPublicacao = null;
                    }
                    else
                    {
                        publicacaoStatus = "ativa";
                        dataPublicacao = DateTime.Now;
                    }
                }
                else
                {
                    var usuarioPodePublicarDireto = await _repository.UsuarioEstaEmAlgumGrupoQuePermitePublicarSemAprovacaoAsync(codigoInternoColaborador, orgId);
                    
                    var requerAprovacao = !usuarioPodePublicarDireto;
                    
                    aprovacaoStatus = requerAprovacao ? "pendente" : "nao_requer";
                    
                    if (requerAprovacao)
                    {
                        publicacaoStatus = "aguardando_aprovacao";
                        dataPublicacao = null;
                    }
                    else if (request.DataAgendamentoPublicacao.HasValue)
                    {
                        publicacaoStatus = "agendada";
                        dataPublicacao = null;
                    }
                    else
                    {
                        publicacaoStatus = "ativa";
                        dataPublicacao = DateTime.Now;
                    }
                }

                var publicacaoId = await _repository.InserirPublicacaoAsync(codigoInternoColaborador, orgId, request, publicacaoStatus, aprovacaoStatus, dataPublicacao);
                result.Retorno = await _repository.ObterPublicacaoPorIdAsync(publicacaoId, codigoInternoColaborador, orgId);
                result.Mensagem = "Publicação criada com sucesso.";

                _dbConnectionUnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                _dbConnectionUnitOfWork.Rollback();
                await TentarRemoverUploadsAsync(uploadsEfetuados);
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Create, DESCRICAO_ENTIDADE);
            }
            return result;
        }

        public async Task<ApiGenericResult<PublicacaoDetalheDTO>> ObterPublicacaoPorIdAsync(string publicacaoId, string codigoInternoColaborador, int orgId)
        {
            var result = new ApiGenericResult<PublicacaoDetalheDTO>();
            try
            {
                if (string.IsNullOrWhiteSpace(codigoInternoColaborador))
                    throw new ArgumentException("Código interno do colaborador é obrigatório para obter publicação.");
                if (string.IsNullOrWhiteSpace(publicacaoId)) throw new ArgumentException("Id da publicação obrigatório.");
                if (!Guid.TryParse(publicacaoId, out var publicacaoGuid))
                    throw new ArgumentException("Id da publicação inválido.");
                var publicacao = await _repository.ObterPublicacaoPorIdAsync(publicacaoGuid, codigoInternoColaborador, orgId);
                if (publicacao == null) throw new ApplicationException("Publicação não encontrada.");
                result.Retorno = publicacao;
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, DESCRICAO_ENTIDADE);
            }
            return result;
        }

        public async Task<ApiGenericResult> AtualizarPublicacaoAsync(string codigoInternoColaborador, int orgId, AtualizarPublicacaoRequestDTO request)
        {
            var result = new ApiGenericResult();
            _dbConnectionUnitOfWork.BeginTransaction();
            try
            {
                if (request == null || request?.PublicacaoId == null)
                    throw new ArgumentException("PublicacaoId obrigatório para atualizar.");

                var publicacaoAtual = await _repository.ObterPublicacaoPorIdAsync(request.PublicacaoId, codigoInternoColaborador, orgId);
                if (publicacaoAtual == null)
                    throw new ApplicationException("Publicação não encontrada para atualização.");
                if (publicacaoAtual.RequerConfirmacaoLeitura && (publicacaoAtual.Analitico?.QuantidadeConfirmacoesLeitura ?? 0) > 0)
                    throw new ApplicationException("Não é possível editar a publicação, pois ela requer confirmação de leitura e já possui aceite(s).");

                if (!string.IsNullOrWhiteSpace(request.Tipo))
                {
                    var tipoValido = string.Equals(request.Tipo, "informativo", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(request.Tipo, "documento", StringComparison.OrdinalIgnoreCase);
                    if (!tipoValido)
                        throw new ArgumentException("Tipo da publicação deve ser 'informativo' ou 'documento'.");
                }
                if (!string.IsNullOrWhiteSpace(request.ComunidadeId) && request.Grupos?.Count > 0)
                    throw new ArgumentException("Quando ComunidadeId for informado, não é permitido informar Grupos; comunidades já possuem grupos relacionados.");
                if (!string.IsNullOrWhiteSpace(request.ComunidadeId))
                    request.Grupos = null;

                var tipoEfetivo = string.IsNullOrWhiteSpace(request.Tipo) ? publicacaoAtual.Tipo : request.Tipo;
                NormalizarLabelsETagsParaTipo(tipoEfetivo, request);
                await ValidarGruposAsync(request.Grupos, orgId);

                request.DataAgendamentoPublicacao = DateTimeUtil.ParaUtcPreservandoInstante(request.DataAgendamentoPublicacao);
                request.DataValidadePublicacao = DateTimeUtil.ParaUtcPreservandoInstante(request.DataValidadePublicacao);

                var sucesso = await _repository.AtualizarPublicacaoAsync(codigoInternoColaborador, orgId, request);
                if (!sucesso) throw new ApplicationException("Publicação não encontrada para atualização.");
                result.Mensagem = "Publicação atualizada com sucesso.";

                _dbConnectionUnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                _dbConnectionUnitOfWork.Rollback();
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Update, DESCRICAO_ENTIDADE);
            }
            return result;
        }

        public async Task<ApiGenericResult> DeletarPublicacaoAsync(string publicacaoId, string codigoInternoColaborador, int orgId)
        {
            var result = new ApiGenericResult();
            try
            {
                if (string.IsNullOrWhiteSpace(publicacaoId)) throw new ArgumentException("Id da publicação obrigatório.");
                if (!Guid.TryParse(publicacaoId, out var publicacaoGuid))
                    throw new ArgumentException("Id da publicação inválido.");
                var publicacao = await _repository.ObterPublicacaoPorIdAsync(publicacaoGuid, codigoInternoColaborador, orgId);
                if (publicacao == null) throw new ApplicationException("Publicação não encontrada para remoção.");
                if (publicacao.Autor == null || !string.Equals(publicacao.Autor.CodigoColaboradorInterno, codigoInternoColaborador, StringComparison.OrdinalIgnoreCase))
                    throw new UnauthorizedAccessException("Somente o autor pode excluir a publicação.");
                var sucesso = await _repository.DeletarPublicacaoAsync(publicacaoId, codigoInternoColaborador, orgId);
                if (!sucesso) throw new ApplicationException("Publicação não encontrada para remoção.");
                result.Mensagem = "Publicação removida com sucesso.";
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Delete, DESCRICAO_ENTIDADE);
            }
            return result;
        }

        public async Task<ApiGenericResult> ArquivarPublicacaoAsync(string publicacaoId, string codigoInternoColaborador, int orgId)
        {
            var result = new ApiGenericResult();
            try
            {
                if (string.IsNullOrWhiteSpace(publicacaoId)) throw new ArgumentException("Id da publicação obrigatório.");
                if (!Guid.TryParse(publicacaoId, out var publicacaoGuid))
                    throw new ArgumentException("Id da publicação inválido.");
                var publicacao = await _repository.ObterPublicacaoPorIdAsync(publicacaoGuid, codigoInternoColaborador, orgId);
                if (publicacao == null) throw new ApplicationException("Publicação não encontrada para arquivamento.");
                if (publicacao.Autor == null || !string.Equals(publicacao.Autor.CodigoColaboradorInterno, codigoInternoColaborador, StringComparison.OrdinalIgnoreCase))
                    throw new UnauthorizedAccessException("Somente o autor pode arquivar a publicação.");
                var sucesso = await _repository.ArquivarPublicacaoAsync(publicacaoId, codigoInternoColaborador, orgId);
                if (!sucesso) throw new ApplicationException("Publicação não encontrada para arquivamento.");
                result.Mensagem = "Publicação arquivada com sucesso.";
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Update, DESCRICAO_ENTIDADE);
            }
            return result;
        }

        public async Task<ApiGenericResult> AtualizarOcultarNoFeedAsync(string codigoInternoColaborador, int orgId, AtualizarOcultarNoFeedRequestDTO request)
        {
            var result = new ApiGenericResult();
            try
            {
                if (request == null)
                    throw new ArgumentException("Request obrigatório.");
                if (request.PublicacaoId == Guid.Empty)
                    throw new ArgumentException("PublicacaoId obrigatório.");
                var sucesso = await _repository.AtualizarOcultarNoFeedAsync(request.PublicacaoId, codigoInternoColaborador, orgId, request.OcultarNoFeed);
                if (!sucesso)
                    throw new ApplicationException("Publicação não encontrada.");
                result.Mensagem = request.OcultarNoFeed ? "Publicação ocultada do feed." : "Publicação exibida no feed.";
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Update, DESCRICAO_ENTIDADE);
            }
            return result;
        }

        public async Task<ApiGenericResult> AdicionarAnexosAsync(string publicacaoId, string codigoInternoColaborador, int orgId, List<PublicacaoAnexoInputDTO> anexos, List<PublicacaoAnexoUploadDTO> anexosUpload)
        {
            var result = new ApiGenericResult();
            var uploadsEfetuados = new List<string>();
            _dbConnectionUnitOfWork.BeginTransaction();
            try
            {
                if (string.IsNullOrWhiteSpace(publicacaoId)) throw new ArgumentException("PublicacaoId obrigatório.");
                if (!Guid.TryParse(publicacaoId, out var publicacaoGuid))
                    throw new ArgumentException("PublicacaoId inválido.");
                if (anexosUpload == null || anexosUpload.Count == 0)
                    throw new ArgumentException("Anexos obrigatórios.");

                var publicacao = await _repository.ObterPublicacaoPorIdAsync(publicacaoGuid, codigoInternoColaborador, orgId);
                if (publicacao == null) throw new ApplicationException("Publicação não encontrada.");

                var anexosInput = await FazerUploadAnexosAsync(anexosUpload, anexos, uploadsEfetuados);
                var sucesso = await _repository.InserirAnexosPublicacaoAsync(publicacaoGuid, anexosInput);
                if (!sucesso) throw new ApplicationException("Não foi possível adicionar os anexos.");

                result.Mensagem = "Anexos adicionados com sucesso.";
                _dbConnectionUnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                _dbConnectionUnitOfWork.Rollback();
                await TentarRemoverUploadsAsync(uploadsEfetuados);
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Update, DESCRICAO_ENTIDADE);
            }
            return result;
        }

        public async Task<ApiGenericResult> RemoverAnexoAsync(string publicacaoId, string anexoId, string codigoInternoColaborador, int orgId)
        {
            var result = new ApiGenericResult();
            _dbConnectionUnitOfWork.BeginTransaction();
            try
            {
                if (string.IsNullOrWhiteSpace(publicacaoId)) throw new ArgumentException("PublicacaoId obrigatório.");
                if (string.IsNullOrWhiteSpace(anexoId)) throw new ArgumentException("AnexoId obrigatório.");
                if (!Guid.TryParse(publicacaoId, out var publicacaoGuid))
                    throw new ArgumentException("PublicacaoId inválido.");
                if (!Guid.TryParse(anexoId, out var anexoGuid))
                    throw new ArgumentException("AnexoId inválido.");

                var anexo = await _repository.ObterAnexoPublicacaoAsync(publicacaoGuid, anexoGuid);
                if (anexo == null) throw new ApplicationException("Anexo não encontrado.");

                var key = ExtrairKeyDoUrl(anexo.UrlArquivo);
                await _uploadFilesClient.DeleteFile(key);

                var sucesso = await _repository.RemoverAnexoPublicacaoAsync(publicacaoGuid, anexoGuid);
                if (!sucesso) throw new ApplicationException("Não foi possível remover o anexo.");

                result.Mensagem = "Anexo removido com sucesso.";
                _dbConnectionUnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                _dbConnectionUnitOfWork.Rollback();
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Delete, DESCRICAO_ENTIDADE);
            }
            return result;
        }

        public async Task<ApiGenericResult> RemoverAnexosAsync(string publicacaoId, List<string> anexoIds, string codigoInternoColaborador, int orgId)
        {
            var result = new ApiGenericResult();
            _dbConnectionUnitOfWork.BeginTransaction();
            try
            {
                if (string.IsNullOrWhiteSpace(publicacaoId)) throw new ArgumentException("PublicacaoId obrigatório.");
                if (anexoIds == null || anexoIds.Count == 0)
                    throw new ArgumentException("Informe ao menos um AnexoId para remoção.");
                if (!Guid.TryParse(publicacaoId, out var publicacaoGuid))
                    throw new ArgumentException("PublicacaoId inválido.");

                var publicacao = await _repository.ObterPublicacaoPorIdAsync(publicacaoGuid, codigoInternoColaborador, orgId);
                if (publicacao == null) throw new ApplicationException("Publicação não encontrada.");

                var anexoIdsValidos = anexoIds.Where(id => !string.IsNullOrWhiteSpace(id)).Distinct().ToList();
                if (anexoIdsValidos.Count == 0)
                    throw new ArgumentException("Nenhum AnexoId válido informado.");

                foreach (var anexoId in anexoIdsValidos)
                {
                    if (!Guid.TryParse(anexoId, out var anexoGuid))
                        continue;

                    var anexo = await _repository.ObterAnexoPublicacaoAsync(publicacaoGuid, anexoGuid);
                    if (anexo == null)
                        continue;

                    var key = ExtrairKeyDoUrl(anexo.UrlArquivo);
                    await _uploadFilesClient.DeleteFile(key);

                    var sucesso = await _repository.RemoverAnexoPublicacaoAsync(publicacaoGuid, anexoGuid);
                    if (!sucesso)
                        throw new ApplicationException($"Não foi possível remover o anexo {anexoId}.");
                }

                result.Mensagem = anexoIdsValidos.Count == 1 ? "Anexo removido com sucesso." : "Anexos removidos com sucesso.";
                _dbConnectionUnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                _dbConnectionUnitOfWork.Rollback();
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Delete, DESCRICAO_ENTIDADE);
            }
            return result;
        }

        private const int MINUTOS_PERMITIDOS_EDITAR_COMENTARIO = 15;

        public async Task<ApiGenericResult<Guid>> AdicionarComentarioAsync(string codigoInternoColaborador, int orgId, InserirComentarioRequestDTO request)
        {
            var result = new ApiGenericResult<Guid>();
            try
            {
                if (request == null)
                    throw new ArgumentException("Request obrigatório.");
                if (request.PublicacaoId == Guid.Empty)
                    throw new ArgumentException("PublicacaoId obrigatório.");
                if (string.IsNullOrWhiteSpace(request.Conteudo))
                    throw new ArgumentException("Conteúdo do comentário obrigatório.");

                var publicacao = await _repository.ObterPublicacaoPorIdAsync(request.PublicacaoId, codigoInternoColaborador, orgId);
                if (publicacao == null)
                    throw new ApplicationException("Publicação não encontrada.");
                if (!publicacao.PermiteComentarios)
                    throw new ApplicationException("Esta publicação não permite comentários.");

                var comentarioPaiId = request.ComentarioPaiId;
                if (comentarioPaiId.HasValue && comentarioPaiId.Value != Guid.Empty)
                {
                    var comentarioPai = await _repository.ObterComentarioParaValidacaoAsync(comentarioPaiId.Value);
                    if (comentarioPai == null)
                        throw new ApplicationException("Comentário pai não encontrado.");
                }

                var id = await _repository.InserirComentarioAsync(request.PublicacaoId, codigoInternoColaborador, request.Conteudo.Trim(), comentarioPaiId);
                if (!id.HasValue)
                    throw new ApplicationException("Não foi possível adicionar o comentário.");
                result.Retorno = id.Value;
                result.Mensagem = "Comentário adicionado com sucesso.";
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Create, DESCRICAO_ENTIDADE);
            }
            return result;
        }

        public async Task<ApiGenericResult> AtualizarComentarioAsync(string codigoInternoColaborador, int orgId, AtualizarComentarioRequestDTO request)
        {
            var result = new ApiGenericResult();
            try
            {
                if (request == null)
                    throw new ArgumentException("Request obrigatório.");
                if (request.ComentarioId == Guid.Empty)
                    throw new ArgumentException("ComentarioId obrigatório.");
                if (string.IsNullOrWhiteSpace(request.Conteudo))
                    throw new ArgumentException("Conteúdo do comentário obrigatório.");

                var comentario = await _repository.ObterComentarioParaValidacaoAsync(request.ComentarioId);
                if (comentario == null)
                    throw new ApplicationException("Comentário não encontrado.");
                if (!string.Equals(comentario.Value.CodigoAutor, codigoInternoColaborador, StringComparison.OrdinalIgnoreCase))
                    throw new UnauthorizedAccessException("Somente o autor pode editar o comentário.");
                var minutosDesdeCriacao = (DateTime.UtcNow - comentario.Value.DataCriacao).TotalMinutes;
                if (minutosDesdeCriacao > MINUTOS_PERMITIDOS_EDITAR_COMENTARIO)
                    throw new UnauthorizedAccessException("Comentário só pode ser editado em até 15 minutos após a criação.");

                var sucesso = await _repository.AtualizarComentarioAsync(request.ComentarioId, codigoInternoColaborador, request.Conteudo.Trim());
                if (!sucesso)
                    throw new ApplicationException("Não foi possível atualizar o comentário.");
                result.Mensagem = "Comentário atualizado com sucesso.";
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Update, DESCRICAO_ENTIDADE);
            }
            return result;
        }

        public async Task<ApiGenericResult> RemoverComentarioAsync(string publicacaoId, string comentarioId, string codigoInternoColaborador, int orgId)
        {
            var result = new ApiGenericResult();
            try
            {
                if (string.IsNullOrWhiteSpace(publicacaoId))
                    throw new ArgumentException("PublicacaoId obrigatório.");
                if (string.IsNullOrWhiteSpace(comentarioId))
                    throw new ArgumentException("ComentarioId obrigatório.");
                if (!Guid.TryParse(publicacaoId, out var publicacaoGuid))
                    throw new ArgumentException("PublicacaoId inválido.");
                if (!Guid.TryParse(comentarioId, out var comentarioGuid))
                    throw new ArgumentException("ComentarioId inválido.");

                var publicacao = await _repository.ObterPublicacaoPorIdAsync(publicacaoGuid, codigoInternoColaborador, orgId);
                if (publicacao == null)
                    throw new ApplicationException("Publicação não encontrada.");

                var comentario = await _repository.ObterComentarioParaValidacaoAsync(comentarioGuid);
                if (comentario == null)
                    throw new ApplicationException("Comentário não encontrado.");
                if (!string.Equals(comentario.Value.CodigoAutor, codigoInternoColaborador, StringComparison.OrdinalIgnoreCase))
                    throw new ApplicationException("Você não tem permissão para remover este comentário.");
                if ((DateTime.UtcNow - comentario.Value.DataCriacao).TotalHours > 24)
                    throw new ApplicationException("Comentário só pode ser removido em até 24 horas após a publicação.");

                var sucesso = await _repository.RemoverComentarioAsync(comentarioGuid, codigoInternoColaborador);
                if (!sucesso)
                    throw new ApplicationException("Comentário não encontrado ou você não tem permissão para removê-lo.");
                result.Mensagem = "Comentário removido com sucesso.";
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Delete, DESCRICAO_ENTIDADE);
            }
            return result;
        }

        public async Task<ApiGenericResult> AdicionarInteracaoComentarioAsync(string codigoInternoColaborador, int orgId, AdicionarInteracaoComentarioRequestDTO request)
        {
            var result = new ApiGenericResult();
            try
            {
                if (request == null)
                    throw new ArgumentException("Request obrigatório.");
                if (request.ComentarioId == Guid.Empty)
                    throw new ArgumentException("ComentarioId obrigatório.");
                if (request.Emoji != null && request.Emoji.Length > 50)
                    throw new ArgumentException("Emoji deve ter no máximo 50 caracteres.");

                var sucesso = await _repository.AdicionarInteracaoComentarioAsync(
                    request.ComentarioId,
                    codigoInternoColaborador,
                    request.Emoji?.Trim() ?? string.Empty);
                if (!sucesso)
                    throw new ApplicationException("Comentário não encontrado.");
                result.Mensagem = "Interação registrada com sucesso.";
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Update, DESCRICAO_ENTIDADE);
            }
            return result;
        }

        public async Task<ApiGenericResult> RemoverInteracaoComentarioAsync(string codigoInternoColaborador, int orgId, RemoverInteracaoComentarioRequestDTO request)
        {
            var result = new ApiGenericResult();
            try
            {
                if (request == null)
                    throw new ArgumentException("Request obrigatório.");
                if (request.ComentarioId == Guid.Empty)
                    throw new ArgumentException("ComentarioId obrigatório.");

                var sucesso = await _repository.RemoverInteracaoComentarioAsync(request.ComentarioId, codigoInternoColaborador);
                if (!sucesso)
                    throw new ApplicationException("Interação não encontrada ou já foi removida.");
                result.Mensagem = "Interação removida com sucesso.";
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Delete, DESCRICAO_ENTIDADE);
            }
            return result;
        }

        public async Task<ApiGenericResult> AdicionarInteracaoPublicacaoAsync(string codigoInternoColaborador, int orgId, AdicionarInteracaoPublicacaoRequestDTO request)
        {
            var result = new ApiGenericResult();
            try
            {
                if (request == null)
                    throw new ArgumentException("Request obrigatório.");
                if (request.PublicacaoId == Guid.Empty)
                    throw new ArgumentException("PublicacaoId obrigatório.");
                if (request.Emoji != null && request.Emoji.Length > 50)
                    throw new ArgumentException("Emoji deve ter no máximo 50 caracteres.");

                var sucesso = await _repository.AdicionarInteracaoPublicacaoAsync(
                    request.PublicacaoId,
                    codigoInternoColaborador,
                    request.Emoji?.Trim() ?? string.Empty);
                if (!sucesso)
                    throw new ApplicationException("Publicação não encontrada.");
                result.Mensagem = "Interação registrada com sucesso.";
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Update, DESCRICAO_ENTIDADE);
            }
            return result;
        }

        public async Task<ApiGenericResult> RemoverInteracaoPublicacaoAsync(string codigoInternoColaborador, int orgId, RemoverInteracaoPublicacaoRequestDTO request)
        {
            var result = new ApiGenericResult();
            try
            {
                if (request == null)
                    throw new ArgumentException("Request obrigatório.");
                if (request.PublicacaoId == Guid.Empty)
                    throw new ArgumentException("PublicacaoId obrigatório.");

                var sucesso = await _repository.RemoverInteracaoPublicacaoAsync(request.PublicacaoId, codigoInternoColaborador);
                if (!sucesso)
                    throw new ApplicationException("Interação não encontrada ou já foi removida.");
                result.Mensagem = "Interação removida com sucesso.";
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Delete, DESCRICAO_ENTIDADE);
            }
            return result;
        }

        private async Task ValidarGruposAsync(List<string> grupos, int orgId)
        {
            if (grupos == null || grupos.Count == 0)
                return;

            var gruposInvalidos = grupos.Where(grupoId => !Guid.TryParse(grupoId, out _)).ToList();
            if (gruposInvalidos.Count > 0)
                throw new ArgumentException($"GrupoId inválido: {string.Join(", ", gruposInvalidos)}");

            var existentes = await _repository.ObterGruposExistentesAsync(grupos, orgId);
            var faltantes = grupos.Except(existentes, StringComparer.OrdinalIgnoreCase).ToList();
            if (faltantes.Count > 0)
                throw new ArgumentException($"Grupo(s) não encontrado(s): {string.Join(", ", faltantes)}");
        }

        private async Task<List<PublicacaoAnexoInputDTO>> FazerUploadAnexosAsync(
            List<PublicacaoAnexoUploadDTO> anexosUpload,
            List<PublicacaoAnexoInputDTO> anexosRequest,
            List<string> uploadsEfetuados)
        {
            var anexos = new List<PublicacaoAnexoInputDTO>();
            var baseUrlMidia = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.SERVICE_MIDIA);

            for (var i = 0; i < anexosUpload.Count; i++)
            {
                var anexo = anexosUpload[i];
                if (anexo?.Conteudo == null || anexo.Conteudo.Length == 0)
                    throw new ArgumentException("Arquivo de anexo vazio.");

                var tamanhoBytes = anexo.TamanhoBytes ?? anexo.Conteudo.Length;
                if (tamanhoBytes > TAMANHO_MAXIMO_ANEXO_BYTES)
                    throw new ArgumentException($"Arquivo excede o limite de 50MB: {anexo.NomeArquivo}");

                var extensao = Path.GetExtension(anexo.NomeArquivo);
                if (string.IsNullOrWhiteSpace(extensao))
                    extensao = ".bin";

                var anexoId = Guid.NewGuid();
                var key = $"{PASTA_ANEXOS_PUBLICACAO}/{anexoId}{extensao}";
                await _uploadFilesClient.UploadFile(key, anexo.Conteudo);
                uploadsEfetuados.Add(key);

                var tipo = (anexosRequest != null && anexosRequest.Count > i) ? anexosRequest[i]?.Tipo : anexo.Tipo;

                anexos.Add(new PublicacaoAnexoInputDTO
                {
                    AnexoId = anexoId,
                    NomeArquivo = anexo.NomeArquivo,
                    UrlArquivo = $"{baseUrlMidia}{key}",
                    Tipo = tipo,
                    TamanhoBytes = tamanhoBytes
                });
            }

            return anexos;
        }

        private static string ExtrairKeyDoUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("Url do anexo inválida.");

            var baseUrlMidia = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.SERVICE_MIDIA);
            if (!string.IsNullOrWhiteSpace(baseUrlMidia) && url.StartsWith(baseUrlMidia, StringComparison.OrdinalIgnoreCase))
            {
                return url.Substring(baseUrlMidia.Length).TrimStart('/');
            }

            if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
            {
                return uri.AbsolutePath.TrimStart('/');
            }

            throw new ArgumentException("Não foi possível extrair o caminho do anexo.");
        }

        private async Task TentarRemoverUploadsAsync(List<string> uploadsEfetuados)
        {
            if (uploadsEfetuados == null || uploadsEfetuados.Count == 0)
                return;

            foreach (var key in uploadsEfetuados)
            {
                try
                {
                    await _uploadFilesClient.DeleteFile(key);
                }
                catch
                {
                    // Best-effort rollback; manter erro original do fluxo.
                }
            }
        }

        /// <summary>
        /// Labels para informativo e documento; tags somente para documento (complementar às labels).
        /// </summary>
        private static void NormalizarLabelsETagsParaTipo(string tipoPublicacao, InserirPublicacaoRequestDTO request)
        {
            if (request == null || string.IsNullOrWhiteSpace(tipoPublicacao))
                return;

            request.Labels ??= new List<string>();
            request.Tags ??= new List<string>();
            request.Labels = request.Labels.Where(l => !string.IsNullOrWhiteSpace(l)).Select(l => l.Trim()).ToList();
            request.Tags = request.Tags.Where(t => !string.IsNullOrWhiteSpace(t)).Select(t => t.Trim()).ToList();

            var documento = string.Equals(tipoPublicacao, "documento", StringComparison.OrdinalIgnoreCase);
            if (!documento && request.Tags.Count > 0)
                throw new ArgumentException("Tags só podem ser usadas em publicações do tipo documento.");
            if (!documento)
                request.Tags = new List<string>();
        }
    }
}
