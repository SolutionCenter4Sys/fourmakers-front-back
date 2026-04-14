using DataTransferObject.Domain.Formacao;
using System.Collections.Generic;

namespace Core.Domain.Formacao
{
    public interface IFormacaoColaboradorRepository
    {
        public void RemoverFormacaoColaborador(FormacaoColaboradorDTO formacaoColab);
        public List<FormacaoColaboradorDTO> ListFormacaoColaborador(FormacaoColaboradorDTO formacaoColab);
        public FormacaoColaboradorDTO CadastrarFormacaoColaborador(FormacaoColaboradorDTO model);
    }
}