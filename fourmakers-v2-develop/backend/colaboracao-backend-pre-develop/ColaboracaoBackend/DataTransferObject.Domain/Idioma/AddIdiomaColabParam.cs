namespace DataTransferObject.Domain.Idioma
{
    public class AddIdiomaColabParam
    {
        public int IdiomaId { get; set; }
        public string Descricao { get; set; }
        public string GestorExternoPerfil { get; set; }
        public long? NivelId { get; set; }
    }
}