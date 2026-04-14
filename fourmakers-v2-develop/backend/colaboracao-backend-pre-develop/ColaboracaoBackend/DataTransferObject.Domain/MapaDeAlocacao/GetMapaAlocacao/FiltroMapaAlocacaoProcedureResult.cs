namespace DataTransferObject.Domain.MapaDeAlocacao.GetMapaAlocacao
{
    public class FiltroMapaAlocacaoProcedureResult
    {
        public string codigo_colaborador { get; set; }

        public string nome { get; set; }

        public string idiomas { get; set; }

        public string hard_skills { get; set; }

        public string gestores { get; set; }

        public string gestores_nome { get; set; }

        public bool ativo { get; set; }

        public int tb_org_id { get; set; }

        public string cod_diretoria { get; set; }

        public string diretoria { get; set; }

        public int eh_tbd { get; set; }

        public string codigo_interno_colaborador { get; set; }

        public int mes { get; set; }

        public int ano { get; set; }

        public double horas { get; set; }

        public string status_colaborador_periodo_alocacao { get; set; }
    }
}