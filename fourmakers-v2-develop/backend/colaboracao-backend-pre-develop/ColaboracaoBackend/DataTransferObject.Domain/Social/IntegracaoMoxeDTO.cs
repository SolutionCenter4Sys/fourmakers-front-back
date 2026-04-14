using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Social
{
    public class IntegracaoMoxeRequestDTO
    {
        public AgendaMoxeDTO Agenda { get; set; }
        public InteracaoMoxeDTO Interacao { get; set; }
        public ClienteMoxeDTO Cliente { get; set; }
        public List<string> EquipeFourtalentsParticipante { get; set; }
    }

    public class AgendaMoxeDTO
    {
        public string Titulo { get; set; }
        public string TipoInteracao { get; set; }
        public string Data { get; set; }
        public string Horario { get; set; }
        public string Local { get; set; }
        public string Descricao { get; set; }
        public int Vagas { get; set; }
    }

    public class InteracaoMoxeDTO
    {
        public string Ata { get; set; }
        public string PrincipaisPontosAudio { get; set; }
        
        public string Categoria { get; set; }
        
        public string SubCategoria { get; set; }
        
    }
    

    public class ClienteMoxeDTO
    {
        public string Empresa { get; set; }
        public string Codigo { get; set; }
        public int QtdAlocados { get; set; }
        public List<GestorMoxeDTO> Gestores { get; set; }
    }

    public class GestorMoxeDTO
    {
        public string Nome { get; set; }
        public string Email { get; set; }
    }

    public class MoxeAiRequestDTO
    {
        [JsonPropertyName("engine")]
        public string Engine { get; set; } = "azure";
        
        [JsonPropertyName("model")]
        public string Model { get; set; } = "gpt-4o-mini";
        
        [JsonPropertyName("prompt")]
        public string Prompt { get; set; }
        
        [JsonPropertyName("image_base64")]
        public string Image_base64 { get; set; }
        
        [JsonPropertyName("system_prompt")]
        public string System_prompt { get; set; }
        
        [JsonPropertyName("temperature")]
        public double Temperature { get; set; } = 0.1;
        
        [JsonPropertyName("max_tokens")]
        public int Max_tokens { get; set; } = 1000;
    }

    public class MoxeAiResponseDTO
    {
        public string Resumo { get; set; }
        public List<string> Passos { get; set; }
    }

    public class MoxeApiRawResponseDTO
    {
        [JsonPropertyName("result")]
        public string Result { get; set; }
        
        [JsonPropertyName("engine")]
        public string Engine { get; set; }
        
        [JsonPropertyName("model")]
        public string Model { get; set; }
        
        [JsonPropertyName("tokens_used")]
        public int TokensUsed { get; set; }
    }

    public class IntegracaoMoxeResponseDTO
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
        public MoxeAiResponseDTO Dados { get; set; }
    }

    public sealed class NomeEmailLookupRow
    {
        public string Nome { get; set; }
        public string Email { get; set; }
    }



}
