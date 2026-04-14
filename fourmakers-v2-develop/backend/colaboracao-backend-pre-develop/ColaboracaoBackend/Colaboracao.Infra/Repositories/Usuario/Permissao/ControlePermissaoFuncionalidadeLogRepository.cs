using Colaboracao.Core.Interfaces;
using Core.Domain.Usuario.Permissao;
using Dapper;
using DataTransferObject.Domain.Usuario.Permissao;
using System;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.Usuario.Permissao
{
    public class PermissaoLogRepository : IPermissaoLogRepository
    {
        private readonly IConnectionStringCore _connectionString;

        public PermissaoLogRepository(IConnectionStringCore connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task InserirLog(UsuarioPermissaoLogDTO log)
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    _connection.Open();

                    string sql = @"
                        INSERT INTO tb_usuario_permissao_log
                        (
                            tb_org_id,
                            codigo_interno_colaborador_criacao,
                            operacao,
                            tb_usuario_id,
                            tb_grupo_acesso_id,
                            tb_funcionalidade_sistema_id
                        )
                        VALUES
                        (
                            @OrgId,
                            @ColaboradorCpfCriacao,
                            @Operacao,
                            @UsuarioId,
                            @GrupoAcessoId,
                            @FuncionalidadeSistemaId
                        )";

                    await _connection.ExecuteAsync(sql, log);
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao inserir log de controle de permissão de funcionalidade", ex);
                }
                finally
                {
                    _connection.Close();
                }
            }
        }
    }
}