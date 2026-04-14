using Colaboracao.Core.Interfaces;
using Core.Domain;
using Dapper;
using DataTransferObject.Domain.Log;
using System;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.Log
{
    public class LogBancoTalentoSRSRepository : ILogBancoTalentoSRSRepository
    {
        private readonly IDBConnection _dapperConnection;

        public LogBancoTalentoSRSRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        public async Task<bool> InserirLogBancoTalentoSRSAsync(LogBancoTalentoSRSDTO logDto)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                var query = @"
                    INSERT INTO tb_log_bancotalento_srs
                    (
                        id,
                        url_linkedin,
                        sucesso,
                        mensagem_retorno,
                        stack_trace,
                        data
                    )
                    VALUES
                    (
                        @Id,
                        @UrlLinkedin,
                        @Sucesso,
                        @MensagemRetorno,
                        @StackTrace,
                        @Data
                    );
                ";

                var parameters = new
                {
                    Id = logDto.Id,
                    UrlLinkedin = logDto.UrlLinkedin,
                    Sucesso = logDto.Sucesso ? 1 : 0,
                    MensagemRetorno = logDto.MensagemRetorno,
                    StackTrace = logDto.StackTrace,
                    Data = logDto.Data
                };

                var result = await connection.ExecuteAsync(query, parameters);
                return result > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
