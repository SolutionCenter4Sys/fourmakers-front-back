using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoAreaAtuacao;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.ClienteOrg
{
    public class GestoresEPerfisDaGestaoDeAlocadosResult
    {
        public string NomeGestorExterno { get; set; }
        public string CodGestorExterno { get; set; }
        public List<GestorExternoAreaAtuacaoResult> AreasDeAtuacaoGestorExterno { get; set; } = new List<GestorExternoAreaAtuacaoResult> { };

        public Guid? GestorExternoPerfilId { get; set; }
        public string GestorExternoPerfilNome { get; set; }
        public string CodCliente { get; set; }

        public string CodigoInternoColaborador { get; set; }
        public bool PossuiLinkedin { get; set; }
        
        [JsonPropertyName("linkLinkedin")]
        public string LinkLinkedin { get; set; }
    }
}