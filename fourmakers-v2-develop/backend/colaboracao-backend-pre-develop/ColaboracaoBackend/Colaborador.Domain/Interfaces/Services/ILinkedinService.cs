using DataTransferObject.Domain.Base;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using DataTransferObject.Domain.Linkedin;

namespace Colaborador.Domain.Interfaces.Services
{
    public interface ILinkedinService
    {
        Task<StatusResult> SincronizarPerfilLinkedin(string profileUrl, string codInternoColaborador, bool useRapidAPI = false, string fullUrl = "");
        Task<StatusResult> SincronizarPerfilLinkedinServicoExterno(string cpfRequest, string profileUrl, string codInternoColaborador, int orgId);

        Task<ApiGenericResult<string>> SincronizarPerfilLinkedinPorDocumento(byte[] documento,string codInternoColaborador);
        Task<StatusResult> ColetarLinkedinColaborador(string linkedin, string codInternoColaborador);
        Task CadastrarSkillsComRetornoIA(string codInternoColaborador, RootIA ret, bool importacaoLote);
        Task CadastrarSkillsComRetornoIA(string codInternoColaborador, Root ret);
    }
}