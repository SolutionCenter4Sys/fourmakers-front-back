namespace DataTransferObject.Domain.MapaDeAlocacao
{
    public class GetMapaAlocacaoInputDTO
    {
        public GetMapaAlocacaoInputDTO(GetMapaAlocacaoRecursoInputParam inputParam)
        {
            EhTbd = inputParam.EhTbd;
            MesInicial = int.Parse(inputParam.MesInicial);
            AnoInicial = int.Parse(inputParam.AnoInicial);
            MesFinal = int.Parse(inputParam.MesFinal);
            AnoFinal = int.Parse(inputParam.AnoFinal);
            Trimestral = inputParam.Trimestral;
            Idioma = inputParam.Idioma;
            HardSkill = inputParam.HardSkill;
            ColaboradorOuTbdFiltro = inputParam.ColaboradorOuTbdFiltro;
            GestorFiltro = inputParam.GestorFiltro;
            CodigoDiretoria = inputParam.CodigoDiretoria;
            NomeDiretoria = inputParam?.NomeDiretoria;
            Cursor = inputParam.Cursor;
            Limite = inputParam.Limite;

            if (inputParam.StatusHorasFiltro.HasValue
            && ((StatusHorasEnum)inputParam.StatusHorasFiltro.Value) != StatusHorasEnum.Todos) // se for TODOS deve ficar null pra não filtrar
            {
                StatusHorasFiltroEnum = (StatusHorasEnum)inputParam.StatusHorasFiltro;
            }
        }

        public string EhTbd { get; set; }
        public int MesInicial { get; set; }
        public int AnoInicial { get; set; }
        public int MesFinal { get; set; }
        public int AnoFinal { get; set; }
        public bool? Trimestral { get; set; }
        public StatusHorasEnum? StatusHorasFiltroEnum { get; set; }
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