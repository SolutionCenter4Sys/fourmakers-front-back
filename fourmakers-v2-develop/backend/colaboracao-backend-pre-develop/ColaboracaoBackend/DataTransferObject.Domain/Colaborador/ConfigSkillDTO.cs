namespace DataTransferObject.Domain.Colaborador
{
    /// <summary>
    /// DTO que contém as configurações de tabelas e campos para cada tipo de skill
    /// </summary>
    public class ConfigSkillDTO
    {
        public string TabelaSkill { get; set; }
        public string TabelaColaborador { get; set; }
        public string CampoId { get; set; }
        public ItemCVEnum? ItemCV { get; set; }
    }
}

