using DataTransferObject.Domain.Comentario;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiClient.Domain.Interfaces
{
    public interface IComentarioClient
    {
        Task<List<ComentarioTipoDTO>> ListarTipoComentarios(string tokenUsuario);
        Task<ComentarioDTO> InserirComentario(string cpf, int type, string texto, string tokenUsuario);
    }
}