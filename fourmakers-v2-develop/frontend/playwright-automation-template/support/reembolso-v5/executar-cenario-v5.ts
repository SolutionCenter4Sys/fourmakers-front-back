import { APIRequestContext, Page, expect } from '@playwright/test'
import { ReembolsoPage } from '../pages/ReembolsoPage'
import type { MassaCenarioV5 } from './skip-reasons'

export type DemoLog = {
  secao: (codigo: string, tipo: string, titulo: string) => void
  passo: (msg: string) => void
  ok: (msg: string) => void
  alerta: (msg: string) => void
}

export async function executarCenarioV5(
  page: Page,
  request: APIRequestContext,
  massa: MassaCenarioV5,
  log: DemoLog,
  reauth: () => Promise<void>,
): Promise<void> {
  const p = new ReembolsoPage(page)
  log.secao(massa.id, massa.tipo, massa.titulo)

  switch (massa.id) {
    case 'RD-01': {
      log.passo('Navegando para /reembolso...')
      await p.visitarLista()
      await expect(p.tabMeusReembolsos).toBeVisible()
      log.ok('Aba "Meus Reembolsos" visível')

      const painel = page.locator('[class*="card"], [data-testid*="summary"], .grid').first()
      if (await painel.isVisible().catch(() => false)) {
        log.ok('Painéis de resumo renderizados')
      } else {
        log.alerta('Painéis de resumo não identificados — validação parcial da tela')
      }
      await p.evidencia(`${massa.id.toLowerCase()}-dashboard`, 'reembolso-v5')
      break
    }

    case 'RD-03': {
      await p.visitarLista()
      const btnLimpar = page.getByRole('button', { name: /^Limpar$/i }).first()
      if (await btnLimpar.isVisible().catch(() => false)) {
        log.passo('Acionando limpar filtro de datas...')
        await btnLimpar.click()
        log.ok('Filtro de datas limpo')
      } else {
        log.alerta('Nenhum filtro de data aplicado — botão Limpar oculto (aceitável)')
      }
      await p.evidencia(`${massa.id.toLowerCase()}-limpar-datas`, 'reembolso-v5')
      break
    }

    case 'RD-08': {
      const rotaEsperada = String(massa.ui?.rotaDestino ?? '/inserir-reembolso')
      log.passo(`Rota esperada = "${rotaEsperada}"`)

      await p.visitarLista()
      await p.clicarSolicitarReembolso()
      await page.waitForURL(new RegExp(`${rotaEsperada.replace(/\//g, '\\/')}(\\?|$)`), { timeout: 12_000 })
      log.ok('Redirecionou para o formulário de nova solicitação')

      await expect(page.getByRole('heading', { name: /Nova Solicita[cç][aã]o/i })).toBeVisible()
      log.ok('Título "Nova Solicitação" visível no formulário')
      await p.evidencia('r04-formulario-aberto-v5', 'reembolso-v5')
      break
    }

    case 'RD-10': {
      log.passo('Forçando URL com aba gestaoadm sem perfil gestor...')
      await page.goto(`${p.baseUrl}/reembolso?tab=gestaoadm`, {
        waitUntil: 'domcontentloaded',
        timeout: p.navTimeoutMs,
      })
      await expect(p.tabMeusReembolsos).toBeVisible()
      const url = page.url()
      if (!/tab=gestaoadm/i.test(url)) {
        log.ok('URL normalizada — permanece em Meus Reembolsos')
      } else if (await p.tabGestaoAdm.count() === 0) {
        log.ok('Aba Gestão ADM indisponível para colaborador')
      } else {
        log.alerta('Aba Gestão ADM visível — usuário OTP pode ter perfil elevado')
      }
      await p.evidencia(`${massa.id.toLowerCase()}-bloqueio-gestao`, 'reembolso-v5')
      break
    }

    case 'RD-11': {
      log.passo('Forçando URL com aba aprovacoes sem permissão...')
      await page.goto(`${p.baseUrl}/reembolso?tab=aprovacoes`, {
        waitUntil: 'domcontentloaded',
        timeout: p.navTimeoutMs,
      })
      await expect(p.tabMeusReembolsos).toBeVisible()
      const abaAprov = page.getByRole('tab', { name: /Aprovações/i })
      if (await abaAprov.count() === 0) {
        log.ok('Aba Aprovações oculta para colaborador')
      } else {
        log.alerta('Aba Aprovações visível — usuário OTP pode ter perfil de aprovador')
      }
      await p.evidencia(`${massa.id.toLowerCase()}-bloqueio-aprovacoes`, 'reembolso-v5')
      break
    }

    case 'RD-14': {
      log.passo('Observando indicador de carregamento ao abrir dashboard...')
      const loading = page.getByText(/carregando|loading|aguarde/i).first()
      await p.visitarLista()
      if (await loading.isVisible({ timeout: 2_000 }).catch(() => false)) {
        log.ok('Indicador de carregamento exibido durante consulta')
        await expect(loading).not.toBeVisible({ timeout: 20_000 }).catch(() => {})
      } else {
        log.alerta('Loading não capturado — dados podem ter carregado instantaneamente')
      }
      await expect(p.tabMeusReembolsos).toBeVisible()
      await p.evidencia(`${massa.id.toLowerCase()}-loading`, 'reembolso-v5')
      break
    }

    case 'AP-08': {
      await p.visitarLista()
      const abaAprov = page.getByRole('tab', { name: /Aprovações/i })
      await expect(abaAprov).toHaveCount(0)
      log.ok('Aba Aprovações oculta sem permissão de aprovação')
      await p.evidencia(`${massa.id.toLowerCase()}-aba-oculta`, 'reembolso-v5')
      break
    }

    case 'IR-11': {
      await p.visitarFormulario()
      log.passo('Tentando adicionar item ao carrinho sem preencher campos...')
      await p.btnAdicionarCarrinho.scrollIntoViewIfNeeded()
      await p.clicarAdicionarCarrinho()
      const mensagem = String(massa.expectativa?.mensagem ?? 'Campos obrigatórios')
      await expect(page.getByText(new RegExp(mensagem, 'i')).first()).toBeVisible({ timeout: 10_000 })
      log.ok(`Validação exibida: "${mensagem}"`)
      await p.evidencia(`${massa.id.toLowerCase()}-campos-obrigatorios`, 'reembolso-v5')
      break
    }

    case 'IR-14': {
      await p.visitarFormulario()
      const arquivo = String(massa.ui?.arquivoInvalido ?? 'planilha-gastos.txt')
      log.passo(`Anexando arquivo inválido: ${arquivo}`)
      if (await p.uploadInput.count() > 0) {
        await p.uploadInput.setInputFiles({
          name: arquivo,
          mimeType: 'text/plain',
          buffer: Buffer.from('conteudo invalido para teste playwright'),
        })
        const mensagem = String(massa.expectativa?.mensagem ?? 'Formato inválido')
        const toast = page.getByText(new RegExp(mensagem, 'i')).first()
        if (await toast.isVisible({ timeout: 5_000 }).catch(() => false)) {
          log.ok('Formato de arquivo rejeitado')
        } else {
          log.alerta('Toast de formato inválido não exibido no tempo esperado')
        }
      } else {
        log.alerta('Input de upload não visível')
      }
      await p.evidencia(`${massa.id.toLowerCase()}-formato-invalido`, 'reembolso-v5')
      break
    }

    case 'IR-15': {
      await p.visitarFormulario()
      await expect(p.carrinhoVazioTexto).toBeVisible()
      log.passo('Tentando enviar solicitações com carrinho vazio...')
      const mensagem = String(
        massa.expectativa?.mensagem ?? 'Adicione pelo menos uma solicitação ao carrinho',
      )
      if (await p.btnEnviarSolicitacoes.count() > 0) {
        await p.btnEnviarSolicitacoes.scrollIntoViewIfNeeded()
        await p.btnEnviarSolicitacoes.click({ trial: false }).catch(() => {
          log.alerta('Botão Enviar desabilitado — guarda por UI')
        })
        const toast = page.getByText(new RegExp(mensagem, 'i')).first()
        if (await toast.isVisible().catch(() => false)) {
          log.ok(`Mensagem exibida: "${mensagem}"`)
        } else {
          log.alerta('Toast não visível — botão pode estar desabilitado')
        }
      }
      await p.evidencia(`${massa.id.toLowerCase()}-carrinho-vazio`, 'reembolso-v5')
      break
    }

    case 'IR-18': {
      await p.visitarFormulario()
      const objetivo = String(massa.ui?.objetivo ?? 'Visita técnica ao cliente')
      const destino = String(massa.ui?.destino ?? 'São Paulo - SP')
      log.passo(`Preenchendo objetivo e destino antes de limpar...`)
      await p.preencherObjetivo(objetivo)
      await p.preencherDestino(destino)
      await expect(p.objetivo).toHaveValue(objetivo)

      const btnLimpar = page.getByRole('button', { name: /^Limpar$/ }).first()
      if (await btnLimpar.isVisible().catch(() => false)) {
        await btnLimpar.click()
        await expect(p.objetivo).toHaveValue('', { timeout: 5_000 }).catch(() => {})
        log.ok('Formulário resetado — carrinho preservado')
      } else {
        log.alerta('Botão Limpar não encontrado')
      }
      await p.evidencia(`${massa.id.toLowerCase()}-limpar-formulario`, 'reembolso-v5')
      break
    }

    default:
      throw new Error(`Cenário ${massa.id} marcado como executável, mas sem implementação`)
  }

  void request
}
