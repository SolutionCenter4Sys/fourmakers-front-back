using Colaboracao.Core.Interfaces;
using Core.Domain.Usuario.GestaoDeAcesso;
using Dapper;
using DataTransferObject.Domain.Usuario.GestaoDeAcesso.Recurso;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.Usuario.GestaoDeAcesso
{
    public class RecursoOrgDisponivelRepository : IRecursoOrgDisponivelRepository
    {
        private readonly IDBConnection _dapperConnection;

        public RecursoOrgDisponivelRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        private string SELECT_DEFAULT => @"
            SELECT 
                tb_org_id AS OrgId,
                codigo_recurso_menu AS CodigoRecursoMenu
            FROM
                tb_recurso_org_disponivel
            WHERE 
                1 = 1
        ";

        public async Task<IEnumerable<RecursoOrgDisponivelDTO>> ConfigurarRecursoOrgDisponivelAsync(IEnumerable<RecursoOrgDisponivelDTO> recursos)
        {

            var connection = _dapperConnection.GetConnection();
            // apaga todos os registros para a org
            
            await connection.ExecuteAsync(@"
                DELETE FROM tb_recurso_org_disponivel"
            );

            // verifica se há recursos a inserir
            if (recursos != null && recursos.Any())
            {
                // executa a inserção em lote
                await connection.ExecuteAsync(@"
                    INSERT INTO tb_recurso_org_disponivel (tb_org_id, codigo_recurso_menu)
                    VALUES (@OrgId, @CodigoRecursoMenu);",
                    recursos);
            }

            return await ListarRecursoOrgDisponivelAsync();

        }

        public async Task<IEnumerable<RecursoOrgDisponivelDTO>> ListarRecursoOrgDisponivelAsync()
        {
            var connection = _dapperConnection.GetConnection();
            var query = SELECT_DEFAULT;
            return await connection.QueryAsync<RecursoOrgDisponivelDTO>(query);
        }

    }
}
