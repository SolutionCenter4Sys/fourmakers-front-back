using System.Collections.Generic;

namespace DataTransferObject.Domain
{
    public class ListaSkillDTO
    {
        public List<string> Dominio { get; set; }
        public List<string> Hardskill { get; set; }
        public List<string> Metodologia { get; set; }
        public List<string> Nao_classificada { get; set; }
        public List<string> Softskill { get; set; }
    }
}