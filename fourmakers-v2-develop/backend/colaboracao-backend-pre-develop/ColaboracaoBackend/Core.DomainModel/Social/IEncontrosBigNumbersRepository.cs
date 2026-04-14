using DataTransferObject.Domain.Social;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.Social
{
    public interface IEncontrosBigNumbersRepository
    {
        Task<EncontrosBigNumbers> ObterBigNumbers(EncontrosBigNumbersParam param, int orgIdUsuarioLogado);
        Task<List<EncontrosBigNumbersCategoria>> ObterBigNumbersCategoria(EncontrosBigNumbersParam param, int orgIdUsuarioLogado);
        Task<List<EncontrosBigNumbersObjetivo>> ObterBigNumbersObjetivo(EncontrosBigNumbersParam param, int orgIdUsuarioLogado);
        Task<List<AgendaRealizadaDetalhe>> AgendaRealizadaDetalhe(EncontrosBigNumbersParam param, int orgIdUsuarioLogado);
        Task<List<AgendaSemInteracaoDetalhe>> AgendaSemInteracaoDetalhe(EncontrosBigNumbersParam param, int orgIdUsuarioLogado);
        Task<List<ClientesImpactadosDetalhe>> ClientesImpactadosDetalhe(EncontrosBigNumbersParam param, int orgIdUsuarioLogado);
        Task<List<GestoresImpactadosDetalhe>> GestoresImpactadosDetalhe(EncontrosBigNumbersParam param, int orgIdUsuarioLogado);
        Task<List<CategoriaComInteracaoDetalhe>> CategoriaComInteracaoDetalhe(EncontrosBigNumbersParam param, int orgIdUsuarioLogado);
        Task<List<ObjetivosAgendaDetalhe>> ObjetivosAgendaDetalhe(string objetivoIds, EncontrosBigNumbersParam param, int orgIdUsuarioLogado);
        Task<List<AcoesEmAtrasoDetalhe>> AcoesEmAtrasoDetalhe(EncontrosBigNumbersParam param, int orgIdUsuarioLogado);
        Task<CategoriaEmFocoDetalhe> CategoriaEmFocoDetalhe(int categoriaId, EncontrosBigNumbersParam param, int orgIdUsuarioLogado);
        Task<AgendaVersaoDTO> AtualizaVersaoApp(string descricao, string versao);
        Task<AgendaVersaoDTO> ListaVersaoApp();
    }
}