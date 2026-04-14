using Colaboracao.Core.Interfaces;
using Core.Domain.Usuario.GestaoDeAcesso;
using Dapper;
using DataTransferObject.Domain.Usuario.GestaoDeAcesso.RecursoMenu;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.Usuario.Usuario.GestaoDeAcesso
{
    public class RecursoMenuRepository : IRecursoMenuRepository
    {
        private readonly IDBConnection _dapperConnection;

        public RecursoMenuRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        private string SELECT_DEFAULT => @"
            SELECT 
                id AS Id,
                ativo AS Ativo,
                nome_menu AS NomeMenu,
                codigo_recurso_menu as CodigoRecursoMenu,
                codigo_recurso AS CodigoRecurso,
                codigo_recurso_menu_pai AS CodigoRecursoMenuPai,
                codigo_icone AS CodigoIcone,
                tipo_menu AS TipoMenu,
                link_externo_nova_pagina AS LinkExternoNovaPagina,
                ordenacao AS Ordenacao,
                em_breve AS EmBreve,
                visivel AS Visivel,
                data_criacao as DataCriacao,
                data_alteracao as DataAlteracao,
                codigo_interno_colaborador_criacao AS CodigoInternoColaboradorCriacao,
                codigo_interno_colaborador_alteracao AS CodigoInternoColaboradorAlteracao
            FROM 
                tb_recurso_menu
            WHERE 
                ativo = 1";

        public async Task<IEnumerable<RecursoMenuResult>> ListarRecursoMenusAsync()
        {
            var connection = _dapperConnection.GetConnection();
            var query = SELECT_DEFAULT;

            var result = await connection.QueryAsync<RecursoMenuResult>(query);
            return result;
        }

        public async Task<RecursoMenuResult> ObterRecursoMenuPorIdAsync(Guid id)
        {
            var connection = _dapperConnection.GetConnection();

            var query = SELECT_DEFAULT + " AND id = @Id";
            return await connection.QueryFirstOrDefaultAsync<RecursoMenuResult>(query, new { Id = id });
        }

        public async Task<RecursoMenuResult> ObterRecursoMenuPorCodigoRecursoAsync(string codigoRecurso)
        {
            var connection = _dapperConnection.GetConnection();

            var query = SELECT_DEFAULT + " AND codigo_recurso = @CodigoRecurso";
            return await connection.QueryFirstOrDefaultAsync<RecursoMenuResult>(query, new
            {
                CodigoRecurso = codigoRecurso
            });
        }
        public async Task<RecursoMenuResult> ObterRecursoMenuPorCodigoRecursoMenuAsync(string codigoRecursoMenu)
        {
            var connection = _dapperConnection.GetConnection();

            var query = SELECT_DEFAULT + " AND codigo_recurso = @CodigoRecursoMenu";
            return await connection.QueryFirstOrDefaultAsync<RecursoMenuResult>(query, new
            {
                CodigoRecursoMenu = codigoRecursoMenu
            });
        }

        public async Task<RecursoMenuResult> InserirRecursoMenuAsync(RecursoMenuInput input)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"
                        INSERT INTO tb_recurso_menu 
                            (id, ativo, codigo_interno_colaborador_criacao, codigo_interno_colaborador_alteracao, nome_menu, codigo_recurso_menu, codigo_recurso, codigo_recurso_menu_pai, codigo_icone, tipo_menu, link_externo_nova_pagina, ordenacao, em_breve, visivel)
                        VALUES 
                            (@Id, @Ativo, @CodigoInternoColaboradorAlteracao, @CodigoInternoColaboradorAlteracao, @NomeMenu, @CodigoRecursoMenu, @CodigoRecurso, @CodigoRecursoMenuPai, @CodigoIcone, @TipoMenu, @LinkExternoNovaPagina, @Ordenacao, @EmBreve, @Visivel)";

            var parameters = new
            {
                input.Id,
                input.Ativo,
                CodigoInternoColaboradorAlteracao = input.CodigoInternoColaboradorAlteracao,
                input.NomeMenu,
                input.CodigoRecursoMenu,
                input.CodigoRecurso,
                input.CodigoRecursoMenuPai,
                input.CodigoIcone,
                input.TipoMenu,
                input.LinkExternoNovaPagina,
                input.Ordenacao,
                input.EmBreve,
                input.Visivel
            };

            int rowsAffected = await connection.ExecuteAsync(query, parameters);
            if (rowsAffected > 0)
            {
                return await ObterRecursoMenuPorIdAsync(input.Id);
            }

            return null;
        }

        public async Task<RecursoMenuResult> AtualizarRecursoMenuAsync(RecursoMenuInput input)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                        UPDATE 
                            tb_recurso_menu 
                        SET 
                            nome_menu = @NomeMenu,
                            codigo_recurso_menu = @CodigoRecursoMenu,
                            codigo_recurso = @CodigoRecurso,
                            codigo_recurso_menu_pai = @CodigoRecursoMenuPai,
                            codigo_icone = @CodigoIcone,
                            tipo_menu = @TipoMenu,
                            link_externo_nova_pagina = @LinkExternoNovaPagina,
                            ordenacao = @Ordenacao,
                            codigo_interno_colaborador_alteracao = @CodigoInternoColaboradorAlteracao
                        WHERE
                            id = @Id";

            var parameters = new
            {
                input.Id,
                input.NomeMenu,
                input.CodigoRecurso,
                input.CodigoRecursoMenuPai,
                input.CodigoIcone,
                input.TipoMenu,
                input.LinkExternoNovaPagina,
                input.Ordenacao,
                input.CodigoInternoColaboradorAlteracao
            };

            int rowsAffected = await connection.ExecuteAsync(query, parameters);
            if (rowsAffected > 0)
            {
                return await ObterRecursoMenuPorIdAsync(input.Id);
            }

            return null;
        }

        public async Task<bool> DeletarRecursoMenuAsync(Guid id)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"UPDATE 
                             tb_recurso_menu
                          SET
                             ativo = @Ativo
                          WHERE
                             id = @Id";

            int rowsAffected = await connection.ExecuteAsync(query, new { Id = id, Ativo = false });
            return rowsAffected > 0;
        }
    }
}