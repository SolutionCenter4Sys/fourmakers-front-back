using System.Collections.Generic;

namespace DataTransferObject.Domain.Apontamento
{
    public class ApontamentoMensalDTO
    {
        public ProjetoApontamentoMensalDTO Projeto { get; set; }
        public decimal Horas { get; set; }
        public bool ApontamentoReprovado { get; set; }
        public int CodStatusGrupoMensal { get; set; }
        // public string DescricaoStatusMensal { get; set; }
        public int? PrioridadeStatusApontamentoGrupo { get; set; }
        public List<AprovadorApontamentoMensalDTO> Aprovadores { get; set; }
        public string Observacao { get; set; }
        public string Atividade { get; set; }
    }

    public class AprovadorApontamentoMensalDTO
    {
        public string Cpf { get; set; }
        public string Nome { get; set; }
    }
}