using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper.Enum;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Calculos;
using DataTransferObject.Domain.Log;
using DataTransferObject.Domain.Match;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SRS.Domain.Interfaces.Service;
using System;
using System.Threading.Tasks;

namespace SRS.API.Controllers
{
    [Authorize]
    [HandleException]
    [Route("api/Vaga/[controller]")]
    [ApiController]
    [LogAction]
    public class CalculosController : ControllerBase
    {
        private readonly ILogCore _log;
        private readonly ICalculosService _calculosService;
        private readonly IAspNetUser _aspNetUser;

        public CalculosController(ILogCore log, ICalculosService calculosService, IAspNetUser aspNetUser)
        {
            _log = log;
            _calculosService = calculosService;
            _aspNetUser = aspNetUser;
        }

        [HttpPost("CalcularSalarioLiquido")]
        public async Task<ActionResult<ApiGenericResult<CalcularSalarioLiquidoOutputDTO>>> CalcularSalarioLiquido([FromBody] CalcularSalarioLiquidoInputDTO input)
        {
            try
            {
                // Validações básicas
                if (input == null)
                {
                    return BadRequest(new ApiGenericResult<CalcularSalarioLiquidoOutputDTO>
                    {
                        Sucesso = false,
                        Mensagem = "Dados de entrada não podem ser nulos"
                    });
                }

                // Executar cálculo (validação já está dentro do service)
                var resultado = await _calculosService.CalcularSalarioLiquidoAsync(input);

                var result = new ApiGenericResult<CalcularSalarioLiquidoOutputDTO>
                {
                    Retorno = resultado,
                    Sucesso = true,
                    Mensagem = "Cálculo realizado com sucesso"
                };

                return Ok(result);
            }
            catch (ApplicationException ex)
            {
                // Exceções de validação - retornar mensagem específica
                return BadRequest(new ApiGenericResult<CalcularSalarioLiquidoOutputDTO>
                {
                    Sucesso = false,
                    Mensagem = ex.Message
                });
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                return StatusCode(500, new ApiGenericResult<CalcularSalarioLiquidoOutputDTO>
                {
                    Sucesso = false,
                    Mensagem = e.Message
                });
            }
        }

        [HttpPost("SimularRemuneracaoTotal")]
        public async Task<ActionResult<ApiGenericResult<SimularRemuneracaoTotalOutputDTO>>> SimularRemuneracaoTotal([FromBody] SimularRemuneracaoTotalInputDTO input)
        {
            try
            {
                // Validações básicas
                if (input == null)
                {
                    return BadRequest(new ApiGenericResult<SimularRemuneracaoTotalOutputDTO>
                    {
                        Sucesso = false,
                        Mensagem = "Dados de entrada não podem ser nulos"
                    });
                }

                // Executar simulação (validação já está dentro do service)
                var resultado = await _calculosService.SimularRemuneracaoTotalAsync(input, _aspNetUser.GetUsuarioLogado().OrgId);

                // Verificar se todas as validações de política estão ok
                bool emConformidadeComAPolitica = VerificarSeEstaTudoOk(resultado);
                //resultado.EmConformidadeComAPolitica = emConformidadeComAPolitica;

                var result = new ApiGenericResult<SimularRemuneracaoTotalOutputDTO>
                {
                    Retorno = resultado,
                    Sucesso = true,
                    Mensagem = "Simulação realizada com sucesso"
                };

                return Ok(result);
            }
            catch (ApplicationException ex)
            {
                // Exceções de validação - retornar mensagem específica
                return BadRequest(new ApiGenericResult<SimularRemuneracaoTotalOutputDTO>
                {
                    Sucesso = false,
                    Mensagem = ex.Message
                });
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                return StatusCode(500, new ApiGenericResult<SimularRemuneracaoTotalOutputDTO>
                {
                    Sucesso = false,
                    Mensagem = e.Message
                });
            }
        }

