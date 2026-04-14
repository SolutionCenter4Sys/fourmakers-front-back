import { jsPDF } from 'jspdf';
import logoFourmakers from '@/assets/logo-fourmakers.svg';
import type {
  BuscarDadosColaboradorResponse,
  CertificadoCompletoProfile360,
  CompetenciaProfile360,
  DominioProfile360,
  IdiomaProfile360,
  MetodologiaProfile360,
  SoftSkillProfile360,
} from '@domain/entities/Profile360';
import type { EscolaridadeColaborador } from '@domain/entities/Escolaridade';

export type ProfilePdfNameMode = 'name_with_surname_initials' | 'full_name' | 'initials_only';

interface GenerateProfile360PDFParams {
  dados: BuscarDadosColaboradorResponse;
  escolaridades: EscolaridadeColaborador[];
  nameMode: ProfilePdfNameMode;
}

const BRAND_PRIMARY = '#9A1BFF';
const TEXT_DARK = '#111827';
const TEXT_MUTED = '#6B7280';
const BORDER = '#E5E7EB';

const hexToRgb = (hex: string): [number, number, number] => {
  const clean = hex.replace('#', '');
  const bigint = parseInt(clean, 16);
  return [(bigint >> 16) & 255, (bigint >> 8) & 255, bigint & 255];
};

const splitName = (name: string): string[] =>
  name
    .trim()
    .split(/\s+/)
    .filter(Boolean);

const getDisplayName = (name: string, mode: ProfilePdfNameMode): string => {
  const parts = splitName(name);
  if (!parts.length) return 'Profissional';

  if (mode === 'full_name') return parts.join(' ');

  if (mode === 'initials_only') {
    return parts.map((part) => `${part[0]?.toUpperCase() ?? ''}.`).join(' ');
  }

  if (parts.length === 1) return parts[0];
  const [firstName, ...rest] = parts;
  const initials = rest.map((part) => `${part[0]?.toUpperCase() ?? ''}.`).join(' ');
  return `${firstName} ${initials}`.trim();
};

const formatMonthYear = (value?: string | null): string => {
  if (!value) return 'Atual';
  const date = new Date(value);
  if (Number.isNaN(date.getTime())) return value;
  return date.toLocaleDateString('pt-BR', { month: '2-digit', year: 'numeric' });
};

const formatDateTime = (value: Date): string =>
  value.toLocaleString('pt-BR', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  });

const toSkillLabel = (
  descricao: string | null | undefined,
  nivel: string | null | undefined,
): string => {
  const descricaoFinal = descricao?.trim() || 'Não informado';
  const nivelFinal = nivel?.trim();
  if (!nivelFinal || nivelFinal.toLowerCase() === 'a definir') return descricaoFinal;
  return `${descricaoFinal} - ${nivelFinal}`;
};

const unique = (items: string[]): string[] => Array.from(new Set(items.filter(Boolean)));

const extractCompetencias = (items: CompetenciaProfile360[]): string[] =>
  unique(items.map((item) => toSkillLabel(item.competencia?.descricao, item.nivel?.descricao)));

const extractSoftskills = (items: SoftSkillProfile360[]): string[] =>
  unique(items.map((item) => toSkillLabel(item.softSkill?.descricao, item.nivel?.descricao)));

const extractIdiomas = (items: IdiomaProfile360[]): string[] =>
  unique(items.map((item) => toSkillLabel(item.idioma?.descricao, item.nivel?.descricao)));

const extractMetodologias = (items: MetodologiaProfile360[]): string[] =>
  unique(items.map((item) => toSkillLabel(item.metodologia?.descricao, item.nivel?.descricao)));

const extractDominios = (items: DominioProfile360[]): string[] =>
  unique(items.map((item) => toSkillLabel(item.dominio?.descricao, item.nivel?.descricao)));

const extractCertificados = (items: CertificadoCompletoProfile360[]): string[] =>
  unique(
    items.map((item) => {
      const cert = item.certificado;
      if (!cert) return '';
      const descricao = cert.descricao?.trim() || 'Certificado';
      const instituicao = cert.instituicao?.trim();
      const conclusao = cert.conclusao ? formatMonthYear(cert.conclusao) : null;
      const complemento = [instituicao, conclusao].filter(Boolean).join(' | ');
      return complemento ? `${descricao} (${complemento})` : descricao;
    }),
  );

const loadImageElement = (src: string): Promise<HTMLImageElement> =>
  new Promise((resolve, reject) => {
    const image = new Image();
    image.onload = () => resolve(image);
    image.onerror = () => reject(new Error('Erro ao carregar imagem'));
    image.src = src;
  });

