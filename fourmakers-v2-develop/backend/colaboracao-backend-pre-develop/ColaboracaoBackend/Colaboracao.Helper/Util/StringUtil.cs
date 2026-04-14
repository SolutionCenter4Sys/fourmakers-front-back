using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Colaboracao.Helper
{
    public static class StringUtil
    {
        /// <summary>
        /// Método para capturar as propriedades dividas por ponto e virgula de uma string (estilo connection string)
        /// Aceita propriedades com aspas ou sem aspas
        /// Ex. string: data source=.\SQLEXPRESS;initial catalog=Target_Database;user id=User;password="My@;assword"
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        /// <exception cref="FormatException"></exception>
        public static Dictionary<string, string> ExtrairPropriedadesSeparadasPontoEVirgula(string param)
        {
            var builder = new DbConnectionStringBuilder();

            builder.ConnectionString = param;

            var dicPropriedades = new Dictionary<string, string>();

            foreach (string key in builder.Keys)
            {
                dicPropriedades.Add(key, builder[key].ToStringOuVazio());
            }

            return dicPropriedades;
        }

        public static List<string> SplitPontoEVirgula(string param)
        {
            var ret = new List<string>();
            if (!string.IsNullOrEmpty(param))
            {
                ret = new List<string>(param.Split(';', StringSplitOptions.RemoveEmptyEntries));
            }
            return ret;
        }

        public static string JuntarListaDeStrings(List<string> lista, string separador)
        {
            if (lista == null || lista.Count == 0)
            {
                return string.Empty;
            }
            return string.Join(separador, lista);
        }

        public static string GetStringAleatoria(int tamanho)
        {
            string caracteres = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789$#!@%&*";
            Random random = new Random();
            char[] stringAleatoria = new char[tamanho];
            bool temMaiuscula = false;
            bool temMinuscula = false;
            bool temNumero = false;
            bool temEspecial = false;

            while (!temMaiuscula || !temMinuscula || !temNumero || !temEspecial)
            {
                for (int i = 0; i < tamanho; i++)
                {
                    int indexAleatorio = random.Next(caracteres.Length);
                    stringAleatoria[i] = caracteres[indexAleatorio];
                }

                temMaiuscula = false;
                temMinuscula = false;
                temNumero = false;
                temEspecial = false;

                foreach (char caracter in stringAleatoria)
                {
                    if (char.IsUpper(caracter))
                    {
                        temMaiuscula = true;
                    }
                    else if (char.IsLower(caracter))
                    {
                        temMinuscula = true;
                    }
                    else if (char.IsDigit(caracter))
                    {
                        temNumero = true;
                    }
                    else if (caracter == '$' || caracter == '#' || caracter == '@' || caracter == '%' || caracter == '&' || caracter == '*')
                    {
                        temEspecial = true;
                    }
                }
            }
            return new string(stringAleatoria);
        }

        public static string GetStringNumericaAleatoria(int tamanho)
        {
            string caracteres = "0123456789";
            Random random = new Random();
            char[] stringAleatoria = new char[tamanho];

            for (int i = 0; i < tamanho; i++)
            {
                int indexAleatorio = random.Next(caracteres.Length);
                stringAleatoria[i] = caracteres[indexAleatorio];
            }
            return new string(stringAleatoria);
        }

        public static string RemoveDiacritics(string text)
        {
            string normalizedString = text.Normalize(NormalizationForm.FormD);

            var stringBuilder = new StringBuilder(normalizedString.Length);

            foreach (char c in normalizedString)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Trim();
        }
        
        public static string SomenteNumeros(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            return Regex.Replace(input, @"[^\d]", "");
        }
    }
}