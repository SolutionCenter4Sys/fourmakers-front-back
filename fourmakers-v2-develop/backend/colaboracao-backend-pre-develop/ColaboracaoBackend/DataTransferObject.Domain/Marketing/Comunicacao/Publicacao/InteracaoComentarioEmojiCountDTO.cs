namespace DataTransferObject.Domain.Marketing.Comunicacao.Publicacao
{
    /// <summary>
    /// Interações de um comentário agrupadas por emoji (string) com quantidade.
    /// </summary>
    public class InteracaoComentarioEmojiCountDTO
    {
        public string Emoji { get; set; }
        public int Count { get; set; }
    }
}
