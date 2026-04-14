namespace DataTransferObject.Domain.MapaDeAlocacao
{
    public class FiltroMapaAlocacaoDTO
    {
        public string Cpf { get; set; }
        public int? CodigoTbd { get; set; }

        public string Nome { get; set; }

        public int Ativo { get; set; }

        // public string HardSkills { get; set; }

        // public string Idioma { get; set; }

        public string Diretoria { get; set; }
        public string CodigoDiretoria { get; set; }
    }
}