using Colaboracao.Helper;
using Colaboracao.Helper.Enum;
using System;
using System.Net;

public static class ExceptionUtil
{
    public static void GerenciarRetornoExcecao(Exception ex, CRUDEnum operacao, string descricaoEntidade)
    {
        //propagar regras de negócio
        if (ex is ApplicationException || ex is ArgumentException || ex is UnauthorizedAccessException)
        {
            throw ex;
        }

        //erro genérico se for um erro desconhecido
        throw new Exception($"Erro ao {operacao.ObterAcao()} {descricaoEntidade}", ex);
    }

    public static void NaoEncontrado(string entidade)
    {
        throw new ApplicationException($"{entidade} não encontrado.");
    }

    public static void NaoInserido(string entidade)
    {
        throw new ApplicationException($"{entidade} não foi inserido ou recuperado corretamente.");
    }

    public static void NaoAtualizado(string entidade)
    {
        throw new ApplicationException($"{entidade} não foi atualizado ou recuperado corretamente.");
    }

    public static void NaoExcluido(string entidade)
    {
        throw new ApplicationException($"{entidade} não foi excluído com sucesso.");
    }

    public static void TratarHttpStatusException(HttpStatusCode httpStatus, string mensagem, string? respostaDetalhada = null)
    {
        var detalhe = !string.IsNullOrEmpty(respostaDetalhada)
                        ? $" - {respostaDetalhada}"
                        : string.Empty;

        var mensagemCompleta = $"{mensagem.ToStringOuVazio()}{detalhe}";

        switch (httpStatus)
        {
            case HttpStatusCode.Unauthorized:
                throw new UnauthorizedAccessException(mensagem);

            case HttpStatusCode.BadRequest:
                throw new ArgumentException(mensagemCompleta);

            default:
                throw new Exception(mensagemCompleta);
        }
    }
}