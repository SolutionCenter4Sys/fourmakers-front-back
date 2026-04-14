using DataTransferObject.Domain.Colaborador;
using System;

namespace DataTransferObject.Domain.Endosso
{
    public class EndossoColaborador
    {
        public DateTime DataEndosso { get; set; }
        public ColaboradorDTO Colaborador { get; set; }
        public TipoEndossoDTO TipoEndosso { get; set; }
    }
}