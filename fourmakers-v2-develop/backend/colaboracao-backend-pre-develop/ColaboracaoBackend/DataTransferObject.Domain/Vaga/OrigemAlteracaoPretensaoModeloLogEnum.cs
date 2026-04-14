namespace DataTransferObject.Domain.Vaga
{
    /// <summary>
    /// Origem registrada em tb_candidato_vaga_log_pretensao_modelo.origem_alteracao
    /// </summary>
    public enum OrigemAlteracaoPretensaoModeloLogEnum
    {
        /// <summary>CandidatarSe (recrutamento / candidatar outra pessoa)</summary>
        Candidato = 0,

        /// <summary>EditarCandidaturaColaboradorComDadosDemograficos</summary>
        Simulador = 1,

        /// <summary>AtualizarCandidatura (Kanban)</summary>
        MovimentacaoKanban = 2
    }

    public static class OrigemAlteracaoPretensaoModeloLogEnumExtensions
    {
        public static string ParaTextoPersistencia(this OrigemAlteracaoPretensaoModeloLogEnum valor) =>
            valor switch
            {
                OrigemAlteracaoPretensaoModeloLogEnum.Candidato => "Candidato",
                OrigemAlteracaoPretensaoModeloLogEnum.Simulador => "Simulador",
                OrigemAlteracaoPretensaoModeloLogEnum.MovimentacaoKanban => "Movimentacao Kanban",
                _ => valor.ToString()
            };
    }
}
