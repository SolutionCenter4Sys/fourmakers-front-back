using System.Collections.Generic;
using DataTransferObject.Domain.Vaga;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfil;

namespace DataTransferObject.Domain.SRS.Vagas
{
    public class ListarVagasRecrutamentoEPerfisResult
    {
        public IEnumerable<VagaRecrutamentoDTO> VagasRecrutamento { get; set; }
        public IEnumerable<ListarPerfisResult> Perfis { get; set; }
        public Dictionary<string, int> TotalVagas { get; set; }
        public int TotalPerfis { get; set; }
        public int TotalInscritoVagas { get; set; }

        public ListarVagasRecrutamentoEPerfisResult()
        {
            VagasRecrutamento = new List<VagaRecrutamentoDTO>();
            Perfis = new List<ListarPerfisResult>();
            TotalVagas = new Dictionary<string, int>();
        }
    }
}
