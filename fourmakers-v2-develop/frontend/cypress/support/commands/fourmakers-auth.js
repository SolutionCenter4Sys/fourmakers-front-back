/**
 * FourMakers — Login via OTP (Produção)
 *
 * Fluxo (3 etapas obrigatórias e sequenciais):
 *   1. EnviaTokenAcessoEmail   → dispara o OTP para o e-mail cadastrado
 *   2. ObtemCodigoAcessoEmailQA → recupera o código via endpoint QA (com polling)
 *   3. ValidaTokenAcessoEmail  → troca o código pelo JWT de sessão
 *
 * Após obter o JWT, ele é injetado no localStorage da aplicação
 * sob a chave "authToken" (padrão FourMakers).
 *
 * Uso no teste:
 *   beforeEach(() => { cy.loginFourMakers(); });
 *
 * Uso com cache de sessão (recomendado para suítes grandes):
 *   beforeEach(() => {
 *     cy.session('fourmakers', () => { cy.loginFourMakers(); });
 *   });
 */

const FM = {
  apiBase: 'https://api.dev.fourmakers.io',
  email:   'solutioncenter@foursys.com.br',
  orgId:   8,
  token:   'Mnx5Y0R3dzNtd3hqaFFuSVVVekVVUHhNdDl2OVowVHNKMnlnclJMOFhWRlB4aGRnT0IxcQ==',
};

/**
 * Polling para ObtemCodigoAcessoEmailQA.
 * Tenta até 12 vezes com intervalo de 400 ms (~5 s no total).
 */
function obtemCodigoComPolling(tentativas = 12) {
  return cy
    .request({
      method:           'GET',
      url:              `${FM.apiBase}/api/Acesso/ObtemCodigoAcessoEmailQA`,
      qs:               { email: FM.email, orgId: FM.orgId },
      headers:          { Authorization: `Bearer ${FM.token}` },
      failOnStatusCode: false,
    })
    .then((res) => {
      const body   = res.body;
      const codigo = body.codigo ?? body.codigoAcesso ?? body.token ?? body.code ?? '';

      if (res.status === 200 && body.sucesso && codigo) {
        return cy.wrap(codigo);
      }

      if (tentativas <= 0) {
        throw new Error(
          `[FourMakers] ObtemCodigoAcessoEmailQA: código não encontrado após 5 s.\n` +
          `Última resposta: ${JSON.stringify(body)}`
        );
      }

      return cy.wait(400).then(() => obtemCodigoComPolling(tentativas - 1));
    });
}

/**
 * Comando principal.
 * @param {string} [appUrl='https://app.fourmakers.io'] URL da aplicação onde o
 *   JWT será injetado.
 */
Cypress.Commands.add('loginFourMakers', (appUrl = 'https://app.dev.fourmakers.io') => {
  cy.request({
    method:  'POST',
    url:     `${FM.apiBase}/api/Acesso/EnviaTokenAcessoEmail`,
    body:    { email: FM.email, orgId: FM.orgId },
    headers: { 'Content-Type': 'application/json' },
  }).then((res) => {
    expect(res.body.sucesso,    '[Step 1] sucesso deve ser true').to.eq(true);
    expect(res.body.tipoAcesso, '[Step 1] organização não pode usar SSO').to.not.eq('SSO');
  });

  obtemCodigoComPolling().then((codigo) => {
    cy.request({
      method:  'POST',
      url:     `${FM.apiBase}/api/Acesso/ValidaTokenAcessoEmail`,
      body:    { email: FM.email, token: codigo, orgId: FM.orgId },
      headers: { 'Content-Type': 'application/json' },
    }).then((res) => {
      expect(res.body.sucesso, '[Step 3] autenticação deve retornar sucesso=true').to.eq(true);
      expect(res.body.token,   '[Step 3] JWT deve estar presente').to.be.a('string');

      const jwt = res.body.token;

      cy.visit(appUrl, { failOnStatusCode: false });
      cy.window().then((win) => {
        win.localStorage.setItem('authToken', jwt);
      });
      cy.reload();
      cy.url().should('not.include', '/login');
    });
  });
});
