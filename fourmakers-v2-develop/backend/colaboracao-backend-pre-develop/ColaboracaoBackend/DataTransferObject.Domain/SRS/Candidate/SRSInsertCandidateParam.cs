using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.SRS.Candidate
{
    public class SRSInsertCandidateParam
    {
        public SRSInsertCandidateParam()
        {
            candidate_emergency_contact = new List<SRSCandidateContatosEmergenciaDTO>();
        }

        [JsonPropertyName("isForusys")]
        public sbyte EhFoursys { get; set; }

        [JsonPropertyName("isAtivo")]
        public sbyte EhAtivo { get; set; }

        [JsonPropertyName("cand_cpf")]
        public string Cand_cpf { get; set; }

        [JsonIgnore]
        public int candidate_id { get; set; }

        [JsonPropertyName("first_name")]
        public string first_name { get; set; }

        [JsonPropertyName("dataNascimento")]
        public DateTime? dataNascimento { get; set; }

        [JsonPropertyName("cand_rg")]
        public string cand_rg { get; set; }

        [JsonPropertyName("cand_estCivil")]
        public string cand_estCivil { get; set; }

        [JsonPropertyName("fonte")]
        public string Fonte { get; set; }

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
        public string key_skills { get; set; }

        [JsonPropertyName("skills")]
        public string Skills { get; set; }

        [JsonPropertyName("methodologies")]
        public string methodologies { get; set; }

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

        [JsonPropertyName("isFumante")]
        public sbyte EhFumante { get; set; }

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

        [JsonPropertyName("tipoCargo")]
        public string TipoCargo { get; set; }

        [JsonPropertyName("site_id")]
        public int site_id { get; set; }

        [JsonPropertyName("source")]
        public string source { get; set; }

        [JsonPropertyName("last_name")]
        public string last_name { get; set; }

        [JsonPropertyName("can_relocate")]
        public int can_relocate { get; set; }

        [JsonPropertyName("entered_by")]
        public int entered_by { get; set; }

        [JsonPropertyName("owner")]
        public int owner { get; set; }

        [JsonPropertyName("date_created")]
        public DateTime date_created { get; set; }

        [JsonPropertyName("date_modified")]
        public DateTime date_modified { get; set; }

        [JsonPropertyName("import_id")]
        public int import_id { get; set; }

        [JsonPropertyName("is_hot")]
        public int is_hot { get; set; }

        [JsonPropertyName("best_time_to_call")]
        public string best_time_to_call { get; set; }

        [JsonPropertyName("cand_cpf")]
        public string cand_cpf { get; set; }

        [JsonPropertyName("cand_fumante")]
        public string cand_fumante { get; set; }

        [JsonPropertyName("source")]
        public string sorce { get; set; }

        [JsonPropertyName("is_active")]
        public int is_active { get; set; }
        
        [JsonPropertyName("notes")]
        public string notes { get; set; }
        
        [JsonPropertyName("maquina_four")]
        public string maquina_four { get; set; }
        
        [JsonPropertyName("maquina_cliente")]
        public string maquina_cliente { get; set; }
    }
}