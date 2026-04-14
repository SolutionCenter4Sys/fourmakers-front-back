using DataTransferObject.Domain.Experiencia;
using System.Collections.Generic;

namespace Core.Domain.BI
{
    public interface IExperienciaProfissionalBIRepository
    {
        List<KeyValuePair<string, ListaExperienciaDTO>> Listar();
    }
}