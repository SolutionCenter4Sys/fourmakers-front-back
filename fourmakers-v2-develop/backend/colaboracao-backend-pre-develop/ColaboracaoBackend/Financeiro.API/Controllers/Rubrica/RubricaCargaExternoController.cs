using Colaboracao.Core;
using Colaboracao.Helper.Util;
using DataTransferObject.Domain.Arquivo;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Financeiro.Rubrica.RubricaCarga;
using Financeiro.Domain.Interfaces.Rubrica.RubricaCarga;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.IO;

using Logs.Infra.Attributes;

namespace Financeiro.API.Controllers.Rubrica;

[Route("api/Financeiro/Rubrica/[controller]")]
[ApiController]
[HandleException]
[LogAction]
public class RubricaCargaExternoController : ControllerBase
{

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IRubricaCargaExternoService _rubricaCargaExternoService;

    public RubricaCargaExternoController(IHttpContextAccessor httpContextAccessor,
                             IRubricaCargaExternoService rubricaCargaExternoService)
    {
        _httpContextAccessor = httpContextAccessor;
        _rubricaCargaExternoService = rubricaCargaExternoService;
    }

    [HttpPost("CargaMassiva/csv")]
    [HandleException]
    public async Task<ActionResult<ApiGenericResult<RubricaCargaMassivaResult>>> CargaMassiva([FromQuery] int politicaErro, [FromQuery] int politicaConflito)
    {
        var result = new ApiGenericResult<RubricaCargaMassivaResult>();

        var arquivoBase64 = await CapturarArquivoCSV();
        if (string.IsNullOrEmpty(arquivoBase64.conteudo))
        {
            result.Sucesso = false;
            result.Mensagem = "Nenhum arquivo CSV válido foi enviado";
            return BadRequest(result);
        }

        var tokenSistema = AuthorizationUtil.ObterTokenBearer(_httpContextAccessor.HttpContext);

        var input = new RubricaCargaMassivaInput
        {
            PoliticaErro = (PoliticaErroCarga)politicaErro,
            PoliticaConflito = (PoliticaConflitoCarga)politicaConflito,
            CargaCSVBase64 = arquivoBase64.conteudo
        };

        var base64DTO = new Base64DTO { Base64 = input.CargaCSVBase64, Tipo = ArquivoTipoEnum.csv };

        var fileResult = await _rubricaCargaExternoService.ProcessarCargaMassiva(tokenSistema, base64DTO, input.PoliticaErro, input.PoliticaConflito, "arquivo_carga_massiva.csv");

        result = fileResult;

        return Ok(result);
    }

    [HttpPost("CargaMassiva/json")]
    [HandleException]
    public async Task<ActionResult<ApiGenericResult<RubricaCargaMassivaResult>>> CargaMassivaJson([FromQuery] int politicaErro, [FromQuery] int politicaConflito, [FromBody] List<RubricaCargaJsonItemDTO> rubricasJson)
    {
        var result = new ApiGenericResult<RubricaCargaMassivaResult>();

        if (rubricasJson == null || !rubricasJson.Any())
        {
            result.Sucesso = false;
            result.Mensagem = "Nenhum dado JSON válido foi enviado";
            return BadRequest(result);
        }

        var tokenSistema = AuthorizationUtil.ObterTokenBearer(_httpContextAccessor.HttpContext);

        var csvBase64 = await _rubricaCargaExternoService.ConverterJsonParaCSV(rubricasJson);
        var base64DTO = new Base64DTO { Base64 = csvBase64, Tipo = ArquivoTipoEnum.csv };

        var fileResult = await _rubricaCargaExternoService.ProcessarCargaMassiva(tokenSistema, base64DTO, (PoliticaErroCarga)politicaErro, (PoliticaConflitoCarga)politicaConflito, "arquivo_carga_massiva.json");

        result = fileResult;

        return Ok(result);
    }

    private async Task<(string conteudo, string nomeArquivo)> CapturarArquivoCSV()
    {
        foreach (var formFile in Request.Form.Files)
        {
            if (formFile.Length > 0 && Path.GetExtension(formFile.FileName).ToLower() == ".csv")
            {
                using (var reader = new StreamReader(formFile.OpenReadStream(), System.Text.Encoding.UTF8, detectEncodingFromByteOrderMarks: true))
                {
                    string conteudoTexto = await reader.ReadToEndAsync();
                    if (!string.IsNullOrEmpty(conteudoTexto))
                    {
                        byte[] arquivo = System.Text.Encoding.UTF8.GetBytes(conteudoTexto);
                        var base64 = Convert.ToBase64String(arquivo);
                        return (base64, formFile.FileName);
                    }
                }
            }
        }
        
        return (null, null);
    }


}