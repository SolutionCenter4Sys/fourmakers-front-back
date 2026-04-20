# Inscrever CV de candidato — Desenvolvimento

## Arquitetura
- **Camada de apresentação:** `InscreverCandidatoModal` em `presentation/components/gestao-vagas/`.
- **Domínio:** `ImportarColaboradorResult` (sucesso, mensagem, codigoColaborador); Use Cases `ImportarColaboradorUseCase`, `ImportarColaboradorLinkedinUseCase`.
- **Dados:** `CurriculoColaboradorApi` (ImportarColaborador, ImportarColaboradorLinkedin); `CurriculoColaboradorRepositoryImpl` mapeia resposta da API para domínio.

## Contratos API
- **ImportarColaborador (POST):** multipart/form-data, campo `File` (PDF). Resposta: `sucesso`, `mensagem` ou `mensagen` (backend), `codigoColaborador`, `erros`.
- **ImportarColaboradorLinkedin (POST):** JSON `{ PerfilIN: string }`. Resposta: mesmo envelope.
- O repositório normaliza `mensagen` para `mensagem` no resultado de domínio.

## Fluxo
1. Usuário seleciona PDF ou preenche LinkedIn e clica em Importar.
2. Modal chama Use Case (PDF ou LinkedIn); não acessa `@data/api` diretamente.
3. Se `res.sucesso === false`: estado local `importError` é preenchido com `res.mensagem`; Alert destrutivo é exibido no modal; modal permanece aberto.
4. Se sucesso: toast, `onSuccess?.(codigoColaborador)`, fechamento do modal e limpeza dos campos.
5. Em exceção (rede, etc.): `importError` e toast com a mensagem; modal permanece aberto. A mensagem exibida é normalizada por `getMensagemAmigavelErro` (shared/utils/errorMessageUtils): erros de rede (failed to fetch, network error, load failed, etc.) são mapeados para "Sem conexão com a internet. Verifique sua rede e tente novamente."

## Rastreabilidade
- Ações significativas do usuário (importar CV) devem utilizar `logUserAction` (Firebase) conforme ARCHITECTURE.md (opcional neste escopo da alteração atual).
