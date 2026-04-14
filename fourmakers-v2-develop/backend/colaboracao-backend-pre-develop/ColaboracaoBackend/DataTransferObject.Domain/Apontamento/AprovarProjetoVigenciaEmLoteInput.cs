using System;
using System.Collections.Generic;

namespace DataTransferObject.Domain.Apontamento
{
    public class AprovarProjetoVigenciaEmLoteInput
    {
        public int Mes { get; set; }
        public int Ano { get; set; }
        public string Justificativa { get; set; }
        public List<AprovarProjetoVigenciaProjetoDTO> Projetos { get; set; }
        public DateTime DataColetaDeDados { get; set; }
    }

    public class AprovarProjetoVigenciaProjetoDTO
    {
        public string Cpf { get; set; }
        public string CodProjeto { get; set; }
    }
}