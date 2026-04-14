using System;

namespace Colaboracao.Infra.Repositories.Projeto
{
    public record ProjetoConstants
    {
        public const string LABEL_CLIENTE = "{0} / {1}";

        public static string RetornarLabelCliente(string codigoCliente, string nomeCliente)
        {
            bool clienteTemCodigo = !String.IsNullOrEmpty(codigoCliente);
            bool clienteTemNome = !String.IsNullOrEmpty(nomeCliente);
            if (!clienteTemCodigo)
            {
                return nomeCliente;
            }
            else if (!clienteTemNome)
            {
                return codigoCliente;
            }
            else if (!clienteTemCodigo && !clienteTemNome)
            {
                return "Cliente sem código e sem nome";
            }
            return string.Format(LABEL_CLIENTE, codigoCliente, nomeCliente);
        }
    }
}