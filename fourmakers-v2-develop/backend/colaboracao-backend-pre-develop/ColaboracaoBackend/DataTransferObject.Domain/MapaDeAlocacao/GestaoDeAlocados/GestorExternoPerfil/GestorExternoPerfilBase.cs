using System;

namespace DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfil
{
    public class GestorExternoPerfilBase
    {
        public string CodGestorExterno { get; set; }
        public string NomePerfil { get; set; }
        public decimal? CustoPerfil { get; set; }
        public decimal? RatecardPerfil { get; set; }
        public string InformacoesRelevantes { get; set; }
        public Guid? PermanenciaId { get; set; }
        public Guid? ModeloTrabalhoId { get; set; }
        public string? ModeloTrabalhoDescricao { get; set; }
        public Guid? ProfissionalLocalidadeId { get; set; }
        public string? Cidade { get; set; }
        public string? Estado { get; set; }
        public string? Pais { get; set; }
        public int? HibridoDias { get; set; }
        public string? Cep { get; set; }
        public Guid? TipoEmpregoLinkedin { get; set; }
        public Guid? NivelExperienciaLinkedin { get; set; }
        public string Atribuicoes { get; set; }
    }
}