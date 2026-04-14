namespace DataTransferObject.Domain.Financeiro.Holerite
{
    /// <summary>
    /// Enum que representa os tipos de processamento de holerite disponíveis no sistema
    /// </summary>
    public enum TipoProcessamentoHoleriteEnum
    {
        /// <summary>
        /// Holerite mensal
        /// </summary>
        Mensal = 0,

        /// <summary>
        /// Holerite de adiantamento
        /// </summary>
        Adiantamento = 1,
        
        /// <summary>
        /// Holerite de Férias
        /// </summary>
        Ferias = 2,
        
        /// <summary>
        /// Holerite de Décimo Terceiro
        /// </summary>
        DecimoTerceiro = 3,
        
        /// <summary>
        /// Holerite de Adiantamento Décimo Terceiro
        /// </summary>
        DecimoTerceiroAdiatamento = 4,
        
        /// <summary>
        /// Holerite de Adiantamento Décimo Terceiro
        /// </summary>
        InformeRendimentos = 5
    }
}

