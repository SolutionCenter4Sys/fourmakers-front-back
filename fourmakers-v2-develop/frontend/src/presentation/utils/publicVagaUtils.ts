import type { VagaDetails, VagaSkill } from '@domain/entities/VagaDetails';
import type { BuscarDadosColaboradorResponse } from '@domain/entities/Profile360';

/** Formata input de WhatsApp (00) 00000-0000. */
export function formatWhatsApp(value: string): string {
  const d = value.replace(/\D/g, '');
  if (d.length <= 2) return d ? `(${d}` : '';
  if (d.length <= 7) return `(${d.slice(0, 2)}) ${d.slice(2)}`;
  return `(${d.slice(0, 2)}) ${d.slice(2, 7)}-${d.slice(7, 11)}`;
}

/** Máscara R$ 00,00 para input (apenas dígitos, centavos). Retorna string formatada pt-BR. */
export function formatCurrencyInputBR(value: string): string {
  const cleaned = value.replace(/\D/g, '');
  const num = parseInt(cleaned || '0', 10) / 100;
  return num.toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
}

/** Converte string pt-BR (ex.: "1.234,56") em número. Retorna null se inválido. */
export function parseBRLCurrencyInput(value: string): number | null {
  const cleaned = (value ?? '').replace(/\D/g, '');
  if (cleaned === '') return null;
  const num = parseInt(cleaned, 10) / 100;
  return Number.isFinite(num) ? num : null;
}

/** Agrupa skills da vaga em técnicas, socioemocionais e outros (idiomas, metodologias, etc.). */
export function groupSkills(skills: VagaDetails['skills']): {
  tecnicas: VagaSkill[];
  socioemocionais: VagaSkill[];
  outros: VagaSkill[];
} {
  if (!Array.isArray(skills) || skills.length === 0) {
    return { tecnicas: [], socioemocionais: [], outros: [] };
  }
  const tecnicas = skills.filter((s) => s.tipoSkillId === 1);
  const socioemocionais = skills.filter((s) => s.tipoSkillId === 2);
  const outros = skills.filter((s) => s.tipoSkillId !== 1 && s.tipoSkillId !== 2);
  return { tecnicas, socioemocionais, outros };
}

export type SkillFaltante = {
  id: string;
  skillId: number;
  skillDescription: string;
  skillNivelDescription: string;
  tipoSkillId: number;
};

/** Skills da vaga que o colaborador ainda não possui no perfil (por descrição/nome). */
export function skillsFaltantes(
  vagaSkills: VagaDetails['skills'],
  dadosColaborador: BuscarDadosColaboradorResponse | null
): SkillFaltante[] {
  if (!Array.isArray(vagaSkills) || vagaSkills.length === 0) return [];
  const perfil = dadosColaborador?.perfilProfissional;
  const descricoesPerfil = new Set<string>();
  if (perfil?.competencias) {
    perfil.competencias.forEach((c) => {
      const d = c.competencia?.descricao?.trim().toLowerCase();
      if (d) descricoesPerfil.add(d);
    });
  }
  if (perfil?.softskills) {
    perfil.softskills.forEach((s) => {
      const d = (s.softSkill?.nome ?? s.softSkill?.descricao)?.trim().toLowerCase();
      if (d) descricoesPerfil.add(d);
    });
  }
  if (perfil?.idiomas) {
    perfil.idiomas.forEach((i) => {
      const d = i.idioma?.descricao?.trim().toLowerCase();
      if (d) descricoesPerfil.add(d);
    });
  }
  return vagaSkills.filter((vs) => {
    const desc = (vs.skillDescription ?? '').trim().toLowerCase();
    return desc && !descricoesPerfil.has(desc);
  });
}

/**
 * Resolve o nível da API pelo texto exibido no card (exigência da vaga ou escolha no popover),
 * para não gravar sempre o primeiro item da lista.
 */
export function matchNivelPorDescricaoParaInclusao(
  niveis: { id: number; descricao: string }[],
  descricaoCard: string
): { id: number; descricao: string } | null {
  const p = (descricaoCard ?? '').trim().toLowerCase();
  if (!p || niveis.length === 0) return null;
  const norm = (s: string) => (s ?? '').trim().toLowerCase();
  const exato = niveis.find((n) => norm(n.descricao) === p);
  if (exato) return exato;
  const apiContemCard = niveis.find((n) => norm(n.descricao).includes(p));
  if (apiContemCard) return apiContemCard;
  const cardContemApi = niveis.find(
    (n) => p.includes(norm(n.descricao)) && norm(n.descricao).length >= 2
  );
  if (cardContemApi) return cardContemApi;
  const prefixo = p.slice(0, Math.min(6, p.length));
  if (prefixo.length >= 3) {
    const porRadical = niveis.find((n) => {
      const d = norm(n.descricao);
      const pd = d.slice(0, Math.min(6, d.length));
      return pd === prefixo || d.startsWith(prefixo) || p.startsWith(pd);
    });
    if (porRadical) return porRadical;
  }
  return null;
}

/** Detecta se a mensagem de erro da API indica candidatura já existente. */
export function isCandidaturaJaExisteMessage(message: string): boolean {
  return /já existe uma candidatura|ja existe uma candidatura/i.test(message);
}
