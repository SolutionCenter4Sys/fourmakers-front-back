namespace DataTransferObject.Domain.Util
{
    public class OrgDTO
    {
        public long Id { get; set; }
        public string Descricao { get; set; }
        public string Subdominio { get; set; }
        public string Dominio_email { get; set; }
        public int? Prioridade { get; set; }
    }
}