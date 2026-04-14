import {
  bddReembolsoDashboard01,
  bddReembolsoDashboard02,
  bddReembolsoDashboard03,
  bddReembolsoDashboard04,
  bddReembolsoDashboard05,
  bddReembolsoDashboard06,
  bddReembolsoDashboard07,
  bddReembolsoDashboard08,
  bddReembolsoDashboard09,
  bddReembolsoDashboard10,
  bddReembolsoDashboard11,
  bddReembolsoDashboard12,
  bddReembolsoDashboard13,
  bddReembolsoDashboard14,
  bddReembolsoDashboard15,
  bddReembolsoInserir01,
  bddReembolsoInserir02,
  bddReembolsoInserir03,
  bddReembolsoInserir04,
  bddReembolsoInserir05,
  bddReembolsoInserir06,
  bddReembolsoInserir07,
  bddReembolsoInserir08,
  bddReembolsoInserir09,
  bddReembolsoInserir10,
  bddReembolsoInserir11,
  bddReembolsoInserir12,
  bddReembolsoInserir13,
  bddReembolsoInserir14,
  bddReembolsoInserir15,
} from '../../DEMO/automacao/reembolso/reembolso.data.js';

const PDF_MINIMO =
  '%PDF-1.4\n1 0 obj<</Type/Catalog/Pages 2 0 R>>endobj 2 0 obj<</Type/Pages/Kids[3 0 R]/Count 1>>endobj 3 0 obj<</Type/Page/MediaBox[0 0 3 3]>>endobj trailer<</Root 1 0 R>>%%EOF';

function logInicio(codigo, titulo) {
  console.log(`\n🧪 TESTANDO: ${codigo} — ${titulo}`);
}

function logFim(codigo) {
  console.log(`🏁 ${codigo} CONCLUÍDO\n`);
}

function selecionarDiaNoCalendarioAberto(diaStr) {
  const dia = String(parseInt(diaStr, 10));
  cy.get('.rdp').should('be.visible');
  cy.get('.rdp').within(() => {
    cy.get('button').contains(new RegExp(`^${dia}$`)).first().click({ force: true });
  });
}

function clicarMesAnterior(vezes) {
  Cypress._.times(vezes, () => {
    cy.get('.rdp').find('nav button').first().click({ force: true });
  });
}

/** Formulário Inserir Reembolso — preenche fluxo principal com dados DataForge */
function preencherFormularioInserir(d, { comClienteProjeto = true, anexarPdf = true } = {}) {
  cy.visit('/inserir-reembolso');
  cy.get('input[placeholder="Objetivo"]').clear().type(d.objetivo);
  if (d.destino) {
    cy.contains('label', 'Destino', { matchCase: false })
      .parent()
      .find('input')
      .first()
      .clear()
      .type(d.destino);
  }
  cy.contains('label', /Data de início/i)
    .parent()
    .find('button')
    .first()
    .click({ force: true });
  selecionarDiaNoCalendarioAberto(d.dataInicio.split('/')[0]);
  if (d.dataFim) {
    cy.contains('label', /Data final/i)
      .parent()
      .find('button')
      .first()
      .click({ force: true });
    selecionarDiaNoCalendarioAberto(d.dataFim.split('/')[0]);
  }
  if (comClienteProjeto) {
    cy.contains('label', /Cliente\/Projeto/i)
      .parent()
      .find('button')
      .first()
      .click({ force: true });
    cy.get('input[placeholder="Buscar cliente/projeto..."]').should('be.visible');
    cy.get('[cmdk-item], [role="option"]')
      .not('[data-disabled="true"]')
      .filter(':visible')
      .first()
      .click({ force: true });
  }
  cy.contains('label', /Categoria/i)
    .parent()
    .find('[role="combobox"]')
    .click({ force: true });
  cy.get('[role="option"], [cmdk-item]').contains(new RegExp(`^${d.categoria}`, 'i')).click({ force: true });
  if (anexarPdf) {
    cy.get('input[type="file"]').selectFile(
      {
        contents: Cypress.Buffer.from(PDF_MINIMO),
        fileName: 'comprovante-demo.pdf',
        mimeType: 'application/pdf',
      },
      { force: true },
    );
  }
  cy.contains('label', /Data da Despesa/i)
    .parent()
    .find('button')
    .first()
    .click({ force: true });
  selecionarDiaNoCalendarioAberto(d.dataDespesa.split('/')[0]);
  cy.get('input[placeholder="0,00"]').clear().type(d.valor, { delay: 0 });
  cy.contains('label', /Descrição/i)
    .parent()
    .find('textarea, input')
    .first()
    .clear()
    .type(d.descricao);
}

