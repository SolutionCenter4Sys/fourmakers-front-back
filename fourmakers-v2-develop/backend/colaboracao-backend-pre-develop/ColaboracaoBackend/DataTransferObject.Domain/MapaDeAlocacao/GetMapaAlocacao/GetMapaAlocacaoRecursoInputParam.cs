namespace DataTransferObject.Domain.MapaDeAlocacao
{
    public class GetMapaAlocacaoRecursoInputParam : GetMapaAlocacaoInputParam
    {
        public string EhTbd { get; set; }
        public int? StatusHorasFiltro { get; set; }
        public string Idioma { get; set; }
        public string HardSkill { get; set; }
        public string ColaboradorOuTbdFiltro { get; set; }
        public string GestorFiltro { get; set; }
        public string CodigoDiretoria { get; set; }
        public string NomeDiretoria { get; set; }
        public int Cursor { get; set; }
        public int Limite { get; set; }
    }
}