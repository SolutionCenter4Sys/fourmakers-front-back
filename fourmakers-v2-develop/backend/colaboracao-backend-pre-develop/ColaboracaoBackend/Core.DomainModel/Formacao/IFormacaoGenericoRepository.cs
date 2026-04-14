using DataTransferObject.Domain.Formacao;

namespace Core.Domain.Formacao
{
    public interface IFormacaoGenericoRepository

    {
        FormacaoColaboradorDTO AtualizaFormacaoColaborador(FormacaoColaboradorDTO formacaoColaborador);
    }
}