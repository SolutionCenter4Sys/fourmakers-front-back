using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Social;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Social.Domain.Interfaces
{
    public interface IEncontrosBigNumbersService
    {
        Task<ApiGenericResult<EncontrosBigNumbers>> ObterBigNumbers(EncontrosBigNumbersParam param, int orgIdUsuarioLogado);
        Task<ApiGenericResult<List<EncontrosBigNumbersCategoria>>> ObterBigNumbersCategoria(EncontrosBigNumbersParam param, int orgIdUsuarioLogado);
        Task<ApiGenericResult<List<EncontrosBigNumbersObjetivo>>> ObterBigNumbersObjetivo(EncontrosBigNumbersParam param, int orgIdUsuarioLogado);
        Task<ApiGenericResult<List<AgendaRealizadaDetalhe>>> AgendaRealizadaDetalhe(EncontrosBigNumbersParam param, int orgIdUsuarioLogado);
        Task<ApiGenericResult<List<AgendaSemInteracaoDetalhe>>> AgendaSemInteracaoDetalhe(EncontrosBigNumbersParam param, int orgIdUsuarioLogado);
        Task<ApiGenericResult<List<ClientesImpactadosDetalhe>>> ClientesImpactadosDetalhe(EncontrosBigNumbersParam param, int orgIdUsuarioLogado);
        Task<ApiGenericResult<List<GestoresImpactadosDetalhe>>> GestoresImpactadosDetalhe(EncontrosBigNumbersParam param, int orgIdUsuarioLogado);
        Task<ApiGenericResult<List<CategoriaComInteracaoDetalhe>>> CategoriaComInteracaoDetalhe(EncontrosBigNumbersParam param, int orgIdUsuarioLogado);
        Task<ApiGenericResult<List<ObjetivosAgendaDetalhe>>> ObjetivosAgendaDetalhe(string objetivoIds, EncontrosBigNumbersParam param, int orgIdUsuarioLogado);
        Task<ApiGenericResult<List<AcoesEmAtrasoDetalhe>>> AcoesEmAtrasoDetalhe(EncontrosBigNumbersParam param, int orgIdUsuarioLogado);
        Task<ApiGenericResult<CategoriaEmFocoDetalhe>> CategoriaEmFocoDetalhe(int categoriaId, EncontrosBigNumbersParam param, int orgIdUsuarioLogado);
        Task<ApiGenericResult<AgendaVersaoDTO>> AtualizaVersaoApp(AtualizaVersaoAppParam param);
        Task<ApiGenericResult<AgendaVersaoDTO>> ListaVersaoApp();
    }
}