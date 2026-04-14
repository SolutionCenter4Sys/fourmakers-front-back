using DataTransferObject.Domain.Base;

namespace DataTransferObject.Domain.Competencia
{
    public class CompetenciaResult : StatusResult
    {
        public ItemPerfilDTO Competencia { get; set; }
    }
}