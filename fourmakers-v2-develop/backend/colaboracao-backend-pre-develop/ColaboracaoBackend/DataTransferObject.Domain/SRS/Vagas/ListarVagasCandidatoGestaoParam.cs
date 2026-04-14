using System.Collections.Generic;

namespace DataTransferObject.Domain.SRS.Vagas
{
    public class ListarVagasCandidatoGestaoParam
    {
        public string Pesquisa { get; set; }
        public string Titulo { get; set; }
        public string Colaborador { get; set; }
        public List<int> StatusList { get; set; }
    }
}