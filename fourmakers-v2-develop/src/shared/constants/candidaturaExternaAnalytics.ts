/**
 * Constantes para analytics (Firebase logUserAction) da jornada de candidatura externa.
 * flowIdentifier e featureName seguem o padrão do projeto (PascalCase) para agregação em relatórios.
 */

/** Identificador do fluxo para candidatura externa (página /public/vaga). */
export const CANDIDATURA_EXTERNA_FLOW = 'CandidaturaExterna'

/** Nomes de features/ações para logUserAction (PascalCase). */
export const CANDIDATURA_EXTERNA_ACTIONS = {
  /** Usuário solicitou código de login por e-mail. */
  SolicitarCodigoLogin: 'SolicitarCodigoLogin',
  /** Login concluído com sucesso (OTP validado). */
  Entrar: 'Entrar',
  /** Cadastro de novo usuário concluído com sucesso. */
  Cadastrar: 'Cadastrar',
  /** Usuário visualizou o passo "Dados para vaga" (formulário de inscrição). */
  VisualizarStepDadosVaga: 'VisualizarStepDadosVaga',
  /** Inscrição na vaga realizada com sucesso. */
  RealizarInscricao: 'RealizarInscricao',
  /** Erro ao realizar inscrição. */
  RealizarInscricaoErro: 'RealizarInscricaoErro',
  /** Skill incluída no perfil 360 (checkbox marcado). */
  IncluirSkillPerfil: 'IncluirSkillPerfil',
  /** Skill removida do perfil 360 (checkbox desmarcado). */
  RemoverSkillPerfil: 'RemoverSkillPerfil',
  /** Nível de skill alterado no perfil 360. */
  AtualizarNivelSkillPerfil: 'AtualizarNivelSkillPerfil',
} as const
