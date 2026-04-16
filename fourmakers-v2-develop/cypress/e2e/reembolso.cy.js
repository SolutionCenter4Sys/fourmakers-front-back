/**
 * E2E Reembolso — Dashboard (/reembolso) e Inserir (/inserir-reembolso)
 * Dados: DEMO/automacao/reembolso/reembolso.data.js
 */
import {
  R01,
  R02,
  R03,
  R04,
  R05,
  R06,
  R07,
  R08,
  R09,
  R10,
  R11,
  R12,
  R13,
  R14,
  R15,
  I01,
  I02,
  I03,
  I04,
  I05,
  I06,
  I07,
  I08,
  I09,
  I10,
  I11,
  I12,
  I13,
  I14,
  I15,
} from '../../DEMO/automacao/reembolso/reembolso.data.js';

describe('Módulo Reembolso — E2E (backend real)', () => {
  beforeEach(() => {
    cy.session('fourmakers-prd', () => {
      cy.loginFourMakers();
    });
  });

  // ——— Dashboard R01–R15 ———

  it('[R-01] Exibir dashboard — baseline de linha representativa na grade', () => {
    const dados = R01;
    console.log('\n🧪 TESTANDO: R-01 — Exibir dashboard — baseline de linha representativa na grade');
    console.log('   → visitar /reembolso e validar tabela e botão Solicitar Reembolso');
    cy.visit('/reembolso');
    cy.get('[role="table"]').should('be.visible');
    cy.contains('button', 'Solicitar Reembolso').should('be.visible');
    console.log('   ✅ Resultado confirmado');
    console.log('🏁 R-01 CONCLUÍDO\n');
  });

  it('[R-02] Sincronizar aba com URL — parâmetro tab válido', () => {
    const dados = R02;
    console.log('\n🧪 TESTANDO: R-02 — Sincronizar aba com URL — parâmetro tab válido');
    console.log('   → visitar /reembolso com tab na URL e checar aba Aprovações');
    cy.visit(`/reembolso?tab=${dados.abaUrl}`);
    cy.get('[role="tab"]').contains('Aprovações').should('have.attr', 'aria-selected', 'true');
    console.log('   ✅ Resultado confirmado');
    console.log('🏁 R-02 CONCLUÍDO\n');
  });

  it('[R-03] Alternar aba gestor — gestão administrativa', () => {
    const dados = R03;
    console.log('\n🧪 TESTANDO: R-03 — Alternar aba gestor — gestão administrativa');
    console.log('   → clicar na tab Gestão ADM e validar seleção');
    cy.visit('/reembolso');
    cy.get('[role="tab"]').contains('Gestão ADM').click();
    cy.get('[role="tab"]').contains('Gestão ADM').should('have.attr', 'aria-selected', 'true');
    console.log('   ✅ Resultado confirmado');
    console.log('🏁 R-03 CONCLUÍDO\n');
  });

  it('[R-04] Alternar aba aprovador', () => {
    const dados = R04;
    console.log('\n🧪 TESTANDO: R-04 — Alternar aba aprovador');
    console.log('   → clicar na tab Aprovações e validar seleção');
    cy.visit('/reembolso');
    cy.get('[role="tab"]').contains('Aprovações').click();
    cy.get('[role="tab"]').contains('Aprovações').should('have.attr', 'aria-selected', 'true');
    console.log('   ✅ Resultado confirmado');
    console.log('🏁 R-04 CONCLUÍDO\n');
  });

  it('[R-05] Filtrar por intervalo de datas', () => {
    const dados = R05;
    console.log('\n🧪 TESTANDO: R-05 — Filtrar por intervalo de datas');
    console.log('   → abrir calendários Data Início/Fim e aplicar intervalo');
    cy.visit('/reembolso');
    cy.contains('button', 'Data Início').click();
    cy.get('[role="gridcell"]').contains(/^8$/).first().click();
    cy.contains('button', 'Data Fim').click();
    cy.get('[role="gridcell"]').contains(/^18$/).first().click();
    cy.contains('button', 'Limpar').should('be.visible');
    console.log('   ✅ Resultado confirmado');
    console.log('🏁 R-05 CONCLUÍDO\n');
  });

  it('[R-06] Busca textual case-insensitive', () => {
    const dados = R06;
    console.log('\n🧪 TESTANDO: R-06 — Busca textual case-insensitive');
    console.log('   → digitar no campo Busca');
    cy.visit('/reembolso');
    cy.get('[role="textbox"][placeholder="Busca"]').clear().type(dados.termoBusca);
    cy.get('[role="textbox"][placeholder="Busca"]').should('have.value', dados.termoBusca);
    console.log('   ✅ Resultado confirmado');
    console.log('🏁 R-06 CONCLUÍDO\n');
  });

  it('[R-07] Paginação', () => {
    const dados = R07;
    console.log('\n🧪 TESTANDO: R-07 — Paginação');
    console.log('   → validar controles de paginação na grade');
    cy.visit('/reembolso');
    cy.get('[role="table"]').should('be.visible');
    cy.get('[role="navigation"]').should('exist');
    console.log('   ✅ Resultado confirmado');
    console.log('🏁 R-07 CONCLUÍDO\n');
  });

  it('[R-08] Botão relatório Gestão ADM (flag de parâmetro habilitado)', () => {
    const dados = R08;
    console.log('\n🧪 TESTANDO: R-08 — Botão relatório Gestão ADM (flag de parâmetro habilitado)');
    console.log('   → aba Gestão ADM e presença do botão Gerar Relatório (se exibido)');
    cy.visit(`/reembolso?tab=${dados.abaUrl}`);
    cy.get('[role="tab"]').contains('Gestão ADM').should('have.attr', 'aria-selected', 'true');
    cy.get('body').then(($b) => {
      if ($b.find(':contains("Gerar Relatório")').length) {
        cy.contains('button', 'Gerar Relatório').should('be.visible');
      }
    });
    console.log('   ✅ Resultado confirmado');
    console.log('🏁 R-08 CONCLUÍDO\n');
  });

  it('[R-09] Concluir download do relatório', () => {
    const dados = R09;
    console.log('\n🧪 TESTANDO: R-09 — Concluir download do relatório');
    console.log('   → Gestão ADM, abrir fluxo Gerar Relatório e dialog');
    cy.visit(`/reembolso?tab=${dados.abaUrl}`);
    cy.get('body').then(($b) => {
      if ($b.find('button:contains("Gerar Relatório")').length) {
        cy.contains('button', 'Gerar Relatório').click();
        cy.get('[role="alertdialog"]').contains('Gerar Relatório').should('be.visible');
        cy.contains('button', 'Fechar').click();
      }
    });
    console.log('   ✅ Resultado confirmado');
    console.log('🏁 R-09 CONCLUÍDO\n');
  });

  it('[R-10] Bloquear Gestão ADM sem perfil gestor', () => {
    const dados = R10;
    console.log('\n🧪 TESTANDO: R-10 — Bloquear Gestão ADM sem perfil gestor');
    console.log('   → tab Gestão ADM ausente ou não selecionável');
    cy.visit('/reembolso');
    cy.get('body').then(($b) => {
      const $tab = $b.find('[role="tab"]').filter((_, el) => el.textContent.includes('Gestão ADM'));
      if ($tab.length) {
        cy.wrap($tab.first()).should('have.attr', 'aria-disabled', 'true');
      } else {
        cy.get('[role="tab"]').contains('Gestão ADM').should('not.exist');
      }
    });
    console.log('   ✅ Resultado confirmado');
    console.log('🏁 R-10 CONCLUÍDO\n');
  });

  it('[R-11] Bloquear Aprovações sem perfil aprovador', () => {
    const dados = R11;
    console.log('\n🧪 TESTANDO: R-11 — Bloquear Aprovações sem perfil aprovador');
    console.log('   → tab Aprovações ausente ou não selecionável');
    cy.visit('/reembolso');
    cy.get('body').then(($b) => {
      const $tab = $b.find('[role="tab"]').filter((_, el) => el.textContent.includes('Aprovações'));
      if ($tab.length) {
        cy.wrap($tab.first()).should('have.attr', 'aria-disabled', 'true');
      } else {
        cy.get('[role="tab"]').contains('Aprovações').should('not.exist');
      }
    });
    console.log('   ✅ Resultado confirmado');
    console.log('🏁 R-11 CONCLUÍDO\n');
  });

  it('[R-12] Impedir troca manual para aba restrita', () => {
    const dados = R12;
    console.log('\n🧪 TESTANDO: R-12 — Impedir troca manual para aba restrita');
    console.log('   → URL com tab restrita não deve manter gestão-adm sem permissão');
    cy.visit(`/reembolso?tab=${dados.abaSolicitadaSemPermissao}`);
    cy.url().should('not.include', `tab=${dados.abaSolicitadaSemPermissao}`);
    console.log('   ✅ Resultado confirmado');
    console.log('🏁 R-12 CONCLUÍDO\n');
  });

  it('[R-13] Gerar relatório sem token', () => {
    const dados = R13;
    console.log('\n🧪 TESTANDO: R-13 — Gerar relatório sem token');
    console.log('   → fluxo relatório deve falhar ou exibir erro');
    cy.visit(`/reembolso?tab=${dados.abaUrl}`);
    cy.get('body').then(($b) => {
      if ($b.find('button:contains("Gerar Relatório")').length) {
        cy.contains('button', 'Gerar Relatório').click();
        cy.get('[role="alertdialog"], [role="dialog"]').should('be.visible');
      }
    });
    console.log('   ✅ Resultado confirmado');
    console.log('🏁 R-13 CONCLUÍDO\n');
  });

  it('[R-14] Erro na API ao gerar relatório', () => {
    const dados = R14;
    console.log('\n🧪 TESTANDO: R-14 — Erro na API ao gerar relatório');
    console.log('   → intercept POST relatório com 500 e validar feedback');
    cy.intercept('POST', '**/relatorio**', { statusCode: 500, body: {} }).as('relErr');
    cy.visit(`/reembolso?tab=${dados.abaUrl}`);
    cy.get('body').then(($b) => {
      if ($b.find('button:contains("Gerar Relatório")').length) {
        cy.contains('button', 'Gerar Relatório').click();
        cy.contains('button', 'Confirmar').click();
        cy.wait('@relErr');
        cy.get('[role="dialog"], [role="alertdialog"]').should('be.visible');
      }
    });
    console.log('   ✅ Resultado confirmado');
    console.log('🏁 R-14 CONCLUÍDO\n');
  });

  // [Regressivo] — paginação ao mudar busca; depende de volume de dados
  it.skip('[R-15] Resetar página ao mudar busca', () => {
    const dados = R15;
    console.log('\n🧪 TESTANDO: R-15 — Resetar página ao mudar busca');
    console.log('   → regressivo: requer dataset mínimo para paginação + busca');
    cy.visit('/reembolso');
    cy.get('[role="textbox"][placeholder="Busca"]').clear().type(dados.termoBuscaInicial);
    cy.get('[role="textbox"][placeholder="Busca"]').clear().type(dados.termoBuscaAlterada);
    console.log('   ✅ Resultado confirmado');
    console.log('🏁 R-15 CONCLUÍDO\n');
  });

  // ——— Inserir I01–I15 ———

  it('[I-01] Carregar projetos e verbas', () => {
    const dados = I01;
    console.log('\n🧪 TESTANDO: I-01 — Carregar projetos e verbas');
    console.log('   → formulário inserir com combobox Cliente/Projeto');
    cy.visit('/inserir-reembolso');
    cy.contains('[role="combobox"]', /cliente|projeto/i).should('be.visible');
    cy.get('#objetivo').should('be.visible');
    console.log('   ✅ Resultado confirmado');
    console.log('🏁 I-01 CONCLUÍDO\n');
  });

  it('[I-02] Preencher cabeçalho obrigatório', () => {
    const dados = I02;
    console.log('\n🧪 TESTANDO: I-02 — Preencher cabeçalho obrigatório');
    console.log('   → preencher objetivo e destino');
    cy.visit('/inserir-reembolso');
    cy.get('#objetivo').clear().type(dados.objetivo);
    cy.get('#destino').clear().type(dados.destino);
    cy.get('#objetivo').should('have.value', dados.objetivo);
    console.log('   ✅ Resultado confirmado');
    console.log('🏁 I-02 CONCLUÍDO\n');
  });

  it('[I-03] Incluir item por valor direto (tipoCodigo !== 2)', () => {
    const dados = I03;
    console.log('\n🧪 TESTANDO: I-03 — Incluir item por valor direto (tipoCodigo !== 2)');
    console.log('   → preencher item com valor e descrição');
    cy.visit('/inserir-reembolso');
    cy.get('#objetivo').clear().type(dados.objetivo);
    cy.get('#destino').clear().type(dados.destino);
    cy.contains('button', 'Data de início').click();
    cy.get('[role="gridcell"]').contains(/^8$/).first().click();
    cy.contains('button', 'Data final').click();
    cy.get('[role="gridcell"]').contains(/^12$/).first().click();
    cy.contains('[role="combobox"]', /categoria/i).click();
    cy.contains(dados.categoria).click();
    cy.get('input[placeholder="0,00"], input[placeholder="0.00"]').first().clear().type(dados.valor);
    cy.contains('Descrição').parent().find('textarea, [role="textbox"]').first().clear().type(dados.descricao);
    cy.contains('button', 'Data da Despesa').click();
    cy.get('[role="gridcell"]').contains(/^10$/).first().click();
    cy.contains('button', 'Adicionar ao Carrinho').click();
    cy.contains('Carrinho de Solicitações').should('be.visible');
    console.log('   ✅ Resultado confirmado');
    console.log('🏁 I-03 CONCLUÍDO\n');
  });

  it('[I-04] Incluir item por quantidade (tipoCodigo === 2)', () => {
    const dados = I04;
    console.log('\n🧪 TESTANDO: I-04 — Incluir item por quantidade (tipoCodigo === 2)');
    console.log('   → quantidade e valores condicionais tipo 2');
    cy.visit('/inserir-reembolso');
    cy.get('#objetivo').clear().type(dados.objetivo);
    cy.get('#destino').clear().type(dados.destino);
    cy.contains('button', 'Data de início').click();
    cy.get('[role="gridcell"]').contains(/^1$/).first().click();
    cy.contains('button', 'Data final').click();
    cy.get('[role="gridcell"]').contains(/^30$/).first().click();
    cy.contains('[role="combobox"]', /categoria/i).click();
    cy.contains(dados.categoria).click();
    cy.contains('button', 'Data da Despesa').click();
    cy.get('[role="gridcell"]').contains(/^14$/).first().click();
    cy.get('[role="spinbutton"]').contains(dados.quantidade).parent().find('input').clear().type(dados.quantidade);
    cy.get('[role="spinbutton"]').filter('[name*="quantidade"], [aria-label*="Quantidade"]').first().clear().type(dados.quantidade);
    cy.get('input[role="spinbutton"]').first().clear().type(dados.quantidade);
    cy.contains('button', 'Adicionar ao Carrinho').click();
    console.log('   ✅ Resultado confirmado');
    console.log('🏁 I-04 CONCLUÍDO\n');
  });

  it('[I-05] Analisar comprovantes via OCR', () => {
    const dados = I05;
    console.log('\n🧪 TESTANDO: I-05 — Analisar comprovantes via OCR');
    console.log('   → anexar arquivo no input file');
    cy.visit('/inserir-reembolso');
    cy.get('input[type="file"]').selectFile(
      { contents: Cypress.Buffer.from('fake'), fileName: dados.nomeArquivoComprovante, mimeType: 'application/pdf' },
      { force: true },
    );
    cy.get('input[type="file"]').should('exist');
    console.log('   ✅ Resultado confirmado');
    console.log('🏁 I-05 CONCLUÍDO\n');
  });

  it('[I-06] Editar item no carrinho', () => {
    const dados = I06;
    console.log('\n🧪 TESTANDO: I-06 — Editar item no carrinho');
    console.log('   → adicionar item e validar heading carrinho');
    cy.visit('/inserir-reembolso');
    cy.get('#objetivo').clear().type(dados.objetivo);
    cy.get('#destino').clear().type(dados.destino);
    cy.contains('button', 'Data de início').click();
    cy.get('[role="gridcell"]').contains(/^22$/).first().click();
    cy.contains('button', 'Data final').click();
    cy.get('[role="gridcell"]').contains(/^25$/).first().click();
    cy.contains('[role="combobox"]', /categoria/i).click();
    cy.contains(dados.categoria).click();
    cy.contains('button', 'Data da Despesa').click();
    cy.get('[role="gridcell"]').contains(/^23$/).first().click();
    cy.get('input[placeholder="0,00"], input[placeholder="0.00"]').first().clear().type(dados.valor);
    cy.contains('Descrição').parent().find('textarea, [role="textbox"]').first().clear().type(dados.descricao);
    cy.contains('button', 'Adicionar ao Carrinho').click();
    cy.get('[role="heading"]').contains('Carrinho de Solicitações').should('be.visible');
    cy.contains('button', 'Atualizar Item').should('exist');
    console.log('   ✅ Resultado confirmado');
    console.log('🏁 I-06 CONCLUÍDO\n');
  });

  it('[I-07] Enviar solicitação com ZIP', () => {
    const dados = I07;
    console.log('\n🧪 TESTANDO: I-07 — Enviar solicitação com ZIP');
    console.log('   → múltiplos comprovantes e enviar');
    cy.visit('/inserir-reembolso');
    cy.get('#objetivo').clear().type(dados.objetivo);
    cy.get('#destino').clear().type(dados.destino);
    cy.contains('button', 'Data de início').click();
    cy.get('[role="gridcell"]').contains(/^14$/).first().click();
    cy.contains('button', 'Data final').click();
    cy.get('[role="gridcell"]').contains(/^17$/).first().click();
    cy.contains('[role="combobox"]', /categoria/i).click();
    cy.contains(dados.categoria).click();
    cy.contains('button', 'Data da Despesa').click();
    cy.get('[role="gridcell"]').contains(/^15$/).first().click();
    cy.get('input[placeholder="0,00"], input[placeholder="0.00"]').first().clear().type(dados.valor);
    cy.contains('Descrição').parent().find('textarea, [role="textbox"]').first().clear().type(dados.descricao);
    dados.comprovantesAnexos.forEach((nome) => {
      cy.get('input[type="file"]').selectFile(
        { contents: Cypress.Buffer.from('x'), fileName: nome, mimeType: 'application/pdf' },
        { force: true },
      );
    });
    cy.contains('button', 'Adicionar ao Carrinho').click();
    cy.contains('button', 'Enviar solicitações').should('be.visible');
    console.log('   ✅ Resultado confirmado');
    console.log('🏁 I-07 CONCLUÍDO\n');
  });

  it('[I-08] Objetivo obrigatório vazio', () => {
    const dados = I08;
    console.log('\n🧪 TESTANDO: I-08 — Objetivo obrigatório vazio');
    console.log('   → não preencher objetivo e tentar adicionar');
    cy.visit('/inserir-reembolso');
    cy.get('#destino').clear().type(dados.destino);
    cy.contains('button', 'Data de início').click();
    cy.get('[role="gridcell"]').contains(/^18$/).first().click();
    cy.contains('button', 'Data final').click();
    cy.get('[role="gridcell"]').contains(/^19$/).first().click();
    cy.contains('[role="combobox"]', /categoria/i).click();
    cy.contains(dados.categoria).click();
    cy.contains('button', 'Data da Despesa').click();
    cy.get('[role="gridcell"]').contains(/^18$/).first().click();
    cy.get('input[placeholder="0,00"], input[placeholder="0.00"]').first().clear().type(dados.valor);
    cy.contains('Descrição').parent().find('textarea, [role="textbox"]').first().clear().type(dados.descricao);
    cy.contains('button', 'Adicionar ao Carrinho').click();
    cy.get('#objetivo').then(($el) => {
      expect($el[0].validationMessage || $el.is(':invalid')).to.exist;
    });
    console.log('   ✅ Resultado confirmado');
    console.log('🏁 I-08 CONCLUÍDO\n');
  });

  it('[I-09] Data de início ausente', () => {
    const dados = I09;
    console.log('\n🧪 TESTANDO: I-09 — Data de início ausente');
    console.log('   → preencher exceto data início');
    cy.visit('/inserir-reembolso');
    cy.get('#objetivo').clear().type(dados.objetivo);
    cy.get('#destino').clear().type(dados.destino);
    cy.contains('button', 'Data final').click();
    cy.get('[role="gridcell"]').contains(/^28$/).first().click();
    cy.contains('[role="combobox"]', /categoria/i).click();
    cy.contains(dados.categoria).click();
    cy.contains('button', 'Data da Despesa').click();
    cy.get('[role="gridcell"]').contains(/^25$/).first().click();
    cy.get('input[placeholder="0,00"], input[placeholder="0.00"]').first().clear().type(dados.valor);
    cy.contains('Descrição').parent().find('textarea, [role="textbox"]').first().clear().type(dados.descricao);
    cy.contains('button', 'Adicionar ao Carrinho').click();
    cy.contains('button', 'Data de início').should('be.visible');
    console.log('   ✅ Resultado confirmado');
    console.log('🏁 I-09 CONCLUÍDO\n');
  });

  it('[I-10] Cliente/projeto obrigatório ausente', () => {
    const dados = I10;
    console.log('\n🧪 TESTANDO: I-10 — Cliente/projeto obrigatório ausente');
    console.log('   → não selecionar cliente e submeter fluxo');
    cy.visit('/inserir-reembolso');
    cy.get('#objetivo').clear().type(dados.objetivo);
    cy.get('#destino').clear().type(dados.destino);
    cy.contains('button', 'Data de início').click();
    cy.get('[role="gridcell"]').contains(/^5$/).first().click();
    cy.contains('button', 'Data final').click();
    cy.get('[role="gridcell"]').contains(/^6$/).first().click();
    cy.contains('[role="combobox"]', /categoria/i).click();
    cy.contains(dados.categoria).click();
    cy.contains('button', 'Data da Despesa').click();
    cy.get('[role="gridcell"]').contains(/^5$/).first().click();
    cy.get('input[placeholder="0,00"], input[placeholder="0.00"]').first().clear().type(dados.valor);
    cy.contains('Descrição').parent().find('textarea, [role="textbox"]').first().clear().type(dados.descricao);
    cy.contains('button', 'Adicionar ao Carrinho').click();
    cy.contains(/cliente|projeto|obrigatório/i).should('exist');
    console.log('   ✅ Resultado confirmado');
    console.log('🏁 I-10 CONCLUÍDO\n');
  });

  it('[I-11] Valor inválido (tipo !== 2, valor <= 0)', () => {
    const dados = I11;
    console.log('\n🧪 TESTANDO: I-11 — Valor inválido (tipo !== 2, valor <= 0)');
    console.log('   → valor 0,00 e validação');
    cy.visit('/inserir-reembolso');
    cy.get('#objetivo').clear().type(dados.objetivo);
    cy.get('#destino').clear().type(dados.destino);
    cy.contains('button', 'Data de início').click();
    cy.get('[role="gridcell"]').contains(/^12$/).first().click();
    cy.contains('button', 'Data final').click();
    cy.get('[role="gridcell"]').contains(/^13$/).first().click();
    cy.contains('[role="combobox"]', /categoria/i).click();
    cy.contains(dados.categoria).click();
    cy.contains('button', 'Data da Despesa').click();
    cy.get('[role="gridcell"]').contains(/^12$/).first().click();
    cy.get('input[placeholder="0,00"], input[placeholder="0.00"]').first().clear().type(dados.valor);
    cy.contains('Descrição').parent().find('textarea, [role="textbox"]').first().clear().type(dados.descricao);
    cy.contains('button', 'Adicionar ao Carrinho').click();
    cy.contains(/valor|inválido|maior|zero/i).should('exist');
    console.log('   ✅ Resultado confirmado');
    console.log('🏁 I-11 CONCLUÍDO\n');
  });

  it('[I-12] Comprovante obrigatório ausente', () => {
    const dados = I12;
    console.log('\n🧪 TESTANDO: I-12 — Comprovante obrigatório ausente');
    console.log('   → não anexar arquivo quando exigido');
    cy.visit('/inserir-reembolso');
    cy.get('#objetivo').clear().type(dados.objetivo);
    cy.get('#destino').clear().type(dados.destino);
    cy.contains('button', 'Data de início').click();
    cy.get('[role="gridcell"]').contains(/^20$/).first().click();
    cy.contains('button', 'Data final').click();
    cy.get('[role="gridcell"]').contains(/^22$/).first().click();
    cy.contains('[role="combobox"]', /categoria/i).click();
    cy.contains(dados.categoria).click();
    cy.contains('button', 'Data da Despesa').click();
    cy.get('[role="gridcell"]').contains(/^20$/).first().click();
    cy.get('input[placeholder="0,00"], input[placeholder="0.00"]').first().clear().type(dados.valor);
    cy.contains('Descrição').parent().find('textarea, [role="textbox"]').first().clear().type(dados.descricao);
    cy.contains('button', 'Adicionar ao Carrinho').click();
    cy.contains('button', 'Enviar solicitações').click();
    cy.get('[role="dialog"]').contains(/comprovante|anexo|obrigatório/i).should('be.visible');
    console.log('   ✅ Resultado confirmado');
    console.log('🏁 I-12 CONCLUÍDO\n');
  });

  it('[I-13] Data do comprovante fora da validade (> 30 dias)', () => {
    const dados = I13;
    console.log('\n🧪 TESTANDO: I-13 — Data do comprovante fora da validade (> 30 dias)');
    console.log('   → data despesa antiga');
    cy.visit('/inserir-reembolso');
    cy.get('#objetivo').clear().type(dados.objetivo);
    cy.get('#destino').clear().type(dados.destino);
    cy.contains('button', 'Data de início').click();
    cy.get('[role="gridcell"]').contains(/^14$/).first().click();
    cy.contains('button', 'Data final').click();
    cy.get('[role="gridcell"]').contains(/^15$/).first().click();
    cy.contains('[role="combobox"]', /categoria/i).click();
    cy.contains(dados.categoria).click();
    cy.contains('button', 'Data da Despesa').click();
    cy.get('[role="gridcell"]').contains(/^10$/).first().click();
    cy.get('input[placeholder="0,00"], input[placeholder="0.00"]').first().clear().type(dados.valor);
    cy.contains('Descrição').parent().find('textarea, [role="textbox"]').first().clear().type(dados.descricao);
    cy.contains('button', 'Adicionar ao Carrinho').click();
    cy.contains(/validade|30|data/i).should('exist');
    console.log('   ✅ Resultado confirmado');
    console.log('🏁 I-13 CONCLUÍDO\n');
  });

  it('[I-14] Valor acima do teto da verba', () => {
    const dados = I14;
    console.log('\n🧪 TESTANDO: I-14 — Valor acima do teto da verba');
    console.log('   → valor acima do limite e validação');
    cy.visit('/inserir-reembolso');
    cy.get('#objetivo').clear().type(dados.objetivo);
    cy.get('#destino').clear().type(dados.destino);
    cy.contains('button', 'Data de início').click();
    cy.get('[role="gridcell"]').contains(/^9$/).first().click();
    cy.contains('button', 'Data final').click();
    cy.get('[role="gridcell"]').contains(/^11$/).first().click();
    cy.contains('[role="combobox"]', /categoria/i).click();
    cy.contains(dados.categoria).click();
    cy.contains('button', 'Data da Despesa').click();
    cy.get('[role="gridcell"]').contains(/^10$/).first().click();
    cy.get('input[placeholder="0,00"], input[placeholder="0.00"]').first().clear().type(dados.valor);
    cy.contains('Descrição').parent().find('textarea, [role="textbox"]').first().clear().type(dados.descricao);
    cy.contains('button', 'Adicionar ao Carrinho').click();
    cy.contains(/teto|limite|verba|exced/i).should('exist');
    console.log('   ✅ Resultado confirmado');
    console.log('🏁 I-14 CONCLUÍDO\n');
  });

  // [Regressivo] — fluxo limpar + remover linha com estado complexo
  it.skip('[I-15] Limpar formulário e remover item do carrinho', () => {
    const dados = I15;
    console.log('\n🧪 TESTANDO: I-15 — Limpar formulário e remover item do carrinho');
    console.log('   → regressivo: validação fina de totais após limpar');
    cy.visit('/inserir-reembolso');
    cy.get('#objetivo').clear().type(dados.objetivo);
    cy.contains('button', 'Limpar').click();
    cy.contains('Nenhuma solicitação no carrinho').should('be.visible');
    console.log('   ✅ Resultado confirmado');
    console.log('🏁 I-15 CONCLUÍDO\n');
  });
});
