using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Usuario;
using System;
using System.Text;

namespace Colaboracao.Core.Impl
{
    public class Tokens : ITokens
    {
        public string ReverterToken(string token)
        {
            try
            {
                return Criptografia.Decrypt(token).Split('|')[0];
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string Base64(string token)
        {
            try
            {
                byte[] textoAsBytes = Encoding.ASCII.GetBytes(token);
                return System.Convert.ToBase64String(textoAsBytes);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string DecodeBase64(string base64String)
        {
            try
            {
                byte[] bytes = System.Convert.FromBase64String(base64String);
                return Encoding.ASCII.GetString(bytes);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public TipoLoginEnum GetTipoLogin(string token)
        {
            try
            {
                var tipoLogin = Criptografia.Decrypt(token).Split('|')[3];
                if (tipoLogin == "1")
                    return TipoLoginEnum.SSO;
                if (tipoLogin == "2")
                    return TipoLoginEnum.SISTEMA;
                throw new Exception("Tipo desconhecido");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}