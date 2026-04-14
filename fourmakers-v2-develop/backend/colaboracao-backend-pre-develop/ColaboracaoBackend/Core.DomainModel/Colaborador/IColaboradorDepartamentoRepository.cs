using DataTransferObject.Domain.Diretoria;
using System.Collections.Generic;

namespace Core.Domain.Colaborador
{
    public interface IColaboradorDepartamentoRepository
    {
        List<DepartamentoColaboradorDTO> ListarDepartamentosDosColaboradores(int orgId, string codigoDiretoria, List<string>? restricaoDiretorias = null);
    }
}