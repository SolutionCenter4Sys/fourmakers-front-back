using ApiClient.Domain;
using Colaboracao.Helper;
using Colaboracao.Helper.Enum;
using Colaboracao.Helper.Util;
using Core.Domain.GestaoPessoa.Pdi;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.GestaoPessoa.Pdi;
using GestaoPessoa.Domain.Interfaces.Services.Pdi;
using Logs.Infra.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoPessoa.Domain.Impl.Services.Pdi
{
    [LogDomainClass]
    public class MeusPdisService : IMeusPdisService
    {
        private const string DESCRICAO_ENTIDADE = "PDI";
        private readonly IPdiRepository _pdiRepository;

        public MeusPdisService(IPdiRepository pdiRepository)
        {
            _pdiRepository = pdiRepository;
        }

        public async Task<ApiGenericResult<IEnumerable<PdiResumoDTO>>> ListarMeusPdisAsync(string cpf, int orgId)
        {
            var result = new ApiGenericResult<IEnumerable<PdiResumoDTO>>();
            try
            {
                result.Retorno = await _pdiRepository.ListarMeusPdisAsync(cpf.ToStringOuVazio(), orgId);
                result.Sucesso = true;
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, DESCRICAO_ENTIDADE);
            }
            return result;
        }

        public async Task<ApiGenericResult<PdiCriarResponseDTO>> CriarPdiAsync(string cpf, int orgId, PdiCriarRequestDTO request)
        {
            var result = new ApiGenericResult<PdiCriarResponseDTO>();
            try
            {
                if (request == null)
                {
                    result.Sucesso = false;
                    result.Mensagem = "Corpo da requisição inválido ou vazio. Envie JSON com titulo, descricao, skills (ao menos uma com codigoSkill) e, se quiser, actionPlans.";
                    return result;
                }
                if (string.IsNullOrWhiteSpace(request.Titulo))
                {
                    result.Sucesso = false;
                    result.Mensagem = "titulo é obrigatório.";
                    return result;
                }
                if (request.Skills == null || request.Skills.Count == 0)
                {
                    result.Sucesso = false;
                    result.Mensagem = "Um PDI deve possuir ao menos uma skill.";
                    return result;
                }
                if (request.Skills.Any(s => string.IsNullOrWhiteSpace(s?.CodigoSkill)))
                {
                    result.Sucesso = false;
                    result.Mensagem = "codigoSkill é obrigatório em todas as skills.";
                    return result;
                }

                var cpfStr = cpf.ToStringOuVazio();
                var id = await _pdiRepository.InserirPdiAsync(cpfStr, orgId, request, codigoInternoCriacao: cpfStr);
                // Criado pelo colaborador com planos já no body: vai direto para Em Análise (sem aprovação).
                var statusInicial = "NOT_STARTED";
                if ((request.ActionPlans?.Count ?? 0) > 0)
                {
                    await _pdiRepository.AtualizarStatusPdiAsync(id, "IN_ANALYSIS", cpfStr);
                    statusInicial = "IN_ANALYSIS";
                }
                result.Retorno = new PdiCriarResponseDTO
                {
                    Id = id,
                    Status = statusInicial,
                    DataCriacao = DateTime.UtcNow
                };
                result.Sucesso = true;
            }
            catch (Exception ex)
            {
                var mensagemInterna = ex.InnerException?.Message ?? ex.Message;
                result.Sucesso = false;
                if (mensagemInterna.IndexOf("foreign key", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    mensagemInterna.IndexOf("Cannot add or update a child row", StringComparison.OrdinalIgnoreCase) >= 0)
                    result.Mensagem = "Colaborador não encontrado na base. Verifique se o usuário está vinculado como colaborador (tb_colaborador).";
                else if (mensagemInterna.IndexOf("Duplicate", StringComparison.OrdinalIgnoreCase) >= 0 ||
                         mensagemInterna.IndexOf("unique", StringComparison.OrdinalIgnoreCase) >= 0)
                    result.Mensagem = "Conflito de dados: skill ou registro já existente.";
                else
                    result.Mensagem = "Erro ao criar PDI.";
            }
            return result;
        }

        public async Task<ApiGenericResult<PdiAtualizarResponseDTO>> AtualizarPdiAsync(Guid id, string cpf, int orgId, PdiAtualizarRequestDTO request)
        {
            var result = new ApiGenericResult<PdiAtualizarResponseDTO>();
            try
            {
                var pertence = await _pdiRepository.PertenceAoColaboradorAsync(id, cpf.ToStringOuVazio());
                if (!pertence)
                {
                    result.Sucesso = false;
                    result.Mensagem = "PDI não encontrado ou sem permissão.";
                    return result;
                }
                var ok = await _pdiRepository.AtualizarPdiAsync(id, cpf.ToStringOuVazio(), request);
                if (!ok)
                {
                    result.Sucesso = false;
                    result.Mensagem = "Não foi possível atualizar o PDI.";
                    return result;
                }
                result.Retorno = new PdiAtualizarResponseDTO { Id = id, DataAtualizacao = DateTime.UtcNow };
                result.Sucesso = true;
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Update, DESCRICAO_ENTIDADE);
            }
            return result;
        }

        public async Task<ApiGenericResult<PdiActionPlanDTO>> AdicionarActionPlanAsync(Guid pdiId, string cpf, int orgId, PdiActionPlanInputDTO request)
        {
            var result = new ApiGenericResult<PdiActionPlanDTO>();
            try
            {
                var pertence = await _pdiRepository.PertenceAoColaboradorAsync(pdiId, cpf.ToStringOuVazio());
                if (!pertence)
                {
                    result.Sucesso = false;
                    result.Mensagem = "PDI não encontrado ou sem permissão.";
                    return result;
                }
                var cpfStr = cpf.ToStringOuVazio();
                var id = await _pdiRepository.InserirActionPlanAsync(pdiId, cpfStr, request);
                result.Retorno = new PdiActionPlanDTO
                {
                    Id = id,
                    Description = request.Description,
                    Deadline = request.Deadline,
                    ConcluidoEm = null
                };
                // Criado pelo colaborador: após add plano de ação → PDI vai para Em Análise (sem aprovação).
                var (owner, criadoPor, _) = await _pdiRepository.ObterCriadorPdiAsync(pdiId);
                if (owner != null && criadoPor != null && string.Equals(owner, criadoPor, StringComparison.OrdinalIgnoreCase))
                    await _pdiRepository.AtualizarStatusPdiAsync(pdiId, "IN_ANALYSIS", cpfStr);
                result.Sucesso = true;
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Create, DESCRICAO_ENTIDADE);
            }
            return result;
        }

        public async Task<ApiGenericResult<PdiConcluirActionPlanResponseDTO>> ConcluirActionPlanAsync(Guid pdiId, Guid actionPlanId, string cpf, int orgId)
        {
            var result = new ApiGenericResult<PdiConcluirActionPlanResponseDTO>();
            try
            {
                var ok = await _pdiRepository.ConcluirActionPlanAsync(pdiId, actionPlanId, cpf.ToStringOuVazio());
                if (!ok)
                {
                    result.Sucesso = false;
                    result.Mensagem = "Action plan não encontrado ou sem permissão.";
                    return result;
                }
                var total = await _pdiRepository.ContarActionPlansAsync(pdiId);
                var concluidos = await _pdiRepository.ContarActionPlansConcluidosAsync(pdiId);
                var progress = total > 0 ? (double)concluidos / total : 1.0;
                result.Retorno = new PdiConcluirActionPlanResponseDTO
                {
                    Id = actionPlanId,
                    ConcluidoEm = DateTime.UtcNow,
                    Progress = progress,
                    Status = progress >= 1.0 ? "COMPLETED" : "IN_PROGRESS"
                };
                result.Sucesso = true;
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Update, DESCRICAO_ENTIDADE);
            }
            return result;
        }

        public async Task<ApiGenericResult<PdiEvidenciaUploadResultDTO>> UploadEvidenciaAsync(Guid pdiId, string cpf, int orgId, string docName, string docPath, string docMime, long? docSize, string tipo, byte[] docBytes, string link)
        {
            var result = new ApiGenericResult<PdiEvidenciaUploadResultDTO>();
            try
            {
                var urlNorm = PdiEvidenciaUrlUtil.TryNormalize(link, out var urlErro);
                if (urlErro != null)
                {
                    result.Sucesso = false;
                    result.Mensagem = urlErro;
                    return result;
                }

                var hasFile = docBytes != null && docBytes.Length > 0;
                if (!hasFile && string.IsNullOrEmpty(urlNorm))
                {
                    result.Sucesso = false;
                    result.Mensagem = "Envie um arquivo ou informe link.";
                    return result;
                }

                var pertence = await _pdiRepository.PertenceAoColaboradorAsync(pdiId, cpf.ToStringOuVazio());
                if (!pertence)
                {
                    result.Sucesso = false;
                    result.Mensagem = "PDI não encontrado ou sem permissão.";
                    return result;
                }

                string pathFinal;
                if (hasFile)
                {
                    var basePath = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.PATH_FOTO_COLABORADOR) ?? "arquivos/colaboradores/";
                    pathFinal = string.IsNullOrWhiteSpace(docPath)
                        ? $"{basePath}pdi/{pdiId}/{Guid.NewGuid():N}_{docName ?? "documento"}"
                        : docPath;
                }
                else
                    pathFinal = null;

                var nomeFinal = hasFile
                    ? (docName ?? "documento")
                    : (string.IsNullOrWhiteSpace(docName) ? "Evidência (link)" : docName.Trim());

                var id = await _pdiRepository.InserirEvidenciaAsync(pdiId, nomeFinal, pathFinal, docMime, docSize, tipo ?? "CERTIFICADO", hasFile ? docBytes : null, urlNorm);
                result.Retorno = new PdiEvidenciaUploadResultDTO
                {
                    Id = id,
                    PdiId = pdiId,
                    DocName = nomeFinal,
                    DocPath = pathFinal,
                    DocMime = docMime,
                    DocSize = docSize,
                    Tipo = tipo ?? "CERTIFICADO",
                    Link = urlNorm,
                    CreatedAt = DateTime.UtcNow
                };
                result.Sucesso = true;
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Create, DESCRICAO_ENTIDADE);
            }
            return result;
        }

        public async Task<ApiGenericResult<IEnumerable<PdiEvidenciaDTO>>> ListarEvidenciasAsync(Guid pdiId, string cpf, int orgId)
        {
            var result = new ApiGenericResult<IEnumerable<PdiEvidenciaDTO>>();
            try
            {
                result.Retorno = await _pdiRepository.ListarEvidenciasAsync(pdiId, cpf.ToStringOuVazio());
                result.Sucesso = true;
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, DESCRICAO_ENTIDADE);
            }
            return result;
        }

        public async Task<ApiGenericResult<PdiEvidenciaDownloadDTO>> ObterEvidenciaAsync(Guid pdiId, Guid evidenciaId, string cpf, int orgId)
        {
            var result = new ApiGenericResult<PdiEvidenciaDownloadDTO>();
            try
            {
                var (doc, docName, docMime, link) = await _pdiRepository.ObterEvidenciaBytesAsync(pdiId, evidenciaId, cpf ?? string.Empty);
                if (doc == null && string.IsNullOrWhiteSpace(link))
                {
                    result.Sucesso = false;
                    result.Mensagem = "Evidência não encontrada ou sem permissão.";
                    return result;
                }
                result.Retorno = new PdiEvidenciaDownloadDTO
                {
                    Content = doc,
                    FileName = docName ?? "documento",
                    ContentType = docMime ?? "application/octet-stream",
                    Link = link
                };
                result.Sucesso = true;
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, DESCRICAO_ENTIDADE);
            }
            return result;
        }
    }
}
