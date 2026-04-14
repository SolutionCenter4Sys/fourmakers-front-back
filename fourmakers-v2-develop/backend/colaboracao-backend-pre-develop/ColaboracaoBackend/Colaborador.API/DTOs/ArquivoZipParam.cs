using Microsoft.AspNetCore.Http;

namespace Colaborador.API.DTOs;

public class ArquivoZipParam
{
    public IFormFile File { get; set; }
} 