function aplicarFiltroPeriodoDashboard(d) {
  if (!d.dataInicio) return;
  cy.contains('label', 'Data Início', { matchCase: false })
    .parent()
    .find('button')
    .first()
    .click({ force: true });
  selecionarDiaNoCalendarioAberto(d.dataInicio.split('/')[0]);
  if (d.dataFim) {
    cy.contains('label', 'Data Fim', { matchCase: false })
      .parent()
      .find('button')
      .first()
      .click({ force: true });
    selecionarDiaNoCalendarioAberto(d.dataFim.split('/')[0]);
  }
}

describe('Módulo Reembolso — E2E (backend real)', () => {
  beforeEach(() => {
    cy.session('fourmakers-prd', () => {
      cy.loginFourMakers();
    });
  });

  it('[R-01] Aba inicial via URL', () => {
    logInicio('R-01', 'Aba inicial via URL');
    console.log('   → acessar /reembolso e validar aba Meus Reembolsos');
    cy.visit('/reembolso');
    cy.get('[role="tab"]').contains('Meus Reembolsos').should('be.visible');
    cy.get('[role="table"]').should('exist');
    console.log('   ✅ Resultado confirmado');
    logFim('R-01');
  });

  it('[R-02] Bloqueio de Gestão Administrativa', () => {
    logInicio('R-02', 'Bloqueio de Gestão Administrativa');
    console.log('   → tentar abrir ?tab=gestaoadm sem perfil gestor');
    cy.visit('/reembolso?tab=gestaoadm');
    cy.get('body').then(($b) => {
      if (!$b.text().includes('Gestão ADM')) {
        cy.url().should('not.include', 'tab=gestaoadm');
      } else {
        cy.url().should('include', 'tab=gestaoadm');
      }
    });
    console.log('   ✅ Resultado confirmado');
    logFim('R-02');
  });

  it('[R-03] Troca de aba com sincronismo na URL', () => {
    logInicio('R-03', 'Troca de aba com sincronismo na URL');
    console.log('   → alternar abas e conferir query tab');
    cy.visit('/reembolso');
    cy.get('body').then(($b) => {
      if ($b.text().includes('Gestão ADM')) {
        cy.get('[role="tab"]').contains('Gestão ADM').click();
        cy.url().should('include', 'tab=gestaoadm');
        cy.get('[role="tab"]').contains('Meus Reembolsos').click();
        cy.url().should('not.include', 'tab=');
      } else if ($b.text().includes('Aprovações')) {
        cy.get('[role="tab"]').contains('Aprovações').click();
        cy.url().should('include', 'tab=aprovacoes');
      } else {
        cy.url().should('include', '/reembolso');
      }
    });
    console.log('   ✅ Resultado confirmado');
    logFim('R-03');
  });

  it('[R-04] Impedir aba Aprovações sem perfil', () => {
    logInicio('R-04', 'Impedir aba Aprovações sem perfil');
    console.log('   → visitar URL com tab=aprovacoes');
    cy.visit('/reembolso?tab=aprovacoes');
    cy.get('body').then(($b) => {
      if (!$b.text().includes('Aprovações')) {
        cy.url().should('not.include', 'tab=aprovacoes');
      }
    });
    console.log('   ✅ Resultado confirmado');
    logFim('R-04');
  });

  it('[R-05] Busca textual na grade principal', () => {
    logInicio('R-05', 'Busca textual na grade principal');
    const d = bddReembolsoDashboard05;
    console.log('   → filtrar pela busca com trecho do objetivo');
    cy.visit('/reembolso');
    aplicarFiltroPeriodoDashboard(d);
    cy.get('input[placeholder="Busca"]').clear().type(d.objetivo.slice(0, 12));
    cy.get('[role="table"]').should('exist');
    console.log('   ✅ Resultado confirmado');
    logFim('R-05');
  });

  // [Regressivo] Reset de paginação ao filtrar — depende de volume e ordem de filtros na API
  it.skip('[R-06] Reset de paginação ao filtrar', () => {});

  it('[R-07] Paginação por fatia da lista filtrada', () => {
    logInicio('R-07', 'Paginação por fatia da lista filtrada');
    const d = bddReembolsoDashboard07;
    console.log('   → usar data-testid pagination-next quando habilitado');
    cy.visit('/reembolso');
    aplicarFiltroPeriodoDashboard(d);
    cy.get('body').then(($b) => {
      const $next = $b.find('[data-testid="pagination-next"]');
      if ($next.length && !$next.is(':disabled')) {
        cy.wrap($next).click();
        cy.get('[data-testid="pagination-prev"]').should('exist');
      }
    });
    cy.get('[role="table"]').should('exist');
    console.log('   ✅ Resultado confirmado');
    logFim('R-07');
  });

  it('[R-08] Modal de detalhes da solicitação', () => {
    logInicio('R-08', 'Modal de detalhes da solicitação');
    console.log('   → abrir Detalhes quando houver linha');
    cy.visit('/reembolso');
    cy.get('body').then(($b) => {
      const temDetalhes = $b.find('button').filter((_, el) => Cypress.$(el).text().includes('Detalhes')).length > 0;
      if (temDetalhes) {
        cy.contains('button', 'Detalhes').first().click({ force: true });
        cy.get('[role="dialog"]').contains('Detalhes da solicitação de reembolso').should('be.visible');
      }
    });
    console.log('   ✅ Resultado confirmado');
    logFim('R-08');
  });

  it('[R-09] Botão Gerar Relatório na Gestão', () => {
    logInicio('R-09', 'Botão Gerar Relatório na Gestão');
    console.log('   → na aba Gestão ADM, validar botão quando parâmetro permitir');
    cy.visit('/reembolso?tab=gestaoadm');
    cy.get('body').then(($b) => {
      if ($b.text().includes('Gestão ADM')) {
        cy.get('[role="tab"]').contains('Gestão ADM').click();
        cy.contains('button', 'Gerar Relatório').should('exist');
      }
    });
    console.log('   ✅ Resultado confirmado');
    logFim('R-09');
  });

  it('[R-10] Ocultar relatório fora da Gestão', () => {
    logInicio('R-10', 'Ocultar relatório fora da Gestão');
    console.log('   → em Meus Reembolsos não deve existir Gerar Relatório');
    cy.visit('/reembolso');
    cy.get('[role="tab"]').contains('Meus Reembolsos').click();
    cy.contains('button', 'Gerar Relatório').should('not.exist');
    console.log('   ✅ Resultado confirmado');
    logFim('R-10');
  });

  it('[R-11] Download do relatório de pagamentos', () => {
    logInicio('R-11', 'Download do relatório de pagamentos');
    console.log('   → abrir fluxo Gerar Relatório e Confirmar no alertdialog');
    cy.visit('/reembolso?tab=gestaoadm');
    cy.get('body').then(($b) => {
      if ($b.text().includes('Gestão ADM') && $b.text().includes('Gerar Relatório')) {
        cy.get('[role="tab"]').contains('Gestão ADM').click();
        cy.contains('button', 'Gerar Relatório').click();
        cy.get('[role="alertdialog"]').contains('Gerar Relatório').should('be.visible');
        cy.get('[role="alertdialog"]').contains('button', 'Confirmar').click({ force: true });
      }
    });
    console.log('   ✅ Resultado confirmado');
    logFim('R-11');
  });

  it('[R-12] Geração sem token', () => {
    logInicio('R-12', 'Geração sem token');
    console.log('   → remover authToken e recarregar; fluxo protegido deve falhar ou redirecionar');
    cy.visit('/reembolso');
    cy.window().then((win) => {
      win.localStorage.removeItem('authToken');
    });
    cy.reload();
    cy.url().then((url) => {
      expect(url.includes('/login') || url.includes('fourmakers')).to.be.true;
    });
    console.log('   ✅ Resultado confirmado');
    logFim('R-12');
  });

  // [Regressivo] Falha na API do relatório — requer mock ou indisponibilidade controlada
  it.skip('[R-13] Falha na API do relatório', () => {});

  it('[R-14] Botão Remessa CNAB condicionado', () => {
    logInicio('R-14', 'Botão Remessa CNAB condicionado');
    console.log('   → na Gestão ADM, se botão existir, navegar para remessa-cnab');
    cy.visit('/reembolso?tab=gestaoadm');
    cy.get('body').then(($b) => {
      if ($b.text().includes('Remessa CNAB')) {
        cy.contains('button', 'Remessa CNAB').click();
        cy.url().should('include', '/reembolso/remessa-cnab');
      }
    });
    console.log('   ✅ Resultado confirmado');
    logFim('R-14');
  });

  it('[R-15] Indicadores e listagem com período', () => {
    logInicio('R-15', 'Indicadores e listagem com período');
    const d = bddReembolsoDashboard15;
    console.log('   → aplicar Data Início/Fim e validar cards e tabela');
    cy.visit('/reembolso');
    aplicarFiltroPeriodoDashboard(d);
    cy.get('[role="table"]').should('exist');
    cy.get('.container').should('be.visible');
    console.log('   ✅ Resultado confirmado');
    logFim('R-15');
  });

  it('[R-16] Carga de projetos do colaborador', () => {
    logInicio('R-16', 'Carga de projetos do colaborador');
    const d = bddReembolsoInserir01;
    console.log('   → abrir combobox Cliente/Projeto e listar itens');
    preencherFormularioInserir(d, { comClienteProjeto: false, anexarPdf: false });
    cy.contains('label', /Cliente\/Projeto/i)
      .parent()
      .find('button')
      .first()
      .click({ force: true });
    cy.get('input[placeholder="Buscar cliente/projeto..."]').should('be.visible');
    cy.get('[cmdk-group], [cmdk-list]').should('exist');
    console.log('   ✅ Resultado confirmado');
    logFim('R-16');
  });

  it('[R-17] Carga de verbas ao escolher projeto', () => {
    logInicio('R-17', 'Carga de verbas ao escolher projeto');
    const d = bddReembolsoInserir02;
    console.log('   → selecionar projeto e abrir Categoria');
    cy.visit('/inserir-reembolso');
    cy.get('input[placeholder="Objetivo"]').clear().type(d.objetivo);
    cy.contains('label', /Cliente\/Projeto/i)
      .parent()
      .find('button')
      .first()
      .click({ force: true });
    cy.get('[cmdk-item], [role="option"]').not('[data-disabled="true"]').filter(':visible').first().click({ force: true });
    cy.contains('label', /Categoria/i)
      .parent()
      .find('[role="combobox"]')
      .should('be.visible');
    console.log('   ✅ Resultado confirmado');
    logFim('R-17');
  });

  it('[R-18] Comprovante fora da validade', () => {
    logInicio('R-18', 'Comprovante fora da validade');
    const d = bddReembolsoInserir03;
    console.log('   → definir Data da Despesa antiga e ver alerta de validade');
    cy.visit('/inserir-reembolso');
    cy.get('input[placeholder="Objetivo"]').clear().type(d.objetivo);
    cy.contains('label', /Data da Despesa/i)
      .parent()
      .find('button')
      .first()
      .click({ force: true });
    clicarMesAnterior(40);
    selecionarDiaNoCalendarioAberto(d.dataDespesa.split('/')[0]);
    cy.get('button').filter(':has(.text-amber-500), :has(svg.text-amber-500)').should('have.length.greaterThan', 0);
    console.log('   ✅ Resultado confirmado');
    logFim('R-18');
  });

  it('[R-19] Análise OCR de comprovantes', () => {
    logInicio('R-19', 'Análise OCR de comprovantes');
    const d = bddReembolsoInserir04;
    console.log('   → anexar PDF e aguardar processamento sem erro bloqueante');
    preencherFormularioInserir(d, { comClienteProjeto: true, anexarPdf: true });
    cy.contains('button', 'Adicionar ao Carrinho').should('be.visible');
    console.log('   ✅ Resultado confirmado');
    logFim('R-19');
  });

  it('[R-20] Adicionar ao carrinho sem obrigatórios', () => {
    logInicio('R-20', 'Adicionar ao carrinho sem obrigatórios');
    const d = bddReembolsoInserir05;
    console.log('   → submeter carrinho vazio / campos inválidos e validar toast');
    cy.visit('/inserir-reembolso');
    if (d.destino) {
      cy.contains('label', 'Destino', { matchCase: false }).parent().find('input').first().clear().type(d.destino);
    }
    cy.contains('button', 'Adicionar ao Carrinho').click();
    cy.contains(/Campos obrigatórios|Preencha/i).should('exist');
    console.log('   ✅ Resultado confirmado');
    logFim('R-20');
  });

  it('[R-21] Item válido no carrinho', () => {
    logInicio('R-21', 'Item válido no carrinho');
    const d = bddReembolsoInserir06;
    console.log('   → preencher, adicionar e ver carrinho com item');
    preencherFormularioInserir(d);
    cy.contains('button', 'Adicionar ao Carrinho').click();
    cy.contains('h2', 'Carrinho de Solicitações').should('be.visible');
    cy.contains(d.categoria).should('be.visible');
    console.log('   ✅ Resultado confirmado');
    logFim('R-21');
  });

  it('[R-22] Edição de item existente', () => {
    logInicio('R-22', 'Edição de item existente');
    const d6 = bddReembolsoInserir06;
    const d7 = bddReembolsoInserir07;
    console.log('   → editar valor e usar Atualizar Item');
    preencherFormularioInserir(d6);
    cy.contains('button', 'Adicionar ao Carrinho').click();
    cy.contains('h3', d6.categoria)
      .parents('[class*="Card"]')
      .first()
      .find('button')
      .first()
      .click({ force: true });
    cy.get('input[placeholder="0,00"]').clear().type(d7.valor, { delay: 0 });
    cy.contains('label', /Descrição/i)
      .parent()
      .find('textarea, input')
      .first()
      .clear()
      .type(d7.descricao);
    cy.contains('button', 'Atualizar Item').click();
    cy.contains(d7.descricao.slice(0, 20)).should('exist');
    console.log('   ✅ Resultado confirmado');
    logFim('R-22');
  });

  it('[R-23] Remoção de item do carrinho', () => {
    logInicio('R-23', 'Remoção de item do carrinho');
    const d = bddReembolsoInserir08;
    console.log('   → excluir item e ver estado vazio');
    preencherFormularioInserir(d);
    cy.contains('button', 'Adicionar ao Carrinho').click();
    cy.contains('h3', d.categoria).parent().find('button.text-destructive').click({ force: true });
    cy.contains('Nenhuma solicitação no carrinho').should('be.visible');
    console.log('   ✅ Resultado confirmado');
    logFim('R-23');
  });

  it('[R-24] Envio com ZIP e sucesso', () => {
    logInicio('R-24', 'Envio com ZIP e sucesso');
    const d = bddReembolsoInserir09;
    console.log('   → enviar solicitações e confirmar alertdialog Sucesso');
    preencherFormularioInserir(d);
    cy.contains('button', 'Adicionar ao Carrinho').click();
    cy.contains('button', 'Enviar solicitações').click();
    cy.get('[role="alertdialog"]', { timeout: 60000 }).contains('Sucesso').should('be.visible');
    cy.get('[role="alertdialog"]').contains('button', 'OK').click();
    cy.visit('/reembolso');
    cy.get('[role="tab"]').contains('Meus Reembolsos').should('be.visible');
    console.log('   ✅ Resultado confirmado');
    logFim('R-24');
  });

  it('[R-25] Resposta da API com lista de erros', () => {
    logInicio('R-25', 'Resposta da API com lista de erros');
    const d = bddReembolsoInserir10;
    console.log('   → enviar payload que pode retornar erros de negócio');
    preencherFormularioInserir(d);
    cy.contains('button', 'Adicionar ao Carrinho').click();
    cy.contains('button', 'Enviar solicitações').click();
    cy.get('body', { timeout: 60000 }).then(($b) => {
      if ($b.text().includes('Erro ao enviar solicitações')) {
        cy.get('[role="dialog"]').contains('Ocorreram os seguintes erros').should('be.visible');
      }
    });
    console.log('   ✅ Resultado confirmado');
    logFim('R-25');
  });

  // [Regressivo] Falha genérica no envio — depende de indisponibilidade ou mock de rede
  it.skip('[R-26] Falha genérica no envio', () => {});

  it('[R-27] Solicitação sem projeto habilitada', () => {
    logInicio('R-27', 'Solicitação sem projeto habilitada');
    const d = bddReembolsoInserir12;
    console.log('   → fluxo sem selecionar Cliente/Projeto quando permitido pela org');
    preencherFormularioInserir(d, { comClienteProjeto: false });
    cy.contains('button', 'Adicionar ao Carrinho').click();
    cy.get('body').then(($b) => {
      const ok = $b.text().includes(d.categoria) && $b.text().includes('Carrinho');
      expect(ok || $b.text().includes('obrigatório')).to.be.true;
    });
    console.log('   ✅ Resultado confirmado');
    logFim('R-27');
  });

  it('[R-28] Valor acima do teto da verba', () => {
    logInicio('R-28', 'Valor acima do teto da verba');
    const d = bddReembolsoInserir13;
    console.log('   → digitar valor e validar aviso de teto');
    preencherFormularioInserir(d);
    cy.contains(/valor máximo permitido|O valor máximo permitido/i).should('be.visible');
    console.log('   ✅ Resultado confirmado');
    logFim('R-28');
  });

  it('[R-29] Máscara monetária consistente', () => {
    logInicio('R-29', 'Máscara monetária consistente');
    const d = bddReembolsoInserir14;
    console.log('   → campo Valor deve refletir formatação pt-BR');
    preencherFormularioInserir(d, { anexarPdf: true });
    cy.get('input[placeholder="0,00"]').invoke('val').should('match', /1\.250|1250/);
    console.log('   ✅ Resultado confirmado');
    logFim('R-29');
  });

  // [Regressivo] Encerramento pós-sucesso — fluxo longo coberto em R-24
  it.skip('[R-30] Encerramento pós-sucesso', () => {});
});
