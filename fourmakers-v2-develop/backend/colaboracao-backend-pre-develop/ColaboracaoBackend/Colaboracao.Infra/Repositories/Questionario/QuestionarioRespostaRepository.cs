using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Colaboracao.Core.Interfaces;
using Core.Domain.Questionario;
using Dapper;
using DataTransferObject.Domain.Questionario;

namespace Colaboracao.Infra.Repositories.Questionario;

public class QuestionarioRespostaRepository(IDBConnection dapperConnection) : IQuestionarioRespostaRepository
{
    private readonly IDbConnection _connection = dapperConnection.GetConnection();

    public async Task InserirRespostaAsync(string codigoQuestionario, string textoJson, string codigoInternoColaborador, int orgId, bool processado)
    {
        var query = @"
            INSERT INTO tb_questionario_resposta_json
            (codigo_questionario, tb_org_id, codigo_interno_colaborador, json_texto, processado)
            VALUES
            (@CodigoQuestionario, @OrgId, @CodigoInternoColaborador, @JsonTexto, @Processado);
        ";

        var parametros = new
        {
            CodigoQuestionario = codigoQuestionario,
            OrgId = orgId,
            CodigoInternoColaborador = codigoInternoColaborador,
            JsonTexto = textoJson,
            Processado = false
        };
        
        var result = await _connection.ExecuteAsync(query, parametros);
        if (result <= 0)
        {
            throw new ApplicationException("Erro ao inserir resposta");
        }
    }

    public async Task<GenericQuestionarioResult<T>> ObterRespostaPorCodigoAsync<T>(int codigoResposta)
    {
        var sql = @"
            SELECT
                id,
                json_texto
            FROM tb_questionario_resposta_json
            WHERE id = @CodigoResposta;
        ";

        var resultado = await _connection.QueryFirstOrDefaultAsync<GenericQuestionarioResult<T>>(
            sql,
            new { CodigoResposta = codigoResposta }
        );

        if (string.IsNullOrWhiteSpace(resultado.JsonTexto))
            throw new ApplicationException(
                "Não foi possível encontrar a resposta solicitada. Tente novamente."
            );

        try
        {
            var obj = JsonSerializer.Deserialize<T>(resultado.JsonTexto);

            if (obj == null)
                throw new ApplicationException(
                    "Ocorreu um problema ao carregar a resposta. Tente novamente."
                );
    
            resultado.Resposta = obj;
            return resultado;
        }
        catch (JsonException _)
        {
            throw new ApplicationException(
                "Ocorreu um erro ao carregar a resposta. Por favor, tente novamente mais tarde."
            );
        }
    }

    public async Task<bool> ExisteCodigoQuestionarioAsync(string codigoQuestionario, int orgId)
    {
        var sql = @"
            SELECT CASE 
                WHEN EXISTS (
                    SELECT 1
                    FROM tb_questionario
                    WHERE codigo_questionario = @CodigoQuestionario
                      AND tb_org_id = @OrgId
                ) THEN 1
                ELSE 0
            END
        ";

        return await _connection.ExecuteScalarAsync<bool>(sql, new
        {
            CodigoQuestionario = codigoQuestionario,
            OrgId = orgId
        });
    }

    public async Task<List<string>> ObterCodigosQuestionarioColaboradorAsync(int orgId, string codigoInternoColaborador)
    {
        var query = @"
            SELECT 
                codigo_questionario
            FROM tb_questionario_resposta_json
            WHERE 
                tb_org_id = @OrgId
                AND codigo_interno_colaborador  = @CodigoInternoColaborador
        ";

        var parametros = new
        {
            OrgId = orgId,
            CodigoInternoColaborador = codigoInternoColaborador
        };

        var result = await _connection.QueryAsync<string>(query, parametros);
        return result.ToList();
    }
    
    public async Task<List<string>> ObterCodigosQuestionariosAtivosAsync(int orgId)
    {
        var query = @"
            SELECT 
                codigo_questionario
            FROM tb_questionario
            WHERE 
                tb_org_id = @OrgId
                AND mostrar_questionario = 1
                AND NOW() BETWEEN data_inicio AND data_fim
        ";

        var parametros = new
        {
            OrgId = orgId
        };

        var result = await _connection.QueryAsync<string>(query, parametros);
        return result.ToList();
    }
}