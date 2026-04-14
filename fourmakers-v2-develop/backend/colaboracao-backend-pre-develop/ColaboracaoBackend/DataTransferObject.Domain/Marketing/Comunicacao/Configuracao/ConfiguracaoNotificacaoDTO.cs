namespace DataTransferObject.Domain.Marketing.Comunicacao.Configuracao
{
    public class ConfiguracaoNotificacaoDTO
    {
        public bool NotificaWeb { get; set; }
        public bool NotificaTeams { get; set; }
        public bool NotificaEmail { get; set; }
        public bool NotificaPlataforma { get; set; }
    }
}
