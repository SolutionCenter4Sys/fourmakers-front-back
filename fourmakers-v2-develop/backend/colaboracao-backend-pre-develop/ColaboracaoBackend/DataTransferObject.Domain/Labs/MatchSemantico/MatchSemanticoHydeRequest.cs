namespace DataTransferObject.Domain.Labs.MatchSemantico
{
    /// <summary>
    /// Request para o endpoint best_candidates/hyde do Match Semântico (GCP).
    /// </summary>
    public class MatchSemanticoHydeRequest
    {
        public string Vaga { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string Categoria { get; set; }
        public string Origem { get; set; }
    }
}
