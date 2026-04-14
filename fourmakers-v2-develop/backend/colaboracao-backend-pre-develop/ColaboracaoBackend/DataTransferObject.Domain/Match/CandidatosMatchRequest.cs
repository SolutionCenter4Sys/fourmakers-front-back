using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataTransferObject.Domain.Match
{
    public class CandidatosMatchRequest
    {
        [JsonPropertyName("nomeCandidato")]
        public string NomeCandidato { get; set; }

        [JsonPropertyName("localizacaoCidade")]
        public string LocalizacaoCidade { get; set; }

        [JsonPropertyName("localizacaoEstado")]
        public string LocalizacaoEstado { get; set; }

        [JsonPropertyName("ultimaAtualizacaoPerfil")]
        public string UltimaAtualizacaoPerfil { get; set; }

        [JsonPropertyName("dataDisponibilidade")]
        public string DataDisponibilidade { get; set; }

        [JsonPropertyName("origem")]
        public List<string> Origem { get; set; }

        [JsonPropertyName("hard_skills")]
        public List<HabilidadeTecnica> HardSkills { get; set; }

        [JsonPropertyName("soft_skills")]
        public List<HabilidadeComportamental> SoftSkills { get; set; }

        [JsonPropertyName("metodologias")]
        public List<Metodologia> Metodologias { get; set; }

        [JsonPropertyName("dominios_negocio")]
        public List<DominioNegocio> DominiosNegocio { get; set; }

        [JsonPropertyName("idiomas")]
        public List<Idioma> Idiomas { get; set; }

        [JsonPropertyName("disponibilidades")]
        public List<Disponibilidade> Disponibilidades { get; set; }

        [JsonPropertyName("peso_hard_skills")]
        public double PesoHardSkills { get; set; }

        [JsonPropertyName("peso_soft_skills")]
        public double PesoSoftSkills { get; set; }

        [JsonPropertyName("peso_metodologias")]
        public double PesoMetodologias { get; set; }

        [JsonPropertyName("peso_dominios_negocio")]
        public double PesoDominiosNegocio { get; set; }

        [JsonPropertyName("peso_idiomas")]
        public double PesoIdiomas { get; set; }

        [JsonPropertyName("peso_disponibilidades")]
        public double PesoDisponibilidades { get; set; }

        [JsonPropertyName("visible_to_org_ids")]
        public List<int> VisibleToOrgIds { get; set; }

        [JsonPropertyName("numero_de_candidatos")]
        public int NumeroDeCandidatos { get; set; }
    }

    public class HabilidadeTecnica
    {
        [JsonPropertyName("nome")]
        public string Nome { get; set; }

        [JsonPropertyName("nivel")]
        public string Nivel { get; set; }

        [JsonPropertyName("obrigatoriedade")]
        public string Obrigatoriedade { get; set; }
    }

    public class HabilidadeComportamental
    {
        [JsonPropertyName("nome")]
        public string Nome { get; set; }

        [JsonPropertyName("obrigatoriedade")]
        public string Obrigatoriedade { get; set; }
    }

    public class Metodologia
    {
        [JsonPropertyName("nome")]
        public string Nome { get; set; }

        [JsonPropertyName("nivel")]
        public string Nivel { get; set; }

        [JsonPropertyName("obrigatoriedade")]
        public string Obrigatoriedade { get; set; }
    }

    public class DominioNegocio
    {
        [JsonPropertyName("nome")]
        public string Nome { get; set; }

        [JsonPropertyName("nivel")]
        public string Nivel { get; set; }

        [JsonPropertyName("obrigatoriedade")]
        public string Obrigatoriedade { get; set; }
    }

    public class Idioma
    {
        [JsonPropertyName("nome")]
        public string Nome { get; set; }

        [JsonPropertyName("nivel")]
        public string Nivel { get; set; }

        [JsonPropertyName("obrigatoriedade")]
        public string Obrigatoriedade { get; set; }
    }

    public class Disponibilidade
    {
        [JsonPropertyName("nome")]
        public string Nome { get; set; }

        [JsonPropertyName("obrigatoriedade")]
        public string Obrigatoriedade { get; set; }
    }


}
