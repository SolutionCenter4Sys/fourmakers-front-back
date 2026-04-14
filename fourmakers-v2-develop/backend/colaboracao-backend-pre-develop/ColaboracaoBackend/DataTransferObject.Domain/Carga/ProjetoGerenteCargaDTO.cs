using System;

namespace DataTransferObject.Domain.Carga
{
    public class ProjetoGerenteCargaDTO
    {
        public String IdentificadorProjeto { get; set; }
        public String IdentificadorColaboradorGerente { get; set; }
        public String IdentificadorTipoGerente { get; set; }
    }
}