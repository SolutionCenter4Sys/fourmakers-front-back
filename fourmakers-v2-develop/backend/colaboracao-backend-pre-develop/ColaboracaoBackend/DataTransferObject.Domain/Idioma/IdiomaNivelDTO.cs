using DataTransferObject.Domain.Nivel;

namespace DataTransferObject.Domain.Idioma
{
    public class IdiomaNivelDTO
    {
        public long Id { get; set; }
        public string Descricao { get; set; }
        public NivelDTO Nivel { get; set; }
    }
}