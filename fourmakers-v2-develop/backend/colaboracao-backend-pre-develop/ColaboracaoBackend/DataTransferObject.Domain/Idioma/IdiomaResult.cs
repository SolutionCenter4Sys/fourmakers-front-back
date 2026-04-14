using DataTransferObject.Domain.Base;

namespace DataTransferObject.Domain.Idioma
{
    public class IdiomaResult : StatusResult
    {
        public int IdiomaId { get; set; }
        public IdiomaDTO Idioma { get; set; }
    }
}