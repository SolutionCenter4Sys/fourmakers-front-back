namespace DataTransferObject.Domain.Projeto.ClienteOrgCrud
{
    public class ClienteOrgCrudBase
    {
        public string CodigoCliente { get; set; }
        public string NomeCliente { get; set; }
        public bool Ativo { get; set; } = true;
    }
}