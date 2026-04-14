using DataTransferObject.Domain.Usuario;
using System;
using Usuario.Domain.Interfaces.Factorys;

namespace Usuario.Domain.Interfaces.Models
{
    public interface IUsuarioModel
    {
        long Id { get; set; }
        UsuarioColaboradorDTO Usuario { get; set; }
        string Login { get; set; }
        string Senha { get; set; }
        sbyte PrimeiroAcessoRealizado { get; set; }
        DateTime? DataAceiteTermo { get; set; }
        string FcmToken { get; set; }
        sbyte Ativo { get; set; }
        string Token { get; set; }
        int Sistemico { get; set; }
        DateTime? DataExpiracao { get; set; }
        int OrgId { get; set; }

        IUsuarioModel GetUserByLogin(string login, IUsuarioDomainFactory factory, int orgId = 0);
        IUsuarioModel GetUserByLoginAndOrg(string login, int orgId, IUsuarioDomainFactory factory);
        IUsuarioModel GetUserByCpf(string cpf);
        IUsuarioModel GetUserByCpfAndOrg(string cpf, int OrgId);
        void Logout(string login, IUsuarioDomainFactory _usuarioFactory);
        IUsuarioModel UpdateModel();
        IUsuarioModel SaveModel();
    }
}