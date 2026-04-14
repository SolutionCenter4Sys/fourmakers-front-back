using System.Collections.Generic;

namespace DataTransferObject.Domain.Questionario;

public class QuestionarioParam
{
    public string CodigoQuestionario { get; set; }
    public string QuestionarioJson { get; set; }
    public List<QuestionarioFileHash> FileHashes { get; set; }
}