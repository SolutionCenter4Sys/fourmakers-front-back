using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Notificacao
{
    public class NotificacaoDTO
    {
        [JsonIgnore]
        public string Id { get; set; }
        [JsonIgnore]
        public string ColaboradorCpf { get; set; }
        public string Titulo { get; set; }
        public string Mensagem { get; set; }
        public string MensagemHtml { get; set; }
        public bool Lida { get; set; }
        public DateTime DataEnvio { get; set; }
        public DateTime? DataLeitura { get; set; }
        [JsonIgnore]
        public int? TbFuncionalidadeSistemaId { get; set; }
        [JsonIgnore]
        public int OrgId { get; set; }
        /// <summary>URL absoluta ou path relativo persistido em tb_notificacao; opcional.</summary>
        [JsonIgnore]
        public string UrlCustomizada { get; set; }
        public string Rota { get; set; }
        public string RotaCompleta { get; set; }

        public void Deconstruct(out string cpf, out string titulo, out string mensagem, out int orgId)
        {
            cpf = ColaboradorCpf;
            titulo = Titulo;
            mensagem = Mensagem;
            orgId = OrgId;
        }
    }
}