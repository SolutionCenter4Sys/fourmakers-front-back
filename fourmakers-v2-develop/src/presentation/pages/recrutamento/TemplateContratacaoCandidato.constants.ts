/**
 * Opções para os Selects da página Template de Contratação.
 * value = valor enviado no save; label = exibido no front.
 */

/** Tipo de Jornada: front "Horas Fechadas" / "Horas Abertas", valor false/true (string no estado). */
export const TIPO_JORNADA_OPCOES = [
  { value: 'false', label: 'Horas Fechadas' },
  { value: 'true', label: 'Horas Abertas' },
] as const;

/** Dias no presencial: 1 a 5. Exibido apenas para modelo Híbrido; quando oculto (ex.: Remoto) usa-se 5 como default. */
export const DIAS_PRESENCIAL_OPCOES = [1, 2, 3, 4, 5].map((n) => ({
  value: String(n),
  label: n === 1 ? '1 Dia Presencial' : `${n} Dias Presenciais`,
}));

/** Valor default para quantidadeDiasPresencial quando o campo está oculto (modelo não é Híbrido). */
export const DIAS_PRESENCIAL_DEFAULT_OCULTO = 5;

/** Tamanho da camiseta: valor e front iguais. */
export const TAMANHO_CAMISETA_OPCOES = ['PP', 'P', 'M', 'G', 'GG', 'XG', 'XXG', 'XXXG'] as const;

/** UF: todos os estados brasileiros em maiúsculo (valor e front iguais). */
export const UF_ESTADOS_BR = [
  'AC', 'AL', 'AP', 'AM', 'BA', 'CE', 'DF', 'ES', 'GO', 'MA', 'MT', 'MS', 'MG', 'PA', 'PB', 'PR', 'PE', 'PI', 'RJ', 'RN', 'RS', 'RO', 'RR', 'SC', 'SP', 'SE', 'TO',
] as const;

/** Tipo de deficiência: value numérico (0-6), ordem e rótulos conforme especificação. */
export const TIPO_DEFICIENCIA_OPCOES: { value: string; label: string }[] = [
  { value: '5', label: 'Nenhuma' },
  { value: '1', label: 'Visual' },
  { value: '2', label: 'Fisica' },
  { value: '3', label: 'Intelectual' },
  { value: '4', label: 'TEA' },
  { value: '0', label: 'Auditiva' },
  { value: '6', label: 'Psicossocial / Mental' },
];

/** Proprietário da Máquina: valor e front iguais. */
export const PROPRIETARIO_MAQUINA_OPCOES = ['FourSys', 'Cliente'] as const;

/** Tipo de Acesso: valor e front iguais. */
export const TIPO_ACESSO_OPCOES = ['Web', 'Outlook'] as const;

/** Lista fixa de Sistemas Liberados (ordem e rótulos da interface). */
export const SISTEMAS_LIBERADOS_LABELS = [
  'CCH - administrador',
  'Mapa de Alocação',
  'Recrutamento e Seleção - Criação de Vagas',
  'SCC - Necessário autorização diretoria',
  'Controle de Gadget',
  'Portal de Projetos',
  'Recrutamento e Seleção - Trabalhar as Vagas',
  'CRM',
  'Recrutamento e Seleção - Aprovador de Vagas',
  'SCC - Custo profissional',
] as const;

/** Lista fixa de Diretórios de Rede (ordem e rótulos da interface). */
export const DIRETORIOS_REDE_LABELS = [
  'Diretório Com - UN 2, 3 e 4 (Executivos de Conta)',
  'Diretório de Marketing',
  'Diretório de RH',
  'Diretório de Infra',
  'Diretório de Pré Vendas - UN 2, 3 e 4 (gerentes de projetos e executivos de contas)',
  'Diretório Faturamento_Bradesco',
  'Diretório de Manutenção Predial',
  'Diretório de Recrutamento e Seleção (RS)',
  'Diretório PMO - UN 1 (Bradesco) Diretório de Propostas',
] as const;

/** Lista fixa de Grupos de E-mail (ordem e rótulos da interface). */
export const GRUPOS_EMAIL_LABELS = ['Alphaville', 'Curitiba', 'Paulista', 'Todos Foursys'] as const;
