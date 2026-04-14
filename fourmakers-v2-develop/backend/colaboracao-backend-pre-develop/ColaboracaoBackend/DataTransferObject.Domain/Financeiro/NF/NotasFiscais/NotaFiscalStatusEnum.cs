namespace DataTransferObject.Domain.Financeiro.NotaFiscal;

public enum NotaFiscalStatusEnum : int
{
    EM_PREPARACAO = 1,
    AGUARDA_EMISSAO_DA_NF = 2,
    NF_EM_ANALISE = 3,
    NF_REPROVADA = 4,
    NF_APROVADA = 5,
    NF_PAGA = 6,
    NF_CANCELADA = 7,
    PROCESSAMENTO_CNAB = 8,
}