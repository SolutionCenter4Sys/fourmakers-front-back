using DataTransferObject.Domain.Vaga;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SRS.API.Controllers
{
    public partial class VagaController
    {
        // TODO: precisamos verificar se não está sendo utilizado, caso não esteja apagar na service e também no banco de dados

        //[HttpPost("CriarOrdemDeTrabalhoInterna")]
        //public async Task CriarOrdemDeTrabalhoInterna(List<VagaDTO> vagaDTO)
        //{
        //    try
        //    {
        //        var token = _aspNetUser.GetUsuarioLogado().Cpf;
        //        var success = await _vagaService.CriarOrdemDeTrabalhoInterna(vagaDTO, _aspNetUser.GetUsuarioLogado().Token);
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        [HttpGet("ListarVagaOrquestracao")]
        public async Task<ActionResult<IEnumerable<VagaOrquestracaoDTO>>> ListarVagaOrquestracao()
        {
            try
            {
                var usuarioLogadoEmail = _aspNetUser.GetUsuarioLogado().Email;
                var vagasOrquestracao = await _vagaService.ListarVagaOrquestracao(usuarioLogadoEmail);
                return Ok(vagasOrquestracao);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("ListarCandidatosPorVaga")]
        public async Task<ActionResult<IEnumerable<CanditatoVagaSrsDTO>>> ListarCandidatosPorVaga(long? idVaga)
        {
            try
            {
                var candidatosPorVaga = await _vagaService.ListarCandidatosPorVaga(idVaga);
                return Ok(candidatosPorVaga);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("TotalizadorCandidaturasVaga")]
        public async Task<ActionResult<IEnumerable<TotalizadoresVagaDTO>>> TotalizadoresVaga(long? idVaga)
        {
            try
            {
                var candidatosPorVaga = await _vagaService.TotalizadoresVaga(idVaga);
                return Ok(candidatosPorVaga);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}