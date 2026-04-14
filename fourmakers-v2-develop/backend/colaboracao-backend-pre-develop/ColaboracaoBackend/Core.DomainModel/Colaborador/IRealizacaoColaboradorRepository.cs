using DataTransferObject.Domain;
using System.Collections.Generic;

namespace Core.Domain
{
    public interface IRealizacaoColaboradorRepository
    {
        List<RealizacaoColaboradorDTO> ListaRealizacoesColaborador(string codInternoColaborador, int orgId);
    }
}