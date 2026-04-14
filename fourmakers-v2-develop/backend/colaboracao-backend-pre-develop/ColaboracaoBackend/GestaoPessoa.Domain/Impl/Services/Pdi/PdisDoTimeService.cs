using Core.Domain.Colaborador;
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
    public class PdisDoTimeService : IPdisDoTimeService
    {
        private const string DESCRICAO_ENTIDADE = "PDI Time";
        private readonly IPdiRepository _pdiRepository;
        private readonly IBuscaColaboradorRepository _buscaColaboradorRepository;

        public PdisDoTimeService(IPdiRepository pdiRepository, IBuscaColaboradorRepository buscaColaboradorRepository)
        {
            _pdiRepository = pdiRepository;
            _buscaColaboradorRepository = buscaColaboradorRepository;
        }

        public async Task<ApiGenericResult<IEnumerable<PdiResumoTimeDTO>>> ListarPdisDoTimeAsync(string cpf, int orgId)
        {
            var result = new ApiGenericResult<IEnumerable<PdiResumoTimeDTO>>();
            try
            {
                var codigosParaListar = string.IsNullOrEmpty(cpf)
                    ? new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                    : ObterCodigosInternosApenasSubordinadosDoTime(cpf, orgId);
                if (codigosParaListar.Count == 0)
                {
                    result.Retorno = Array.Empty<PdiResumoTimeDTO>();
                    result.Sucesso = true;
                    return result;
                }
                result.Retorno = await _pdiRepository.ListarPdisDoTimeAsync(codigosParaListar.ToList(), orgId);
                result.Sucesso = true;
            }
            catch (Exception)
            {
                result.Sucesso = false;
                result.Mensagem = "Erro ao listar PDIs do time.";
            }
            return result;
        }

        public async Task<ApiGenericResult<PdiListagemTimeResult>> ListarPdisDoTimePaginadoAsync(string cpf, int orgId, int pagina, int tamanhoPagina)
        {
            var result = new ApiGenericResult<PdiListagemTimeResult>();
            try
            {
                var codigosParaListar = string.IsNullOrEmpty(cpf)
                    ? new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                    : ObterCodigosInternosApenasSubordinadosDoTime(cpf, orgId);
                if (codigosParaListar.Count == 0)
                {
                    result.Retorno = new PdiListagemTimeResult
                    {
                        Items = Array.Empty<PdiResumoTimeDTO>(),
                        TotalCount = 0,
                        Pagina = 1,
                        TamanhoPagina = Math.Max(1, Math.Min(100, tamanhoPagina)),
                        TotalPaginas = 0
                    };
                    result.Sucesso = true;
                    return result;
                }
                var paginaNorm = Math.Max(1, pagina);
                var tamanhoNorm = Math.Max(1, Math.Min(100, tamanhoPagina));
                var (items, totalCount) = await _pdiRepository.ListarPdisDoTimeAsync(codigosParaListar.ToList(), orgId, paginaNorm, tamanhoNorm);
                var totalPaginas = totalCount <= 0 ? 0 : (int)Math.Ceiling((double)totalCount / tamanhoNorm);
                result.Retorno = new PdiListagemTimeResult
                {
                    Items = items?.ToList() ?? new List<PdiResumoTimeDTO>(),
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
                result.Mensagem = "Erro ao listar PDIs do time (paginado).";
            }
            return result;
        }

        public async Task<ApiGenericResult<IEnumerable<PdiResumoDTO>>> ListarPdisPorColaboradorIdAsync(string colaboradorId, string cpfGestor, int orgId)
        {
            var result = new ApiGenericResult<IEnumerable<PdiResumoDTO>>();
            try
            {
                var colaboradorNorm = NormalizarCodigoInterno(colaboradorId);
                var codigosPermitidos = ObterCodigosPermitidos(cpfGestor, orgId);
                var podeVerPeloTimeAgregado = codigosPermitidos.Contains(colaboradorNorm);
                var podeVerPelaCadeiaHierarquia = _buscaColaboradorRepository.ColaboradorEstaNaHierarquiaSubordinadaDoGestorNaOrg(cpfGestor, colaboradorNorm, orgId);
                if (!podeVerPeloTimeAgregado && !podeVerPelaCadeiaHierarquia)
                {
                    result.Sucesso = false;
                    result.Mensagem = "Este colaborador não está na sua hierarquia de gestão. Só é possível visualizar PDIs de quem pertence ao seu time ou ao time dos seus subordinados.";
                    result.Retorno = Array.Empty<PdiResumoDTO>();
                    return result;
                }
                var lista = (await _pdiRepository.ListarPdisPorColaboradorIdAsync(colaboradorNorm, orgId))?.ToList() ?? new List<PdiResumoDTO>();
                result.Retorno = lista;
                result.Sucesso = true;
                if (lista.Count == 0)
                    result.Mensagem = "Este colaborador não possui PDIs cadastrados.";
            }
            catch (Exception)
            {
                result.Sucesso = false;
                result.Mensagem = "Erro ao listar PDIs do colaborador.";
            }
            return result;
        }

        private HashSet<string> ObterCodigosPermitidos(string cpfGestor, int orgId)
        {
            if (string.IsNullOrEmpty(cpfGestor))
                return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            return _buscaColaboradorRepository.ListarCodigosInternosHierarquiaTimeIncluindoGestor(cpfGestor, orgId);
        }

        /// <summary>Subordinados em todos os níveis, sem o próprio gestor (para listagem “só do time”).</summary>
        private HashSet<string> ObterCodigosInternosApenasSubordinadosDoTime(string cpfGestor, int orgId)
        {
            var todos = ObterCodigosPermitidos(cpfGestor, orgId);
            var self = NormalizarCodigoInterno(cpfGestor);
            if (self.Length > 0)
                todos.Remove(self);
            return todos;
        }

        /// <summary>Alinha com valores vindos de rota/query e do banco (TRIM + vazio estável).</summary>
        private static string NormalizarCodigoInterno(string codigo) =>
            string.IsNullOrWhiteSpace(codigo) ? string.Empty : codigo.Trim();

        public async Task<ApiGenericResult<PdiCompletoTimeDTO>> ObterPdiCompletoDoTimeAsync(Guid pdiId, string cpf, int orgId)
        {
            var result = new ApiGenericResult<PdiCompletoTimeDTO>();
            try
            {
                var codigosPermitidos = ObterCodigosPermitidos(cpf, orgId);
                var pdi = await _pdiRepository.ObterPdiCompletoDoTimeAsync(pdiId, orgId);
                if (pdi == null)
                {
                    result.Sucesso = false;
                    result.Mensagem = "PDI não encontrado.";
                    return result;
                }
                if (!codigosPermitidos.Contains(NormalizarCodigoInterno(pdi.ColaboradorId)))
                {
                    result.Sucesso = false;
                    result.Mensagem = "PDI não pertence ao seu time.";
                    return result;
                }
                result.Retorno = pdi;
                result.Sucesso = true;
            }
            catch (Exception)
            {
                result.Sucesso = false;
                result.Mensagem = "Erro ao obter PDI do time.";
            }
            return result;
        }

        public async Task<ApiGenericResult<PdiCriarResponseDTO>> CriarPdiParaColaboradorAsync(string colaboradorId, string cpfGestor, int orgId, PdiCriarRequestDTO request)
        {
            var result = new ApiGenericResult<PdiCriarResponseDTO>();
            try
            {
                var codigosPermitidos = ObterCodigosPermitidos(cpfGestor, orgId);
                if (!codigosPermitidos.Contains(NormalizarCodigoInterno(colaboradorId)))
                {
                    result.Sucesso = false;
                    result.Mensagem = "Colaborador não pertence ao seu time.";
                    return result;
                }
                if (request?.Skills == null || request.Skills.Count == 0)
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
                var id = await _pdiRepository.InserirPdiAsync(colaboradorId, orgId, request, codigoInternoCriacao: cpfGestor);
                result.Retorno = new PdiCriarResponseDTO { Id = id, Status = "NOT_STARTED", DataCriacao = DateTime.UtcNow };
                result.Sucesso = true;
            }
            catch (Exception ex)
            {
                var msg = ex.InnerException?.Message ?? ex.Message;
                result.Sucesso = false;
                if (msg.IndexOf("foreign key", StringComparison.OrdinalIgnoreCase) >= 0 || msg.IndexOf("Cannot add or update a child row", StringComparison.OrdinalIgnoreCase) >= 0)
                    result.Mensagem = "Colaborador não encontrado na base. Verifique se o usuário está vinculado como colaborador (tb_colaborador).";
                else if (msg.IndexOf("Duplicate", StringComparison.OrdinalIgnoreCase) >= 0 || msg.IndexOf("unique", StringComparison.OrdinalIgnoreCase) >= 0)
                    result.Mensagem = "Conflito de dados: skill ou registro já existente.";
                else
                    result.Mensagem = "Erro ao criar PDI para o colaborador.";
            }
            return result;
        }

        public async Task<ApiGenericResult<PdiAtualizarResponseDTO>> AtualizarPdiParaColaboradorAsync(string colaboradorId, Guid pdiId, string cpfGestor, int orgId, PdiAtualizarRequestDTO request)
        {
            var result = new ApiGenericResult<PdiAtualizarResponseDTO>();
            try
            {
                var codigosPermitidos = ObterCodigosPermitidos(cpfGestor, orgId);
                if (!codigosPermitidos.Contains(NormalizarCodigoInterno(colaboradorId)))
                {
                    result.Sucesso = false;
                    result.Mensagem = "Colaborador não pertence ao seu time.";
                    return result;
                }
                var pdi = await _pdiRepository.ObterPdiCompletoDoTimeAsync(pdiId, orgId);
                if (pdi == null || (pdi.ColaboradorId ?? string.Empty) != colaboradorId)
                {
                    result.Sucesso = false;
                    result.Mensagem = "PDI não encontrado ou não pertence a este colaborador.";
                    return result;
                }
                var ok = await _pdiRepository.AtualizarPdiAsync(pdiId, colaboradorId, request);
                if (!ok) { result.Sucesso = false; result.Mensagem = "Não foi possível atualizar o PDI."; return result; }
                result.Retorno = new PdiAtualizarResponseDTO { Id = pdiId, DataAtualizacao = DateTime.UtcNow };
                result.Sucesso = true;
            }
            catch (Exception)
            {
                result.Sucesso = false;
                result.Mensagem = "Erro ao atualizar PDI do colaborador.";
            }
            return result;
        }

        public async Task<ApiGenericResult<PdiActionPlanDTO>> AdicionarActionPlanParaColaboradorAsync(string colaboradorId, Guid pdiId, string cpfGestor, int orgId, PdiActionPlanInputDTO request)
        {
            var result = new ApiGenericResult<PdiActionPlanDTO>();
            try
            {
                var codigosPermitidos = ObterCodigosPermitidos(cpfGestor, orgId);
                if (!codigosPermitidos.Contains(NormalizarCodigoInterno(colaboradorId)))
                {
                    result.Sucesso = false;
                    result.Mensagem = "Colaborador não pertence ao seu time.";
                    return result;
                }
                var pertence = await _pdiRepository.PertenceAoColaboradorAsync(pdiId, colaboradorId);
                if (!pertence)
                {
                    result.Sucesso = false;
                    result.Mensagem = "PDI não encontrado ou não pertence a este colaborador.";
                    return result;
                }
                var pdiDeadline = await _pdiRepository.ObterDeadlinePdiAsync(pdiId);
                if (request.Deadline.HasValue && pdiDeadline.HasValue && request.Deadline.Value > pdiDeadline.Value)
                {
                    result.Sucesso = false;
                    result.Mensagem = "O prazo do plano de ação deve ser anterior ao prazo de conclusão do PDI.";
                    return result;
                }
                var id = await _pdiRepository.InserirActionPlanAsync(pdiId, cpfGestor, request);
                result.Retorno = new PdiActionPlanDTO { Id = id, Description = request.Description, Deadline = request.Deadline, ConcluidoEm = null };
                result.Sucesso = true;
            }
            catch (Exception)
            {
                result.Sucesso = false;
                result.Mensagem = "Erro ao adicionar action plan ao PDI do colaborador.";
            }
            return result;
        }

        public async Task<ApiGenericResult<PdiActionPlanDTO>> AtualizarActionPlanParaColaboradorAsync(string codigoInternoColaborador, Guid pdiId, Guid actionPlanId, string cpfGestor, int orgId, PdiActionPlanInputDTO request)
        {
            var result = new ApiGenericResult<PdiActionPlanDTO>();
            try
            {
                var codigosPermitidos = ObterCodigosPermitidos(cpfGestor, orgId);
                var ehOMesmoColaborador = string.Equals(NormalizarCodigoInterno(codigoInternoColaborador), NormalizarCodigoInterno(cpfGestor), StringComparison.OrdinalIgnoreCase);
                if (!codigosPermitidos.Contains(NormalizarCodigoInterno(codigoInternoColaborador)) && !ehOMesmoColaborador)
                {
                    result.Sucesso = false;
                    result.Mensagem = "Colaborador não pertence ao seu time.";
                    return result;
                }
                var pertence = await _pdiRepository.PertenceAoColaboradorAsync(pdiId, codigoInternoColaborador);
                if (!pertence)
                {
                    result.Sucesso = false;
                    result.Mensagem = "PDI não encontrado ou não pertence a este colaborador.";
                    return result;
                }
                var pdiDeadline = await _pdiRepository.ObterDeadlinePdiAsync(pdiId);
                if (request.Deadline.HasValue && pdiDeadline.HasValue && request.Deadline.Value > pdiDeadline.Value)
                {
                    result.Sucesso = false;
                    result.Mensagem = "O prazo do plano de ação deve ser anterior ao prazo de conclusão do PDI.";
                    return result;
                }
                var ok = await _pdiRepository.AtualizarActionPlanAsync(pdiId, actionPlanId, codigoInternoColaborador, request);
                if (!ok)
                {
                    result.Sucesso = false;
                    result.Mensagem = "Action plan não encontrado ou sem permissão.";
                    return result;
                }
                result.Retorno = new PdiActionPlanDTO { Id = actionPlanId, Description = request.Description, Deadline = request.Deadline, ConcluidoEm = null };
                result.Sucesso = true;
            }
            catch (Exception)
            {
                result.Sucesso = false;
                result.Mensagem = "Erro ao atualizar action plan do PDI do colaborador.";
            }
            return result;
        }

        public async Task<ApiGenericResult> RemoverActionPlanParaColaboradorAsync(string colaboradorId, Guid pdiId, Guid actionPlanId, string cpfGestor, int orgId)
        {
            var result = new ApiGenericResult();
            try
            {
                var codigosPermitidos = ObterCodigosPermitidos(cpfGestor, orgId);
                var ehOMesmoColaborador = string.Equals(NormalizarCodigoInterno(colaboradorId), NormalizarCodigoInterno(cpfGestor), StringComparison.OrdinalIgnoreCase);
                if (!codigosPermitidos.Contains(NormalizarCodigoInterno(colaboradorId)) && !ehOMesmoColaborador)
                {
                    result.Sucesso = false;
                    result.Mensagem = "Colaborador não pertence ao seu time.";
                    return result;
                }
                var pertence = await _pdiRepository.PertenceAoColaboradorAsync(pdiId, colaboradorId);
                if (!pertence)
                {
                    result.Sucesso = false;
                    result.Mensagem = "PDI não encontrado ou não pertence a este colaborador.";
                    return result;
                }
                var ok = await _pdiRepository.ExcluirActionPlanAsync(pdiId, actionPlanId, colaboradorId);
                if (!ok)
                {
                    result.Sucesso = false;
                    result.Mensagem = "Action plan não encontrado ou sem permissão.";
                    return result;
                }
                result.Sucesso = true;
            }
            catch (Exception)
            {
                result.Sucesso = false;
                result.Mensagem = "Erro ao remover action plan do PDI do colaborador.";
            }
            return result;
        }

        public async Task<ApiGenericResult<PdiConcluirActionPlanResponseDTO>> ConcluirActionPlanParaColaboradorAsync(string codigoInternoColaborador, Guid pdiId, Guid actionPlanId, string cpfGestor, int orgId)
        {
            var result = new ApiGenericResult<PdiConcluirActionPlanResponseDTO>();
            try
            {
                var codigosPermitidos = ObterCodigosPermitidos(cpfGestor, orgId);
                if (!codigosPermitidos.Contains(NormalizarCodigoInterno(codigoInternoColaborador)))
                {
                    result.Sucesso = false;
                    result.Mensagem = "Colaborador não pertence ao seu time.";
                    return result;
                }
                var ok = await _pdiRepository.ConcluirActionPlanAsync(pdiId, actionPlanId, codigoInternoColaborador);
                if (!ok)
                {
                    result.Sucesso = false;
                    result.Mensagem = "Action plan não encontrado ou sem permissão.";
                    return result;
                }
                var total = await _pdiRepository.ContarActionPlansAsync(pdiId);
                var concluidos = await _pdiRepository.ContarActionPlansConcluidosAsync(pdiId);
                var progress = total > 0 ? (double)concluidos / total : 1.0;
                result.Retorno = new PdiConcluirActionPlanResponseDTO { Id = actionPlanId, ConcluidoEm = DateTime.UtcNow, Progress = progress, Status = progress >= 1.0 ? "COMPLETED" : "IN_PROGRESS" };
                result.Sucesso = true;
            }
            catch (Exception)
            {
                result.Sucesso = false;
                result.Mensagem = "Erro ao concluir action plan do PDI do colaborador.";
            }
            return result;
        }

        public async Task<ApiGenericResult<PdiEvidenciaUploadResultDTO>> UploadEvidenciaParaColaboradorAsync(string colaboradorId, Guid pdiId, string cpfGestor, int orgId, string docName, string docPath, string docMime, long? docSize, string tipo, byte[] docBytes, string link)
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

                var codigosPermitidos = ObterCodigosPermitidos(cpfGestor, orgId);
                if (!codigosPermitidos.Contains(NormalizarCodigoInterno(colaboradorId)))
                {
                    result.Sucesso = false;
                    result.Mensagem = "Colaborador não pertence ao seu time.";
                    return result;
                }
                var pertence = await _pdiRepository.PertenceAoColaboradorAsync(pdiId, colaboradorId);
                if (!pertence)
                {
                    result.Sucesso = false;
                    result.Mensagem = "PDI não encontrado ou não pertence a este colaborador.";
                    return result;
                }

                var nomeFinal = hasFile
                    ? (docName ?? "documento")
                    : (string.IsNullOrWhiteSpace(docName) ? "Evidência (link)" : docName.Trim());
                var pathFinal = hasFile ? (docPath ?? "") : null;

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
            catch (Exception)
            {
                result.Sucesso = false;
                result.Mensagem = "Erro ao enviar evidência para o PDI do colaborador.";
            }
            return result;
        }

        public async Task<ApiGenericResult<IEnumerable<PdiEvidenciaDTO>>> ListarEvidenciasParaColaboradorAsync(string colaboradorId, Guid pdiId, string cpfGestor, int orgId)
        {
            var result = new ApiGenericResult<IEnumerable<PdiEvidenciaDTO>>();
            try
            {
                var codigosPermitidos = ObterCodigosPermitidos(cpfGestor, orgId);
                if (!codigosPermitidos.Contains(NormalizarCodigoInterno(colaboradorId)))
                {
                    result.Sucesso = false;
                    result.Mensagem = "Colaborador não pertence ao seu time.";
                    result.Retorno = Array.Empty<PdiEvidenciaDTO>();
                    return result;
                }
                var pertence = await _pdiRepository.PertenceAoColaboradorAsync(pdiId, colaboradorId);
                if (!pertence)
                {
                    result.Sucesso = false;
                    result.Mensagem = "PDI não encontrado ou não pertence a este colaborador.";
                    result.Retorno = Array.Empty<PdiEvidenciaDTO>();
                    return result;
                }
                result.Retorno = await _pdiRepository.ListarEvidenciasAsync(pdiId, colaboradorId);
                result.Sucesso = true;
            }
            catch (Exception)
            {
                result.Sucesso = false;
                result.Mensagem = "Erro ao listar evidências do PDI do colaborador.";
            }
            return result;
        }

        public async Task<ApiGenericResult<PdiEvidenciaDownloadDTO>> ObterEvidenciaParaColaboradorAsync(string colaboradorId, Guid pdiId, Guid evidenciaId, string cpfGestor, int orgId)
        {
            var result = new ApiGenericResult<PdiEvidenciaDownloadDTO>();
            try
            {
                var codigosPermitidos = ObterCodigosPermitidos(cpfGestor, orgId);
                if (!codigosPermitidos.Contains(NormalizarCodigoInterno(colaboradorId)))
                {
                    result.Sucesso = false;
                    result.Mensagem = "Colaborador não pertence ao seu time.";
                    return result;
                }
                var (doc, docName, docMime, link) = await _pdiRepository.ObterEvidenciaBytesAsync(pdiId, evidenciaId, colaboradorId);
                if (doc == null && string.IsNullOrWhiteSpace(link))
                {
                    result.Sucesso = false;
                    result.Mensagem = "Evidência não encontrada ou não pertence a este PDI.";
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
            catch (Exception)
            {
                result.Sucesso = false;
                result.Mensagem = "Erro ao obter evidência do PDI do colaborador.";
            }
            return result;
        }

        public async Task<ApiGenericResult<PdiAprovarResponseDTO>> AprovarPdiParaColaboradorAsync(string colaboradorId, Guid pdiId, string cpfGestor, int orgId)
        {
            var result = new ApiGenericResult<PdiAprovarResponseDTO>();
            try
            {
                var codigosPermitidos = ObterCodigosPermitidos(cpfGestor, orgId);
                if (!codigosPermitidos.Contains(NormalizarCodigoInterno(colaboradorId)))
                {
                    result.Sucesso = false;
                    result.Mensagem = "Colaborador não pertence ao seu time.";
                    return result;
                }
                var pertence = await _pdiRepository.PertenceAoColaboradorAsync(pdiId, colaboradorId);
                if (!pertence)
                {
                    result.Sucesso = false;
                    result.Mensagem = "PDI não encontrado ou não pertence a este colaborador.";
                    return result;
                }
                var (owner, criadoPor, status) = await _pdiRepository.ObterCriadorPdiAsync(pdiId);
                if (string.Equals(owner, criadoPor, StringComparison.OrdinalIgnoreCase))
                {
                    result.Sucesso = false;
                    result.Mensagem = "PDI foi criado pelo próprio colaborador; não requer aprovação do gestor.";
                    return result;
                }
                // Permite aprovar quando status = NOT_STARTED ou IN_ANALYSIS (sempre em inglês).
                var statusNorm = (status ?? string.Empty).Trim();
                var aguardandoAprovacao = string.Equals(statusNorm, "NOT_STARTED", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(statusNorm, "IN_ANALYSIS", StringComparison.OrdinalIgnoreCase);
                if (!aguardandoAprovacao)
                {
                    result.Sucesso = false;
                    result.Mensagem = "PDI já foi aprovado ou não está aguardando aprovação (status atual: " + (string.IsNullOrEmpty(statusNorm) ? "desconhecido" : statusNorm) + ").";
                    return result;
                }
                var ok = await _pdiRepository.AtualizarStatusPdiAsync(pdiId, "IN_PROGRESS", cpfGestor);
                if (!ok)
                {
                    result.Sucesso = false;
                    result.Mensagem = "Não foi possível aprovar o PDI.";
                    return result;
                }
                result.Retorno = new PdiAprovarResponseDTO { Id = pdiId, Status = "IN_PROGRESS", AprovadoEm = DateTime.UtcNow };
                result.Sucesso = true;
            }
            catch (Exception)
            {
                result.Sucesso = false;
                result.Mensagem = "Erro ao aprovar PDI do colaborador.";
            }
            return result;
        }
    }
}
