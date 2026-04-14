using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Usuario;
using Financeiro.Domain.Interfaces.IntegracaoBancaria.RetornoBancaria;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Financeiro.IntegracaoBancaria.RetornoBancaria;

namespace Financeiro.API.Controllers.IntegracaoBancaria
{
    [Authorize]
    [Route("api/Financeiro/IntegracaoBancaria/[controller]")]
    [HandleException]
    [ApiController]
    public class RetornoBancariaController : ControllerBase
    {
        private readonly IRetornoBancariaService _retornoBancariaService;
        private readonly UsuarioLogadoDTO _usuarioLogado;

        public RetornoBancariaController(IRetornoBancariaService retornoBancariaService, IAspNetUser aspNetUser)
        {
            _retornoBancariaService = retornoBancariaService;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
        }

        [HttpPost("ProcessarRetornoCnab")]
        public async Task<ActionResult<ApiGenericResult<ProcessarRetornoCnabResult>>> ProcessarRetornoCnab()
        {
            try
            {
                // Pegar o hash da remessa do form
                StringValues hashRemessaParam;
                StringValues tipoRemessaParam;
                Request.Form.TryGetValue("hashRemessa", out hashRemessaParam);
                Request.Form.TryGetValue("tipoRemessa", out tipoRemessaParam);
                string hashRemessa = hashRemessaParam.ToString();
                string tipoRemessa = tipoRemessaParam.ToString();

                if (string.IsNullOrEmpty(hashRemessa))
                {
                    return BadRequest("Hash da remessa não fornecido.");
                }

                string conteudoArquivo = null;
                string nomeArquivo = null;

                // Processar arquivo enviado
                foreach (var formFile in Request.Form.Files)
                {
                    if (formFile.Length > 0)
                    {
                        nomeArquivo = formFile.FileName;
                        using (var reader = new StreamReader(formFile.OpenReadStream(), Encoding.UTF8))
                        {
                            conteudoArquivo = await reader.ReadToEndAsync();
                        }
                        break; // Processar apenas o primeiro arquivo
                    }
                }

                if (string.IsNullOrEmpty(conteudoArquivo))
                {
                    return BadRequest("Arquivo de retorno não fornecido ou vazio.");
                }

                var result = await _retornoBancariaService.ProcessarRetornoCnab(
                    hashRemessa,
                    nomeArquivo,
                    conteudoArquivo,
                    _usuarioLogado.Cpf,
                    tipoRemessa,
                    _usuarioLogado.OrgId);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Sucesso = false, Mensagem = ex.Message });
            }
        }
    }
}
