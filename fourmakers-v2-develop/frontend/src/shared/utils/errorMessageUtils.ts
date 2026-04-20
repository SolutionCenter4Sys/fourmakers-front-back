/**
 * Mensagens amigáveis ao usuário para erros de rede e exceções.
 * Alinhado à documentação do projeto (mensagens claras, sem jargões técnicos).
 */

const MENSAGEM_SEM_CONEXAO =
  'Sem conexão com a internet. Verifique sua rede e tente novamente.';

/** Indica se o erro é típico de falha de rede (offline, timeout, recusa de conexão). */
function isErroDeRede(error: unknown): boolean {
  if (!(error instanceof Error)) return false;
  const msg = error.message.toLowerCase();
  const name = (error.name || '').toLowerCase();
  return (
    msg.includes('failed to fetch') ||
    msg.includes('network error') ||
    msg.includes('networkerror') ||
    msg.includes('load failed') ||
    msg.includes('connection refused') ||
    msg.includes('net::err_') ||
    (name === 'typeerror' && msg.includes('fetch'))
  );
}

/**
 * Retorna uma mensagem amigável para exibir ao usuário a partir de um erro.
 * Erros de rede (falta de conexão, offline) são mapeados para mensagem padrão;
 * demais erros usam a mensagem do erro se for válida, ou um fallback genérico.
 */
export function getMensagemAmigavelErro(
  error: unknown,
  fallback: string = 'Algo deu errado. Tente novamente.'
): string {
  if (!(error instanceof Error)) return fallback;
  if (isErroDeRede(error)) return MENSAGEM_SEM_CONEXAO;
  const msg = error.message?.trim();
  return msg && msg.length > 0 ? msg : fallback;
}
