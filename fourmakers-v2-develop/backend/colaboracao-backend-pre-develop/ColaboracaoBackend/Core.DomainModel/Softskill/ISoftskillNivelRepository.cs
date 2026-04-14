using DataTransferObject.Domain.Nivel;
using System.Collections.Generic;

namespace Competencia.Domain.Interfaces.Services
{
    public interface ISoftskillNivelRepository
    {
        long BuscarIdItemPerfilPorDescricao(string descricao);
        List<NivelDTO> ListarNivelSoftskill(long itemPerfilId);
    }
}