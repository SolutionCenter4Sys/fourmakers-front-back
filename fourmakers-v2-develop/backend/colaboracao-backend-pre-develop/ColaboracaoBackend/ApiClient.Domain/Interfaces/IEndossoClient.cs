using DataTransferObject.Domain.Endosso;
using NewApiAppColaboracao.Models.BI;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiClient.Domain.Interfaces
{
    public interface IEndossoClient
    {
        Task<List<PedidoEndossoDTO>> ListarEndossosColaborador(string cpf, string tokenUsuario);
        Task<InfoEndossoGrafico> ListarEndossosPorCompetencia(long id, string tokenUsuario);
    }
}