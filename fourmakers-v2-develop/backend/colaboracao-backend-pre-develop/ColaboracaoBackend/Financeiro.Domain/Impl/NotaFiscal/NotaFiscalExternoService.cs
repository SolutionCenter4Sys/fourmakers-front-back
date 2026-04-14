using ApiClient.Domain;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Colaboracao.Helper.Util;
using Core.Domain.Financeiro.NotaFiscal;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Financeiro.NotaFiscal;
using DataTransferObject.Domain.Util.Enum;
using Financeiro.Domain.Interfaces.NotaFiscal;
using Logs.Infra.Attributes;

namespace Financeiro.Domain.Impl.NotaFiscal;

[LogDomainClass]
public class NotaFiscalExternoService(
    INotaFiscalRepository _notaFiscalRepository,
    INotaFiscalService _notaFiscalService,
    ITokenSistemaService _tokenSistemaService) : INotaFiscalExternoService
{
    public async Task<ApiGenericResult<IEnumerable<NotaFiscalResult>>> ProcessarNotaFiscalAprovadaAsync(
    string tokenSistema,
    string competencia,
    bool atualizarParaPago,
    string? codDiretoria,
    string? documentoColaborador,
    string? codigoColaboradorExternoAprovador = "",
    string numeroNf = "")
    {
        var result = new ApiGenericResult<IEnumerable<NotaFiscalResult>>();

        int orgId = _tokenSistemaService.GetOrgTokenSistemaWithValidatingOrgs(
            tokenSistema,
            EnumORG.NUMEN_4,
            EnumORG.ROYAL_9);

        var competenciaNormalizada = competencia.Replace("-", "/");
        var partesCompetencia = competenciaNormalizada.Split('/');
        int mes = int.Parse(partesCompetencia[0]);
        int ano = int.Parse(partesCompetencia[1]);

        var notasFiscaisAprovadasGenericResult = await _notaFiscalService.ListarNotasFiscaisPorVigenciaAsync(
            null,
            NotaFiscalStatusEnum.NF_APROVADA,
            mes,
            ano,
            codDiretoria,
            documentoColaborador,
            null,
            0,
            int.MaxValue,
            orgId,
            null,
            numeroNf);

        if (!notasFiscaisAprovadasGenericResult.Sucesso)
        {
            return notasFiscaisAprovadasGenericResult;
        }

        var notasFiscaisList = notasFiscaisAprovadasGenericResult.Retorno.ToList();

        if (atualizarParaPago && notasFiscaisList.Any())
        {
            string codigoInternoColaboradorAprovador = "";

            if (string.IsNullOrWhiteSpace(codigoColaboradorExternoAprovador))
            {
                result.Sucesso = false;
                result.Mensagem = "Código do colaborador externo aprovador é obrigatório quando atualizarStatusParaPago = true.";
                return result;
            }

            codigoInternoColaboradorAprovador = await _notaFiscalRepository.BuscarCodigoInternoColaboradorPorExterno(codigoColaboradorExternoAprovador, orgId);

            if (string.IsNullOrWhiteSpace(codigoInternoColaboradorAprovador))
            {
                result.Sucesso = false;
                result.Mensagem = "Código do colaborador externo aprovador não encontrado para esta organização.";
                return result;
            }

            var param = new NotaFiscalStatusUpdateParam
            {
                Ids = notasFiscaisList.Select(nf => nf.Id).ToList()
            };

            await _notaFiscalService.GerarPagamentoDeSolicitacoesPorIds(param, codigoInternoColaboradorAprovador, orgId, false);

            result.Sucesso = true;
            result.Mensagem = "Processamento de notas fiscais aprovadas efetuado com sucesso.";
            result.Retorno = notasFiscaisList;
            return result;
        }

        result.Sucesso = true;
        result.Mensagem = "Consulta a listagem de notas fiscais aprovadas efetuada com sucesso.";
        result.Retorno = notasFiscaisList;
        return result;
    }


    public async Task<ApiGenericResult<LiberarEmissaoDeNfsResult>> LiberarEmissaoDeNotasFiscaisPorVigenciaExterno(string tokenSistema, string competencia, string? codigoDiretoria, bool enviarEmail, string codigoColaboradorExternoEmissao)
    {
        int orgId = _tokenSistemaService.GetOrgTokenSistemaWithValidatingOrgs(tokenSistema, EnumORG.NUMEN_4, EnumORG.ROYAL_9);

        var competenciaNormalizada = competencia.Replace("-", "/");
        var partesCompetencia = competenciaNormalizada.Split('/');
        var mes = int.Parse(partesCompetencia[0]);
        var ano = int.Parse(partesCompetencia[1]);

        string codigoInternoColaboradorEmissao = "";

        if (string.IsNullOrWhiteSpace(codigoColaboradorExternoEmissao))
            throw new ArgumentException("Código do colaborador externo emissor é obrigatório.");
        else
        {
            codigoInternoColaboradorEmissao = await _notaFiscalRepository.BuscarCodigoInternoColaboradorPorExterno(codigoColaboradorExternoEmissao, orgId);

            if (string.IsNullOrWhiteSpace(codigoInternoColaboradorEmissao))
                throw new ArgumentException("Código do colaborador externo emissor não encontrado para esta organização.");
        }

        return await _notaFiscalService.LiberarEmissaoDeNotasFiscaisPorVigencia(orgId, mes, ano, codigoDiretoria, enviarEmail, codigoInternoColaboradorEmissao);
    }
}