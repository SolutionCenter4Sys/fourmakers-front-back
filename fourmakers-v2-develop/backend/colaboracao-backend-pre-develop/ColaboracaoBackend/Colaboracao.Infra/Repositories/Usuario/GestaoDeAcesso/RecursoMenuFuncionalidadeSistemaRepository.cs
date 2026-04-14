using Colaboracao.Core.Interfaces;
using Core.Domain.Usuario.GestaoDeAcesso;
using Dapper;
using DataTransferObject.Domain.Usuario.GestaoDeAcesso.RecursoFuncionalidadeSistema;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.Usuario.Usuario.GestaoDeAcesso
{
    public class RecursoMenuFuncionalidadeSistemaRepository : IRecursoMenuFuncionalidadeSistemaRepository
    {
        private readonly IDBConnection _dapperConnection;

        public RecursoMenuFuncionalidadeSistemaRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        private string SELECT_DEFAULT => @"
            SELECT 
                tb_funcionalidade_sistema_id AS TbFuncionalidadeSistemaId,
                codigo_recurso_menu AS CodigoRecursoMenu
            FROM 
                tb_recurso_menu_funcionalidade_sistema
            WHERE 
                1 = 1"
        ;

        public async Task<IEnumerable<RecursoFuncionalidadeSistemaResult>> ObterRecursoMenuFuncionalidadeSistemaPorCodigoRecursoMenuAsync(string codigoRecursoMenu)
        {
            var connection = _dapperConnection.GetConnection();

            var query = SELECT_DEFAULT + " AND codigo_recurso_menu = @CodigoRecursoMenu";
            return await connection.QueryAsync<RecursoFuncionalidadeSistemaResult>(query, new
            {
                CodigoRecursoMenu = codigoRecursoMenu
            });
        }

        public async Task<RecursoFuncionalidadeSistemaResult> ObterRecursoMenuFuncionalidadeSistemaPorCodigoEFuncionalidadeSistemaIdAsync(string codigoRecursoMenu, int tbFuncionalidadeSistemaId)
        {
            var connection = _dapperConnection.GetConnection();

            var query = SELECT_DEFAULT + " AND codigo_recurso_menu = @CodigoRecursoMenu AND tb_funcionalidade_sistema_id = @TbFuncionalidadeSistemaId";
            return await connection.QueryFirstOrDefaultAsync<RecursoFuncionalidadeSistemaResult>(query, new
            {
                CodigoRecursoMenu = codigoRecursoMenu,
                TbFuncionalidadeSistemaId = tbFuncionalidadeSistemaId
            });
        }

        public async Task<RecursoFuncionalidadeSistemaResult> InserirRecursoMenuFuncionalidadeSistemaAsync(RecursoMenuFuncionalidadeSistemaInput input)
        {
            // Verifica se a associação já existe
            var recursoExistente = await ObterRecursoMenuFuncionalidadeSistemaPorCodigoEFuncionalidadeSistemaIdAsync(input.CodigoRecursoMenu, input.TbFuncionalidadeSistemaId);
            if (recursoExistente != null)
            {
                return recursoExistente; // Retorna o recurso existente
            }

            // Insere a associação se ela não existir
            var connection = _dapperConnection.GetConnection();
            var query = @"
                INSERT INTO tb_recurso_menu_funcionalidade_sistema 
                    (tb_funcionalidade_sistema_id, codigo_recurso_menu)
                VALUES 
                    (@TbFuncionalidadeSistemaId, @CodigoRecursoMenu)";

            var parameters = new
            {
                input.TbFuncionalidadeSistemaId,
                input.CodigoRecursoMenu,
            };

            int rowsAffected = await connection.ExecuteAsync(query, parameters);

            if (rowsAffected > 0)
            {
                return await ObterRecursoMenuFuncionalidadeSistemaPorCodigoEFuncionalidadeSistemaIdAsync(input.CodigoRecursoMenu, input.TbFuncionalidadeSistemaId);
            }

            return null; // Retorna null caso a inserção falhe
        }


        public async Task<bool> DeletarRecursoFuncionalidadeSistemaAsync(RecursoMenuFuncionalidadeSistemaInput input)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                        DELETE 
                            tb_recurso_menu_funcionalidade_sistema 
                        WHERE
                            tb_funcionalidade_sistema_id = @TbFuncionalidadeSistemaId
                            AND codigo_recurso_menu = @CodigoRecursoMenu";

            var parameters = new
            {
                input.TbFuncionalidadeSistemaId,
                input.CodigoRecursoMenu,
            };

            int rowsAffected = await connection.ExecuteAsync(query, parameters);
         
            return rowsAffected > 0;
        }

        //public async Task<bool> DeletarRecursoFuncionalidadeSistemaAsync(Guid id)
        //{
        //    var connection = _dapperConnection.GetConnection();

        //    var query = @"UPDATE 
        //                     tb_recurso_menu_funcionalidade_sistema
        //                  SET
        //                     ativo = @Ativo
        //                  WHERE
        //                     id = @Id";

        //    int rowsAffected = await connection.ExecuteAsync(query, new { Id = id, Ativo = false });
        //    return rowsAffected > 0;
        //}
    }
}