using Colaboracao.Core.Interfaces;
using Core.Domain.Usuario.Permissao;
using Dapper;
using DataTransferObject.Domain.Usuario.Permissao;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.Usuario.Permissao
{
    public class UsuarioGrupoAcessoRepository : IUsuarioGrupoAcessoRepository
    {
        private readonly IDBConnection _dapperConnection;

        public UsuarioGrupoAcessoRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        public async Task<IEnumerable<UsuarioGrupoAcessoResult>> ObterGruposAcessoPorUsuario(int usuarioId, int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var sql = @"
                SELECT
                    tuga.tb_usuario_id AS UsuarioId,
                    tga.id AS GrupoAcessoId,
                    tga.descricao AS Descricao,
                    tuga.data_criacao AS DataCriacao,
                    tuga.data_alteracao AS DataAlteracao,
                    tuga.ativo AS Ativo,
                    tc.nome_completo AS NomeCompleto,
                    tc.codigo_interno_colaborador AS Cpf,
                    tga.tb_org_id AS OrgId
                FROM
                    tb_usuario_grupo_acesso tuga
                JOIN
                    tb_grupo_acesso tga ON tuga.tb_grupo_acesso_id = tga.id
                JOIN
                    tb_usuario tu ON tuga.tb_usuario_id = tu.id
                JOIN
                    tb_colaborador tc ON tu.codigo_interno_colaborador = tc.codigo_interno_colaborador
                WHERE
                    tuga.ativo = 1
                    AND tga.tb_org_id = @OrgId
                    AND tuga.tb_usuario_id = @UsuarioId";

            return await connection.QueryAsync<UsuarioGrupoAcessoResult>(sql, new
            {
                OrgId = orgId,
                UsuarioId = usuarioId
            });
        }

        public async Task AdicionarUsuarioGrupoAcesso(int usuarioId, int grupoAcessoId, string cpfAlterador = null)
        {
            var connection = _dapperConnection.GetConnection();

            var sql = @"
                INSERT INTO tb_usuario_grupo_acesso (tb_usuario_id, tb_grupo_acesso_id, ativo)
                VALUES (@UsuarioId, @GrupoAcessoId, 1);
                SELECT LAST_INSERT_ID();";

            var id = await connection.ExecuteScalarAsync<int>(sql, new { UsuarioId = usuarioId, GrupoAcessoId = grupoAcessoId });

            var objetoCriado = await connection.QueryFirstOrDefaultAsync<dynamic>(
                "SELECT * FROM tb_usuario_grupo_acesso WHERE id = @Id",
                new { Id = id });

            await InserirLogUsuarioGrupoAcesso(id, "CREATE", null, objetoCriado, cpfAlterador);
        }

        public async Task RemoverUsuarioGrupoAcesso(int usuarioId, int grupoAcessoId, string cpfAlterador = null)
        {
            var connection = _dapperConnection.GetConnection();

            var objetoAnterior = await connection.QueryFirstOrDefaultAsync<dynamic>(
                "SELECT * FROM tb_usuario_grupo_acesso WHERE tb_usuario_id = @UsuarioId AND tb_grupo_acesso_id = @GrupoAcessoId",
                new { UsuarioId = usuarioId, GrupoAcessoId = grupoAcessoId });

            if (objetoAnterior != null)
            {
                await InserirLogUsuarioGrupoAcesso((int)objetoAnterior.id, "DELETE", objetoAnterior, null, cpfAlterador);

                var sql = @"
                    DELETE FROM tb_usuario_grupo_acesso
                    WHERE tb_usuario_id = @UsuarioId AND tb_grupo_acesso_id = @GrupoAcessoId";

                await connection.ExecuteAsync(sql, new { UsuarioId = usuarioId, GrupoAcessoId = grupoAcessoId });
            }
        }

        private async Task InserirLogUsuarioGrupoAcesso(int id, string acao, dynamic objetoAnterior, dynamic objetoAtualizado, string cpfAlterador)
        {
            var connection = _dapperConnection.GetConnection();
            var logId = Guid.NewGuid().ToString();
            var objetoJson = objetoAnterior != null ? JsonSerializer.Serialize(objetoAnterior) : "{}";
            var alteracoesJson = objetoAtualizado != null ? JsonSerializer.Serialize(objetoAtualizado) : "{}";

            var logSql = @"
                INSERT INTO tb_usuario_grupo_acesso_log
                (id, tb_usuario_grupo_acesso_id, acao, tb_colaborador_codigo_interno_colaborador_alterador,
                 data_alteracao, objeto, alteracoes)
                VALUES
                (@Id, @IdRegistro, @Acao, @CpfAlterador,
                 CURRENT_TIMESTAMP, @Objeto, @Alteracoes)";

            await connection.ExecuteAsync(logSql, new
            {
                Id = logId,
                IdRegistro = id,
                Acao = acao,
                CpfAlterador = cpfAlterador,
                Objeto = objetoJson,
                Alteracoes = alteracoesJson
            });
        }

        public async Task<bool> UsuarioNaoPertenceOrg(int usuarioId, int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var sql = @"
                SELECT COUNT(1)
                FROM tb_colaborador_org tco
                JOIN tb_usuario tu ON tco.codigo_interno_colaborador = tu.codigo_interno_colaborador
                WHERE tu.id = @UsuarioId AND tco.tb_org_id = @OrgId";

            var count = await connection.ExecuteScalarAsync<int>(sql, new { UsuarioId = usuarioId, OrgId = orgId });

            return count == 0;
        }

        public async Task<bool> UsuarioEstaRelacionadoGrupoAcesso(int usuarioId, int grupoAcessoId)
        {
            var connection = _dapperConnection.GetConnection();

            var sql = @"
                SELECT COUNT(1)
                FROM tb_usuario_grupo_acesso
                WHERE tb_usuario_id = @UsuarioId
                    AND tb_grupo_acesso_id = @GrupoAcessoId";

            var count = await connection.ExecuteScalarAsync<int>(sql, new { UsuarioId = usuarioId, GrupoAcessoId = grupoAcessoId });

            return count > 0;
        }
    }
}
