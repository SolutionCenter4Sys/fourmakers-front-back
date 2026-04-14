using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.MapaDeAlocacao
{
    public class GetMapaAlocacaoInputParam
    {
        public string MesInicial { get; set; }
        public string AnoInicial { get; set; }
        public string MesFinal { get; set; }
        public string AnoFinal { get; set; }
        public bool? Trimestral { get; set; }
        [JsonIgnore]
        public DateTime DataInicial => new DateTime(int.Parse(AnoInicial), int.Parse(MesInicial), 1);
        [JsonIgnore]
        public DateTime DataFinal => new DateTime(int.Parse(AnoFinal), int.Parse(MesFinal), 1);
    }
}