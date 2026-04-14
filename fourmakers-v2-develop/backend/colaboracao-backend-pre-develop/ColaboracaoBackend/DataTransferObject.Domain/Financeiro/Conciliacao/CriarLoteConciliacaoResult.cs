namespace DataTransferObject.Domain.Financeiro.Conciliacao
{
    public class CriarLoteConciliacaoResult
    {
        /// <summary>
        /// ID do lote criado
        /// </summary>
        public string LoteId { get; set; }

        /// <summary>
        /// CNPJ da empresa
        /// </summary>
        public string Cnpj { get; set; }

        /// <summary>
        /// Competência no formato MM/YYYY
        /// </summary>
        public string Competencia { get; set; }

        /// <summary>
        /// ID da organização
        /// </summary>
        public int OrgId { get; set; }

        /// <summary>
        /// Código interno do solicitante
        /// </summary>
        public string CodigoInternoSolicitante { get; set; }
    }
} 