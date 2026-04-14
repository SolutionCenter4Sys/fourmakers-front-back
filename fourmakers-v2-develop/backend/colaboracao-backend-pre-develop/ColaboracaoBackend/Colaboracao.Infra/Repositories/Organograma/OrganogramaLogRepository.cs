using Colaboracao.Core.Interfaces;
using Core.Domain.Organograma;
using Dapper;
using DataTransferObject.Domain;
using System;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.Organograma
{
    public class OrganogramaLogRepository : IOrganogramaLogRepository
    {
        private readonly IDBConnection _dapperConnection;

        public OrganogramaLogRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        public async Task OrganogramaInserirLog(OrganogramaLogDTO param, string tabLog)
        {
            string tabela = tabLog switch
            {
                "DEPARTAMENTO" => "tb_organograma_departamento_log",
                "POSICAO" => "tb_organograma_posicao_log",
                "ALOCACAO" => "tb_organograma_posicao_alocacao_log",
                "PERFIL_CORP" => "tb_perfil_corporativo_log",
                "PERFIL_CORP_SKILL" => "tb_perfil_corporativo_skill_log",
                "PERFIL_CORP_ALOC" => "tb_perfil_corporativo_alocacao_log",
                _ => throw new ArgumentException("Tipo tabela de log inválido")
            };

            var conn = _dapperConnection.GetConnection();

            var log = new OrganogramaLogDTO
            {
                Id = param.Id,
                CodigoInternoColaborador = param.CodigoInternoColaborador,
                Acao = param.Acao,
                CodigoInternoColaboradorAlterador = param.CodigoInternoColaborador,
                DataAlteracao = DateTime.Now,
                Objeto = param.Objeto,
                Alteracoes = param.Acao == "UPDATE" ? param.Alteracoes : ""
            };
            
            var logQuery = $@"
                        INSERT INTO {tabela} 
                        (id, tb_colaborador_codigo_interno_colaborador, acao, tb_colaborador_codigo_interno_colaborador_alterador, 
                         data_alteracao, objeto, alteracoes)
                        VALUES 
                        (UUID(), @CodigoInternoColaborador, @Acao, @CodigoInternoColaboradorAlterador, 
                         CURDATE(), @Objeto, @Alteracoes)";

            await conn.ExecuteAsync(logQuery, log);
        }
    }
}