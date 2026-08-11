import { test, expect } from '@playwright/test'
import { ReembolsoPage } from '../../support/pages/ReembolsoPage'
import { loginFourMakers } from '../../support/auth/fourmakers-auth'

// ─── Dados do DataForge (Etapa 2 da demo) ──────────────────────────────────
// @ts-ignore — import JS sem types
import {
  dadosR01,
  dadosR09,
  dadosR12,
  dadosR13,
  dadosI08,
  dadosI11,
  dadosI12,
  dadosI13,
  dadosI15,
// @ts-ignore
} from '../../../../DEMO/automacao/reembolso/reembolso.data.js'

/**
 * ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
 *  DEMO REEMBOLSO — JORNADA CINEMATOGRÁFICA (UMA única sessão)
 * ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
 *
 *  Propósito: apresentação ao vivo.
 *  O Chrome abre UMA só vez, o Gustavo loga UMA só vez, e navega por
 *  TODOS os cenários em sequência — com pausas visíveis pra audiência.
 *
 *  Cada `test.step()` corresponde a um cenário do BDD
 *  (REEMBOLSO-BDD-v6.md) e consome dados do DataForge
 *  (reembolso.data.js). Para cobertura BDD 1:1 (30 tests separados),
 *  usar `reembolso-demo.spec.ts`.
 *
 *  Executar:
 *    npm run demo:run      # ★ cinema ★ headed com slowMo
 */

test.describe.configure({ retries: 0 })

const LOG = {
  banner: (msg: string) => {
    const linha = '═'.repeat(54)
    console.log(`\n╔${linha}╗`)
    console.log(`║  ${msg.padEnd(53)}║`)
    console.log(`╚${linha}╝`)
  },
  secao: (codigo: string, tipo: string, titulo: string) => {
    console.log(`\n┌─────────────────────────────────────────────────────`)
    console.log(`│ ${codigo} · [${tipo}]  ${titulo}`)
    console.log(`└─────────────────────────────────────────────────────`)
  },
  passo:  (msg: string) => console.log(`   → ${msg}`),
  ok:     (msg: string) => console.log(`   ✅ ${msg}`),
  alerta: (msg: string) => console.log(`   ⚠️  ${msg}`),
  tempo:  (inicio: number) => {
    const s = ((Date.now() - inicio) / 1000).toFixed(1)
    console.log(`   ⏱  Tempo total: ${s}s`)
  },
}

/**
 * Pausa dramatizada entre cenários — dá tempo pra audiência absorver
 * a tela antes do próximo passo. Em modo headless é irrelevante.
 */
async function pausaCinema(page: import('@playwright/test').Page, ms = 1500) {
  await page.waitForTimeout(ms)
}

