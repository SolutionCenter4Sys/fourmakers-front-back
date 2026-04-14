using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Apontamento.FolhaPonto
{
    public class LoteFilaResult : LoteFilaFolhaDTO
    {
        
        public string Status { get; set; }

    }

    public class LoteFilaFolhaDTO : LoteFilaDTO
    {
        /// <summary>
        /// Sumário da folha de ponto
        /// </summary>
        [JsonIgnore]
        public string SumarioFolhaPontoString { get; set; }
        public SumarioFolhaPontoDTO SumarioFolhaPonto { get; set; }
    }
    public class LoteFilaDTO
    {
        /// <summary>
        /// ID único do lote
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Quantidade de páginas do PDF
        /// </summary>
        public int QuantidadeDePaginas { get; set; }

        /// <summary>
        /// Caminho do arquivo PDF no S3
        /// </summary>
        public string Pdf { get; set; }

        /// <summary>
        /// Data de criação do lote
        /// </summary>
        public DateTime DataCriacao { get; set; }

        /// <summary>
        /// Data de finalização do processamento
        /// </summary>
        public DateTime? DataFinalizacao { get; set; }

        /// <summary>
        /// Indica se o lote foi aprovado para processamento (0 = Não, 1 = Sim)
        /// </summary>
        public bool AprovadoParaProcessamento { get; set; }


    }
} 