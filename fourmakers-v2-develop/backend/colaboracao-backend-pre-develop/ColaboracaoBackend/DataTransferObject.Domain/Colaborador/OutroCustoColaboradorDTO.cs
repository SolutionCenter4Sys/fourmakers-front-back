using System;

namespace DataTransferObject.Domain.Colaborador
{
    public class OutroCustoColaboradorDTO
    {
        public string Id { get; set; } // GUID (null para novo registro)
        public string Descricao { get; set; } // Descrição do custo
        public decimal Valor { get; set; } // Valor do custo (R$)
    }
}

