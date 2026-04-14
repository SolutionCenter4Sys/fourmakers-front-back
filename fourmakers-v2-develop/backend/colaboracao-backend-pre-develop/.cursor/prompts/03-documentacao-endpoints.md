---
description: Formato obrigatório para documentação de endpoints em .txt (estilo Fourmakers)
alwaysApply: false
---

# Documentação de endpoints (arquivo .txt)

Quando o usuário pedir **documentação de endpoint**, **doc de API**, **especificação de rotas** ou equivalente, produzir o conteúdo **neste modelo** (mesmo estilo de `Feedback360_Endpoints.txt` e `Candidatura_LogPretensaoModelo_Endpoints.txt`).

## Estrutura do arquivo

1. **Cabeçalho** com linhas `====`, título do domínio/recurso, subtítulo opcional, linha **Gerado em: DD/MM/AAAA** (usar data atual do contexto quando existir).
2. **Base URL** e **Auth** (ex.: Bearer, rotas protegidas).
3. **Contexto** curto (tabelas, domínio) quando fizer sentido.
4. **Envelope padrão de resposta** com exemplo JSON (`sucesso`, `mensagem`, `erros`, `retorno`) alinhado ao tipo real do projeto (`ApiGenericResult`, etc.).
5. **Uma seção numerada por endpoint**, cada uma com:
   - Linhas `================================================================================`
   - Título descritivo + **MÉTODO** + path completo
   - **QUERY PARAMS** / **REQUEST BODY** com tipos, obrigatoriedade, padrões
   - **Exemplo** de URL ou body quando útil
   - **RESPONSE** com JSON de exemplo realista
   - **ERROS POSSÍVEIS** com código HTTP e motivo
6. Opcional: **REFERÊNCIA RÁPIDA** (tabela coluna a coluna dos campos do DTO).
7. Opcional: **COMPARATIVO** quando houver mais de um endpoint relacionado.
8. **REFERÊNCIA DE STATUS HTTP** no final.

## Convenções

- Texto em português, claro e direto.
- Nomes de propriedades JSON em **camelCase** como na API serializada.
- Indicar regras de negócio (permissões, escopo por org, etc.) na seção correta.
- Se o usuário indicar pasta/nome de arquivo, salvar lá; caso contrário, sugerir nome `*Endpoints.txt` coerente com o recurso.

## Referência de estilo no repositório do usuário

Exemplos de referência (fora do backend): documentos `*Endpoints.txt` na pasta de SQL/documentação do usuário, no mesmo formato acima.
