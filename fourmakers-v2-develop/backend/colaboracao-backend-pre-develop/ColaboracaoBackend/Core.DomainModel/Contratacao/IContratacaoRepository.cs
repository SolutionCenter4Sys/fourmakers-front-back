using DataTransferObject.Domain.Contratacao;
using DataTransferObject.Domain.SRS;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.Contratacao
{
    public interface IContratacaoRepository
    {
        Task<TemplateDTO> ObterTemplatePorIdAsync(Guid id);
        Task<TemplateDTO> ObterTemplatePorCandidatoVagaIdAsync(Guid tbCandidatoVagaId);
        Task<TemplateDTO> ObterInformacoesColaboradorPorCandidatoAsync(Guid tbCandidatoVagaId);
        Task<TemplateDTO> ObterNomeECargoCandidatoAsync(Guid tbCandidatoVagaId);
        Task<bool> ExisteEmailUsuarioAsync(string email);
        Task<List<TemplateDTO>> ListarTemplatesAsync(int limite, int cursor);
        Task<TemplateDTO> CriarTemplateAsync(TemplateDTO template, string codColaboradorLogado);
        Task<TemplateDTO> AtualizarTemplateAsync(TemplateDTO template, string codColaboradorLogado);
        Task<bool> DeletarTemplateAsync(Guid id);
        Task<int> ContarTemplatesAsync();
        Task<bool> ExisteTemplateParaCandidatoAsync(string tbCandidatoVagaId);
        Task<List<EquipamentoPadraoDTO>> ListarEquipamentosPadroesAsync();
        Task<List<EquipamentoPadraoAninhadoDTO>> ListarEquipamentosPadroesAninhadosAsync();
        Task<bool> ExisteColaboradorPorCodigoInternoAsync(string codigoInternoColaborador);
        Task<bool> ExisteEquipamentoPadraoCargoFuncaoAsync(string equipamentoPadraoCargoFuncaoId);
    }
}
