namespace DataTransferObject.Domain.Apontamento.FolhaPonto
{
    /// <summary>
    /// Enum que representa os tipos de fila disponíveis no sistema
    /// </summary>
    public enum TipoFilaEnum
    {
        /// <summary>
        /// Fila para processamento de folha de ponto
        /// </summary>
        FOLHAPONTO = 1,

        /// <summary>
        /// Fila para processamento de holerite
        /// </summary>
        HOLERITE = 2,

        /// <summary>
        /// Fila para análise de holerite
        /// </summary>
        ANALISE_HOLERITE = 3,

        /// <summary>
        /// Fila para holerite já analisado
        /// </summary>
        HOLERITE_ANALISADO = 4,
        
        /// <summary>
        /// Fila para processamento de rubrica carga
        /// </summary>
        RUBRICA_CARGA = 5
    }

} 