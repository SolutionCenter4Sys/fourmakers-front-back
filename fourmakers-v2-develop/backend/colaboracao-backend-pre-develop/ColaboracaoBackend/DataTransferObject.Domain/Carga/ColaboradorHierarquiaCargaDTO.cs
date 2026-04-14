using System;

namespace DataTransferObject.Domain.Carga
{
    public class ColaboradorHierarquiaCargaDTO
    {
        public String IdentificadorColaborador { get; set; }
        public String IdentificadorSuperior { get; set; }
    }
}