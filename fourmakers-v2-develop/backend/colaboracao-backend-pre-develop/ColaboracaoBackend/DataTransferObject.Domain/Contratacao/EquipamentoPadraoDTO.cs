using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Contratacao
{
    public class EquipamentoPadraoDTO
    {
        [JsonPropertyName("mapeamentoId")]
        public Guid MapeamentoId { get; set; }

        [JsonPropertyName("grupoArea")]
        public string GrupoArea { get; set; }

        [JsonPropertyName("cargoId")]
        public Guid CargoId { get; set; }

        [JsonPropertyName("nomeCargoFuncao")]
        public string NomeCargoFuncao { get; set; }

        [JsonPropertyName("categoriaNome")]
        public string CategoriaNome { get; set; }

        [JsonPropertyName("tipoEquipamento")]
        public string TipoEquipamento { get; set; }

        [JsonPropertyName("sistemaOperacional")]
        public string SistemaOperacional { get; set; }

        [JsonPropertyName("cpuGeracao")]
        public string CpuGeracao { get; set; }

        [JsonPropertyName("gpu")]
        public string Gpu { get; set; }

        [JsonPropertyName("memoriaRam")]
        public string MemoriaRam { get; set; }

        [JsonPropertyName("armazenamentoDisco")]
        public string ArmazenamentoDisco { get; set; }

        [JsonPropertyName("modeloAprovado1IntelLenovo")]
        public string ModeloAprovado1IntelLenovo { get; set; }

        [JsonPropertyName("modeloAprovado2IntelDell")]
        public string ModeloAprovado2IntelDell { get; set; }

        [JsonPropertyName("modeloAprovado3AmdLenovo")]
        public string ModeloAprovado3AmdLenovo { get; set; }

        [JsonPropertyName("modeloAprovado4Apple")]
        public string ModeloAprovado4Apple { get; set; }

        [JsonPropertyName("descricaoUpgrade")]
        public string DescricaoUpgrade { get; set; }
    }
}
