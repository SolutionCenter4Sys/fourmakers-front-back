using DataTransferObject.Domain.Idioma;
using System.Collections.Generic;

namespace Core.Domain.IIdioma
{
    public interface IIdiomaColaboradorRepository
    {
        public List<IdiomaColaboradorDTO> ListarIdiomaColaborador(IdiomaColaboradorDTO idiomaColaboradorDTO);
        void RemoverIdiomaColaborador(IdiomaColaboradorDTO idiomaColaborador);
        public IdiomaColaboradorDTO AdicionarIdiomaColaborador(IdiomaColaboradorDTO idioma);
        IdiomaColaboradorDTO AlterarIdiomaColaborador(IdiomaColaboradorDTO idiomaColaborador);
    }
}