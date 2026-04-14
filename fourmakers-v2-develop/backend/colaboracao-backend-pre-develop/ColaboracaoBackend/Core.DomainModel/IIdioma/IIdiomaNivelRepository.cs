using DataTransferObject.Domain.Nivel;
using System.Collections.Generic;

namespace Core.Domain.IIdioma
{
    public interface IIdiomaNivelRepository
    {
        public NivelDTO GetModel(NivelDTO model);
        public NivelDTO GetByDescricao(string descricao);
        public List<NivelDTO> ListaNivelIdioma();
    }
}