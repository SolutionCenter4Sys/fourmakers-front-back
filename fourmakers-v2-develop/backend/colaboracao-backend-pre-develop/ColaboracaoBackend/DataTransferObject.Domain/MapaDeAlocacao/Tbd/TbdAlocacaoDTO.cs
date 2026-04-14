namespace DataTransferObject.Domain.MapaDeAlocacao.Tbd
{
    public class TbdAlocacaoDTO
    {
        public int CodTbdAlocado { get; set; }
        public string Descricao { get; set; }
        public string CpfGestor { get; set; }
        public string CodDiretoria { get; set; }
        public string DescricaoDiretoria { get; set; }
        public string CodDepartamento { get; set; }
        public string DescricaoDepartamento { get; set; }
        public string CodGestor { get; set; }
        public int OrgId { get; set; }
        public string DataCriacao { get; set; }
        public string DataAlteracao { get; set; }
    }
}