using System;

namespace DataTransferObject.Domain.Marketing.Comunicacao.Publicacao
{
    public class PublicacaoAnexoUploadDTO
    {
        public string NomeArquivo { get; set; }
        public string Tipo { get; set; }
        public long? TamanhoBytes { get; set; }
        public string ContentType { get; set; }
        public byte[] Conteudo { get; set; }
    }
}
