using System;

namespace DataTransferObject.Domain.Marketing.Comunicacao.Publicacao
{
    public class PublicacaoAnexoInputDTO
    {
        public Guid? AnexoId { get; set; }
        public string NomeArquivo { get; set; }
        public string UrlArquivo { get; set; }
        public string Tipo { get; set; }
        public long? TamanhoBytes { get; set; }
    }
}
