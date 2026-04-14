using DataTransferObject.Domain.Formacao;
using DataTransferObject.Domain.Nivel;
using System.Collections.Generic;

namespace Core.Domain.Formacao
{
    public interface IFormacaoNivelRepository
    {
        public NivelDTO GetNivel(NivelDTO model);
        public NivelDTO GetNivelByKey(string key);
        public List<NivelDTO> ListNivel(NivelDTO model);
        public List<FormacaoNivel> ListNivelFormacao(NivelDTO model);
    }
}