using System.Collections.Generic;

namespace DataTransferObject.Domain.MapaDeAlocacao
{
    public class AlocacaoMensalResultDTO
    {
        public int Mes { get; set; }
        public int Ano { get; set; }
        public List<ColaboradorAlocacaoDTO> Alocacoes { get; set; } = new List<ColaboradorAlocacaoDTO>();
    }

    public class ColaboradorAlocacaoDTO
    {
        public string NomeColaborador { get; set; }
        public string CodigoColaborador { get; set; }
        public string CodigoInternoColaborador { get; set; }
        public List<ProjetoAlocacaoMensalDTO> Projetos { get; set; } = new List<ProjetoAlocacaoMensalDTO>();
    }

    public class ProjetoAlocacaoMensalDTO
    {
        public string CodigoProjeto { get; set; }
        public string NomeProjeto { get; set; }
        public double HorasUteis { get; set; }
    }
}
