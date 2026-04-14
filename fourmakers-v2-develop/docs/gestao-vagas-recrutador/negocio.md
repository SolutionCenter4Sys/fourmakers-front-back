# Gestão de Vagas — Atribuição de Recrutador (Negócio)

## Objetivo
Permitir que o usuário atribua ou altere o recrutador responsável por uma vaga ou por uma candidatura.

## Regras e fluxos
- **Modal "Recrutador/a" (vaga):** atribuir recrutador responsável por uma vaga. Exibe título da vaga no texto de apoio.
- **Modal "Recrutador(a)" (candidato):** atribuir recrutador responsável por uma candidatura.
- Lista de opções carregada a partir dos colaboradores da organização (Use Case de listagem).
- Seleção por nome; persistência via Use Cases (AdicionarRecrutadorVaga / AtualizarRecrutadorResponsavel).
- Pré-seleção quando já existe recrutador atual (por nome).

## Restrições
- Requer token de autenticação.
- Salvamento habilitado apenas quando há recrutador selecionado.

## Critérios de aceite (relevantes à melhoria)
- Usuário consegue rolar a lista de recrutadores com o scroll do mouse.
- Usuário consegue pesquisar por nome com autocomplete (filtro em tempo real, tolerante a acentos e maiúsculas/minúsculas).
