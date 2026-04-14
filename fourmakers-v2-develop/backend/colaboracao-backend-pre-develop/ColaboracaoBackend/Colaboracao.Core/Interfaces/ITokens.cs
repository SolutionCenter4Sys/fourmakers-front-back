using DataTransferObject.Domain.Usuario;

namespace Colaboracao.Core.Interfaces
{
    public interface ITokens
    {
        string ReverterToken(string token);
        string Base64(string token);
        public string DecodeBase64(string base64String);
        TipoLoginEnum GetTipoLogin(string token);
    }
}