const getWhiteLogoDataUrl = async (): Promise<string | null> => {
  try {
    const response = await fetch(logoFourmakers);
    if (!response.ok) return null;
    const svgContent = await response.text();
    const whiteSvg = svgContent.replace(/#242424/gi, '#FFFFFF');
    const svgBase64 = btoa(unescape(encodeURIComponent(whiteSvg)));
    const svgDataUrl = `data:image/svg+xml;base64,${svgBase64}`;
    const svgImage = await loadImageElement(svgDataUrl);
    const canvas = document.createElement('canvas');
    canvas.width = svgImage.width;
    canvas.height = svgImage.height;
    const context = canvas.getContext('2d');
    if (!context) return null;
    context.drawImage(svgImage, 0, 0);
    return canvas.toDataURL('image/png');
  } catch {
    return null;
  }
};

export const generateProfile360PDF = async ({
  dados,
  escolaridades,
  nameMode,
}: GenerateProfile360PDFParams): Promise<void> => {
  const doc = new jsPDF({ unit: 'mm', format: 'a4' });
  const margin = 14;
  const pageWidth = doc.internal.pageSize.getWidth();
  const pageHeight = doc.internal.pageSize.getHeight();
  const maxContentWidth = pageWidth - margin * 2;
  const emissionDate = new Date();
  let y = margin;

  const ensureSpace = (heightNeeded: number) => {
    if (y + heightNeeded <= pageHeight - 16) return;
    doc.addPage();
    y = margin;
  };

  const writeSectionTitle = (title: string) => {
    const spacing = 3.2;
    ensureSpace(12 + spacing * 2);
    y += spacing;
    doc.setFont('helvetica', 'bold');
    doc.setTextColor(...hexToRgb(TEXT_DARK));
    doc.setFontSize(13);
    doc.text(title, margin, y);
    y += spacing;
  };

  const writeParagraph = (text: string) => {
    const safeText = text.trim() || 'Não informado.';
    doc.setFont('helvetica', 'normal');
    doc.setTextColor(...hexToRgb(TEXT_DARK));
    doc.setFontSize(10);
    const lines = doc.splitTextToSize(safeText, maxContentWidth);
    lines.forEach((line: string) => {
      ensureSpace(5);
      doc.text(line, margin, y);
      y += 4.6;
    });
  };

  const drawChip = (text: string, x: number, startY: number): { width: number; height: number } => {
    const chipText = text.trim();
    doc.setFont('helvetica', 'normal');
    doc.setFontSize(8.2);
    const maxChipWidth = maxContentWidth * 0.52;
    const textWidth = Math.min(doc.getTextWidth(chipText), maxChipWidth - 4.4);
    const chipWidth = textWidth + 4.4;
    const chipHeight = 4.8;

    doc.setFillColor(238, 242, 255);
    doc.setDrawColor(224, 231, 255);
    doc.roundedRect(x, startY, chipWidth, chipHeight, 1, 1, 'FD');
    doc.setTextColor(30, 64, 175);
    doc.text(chipText, x + 2.2, startY + 3.25, { maxWidth: maxChipWidth - 4.4 });
    return { width: chipWidth, height: chipHeight };
  };

  const writeTagSection = (title: string, items: string[]) => {
    if (!items.length) return;
    writeSectionTitle(title);
    let chipX = margin;
    let chipY = y;
    const gapX = 1.8;
    const gapY = 1.8;

    items.forEach((item) => {
      const value = item.trim();
      if (!value) return;

      const predictedWidth = Math.min(doc.getTextWidth(value), maxContentWidth * 0.52 - 4.4) + 4.4;
      if (chipX + predictedWidth > margin + maxContentWidth) {
        chipX = margin;
        chipY += 4.8 + gapY;
      }

      ensureSpace(8);
      const chip = drawChip(value, chipX, chipY);
      chipX += chip.width + gapX;
    });

    y = chipY + 4.8 + 3.4;
  };

  const nameToDisplay = getDisplayName(dados.colaborador.nomeCompleto || '', nameMode);
  const cityUf = [dados.colaborador.endereco?.cidade, dados.colaborador.endereco?.estado]
    .filter(Boolean)
    .join('/');
  const logoDataUrl = await getWhiteLogoDataUrl();

  doc.setFillColor(...hexToRgb(BRAND_PRIMARY));
  doc.rect(0, 0, pageWidth, 28, 'F');
  if (logoDataUrl) {
    doc.addImage(logoDataUrl, 'PNG', pageWidth - margin - 26, 6, 26, 11);
  }
  doc.setTextColor(255, 255, 255);
  doc.setFont('helvetica', 'bold');
  doc.setFontSize(16);
  doc.text('Currículo Profissional', margin, 11);
  doc.setFontSize(10);
  doc.setFont('helvetica', 'normal');
  doc.text(`Emitido em ${formatDateTime(emissionDate)}`, margin, 18);

  y = 36;
  doc.setTextColor(...hexToRgb(TEXT_DARK));
  doc.setFont('helvetica', 'bold');
  doc.setFontSize(18);
  doc.text(nameToDisplay, margin, y);
  y += 6;

  doc.setFont('helvetica', 'normal');
  doc.setFontSize(10);
  doc.setTextColor(...hexToRgb(TEXT_MUTED));
  const contato = [cityUf, dados.colaborador.contatoPrincipal, dados.colaborador.emailAlternativo || dados.colaborador.email]
    .filter(Boolean)
    .join('  •  ');
  doc.text(contato || 'Contato não informado', margin, y);
  y += 6;

  doc.setDrawColor(...hexToRgb(BORDER));
  doc.line(margin, y, pageWidth - margin, y);
  y += 7;

  writeSectionTitle('Sobre');
  writeParagraph(dados.colaborador.sobre || 'Não informado.');
  y += 2.5;

  const perfil = dados.perfilProfissional;
  writeTagSection('Habilidades Técnicas', extractCompetencias(perfil.competencias || []));
  writeTagSection('Habilidades Socioemocionais', extractSoftskills(perfil.softskills || []));
  writeTagSection('Idiomas', extractIdiomas(perfil.idiomas || []));
  writeTagSection('Metodologias', extractMetodologias(perfil.metodologias || []));
  writeTagSection('Domínios de Negócio', extractDominios(perfil.dominios || []));
  writeTagSection('Certificados', extractCertificados(perfil.certificados || []));

  if (escolaridades.length) {
    writeSectionTitle('Formação Acadêmica');
    escolaridades.forEach((item) => {
      ensureSpace(8);
      doc.setFont('helvetica', 'bold');
      doc.setFontSize(10);
      doc.setTextColor(...hexToRgb(TEXT_DARK));
      doc.text(item.formacaoDescricao || 'Formação', margin, y);
      doc.setFont('helvetica', 'normal');
      doc.setTextColor(...hexToRgb(TEXT_MUTED));
      const periodo = `${formatMonthYear(item.dataInicio)} - ${
        item.dataTermino ? formatMonthYear(item.dataTermino) : 'Atual'
      }`;
      doc.text(periodo, pageWidth - margin, y, { align: 'right' });
      y += 4.6;
      writeParagraph(
        [item.instituicao, item.descricao].filter(Boolean).join(' | ') || 'Não informado.',
      );
      y += 1.5;
    });
  }

  if (perfil.experienciaEmpresas?.length) {
    writeSectionTitle('Experiências Profissionais');
    perfil.experienciaEmpresas.forEach((empresa) => {
      ensureSpace(8);
      doc.setFont('helvetica', 'bold');
      doc.setFontSize(10.5);
      doc.setTextColor(...hexToRgb(TEXT_DARK));
      doc.text(empresa.empresa || 'Empresa não informada', margin, y);
      doc.setFont('helvetica', 'normal');
      doc.setTextColor(...hexToRgb(TEXT_MUTED));
      doc.text(
        `${formatMonthYear(empresa.dataInicio)} - ${
          empresa.atual ? 'Atual' : formatMonthYear(empresa.dataSaida)
        }`,
        pageWidth - margin,
        y,
        { align: 'right' },
      );
      y += 5;

      (empresa.experiencias || []).forEach((experiencia) => {
        ensureSpace(8);
        doc.setFont('helvetica', 'bold');
        doc.setFontSize(10);
        doc.setTextColor(...hexToRgb(TEXT_DARK));
        doc.text(`• ${experiencia.funcao || 'Função não informada'}`, margin, y);
        y += 4.2;
        doc.setFont('helvetica', 'normal');
        doc.setTextColor(...hexToRgb(TEXT_DARK));
        const atividades = doc.splitTextToSize(
          experiencia.atividades || 'Atividades não informadas.',
          maxContentWidth - 4,
        );
        atividades.forEach((line: string) => {
          ensureSpace(4.4);
          doc.text(line, margin + 4, y);
          y += 4;
        });
        y += 1;
      });
      y += 1.5;
    });
  }

  const vistos = dados.colaborador.vistos?.map((visto) => {
    const pais = visto.descricaoPais || 'País não informado';
    const validade = formatMonthYear(visto.validade);
    return `${pais} - Validade: ${validade}`;
  }) ?? [];

  const passaportes = dados.colaborador.passaportes?.map((passaporte) => {
    const nacionalidade = passaporte.descricaoNacionalidade || 'Nacionalidade não informada';
    const validade = formatMonthYear(passaporte.validade);
    return `${nacionalidade} - Validade: ${validade}`;
  }) ?? [];

  writeTagSection('Vistos', vistos);
  writeTagSection('Passaportes', passaportes);

  const totalPages = doc.getNumberOfPages();
  const rodapeData = formatDateTime(emissionDate);
  for (let page = 1; page <= totalPages; page += 1) {
    doc.setPage(page);
    doc.setFont('helvetica', 'normal');
    doc.setFontSize(8.5);
    doc.setTextColor(...hexToRgb(TEXT_MUTED));
    doc.text(rodapeData, margin, pageHeight - 8);
    doc.text(`Página ${page} de ${totalPages}`, pageWidth - margin, pageHeight - 8, { align: 'right' });
  }

  const fileNameBase = (nameToDisplay || 'curriculo')
    .normalize('NFD')
    .replace(/[\u0300-\u036f]/g, '')
    .replace(/[^\w\s.-]/g, '')
    .trim()
    .replace(/\s+/g, '_');

  doc.save(`${fileNameBase || 'curriculo'}-CV.pdf`);
};
