# Do CÃ³digo ao Teste â€” Fluxo Completo com IA
### GherkinFlow Â· DataForge Â· Playwright Â· Qualquer projeto

---

## Como funciona

TrÃªs etapas, cada uma produz um artefato que alimenta a prÃ³xima:

```
ETAPA 1             ETAPA 2             ETAPA 3
GherkinFlow    â†’    DataForge      â†’    Playwright
CenÃ¡rios BDD        Dados SQL           Testes no browser
    â†“                   â†“                   â†“
arquivo .md         arquivo .sql        relatÃ³rio HTML
```

A IA faz o trabalho pesado em cada etapa. VocÃª fornece o contexto, revisa o resultado e guarda os arquivos. Quanto mais contexto vocÃª der Ã  IA, melhor o que ela vai produzir.

---

## Antes de ComeÃ§ar

### O que vocÃª precisa reunir

| Item | Para que serve |
|------|---------------|
| Acesso ao repositÃ³rio do projeto | A IA precisa ler o cÃ³digo das telas |
| Quais telas serÃ£o testadas | Para saber o que pedir ao GherkinFlow |
| Arquivo de schema do banco (DDL) | O DataForge usa para gerar os dados |
| URL do ambiente onde a aplicaÃ§Ã£o roda | Para configurar o Playwright |
| Credenciais de acesso ao sistema | Para os testes conseguirem fazer login |

NÃ£o sabe onde encontrar algum desses itens? Pergunte Ã  IA:

> "Quais arquivos de schema de banco existem neste projeto?"

> "Onde estÃ£o os arquivos de configuraÃ§Ã£o de ambiente deste projeto?"

### Verificando o Node.js

No terminal (PowerShell: tecla Windows + R â†’ `powershell` â†’ Enter):

```powershell
node -v
```

