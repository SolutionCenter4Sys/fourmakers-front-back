using DataTransferObject.Domain.Competencia;
using System;
using System.Collections.Generic;

namespace DataTransferObject.Domain.Vaga
{
    public class ListarVagasCadastradasFourmakersResult
    {
        public int CodigoVaga { get; set; }
        public DateTime? DataAprovacao { get; set; }
        public string PerfilId { get; set; }
        public string NomePerfil { get; set; }
        public string CodigoCliente { get; set; }
        public string NomeCliente { get; set; }
        public int Candidaturas { get; set; }
        public List<SkillNivelDTO> Habilidades { get; set; }
        public string NomeAprovador { get; set; }
        public string StatusVaga { get; set; }
    }
}