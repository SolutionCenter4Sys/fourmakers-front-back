using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Domain.MapaAlocacao.GestaoDeAlocados;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados;
using MapaDeAlocacao.Domain.Interfaces.GestaoDeAlocados;

using Logs.Infra.Attributes;

namespace MapaDeAlocacao.Domain.Impl.GestaoDeAlocados
{
    [LogDomainClass]
    public class FeedbackLabIAService : IFeedbackLabIAService
    {
        private readonly IFeedbackLabIARepository _feedbackLabIARepository;
        
    
        public FeedbackLabIAService(IFeedbackLabIARepository feedbackLabIARepository)
        {
            _feedbackLabIARepository = feedbackLabIARepository;
        }

        public async Task<ApiGenericResult<FeedbackLabIADTO>> InserirFeedbackLabIA(FeedbackLabIAParam param,  string codigoColaborador)
        {
            try
            { 
                var res =  await _feedbackLabIARepository.InserirFeedbackLabIA(param,  codigoColaborador);
                
                
                if (res == null)
                {
                    return new ApiGenericResult<FeedbackLabIADTO>
                    {
                        Retorno = null,
                        Sucesso = false,
                        Mensagem = "Erro ao inserir feedback"
                            
                    };
                }
                
                return new ApiGenericResult<FeedbackLabIADTO>
                        {
                            Retorno = res,
                            Sucesso = true,
                            Mensagem = "Feedback inserido com sucesso"
                            
                        };
                    
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new Exception($"Não foi possível inserir o feedback: {e.Message}");
            }
           
        }

        public async Task<ApiGenericResult<List<FeedbackLabIADTO>>> ListarFeedbacksLabIa()
        {
            try
            {
                var res =  await _feedbackLabIARepository.ListarFeedbacksLabIa();
                
                if (res == null)
                {
                    return new ApiGenericResult<List<FeedbackLabIADTO>>
                    {
                        Retorno = null,
                        Sucesso = false,
                        Mensagem = "Erro ao listar feedbacks"
                            
                    };
                }
                return new ApiGenericResult<List<FeedbackLabIADTO>>
                        {
                            Retorno = res,
                            Sucesso = true,
                            Mensagem = "Feedbacks listados com sucesso"
                            
                        };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new Exception($"Não foi possível listar os feedbacks: {e.Message}");
            }
            
        }
    }
    
}
