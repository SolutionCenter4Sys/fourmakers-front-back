using DataTransferObject.Domain.Base;
using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.SRS.Vagas
{
    public class CargoDropdownItemDTO : DropDownItemDTO
    {
        [JsonPropertyName("descricaoCargo")]
        public String DescricaoCargo { get; set; }
    }
}