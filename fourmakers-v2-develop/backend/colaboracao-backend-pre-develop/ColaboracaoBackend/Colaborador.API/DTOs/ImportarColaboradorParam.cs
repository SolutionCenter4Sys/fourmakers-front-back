using Microsoft.AspNetCore.Http;

namespace Colaborador.API.DTOs;

public class ImportarColaboradorParam
{
    public IFormFile File { get; set; }
}