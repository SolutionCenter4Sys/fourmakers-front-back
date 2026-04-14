import { inject, injectable } from 'tsyringe'
import type { TemplateContratacaoRepository } from '@domain/repositories/TemplateContratacaoRepository'
import type {
  CandidaturaDetalhesVaga,
  DiretorioContratacaoItem,
  EquipamentosPadroesAninhadosGrupo,
  GrupoEmailContratacaoItem,
  ObterTemplateContratacaoResult,
  SistemaLiberadoContratacaoItem,
  TemplateContratacaoData,
} from '@domain/entities/TemplateContratacao'
import { GRUPO_EMAIL_CONTRATO_DEFAULT } from '@domain/entities/TemplateContratacao'
import { DiTokens } from '@core/di/tokens'
import type { VagaApi, EnviarEmailTemplateCandidatoParam } from '@data/api/VagaApi'
import type { CandidaturaApi } from '@data/api/CandidaturaApi'
import type { ObterCandidaturaPorIdRetorno } from '@data/api/CandidaturaApi'

@injectable()
export class TemplateContratacaoRepositoryImpl implements TemplateContratacaoRepository {
  constructor(
    @inject(DiTokens.vagaApi) private readonly vagaApi: VagaApi,
    @inject(DiTokens.candidaturaApi) private readonly candidaturaApi: CandidaturaApi
  ) {}
  private static readonly TEMPLATE_ID_CRIACAO = '00000000-0000-0000-0000-000000000000'

  /**
   * Mapeia o retorno da API ObterCandidaturaPorId para CandidaturaDetalhesVaga.
   * Os valores exibidos na tela vêm sempre do retorno da API em cada resposta (não de exemplos fixos).
   */
  private mapearCandidaturaParaDetalhesVaga(raw: ObterCandidaturaPorIdRetorno): CandidaturaDetalhesVaga {
    return {
      ...raw,
      status: raw.status ?? raw.statusDescricao ?? null,
      dataAbertura: raw.dataAbertura ?? raw.candidatura ?? null,
      tituloVaga: raw.tituloVaga ?? null,
      descricaoBreveVaga: raw.descricaoBreveVaga ?? raw.tituloVaga ?? null,
      modeloTrabalhoId: raw.modeloTrabalhoId ?? null,
      codigoVaga: raw.codigoVaga ?? raw.idVaga ?? null,
      nomeCliente: raw.nomeCliente ?? null,
      requisitante: raw.requisitante ?? null,
      tipoContratacao: raw.tipoContratacao ?? null,
      tipoVaga: raw.tipoVaga ?? null,
      requisicao: raw.requisicao ?? null,
      colaboradorResponsavel: raw.colaboradorResponsavel ?? null,
      pretencaoSalarial: raw.pretencaoSalarial ?? null,
      quantidadeDiasPresencial: raw.quantidadeDiasPresencial ?? null,
      ultimaAlteracao: raw.ultimaAlteracao ?? null,
      observacoesInternas: raw.observacoesInternas ?? null,
    }
  }

  /**
   * Extrai DDD (2 dígitos) e telefone (restante) de contatoPrincipal para alimentar os campos separados na tela.
   */
  private extrairDddETelefoneDeContatoPrincipal(contatoPrincipal: string | null | undefined): {
    dddColaborador: string | null
    telefone: string | null
  } {
    const digits = (contatoPrincipal ?? '').replace(/\D/g, '')
    if (digits.length >= 10) {
      return {
        dddColaborador: digits.slice(0, 2),
        telefone: digits.slice(2),
      }
    }
    return { dddColaborador: null, telefone: null }
  }

