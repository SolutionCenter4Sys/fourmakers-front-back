using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.SRS.Candidate
{
    public class SRSCandidateDTO
    {
        public SRSCandidateDTO()
        {
            candidate_emergency_contact = new List<SRSCandidateContatosEmergenciaDTO>();
        }

        [JsonPropertyName("candidate_id")]
        public int candidate_id { get; set; }

        [JsonPropertyName("first_name")]
        public string first_name { get; set; }

        [JsonPropertyName("dataNascimento")]
        public DateTime? dataNascimento { get; set; }

        [JsonPropertyName("cand_rg")]
        public string cand_rg { get; set; }

        [JsonPropertyName("cand_estCivil")]
        public string cand_estCivil { get; set; }

        [JsonPropertyName("phone_home")]
        public string phone_home { get; set; }

        [JsonPropertyName("phone_cell")]
        public string phone_cell { get; set; }

        [JsonPropertyName("address")]
        public string address { get; set; }

        [JsonPropertyName("address_number")]
        public string address_number { get; set; }

        [JsonPropertyName("address_complement")]
        public string address_complement { get; set; }

        [JsonPropertyName("district")]
        public string district { get; set; }

        [JsonPropertyName("city")]
        public string city { get; set; }

        [JsonPropertyName("state")]
        public string state { get; set; }

        [JsonPropertyName("zip")]
        public string zip { get; set; }

        [JsonPropertyName("key_skills")]
        public List<string> key_skills { get; set; }

        [JsonPropertyName("methodologies")]
        public List<string> methodologies { get; set; }

        [JsonPropertyName("email1")]
        public string email1 { get; set; }

        [JsonPropertyName("email2")]
        public string email2 { get; set; }

        [JsonPropertyName("emailFoursys")]
        public string emailFoursys { get; set; }

        [JsonPropertyName("desired_pay")]
        public decimal? desired_pay { get; set; }

        [JsonPropertyName("current_pay")]
        public decimal? current_pay { get; set; }

        [JsonPropertyName("cand_Lkdin")]
        public string cand_Lkdin { get; set; }

        [JsonPropertyName("cand_skype")]
        public string cand_skype { get; set; }

        [JsonPropertyName("instagram")]
        public string instagram { get; set; }

        [JsonPropertyName("facebook")]
        public string facebook { get; set; }

        [JsonPropertyName("twitter")]
        public string twitter { get; set; }

        [JsonPropertyName("disponibilidade")]
        public string disponibilidade { get; set; }

        [JsonPropertyName("zona")]
        public string zona { get; set; }

        [JsonPropertyName("cand_modalidade")]
        public string cand_modalidade { get; set; }

        [JsonPropertyName("cand_eng_level")]
        public string cand_eng_level { get; set; }

        [JsonPropertyName("cand_esp_level")]
        public string cand_esp_level { get; set; }

        [JsonPropertyName("cand_other_level")]
        public string cand_other_level { get; set; }

        [JsonPropertyName("indicacao_4makers")]
        public string indicacao_4makers { get; set; }

        [JsonPropertyName("cand_filhos")]
        public int cand_filhos { get; set; }

        [JsonPropertyName("school_level")]
        public string school_level { get; set; }

        [JsonPropertyName("genre")]
        public string genre { get; set; }

        [JsonPropertyName("sexual_orientation")]
        public string sexual_orientation { get; set; }

        [JsonPropertyName("ethnicity")]
        public string ethnicity { get; set; }

        [JsonPropertyName("refugee_person")]
        public sbyte refugee_person { get; set; }

        [JsonPropertyName("uniresp")]
        public string uniresp { get; set; }

        [JsonPropertyName("cand_residentes")]
        public string cand_residentes { get; set; }

        [JsonPropertyName("cand_familiares")]
        public string cand_familiares { get; set; }

        [JsonPropertyName("pcd")]
        public sbyte pcd { get; set; }

        [JsonPropertyName("cand_gruporisco")]
        public string cand_gruporisco { get; set; }

        [JsonPropertyName("cand_saude")]
        public string cand_saude { get; set; }

        [JsonPropertyName("candidate_emergency_contact")]
        public List<SRSCandidateContatosEmergenciaDTO> candidate_emergency_contact { get; set; }

        [JsonPropertyName("source")]
        public string source { get; set; }

        [JsonPropertyName("file")]
        public string file { get; set; }
        
    }
}