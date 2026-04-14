namespace DataTransferObject.Domain.GestaoPessoa.GestaoDesempenho.Gestor
{
    public class InserirPautaSugeridaGestorRequestDTO
    {
        public string CodigoInternoColaboradorAvaliado { get; set; }
        public string DescricaoPautaSugerida { get; set; }
    }

    public class InserirPautaSugeridaGestorResponseDTO
    {
        public string Id { get; set; }
        public string CodigoInternoColaboradorSuperior { get; set; }
        public string CodigoInternoColaboradorAvaliado { get; set; }
        public string DescricaoPautaSugerida { get; set; }
    }
}
