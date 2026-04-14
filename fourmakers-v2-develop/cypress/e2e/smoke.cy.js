describe('Smoke Test — Login OTP FourMakers', () => {
  beforeEach(() => {
    cy.session('fourmakers-prd', () => {
      cy.loginFourMakers();
    });
  });

  it('deve autenticar e não estar na tela de login', () => {
    cy.visit('/');
    cy.url().should('not.include', '/login');
  });

  it('deve acessar a tela de Reembolso', () => {
    cy.visit('/reembolso');
    cy.contains('Reembolso').should('be.visible');
  });
});