        /// <summary>
        /// Verifica se todas as validações de política estão ok tanto na remuneração proposta quanto na pretendida
        /// </summary>
        private bool VerificarSeEstaTudoOk(SimularRemuneracaoTotalOutputDTO resultado)
        {
            // Verificar remuneração proposta
            bool propostaOk = VerificarRemuneracaoOk(resultado.PrimeiraOpcao);

            // Verificar remuneração pretendida (se existir)
            bool pretendidaOk = resultado.SegundaOpcao != null 
                ? VerificarRemuneracaoOk(resultado.SegundaOpcao)
                : true; // Se não houver pretendida, considera ok

            // Tudo está ok se ambas estiverem ok
            return propostaOk && pretendidaOk;
        }

        /// <summary>
        /// Verifica se todas as validações de política de uma remuneração estão ok
        /// </summary>
        private bool VerificarRemuneracaoOk(RemuneracaoDTO remuneracao)
        {
            if (remuneracao == null)
                return false;

            // Verificar todas as validações
            bool cltOk = remuneracao.ValidacaoCLT?.DentroDaPolitica ?? false;
            bool valeRefeicaoOk = remuneracao.ValidacaoValeRefeicao?.DentroDaPolitica ?? false;
            bool valeAlimentacaoOk = remuneracao.ValidacaoValeAlimentacao?.DentroDaPolitica ?? false;
            bool auxilioEducacaoOk = remuneracao.ValidacaoAuxilioEducacao?.DentroDaPolitica ?? false;
            bool mobilidadeOk = remuneracao.ValidacaoMobilidade?.DentroDaPolitica ?? false;
            bool ajudaDeCustoOk = remuneracao.ValidacaoAjudaDeCusto?.DentroDaPolitica ?? false;
            bool custoVagaUltrapassado = remuneracao.CustoVaga < remuneracao.CustoTotalEmpresa ? false : true;

            // Tudo está ok se todas as validações estiverem dentro da política
            return cltOk && valeRefeicaoOk && valeAlimentacaoOk && auxilioEducacaoOk && mobilidadeOk && ajudaDeCustoOk;
        }

        [HttpGet("CalcularAderenciaColaboradorVaga")]
        public async Task<ActionResult<ApiGenericResult<CandidatosMatchResponse>>> CalcularAderenciaColaboradorVaga([FromQuery] string vagaId, [FromQuery] string codigoInternoColaborador)
        {
            try
            {
                // Validações básicas
                if (string.IsNullOrWhiteSpace(vagaId))
                {
                    return BadRequest(new ApiGenericResult<CandidatosMatchResponse>
                    {
                        Sucesso = false,
                        Mensagem = "O ID da vaga é obrigatório."
                    });
                }

                if (string.IsNullOrWhiteSpace(codigoInternoColaborador))
                {
                    return BadRequest(new ApiGenericResult<CandidatosMatchResponse>
                    {
                        Sucesso = false,
                        Mensagem = "O código interno do colaborador é obrigatório."
                    });
                }

                // Executar cálculo de aderência
                var resultado = await _calculosService.CalcularAderenciaColaboradorVaga(vagaId, codigoInternoColaborador);

                var result = new ApiGenericResult<CandidatosMatchResponse>
                {
                    Retorno = resultado,
                    Sucesso = true,
                    Mensagem = "Cálculo de aderência realizado com sucesso"
                };

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                // Exceções de validação - retornar mensagem específica
                return BadRequest(new ApiGenericResult<CandidatosMatchResponse>
                {
                    Sucesso = false,
                    Mensagem = ex.Message
                });
            }
            catch (ApplicationException ex)
            {
                // Exceções de aplicação - retornar mensagem específica
                return BadRequest(new ApiGenericResult<CandidatosMatchResponse>
                {
                    Sucesso = false,
                    Mensagem = ex.Message
                });
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                return StatusCode(500, new ApiGenericResult<CandidatosMatchResponse>
                {
                    Sucesso = false,
                    Mensagem = e.Message
                });
            }
        }
    }
}
