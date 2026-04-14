using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Certificado;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Formacao;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiClient.Domain.Interfaces
{
    public interface IFormacaoClient
    {
        Task<FormacaoDTO> GetFormacaoById(long id, string tokenUsuario);
        Task<List<FormacaoColaboradorDTO>> ListarFormacoesColaborador(string cpf, string tokenUsuario);
        Task<List<ItemPerfilResult>> InserirFormacaoColaborador(string token, List<AdicionarRemoverItemDTO> dtos);
        Task<StatusResult> RemoverFormacaoColaborador(string token, string cpf, long id);
        Task<CertificadoDTO> InserirCertificadoFormacaColaborador(string cpf, long formacaoColaboradorId, byte[] file, TipoCertificadoEnum tipo, string tokenUsuario);
        Task<ItemPerfilResult> AlterarFormacaoColaborador(AdicionarRemoverItemDTO dtos, string token);
        Task<StatusResult> MergeFormacaoColaborador(MergeItemPerfilDTO dtos, string token);
    }
}