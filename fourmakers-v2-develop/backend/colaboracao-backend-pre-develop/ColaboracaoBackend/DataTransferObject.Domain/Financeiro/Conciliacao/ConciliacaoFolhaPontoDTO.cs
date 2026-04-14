using System;

namespace DataTransferObject.Domain.Financeiro.Conciliacao
{
    public class ConciliacaoFolhaPontoDTO
    {
        /// <summary>
        /// ID único da conciliação de folha ponto
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Competência no formato MM/YYYY
        /// </summary>
        public string Competencia { get; set; }

        /// <summary>
        /// CNPJ da empresa
        /// </summary>
        public string Cnpj { get; set; }

        /// <summary>
        /// ID do lote relacionado
        /// </summary>
        public string TbLoteId { get; set; }

        /// <summary>
        /// Indica se a conciliação foi aprovada
        /// </summary>
        public bool Aprovado { get; set; }

        /// <summary>
        /// Data e hora da aprovação da conciliação
        /// </summary>
        public DateTime? DataAprovacao { get; set; }
    }
} 