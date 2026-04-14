using System.Collections.Generic;

namespace DataTransferObject.Domain.GestaoPessoa.Pdi
{
    /// <summary>
    /// Item de gestor para listagem/filtro (ex.: dropdown Métricas de PDI).
    /// </summary>
    public class PdiGestorItemDTO
    {
        /// <summary>Código interno do gestor (cpf/codigo_interno_colaborador).</summary>
        public string Cpf { get; set; }

        /// <summary>Nome completo do gestor.</summary>
        public string Nome { get; set; }

        /// <summary>Diretorias/unidades dos subordinados ativos desse gestor na org (distintas).</summary>
        public List<PdiGestorUnidadeDTO> Unidades { get; set; } = new List<PdiGestorUnidadeDTO>();
    }
}
