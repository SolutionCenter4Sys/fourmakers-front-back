using DataTransferObject.Domain.MapaDeAlocacao.DetalharColaborador;
using System.Collections.Generic;

namespace DataTransferObject.Domain.MapaDeAlocacao
{
    public class DetalharColaboradorDTO
    {
        public string NomeColaborador { get; set; }
        public string CodigoColaborador { get; set; }
        public int? CodigoTBD { get; set; }
        public string FotoUrl { get; set; }
        public string NomeProfissionalSuperior { get; set; }
        public string CodProfissionalSuperior { get; set; }
        public double HorasAlocadas { get; set; }
        public double HorasDisponiveis { get; set; }
        public string Periodo { get; set; }
        public List<ProjetosDetalhadosDTO> Projetos { get; set; }
        public List<DisponibilidadeHoras> DisponibilidadeDeHoras { get; set; }
    }
}