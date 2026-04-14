namespace DataTransferObject.Domain.CRM
{
    public class VwCotacoesDTO
    {
        public int Quote_Id { get; set; }
        public string Quote_No { get; set; }
        public string Assunto { get; set; }
        public string Estagio { get; set; }
        public string Estagio_Cotacao { get; set; }
        public string Estagio_Cotacao_Pt { get; set; }
        public int Account_Id { get; set; }
        public string Nome_Conta { get; set; }
        public int? Contact_Id { get; set; }

        public string Nome_Contato { get; set; }
        public string Nome_Oportunidade { get; set; }

        public int? Potential_Id { get; set; }
        public string Unidade { get; set; }
    }
}