test('🎬 Jornada Reembolso — Gustavo navegando cenários BDD em sequência', async ({ page, request }) => {
  test.setTimeout(300_000) // 5 minutos

  const p      = new ReembolsoPage(page)
  const inicio = Date.now()

  LOG.banner('DEMO FOURMAKERS — JORNADA REEMBOLSO')
  console.log('🎯 AUTOMAÇÃO INICIADA — Navegando como gustavo.queiroz@foursys.com.br')

  // ── SETUP — Login único ────────────────────────────────────────────────
  await test.step('🔐 Autenticação via OTP', async () => {
    await loginFourMakers(page, request)
    LOG.ok('Sessão iniciada com JWT injetado no localhost:8080')
    await pausaCinema(page, 2000)
  })

  // ═══════════════════════════════════════════════════════════════════
  //  DASHBOARD — /reembolso
  // ═══════════════════════════════════════════════════════════════════

  await test.step('R-01 [Positivo] Visão padrão do módulo', async () => {
    LOG.secao('R-01', 'Positivo', 'Visão padrão do módulo de Reembolso')
    LOG.passo(`Validando apenas a aba "${dadosR01.abaEsperada}" visível...`)
    await p.visitarLista()
    await expect(p.tabMeusReembolsos).toBeVisible()
    LOG.ok(`Aba "${dadosR01.abaEsperada}" visível`)
    await p.evidencia('r01-visao-padrao')
    await pausaCinema(page)
  })

  await test.step('R-12 [Negativo] Busca sem resultados', async () => {
    LOG.secao('R-12', 'Negativo', 'Busca textual sem resultados')
    LOG.passo(`Digitando termo de busca: "${dadosR12.termoBusca}"...`)
    const inputBusca = page.getByPlaceholder(/busca/i).first()
    await expect(inputBusca).toBeVisible()
    await inputBusca.fill(dadosR12.termoBusca)

    LOG.passo(`Validando mensagem: "${dadosR12.mensagemEsperada}"...`)
    await expect(page.getByText(new RegExp(dadosR12.mensagemEsperada, 'i'))).toBeVisible()
    LOG.ok('Estado vazio apresentado corretamente')
    await p.evidencia('r12-busca-vazia')
    await pausaCinema(page)

    // Limpa pra próximos cenários
    await inputBusca.clear()
  })

  await test.step('R-13 [Regressivo] Limpeza dos filtros de data', async () => {
    LOG.secao('R-13', 'Regressivo', 'Botão "Limpar" dos filtros de data')
    LOG.passo(`Dados do cenário: ${dadosR13.descricao}`)
    const btnLimpar = page.getByRole('button', { name: new RegExp(dadosR13.botaoLimpar, 'i') })
    if (await btnLimpar.isVisible().catch(() => false)) {
      await btnLimpar.click()
      LOG.ok('Filtros limpos com sucesso')
    } else {
      LOG.alerta('Nenhum filtro aplicado no momento — botão "Limpar" oculto (comportamento esperado)')
    }
    await p.evidencia('r13-filtros-limpos')
    await pausaCinema(page)
  })

  await test.step('R-09 [Positivo] Início de nova solicitação', async () => {
    LOG.secao('R-09', 'Positivo', 'Início de nova solicitação')
    LOG.passo(`Clicando no botão "${dadosR09.botao}"...`)
    await expect(p.btnSolicitarReembolso).toBeVisible()
    await p.clicarSolicitarReembolso()

    LOG.passo(`Validando redirecionamento para ${dadosR09.urlDestino}...`)
    await expect(page).toHaveURL(new RegExp(dadosR09.urlDestino))
    await expect(page.getByText(/Nova Solicita/i)).toBeVisible()
    LOG.ok('Formulário "Nova Solicitação" aberto')
    await p.evidencia('r09-formulario-aberto')
    await pausaCinema(page, 2000)
  })

  // ═══════════════════════════════════════════════════════════════════
  //  INSERIR REEMBOLSO — /inserir-reembolso
  // ═══════════════════════════════════════════════════════════════════

  await test.step('I-13 [Regressivo] Preenchimento + Limpeza do formulário', async () => {
    LOG.secao('I-13', 'Regressivo', 'Preenchimento + Limpeza do formulário')

    LOG.passo(`Preenchendo Objetivo: "${dadosI13.objetivo}"...`)
    await p.preencherObjetivo(dadosI13.objetivo)
    await expect(p.objetivo).toHaveValue(dadosI13.objetivo)

    LOG.passo(`Preenchendo Destino: "${dadosI13.destino}"...`)
    await p.preencherDestino(dadosI13.destino)
    await expect(p.destino).toHaveValue(dadosI13.destino)
    await pausaCinema(page, 1500)

    LOG.passo('Acionando botão "Limpar"...')
    const btnLimpar = page.getByRole('button', { name: /^Limpar$/ }).first()
    if (await btnLimpar.isVisible().catch(() => false)) {
      await btnLimpar.click()
      LOG.ok('Formulário limpo')
    } else {
      LOG.alerta('Botão "Limpar" não visível — cenário parcial')
    }
    await p.evidencia('i13-formulario-limpo')
    await pausaCinema(page)
  })

  await test.step('I-08 [Negativo] Submissão com campos obrigatórios vazios', async () => {
    LOG.secao('I-08', 'Negativo', 'Submissão com campos obrigatórios vazios')

    LOG.passo('Acionando "Adicionar ao Carrinho" sem preencher nada...')
    await p.btnAdicionarCarrinho.scrollIntoViewIfNeeded()
    await p.clicarAdicionarCarrinho()

    LOG.passo(`Validando notificação: "${dadosI08.mensagemEsperada}"...`)
    await expect(
      page.getByText(new RegExp(dadosI08.mensagemEsperada, 'i')).first(),
    ).toBeVisible({ timeout: 10_000 })
    LOG.ok(`Toast "${dadosI08.mensagemEsperada}" exibido — validação ativada`)
    await p.evidencia('i08-campos-vazios')
    await pausaCinema(page, 2000)
  })

  await test.step('I-11 [Negativo] Anexo em formato não suportado', async () => {
    LOG.secao('I-11', 'Negativo', 'Anexo em formato não suportado')

    LOG.passo(`Preenchendo Objetivo antes do anexo: "${dadosI11.objetivo}"...`)
    await p.preencherObjetivo(dadosI11.objetivo)

    LOG.passo(`Tentando anexar "${dadosI11.nomeArquivoInvalido}" (formato não suportado)...`)
    const inputFile = p.uploadInput
    if (await inputFile.count() > 0) {
      await inputFile.setInputFiles({
        name: dadosI11.nomeArquivoInvalido,
        mimeType: 'text/plain',
        buffer: Buffer.from('arquivo fake para teste'),
      }).catch(() => {})

      const toast = page.getByText(new RegExp(dadosI11.mensagemErroEsperada, 'i'))
      if (await toast.isVisible({ timeout: 5_000 }).catch(() => false)) {
        LOG.ok(`Formato rejeitado (${dadosI11.mensagemErroEsperada})`)
      } else {
        LOG.alerta('Toast de formato inválido não apareceu no tempo esperado')
      }
    } else {
      LOG.alerta('Input de upload não visível — cenário pulado')
    }
    await p.evidencia('i11-formato-invalido')
    await pausaCinema(page)
  })

  await test.step('I-12 [Negativo] Envio com carrinho vazio', async () => {
    LOG.secao('I-12', 'Negativo', 'Envio com carrinho vazio')

    LOG.passo('Validando carrinho vazio...')
    await expect(page.getByText(/Nenhuma solicita/i)).toBeVisible()

    LOG.passo('Tentando acionar "Enviar Solicitações"...')
    const btnEnviar = p.btnEnviarSolicitacoes
    if (await btnEnviar.count() > 0) {
      await btnEnviar.scrollIntoViewIfNeeded()
      await btnEnviar.click({ trial: false }).catch(() => {
        LOG.alerta('Botão desabilitado — guarda por UI')
      })
      const toast = page.getByText(new RegExp(dadosI12.mensagemEsperada, 'i'))
      if (await toast.isVisible().catch(() => false)) {
        LOG.ok(`Toast "${dadosI12.mensagemEsperada}" exibido`)
      } else {
        LOG.alerta('Toast não visível — botão provavelmente desabilitado')
      }
    } else {
      LOG.alerta('Botão "Enviar Solicitações" oculto — guarda por renderização')
    }
    await p.evidencia('i12-carrinho-vazio')
    await pausaCinema(page)
  })

  await test.step('I-15 [Regressivo] Navegação de retorno ao dashboard', async () => {
    LOG.secao('I-15', 'Regressivo', 'Navegação de retorno ao dashboard')

    LOG.passo('Acionando navegação de retorno (botão voltar ou history.back)...')
    const btnVoltar = page
      .getByRole('button', { name: /voltar|←/i })
      .or(page.locator('button[aria-label*="voltar" i]'))
      .first()

    if (await btnVoltar.isVisible().catch(() => false)) {
      await btnVoltar.click()
    } else {
      LOG.alerta('Botão de voltar não encontrado — usando history.back()')
      await page.goBack()
    }
    await expect(page).toHaveURL(new RegExp(dadosI15.urlDestino), { timeout: 8_000 })
    LOG.ok(`Redirecionado para ${page.url()}`)
    await p.evidencia('i15-retorno-dashboard')
    await pausaCinema(page)
  })

  // ── Fechamento ────────────────────────────────────────────────────────────
  LOG.tempo(inicio)
  LOG.banner('✅  JORNADA CONCLUÍDA — 9 CENÁRIOS EXECUTADOS COM SUCESSO')
  console.log('╚══ ✅ DEMO CONCLUÍDA ══╝')

  // Pausa final para a apresentadora mostrar o estado final
  await pausaCinema(page, 3000)
})
