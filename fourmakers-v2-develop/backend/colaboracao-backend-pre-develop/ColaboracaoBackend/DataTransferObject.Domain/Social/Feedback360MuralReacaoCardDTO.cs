namespace DataTransferObject.Domain.Social
{
    /// <summary>
    /// Uma reação (emoji) exibida em um card do mural, com quantidade de pessoas que reagiu e se o usuário logado reagiu.
    /// </summary>
    public class Feedback360MuralReacaoCardDTO
    {
        public int ReacaoId { get; set; }
        /// <summary>Identificador técnico da reação (ex: love, rocket, clap).</summary>
        public string Slug { get; set; }
        public string Emoji { get; set; }
        public int Quantidade { get; set; }
        public bool UsuarioReagiu { get; set; }
    }
}
