using DataTransferObject.Domain.Idioma;

namespace Core.Domain.IIdioma
{
    public interface IIdiomaRepositoryGenerico
    {
        public IdiomaColaboradorDTO AlterarIdiomaColaborador(IdiomaColaboradorDTO idiomaColaborador);
    }
}