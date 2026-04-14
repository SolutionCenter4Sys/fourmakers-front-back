using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Contratacao
{
    public class EquipamentoPadraoAninhadoDTO
    {
        [JsonPropertyName("grupoArea")]
        public string GrupoArea { get; set; }

        [JsonPropertyName("cargosFuncoes")]
        public List<CargoFuncaoEquipamentoDTO> CargosFuncoes { get; set; } = new List<CargoFuncaoEquipamentoDTO>();
    }

    public class CargoFuncaoEquipamentoDTO
    {
        [JsonPropertyName("idCargoFuncao")]
        public string IdCargoFuncao { get; set; }

        [JsonPropertyName("grupoArea")]
        public string GrupoArea { get; set; }

        [JsonPropertyName("nomeCargoFuncao")]
        public string NomeCargoFuncao { get; set; }

        [JsonPropertyName("opcoesEquipamento")]
        public List<OpcaoEquipamentoDTO> OpcoesEquipamento { get; set; } = new List<OpcaoEquipamentoDTO>();
    }

    public class OpcaoEquipamentoDTO
    {
        [JsonPropertyName("tipoOpcao")]
        public string TipoOpcao { get; set; }

        [JsonPropertyName("categoriaNome")]
        public string CategoriaNome { get; set; }

        [JsonPropertyName("tipoEquipamento")]
        public string TipoEquipamento { get; set; }

        [JsonPropertyName("cpuGeracao")]
        public string CpuGeracao { get; set; }

        [JsonPropertyName("memoriaRam")]
        public string MemoriaRam { get; set; }

        [JsonPropertyName("armazenamentoDisco")]
        public string ArmazenamentoDisco { get; set; }

        [JsonPropertyName("so")]
        public string So { get; set; }

        [JsonPropertyName("gpu")]
        public string Gpu { get; set; }

        [JsonPropertyName("modelosPossiveis")]
        public List<string> ModelosPossiveis { get; set; } = new List<string>();

        [JsonPropertyName("descricaoUpgrade")]
        public string DescricaoUpgrade { get; set; }
    }
}
