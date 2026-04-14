using DataTransferObject.Domain.Idioma;
using System.Collections.Generic;

namespace Core.Domain.IIdioma
{
    public interface IIdiomaRepository
    {
        public void DeleteIdioma(IdiomaDTO idioma);
        public IdiomaDTO GetIdioma(IdiomaDTO idioma);
        public IdiomaDTO GetIdiomaById(int id);
        public List<IdiomaDTO> ListarIdiomas(string busca, int cursor, int limite);
        public IdiomaDTO AdicionarIdioma(IdiomaDTO idioma);
        public List<int> ListarIdiomasAtribuidos(string cpfColaborador);
    }
}