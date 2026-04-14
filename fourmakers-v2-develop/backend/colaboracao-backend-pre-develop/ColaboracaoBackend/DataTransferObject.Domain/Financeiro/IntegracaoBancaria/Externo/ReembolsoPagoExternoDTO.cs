using System;

namespace DataTransferObject.Domain.Financeiro.IntegracaoBancaria.Externo
{
    public class ReembolsoPagoExternoDTO
    {
        public int CdProfissional { get; set; }
        public int CdProjeto { get; set; }
        public int DtMesReferencia { get; set; }
        public int DtAnoReferencia { get; set; }
        public int CdTipoReembolso { get; set; }
        public string DsAdicionalReembolso { get; set; }
        public DateTime DtNotaFiscal { get; set; }
        public decimal VlReembolso { get; set; }
        public string CodigoMoeda { get; set; }
    }
}
