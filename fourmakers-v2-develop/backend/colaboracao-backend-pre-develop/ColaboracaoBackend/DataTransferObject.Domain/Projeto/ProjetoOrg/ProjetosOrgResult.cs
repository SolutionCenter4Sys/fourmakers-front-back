using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DataTransferObject.Domain.Projeto.ProjetoOrg
{
    public class ProjetosOrgResult
    {
        public string CodProjeto { get; set; }
        public string NomeProjeto { get; set; }
        public string CodClienteProjeto { get; set; }
        public string NomeCliente { get; set; }
        public string LabelCodigoCliente { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public string Status { get; set; }
        public int CodStatus { get; set; }
        public bool Prioritario { get; set; }
        public string CodOportunidade { get; set; }
        public IEnumerable<ProjetoOrgGerenteDTO> Aprovadores { get; set; }
        public int ApontSemAlocacao { get; set; }
        public int ApontSemAlocacaoOutroColab { get; set; }
    }

    public class ProjetoOrgGerenteDTO
    {
        public string Codigo { get; set; }
        public string Nome { get; set; }
        [JsonIgnore]
        public string CodProjeto { get; set; }
    }
}