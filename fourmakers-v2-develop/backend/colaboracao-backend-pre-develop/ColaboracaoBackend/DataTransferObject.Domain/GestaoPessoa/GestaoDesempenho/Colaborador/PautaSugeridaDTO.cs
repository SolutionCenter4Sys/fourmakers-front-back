namespace DataTransferObject.Domain.GestaoPessoa.GestaoDesempenho.Colaborador
{
    public class InserirPautaSugeridaColaboradorRequestDTO
    {
        public string CodigoInternoColaboradorSuperior { get; set; }
        public string DescricaoPautaSugerida { get; set; }
    }

    public class InserirPautaSugeridaColaboradorResponseDTO
    {
        public string Id { get; set; }
        public string CodigoInternoColaboradorSuperior { get; set; }
        public string CodigoInternoColaboradorAvaliado { get; set; }
        public string DescricaoPautaSugerida { get; set; }
    }
}
