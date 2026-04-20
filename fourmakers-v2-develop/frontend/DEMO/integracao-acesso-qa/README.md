# Integração — Acesso QA (OTP Login)

Artefatos de referência para a automação autenticar via OTP de 3 passos (com bypass de e-mail via endpoint exclusivo de QA).

## Conteúdo da pasta

| Arquivo | Descrição |
|---|---|
| `integracao-acesso-qa.md` | Documentação funcional completa dos 3 endpoints. |
| `OTP-login.postman_collection.json` | Collection do Postman com as 3 requisições prontas. |

## Resumo do fluxo (3 passos)

```
┌──────────────────────────────────────────────────────────────────┐
│  1. POST /api/Acesso/EnviaTokenAcessoEmail                       │
│     → gera código de 6 dígitos e envia por e-mail                │
│     → Bearer com o segredo RAW do ambiente                       │
│                                                                  │
│  2. GET  /api/Acesso/ObtemCodigoAcessoEmailQA?email=&orgId=      │
│     → recupera o código sem depender do e-mail                   │
│     → Bearer Base64Url("{orgId}|{segredo}")                      │
│                                                                  │
│  3. POST /api/Acesso/ValidaTokenAcessoEmail                      │
│     → valida o código por parâmetro e devolve o JWT              │
└──────────────────────────────────────────────────────────────────┘
```

## Credenciais usadas pela demo (dev)

| Campo | Valor |
|---|---|
| **Usuário** | `gustavo.queiroz@foursys.com.br` |
| **orgId** | `8` |
| **Backend (dev)** | `https://spw.app.foursys.com/backoffice-rf-hom` |
| **Front (local via Vite)** | `http://localhost:8080` |
| **Segredo RAW** (Step 1) | `5YB40IzcB7x83WsFYK0qCioG6i3lFhP3qlYbCirzmc995KtB8B` |
| **Token Base64Url** (Steps 2 & 3) | `OHw1WUI0MEl6Y0I3eDgzV3NGWUswcUNpb0c2aTNsRmhQM3FsWWJDaXJ6bWM5OTVLdEI4Qg==` |

> **Atenção:** credenciais restritas a automação/QA no ambiente dev. Não expor em fluxos de produção voltados ao usuário final. Consulte o time de backend para obter credenciais de outros ambientes.

## Por que o front roda local via Vite

O deploy `spw.app.foursys.com/backoffice-rf-hom` serve apenas **API** — não hospeda o front-end. Por isso a demo sobe o front do próprio repo via `npm run dev` (Vite), configurado para **proxiar** as chamadas `/api/*` direto pro backend dev. Isso:

- Resolve CORS (tudo vira same-origin do ponto de vista do browser).
- Permite que qualquer mudança no front-end seja testada antes de chegar em produção.
- Mantém o JWT e o localStorage idênticos ao fluxo real do usuário.

## Configuração do proxy (.env.local na raiz do repo)

```
VITE_API_PROXY_TARGET=https://spw.app.foursys.com/backoffice-rf-hom
VITE_API_FOURMAKERS_URL=
```

## Implementação atual na automação

O fluxo está implementado em:

- `playwright-automation-template/support/auth/fourmakers-auth.ts`
  → função `loginFourMakers(page, request, appUrl?)` executa os 3 passos OTP (com polling no Passo 2), injeta o JWT + `lastOrgId` no `localStorage` e recarrega a página.

## Como importar a collection no Postman

1. Postman → **File → Import**
2. Selecionar `OTP-login.postman_collection.json`
3. As 3 requisições aparecerão na aba **Collections** sob o nome **OTP login**

## Observações rápidas

- Código OTP tem validade de **15 minutos**.
- Chamar o Passo 1 novamente **invalida** códigos anteriores do mesmo usuário.
- Passo 2 apenas **lê** o código — não consome nem invalida.
- Passo 3 **consome** o código: após uso, não pode ser reutilizado.
- Cada execução da automação gera um OTP novo → validade de 15 min não é um problema na prática.
