namespace DataTransferObject.Domain.Competencia
{
    public class AddCompetenciaColabParam
    {
        public long Id { get; set; }
        public string Descricao { get; set; }
        public long? NivelId { get; set; }
        public string GestorExternoPerfil { get; set; }
    }
}