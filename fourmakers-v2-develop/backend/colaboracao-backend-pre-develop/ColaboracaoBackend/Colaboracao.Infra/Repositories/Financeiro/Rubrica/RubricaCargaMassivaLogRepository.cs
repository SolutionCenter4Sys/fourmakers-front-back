using System.Threading.Tasks;
using Colaboracao.Core.Interfaces;
using Core.Domain.Financeiro.Rubrica;
using Dapper;
using DataTransferObject.Domain.Financeiro.Rubrica.RubricaCarga;
using DataTransferObject.Domain.Util.Enum;

namespace Colaboracao.Infra.Repositories.Financeiro.Rubrica;

public class RubricaCargaMassivaLogRepository(IDBConnection dapperConnection) : IRubricaCargaMassivaLogRepository
{
    public async Task<int> InserirLogCarga(RubricaCargaMassivaLogDTO logCarga)
    {
        var connection = dapperConnection.GetConnection();
        
        var sql = @"
            INSERT INTO tb_rubrica_carga_log (
                codigo_carga_rubrica,
                arquivo_origem,
                cod_diretoria,
                tb_rubrica_id,
                tb_rubrica_template_id,
                status_processamento,
                mensagem_erro,
                qtd_registros_processados_sucesso,
                qtd_registros_retornados,
                tb_org_id,
                codigo_interno_colaborador_criacao,
                mes,
                ano,
                data_criacao
            ) VALUES (
                @CodigoCargaRubrica,
                @ArquivoOrigem,
                @CodDiretoria,
                @TbRubricaId,
                @TbRubricaTemplateId,
                @StatusProcessamento,
                @MensagemErro,
                @QtdRegistrosProcessadosSucesso,
                @QtdRegistrosRetornados,
                @TbOrgId,
                @CodigoInternoColaboradorCriacao,
                @Mes,
                @Ano,
                @DataCriacao
            );
            SELECT LAST_INSERT_ID();";
        
        var parametros = new
        {
            logCarga.CodigoCargaRubrica,
            logCarga.ArquivoOrigem,
            logCarga.CodDiretoria,
            logCarga.TbRubricaId,
            logCarga.TbRubricaTemplateId,
            StatusProcessamento = logCarga.StatusProcessamento.ToString().ToLower(),
            logCarga.MensagemErro,
            logCarga.QtdRegistrosProcessadosSucesso,
            logCarga.QtdRegistrosRetornados,
            logCarga.TbOrgId,
            logCarga.CodigoInternoColaboradorCriacao,
            logCarga.Mes,
            logCarga.Ano,
            logCarga.DataCriacao
        };
        
        return await connection.QuerySingleAsync<int>(sql, parametros);
    }

    public async Task AtualizarStatusCarga(string codigoCargaRubrica, string status, string mensagemErro = null, int qtdSucesso = 0, int qtdRetornados = 0)
    {
        var connection = dapperConnection.GetConnection();
        
        var sql = @"
            UPDATE tb_rubrica_carga_log 
            SET status_processamento = @Status,
                mensagem_erro = @MensagemErro,
                qtd_registros_processados_sucesso = @QtdSucesso,
                qtd_registros_retornados = @QtdRetornados,
                data_processamento_fim = NOW()
            WHERE codigo_carga_rubrica = @CodigoCargaRubrica";
        
        await connection.ExecuteAsync(sql, new
        {
            Status = status,
            MensagemErro = mensagemErro,
            QtdSucesso = qtdSucesso,
            QtdRetornados = qtdRetornados,
            CodigoCargaRubrica = codigoCargaRubrica
        });
    }

    public async Task AtualizarMesAnoEQuantidade(string codigoCargaRubrica, int mes, int ano, int qtdRetornados)
    {
        var connection = dapperConnection.GetConnection();
        
        var sql = @"
            UPDATE tb_rubrica_carga_log 
            SET mes = @Mes,
                ano = @Ano,
                qtd_registros_retornados = @QtdRetornados
            WHERE codigo_carga_rubrica = @CodigoCargaRubrica";
        
        await connection.ExecuteAsync(sql, new
        {
            Mes = mes,
            Ano = ano,
            QtdRetornados = qtdRetornados,
            CodigoCargaRubrica = codigoCargaRubrica
        });
    }

    public async Task<int> InserirLogItem(RubricaCargaMassivaItemLogDTO logItem)
    {
        var connection = dapperConnection.GetConnection();
        
        var sql = @"
            INSERT INTO tb_rubrica_carga_item_log (
                codigo_carga_rubrica,
                json_item_tentativa,
                status_processamento,
                tipo_identificacao,
                codigo_interno_colaborador,
                tb_rubrica_colaborador_id,
                mensagem_erro,
                data_processamento
            ) VALUES (
                @CodigoCargaRubrica,
                @JsonItemTentativa,
                @StatusProcessamento,
                @TipoIdentificacao,
                @CodigoInternoColaborador,
                @TbRubricaColaboradorId,
                @MensagemErro,
                @DataProcessamento
            );
            SELECT LAST_INSERT_ID();";
        
        var parametros = new
        {
            logItem.CodigoCargaRubrica,
            logItem.JsonItemTentativa,
            StatusProcessamento = ConverterStatusItemParaBanco(logItem.StatusProcessamento),
            TipoIdentificacao = logItem.TipoIdentificacao.HasValue ? ConverterTipoIdentificacaoParaBanco(logItem.TipoIdentificacao.Value) : null,
            logItem.CodigoInternoColaborador,
            logItem.TbRubricaColaboradorId,
            logItem.MensagemErro,
            logItem.DataProcessamento
        };
        
        return await connection.QuerySingleAsync<int>(sql, parametros);
    }

    private static string ConverterStatusItemParaBanco(StatusProcessamentoItemEnum status)
    {
        return status switch
        {
            StatusProcessamentoItemEnum.Sucesso => "sucesso",
            StatusProcessamentoItemEnum.ErroColaboradorNaoEncontrado => "erro_colaborador_nao_encontrado",
            StatusProcessamentoItemEnum.ErroSalvamento => "erro_salvamento",
            StatusProcessamentoItemEnum.ErroProcessamentoGeral => "erro_processamento_geral",
            _ => "erro_processamento_geral"
        };
    }

    private static string ConverterTipoIdentificacaoParaBanco(TipoIdentificacaoEnum tipo)
    {
        return tipo switch
        {
            TipoIdentificacaoEnum.CpfColaborador => "cpf_colaborador",
            TipoIdentificacaoEnum.NaoEncontrado => "nao_encontrado",
            TipoIdentificacaoEnum.ErroGeral => "erro_geral",
            _ => "erro_geral"
        };
    }
}