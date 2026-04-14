using System.Collections.Generic;

namespace DataTransferObject.Domain.MapaDeAlocacao
{
    public class ConsultarColaboradorProjetoDTO
    {
        public ResumoConsultarColaboradorProjetoDTO ResumoConsultarColaboradorProjeto { get; set; }
        public List<ForcaMensalProjetoDTO> forcaMensalProjeto { get; set; }

        public List<ColaboradorAlocadoProjetoDTO> colaboradorAlocadoProjeto { get; set; }
    }
}