namespace DataTransferObject.Domain.SRS
{
    public class Usuario
    {
        public int user_id { get; set; }
        public string user_kenoby_id { get; set; }
        public int site_id { get; set; }
        public string user_name { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public int access_level { get; set; }
        public string approve_template_admission { get; set; }
        public int can_change_password { get; set; }
        public int is_test_user { get; set; }
        public string last_name { get; set; }
        public string first_name { get; set; }
        public int is_demo { get; set; }
        public string categories { get; set; }
        public string session_cookie { get; set; }
        public int pipeline_entries_per_page { get; set; }
        public string column_preferences { get; set; }
        public int force_logout { get; set; }
        public string title { get; set; }
        public string phone_work { get; set; }
        public string phone_cell { get; set; }
        public string phone_other { get; set; }
        public string address { get; set; }
        public string notes { get; set; }
        public string company { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip_code { get; set; }
        public string country { get; set; }
        public int can_see_eeo_info { get; set; }
        public string usu_categoria { get; set; }
        public int perfilC { get; set; }
        public string typec { get; set; }
        public int perfilRS { get; set; }
        public int perfilAV { get; set; }
        public string visualiza_vagas_unidades { get; set; }
        public int uniresp { get; set; }
        public int flg_trabalhar_vaga { get; set; }
        public int flg_sugestoes_alteracao { get; set; }
        public string template_admission_onboarding { get; set; }
    }
}