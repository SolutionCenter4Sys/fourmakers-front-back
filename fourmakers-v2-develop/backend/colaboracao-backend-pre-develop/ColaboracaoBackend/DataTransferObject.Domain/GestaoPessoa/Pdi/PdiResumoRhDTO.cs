using System;

namespace DataTransferObject.Domain.GestaoPessoa.Pdi
{
    /// <summary>Resumo de PDI para listagem RH (org), com diretoria e gestor imediato na hierarquia.</summary>
    public class PdiResumoRhDTO
    {
        public string ColaboradorId { get; set; }

        public string NomeColaborador { get; set; }

        /// <summary>Nome do gestor imediato (<c>tb_colaborador_hierarquia</c>, primeiro nível acima do colaborador).</summary>
        public string NomeColaboradorSuperiorImediato { get; set; }

        /// <summary>Descrição da unidade/diretoria do colaborador na org (<c>tb_colaborador_org.diretoria</c>).</summary>
        public string DescricaoDiretoria { get; set; }

        public Guid PdiId { get; set; }
        public string Titulo { get; set; }
        public string Status { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        /// <summary>0..1 (planos concluídos / total), igual ao endpoint de métricas PDI.</summary>
        public double Progress { get; set; }
        /// <summary>Maior deadline dos planos de ação (coluna previsão na UI de métricas).</summary>
        public DateTime? Previsao { get; set; }
    }
}
