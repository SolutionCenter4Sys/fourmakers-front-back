using System.Collections.Generic;

namespace DataTransferObject.Domain.Foursys
{
    public class ColaboradoresTotaisOrgIdResult
    {
        public List<ColaboradoresOrgIdResult> Colaboradores { get; set; }
        public List<TotaisOrgDTO> TotaisOrg { get; set; }
    }
}