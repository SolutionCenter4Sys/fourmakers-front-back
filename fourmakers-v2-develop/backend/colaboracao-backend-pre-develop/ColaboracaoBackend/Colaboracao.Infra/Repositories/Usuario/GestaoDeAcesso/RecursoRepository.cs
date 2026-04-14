using Colaboracao.Core.Interfaces;
using Core.Domain.Usuario.GestaoDeAcesso;
using Dapper;
using DataTransferObject.Domain.Usuario.GestaoDeAcesso.Recurso;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.Usuario.Usuario.GestaoDeAcesso
{
    public class RecursoRepository : IRecursoRepository
    {
        private readonly IDBConnection _dapperConnection;

        public RecursoRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }


        private const string COLUNAS_RECURSO = @"   tr.codigo_recurso AS CodigoRecurso,
                                                    tr.ativo AS Ativo,
                                                    tr.data_criacao AS DataCriacao,
                                                    tr.data_alteracao AS DataAlteracao,
                                                    tr.codigo_interno_colaborador_criacao AS CodigoInternoColaboradorCriacao,
                                                    tr.codigo_interno_colaborador_alteracao AS CodigoInternoColaboradorAlteracao";


        public async Task<IEnumerable<RecursoResult>> ListarRecursosAsync()
        {
            var connection = _dapperConnection.GetConnection();

            var query = @$"
                            SELECT 
                                {COLUNAS_RECURSO},
                                trm.codigo_recurso_menu AS CodigoRecursoMenuTemp
                            FROM 
                                tb_recurso tr
                            JOIN 
                                tb_recurso_menu trm ON tr.codigo_recurso = trm.codigo_recurso";

            var result = await connection.QueryAsync<RecursoResult>(query);
            return result;
        }


        public async Task<IEnumerable<RecursoMenuAninhadoResult>> ListarRecursosVisaoMenuAsync(string codigoInternoColaborador, int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            // precisamos do usuarioId para verificar relacionamento da tb_usuario_grupo_acesso
            var usuarioQuery = @" SELECT
                                        id 
                                    FROM
                                        tb_usuario
                                    WHERE
                                        tb_org_id = @OrgId AND codigo_interno_colaborador = @CodigoInternoColaborador AND ativo = 1;";

            var usuarioId = await connection.QuerySingleOrDefaultAsync<int?>(usuarioQuery, new { OrgId = orgId, CodigoInternoColaborador = codigoInternoColaborador });

            if (usuarioId == null)
            {
                throw new Exception("Usuário não pertence a Org logada.");
            }

            var query = @"  
                            -- apagar a tabela temporária caso já exista
                            DROP TEMPORARY TABLE IF EXISTS temp_recurso_menu_disponivel;
                            
                            -- criar nova tabela temporária já filtrando menus disponíveis pra org
                            CREATE TEMPORARY TABLE temp_recurso_menu_disponivel AS
                            SELECT 
                                trm.codigo_recurso AS CodigoRecurso,
                                trm.codigo_recurso_menu AS CodigoRecursoMenu,
                                trm.id AS Id,
                                trm.data_criacao AS DataCriacao,
                                trm.data_alteracao AS DataAlteracao,
                                trm.nome_menu AS NomeMenu,
                                trm.codigo_recurso_menu_pai AS CodigoRecursoMenuPai,
                                trm.codigo_icone AS CodigoIcone,
                                trm.tipo_menu AS TipoMenu,
                                trm.link_externo_nova_pagina AS LinkExternoNovaPagina,
                                trm.ordenacao AS Ordenacao,
                                trm.em_breve AS EmBreve,
                                trm.visivel AS Visivel
                            FROM 
                                tb_recurso_org_disponivel trod
                            JOIN 
                                tb_recurso_menu trm ON trm.codigo_recurso_menu = trod.codigo_recurso_menu
                            WHERE
                                trod.tb_org_id = @OrgId;
                            
                            -- monta o select final aplicando um filtro adicional para exibir os menus que não possuem associação com a tabela funcionalidade_sistema
                            -- caso um menu esteja associado a uma funcionalidade, será necessário que o usuário tenha a devida permissão de acesso para visualizá-lo
                            SELECT 
                                subquery.CodigoRecurso,
                                subquery.CodigoRecursoMenu,
                                subquery.Id,
                                subquery.DataCriacao,
                                subquery.DataAlteracao,
                                subquery.NomeMenu,
                                subquery.CodigoRecursoMenuPai,
                                subquery.CodigoIcone,
                                subquery.TipoMenu,
                                subquery.LinkExternoNovaPagina,
                                subquery.Ordenacao,
                                subquery.EmBreve,
                                subquery.Visivel,
                                subquery.ExisteNaTabelaRecursoFuncionalidadeSistema,
                                subquery.PossuiAcessoAFuncionalidadeSistema
                            FROM (
                                SELECT 
                                    trmd.*,
                                    CASE 
                                        WHEN EXISTS (
                                            SELECT 
                                                1 
                                            FROM
                                                tb_recurso_menu_funcionalidade_sistema trmfs 
                                            WHERE
                                                trmfs.codigo_recurso_menu = trmd.CodigoRecursoMenu
                                        ) THEN
                                            TRUE
                                        ELSE
                                            FALSE
                                        END AS ExisteNaTabelaRecursoFuncionalidadeSistema,
                                    CASE 
                                        WHEN EXISTS (
                                            SELECT
                                                1
                                            FROM 
                                                tb_grupo_acesso_funcionalidade_sistema tgafs
                            				JOIN
                            					tb_grupo_acesso tga ON tgafs.tb_grupo_acesso_id = tga.id
                                            JOIN
                                                tb_usuario_grupo_acesso tuga ON tuga.tb_grupo_acesso_id = tgafs.tb_grupo_acesso_id
                            				JOIN
                            					tb_usuario tu ON tuga.tb_usuario_id AND tga.tb_org_id = tu.tb_org_id 
                                            JOIN
                                                tb_recurso_menu_funcionalidade_sistema trmfs ON trmfs.tb_funcionalidade_sistema_id = tgafs.tb_funcionalidade_sistema_id
                                            WHERE 
                                                tuga.tb_usuario_id = @UsuarioId AND trmfs.codigo_recurso_menu = trmd.CodigoRecursoMenu AND tga.tb_org_id = @OrgId
                                        ) THEN 
                                            TRUE
                                          ELSE
                                            FALSE
                                          END AS PossuiAcessoAFuncionalidadeSistema
                                FROM 
                                    temp_recurso_menu_disponivel trmd
                                ) subquery
                            WHERE 
                                ExisteNaTabelaRecursoFuncionalidadeSistema = PossuiAcessoAFuncionalidadeSistema;
                            
                            -- Regra de exibição dos menus:
                            -- 1. Quando o menu NÃO possui recurso associado em tb_recurso_menu_funcionalidade_sistema,
                            --    ambos os campos ficam FALSE e passa no filtro (FALSE && FALSE), sendo assim, o menu é exibido.
                            -- 2. Quando o menu POSSUI recurso associado e o usuário tem permissão,
                            --    ambos os campos ficam TRUE e passa no filtro (TRUE && TRUE) ? o menu é exibido.
                            -- 3. Quando o menu POSSUI recurso associado mas o usuário NÃO tem permissão,
                            --    fica TRUE && FALSE e não passa no filtro, isto é, o menu não é exibido.

                            ";

            var flatResult = await connection.QueryAsync<RecursoMenuAninhadoResult>(query, new { OrgId = orgId, UsuarioId = usuarioId });

            // Transformar o flatResult em estrutura aninhada, se necessário
            return flatResult;

        }

        public async Task<RecursoResult> ObterRecursoPorCodigoRecursoAsync(string codigo)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @$"
                             SELECT 
                                 {COLUNAS_RECURSO}
                             FROM 
                                 tb_recurso tr
                             WHERE 
                                 tr.ativo = 1 AND tr.codigo_recurso = @Codigo";

            return await connection.QueryFirstOrDefaultAsync<RecursoResult>(query, new { Codigo = codigo });
        }


        public async Task<RecursoResult> InserirRecursoAsync(RecursoInput input)
        {
            // Verifica se o recurso já existe
            var recursoExistente = await ObterRecursoPorCodigoRecursoAsync(input.CodigoRecurso);
            if (recursoExistente != null)
            {
                return recursoExistente; // Retorna o recurso existente
            }

            // Insere o recurso se ele não existir
            var connection = _dapperConnection.GetConnection();
            var query = @"
                INSERT INTO tb_recurso 
                    (codigo_recurso, ativo, codigo_interno_colaborador_criacao, codigo_interno_colaborador_alteracao)
                VALUES 
                    (@CodigoRecurso, @Ativo, @CodigoInternoColaboradorAlteracao, @CodigoInternoColaboradorAlteracao)";

            var parameters = new
            {
                input.CodigoRecurso,
                input.Ativo,
                input.CodigoInternoColaboradorAlteracao
            };

            int rowsAffected = await connection.ExecuteAsync(query, parameters);
            if (rowsAffected > 0)
            {
                return await ObterRecursoPorCodigoRecursoAsync(input.CodigoRecurso);
            }

            return null; // Retorna null caso a inserção falhe
        }


        public async Task<RecursoResult> AtualizarRecursoAsync(RecursoInput input)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"
                        UPDATE 
                            tb_recurso 
                        SET 
                            codigo_recurso = @CodigoRecurso,
                            codigo_interno_colaborador_alteracao = @CodigoInternoColaboradorAlteracao
                        WHERE
                            codigo_recurso = @CodigoRecurso";

            var parameters = new
            {
                input.CodigoRecurso,
                input.CodigoInternoColaboradorAlteracao
            };

            int rowsAffected = await connection.ExecuteAsync(query, parameters);
            if (rowsAffected > 0)
            {
                return await ObterRecursoPorCodigoRecursoAsync(input.CodigoRecurso);
            }

            return null;
        }

        public async Task<bool> DeletarRecursoAsync(string codigoRecurso)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"DELETE FROM  
                             tb_recurso
                          WHERE
                             codigo_recurso = @CodigoRecurso";

            int rowsAffected = await connection.ExecuteAsync(query, new { CodigoRecurso = codigoRecurso });
            return rowsAffected > 0;
        }

        public async Task<bool> DeletarTodosRecursosAsync()
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"DELETE FROM  
                             tb_recurso";

            int rowsAffected = await connection.ExecuteAsync(query );
            return rowsAffected > 0;
        }
    }
}