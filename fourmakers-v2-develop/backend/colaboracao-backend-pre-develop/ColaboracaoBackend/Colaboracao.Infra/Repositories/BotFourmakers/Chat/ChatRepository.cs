using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colaboracao.Core.Interfaces;
using Core.Domain.BotFourmakers;
using Core.Domain.BotFourmakers.Chat;
using Dapper;
using DataTransferObject.Domain.BotFourmakers.Chat;

namespace Colaboracao.Infra.Repositories.BotFourmakers.Chat;

public class ChatRepository : IChatRepository
{
    private readonly IDBConnection _dapperConnection;

    private const string SELECT_DEFAULT = @"
        WITH ultima_mensagem AS (
            SELECT
                chat_id,
                mensagem,
                data_criacao,
                ROW_NUMBER() OVER (PARTITION BY chat_id ORDER BY data_criacao DESC) AS rn
            FROM
                tb_questao_chat_bot
            WHERE
                tipo = 1
        )
        SELECT 
            tcb.id AS Id,
            tcb.codigo_interno_colaborador AS CodigoInternoColaborador,
            um.mensagem AS Mensagem,
            um.data_criacao AS CriadoEm
        FROM 
            tb_chat_bot tcb
        LEFT JOIN 
            ultima_mensagem um
            ON um.chat_id = tcb.id AND um.rn = 1
    ";

    public ChatRepository(IDBConnection dapperConnection)
    {
        _dapperConnection = dapperConnection;
    }
    
    public async Task<ChatDTO> InserirAsync(string codigoInternoColaborador, int orgId)
    {
        var connection = _dapperConnection.GetConnection();

        var query = @"
            INSERT INTO tb_chat_bot(codigo_interno_colaborador, tb_org_id)
            VALUES(@CodigoInternoColaborador, @OrgId);

            SELECT LAST_INSERT_ID();
        ";

        var parametros = new
        {
            CodigoInternoColaborador = codigoInternoColaborador,
            OrgId = orgId
        };

        var insert = await connection.ExecuteScalarAsync<int>(query, parametros);
        return await ObterPorIdAsync(insert);
    }
    
    public async Task<List<ChatDTO>> ListarAsync(string codigoInternoColaborador, int orgId)
    {
        var connection = _dapperConnection.GetConnection();

        var query = SELECT_DEFAULT;
        query += @"
            WHERE
                tcb.codigo_interno_colaborador = @CodigoInternoColaborador
                AND tcb.tb_org_id = @OrgId
                AND tcb.ativo = 1
            ORDER BY 
                um.data_criacao DESC;
        ";

        var parametros = new
        {
            CodigoInternoColaborador = codigoInternoColaborador,
            OrgId = orgId
        };

        var result = await connection.QueryAsync<ChatDTO>(query, parametros);
        return result.ToList();
    }

    public async Task<ChatDTO> ObterPorIdAsync(int chatId)
    {
        var connection = _dapperConnection.GetConnection();

        var query = SELECT_DEFAULT;
        query += @"
            WHERE
                tcb.id = @ChatId
                AND tcb.ativo = 1;
        ";

        var parametros = new
        {
            ChatId = chatId,
        };

        var result = await connection.QueryFirstOrDefaultAsync<ChatDTO>(query, parametros);
        return result;
    }
    
    public async Task RemoverAsync(int chatId)
    {
        var connection = _dapperConnection.GetConnection();

        var query = @"
            UPDATE tb_chat_bot tcb 
                SET tcb.ativo = 0
            WHERE
                tcb.id = @ChatId
        ";

        var parametros = new
        {
            ChatId = chatId,
        };

        await connection.ExecuteAsync(query, parametros);
    }
}