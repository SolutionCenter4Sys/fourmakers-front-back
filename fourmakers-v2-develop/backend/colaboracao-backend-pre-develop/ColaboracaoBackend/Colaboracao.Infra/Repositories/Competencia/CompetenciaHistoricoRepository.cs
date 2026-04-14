using Colaboracao.Core.Interfaces;
using Colaboracao.Infra.Context;
using Core.DomainModel.Competencia;
using DataTransferObject.Domain.Usuario;
using System;
using System.Threading.Tasks;
using Dapper;

namespace Colaboracao.Infra.Repositories.Competencia
{
    public class CompetenciaHistoricoRepository : ICompetenciaHistoricoRepository
    {
        private readonly ColaboradorContext _colaboradorContext;
        private readonly UsuarioLogadoDTO _usuarioLogado;
        private readonly IAspNetUser _aspNetUser;
        private readonly IDBConnection _dapperConnection; 
        public CompetenciaHistoricoRepository(ColaboradorContext colaboradorContext, IAspNetUser aspNetUser, IDBConnection dapperConnection)
        {
            _colaboradorContext = colaboradorContext;
            _aspNetUser = aspNetUser;
            _dapperConnection = dapperConnection;
            _usuarioLogado = _aspNetUser.GetUsuarioLogado();
        }

        public async Task<bool> InserirHistoricoCompetencia(string tipoCompetencia, string descricaoCompetencia, string situacao, string observacao, string cpfUsuarioCriacao)
        {
            var connection = _dapperConnection.GetConnection();
            
            var query = @"
                INSERT INTO tb_historico_competencia
                    (id, tipo_competencia_enum, descricao_competencia, situacao, observacao, codigo_interno_colaborador_alteracao, codigo_interno_colaborador_criacao)
                VALUES(UUID(), @TipoCompetencia, @DescricaoCompetencia, @Situacao, @Observacao, @UsuarioLogado, @CpfUsuarioCriacao);
            ";

            var parametros = new
            {
                TipoCompetencia = tipoCompetencia,
                DescricaoCompetencia = descricaoCompetencia,
                Situacao = situacao,
                Observacao = observacao,
                UsuarioLogado = _usuarioLogado.Cpf,
                CpfUsuarioCriacao = cpfUsuarioCriacao
            };

            var insert = await connection.ExecuteAsync(query, parametros);
            if(insert != 1)
                return false;
            return true;
        }
    }
}