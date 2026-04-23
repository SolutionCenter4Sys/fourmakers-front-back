# Instructions

- Following Playwright test failed.
- Explain why, be concise, respect Playwright best practices.
- Provide a snippet of code with the fix, if possible.

# Test info

- Name: reembolso\versionadas\reembolso-demo-v3.spec.ts >> I-08 [Negativo] Submissão com campos obrigatórios vazios
- Location: tests\reembolso\versionadas\reembolso-demo-v3.spec.ts:362:5

# Error details

```
TimeoutError: locator.scrollIntoViewIfNeeded: Timeout 15000ms exceeded.
Call log:
  - waiting for getByRole('button', { name: 'Adicionar ao Carrinho' })

```

# Test source

```ts
  269 |     for (const r of CATALOGO_SKIP) {
  270 |       const id = r.id.padEnd(5)
  271 |       const tipo = `[${r.tipo}]`.padEnd(13)
  272 |       console.log(`  ⏭️  ${id} ${tipo} ${r.titulo.padEnd(42)} — ${r.razao}`)
  273 |     }
  274 |   }
  275 | 
  276 |   console.log(`\n╚══ ✅ DEMO CONCLUÍDA — ${passados}/${executados.length} cenários executados com sucesso ══╝\n`)
  277 | })
  278 | 
  279 | // ═════════════════════════════════════════════════════════════════════
  280 | //  DASHBOARD — Reembolso.tsx (/reembolso)
  281 | // ═════════════════════════════════════════════════════════════════════
  282 | 
  283 | test('R-01 [Positivo] Visão padrão do módulo', async ({ page }) => {
  284 |   const p = new ReembolsoPage(page)
  285 |   LOG.secao('R-01', 'Positivo', 'Visão padrão do módulo de Reembolso')
  286 | 
  287 |   LOG.passo(`Validando que apenas a aba "${dadosR01.abaEsperada}" está visível...`)
  288 |   await p.visitarLista()
  289 |   await expect(p.tabMeusReembolsos).toBeVisible()
  290 |   LOG.ok(`Aba "${dadosR01.abaEsperada}" visível`)
  291 | 
  292 |   for (const abaRestrita of dadosR01.abasRestritas) {
  293 |     const aba = page.getByRole('tab', { name: abaRestrita })
  294 |     if (await aba.count() > 0) {
  295 |       LOG.alerta(`Aba "${abaRestrita}" presente no DOM — usuário tem perfil elevado`)
  296 |     } else {
  297 |       LOG.ok(`Aba "${abaRestrita}" ausente (como esperado)`)
  298 |     }
  299 |   }
  300 |   await p.evidencia('r01-visao-padrao')
  301 | })
  302 | 
  303 | test('R-09 [Positivo] Início de nova solicitação', async ({ page }) => {
  304 |   const p = new ReembolsoPage(page)
  305 |   LOG.secao('R-09', 'Positivo', 'Início de nova solicitação')
  306 | 
  307 |   LOG.passo('Navegando para /reembolso...')
  308 |   await p.visitarLista()
  309 |   await expect(p.btnSolicitarReembolso).toBeVisible()
  310 | 
  311 |   LOG.passo(`Clicando no botão "${dadosR09.botao}"...`)
  312 |   await p.clicarSolicitarReembolso()
  313 | 
  314 |   LOG.passo(`Validando redirecionamento para ${dadosR09.urlDestino}...`)
  315 |   await expect(page).toHaveURL(new RegExp(dadosR09.urlDestino))
  316 |   await expect(page.getByText(/Nova Solicita/i)).toBeVisible()
  317 |   LOG.ok('Redirecionamento para formulário de nova solicitação')
  318 |   await p.evidencia('r09-formulario-aberto')
  319 | })
  320 | 
  321 | test('R-12 [Negativo] Busca sem resultados', async ({ page }) => {
  322 |   const p = new ReembolsoPage(page)
  323 |   LOG.secao('R-12', 'Negativo', 'Busca sem resultados')
  324 | 
  325 |   await p.visitarLista()
  326 | 
  327 |   LOG.passo(`Informando termo de busca: "${dadosR12.termoBusca}"...`)
  328 |   const inputBusca = page.getByPlaceholder(/busca/i).first()
  329 |   await expect(inputBusca).toBeVisible()
  330 |   await inputBusca.fill(dadosR12.termoBusca)
  331 | 
  332 |   LOG.passo(`Validando mensagem: "${dadosR12.mensagemEsperada}"...`)
  333 |   await expect(page.getByText(new RegExp(dadosR12.mensagemEsperada, 'i'))).toBeVisible()
  334 |   LOG.ok('Mensagem de estado vazio apresentada')
  335 |   await p.evidencia('r12-busca-vazia')
  336 | })
  337 | 
  338 | test('R-13 [Regressivo] Limpeza dos filtros de data', async ({ page }) => {
  339 |   const p = new ReembolsoPage(page)
  340 |   LOG.secao('R-13', 'Regressivo', 'Limpeza dos filtros de data')
  341 | 
  342 |   await p.visitarLista()
  343 | 
  344 |   LOG.passo(`Aplicando filtro de período: ${dadosR13.dataInicio} → ${dadosR13.dataFim}...`)
  345 |   const btnLimpar = page.getByRole('button', { name: new RegExp(dadosR13.botaoLimpar, 'i') })
  346 | 
  347 |   if (await btnLimpar.isVisible().catch(() => false)) {
  348 |     LOG.passo(`Acionando botão "${dadosR13.botaoLimpar}"...`)
  349 |     await btnLimpar.click()
  350 |     await expect(btnLimpar).not.toBeVisible({ timeout: 5_000 }).catch(() => {})
  351 |     LOG.ok('Filtros de data limpos com sucesso')
  352 |   } else {
  353 |     LOG.alerta('Filtros não estão aplicados no momento — botão "Limpar" ausente (comportamento esperado quando sem filtros)')
  354 |   }
  355 |   await p.evidencia('r13-filtros-limpos')
  356 | })
  357 | 
  358 | // ═════════════════════════════════════════════════════════════════════
  359 | //  INSERIR REEMBOLSO — InserirReembolso.tsx (/inserir-reembolso)
  360 | // ═════════════════════════════════════════════════════════════════════
  361 | 
  362 | test('I-08 [Negativo] Submissão com campos obrigatórios vazios', async ({ page }) => {
  363 |   const p = new ReembolsoPage(page)
  364 |   LOG.secao('I-08', 'Negativo', 'Submissão com campos obrigatórios vazios')
  365 | 
  366 |   await p.visitarFormulario()
  367 | 
  368 |   LOG.passo('Acionando "Adicionar ao Carrinho" sem preencher nada...')
> 369 |   await p.btnAdicionarCarrinho.scrollIntoViewIfNeeded()
      |                                ^ TimeoutError: locator.scrollIntoViewIfNeeded: Timeout 15000ms exceeded.
  370 |   await p.clicarAdicionarCarrinho()
  371 | 
  372 |   LOG.passo(`Validando notificação: "${dadosI08.mensagemEsperada}"...`)
  373 |   await expect(
  374 |     page.getByText(new RegExp(dadosI08.mensagemEsperada, 'i')).first(),
  375 |   ).toBeVisible({ timeout: 10_000 })
  376 |   LOG.ok(`Toast "${dadosI08.mensagemEsperada}" exibido — validação ativada`)
  377 |   await p.evidencia('i08-campos-vazios')
  378 | })
  379 | 
  380 | test('I-11 [Negativo] Anexo em formato não suportado', async ({ page }) => {
  381 |   const p = new ReembolsoPage(page)
  382 |   LOG.secao('I-11', 'Negativo', 'Anexo em formato não suportado')
  383 | 
  384 |   await p.visitarFormulario()
  385 |   await p.preencherObjetivo(dadosI11.objetivo)
  386 |   await p.preencherDestino(dadosI11.destino)
  387 | 
  388 |   LOG.passo(`Tentando anexar "${dadosI11.nomeArquivoInvalido}" (formato não suportado)...`)
  389 |   const conteudoFake = Buffer.from('arquivo fake para teste de formato invalido')
  390 |   const inputFile = p.uploadInput
  391 |   if (await inputFile.count() > 0) {
  392 |     await inputFile.setInputFiles({
  393 |       name: dadosI11.nomeArquivoInvalido,
  394 |       mimeType: 'application/octet-stream',
  395 |       buffer: conteudoFake,
  396 |     }).catch(() => {})
  397 | 
  398 |     LOG.passo(`Validando mensagem: "${dadosI11.mensagemErroEsperada}"...`)
  399 |     const toast = page.getByText(new RegExp(dadosI11.mensagemErroEsperada, 'i'))
  400 |     const toastVisivel = await toast.isVisible({ timeout: 5_000 }).catch(() => false)
  401 |     if (toastVisivel) {
  402 |       LOG.ok(`Formato rejeitado como esperado`)
  403 |     } else {
  404 |       LOG.alerta('Toast de formato inválido não apareceu — app pode aceitar silenciosamente')
  405 |     }
  406 |   } else {
  407 |     LOG.alerta('Input de upload não visível neste estado — cenário pulado')
  408 |   }
  409 |   await p.evidencia('i11-formato-invalido')
  410 | })
  411 | 
  412 | test('I-12 [Negativo] Envio com carrinho vazio', async ({ page }) => {
  413 |   const p = new ReembolsoPage(page)
  414 |   LOG.secao('I-12', 'Negativo', 'Envio com carrinho vazio')
  415 | 
  416 |   await p.visitarFormulario()
  417 | 
  418 |   LOG.passo('Verificando estado inicial do carrinho (deve estar vazio)...')
  419 |   await expect(page.getByText(/Nenhuma solicita/i)).toBeVisible()
  420 | 
  421 |   LOG.passo('Tentando acionar "Enviar Solicitações" com carrinho vazio...')
  422 |   const btnEnviar = p.btnEnviarSolicitacoes
  423 |   if (await btnEnviar.count() > 0) {
  424 |     await btnEnviar.scrollIntoViewIfNeeded()
  425 |     await btnEnviar.click({ trial: false }).catch(() => {
  426 |       LOG.alerta('Botão "Enviar" desabilitado — guarda ativa por UI (aceitável)')
  427 |     })
  428 | 
  429 |     const toast = page.getByText(new RegExp(dadosI12.mensagemEsperada, 'i'))
  430 |     if (await toast.isVisible().catch(() => false)) {
  431 |       LOG.ok(`Toast "${dadosI12.mensagemEsperada}" exibido`)
  432 |     } else {
  433 |       LOG.alerta('Toast não visível — provavelmente o botão ficou desabilitado')
  434 |     }
  435 |   } else {
  436 |     LOG.alerta('Botão "Enviar Solicitações" ausente — guarda por renderização condicional')
  437 |   }
  438 |   await p.evidencia('i12-carrinho-vazio')
  439 | })
  440 | 
  441 | test('I-13 [Regressivo] Limpeza do formulário', async ({ page }) => {
  442 |   const p = new ReembolsoPage(page)
  443 |   LOG.secao('I-13', 'Regressivo', 'Limpeza do formulário')
  444 | 
  445 |   await p.visitarFormulario()
  446 | 
  447 |   LOG.passo(`Preenchendo Objetivo: "${dadosI13.objetivo}"...`)
  448 |   await p.preencherObjetivo(dadosI13.objetivo)
  449 |   await expect(p.objetivo).toHaveValue(dadosI13.objetivo)
  450 | 
  451 |   LOG.passo(`Preenchendo Destino: "${dadosI13.destino}"...`)
  452 |   await p.preencherDestino(dadosI13.destino)
  453 |   await expect(p.destino).toHaveValue(dadosI13.destino)
  454 | 
  455 |   LOG.passo('Acionando botão "Limpar"...')
  456 |   const btnLimpar = page.getByRole('button', { name: /^Limpar$/ }).first()
  457 |   if (await btnLimpar.isVisible().catch(() => false)) {
  458 |     await btnLimpar.click()
  459 |     await expect(p.objetivo).toHaveValue('', { timeout: 5_000 }).catch(() => {})
  460 |     LOG.ok('Formulário limpo (Objetivo e Destino esvaziados)')
  461 |   } else {
  462 |     LOG.alerta('Botão "Limpar" não visível neste estado')
  463 |   }
  464 |   await p.evidencia('i13-formulario-limpo')
  465 | })
  466 | 
  467 | test('I-15 [Regressivo] Navegação de retorno ao dashboard', async ({ page }) => {
  468 |   const p = new ReembolsoPage(page)
  469 |   LOG.secao('I-15', 'Regressivo', 'Navegação de retorno ao dashboard')
```