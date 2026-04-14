using Microsoft.AspNetCore.Http;

namespace Colaborador.API.DTOs
{
    public class ArquivoExcelParam
    {
        public IFormFile File { get; set; }
    }
} 