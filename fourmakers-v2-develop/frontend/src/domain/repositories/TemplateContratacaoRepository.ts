import type {
  DiretorioContratacaoItem,
  EquipamentosPadroesAninhadosGrupo,
  GrupoEmailContratacaoItem,
  ObterTemplateContratacaoResult,
  SistemaLiberadoContratacaoItem,
} from '@domain/entities/TemplateContratacao'

export interface TemplateContratacaoRepository {
  obterPorCandidatura(
    token: string,
    idCandidatura: string
  ): Promise<ObterTemplateContratacaoResult>

  /** Salva o template usando CriarTemplate ou AtualizarTemplate conforme id; retorna o retorno da API. */
  salvarTemplate(
    token: string,
    idCandidatura: string,
    payload: Record<string, unknown>
  ): Promise<{ retorno?: unknown; sucesso?: boolean; mensagem?: string | null }>

  listarEquipamentosPadroesAninhados(token: string): Promise<EquipamentosPadroesAninhadosGrupo[]>

  listarDiretorios(token: string): Promise<DiretorioContratacaoItem[]>

  listarSistemasLiberados(token: string): Promise<SistemaLiberadoContratacaoItem[]>

  listarGruposEmails(token: string): Promise<GrupoEmailContratacaoItem[]>

  /** Envia e-mail do template de contratação (API EnviarEmailTemplateCandidato; anexo é definido pela presença de file). */
  enviarEmailTemplateCandidato(
    token: string,
    params: { idCandidatura: string; emailsAdicionais: string[]; ocultarValores: boolean },
    file?: File
  ): Promise<{ sucesso?: boolean; mensagem?: string | null }>
}
