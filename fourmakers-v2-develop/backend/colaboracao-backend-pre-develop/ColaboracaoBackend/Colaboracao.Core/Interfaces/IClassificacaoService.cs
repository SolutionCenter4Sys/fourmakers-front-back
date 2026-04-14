using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransferObject.Domain.Vaga;

namespace Colaboracao.Core.Interfaces;

public interface IClassificacaoService
{
    Task AtualizarClassificacaoProfissionalPorCodigoInternoColaboradorAsync(string codigoInternoColaborador);
    Task AtualizarClassificacaoProfissionalPorListaDeCodigoInternoColaboradorAsync(List<string> codigosInternos);
    void AtualizarClassificacaoProfissionalPorCodigoInternoColaborador(string codigoInternoColaborador);
    Task AtualizarClassificacaoPerfilAsync(Guid gestorExternoPerfilId);
    Task<string> ClassificarPerfilExtraidoAsync(PerfilExtraido perfilExtraido);
    Task AtualizarClassificacaoVagaAsync(string vagaId);
    Task AtualizarClassificacaoVagaAsync(DataTransferObject.Domain.Vaga.VagaRecrutamentoDTO vaga);
}