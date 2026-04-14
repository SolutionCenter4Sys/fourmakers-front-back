using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Usuario;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransferObject.Domain.Base;

namespace Usuario.Domain.Interfaces.Services
{
    public interface IUsuarioService
    {
        Task<UsuarioResult> Login(string login, string senha, int orgId);
        Task<ColaboradorDTO> PermitePrimeiroAcesso(string cpf);
        Task<bool> ResetaSenhaUsuario(string cpf);
        void Logout(string login);
        Task<bool> AlteraSenha(string token, string novaSenha, string senhaAntiga);
        Task<UsuarioResult> GenerateAccessToken(string accessCode, int orgId);
        Task<UsuarioResult> GenerateAccessTokenMobile(string accessCode, int orgId);
        Task<UsuarioColaboradorDTO> ShowMe(string cpf, int orgId);
        Task<UsuarioColaboradorDTO> ConfirmaPrimeiroAcessoCandidato(string cpf, string fcmToken);
        UsuarioColaboradorDTO BuscarDadosUsuario(string cpf);
        bool ValidaAcessoGrupoFuncionalidade(string cpf, FuncionalidadeSistemaEnum funcionalidadeSistemaEnum);
        Task<List<BuscarHierarquiaResult>> BuscarHierarquia(string cpfUsuario, int orgId);
        void ConfirmaPrimeiroAcessoFourmakers(string cpf, int orgId);
        bool SeTokenSistema(string token);
        Task InsereUsuarioFourmakers(string cpf, string nomeCompleto, string email, string senha);
        void AlterarSenhaPorToken(string token, string senha);
        void EnviarEmailAlterarSenha(string email);
        void SubmeterConviteEmpresa(string cpf, string cnpj, bool confirmado);
        void InserePrestadorServicoPessoaJuridica(string cnpj, PrestadorServicoDTO prestadorServico, RegimeTributarioDTO regimeTributario, string cpf);
        void AlterarPrestadorServicoPessoaJuridica(string cnpj, PrestadorServicoDTO prestadorServico, RegimeTributarioDTO regimeTributario, string cpf);
        Task<UsuarioColaboradorDTO> ShowMeSSOCandidato(string token, int? userId = null);
        void AlterarSenhaUsuarioLogado(string senha);

        void ConfirmacaoEmail(int codigo, string cpf);

        void EnviarConfirmacaoEmail(string cpf);

        void ReenviarCodigoConfirmacao(string cpf);

        bool IsEmailValidated(string cpf, int orgId);
        string GenerateJWTToken(string email, string cpf, string codColaborador, int orgId, TipoLoginEnum tipoLogin);
        Task EnviaEmailComCodigoAcesso(string codColaboradorInterno, string nomeColaborador, string enderecoEmail, int orgId);
        Task<ApiGenericResult> EnviarEmailDenuncia(string email, string mensagem, int orgId);
    }
}