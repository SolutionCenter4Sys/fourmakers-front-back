using System;

namespace DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.RateCardsDosPerfis
{
    public class RateCardsDosPerfisResult
    {
        public string NomeGestorExterno { get; set; }
        public string GestorExternoPerfilNome { get; set; }
        public Guid? GestorExternoPerfilId { get; set; }
        public decimal? RatecardPerfil { get; set; }
        public decimal? CustoPerfil { get; set; }
    }
}