  async obterPorCandidatura(
    token: string,
    idCandidatura: string
  ): Promise<ObterTemplateContratacaoResult> {
    const candidaturaRes = await this.candidaturaApi.obterCandidaturaPorId(token, idCandidatura);
    const raw = candidaturaRes?.retorno ?? null;
    const candidatura = raw ? this.mapearCandidaturaParaDetalhesVaga(raw) : null;

    let template: TemplateContratacaoData | null = null;
    try {
      const templateRes = await this.vagaApi.obterTemplatePorCandidatura(token, idCandidatura);
      template = (templateRes?.retorno ?? null) as TemplateContratacaoData | null;
    } catch {
      /** API pode retornar 404 quando ainda não existe template (modo criação); segue com template null e candidatura. */
    }

    const { dddColaborador: dddExtraido, telefone: telefoneExtraido } = template
      ? this.extrairDddETelefoneDeContatoPrincipal(template.contatoPrincipal)
      : { dddColaborador: null, telefone: null }

    /** Sistemas liberados: API retorna [{ id, descricao }] ou [{ id, descricao, liberado: true }]; cruza por id com a lista da tela para Check on. */
    const sistemasLiberadosNormalizados =
      template?.sistemasLiberados != null && Array.isArray(template.sistemasLiberados)
        ? (template.sistemasLiberados as Array<{ id?: string; descricao?: string; liberado?: boolean }>)
            .filter((s) => s.liberado !== false)
            .map((s) => ({ id: s.id ?? '', descricao: s.descricao ?? '' }))
        : undefined

    /** Grupos de E-mail: normaliza para [{ id, descricao }, ...] para a UI (checkboxes) e envio no save. */
    const gruposEmailsNormalizados =
      template?.gruposEmails != null && Array.isArray(template.gruposEmails)
        ? (template.gruposEmails as Array<{ id?: string; descricao?: string }>).map((g) => ({
            id: g.id ?? '',
            descricao: g.descricao ?? '',
          }))
        : undefined

    /** Diretórios de Rede: API retorna [{ id, descricao, leitura, escrita }, ...]; normaliza para a UI (check por id, exibir Leitura/Escrita). */
    const diretoriosNormalizados =
      template?.diretorios != null && Array.isArray(template.diretorios)
        ? (template.diretorios as Array<{ id?: string; descricao?: string; leitura?: boolean; escrita?: boolean }>).map(
            (d) => ({
              id: d.id ?? '',
              descricao: d.descricao ?? '',
              leitura: !!d.leitura,
              escrita: !!d.escrita,
            })
          )
        : undefined

    /** Modo criação: quando a API não retorna template (null/404), monta template vazio com id de criação para exibir o formulário. */
    const templateMerged: TemplateContratacaoData | null = template
      ? (() => {
          const codigoVagaMerged = template.codigoVaga ?? candidatura?.codigoVaga;
          const codigoVagaNumber =
            typeof codigoVagaMerged === 'number' ? codigoVagaMerged : (typeof codigoVagaMerged === 'string' ? Number(codigoVagaMerged) : undefined);
          return {
          ...template,
          codigoVaga: codigoVagaNumber ?? undefined,
          /** Tipo da vaga: apenas da API do template (readOnly na tela). */
          descricaoTipoVaga: template.descricaoTipoVaga ?? null,
          /** Cargo: preenchido com tituloVaga da API de dados da vaga (ObterCandidaturaPorId). */
          cargo: candidatura?.tituloVaga ?? template.cargo ?? null,
          /**
           * Modelo de trabalho e dias presenciais: o template salvo tem prioridade sobre a vaga (candidatura).
           * Se o template não trouxer esses campos, usa-se os dados da vaga.
           */
          ...(() => {
            const tplId = template.modeloTrabalhoId;
            const templateTemModelo =
              tplId != null && String(tplId).trim() !== ''
            const diasTpl = template.quantidadeDiasPresencial
            const templateTemDiasPresencial =
              diasTpl != null &&
              typeof diasTpl === 'number' &&
              Number.isFinite(diasTpl) &&
              diasTpl >= 1 &&
              diasTpl <= 5
            return {
              modeloTrabalhoId: templateTemModelo
                ? tplId
                : (candidatura?.modeloTrabalhoId ?? tplId ?? null),
              modeloTrabalhoDescricao: templateTemModelo
                ? (template.modeloTrabalhoDescricao ?? null)
                : null,
              quantidadeDiasPresencial: templateTemDiasPresencial
                ? diasTpl
                : (candidatura?.quantidadeDiasPresencial ?? diasTpl ?? null),
            }
          })(),
          dddColaborador: dddExtraido ?? template.dddColaborador ?? null,
          telefone: telefoneExtraido ?? template.telefone ?? null,
          sistemasLiberados: sistemasLiberadosNormalizados ?? template.sistemasLiberados ?? null,
          gruposEmails: gruposEmailsNormalizados ?? template.gruposEmails ?? null,
          diretorios: diretoriosNormalizados ?? template.diretorios ?? null,
        };
        })()
      : candidatura
        ? ({
            id: TemplateContratacaoRepositoryImpl.TEMPLATE_ID_CRIACAO,
            codigoVaga: typeof candidatura.codigoVaga === 'number' ? candidatura.codigoVaga : (typeof candidatura.codigoVaga === 'string' ? Number(candidatura.codigoVaga) : undefined),
            descricaoTipoVaga: candidatura.tituloVaga ?? null,
            cargo: candidatura.tituloVaga ?? null,
            modeloTrabalhoId: candidatura.modeloTrabalhoId ?? null,
            quantidadeDiasPresencial: candidatura.quantidadeDiasPresencial ?? null,
            tituloVaga: candidatura.tituloVaga ?? null,
            nomeClienteVaga: candidatura.nomeCliente ?? null,
            grupoEmailContrato: GRUPO_EMAIL_CONTRATO_DEFAULT,
          } as TemplateContratacaoData)
        : null;
    return { template: templateMerged, candidatura }
  }

