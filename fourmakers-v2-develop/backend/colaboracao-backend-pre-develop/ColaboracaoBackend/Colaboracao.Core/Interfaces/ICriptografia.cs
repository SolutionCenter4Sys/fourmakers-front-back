namespace Colaboracao.Core
{
    public interface ICriptografia
    {
        string Encrypt(string plainText);

        string Decrypt(string encryptedText);

        string MD5Hash(string text);
    }
}