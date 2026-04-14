using System;

namespace DataTransferObject.Domain.Colaborador
{
    public class ImportacaoColaboradorFilaDTO
    {
        public string IdLote { get; set; }
        public string NomeArquivo { get; set; }
        public string S3Key { get; set; } // Chave do arquivo no S3 (obrigatório)
    }
} 