using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoAreaAtuacao;
using DataTransferObject.Domain.Projeto.GestorExterno;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.MapaAlocacao.GestaoDeAlocados
{
    public interface IGestorExternoRepository
    {
        Task<IEnumerable<GestorExternoResult>> ListarGestoresExternosTodosAsync(int orgId, bool filtrarAtivos = true);
        Task<IEnumerable<GestorExternoResult>> ListarGestoresExternosAsync(string codigoCliente, int orgId, string busca);
        Task<List<GestorExternoAreaAtuacaoResult>> ObterAreasDeAtuacaoPorGestorAsync(string codGestorExterno);
        Task<GestorExternoResult> ObterGestorExternoPorCodigoAsync(string codGestorExterno, int orgId);
        Task<GestorExternoResult> ObterGestorExternoPorEmailAsync(string email, int orgId);
        Task<GestorExternoResult> ObterGestorExternoPorNomeAsync(string nome, int orgId);
        Task<GestorExternoResult> InserirGestorExternoAsync(GestorExternoInput parametroRepositoryInput, TipoCadastrogGestorExternoEnum tipoCadastroEnum);
        Task<GestorExternoResult> AtualizarGestorExternoAsync(GestorExternoInput parametroRepositoryInput, string codGestorExternoChave, TipoCadastrogGestorExternoEnum tipoCadastroEnum, bool forcarAtualizacaoCodClienteCarga = false);
        Task<bool> DeletarGestorExternoAsync(string codGestorExterno, string codigoInternoColaborador, int orgId);
        Task AssociarAreasDeAtuacaoAoGestorExterno(string codGestorExterno, List<GestorExternoAreaAtuacaoInput> areasDeAtuacao, int orgId);
        Task<string> GerarCodigoGestorExternoAsync(int orgId);
        Task AtualizarCodColaboradorGestorExternoAsync(string codigoInternoColaborador, string codGestorExterno, int orgId);
    }
}