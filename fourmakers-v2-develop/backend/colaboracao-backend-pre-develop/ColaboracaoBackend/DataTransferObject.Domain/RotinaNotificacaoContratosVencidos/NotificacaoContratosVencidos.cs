using System;
using System.Collections.Generic;

namespace DataTransferObject.Domain.RotinaNotificacaoContratosVencidos
{
    public class NotificacaoContratosVencidos
    {
        public List<ContratosPorEmail> ContratosAVencer { get; set; } = new();
        public List<ContratosPorEmail> ContratosVencidos { get; set; } = new();
    }

    public class ContratoNotificacaoDetalhe
    {
        public string NomeEmpresa { get; set; }
        public string NomeContrato { get; set; }
        public DateTime? FimContrato { get; set; }
    }

    public class ContratosPorEmail
    {
        public string Email { get; set; }
        public List<ContratoNotificacaoDetalhe> Contratos { get; set; } = new();
        public int QuantidadeContratos => Contratos?.Count ?? 0;
    }
}
