using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoAreaAtuacao.AreaAtuacao
{
    public class AreaAtuacaoInput : AreaAtuacaoBase
    {
        [JsonIgnore]
        public Guid Id { get; set; }
        [JsonIgnore]
        public int OrgId { get; set; }

        public void ConfigurarParaPersistencia(Guid id, int orgId)
        {
            Id = id;
            OrgId = orgId;
        }
    }
}