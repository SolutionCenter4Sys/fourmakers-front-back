using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Colaboracao.Helper
{
    public static class StringExtension
    {
        public static string ToNomeCompleto(this string nome, string sobrenome)
        {
            return ($"{nome} {sobrenome.ToStringOuVazio()}").Trim();
        }

        public static string RemoveMascaraCpf(this string cpf)
        {
            return cpf.Trim().Replace(".", "").Replace("-", "");
        }

        public static string RemoveMascaraCnpj(this string cnpj)
        {
            return cnpj.Replace(".", "").Replace("-", "").Replace("/", "");
        }
    }
}