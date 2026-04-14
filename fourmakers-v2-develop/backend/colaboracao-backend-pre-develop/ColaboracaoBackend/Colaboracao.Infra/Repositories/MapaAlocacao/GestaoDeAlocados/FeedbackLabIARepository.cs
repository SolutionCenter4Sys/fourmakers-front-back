using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colaboracao.Core.Interfaces;
using Core.Domain.MapaAlocacao.GestaoDeAlocados;
using Dapper;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados;

namespace Colaboracao.Infra.Repositories.MapaAlocacao.GestaoDeAlocados
{

    public class FeedbackLabIARepository: IFeedbackLabIARepository
    {
        
        private readonly IDBConnection _dapperConnection;
        
        public FeedbackLabIARepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }
        
        public async Task<FeedbackLabIADTO> InserirFeedbackLabIA(FeedbackLabIAParam param,  string codigoColaborador)
        {
           var _connection = _dapperConnection.GetConnection();
           var feedback = new FeedbackLabIADTO();
           var id = Guid.NewGuid();
           var dataRequisicao = DateTime.UtcNow;

           var querry = @"
                INSERT INTO tb_feedback_lab_ia
                (
                 id,
                 tb_colaborador_codigo_interno_colaborador,
                 contexto,
                 pergunta,
                 resposta,
                 aprovado,
                 data_requisicao
                ) 
                VALUES 
                (
                 @Id,
                 @CodigoUsuario,
                 @Contexto,
                 @Pergunta,
                 @Resposta,
                 @Aprovado,
                 @DataRequisicao
                );";
           
              var linhasInseridas = await _connection.ExecuteAsync(
                querry,
                new
                {
                    Id = id,
                    CodigoUsuario = codigoColaborador,
                    param.Contexto,
                    param.Pergunta,
                    param.Resposta,
                    param.Aprovado,
                    DataRequisicao = dataRequisicao
                }
            );
              
            if (linhasInseridas > 0)
            {
                feedback.ID = id.ToString();
                feedback.CodigoUsuario = codigoColaborador;
                feedback.Contexto = param.Contexto;
                feedback.Pergunta = param.Pergunta;
                feedback.Resposta = param.Resposta;
                feedback.Aprovado = param.Aprovado;
                feedback.DataRequisicao = dataRequisicao.ToString("yyyy-MM-dd HH:mm:ss");
            }
            return feedback;
        }
        
        public async Task<List<FeedbackLabIADTO>> ListarFeedbacksLabIa()
        {
            var _connection = _dapperConnection.GetConnection();
            var query = @"
                SELECT
                    id as Id,
                    tb_colaborador_codigo_interno_colaborador as CodigoUsuario,
                    contexto as Contexto,
                    pergunta as Pergunta,
                    resposta as Resposta,
                    aprovado as Aprovado,
                    data_requisicao as DataRequisicao
                FROM
                    tb_feedback_lab_ia;";

            var result = await _connection.QueryAsync<FeedbackLabIADTO>(query);
            return result.ToList();
        }
        
    }
    
}
