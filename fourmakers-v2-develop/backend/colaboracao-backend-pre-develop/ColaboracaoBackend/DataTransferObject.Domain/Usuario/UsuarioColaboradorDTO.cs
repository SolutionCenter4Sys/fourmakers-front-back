using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Org;
using DataTransferObject.Domain.Usuario.Permissao;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Usuario
{
    public class UsuarioColaboradorDTO
    {
        public UsuarioColaboradorDTO()
        {
            Colaborador = new ColaboradorDTO();
        }

        [JsonPropertyName("nomeColaborador")]
        public string NomeColaborador { get; set; }

        public string ContatoPrincipalDDI { get; set; }
        [JsonPropertyName("contatoPrincipal")]
        public string ContatoPrincipal { get; set; }

        [JsonPropertyName("usuarioId")]
        public long UsuarioId { get; set; }

        [JsonPropertyName("cpf")]
        public string Cpf { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("colaborador")]
        public ColaboradorDTO Colaborador { get; set; }
        [JsonPropertyName("colaboradorOrg")]
        public ColaboradorOrgDTO ColaboradorOrg { get; set; }

        [JsonPropertyName("orgHierarquia")]
        public ColaboradorOrgHierarquiaDTO OrgHierarquia { get; set; }

        [JsonPropertyName("funcionalidadeSistema")]
        public List<FuncionalidadeSistemaDTO> FuncionalidadeSistema { get; set; }

        [JsonPropertyName("ultimaAlteracao")]
        public AtualizacaoCVDTO UltimaAlteracao { get; set; }
        public int OrgId { get; set; }
        public bool SouGestorDeAprovadores { get; set; }
        public bool OcultaTimeSheet { get; set; }
        public List<string> QuestionariosPreenchidos { get; set; }
        public List<string> QuestionariosAtivos { get; set; }
        public string DescricaoOrg { get; set; }
    }
}