using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoAreaAtuacao.AreaAtuacao;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoAreaAtuacao.Permanencia;

namespace DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoAreaAtuacao
{
    public class GestorExternoAreaAtuacaoResult : GestorExternoAreaAtuacaoBase
    {
        public PermanenciaResult? Permanencia { get; set; }
        public AreaAtuacaoResult AreaDeAtuacao { get; set; }
    }
}