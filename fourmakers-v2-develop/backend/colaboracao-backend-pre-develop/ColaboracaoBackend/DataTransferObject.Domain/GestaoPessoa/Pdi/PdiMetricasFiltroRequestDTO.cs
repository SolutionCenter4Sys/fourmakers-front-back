using System;

namespace DataTransferObject.Domain.GestaoPessoa.Pdi
{
    /// <summary>
    /// Filtros da tela Métricas de PDI (org + restrição de diretoria + datas + gestor + colaborador + status).
    /// </summary>
    public class PdiMetricasFiltroRequestDTO
    {
        /// <summary>Cpf/código interno do gestor (time = gestor + subordinados).</summary>
        public string CpfGestor { get; set; }

        /// <summary>Cpf/código interno do colaborador (um único titular do PDI).</summary>
        public string CpfColaborador { get; set; }

        /// <summary>Código da diretoria/unidade (tb_colaborador_org.cod_diretoria).</summary>
        public string CodDiretoria { get; set; }

        /// <summary>Data início: data_criacao do PDI &gt;= (meia-noite UTC/local conforme binding).</summary>
        public DateTime? DataInicio { get; set; }

        /// <summary>Data conclusão (tela): dead_line do PDI &lt;=.</summary>
        public DateTime? DataConclusao { get; set; }

        /// <summary>Status separados por vírgula: NOT_STARTED,IN_ANALYSIS,IN_PROGRESS,COMPLETED,CANCELLED. Vazio = todos.</summary>
        public string Statuses { get; set; }

        /// <summary>Página da lista de PDIs na busca principal (>= 1). Padrão 1.</summary>
        public int? Pagina { get; set; }

        /// <summary>Tamanho da página da lista de PDIs (1–200). Padrão 25.</summary>
        public int? TamanhoPagina { get; set; }
    }
}
