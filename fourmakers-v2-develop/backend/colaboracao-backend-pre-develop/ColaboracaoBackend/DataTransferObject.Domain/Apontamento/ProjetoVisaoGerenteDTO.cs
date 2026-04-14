namespace DataTransferObject.Domain.Apontamento
{
    public class ProjetoVisaoGerenteDTO
    {
        public int Mes { get; set; }
        public int Ano { get; set; }
        public string CodProjeto { get; set; }
        public string NomeProjeto { get; set; }
        public string DataFimProjeto { get; set; }
        public string CpfColaborador { get; set; }
        public string NomeColaborador { get; set; }
        public bool Ativo { get; set; }
        public string DataInativacao { get; set; }
        public string DataAdmissao { get; set; }
        public string NomeGestorAdm { get; set; }
        public int OrgId { get; set; }
        public int CodStatusMensal { get; set; }
        public string DescricaoStatusMensal { get; set; }
        public int TotalHoras { get; set; }
        public string CodCliente { get; set; }
        public string NomeCliente { get; set; }
    }
}