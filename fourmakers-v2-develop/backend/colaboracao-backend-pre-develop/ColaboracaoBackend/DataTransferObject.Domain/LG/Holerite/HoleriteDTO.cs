using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.LG.Holerite
{
    public class HoleriteDTO
    {
        [JsonPropertyName("cpf")]
        public string Cpf { get; set; }

        [JsonPropertyName("org_id")]
        public long OrgId { get; set; }

        [JsonPropertyName("lg_pessoa_id")]
        public long LGPessoaId { get; set; }

        [JsonPropertyName("tb_colaborador_lg_id")]
        public long ColaboradorLGId { get; set; }

        [JsonPropertyName("competencia_mes")]
        public string CompetenciaMes { get; set; }

        [JsonPropertyName("competencia_ano")]
        public string CompetenciaAno { get; set; }

        [JsonPropertyName("lg_id_tarefa")]
        public string LGIdTarefa { get; set; }

        [JsonPropertyName("lg_blob_url_arquivo")]
        public string LGBlobUrlArquivo { get; set; }

        [JsonPropertyName("s3_url_arquivo")]
        public string S3UrlArquivo { get; set; }

        [JsonPropertyName("processo")]
        public int Processo { get; set; }

        [JsonPropertyName("status")]
        public int Status { get; set; }

        [JsonPropertyName("data_emissao")]
        public DateTime? DataEmissao { get; set; }

        [JsonIgnore]
        [JsonPropertyName("data_criacao")]
        public DateTime DataCriacao { get; set; }

        [JsonIgnore]
        [JsonPropertyName("data_alteracao")]
        public DateTime DataAlteracao { get; set; }
    }
}