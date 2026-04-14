using DataTransferObject.Domain;
using System;
using System.Threading.Tasks;

namespace Core.Domain
{
    public interface IAcessoUsuarioRepository
    {
        Task CriaTokenAcesso(string token, string codInternoColab, DateTime validade, TipoTokenAcessoEnum tipoToken, int orgId);
        public void FechaTokens(string codInternoColab);
        public TokenValidacaoAcessoDTO GetToken(string codInternoColab, TipoTokenAcessoEnum tipoToken, int orgId);
    }
}