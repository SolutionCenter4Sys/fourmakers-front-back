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

## Roteiro da demonstracao (passo a passo)

1. **Preparacao tecnica**
   - Garanta `npm install` na raiz e em `frontend/playwright-automation-template`.
   - Garanta navegadores Playwright instalados (`npx playwright install chromium` no template).
2. **Validacao de autenticacao OTP (API-only)**
   - Execute em `frontend/playwright-automation-template`:
   - `npm run test:solution-center:smoke`
   - Siga apenas se o resultado for `4 passed`.
3. **Disparo da demo completa no chat**
   - No Cursor Chat, execute `demo reembolso`.
   - A etapa Playwright passa a executar automaticamente a spec versionada mais recente (`reembolso-demo-vN.spec.ts`) com o mesmo `vN` do BDD e da massa.
4. **Execucao visual da jornada E2E**
   - O Playwright abre Chromium em modo visivel e executa a jornada de Reembolso.
5. **Fechamento com evidencias**
   - Mostrar os arquivos versionados (`vN`) e as evidencias em `frontend/playwright-automation-template/evidencias/`.
   - Mostrar o HTML final consolidado em `frontend/playwright-automation-template/evidencias/relatorios/demo-reembolso-vN.html`.

## O que acontece durante a execucao

1. O front local sobe em `http://localhost:8080` (Vite com proxy para backend dev).
2. O smoke OTP Solution Center valida autenticacao API-only (`EnviaTokenAcessoEmail` -> `ObtemCodigoAcessoEmailQA` -> `ValidaTokenAcessoEmail`) com `4 passed`.
3. O GherkinFlow gera um novo arquivo de BDD da jornada de Reembolso.
4. O DataForge gera `reembolso.data.vN.js` com dados realistas para os cenarios.
5. O Playwright:
   - faz login OTP real (`EnviaTokenAcessoEmail` -> `ObtemCodigoAcessoEmailQA` -> `ValidaTokenAcessoEmail`);
   - injeta JWT no `localStorage`;
   - abre o Chromium em modo visivel (`headed`) com `slowMo`;
   - executa a jornada de Reembolso.
6. Ao final, sao exibidos banners de etapa concluida no chat e logs no terminal.
7. A etapa Playwright gera um HTML consolidado com resumo da execucao, cenarios testados, massa atendida, uso do JSON da tela e logs completos (incluindo falhas, quando houver).

## O que voce ve no Cursor

- Arquivos versionados gerados aparecem no historico da conversa e podem ser clicados.
- A etapa do Playwright abre o navegador para visualizacao da jornada.
- Evidencias (screenshots/relatorios) ficam em `frontend/playwright-automation-template/evidencias/`.
- O HTML final consolidado fica em `frontend/playwright-automation-template/evidencias/relatorios/demo-reembolso-vN.html`.

## Comandos uteis

- Rodar smoke OTP Solution Center (obrigatorio no roteiro): `npm --prefix frontend/playwright-automation-template run test:solution-center:smoke`
- Rodar apenas a automacao Playwright (com Vite ja ligado, usando spec/massa versionadas mais recentes): `npm run demo:run`
- Rodar fluxo completo de uma vez: `npm run demo`
- Rodar fluxo completo com navegador visivel: `npm run demo:headed`

## Troubleshooting rapido

- Se o navegador abrir e fechar rapido, prefira `npm run demo:run` com `npm run dev` ja ativo.
- Se houver erro de autenticacao, valide se o ambiente dev e as credenciais de demo estao corretos.
- Se o smoke OTP nao passar com `4 passed`, corrigir autenticacao antes de iniciar a demonstracao E2E.
- Se houver erro de API/CORS, confira `.env.local` e o valor de `VITE_API_PROXY_TARGET`.
