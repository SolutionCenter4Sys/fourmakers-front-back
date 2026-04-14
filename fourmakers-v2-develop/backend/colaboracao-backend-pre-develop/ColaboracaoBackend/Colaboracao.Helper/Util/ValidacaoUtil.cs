using System;
using System.Runtime.CompilerServices;

namespace Colaboracao.Helper
{
    public static class ValidacaoUtil
    {
        public static void ValidaMesAno(string mes, string ano)
        {
            ValidaMesAno(mes.ToIntOuZero(), ano.ToIntOuZero());
        }

        public static void ValidaMesAno(int mes, int ano)
        {
            var mensagem = string.Empty;

            if (mes < 1 || mes > 12)
            {
                mensagem = "Mês informado inválido.";
            }

            if (ano < 1000 || ano > 9999)
            {
                mensagem += (mensagem == string.Empty ? "" : " ") + "Ano informado inválido.";
            }

            if (mensagem != string.Empty)
            {
                throw new ApplicationException(mensagem);
            }
        }

        public static void ObrigaStringNotNullOrEmpty(string value, [CallerArgumentExpression("value")] string paramName = null)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException($"O campo '{paramName}' não pode ser nulo nem vazio.", paramName);
            }
        }

        public static void ObrigaIntNotNullOrZero(int? value, [CallerArgumentExpression("value")] string paramName = null)
        {
            if (!value.HasValue || value == 0)
            {
                throw new ArgumentException($"O campo '{paramName}' não pode ser nulo nem zero.", paramName);
            }
        }

        public static void ObrigaMesAno(int mes, int ano)
        {
            if (mes == 0 || ano == 0)
            {
                throw new ArgumentException("Os parâmetros mês e ano são obrigatórios e devem ser diferentes de zero.");
            }
        }

        public static void ObrigaCursorLimite(int cursor, int limite)
        {
            if (cursor < 0 || limite < 1)
            {
                throw new ApplicationException("Cursor e Limite são obrigatórios");
            }
        }
    }
}