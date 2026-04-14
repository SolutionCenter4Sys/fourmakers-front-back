using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoAreaAtuacao;
using DataTransferObject.Domain.Util;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Projeto.GestorExterno
{
    public class GestorExternoInput : GestorExternoBase
    {
        public List<GestorExternoAreaAtuacaoInput> AreasDeAtuacao { get; set; } = new List<GestorExternoAreaAtuacaoInput> { };

        [JsonIgnore]
        public int OrgId { get; set; }

        [JsonIgnore]
        public bool Ativo { get; set; } = true;

        [JsonIgnore]
        public string CodigoInternoColaboradorAlteracao { get; set; }

        [JsonIgnore]
        public string CodigoInternoColaborador { get; set; }

        public void AtualizarPropriedadesDaClasseBase(GestorExternoBase gestorExternoBase)
        {
            this.AtualizarSafeComPropriedadesDe(gestorExternoBase);
        }

        public void ConfigurarParaPersistencia(int orgId, string codigoInternoColaboradorAlteracao, string codigoInternoColaborador, string codGestorExterno = null)
        {
            OrgId = orgId;
            CodigoInternoColaboradorAlteracao = codigoInternoColaboradorAlteracao;
            CodigoInternoColaborador = codigoInternoColaborador;

            if (string.IsNullOrEmpty(CodGestorExterno))
            {
                CodGestorExterno = codGestorExterno;
            }
        }
    }
}