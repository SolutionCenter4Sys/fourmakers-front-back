using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataTransferObject.Domain.MapaDeAlocacao.Aderencia;

namespace DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.Aderencia
{
    public class CalculoAderenciaComPerfilEColaboradorDTO
    {
        public string CodigoInternoColaborador { get; set; }
        public Guid PerfilId { get; set; }
        public string NomePerfil { get; set; }
        public string NomeColaborador { get; set; }
        public string Cargo { get; set;  }
        public CalculoAderenciaDTO CalculoAderencia { get; set; }
    }
}