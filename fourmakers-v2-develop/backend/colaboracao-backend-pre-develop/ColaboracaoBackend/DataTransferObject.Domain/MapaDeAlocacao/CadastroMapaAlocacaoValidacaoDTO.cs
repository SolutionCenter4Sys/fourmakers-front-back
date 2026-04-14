using System;

namespace DataTransferObject.Domain.MapaDeAlocacao
{
    public class CadastroMapaAlocacaoValidacaoDTO
    {
        public long PeriodoAlocadoId { get; set; }
        public string CodigoColaborador { get; set; }
        public long ColaboradorAlocadoId { get; set; }
        public string CpfColaborador { get; set; }
        public string CpfSolicitante { get; set; }
        public int? CodigoTbd { get; set; }
        public string CodigoProjeto { get; set; }
        public int OrgIdAlterada { get; set; }
        public int OrgIdLogada { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public DateTime DataAlteracao { get; set; }
        public bool IncluiFimDeSemana { get; set; }
        public double? QuantidadeHoras { get; set; }

        public bool IsTbd => CodigoTbd != null && CodigoTbd > 0;
        public bool IsColaborador => !string.IsNullOrEmpty(CpfColaborador) && CpfColaborador != "null" && CpfColaborador != "0";
    }

    public static class CadastroMapaAlocacaoValidacaoDTOExtensao
    {
        public static CadastroMapaAlocacaoValidacaoDTO ToCadastroMapaAlocacaoValidacaoDTO(this CadastroMapaAlocacaoDTO mapaAlocacao, int orgId, string cpfSolicitante)
        {
            return new CadastroMapaAlocacaoValidacaoDTO
            {
                CodigoColaborador = mapaAlocacao.CodigoColaborador,
                CpfColaborador = mapaAlocacao.CpfColaborador,
                CpfSolicitante = cpfSolicitante,
                CodigoTbd = String.IsNullOrEmpty(mapaAlocacao.CodigoTbd) ? null : int.Parse(mapaAlocacao.CodigoTbd),
                CodigoProjeto = mapaAlocacao.CodigoProjeto,
                OrgIdLogada = orgId,
                DataInicio = mapaAlocacao.DataInicio,
                DataFim = mapaAlocacao.DataFim,
                QuantidadeHoras = mapaAlocacao.QuantidadeHoras
            };
        }
    }
}