using DataTransferObject.Domain.Experiencia;
using System.Collections.Generic;

namespace Core.Domain
{
    public interface IExperienciaProfissionalRepository<TModel, TFactory>
    {
        TModel Save(TModel model);
        List<TModel> Listar(string busca, int cursor, int limite, TFactory factory, string cpf);
        TModel GetModel(TModel model);
        void DeleteExperienciaProfissioanlModel(TModel model);
        TModel Update(TModel model);
        List<string> GetAutoCompleteEmpresa(string nomeEmpresa, int limite, int cursor);
        List<string> GetAutoCompleteProjeto(string nomeProjeto, int limite, int cursor);
        void UpsertSobre(string cpf, string sobre);
        ColaboradorSobreDTO GetSobre(string cpf);
        void RemoveSobre(string cpf);
    }
}