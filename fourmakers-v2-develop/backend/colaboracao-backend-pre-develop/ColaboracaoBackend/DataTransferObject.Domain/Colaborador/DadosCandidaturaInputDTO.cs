namespace DataTransferObject.Domain.Colaborador
{
    /// <summary>
    /// DTO de entrada para dados da candidatura
    /// </summary>
    public class DadosCandidaturaInputDTO
    {
        /// <summary>
        /// ID da candidatura (GUID) - obrigatório
        /// </summary>
        public string IdCandidatura { get; set; }

        /// <summary>
        /// GUID do modelo de trabalho (deve existir em tb_modelo_trabalho) - obrigatório
        /// </summary>
        public string ModeloTrabalhoId { get; set; }

        /// <summary>
        /// Quantidade de dias presenciais desejados (1, 2, 3 ou 4) - opcional
        /// Atualiza tb_candidato_vaga.quantidade_dias_presencial
        /// </summary>
        public int? QuantidadeDiasPresencial { get; set; }

        /// <summary>
        /// Pretensão salarial (valor em R$) - opcional
        /// Atualiza tb_candidato_vaga.pretensao_salarial
        /// </summary>
        public decimal? PretencaoSalarial { get; set; }
    }
}

