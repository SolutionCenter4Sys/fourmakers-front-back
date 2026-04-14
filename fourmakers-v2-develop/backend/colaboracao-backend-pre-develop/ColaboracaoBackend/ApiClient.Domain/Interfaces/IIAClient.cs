using System.Threading.Tasks;
using DataTransferObject.Domain.Financeiro.Reembolso.Solicitacao;

namespace ApiClient.Domain.Interfaces;

public interface IIAClient
{
    Task<AnaliseDocumentoSolicitacaoDTO> AnalisarDocumentoIA(string base64Image, string tokenAcesso);
}