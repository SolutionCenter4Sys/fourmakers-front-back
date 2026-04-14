using System.Collections.Generic;
using DataTransferObject.Domain.Escolaridade;

namespace Core.DomainModel
{
    public interface IEscolaridadeColaboradorDtoRepository
    {
        EscolaridadeDTO Save(EscolaridadeDTO model);
        List<EscolaridadeDTO> Listar(string cpf);
        EscolaridadeDTO GetModel(EscolaridadeDTO model);
        void DeleteEscolaridadeColaboradorModel(EscolaridadeDTO model);
        EscolaridadeDTO Update(EscolaridadeDTO model);
        List<EscolaridadeDTO> Listar(string busca, int cursor, int limite, string cpf);
    }
}
