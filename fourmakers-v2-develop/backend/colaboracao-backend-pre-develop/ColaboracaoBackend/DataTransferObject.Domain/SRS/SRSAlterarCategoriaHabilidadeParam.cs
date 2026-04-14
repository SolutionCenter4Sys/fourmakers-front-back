namespace DataTransferObject.Domain.SRS
{
    public class SRSAlterarCategoriaHabilidadeParam
    {
        public int TipoAntigo { get; set; }
        public int TipoNovo { get; set; }
        public int IdHabilidadeAntiga { get; set; }
        public int IdHabilidadeNova { get; set; }
    }
}