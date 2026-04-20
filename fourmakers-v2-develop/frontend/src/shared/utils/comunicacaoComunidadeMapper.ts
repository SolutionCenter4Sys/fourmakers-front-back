import type {
  CommunityGroup,
  GroupSettings,
  ComunicacaoComunidade,
  ComunicacaoComunidadeDetalheRetorno,
} from '@domain/entities/comunicacao';

function resolveCapaUrl(capaUrl: string | null, token: string | undefined): string {
  if (!capaUrl) return '';
  if (!token) return capaUrl;
  const tokenBase64 = btoa(token);
  return capaUrl.replace(/\$1/g, tokenBase64);
}

/**
 * Mapeia um item da API Comunidades para CommunityGroup (domínio).
 */
export function comunidadeApiToCommunityGroup(
  c: ComunicacaoComunidade,
  token?: string,
): CommunityGroup {
  const type = c.tipo === 'privada' ? 'private' : 'free';
  const settings: GroupSettings = {
    allowFreeEntry: type === 'free',
    allowMemberPosts: c.permitePostagemMembro,
    allowMemberLeave: c.permiteSair,
    requiresApproval: c.publicacaoConfiguracaoPolitica !== 'livre',
    commentsEnabledByDefault: c.publicacaoPermiteComentario,
    likesEnabledByDefault: c.publicacaoPermiteLikeHabilitado,
  };
  const moderators =
    c.moderadores?.map((m) => ({
      id: m.codigoInternoColaborador,
      userId: m.codigoInternoColaborador,
      userName: m.nomeCompleto ?? '',
      role: 'moderator' as const,
      joinedAt: '',
    })) ?? [];
  return {
    id: c.id,
    name: c.nome,
    description: c.descricao ?? '',
    coverImage: resolveCapaUrl(c.capaUrl, token) || undefined,
    type,
    creatorId: '',
    creatorName: '',
    moderators,
    status: 'active',
    createdAt: '',
    memberCount: c.totalMembros ?? 0,
    postCount: c.totalPublicacoes ?? 0,
    settings,
    linkedUserGroups: [],
  };
}

/**
 * Mapeia o retorno do detalhe da comunidade (GET .../Comunidade/{id}) para CommunityGroup.
 */
export function comunidadeDetalheToCommunityGroup(
  d: ComunicacaoComunidadeDetalheRetorno,
  token?: string,
): CommunityGroup {
  const base = comunidadeApiToCommunityGroup(
    {
      id: d.id,
      nome: d.nome,
      descricao: d.descricao,
      capaUrl: d.capaUrl,
      tipo: d.tipo,
      permitePostagemMembro: d.permitePostagemMembro,
      permiteSair: d.permiteSair,
      publicacaoConfiguracaoPolitica: d.publicacaoConfiguracaoPolitica,
      publicacaoPermiteComentario: d.publicacaoPermiteComentario,
      publicacaoPermiteLikeHabilitado: d.publicacaoPermiteLikeHabilitado,
      totalPublicacoes: d.totalPublicacoes,
      totalMembros: d.totalMembros,
    },
    token,
  );
  const moderators =
    d.moderadores?.map((m) => ({
      id: m.codigoInternoColaborador,
      userId: m.codigoInternoColaborador,
      userName: m.nomeCompleto ?? '',
      role: 'moderator' as const,
      joinedAt: '',
    })) ?? [];
  return { ...base, moderators };
}

/**
 * Monta FormData no mesmo formato do criar comunidade, a partir do retorno do GET detalhe.
 * Usado para PUT (ex.: arquivar com ativo: false).
 * Quando capaUrl existe, busca a imagem e anexa como capaComunidade (token usado para resolver $1 na URL).
 */
export async function buildComunidadeFormDataFromDetalhe(
  detalhe: ComunicacaoComunidadeDetalheRetorno,
  ativo: boolean,
  token?: string,
): Promise<FormData> {
  const form = new FormData();
  form.append('nome', detalhe.nome ?? '');
  form.append('descricao', detalhe.descricao ?? '');
  form.append('tipo', detalhe.tipo ?? 'publica');
  form.append('permitePostagemMembro', detalhe.permitePostagemMembro ? 'true' : 'false');
  form.append('permiteSair', detalhe.permiteSair ? 'true' : 'false');
  form.append('publicacaoConfiguracaoPolitica', detalhe.publicacaoConfiguracaoPolitica ?? 'desativado');
  form.append('publicacaoPermiteComentario', detalhe.publicacaoPermiteComentario ? 'true' : 'false');
  form.append('publicacaoPermiteLikeHabilitado', detalhe.publicacaoPermiteLikeHabilitado ? 'true' : 'false');
  form.append('ativo', ativo ? 'true' : 'false');
  (detalhe.moderadores ?? []).forEach((m) => {
    form.append('codigosInternoColaboradoresModeradores', m.codigoInternoColaborador);
  });
  if (detalhe.tipo === 'privada' && detalhe.membros?.length) {
    detalhe.membros.forEach((m) => {
      form.append('codigoInternoColaboradoresParticipantes', m.codigoInternoColaborador);
    });
  }
  if (detalhe.capaUrl && detalhe.capaUrl.trim() !== '' && token) {
    try {
      const capaUrlResolved = resolveCapaUrl(detalhe.capaUrl, token);
      const res = await fetch(capaUrlResolved);
      if (res.ok) {
        const blob = await res.blob();
        const ext = detalhe.capaUrl?.match(/\.(jpe?g|png|gif|webp)$/i)?.[1] ?? 'jpg';
        form.append('capaComunidade', blob, `capa.${ext}`);
      }
    } catch {
      // Falha ao buscar capa: segue sem o arquivo
    }
  }
  return form;
}
