using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Marketing.Comunicacao.IA
{
    /// <summary>
    /// Modos válidos para refatoração de texto no assistente de análise documental.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ModoRefatoracaoTextoEnum
    {
        melhorar_texto,
        mais_profissional,
        resumo,
        sumario_executivo,
        adicionar_topicos,
        expandir_conteudo
    }
}
