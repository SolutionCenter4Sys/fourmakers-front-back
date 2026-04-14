using DataTransferObject.Domain.Usuario;
using System;

namespace Core.Domain.Usuario
{
    public interface IUsuarioExternoRepository
    {
        UsuarioColaboradorDTO GetUserByEmailSenha(string email, string senhaMD5, int orgId);
        UsuarioColaboradorDTO GetUserByCPFSenha(string cpf, string senhaMD5, int orgId);
        UsuarioColaboradorDTO GetUserByEmail(string email, int orgId);
        bool IfPrimeiroAcessoUsuario(long usuarioId);
        DateTime? GetDataAceiteTermo(long usuarioId);
        void AlteraSenhaUsuario(long usuarioId, string password);
        void SavePedidoReseteSenha(string token, long usuarioId, DateTime validade);
        long GetUsuarioFromTokenReseteSenha(string token);
        void DeletePedidoReseteSenha(string token);
        void SubmeterConvite(string cpf, string cnpj, bool confirmado);
        long AlterarSenhaUsuarioLogado(string cpf);
        ConfirmacaoEmailDTO RetornaEmailByCpf(string cpf);
        bool SalvarAtualizarConfirmacaoEmail(int codigo, ConfirmacaoEmailDTO confirmacaoEmailDTO, bool reenviar);
        UsuarioColaboradorDTO GetUserByCpfEOrgId(string cpf, int orgId);
        ConfirmacaoEmailDTO GetConfirmacaoEmailByCpf(string cpf);
        bool RetornaExisteEmail(string email, int orgId);
    }
}