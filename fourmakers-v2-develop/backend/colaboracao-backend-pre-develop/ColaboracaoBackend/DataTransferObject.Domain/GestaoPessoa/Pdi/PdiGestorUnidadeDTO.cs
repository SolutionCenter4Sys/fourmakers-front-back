namespace DataTransferObject.Domain.GestaoPessoa.Pdi
{
    /// <summary>Unidade (diretoria) associada ao time de um gestor em métricas PDI.</summary>
    public class PdiGestorUnidadeDTO
    {
        public string CodDiretoria { get; set; }
        /// <summary>Nome/descrição da diretoria em <c>tb_colaborador_org.diretoria</c>.</summary>
        public string Descricao { get; set; }
    }
}
