namespace DataTransferObject.Domain.Curadoria;

public class CuradoriaSkillResponse
{
    public string Input { get; set;  }
    public string Suggested { get; set; }
    public float Distance { get; set; }
    public CuradoriaSkillType Type { get; set; }
}

public enum CuradoriaSkillType
{
    lookup,
    embedding
}