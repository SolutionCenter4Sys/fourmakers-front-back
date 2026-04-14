using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Contratacao;
using DataTransferObject.Domain.SRS;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SRS.Domain.Interfaces.Service
{
    public interface IContratacaoService
    {
        Task<ApiGenericResult<TemplateDTO>> CriarTemplateAsync(TemplateDTO template, string cpf);
        Task<ApiGenericResult<TemplateDTO>> ObterTemplatePorIdAsync(Guid id);
        Task<ApiGenericResult<TemplateDTO>> ObterTemplatePorCandidatoVagaIdAsync(Guid tbCandidatoVagaId);
        Task<ApiGenericResult<List<TemplateDTO>>> ListarTemplatesAsync(int limite = 10, int cursor = 0);
        Task<ApiGenericResult<TemplateDTO>> AtualizarTemplateAsync(Guid id, TemplateDTO template, string cpf);
        Task<ApiGenericResult<bool>> DeletarTemplateAsync(Guid id);
        Task<ApiGenericResult<bool>> ExisteTemplateParaCandidatoAsync(string tbCandidatoVagaId);
        Task<ApiGenericResult<List<EquipamentoPadraoDTO>>> ListarEquipamentosPadroesAsync();
        Task<ApiGenericResult<List<EquipamentoPadraoAninhadoDTO>>> ListarEquipamentosPadroesAninhadosAsync();
        Task<ApiGenericResult<bool>> ExisteEmailUsuarioAsync(string email);
        Task<bool> EnviarEmailTemplateCandidato(string idCandidatura, byte[] documento, int orgId, bool anexo, string nomePDF, List<string> emailsAdicionais = null, bool ocultarValores = false);
    }
}

