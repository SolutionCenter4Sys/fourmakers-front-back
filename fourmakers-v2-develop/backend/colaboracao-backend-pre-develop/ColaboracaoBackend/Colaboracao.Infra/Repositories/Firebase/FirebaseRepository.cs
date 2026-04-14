using Colaboracao.Core.Interfaces;
using Dapper;
using Core.DomainModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.Firebase
{
    public class FirebaseRepository(IDBConnection dapperConnection) : IFirebaseRepository
    {
        public async Task<string> BuscarDeviceTokenApp(string codigoColaborador)
        {
            var conn = dapperConnection.GetConnection();

            var sql = @"SELECT tu.fcm_token
                        FROM tb_usuario tu
                        WHERE tu.codigo_interno_colaborador = @Codigo;";

            return await conn.QueryFirstOrDefaultAsync<string>(sql, new { Codigo = codigoColaborador });
        }

        public async Task<List<string>> BuscarDeviceTokensAppEmLote(List<string> codigosColaboradores)
        {
            var conn = dapperConnection.GetConnection();

            var sql = @"SELECT tu.fcm_token
                        FROM tb_usuario tu
                        WHERE tu.codigo_interno_colaborador IN @Codigos;";

            return (await conn.QueryAsync<string>(sql, new { Codigos = codigosColaboradores })).AsList();
        }

        public async Task<string> BuscarEmailColaborador(string codigoInternoColaborador)
        {
            var conn = dapperConnection.GetConnection();

            var sql = @"SELECT tu.email
                        FROM tb_usuario tu
                        WHERE tu.codigo_interno_colaborador = @Codigo;";

            return await conn.QueryFirstOrDefaultAsync<string>(sql, new { Codigo = codigoInternoColaborador });
        }
    }
}
