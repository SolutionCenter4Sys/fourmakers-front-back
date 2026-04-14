using DataTransferObject.Domain.Apontamento;
using System.Collections.Generic;

namespace Apontamento.Domain.Interfaces.Service
{
    public interface IApontamentoCacheService
    {
        List<StatusApontamentoResult> ObterListaStatusCache(string idioma);
        TemplateSemanaVigenciaResult ObterTemplateSemanaVigenciaCache(int mes, int ano, int diaQuebraSemana, string idioma, int orgId);
        void AtualizarTemplateSemanaVigenciaCache(int mes, int ano, int diaQuebraSemana, TemplateSemanaVigenciaResult templateSemanaVigencia, string idioma, int orgId);
        void AtualizarListaStatusCache(List<StatusApontamentoResult> listaStatus, string idioma);
        List<FeriadoDTO> ObterListaFeriadoCache(int orgId);
        void AtualizarListaFeriadoCache(List<FeriadoDTO> listaFeriados, int orgId);
    }
}