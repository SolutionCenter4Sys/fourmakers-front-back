namespace Apontamento.Domain.Enums
{
    public enum EnumStatusApontamentoGrupo
    {
        Pendente = 1,
        Aprovado = 2,
        Reprovado = 3,
        Deletado = 4
    }

    public enum EnumStatusApontamento
    {
        PendenteGestorProjeto = 1,
        PendenteGestorAdm = 2,
        ReprovadoGestorAdm = 3,
        ReprovadoGestorProjeto = 4,
        Aprovado = 5,
        Deletado = 6
    }
}