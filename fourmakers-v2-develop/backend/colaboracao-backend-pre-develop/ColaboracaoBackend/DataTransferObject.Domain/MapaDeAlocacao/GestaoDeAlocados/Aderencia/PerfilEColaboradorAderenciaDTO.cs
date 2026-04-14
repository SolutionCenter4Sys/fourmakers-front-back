using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.Aderencia
{
    public class PerfilEColaboradorAderenciaDTO
    {
        public string CodigoInternoColaborador { get; set; }
        public Guid PerfilId { get; set; }
    }
}