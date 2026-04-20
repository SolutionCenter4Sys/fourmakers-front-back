# DEMO Reembolso - Guia rapido

Este documento explica como funciona a demo automatizada de Reembolso, como rodar e o que acontece durante a execucao.

## Objetivo da demo

A demo mostra uma jornada E2E real do modulo de Reembolso usando 3 agentes em sequencia:

1. **GherkinFlow**: gera uma nova versao dos cenarios BDD.
2. **DataForge**: gera uma nova versao da massa de dados a partir do BDD.
3. **Playwright**: gera uma nova versao da spec e executa a automacao no navegador visivel.

## Estrutura principal

- `frontend/DEMO/cenarios-bdd/` -> cenarios BDD versionados (`REEMBOLSO-BDD-vN.md`)
- `frontend/DEMO/automacao/reembolso/versionadas/` -> massa de dados versionada (`reembolso.data.vN.js`)
- `frontend/DEMO/ui-elements/reembolso-ui.json` -> mapeamento de elementos da UI
- `frontend/playwright-automation-template/tests/reembolso/versionadas/` -> specs versionadas (`reembolso-demo-vN.spec.ts`)

## Politica de versionamento e retencao

- Cada execucao cria um trio com o mesmo `vN`:
  - `REEMBOLSO-BDD-vN.md`
  - `reembolso.data.vN.js`
  - `reembolso-demo-vN.spec.ts`
- A demo **nao sobrescreve** arquivos de versoes anteriores.
- Sao mantidas no maximo 10 versoes por pasta versionada.
- Ao atingir `v10`, o processo fica autorizado a limpar as versoes antigas e reiniciar em `v1`.

## Pre-requisitos

1. Dependencias instaladas:
   - Na raiz `fourmakers-v2-develop`: `npm install`
   - Em `frontend/playwright-automation-template`: `npm install` e `npx playwright install chromium`
2. Arquivo `frontend/.env.local` com:

```env
VITE_API_PROXY_TARGET=https://spw.app.foursys.com/backoffice-rf-hom
VITE_API_FOURMAKERS_URL=
```

3. Acesso de rede ao ambiente dev da Foursys.

## Como rodar a demo

### Forma recomendada (chat)

No Cursor Chat, digite exatamente:

```text
demo reembolso
```

Esse comando dispara o orquestrador completo (Etapa 1 -> Etapa 2 -> Etapa 3) sem pausas e com geracao de nova versao.

### Forma manual (terminal)

Na raiz do projeto `fourmakers-v2-develop`:

```bash
npm run dev
```

Em outro terminal, ainda na raiz:

```bash
npm run demo:run
```

Opcional (one-shot):

```bash
npm run demo
npm run demo:headed
```

## O que acontece durante a execucao

1. O front local sobe em `http://localhost:8080` (Vite com proxy para backend dev).
2. O GherkinFlow gera um novo arquivo de BDD da jornada de Reembolso.
3. O DataForge gera `reembolso.data.vN.js` com dados realistas para os cenarios.
4. O Playwright:
   - faz login OTP real (`EnviaTokenAcessoEmail` -> `ObtemCodigoAcessoEmailQA` -> `ValidaTokenAcessoEmail`);
   - injeta JWT no `localStorage`;
   - abre o Chromium em modo visivel (`headed`) com `slowMo`;
   - executa a jornada de Reembolso.
5. Ao final, sao exibidos banners de etapa concluida no chat e logs no terminal.

## O que voce ve no Cursor

- Arquivos versionados gerados aparecem no historico da conversa e podem ser clicados.
- A etapa do Playwright abre o navegador para visualizacao da jornada.
- Evidencias (screenshots/relatorios) ficam em `frontend/playwright-automation-template/evidencias/`.

## Comandos uteis

- Rodar apenas a automacao Playwright (com Vite ja ligado): `npm run demo:run`
- Rodar fluxo completo de uma vez: `npm run demo`
- Rodar fluxo completo com navegador visivel: `npm run demo:headed`

## Troubleshooting rapido

- Se o navegador abrir e fechar rapido, prefira `npm run demo:run` com `npm run dev` ja ativo.
- Se houver erro de autenticacao, valide se o ambiente dev e as credenciais de demo estao corretos.
- Se houver erro de API/CORS, confira `.env.local` e o valor de `VITE_API_PROXY_TARGET`.
