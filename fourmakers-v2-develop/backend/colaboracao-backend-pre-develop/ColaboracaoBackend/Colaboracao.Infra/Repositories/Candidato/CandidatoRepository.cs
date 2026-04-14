using Colaboracao.Core.Interfaces;
using Core.Domain.Candidato;
using Core.DomainModel;
using Dapper;
using DataTransferObject.Domain;
using DataTransferObject.Domain.Candidato;
using DataTransferObject.Domain.Vaga;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.Candidato
{
    public class CandidatoRepository : ICandidatoRepository
    {
        private readonly IDBConnection _dapperConnection;

        public CandidatoRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        public async Task<CandidatoDTO> BuscarCandidatoPorCpf(string cpf)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                SELECT
                    c.id AS Id,
                    c.path_curriculo AS PathCurriculo,
                    c.pretensao_salarial AS PretencaoSalarial,
                    c.cargo_atual_ultimo AS CargoAtualUltimo,
                    c.salario_atual_ultimo AS SalarioAtualUltimo,
                    c.tipo_contrato_atual_ultimo AS TipoContratoAtualUltimo,
                    c.modalidade_atual_ultima AS ModalidadeAtualUltima,
                    c.aceita_sugestoes_vagas AS AceitaSugestoes,
                    c.data_criacao AS DataCriacao,
                    c.data_alteracao AS DataAlteracao,
                    c.ativo AS Ativo,
                    c.codigo_interno_colaborador AS CodigoInternoColaborador,
                    c.codigo_interno_colaborador AS Cpf,
                    col.nome_completo AS NomeCompleto,
                    c.estagio_processo_seletivo_id AS EstagioProcessoSeletivoId
                FROM
                    tb_candidato c
                    LEFT JOIN tb_colaborador col ON col.codigo_interno_colaborador = c.codigo_interno_colaborador
                WHERE
                    c.codigo_interno_colaborador = @cpf;";

            var result = await connection.QueryFirstOrDefaultAsync<CandidatoDTO>(query, new { cpf });
            return result;
        }

        public async Task<int> BuscarEstagioProcessoSeletivoId(int faseId)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"SELECT id FROM tb_estagio_processo_seletivo WHERE id = @faseId AND ativo = 1;";

            var result = await connection.QueryFirstOrDefaultAsync<int>(query, new { faseId });
            return result;
        }

        public async Task<long> SaveCandidato(string cpf, double? pretensaoSalarial, DateTime dataEstagioProcesso, int estagioProcessoSeletivoId, string pathCurriculo)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                INSERT INTO tb_candidato
                    (codigo_interno_colaborador, ativo, pretensao_salarial, data_estagio_processo, estagio_processo_seletivo_id, path_curriculo)
                VALUES
                    (@cpf, 1, @pretensaoSalarial, @dataEstagioProcesso, @estagioProcessoSeletivoId, @pathCurriculo);
                SELECT LAST_INSERT_ID();";

            var result = await connection.QueryFirstAsync<long>(query, new
            {
                cpf,
                pretensaoSalarial,
                dataEstagioProcesso,
                estagioProcessoSeletivoId,
                pathCurriculo
            });
            return result;
        }
    }
}
