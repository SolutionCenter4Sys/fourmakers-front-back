using DataTransferObject.Domain.Diretoria;
using System.Collections.Generic;

namespace Core.Domain.Colaborador
{
    public interface IColaboradorDiretoriaRepository
    {
        List<DiretoriaColaboradorDTO> ListarDiretoriaDosColaboradores(int orgId, List<string>? restricaoDiretorias = null);
    }
}