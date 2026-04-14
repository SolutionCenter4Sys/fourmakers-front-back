using DataTransferObject.Domain.Competencia;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Usuario
{
    public class BuscarHierarquiaResult
    {
        [JsonPropertyName("cdProfissional")]
        public string CodigoProfissional { get; set; }

        [JsonPropertyName("cpf")]
        public string Cpf { get; set; }

        [JsonPropertyName("nmProfissional")]
        public string NomeProfissional { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("cdCargo")]
        public string CodigoCargo { get; set; }

        [JsonPropertyName("nmCargo")]
        public string NomeCargo { get; set; }

        [JsonPropertyName("enumeradorDeAcesso")]
        public EnumeradorDeAcesso EnumeradorDeAcesso { get; set; }

        [JsonPropertyName("aniversário")]
        public DateTime? Aniversario { get; set; }

        [JsonPropertyName("keeper")]
        public bool Kepper { get; set; }

        [JsonPropertyName("fotoPerfil")]
        public string? FotoPerfil { get; set; }

        [JsonPropertyName("hardSkills")]
        public List<CompetenciaColaboradorDTO> Competencias { get; set; }

        [JsonPropertyName("projetosRecurso")]
        public List<string> NomeProjetosRecurso { get; set; }
    }
}