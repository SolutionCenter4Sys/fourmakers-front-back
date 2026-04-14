using System;
using System.Security.Cryptography;
using System.Text;

namespace DataTransferObject.Domain.Colaborador
{
    public class BuscaColaboradorDTO
    {
        public string CpfColaborador { get; set; }
        public GrupoPesquisa Grupo { get; set; }

        public override int GetHashCode()
        {
            return BitConverter.ToInt32(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(CpfColaborador)), 0);
        }
    }
}