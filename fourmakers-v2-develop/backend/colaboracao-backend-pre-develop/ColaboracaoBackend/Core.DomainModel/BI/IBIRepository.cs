using System.Collections.Generic;

namespace Core.Domain.BI
{
    public interface IBIRepository<TModel, TFactory>
    {
        TModel SaveModel(TModel model);
        List<TModel> GeraGraficoCompetenciaEndosso(TModel model, TFactory factory);
        List<TModel> GraficoColaboradorPorDiretoria(TModel model, TFactory factory);
        List<TModel> GraficoCadastroCandidato(TModel model, TFactory factory);
        List<TModel> GraficoCompetenciaCandidato(TModel model, TFactory factory);
        List<TModel> GraficoCompetenciaColaborador(TModel model, TFactory factory);
        List<TModel> GraficoTrending(TModel model, TFactory factory);
        List<string> ListaCpfDosColaboradoresECandidatos(TModel model);
        TModel VerificaPublicoAcesso(TModel model);
    }
}