using DataTransferObject.Domain.Formacao;
using System.Collections.Generic;

namespace Formacao.Domain.Interfaces.Services
{
    public interface IFormacaoService
    {
        FormacaoDTO AddFormacao(string descricao);
        List<FormacaoDTO> ListarFormacao(string busca, int cursor, int limite);
        FormacaoDTO GetFormacaoById(long id);
    }
}