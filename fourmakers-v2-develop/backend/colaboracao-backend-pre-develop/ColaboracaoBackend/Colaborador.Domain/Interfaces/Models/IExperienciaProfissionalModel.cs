using DataTransferObject.Domain.Experiencia;
using System.Collections.Generic;

namespace Colaborador.Domain.Interfaces.Models
{
    public interface IExperienciaProfissionalModel
    {
        ExperienciaDTO ExperienciaDTO { get; set; }
        ListaExperienciaDTO ListaExperienciaDTO { get; set; }
        UpdateExperienciaDTO UpdateExperienciaDTO { get; set; }
        IExperienciaProfissionalModel Create();
        List<IExperienciaProfissionalModel> List(string busca, int cursor, int limite, string cpf);
        IExperienciaProfissionalModel GetById(long id);
        void RemoveExperienciaProfissional();
        IExperienciaProfissionalModel Update();
    }
}