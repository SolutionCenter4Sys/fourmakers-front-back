using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Candidato;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.MapaDeAlocacao.CargaMapaAlocacao;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiClient.Domain.Interfaces
{
    public interface IColaboradorClient
    {
        Task<ColaboradorDTO> GetColaboradorByCpf(string cpf, string tokenUsuario, bool lancarExcpNaoAutorizado = false);
        Task<bool> AlterarDadosColaborador(ColaboradorDTO colabInfo, byte[] imagem, string tokenUsuario);
        Task<ListaCandidatoResult> BuscarCandidato(string cpf, string busca, int cursor, int limite, string tokenUsuario);
        Task<StatusResult> InsereColaboradorCandidato(ColaboradorDTO colabInfo, byte[] imagem);
        Task<ColaboradorDTO> BuscarDadosColaboradorAdmin(string cpf, string tokenUsuario);
        Task<RedeColaboradorDTO> BuscarRedeColaborador(string cpf, string token);
        Task<SimpleColaboradorDTO> BuscarNomeColaborador(string cpf, string token);
        Task<List<ColaboradorDTO>> BuscarColaborador(string cpf, string busca, int cursor, int limite, string token);
        Task<StatusResult> EnviaPushNotificationRedeColaborador(string cpf, string titulo, string mensagem, string tokenUsuario);
        Task<CargaMapaAlocacaoResult> BuscarCargaMapaAlocacao(int cursor, int limit);
        Task<bool> SincronizarPerfilLinkedinServicoExterno(string codigoInternoColaborador, string profileUrl, string tokenUsuario);
    }
}