using Competencia.Domain.Constants;
using Competencia.Domain.Enums;
using System;

namespace Competencia.Domain.Utils
{
    public static class ValidacaoCompetenciaUtil
    {
        public static string RetornarLabelObservacaoHistoricoCompetencia(AcaoHistoricoCompetenciaEnum acao, string descricaoCompetencia1, string descricaoCompetencia2)
        {
            if (acao == AcaoHistoricoCompetenciaEnum.Unificar)
            {
                return String.Format(CompetenciaHistoricoConstants.LABEL_OBSERVACAO_UNIFICAR, descricaoCompetencia1, descricaoCompetencia2);
            }
            if (acao == AcaoHistoricoCompetenciaEnum.Aprovar)
            {
                return String.Format(CompetenciaHistoricoConstants.LABEL_OBSERVACAO_APROVAR, descricaoCompetencia1);
            }
            if (acao == AcaoHistoricoCompetenciaEnum.Reprovar)
            {
                return String.Format(CompetenciaHistoricoConstants.LABEL_OBSERVACAO_REPROVAR, descricaoCompetencia1);
            }
            if (acao == AcaoHistoricoCompetenciaEnum.Adicionar)
            {
                return String.Format(CompetenciaHistoricoConstants.LABEL_OBSERVACAO_ADICIONAR, descricaoCompetencia1);
            }
            if (acao == AcaoHistoricoCompetenciaEnum.Editar)
            {
                return String.Format(CompetenciaHistoricoConstants.LABEL_OBSERVACAO_EDITAR, descricaoCompetencia1, descricaoCompetencia2);
            }
            throw new ArgumentException("Ação não encontrada.");
        }
    }
}