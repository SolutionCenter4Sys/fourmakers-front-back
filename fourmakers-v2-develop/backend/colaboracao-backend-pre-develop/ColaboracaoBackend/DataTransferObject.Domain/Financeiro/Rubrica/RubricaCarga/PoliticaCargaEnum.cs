namespace DataTransferObject.Domain.Financeiro.Rubrica.RubricaCarga;

public enum PoliticaErroCarga
{
    ContinuarComAviso = 1,    // Linhas com erro são rejeitadas, mas segue o processamento
    RejeitarArquivo = 2       // Qualquer erro invalida a carga inteira
}

public enum PoliticaConflitoCarga
{
    NaoAtualizar = 1,        // Não altera registros já existentes (insere só os novos)
    AtualizarTodos = 2       // Atualiza todos os registros que já existem
}