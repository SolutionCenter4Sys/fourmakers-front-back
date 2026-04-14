using Competencia.Domain.Enums;

namespace DataTransferObject.Domain.Competencia.GestaoDeCompetencia
{
    public class AlterarCategoriaHabilidadeParam
    {
        public TipoCompetenciaSRSEnum CategoriaAtual { get; set; }
        public int Id { get; set; }
        public TipoCompetenciaSRSEnum CategoriaDestino { get; set; }
    }
}