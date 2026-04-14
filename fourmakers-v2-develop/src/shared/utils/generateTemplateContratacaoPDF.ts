import { jsPDF } from 'jspdf';

const MARGIN = 14;
const LINE_HEIGHT = 6;
const FONT_SIZE_NORMAL = 10;
const FONT_SIZE_TITLE = 14;

export type TipoPdfTemplateContratacao = 'com_remuneracao' | 'sem_remuneracao';

/** Seção "Benefícios e Lançamento Complementar" – exibida apenas em PDF com remuneração. */
const SECAO_BENEFICIOS_KEYS = [
  'salario',
  'cargoConfianca',
  'valorAdicionalCargoConfianca',
  'custoHora',
  'vr',
  'va',
  'assistenciaMedica',
  'ajudaDeCusto',
  'mobilidade',
  'educacao',
  'remuneracaoTotal',
];

export interface GerarTemplateContratacaoPdfOpcoes {
  /** Org 9: mesmo recorte visual do formulário (sem acessórios, localização/equip., acessos, etc.). */
  modoOrg9?: boolean;
}

/**
 * Gera um PDF com os dados do retorno do template de contratação (Criar/Atualizar).
 * - "com_remuneracao": segue o modelo Ficha de Colaborador, incluindo a seção Benefícios.
 * - "sem_remuneracao": mesmo layout, porém sem nenhum campo do grupo Benefícios (valores, cargo confiança, remuneração total, etc.).
 */
