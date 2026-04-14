using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.MapaAlocacao.GestaoDeAlocados
{

    public interface IParceirosRepository
    {
        Task<ParceiroArchiveDTO> BuscarArquivoPorId(string arquivoId);
        Task<List<ParceiroArchiveDTO>> BuscarArquivosPorParceiroId(string parceiroID);

        Task<ParceiroArchiveDTO> InserirArquivo(ParceiroArchiveParam param, string url);

        // ──────────────── Parceiro CRUD ────────────────
        Task<ParceiroDTO> BuscarParceiroPorId(string parceiroID);
        Task<List<ParceiroDTO>> BuscarTodosParceiros(int? orgId, string? filtro, string? bucket);
        Task<ParceiroParamDTO> InserirParceiro(ParceiroInserirParam param, string cpfUsuarioLogado);
        Task<ParceiroParamDTO> AtualizarParceiro(string parceiroID, ParceiroIDParam param, string cpfUsuarioLogado);
        Task<bool> DeletarParceiro(string parceiroID);
        Task<ParceiroGestaoContratoDTO> InserirContrato(ParceiroGestaoContratoParceiroIDParam param);
        Task<ParceiroGestaoContratoDTO> AtualizarContrato(ParceiroGestaoContratoParam param);
        Task<bool> DeletarContrato(string ID);
        Task<bool> DeletarArquivo(string ID);
        Task<ParceiroGestaoContratoDTO> BuscarGestaoContratoPorId(string gestaoContratoID);
        Task<List<dynamic>> RelatorioParceriaAliancas(int orgId);
        Task<(int Sucesso, int Erros, List<string> Mensagens)> ImportacaoPlanilhaContrato(string caminhoArquivo, string cpfUsuarioLogado, int orgId);
    }
}
