using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MapaDeAlocacao.Domain.Interfaces.GestaoDeAlocados
{
    public interface IParceirosService
    {
        Task<ApiGenericResult<ParceiroArchiveDTO>> BuscarArquivoPorId(string arquivoId);
        Task<ApiGenericResult<List<ParceiroArchiveDTO>>> BuscarArquivosPorParceiroId(string parceiroID);
        Task<ApiGenericResult<ParceiroArchiveDTO>> InserirArquivo(ParceiroArchiveParam param);
        Task<ApiGenericResult<ParceiroDTO>> BuscarParceiroPorId(string parceiroID);
        Task<ApiGenericResult<List<ParceiroDTO>>> BuscarTodosParceiros(int? orgId, string? filtro, string? bucket);
        Task<ApiGenericResult<ParceiroParamDTO>> InserirParceiro(ParceiroInserirParam param, string cpfUsuarioLogado);
        Task<ApiGenericResult<ParceiroParamDTO>> AtualizarParceiro(string parceiroID, ParceiroIDParam param, string cpfUsuarioLogado);
        Task<ApiGenericResult<bool>> DeletarParceiro(string parceiroID);
        Task<ApiGenericResult<ParceiroGestaoContratoDTO>> InserirContrato(ParceiroGestaoContratoParceiroIDParam param);
        Task<ApiGenericResult<ParceiroGestaoContratoDTO>> AtualizarContrato(ParceiroGestaoContratoParam param);
        Task<ApiGenericResult<bool>> DeletarContrato(string ID);
        Task<ApiGenericResult<bool>> DeletarArquivo(string ID);
        Task<ApiGenericResult<ParceiroGestaoContratoDTO>> BuscarGestaoContratoPorId(string gestaoContratoID);
        Task<ApiGenericResult<FileContentResult>> RelatorioParceriaAliancas(int orgId);
        Task<ApiGenericResult<(int Sucesso, int Erros, List<string> Mensagens)>> ImportacaoPlanilhaContrato(string caminhoArquivo, string cpfUsuarioLogado, int orgId);

    }
};