export function generateTemplateContratacaoPDF(
  retorno: unknown,
  tipoPdf?: TipoPdfTemplateContratacao,
  opcoes?: GerarTemplateContratacaoPdfOpcoes
): void {
  if (retorno == null || typeof retorno !== 'object') return;

  const doc = new jsPDF();
  const pageWidth = doc.internal.pageSize.getWidth();
  const maxWidth = pageWidth - 2 * MARGIN;
  let y = MARGIN;

  const data = retorno as Record<string, unknown>;
  const semRemuneracao = tipoPdf === 'sem_remuneracao';
  const modoOrg9 = opcoes?.modoOrg9 === true;

  const nomeArquivo =
    modoOrg9
      ? 'template-contratacao.pdf'
      : tipoPdf === 'com_remuneracao'
        ? 'template-contratacao-com-remuneracao.pdf'
        : tipoPdf === 'sem_remuneracao'
          ? 'template-contratacao-sem-remuneracao.pdf'
          : 'template-contratacao.pdf';

  const formatValue = (value: unknown): string => {
    if (value == null) return '—';
    if (typeof value === 'boolean') return value ? 'Sim' : 'Não';
    if (typeof value === 'object') return JSON.stringify(value);
    return String(value);
  };

  const formatCurrency = (value: unknown): string => {
    if (value == null || value === '') return 'R$ 0';
    const num = typeof value === 'number' ? value : parseFloat(String(value).replace(',', '.')) || 0;
    if (Number.isNaN(num)) return 'R$ 0';
    return `R$ ${num.toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`;
  };

  const pushText = (text: string, options?: { bold?: boolean }) => {
    if (y > doc.internal.pageSize.getHeight() - 20) {
      doc.addPage();
      y = MARGIN;
    }
    doc.setFont('helvetica', options?.bold ? 'bold' : 'normal');
    const lines = doc.splitTextToSize(text, maxWidth);
    lines.forEach((line: string) => {
      doc.text(line, MARGIN, y);
      y += LINE_HEIGHT;
    });
  };

  // Título no modelo da Ficha de Colaborador
  doc.setFontSize(16);
  doc.setFont('helvetica', 'bold');
  doc.text('Ficha de Colaborador', MARGIN, y);
  y += LINE_HEIGHT;

  const nome = formatValue(data.nomeCompleto);
  doc.setFontSize(12);
  doc.setFont('helvetica', 'normal');
  doc.text(nome !== '—' ? nome : '—', MARGIN, y);
  y += LINE_HEIGHT + 4;

  const sectionsPadrao: { title: string; keys: string[] }[] = [
    {
      title: 'Dados Pessoais',
      keys: [
        'nomeCompleto',
        'documentoColaborador',
        'rgColaborador',
        'dataNascimento',
        'contatoPrincipal',
        'emailPessoal',
        'emailCorporativo',
        'loginRede',
        'tipoLoginRede',
      ],
    },
    {
      title: 'Saúde do Colaborador',
      keys: ['saude'],
    },
    {
      title: 'Endereço',
      keys: ['endereco'],
    },
    {
      title: 'Informações da Contratação',
      keys: [
        'codigoVaga',
        'tituloVaga',
        'nomeClienteVaga',
        'cargo',
        'grupoAreaEquipamentoPadraoCargoFuncao',
        'dataInicio',
        'modeloTrabalhoDescricao',
        'nomeColaboradorSuperiorImediato',
        'nomeColaboradorAnalista',
        'observacoesAcessoUsuario',
      ],
    },
    {
      title: 'Equipamento',
      keys: [
        'primeiraOpcaoEquipamentoPadraoCargoFuncao',
        'segundaOpcaoEquipamentoPadraoCargoFuncao',
        'tipoMaquina',
        'horarioJornada',
        'tipoHorarioJornada',
      ],
    },
    ...(semRemuneracao
      ? []
      : [
          {
            title: 'Benefícios',
            keys: SECAO_BENEFICIOS_KEYS,
          },
        ]),
    {
      title: 'Equipamentos Foursys',
      keys: ['celular', 'planoDados', 'quantidadeMinutosPlanoDados', 'cartaoVisitas', 'quantidadeCartaoVisitas', 'outrosEquipamentos'],
    },
    {
      title: 'Diretórios de Rede',
      keys: ['diretorios'],
    },
    {
      title: 'Outros Grupos (E-mail)',
      keys: ['outrosGrupos'],
    },
    {
      title: 'Sistemas Liberados',
      keys: ['sistemasLiberados'],
    },
    {
      title: 'Grupos de E-mail',
      keys: ['gruposEmails'],
    },
  ];

  const sectionsOrg9: { title: string; keys: string[] }[] = [
    {
      title: 'Dados Pessoais',
      keys: [
        'nomeCompleto',
        'documentoColaborador',
        'rgColaborador',
        'dataNascimento',
        'contatoPrincipal',
        'emailPessoal',
        'emailCorporativo',
      ],
    },
    { title: 'Saúde do Colaborador', keys: ['saude'] },
    { title: 'Endereço', keys: ['endereco'] },
    {
      title: 'Informações da Contratação',
      keys: [
        'codigoVaga',
        'tituloVaga',
        'nomeClienteVaga',
        'cargo',
        'dataInicio',
        'modeloTrabalhoDescricao',
        'nomeColaboradorAnalista',
      ],
    },
    ...(semRemuneracao
      ? []
      : [{ title: 'Benefícios', keys: SECAO_BENEFICIOS_KEYS }]),
  ];

  const sections = modoOrg9 ? sectionsOrg9 : sectionsPadrao;

  for (const section of sections) {
    doc.setFontSize(FONT_SIZE_TITLE);
    doc.setFont('helvetica', 'bold');
    doc.text(section.title, MARGIN, y);
    y += LINE_HEIGHT + 2;
    doc.setFontSize(FONT_SIZE_NORMAL);

    for (const key of section.keys) {
      const raw = data[key];

      if (key === 'endereco' && raw && typeof raw === 'object') {
        const end = raw as Record<string, unknown>;
        pushText(
          `Rua: ${formatValue(end.endereco)} Número: ${formatValue(end.numero)} Complemento: ${formatValue(end.complemento) || 'N/A'} Bairro: ${formatValue(end.bairro)}`
        );
        pushText(`Cidade: ${formatValue(end.cidade)} Estado: ${formatValue(end.estado)} CEP: ${formatValue(end.cep)}`);
        continue;
      }

      if (key === 'saude' && raw && typeof raw === 'object') {
        const s = raw as Record<string, unknown>;
        pushText(
          `Candidato PCD: ${formatValue(s.pcd)} Deficiência descrita: ${formatValue(s.tipoPcd)} Risco COVID: ${formatValue(s.grupoDeRiscoCovid)} Adaptações necessárias: ${formatValue((data.restricaoAdaptacao as string) ?? (s.condicaoDeSaudeRelevante as string)) || 'N/A'}`
        );
        continue;
      }

      if (key === 'diretorios' || key === 'gruposEmails' || key === 'outrosGrupos' || key === 'sistemasLiberados') {
        const arr = Array.isArray(raw) ? raw : [];
        const emptyMsg =
          key === 'diretorios'
            ? 'Sem diretórios informados'
            : key === 'outrosGrupos'
              ? 'Sem outros grupos de e-mail informados'
              : key === 'sistemasLiberados'
                ? 'Sem sistemas liberados informados'
                : 'Sem grupos informados';
        if (arr.length === 0) {
          pushText(emptyMsg);
        } else {
          arr.slice(0, 15).forEach((item: unknown) => {
            const line =
              typeof item === 'object' && item != null
                ? (item as Record<string, unknown>).descricao ??
                  (item as Record<string, unknown>).emailGrupo ??
                  JSON.stringify(item)
                : String(item);
            pushText(`  • ${line}`);
          });
          if (arr.length > 15) pushText(`  ... e mais ${arr.length - 15}`);
        }
        continue;
      }

      // Benefícios: labels em português e valores em R$
      if (SECAO_BENEFICIOS_KEYS.includes(key)) {
        const label =
          key === 'salario'
            ? 'Salário'
            : key === 'cargoConfianca'
              ? 'É cargo de confiança'
              : key === 'valorAdicionalCargoConfianca'
                ? 'Ad. Cargo de Confiança'
                : key === 'custoHora'
                  ? 'Custo Hora'
                  : key === 'vr'
                    ? 'VR'
                    : key === 'va'
                      ? 'VA'
                      : key === 'assistenciaMedica'
                        ? 'Assistência Médica'
                        : key === 'ajudaDeCusto'
                          ? 'Ajuda de Custo'
                          : key === 'mobilidade'
                            ? 'Mobilidade'
                            : key === 'educacao'
                              ? 'Educação'
                              : key === 'remuneracaoTotal'
                                ? 'Remuneração Total'
                                : key;
        const value = key === 'cargoConfianca' ? formatValue(raw) : formatCurrency(raw);
        pushText(`${label}: ${value}`);
        continue;
      }

      const label = key.replace(/([A-Z])/g, ' $1').replace(/^./, (s) => s.toUpperCase()).trim();
      pushText(`${label}: ${formatValue(raw)}`);
    }
    y += 4;
  }

  doc.save(nomeArquivo);
}
