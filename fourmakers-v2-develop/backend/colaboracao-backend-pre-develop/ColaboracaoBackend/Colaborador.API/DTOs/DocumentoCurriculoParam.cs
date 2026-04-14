using Microsoft.AspNetCore.Http;

namespace Colaborador.API.DTOs;

public class DocumentoCurriculoParam
{
    public IFormFile File { get; set; }
}