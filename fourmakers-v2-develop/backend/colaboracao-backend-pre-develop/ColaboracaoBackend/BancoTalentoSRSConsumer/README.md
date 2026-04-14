# BancoTalentoSRSConsumer

Este é um consumer SQS que processa mensagens para sincronização de colaboradores no Banco de Talentos SRS.

## Funcionalidade

O consumer lê mensagens da fila AWS SQS configurada na variável de ambiente `AWS_SQS_QUEUE_URL_BANCOTALENTOS_SRS` e processa mensagens no formato:

```json
{
  "perfilIN": "carlos-domingos"
}
```

Para cada mensagem recebida, o consumer chama o método `SincronizarColaboradorBancoTalentos` do serviço `IImportacaoColaboradorService`.

## Configuração

### Variáveis de Ambiente

- `AWS_SQS_QUEUE_URL_BANCOTALENTOS_SRS`: URL da fila SQS para mensagens do Banco de Talentos
- `DEFAULT_ORG_ID`: ID da organização padrão (opcional, padrão: 1)
- `DEFAULT_TOKEN`: Token padrão para autenticação (opcional, padrão: "sistema-background-process")

### Arquivo de Configuração

Configure o `appsettings.json` com as seguintes seções:

```json
{
  "AWS": {
    "Region": "us-east-1",
    "QueueBancoTalentosUrl": "https://sqs.us-east-1.amazonaws.com/ACCOUNT/BancoTalentoSRS"
  },
  "Default": {
    "OrgId": "1",
    "Token": "sistema-background-process"
  }
}
```

## Dependências

O projeto utiliza as mesmas dependências do `Colaborador.Application`:

- `AddBaseGeralServicosColaboracao`
- `AddColaboradorServicesDependencies`
- `AddTemplateRepositoryDependencies`
- `AddHistoricoCVDependencies`
- `AddCidadaniaServicesDependencies`

## Estrutura

O projeto segue a mesma estrutura do `FolhaColaboradorConsumer`:

- `Worker.cs`: Classe principal que consome mensagens da fila SQS
- `Program.cs`: Configuração de dependências e inicialização
- `BancoTalentoSRSMessage.cs`: DTO para representar a mensagem recebida

## Health Check

O consumer expõe um endpoint de health check em `/health` para monitoramento.

## Execução

Para executar o consumer:

```bash
dotnet run --project BancoTalentoSRSConsumer
```
