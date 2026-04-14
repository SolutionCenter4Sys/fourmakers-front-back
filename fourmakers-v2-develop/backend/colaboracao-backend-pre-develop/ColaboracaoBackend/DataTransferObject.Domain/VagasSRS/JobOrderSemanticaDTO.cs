
using System.Collections.Generic;

namespace DataTransferObject.Domain.VagasSRS;

public class JobOrderSemanticaDTO
{
    public int JoborderId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int? CargoId { get; set; }
    public string Cargo { get; set; }
    public string StackPrincipal { get; set; }
    public string Skills { get; set; }
    public List<JobOrderSkillDTO> JobOrderSkills { get; set; }
}