using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Apontamento.ColaboradorEApontamento
{
    public class ColaboradorEApontamentoResult
    {
        public string ColaboradorNome { get; set; }

        public string ColaboradorCPF { get; set; }
        public bool Ativo { get; set; }
        public string DataAdmissao { get; set; }
        public string DataInativacao { get; set; }

        public string NomeCompletoGerente { get; set; }

        public string CodigoGerente { get; set; }

        public int CodigoStatusApontamentoPeriodo { get; set; }

        public string DescricaoStatusApontamento { get; set; }

        public decimal SomaHoras { get; set; }

        // TODO: retirar este campo observacao, pois não retorna mais com a proc, precisa verificar se quebra front
        public string Observacao { get; set; }

        //public List<ApontamentoMensalDTO> Apontamentos { get; set; }

        public string Projeto { get; set; }

        //public string[] Aprovadores { get; set; }

        public List<AprovadorResult> Aprovadores { get; set; }
        [JsonIgnore]
        [JsonPropertyName("cod_projeto")]
        public string CodProjeto { get; set; }
    }

    public class AprovadorResult
    {
        public string NomeAprovador { get; set; }
        public string CodigoInternoAprovador { get; set; }
        [JsonIgnore]
        public string CodigoProjeto { get; set; }
    }
}