Apareceu `v18.x.x` ou maior? Pode seguir. Se der erro, instale em [nodejs.org](https://nodejs.org) e volte aqui.

---

## ETAPA 1 â€” CenÃ¡rios de Teste com GherkinFlow

### O que acontece aqui

O GherkinFlow Ã© um agente de IA especializado em ler cÃ³digo de telas e transformar isso em cenÃ¡rios de teste escritos em portuguÃªs de negÃ³cio â€” sem jargÃ£o tÃ©cnico, sem nomes de componentes, sem detalhes de implementaÃ§Ã£o. SÃ³ o que importa para quem usa o sistema.

### O que Ã© um cenÃ¡rio BDD

```
Dado que   [contexto â€” quem Ã© o usuÃ¡rio e o que jÃ¡ existe]
Quando     [aÃ§Ã£o â€” o que o usuÃ¡rio faz]
EntÃ£o      [resultado â€” o que o sistema entrega de volta]
```

Cada cenÃ¡rio pode ser **Positivo** (fluxo que funciona), **Negativo** (erro ou bloqueio esperado) ou **Regressivo** (comportamento crÃ­tico que nÃ£o pode quebrar com mudanÃ§as futuras).

### Prompt para usar no chat do Cursor

```
Leia e siga completamente o agente: @BMAD QA/gherkinflow-agent.md

Analise as seguintes telas:
- @[caminho do arquivo da tela 1]
- @[caminho do arquivo da tela 2]

Execute o protocolo GherkinFlow completo:
1. AnÃ¡lise das funcionalidades e regras de negÃ³cio
2. Mapa de Visibilidade â€” perfis, permissÃµes e elementos condicionais
3. GeraÃ§Ã£o dos cenÃ¡rios BDD em portuguÃªs â€” Positivo, Negativo e Regressivo

Salve o resultado em uma pasta de output organizada para este projeto.
```

NÃ£o sabe os caminhos dos arquivos? Pergunte primeiro:

> "Quais sÃ£o os arquivos de tela deste projeto? Liste os caminhos."

### O que vai aparecer

```
| Tipo       | Qtd |
|------------|:---:|
| Positivo   |  X  |
| Negativo   |  X  |
| Regressivo |  X  |
| Total      |  X  |

## [Nome da Tela]
| # | Funcionalidade | Dado | Quando | EntÃ£o | E... | ReferÃªncia |
|---|---|---|---|---|---|---|
| 01 | [Positivo] ... | ... | ... | ... | ... | arquivo.tsx:linha |
...

ðŸ“ Arquivo salvo em: [caminho]
```

### O que verificar antes de continuar

Abra o arquivo `.md` e leia alguns cenÃ¡rios. Eles devem fazer sentido para alguÃ©m que usa o sistema â€” se um cenÃ¡rio parece tÃ©cnico demais ou fora de contexto, use estas perguntas para afinar:

> "O cenÃ¡rio 05 faz sentido para este sistema? Tem algo que parece errado?"

> "Esse formulÃ¡rio tem campos obrigatÃ³rios que nÃ£o foram cobertos nos cenÃ¡rios?"

> "Quais perfis de usuÃ¡rio existem neste sistema e o que cada um pode acessar?"

> "Faltou cobrir algum fluxo importante desta tela?"

---

## ETAPA 2 â€” Massa de Dados com DataForge

### O que acontece aqui

O DataForge lÃª os cenÃ¡rios da Etapa 1 e o schema do banco, e gera um script SQL com dados realistas para cada cenÃ¡rio. Para os cenÃ¡rios negativos, ele cria dados que vÃ£o provocar o erro descrito â€” de propÃ³sito.

### Prompt para usar no chat do Cursor

```
Leia e siga completamente o agente: @BMAD QA/dataforge-agent.md

Inputs:
- Schema do banco:      @[caminho do arquivo DDL/SQL]
- DicionÃ¡rio de dados:  @[caminho, se existir]
- CenÃ¡rios BDD:         @[caminho do arquivo .md gerado na Etapa 1]

Gere o script SQL com:
- CabeÃ§alho comentado com dialeto SQL detectado e total de registros
- BEGIN TRANSACTION / COMMIT
- Cada bloco de INSERT referenciando o cenÃ¡rio correspondente nos comentÃ¡rios
- Dados realistas â€” sem nomes genÃ©ricos, sem CPFs repetidos ou sequenciais

Salve em uma pasta organizada para este projeto.
```

### O que verificar antes de continuar

Abra o arquivo `.sql` e procure por comentÃ¡rios como `-- CenÃ¡rio 03`. Se nÃ£o tiver referÃªncias, peÃ§a:

> "Adicione comentÃ¡rios referenciando os cenÃ¡rios Gherkin em cada bloco de INSERT."

Outras perguntas Ãºteis:

> "Por que os dados do cenÃ¡rio negativo sÃ£o diferentes dos do positivo?"

> "Este script precisa ser executado antes de rodar os testes?"

> "Como executo este SQL no banco de dados do projeto?"

---

## ETAPA 3 â€” Testes Automatizados com Playwright

### O que acontece aqui

O Playwright controla o browser como se fosse um usuÃ¡rio real â€” navega, preenche campos, clica, verifica resultados. VocÃª pede Ã  IA que gere os testes a partir dos cenÃ¡rios BDD, e depois os executa no seu computador.

### Configurando o Playwright

Na pasta onde ficarÃ£o os testes:

```powershell
npm init playwright@latest
```

Quando perguntar:
- Linguagem â†’ **TypeScript**
- Pasta dos testes â†’ aceite o padrÃ£o (`tests`)
- GitHub Actions â†’ **nÃ£o** (por enquanto)

Instale o browser:

```powershell
npx playwright install chromium
```

### Configurando para o seu ambiente

```
Ajuste o playwright.config.ts para este projeto:
- baseURL: [URL onde a aplicaÃ§Ã£o estÃ¡ rodando]
- Reporter: list (terminal) + html (relatÃ³rio visual)
- Screenshot: apenas em falhas
- Video: apenas em falhas
```

NÃ£o sabe a URL? Pergunte Ã  IA:

> "Qual Ã© a URL padrÃ£o de desenvolvimento deste projeto? Onde fica essa configuraÃ§Ã£o?"

### Gerando os testes

```
Com base nos cenÃ¡rios BDD de @[caminho do .md] e no cÃ³digo de @[caminho da tela],
gere os testes Playwright em TypeScript.

- Um test() por cenÃ¡rio BDD, identificado pelo nÃºmero do cenÃ¡rio
- Seletores robustos: getByRole, getByLabel ou getByTestId â€” nunca CSS genÃ©rico
- Logs no terminal em cada passo com emojis padronizados
- Salve em tests/[nome-da-tela]/[nome-da-tela].spec.ts
```

### Rodando os testes

```powershell
# Browser visÃ­vel â€” bom para ver o que estÃ¡ acontecendo
npx playwright test --headed

# SÃ³ um arquivo
npx playwright test tests/[nome]/[nome].spec.ts --headed

# Interface visual para inspecionar passo a passo
npx playwright test --ui
```

### Vendo o relatÃ³rio

```powershell
npx playwright show-report
```

Abre no browser com cada teste, cada passo e as evidÃªncias capturadas.

### Quando um teste falhar

Cole a mensagem de erro inteira e pergunte:

> "O teste falhou com essa mensagem: [cole aqui]. O que causou isso e como corrijo?"

> "O seletor parou de funcionar. Como encontro um seletor mais robusto para este elemento?"

> "O teste passa quando eu olho, mas falha quando roda sozinho. Por quÃª?"

---

## Perguntas que a IA Responde Bem

Guarde estas perguntas â€” elas resolvem a maioria dos travamentos:

### Sobre o projeto

> "Quais telas existem neste projeto e qual Ã© o propÃ³sito de cada uma?"

> "Quais sÃ£o os fluxos principais que um usuÃ¡rio faz neste sistema?"

> "Quais integraÃ§Ãµes externas (APIs, serviÃ§os) esta tela usa?"

### Sobre os cenÃ¡rios

> "Me explica o cenÃ¡rio [X] como se eu fosse um usuÃ¡rio do sistema."

> "Por que esse cenÃ¡rio foi classificado como Regressivo e nÃ£o Positivo?"

> "Quais cenÃ¡rios cobrem o que acontece quando o servidor retorna um erro?"

### Sobre os testes

> "Como faÃ§o o teste aguardar o carregamento de uma tabela antes de verificar os dados?"

> "Como rodo apenas os testes que falharam, sem rodar tudo de novo?"

> "Como capturo uma screenshot dentro do teste para usar como evidÃªncia?"

> "Como faÃ§o o teste simular uma resposta de API sem depender do servidor real?"

---

## Guardando as EvidÃªncias

Antes da apresentaÃ§Ã£o, confirme que vocÃª tem:

| O que guardar | Como garantir |
|---------------|--------------|
| Arquivo `.md` com os cenÃ¡rios BDD | Salvo pela IA â€” confirmar que existe na pasta |
| Arquivo `.sql` com a massa de dados | Salvo pela IA â€” confirmar que existe na pasta |
| RelatÃ³rio HTML dos testes | Gerado apÃ³s rodar â€” pasta `playwright-report/` |
| Screenshot do terminal com testes aprovados | Tirar apÃ³s a execuÃ§Ã£o passar |
| Screenshot do relatÃ³rio HTML aberto | Tirar apÃ³s `npx playwright show-report` |

### Checklist final

```
[ ] Arquivo .md com cenÃ¡rios BDD â€” salvo e revisado
[ ] Arquivo .sql com massa de dados â€” salvo
[ ] Testes Playwright â€” pelo menos o fluxo principal passou
[ ] RelatÃ³rio HTML â€” gerado e acessÃ­vel
[ ] Screenshot do terminal com os resultados
[ ] Screenshot do relatÃ³rio HTML
[ ] (BÃ´nus) GravaÃ§Ã£o do browser sendo controlado automaticamente
```

---

## GlossÃ¡rio

| Termo | O que Ã© |
|-------|---------|
| **BDD** | Escrever testes em linguagem de negÃ³cio, nÃ£o tÃ©cnica |
| **Gherkin** | Estrutura: Dado que / Quando / EntÃ£o |
| **GherkinFlow** | Agente de IA que gera cenÃ¡rios BDD lendo o cÃ³digo das telas |
| **DataForge** | Agente de IA que gera dados SQL a partir dos cenÃ¡rios |
| **Playwright** | Ferramenta que controla o browser automaticamente |
| **Happy Path** | O fluxo que funciona sem erros â€” o caminho principal |
| **CenÃ¡rio Positivo** | Testa o comportamento correto e esperado |
| **CenÃ¡rio Negativo** | Testa erros, bloqueios e validaÃ§Ãµes |
| **CenÃ¡rio Regressivo** | Testa comportamentos que nÃ£o podem quebrar com mudanÃ§as futuras |
| **Massa de dados** | Registros criados no banco para os testes usarem |
| **DDL / Schema** | Arquivo SQL com os `CREATE TABLE` â€” estrutura do banco |
| **Page Object** | Arquivo que organiza os seletores de uma tela |
| **RelatÃ³rio HTML** | PÃ¡gina visual com o resultado de cada teste |
| **Screenshot** | Foto automÃ¡tica da tela do browser durante o teste |

---

> Travou? Cole a situaÃ§Ã£o no chat do Cursor e pergunte Ã  IA.
> Ela conhece o projeto, os agentes e o contexto â€” Ã© sÃ³ perguntar.

