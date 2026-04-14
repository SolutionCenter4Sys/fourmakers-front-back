using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoAreaAtuacao.AreaAtuacao;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoAreaAtuacao.Permanencia;

namespace DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoAreaAtuacao
{
    public class GestorExternoAreaAtuacaoInput : GestorExternoAreaAtuacaoBase
    {
        public string CodGestorExterno { get; set; }
        public PermanenciaInput? Permanencia { get; set; }
        public AreaAtuacaoInput AreaDeAtuacao { get; set; }
    }
}