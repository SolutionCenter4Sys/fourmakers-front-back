using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados
{
    public class FeedbackLabIADTO
    {
        [JsonPropertyName("id")]
        public string ID { get; set; }
        
        [Description("Contexto da pergunta. Ex: 'Benefícios', 'Alocação', 'Aderência'")]
        [JsonPropertyName("contexto")]
        public string Contexto { get; set; }
        
        [Description("Pergunta feita pelo usuário")]
        [JsonPropertyName("pergunta")]
        public string Pergunta { get; set; }
        
        [Description("Resposta dada pela IA para a pergunta")]
        [JsonPropertyName("resposta")]
        public string Resposta { get; set; }
        
        [Description("Aprovado ou não")]
        [JsonPropertyName("aprovado")]
        public bool Aprovado { get; set; }
            
        [JsonPropertyName("codigo_usuario")]
        [Description("Código do usuário que fez a pergunta")]
        public string CodigoUsuario { get; set; }
        
        [JsonPropertyName("data_requisicao")]
        public string DataRequisicao { get; set; }
    }
    
    public class FeedbackLabIAParam
    {
        [Required]
        [JsonPropertyName("contexto")]
        [Description("Contexto da pergunta. Ex: 'Benefícios', 'Alocação', 'Aderência'")]
        public string Contexto { get; set; }
        
        [Required]
        [JsonPropertyName("pergunta")]
        [Description("Pergunta feita pelo usuário")]
        public string Pergunta { get; set; }
        
        [Required]
        [JsonPropertyName("resposta")]
        [Description("Resposta dada pela IA para a pergunta")]
        public string Resposta { get; set; }
        
        [Required]
        [JsonPropertyName("aprovado")]
        [Description("Aprovado ou não")]
        public bool Aprovado { get; set; }
        
    }
    
}

