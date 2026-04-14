using System;

namespace Colaborador.API.DTOs
{
    public class DadosDemograficosResult
    {
        public int? QuantidadePessoasResidencia { get; set; }
        public int? DependentesIRPF { get; set; }
        public bool? PossuiConjuge { get; set; }
        public DateTime? DataNascimentoConjuge { get; set; }
        public bool? PossuiFilhos { get; set; }
        public bool? PossuiSeguroSaude { get; set; }
        public decimal? ValorAtualSeguroSaude { get; set; }
        public string OperadoraSeguroSaude { get; set; }
        public string AcomodacaoSeguroSaude { get; set; }
        public bool? SeguroSaudePossuiCoparticipacao { get; set; }
        public string ObservacoesSeguroSaude { get; set; }
        public bool? PossuiInteressePlanoFoursys { get; set; }
        public string FaixaEtaria { get; set; }
        public string CategoriaPlanoSaude { get; set; }
        public bool? IncluirDependentesPlanoFoursys { get; set; }
        public int? QuantidadeDependentesPlanoFoursys { get; set; }
        public decimal? ValorPlanoDependentes { get; set; }
        public decimal? ValorCartaoRefeicao { get; set; }
        public decimal? ValorCartaoAlimentacao { get; set; }
        public bool? EstudaAtualmente { get; set; }
        public decimal? CustoMensalEducacao { get; set; }
        public bool? FilhosEstudamAte24Anos { get; set; }
        public decimal? CustoMensalEducacaoFilhos { get; set; }
        public decimal? CustoTotalEducacao { get; set; }
        // Nota: ModeloDeTrabalhoPretendido, DiasPresenciaisDesejados e PretencaoLiquidaRef foram removidos - devem ser atualizados em tb_candidato_vaga
        public decimal? DistanciaIdaVolta { get; set; }
    }
}
