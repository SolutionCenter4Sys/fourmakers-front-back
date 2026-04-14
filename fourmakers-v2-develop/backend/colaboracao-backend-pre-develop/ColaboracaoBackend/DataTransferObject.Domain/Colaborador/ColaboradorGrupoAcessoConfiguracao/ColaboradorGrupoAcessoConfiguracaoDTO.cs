namespace DataTransferObject.Domain.Colaborador
{
    public class ColaboradorGrupoAcessoConfiguracaoDTO
    {
        public string Tabela { get; set; }
        public string Coluna { get; set; }
        public string Condicao { get; set; }
        public string Chave { get; set; }
        public string Acao { get; set; }
        public int GrupoAcessoId { get; set; }
        public int OrgId { get; set; }
    }
}
