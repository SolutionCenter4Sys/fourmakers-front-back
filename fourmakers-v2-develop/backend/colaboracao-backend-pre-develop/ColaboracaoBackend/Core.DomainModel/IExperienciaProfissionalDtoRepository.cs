using System.Collections.Generic;
using DataTransferObject.Domain.Experiencia;

namespace Core.DomainModel
{
    public interface IExperienciaProfissionalDtoRepository
    {
        ExperienciaDTO Save(ExperienciaDTO model);
        List<ExperienciaDTO> Listar(string busca, int cursor, int limite, string cpf);
        ExperienciaDTO GetModel(ExperienciaDTO model);
        void DeleteExperienciaProfissionalModel(ExperienciaDTO model);
        ExperienciaDTO Update(ExperienciaDTO model);
        List<string> GetAutoCompleteEmpresa(string nomeEmpresa, int limite, int cursor);
        List<string> GetAutoCompleteProjeto(string nomeProjeto, int limite, int cursor);
        void UpsertSobre(string cpf, string sobre);
        ColaboradorSobreDTO GetSobre(string cpf);
        void RemoveSobre(string cpf);
    }
}
