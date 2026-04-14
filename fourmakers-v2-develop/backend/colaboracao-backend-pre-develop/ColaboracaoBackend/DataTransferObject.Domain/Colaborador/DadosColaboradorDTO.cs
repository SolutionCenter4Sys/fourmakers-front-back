using DataTransferObject.Domain.Escolaridade;
using System.Collections.Generic;

namespace DataTransferObject.Domain.Colaborador
{
    public class DadosColaboradorDTO
    {
        public ColaboradorCvDadosDTO Colaborador { get; set; }
        public PerfilProfissionalDTO PerfilProfissional { get; set; }
        public List<EscolaridadeDTO> Escolaridades { get; set; }
    }
}