  async salvarTemplate(
    token: string,
    _idCandidatura: string,
    payload: Record<string, unknown>
  ): Promise<{ retorno?: unknown; sucesso?: boolean; mensagem?: string | null }> {
    const idTemplate = ((payload.id as string) ?? '').toString().trim()
    const isCriacao =
      idTemplate === '' || idTemplate === TemplateContratacaoRepositoryImpl.TEMPLATE_ID_CRIACAO
    if (isCriacao) {
      const res = await this.vagaApi.criarTemplate(token, payload)
      return { retorno: res?.retorno, sucesso: res?.sucesso, mensagem: res?.mensagem }
    }
    const res = await this.vagaApi.atualizarTemplate(token, idTemplate, payload)
    return { retorno: res?.retorno, sucesso: res?.sucesso, mensagem: res?.mensagem }
  }

  async listarEquipamentosPadroesAninhados(
    token: string
  ): Promise<EquipamentosPadroesAninhadosGrupo[]> {
    const res = await this.vagaApi.listarEquipamentosPadroesAninhados(token)
    return Array.isArray(res?.retorno) ? (res.retorno as EquipamentosPadroesAninhadosGrupo[]) : []
  }

  async listarDiretorios(token: string): Promise<DiretorioContratacaoItem[]> {
    const res = await this.vagaApi.listarDiretorios(token)
    return Array.isArray(res?.retorno) ? (res.retorno as DiretorioContratacaoItem[]) : []
  }

  async listarSistemasLiberados(token: string): Promise<SistemaLiberadoContratacaoItem[]> {
    const res = await this.vagaApi.listarSistemasLiberados(token)
    return Array.isArray(res?.retorno) ? (res.retorno as SistemaLiberadoContratacaoItem[]) : []
  }

  async listarGruposEmails(token: string): Promise<GrupoEmailContratacaoItem[]> {
    const res = await this.vagaApi.listarGruposEmails(token)
    return Array.isArray(res?.retorno) ? (res.retorno as GrupoEmailContratacaoItem[]) : []
  }

  async enviarEmailTemplateCandidato(
    token: string,
    params: { idCandidatura: string; emailsAdicionais: string[]; ocultarValores: boolean },
    file?: File
  ): Promise<{ sucesso?: boolean; mensagem?: string | null }> {
    const param: EnviarEmailTemplateCandidatoParam = {
      ...params,
      anexo: Boolean(file),
    };
    return this.vagaApi.enviarEmailTemplateCandidato(token, param, file);
  }
}
