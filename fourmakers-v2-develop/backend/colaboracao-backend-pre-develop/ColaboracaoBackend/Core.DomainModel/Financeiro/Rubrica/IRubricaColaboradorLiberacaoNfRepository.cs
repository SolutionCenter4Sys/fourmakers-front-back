using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransferObject.Domain.Financeiro.Rubrica.RubricaColaborador;
using DataTransferObject.Domain.Financeiro.Rubrica.RubricaColaboradorLiberacao;

namespace Core.Domain.Financeiro.Rubrica;

public interface IRubricaColaboradorLiberacaoNfRepository
{
    Task<List<Guid>> ListarRubricaColaboradorIdsEmLiberacaoPorVigenciaAsync(int mes, int ano, int orgId);
    Task InserirRubricaLiberacaoColaboradorAsync(int orgId, Guid rubricaColaboradorId, int mes, int ano);
    Task<List<RubricaColaboradorLiberacaoNfDTO>> ListarRubricasColaboradorLiberacaoNfPorVigenciaAsync(int? mes, int? ano, int orgId, string codigoInternoColaborador);
    Task<List<RubricaColaboradorLiberacaoNfDTO>> ListarRubricasColaboradorLiberacaoNfPorListaDeIds(List<Guid> ids);
    Task<List<RubricaColaboradorLiberacaoNfDTO>> ListarRubricasLiberacaoNaoUsadasPorVigenciaAsync(int? mes, int? ano, int orgId, string? codigoInternoColaborador, string? codigoDiretoria);
}