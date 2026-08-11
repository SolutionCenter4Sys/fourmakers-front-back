# Integração — Fluxo de Acesso por E-mail com Bypass para QA

## Visão geral

Este documento descreve o fluxo completo de autenticação por código de e-mail, incluindo o endpoint exclusivo para times de QA que permite recuperar o código de acesso gerado sem depender do recebimento do e-mail.

```
┌──────────────────────────────────────────────────────────────────┐
│  1. EnviaTokenAcessoEmail   →  gera código e envia por e-mail    │
│  2. ObtemCodigoAcessoEmailQA →  recupera o código via API (QA)   │
│  3. ValidaTokenAcessoEmail  →  valida o código e retorna JWT     │
└──────────────────────────────────────────────────────────────────┘
```

---

## Ambientes

| Ambiente | Base URL |
|---|---|
| **Produção (PRD)** | `https://api.fourmakers.io` |
| **Homologação (HML) — Hub genérico** | `https://fourmakershub-api.dev.fourmakers.io` |
| **Homologação (HML) — Demo Reembolso (FIXO)** | `https://spw.app.foursys.com/backoffice-rf-hom` |

> **Demo pré-vendas:** use sempre `backoffice-rf-hom` (`DEMO/scripts/demo-ambiente.cjs`). O host `fourmakershub-api.dev` é outra entrada HML do produto — **não** é o backend desta demo. O trecho `dev` no hostname do Hub **não** significa “ambiente de desenvolvimento da demo”.

Todos os endpoints estão sob o prefixo `/api/Acesso`.

---

## Passo 1 — Solicitar código de acesso

Dispara o envio do código de 6 dígitos para o e-mail do usuário e grava o token na base de dados com validade de **15 minutos**.

### Requisição

```
POST /api/Acesso/EnviaTokenAcessoEmail
Content-Type: application/json
```

**Body:**

```json
{
  "email": "usuario@empresa.com",
  "orgId": 2
}
```

| Campo | Tipo | Obrigatório | Descrição |
|---|---|---|---|
| `email` | string | sim | E-mail cadastrado do colaborador |
| `orgId` | integer | sim | Identificador da organização |

### Respostas

**200 OK — código enviado por e-mail**

```json
{
  "sucesso": true,
  "tipoAcesso": "Email"
}
```

> Quando a organização possui SSO configurado, `tipoAcesso` retorna `"SSO"` e o e-mail enviado contém a URL de acesso ao provedor de identidade, sem código numérico.

**400 Bad Request — dados inválidos**

```json
{
  "sucesso": false,
  "mensagem": "Nenhum usuário foi encontrado para este e-mail"
}
```

Possíveis mensagens de erro:

| Mensagem | Causa |
|---|---|
| `"Endereço email inválido"` | Formato de e-mail incorreto |
| `"Organização não informada"` | `orgId` igual a `0` |
| `"Nenhum usuário foi encontrado para este e-mail"` | E-mail não cadastrado na organização |
| `"Colaborador inativo na organização. Acesso não permitido."` | Colaborador sem vínculo ativo |

**401 Unauthorized**

```json
{
  "sucesso": false,
  "mensagem": "Não autorizado"
}
```

---

## Passo 2 — Recuperar o código gerado (exclusivo para QA)

Retorna o código de acesso ativo para um usuário, sem necessidade de acesso ao e-mail. **Uso restrito a automações e times de QA.** Requer um token de sistema válido.

### Requisição

```
GET /api/Acesso/ObtemCodigoAcessoEmailQA?email={email}&orgId={orgId}
Authorization: Bearer {tokenSistema}
```

**Query parameters:**

| Parâmetro | Tipo | Obrigatório | Descrição |
|---|---|---|---|
| `email` | string | sim | E-mail do colaborador |
| `orgId` | integer | sim | Identificador da organização |

**Header:**

| Header | Formato | Descrição |
|---|---|---|
| `Authorization` | `Bearer {tokenSistema}` | Token de sistema codificado em Base64Url no formato `{orgId}\|{token}` |

> O token de sistema é gerado e gerenciado internamente na tabela `tb_token_sistema`. Para obter um token válido para o ambiente desejado, consulte o time de backend.

### Respostas

**200 OK — código encontrado**

```json
{
  "sucesso": true,
  "codigo": "847291"
}
```

| Campo | Tipo | Descrição |
|---|---|---|
| `sucesso` | boolean | Sempre `true` em caso de sucesso |
| `codigo` | string | Código numérico de 6 dígitos gerado no Passo 1 |

