# Inscrever CV de candidato — Regras de negócio

## Objetivo
Permitir inscrever um candidato em uma vaga a partir do currículo em PDF ou do perfil LinkedIn, com feedback claro em caso de falha na importação.

## Regras
- O usuário pode enviar **um** currículo em PDF **ou** informar o link/id do perfil LinkedIn.
- Apenas arquivos PDF são aceitos para upload.
- Em caso de sucesso na importação, o candidato é vinculado à vaga e o modal é fechado; o callback `onSuccess` é chamado com o `codigoColaborador`.
- Em caso de **erro** (API retorna `sucesso: false` ou exceção): o usuário deve ver a mensagem de erro **em tela** (no modal), sem fechar o fluxo, para poder ajustar e tentar novamente.
- Critérios de aceite: mensagem retornada pelo backend (ex.: "Não foi possivel ler o documento, ajuste e importe novamente") deve ser exibida no modal; toast adicional em erros de rede/exceção é permitido.
- **Falta de conexão:** quando a requisição falhar por indisponibilidade de rede (offline, timeout, etc.), o usuário deve ver mensagem amigável em tela, por exemplo: "Sem conexão com a internet. Verifique sua rede e tente novamente.", e não mensagens técnicas do navegador.

## Restrições
- Token de autenticação obrigatório para importar.
- Backend pode retornar o campo de mensagem com grafia `mensagen`; o front deve exibir o texto corretamente.
