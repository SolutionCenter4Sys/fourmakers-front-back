namespace DataTransferObject.Domain.Colaborador
{
    /// <summary>
    /// DTO de entrada para ColaboradorSaude que aceita grupoDeRiscoCovid como booleano
    /// </summary>
    public class ColaboradorSaudeInputDTO
    {
        public EnumPCD? PCD { get; set; }

        /// <summary>
        /// Número do EnumPCD enviado pelo front-end (0, 1, 2, 3, 4, 5, 6)
        /// </summary>
        public int? EnumPCD { get; set; }

        /// <summary>
        /// Grupo de risco COVID - aceita booleano (true/false) ou número (0/1)
        /// </summary>
        public sbyte GrupoDeRiscoCovid { get; set; }

        public string CondicaoDeSaudeRelevante { get; set; }
    }
}

