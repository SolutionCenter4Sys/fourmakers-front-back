/**
 * Ambiente FIXO da Demo Reembolso — fonte da verdade.
 * A demo NÃO roda em "dev" genérico nem em PRD: sempre HML (backoffice-rf-hom).
 */
const DEMO_AMBIENTE = Object.freeze({
  nome: 'homolog',
  label: 'HML / homolog',
  backend: 'https://spw.app.foursys.com/backoffice-rf-hom',
  frontLocal: 'http://localhost:8080',
  email: 'usuario_qa@foursys.com.br',
  orgId: 5,
  orgNome: 'Showcase',
  flutterflow: 'https://wf.dev.fourmakers.io',
});

/**
 * Normaliza credenciais para o ambiente HML fixo da demo.
 * Sobrescreve ambiente/backend/proxy mesmo se o JSON local disser "dev".
 */
function forceAmbienteHml(cred = {}) {
  const envLocal = { ...(cred.envLocal || {}) };
  envLocal.VITE_API_PROXY_TARGET = DEMO_AMBIENTE.backend;
  if (!String(envLocal.VITE_FLUTTERFLOW_BASE_URL || '').trim()) {
    envLocal.VITE_FLUTTERFLOW_BASE_URL = DEMO_AMBIENTE.flutterflow;
  }
  if (!('VITE_API_FOURMAKERS_URL' in envLocal)) {
    envLocal.VITE_API_FOURMAKERS_URL = '';
  }

  return {
    ...cred,
    ambiente: DEMO_AMBIENTE.nome,
    backend: DEMO_AMBIENTE.backend,
    frontLocal: cred.frontLocal || DEMO_AMBIENTE.frontLocal,
    email: cred.email || DEMO_AMBIENTE.email,
    orgId: cred.orgId != null ? Number(cred.orgId) : DEMO_AMBIENTE.orgId,
    orgNome: cred.orgNome || DEMO_AMBIENTE.orgNome,
    envLocal,
  };
}

function describeAmbiente() {
  return `${DEMO_AMBIENTE.label} · ${DEMO_AMBIENTE.backend}`;
}

module.exports = {
  DEMO_AMBIENTE,
  forceAmbienteHml,
  describeAmbiente,
};
