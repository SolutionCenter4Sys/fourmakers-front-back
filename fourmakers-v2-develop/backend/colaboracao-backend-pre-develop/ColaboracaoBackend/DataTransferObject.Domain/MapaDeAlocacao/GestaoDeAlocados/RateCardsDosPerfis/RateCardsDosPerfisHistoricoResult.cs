using System;

namespace DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.RateCardsDosPerfis
{
    public class RateCardsDosPerfisHistoricoResult
    {
        public string GestorExternoPerfilNome { get; set; }
        public DateTime DataAlteracao { get; set; }
        public decimal? CustoPerfil { get; set; }
        public decimal? RatecardPerfil { get; set; }
        public string DescricaoModalidadeTrabalho { get; set; }
    }
}