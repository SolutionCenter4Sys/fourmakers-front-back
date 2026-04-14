using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Questionario;

public class GenericQuestionarioResult<T>
{
    public int Id { get; set; }
    public T Resposta { get; set; }
    [JsonIgnore]
    public string JsonTexto { get; set; }
}