using DataTransferObject.Domain.Noticia;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiClient.Domain.Interfaces
{
    public interface INoticiaClient
    {
        Task<List<NoticiaDTO>> BuscarNoticia(string busca, int cursor, int limite, string tokenUsuario);
    }
}