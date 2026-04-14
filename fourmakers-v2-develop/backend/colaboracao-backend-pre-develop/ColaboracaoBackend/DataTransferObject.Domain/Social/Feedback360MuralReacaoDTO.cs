namespace DataTransferObject.Domain.Social
{
    /// <summary>
    /// Opção de emoji para reação nos cards do mural de reconhecimento (coração, foguete, palmas, etc.).
    /// </summary>
    public class Feedback360MuralReacaoDTO
    {
        public int Id { get; set; }
        /// <summary>Identificador técnico da reação (ex: love, rocket, clap).</summary>
        public string Slug { get; set; }
        /// <summary>Emoji exibido no front (ex: ❤️, 🚀).</summary>
        public string Emoji { get; set; }
        public bool Ativo { get; set; }
    }
}
