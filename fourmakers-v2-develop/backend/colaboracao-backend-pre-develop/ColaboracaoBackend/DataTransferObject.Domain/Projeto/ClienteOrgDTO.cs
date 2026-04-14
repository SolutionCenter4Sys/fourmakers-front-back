namespace DataTransferObject.Domain.Projeto
{
    public class ClienteOrgDTO
    {
        public string CodigoCliente { get; set; }
        public string NomeCliente { get; set; }
        public string LabelCodigoCliente { get; set; }
        public bool? Ativo { get; set; } = true;
    }
}