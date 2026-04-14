using System;

namespace Colaboracao.Infra.Repositories.MapaAlocacao
{
    public record MapaDeAlocacaoConstants
    {
        public const string LABEL_CLIENTE_PROJETO_COMPLETA = "{0} - {1} / {2} - {3}";
        public const string LABEL_PROJETO_SEM_CLIENTE = "{0} - {1}";
        public const string LABEL_CODIGO_PROFISSIONAL_NOME_PROFISSIONAL = "{0} - {1}";
        public const string LABEL_CLIENTE_PROJETO_INCOMPLETA = "{0} / {1} - {2}";

        public static string RetornarLabelCliente(string codigoCliente, string nomeCliente, string codigoProjeto, string nomeProjeto)
        {
            bool clienteTemCodigo = !String.IsNullOrEmpty(codigoCliente);
            bool clienteTemNome = !String.IsNullOrEmpty(nomeCliente);

            if (clienteTemCodigo && !clienteTemNome)
            {
                return string.Format(LABEL_CLIENTE_PROJETO_INCOMPLETA, codigoCliente, codigoProjeto ?? "Projeto sem código", nomeProjeto ?? "Projeto sem nome");
            }
            else if (!clienteTemCodigo && clienteTemNome)
            {
                return string.Format(LABEL_CLIENTE_PROJETO_INCOMPLETA, nomeCliente, codigoProjeto ?? "Projeto sem código", nomeProjeto ?? "Projeto sem nome");
            }
            else if (!clienteTemCodigo && !clienteTemNome)
            {
                return string.Format(LABEL_PROJETO_SEM_CLIENTE, codigoProjeto ?? "Projeto sem código", nomeProjeto ?? "Projeto sem nome");
            }

            return string.Format(LABEL_CLIENTE_PROJETO_COMPLETA, codigoCliente, nomeCliente, codigoProjeto ?? "Projeto sem código", nomeProjeto ?? "Projeto sem nome");
        }
        public static string RetornarLabelProfissional(string codigo, string nome)
        {
            return string.Format(LABEL_CODIGO_PROFISSIONAL_NOME_PROFISSIONAL, codigo ?? "Sem código", nome ?? "Sem nome");
        }
    }
}