**400 Bad Request — dados inválidos ou código expirado**

```json
{
  "sucesso": false,
  "mensagem": "Nenhum código de acesso ativo encontrado para este e-mail"
}
```

Possíveis mensagens de erro:

| Mensagem | Causa |
|---|---|
| `"Endereço email inválido"` | Formato de e-mail incorreto |
| `"Organização não informada"` | `orgId` igual a `0` |
| `"Nenhum usuário foi encontrado para este e-mail"` | E-mail não cadastrado na organização |
| `"Nenhum código de acesso ativo encontrado para este e-mail"` | Código não gerado, já utilizado ou expirado (> 15 min) |

**401 Unauthorized — token de sistema ausente ou inválido**

```json
{
  "sucesso": false,
  "mensagem": "Não autorizado"
}
```

---

## Passo 3 — Validar o código e obter o JWT

Valida o código de acesso e, em caso de sucesso, retorna o JWT de sessão do usuário.

### Requisição

```
POST /api/Acesso/ValidaTokenAcessoEmail
Content-Type: application/json
```

**Body:**

```json
{
  "email": "usuario@empresa.com",
  "token": "847291",
  "orgId": 2
}
```

| Campo | Tipo | Obrigatório | Descrição |
|---|---|---|---|
| `email` | string | sim | E-mail do colaborador |
| `token` | string | sim | Código de 6 dígitos recebido por e-mail ou obtido via Passo 2 |
| `orgId` | integer | sim | Identificador da organização |

### Respostas

**200 OK — autenticação bem-sucedida**

```json
{
  "sucesso": true,
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "primeiroAcessoRealizado": false,
  "dataAceitePrimeiroAcesso": null,
  "usuario": {
    "nomeColaborador": "João Silva",
    "usuarioId": 123,
    "cpf": "000.000.000-00",
    "email": "usuario@empresa.com",
    "orgId": 2,
    "colaborador": { ... },
    "colaboradorOrg": { ... },
    "funcionalidadeSistema": [ ... ]
  }
}
```

| Campo | Tipo | Descrição |
|---|---|---|
| `token` | string | JWT de sessão a ser utilizado nas demais requisições autenticadas |
| `primeiroAcessoRealizado` | boolean | Indica se o usuário já aceitou os termos de primeiro acesso |
| `dataAceitePrimeiroAcesso` | string (ISO 8601) \| null | Data de aceite dos termos, ou `null` se ainda não realizado |
| `usuario` | object | Dados completos do colaborador autenticado |

**401 Unauthorized — código inválido ou expirado**

```json
{
  "sucesso": false,
  "mensagem": "Código de acesso inválido."
}
```

---

## Fluxo completo — exemplo para automação de QA

```
# 1. Solicitar o envio do código
POST /api/Acesso/EnviaTokenAcessoEmail
{
  "email": "qa.user@empresa.com",
  "orgId": 2
}
→ 200 OK { "sucesso": true, "tipoAcesso": "Email" }

# 2. Recuperar o código sem precisar do e-mail
GET /api/Acesso/ObtemCodigoAcessoEmailQA?email=qa.user@empresa.com&orgId=2
Authorization: Bearer eyJvcmdJZCI6Mn0...
→ 200 OK { "sucesso": true, "codigo": "847291" }

# 3. Autenticar com o código obtido
POST /api/Acesso/ValidaTokenAcessoEmail
{
  "email": "qa.user@empresa.com",
  "token": "847291",
  "orgId": 2
}
→ 200 OK { "sucesso": true, "token": "eyJhbGci..." }
```

---

## Observações importantes

- O código gerado no Passo 1 tem **validade de 15 minutos**. Após esse prazo, é necessário reiniciar o fluxo a partir do Passo 1.
- Ao executar o Passo 1 novamente para o mesmo usuário, todos os códigos anteriores são **invalidados imediatamente**, mesmo que ainda estejam dentro do prazo de validade.
- O Passo 2 (`ObtemCodigoAcessoEmailQA`) **não consome nem invalida** o código — ele apenas o lê. O código permanece ativo para uso no Passo 3.
- Após a validação bem-sucedida no Passo 3, o código é **marcado como utilizado** e não pode ser reutilizado.
- O endpoint do Passo 2 é protegido por **token de sistema** (`tb_token_sistema`) e **não deve ser exposto em fluxos de produção voltados ao usuário final**.
