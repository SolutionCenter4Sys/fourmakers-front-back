import { jsPDF } from 'jspdf';
import type { ColaboradorDetalhes } from '@shared/types/bookColaborador';
import { formatData, formatTelefone, formatSalario, formatSaldoHoras } from './bookColaboradorFormatters';

const BRAND_PRIMARY = '#9A1BFF';
const TEXT_DARK = '#0F172A';
const TEXT_MUTED = '#64748B';
const BORDER_SOFT = '#E2E8F0';

const hexToRgb = (hex: string): [number, number, number] => {
  const clean = hex.replace('#', '');
  const bigint = parseInt(clean, 16);
  return [(bigint >> 16) & 255, (bigint >> 8) & 255, bigint & 255];
};

export const generateCVPDF = (detalhes: ColaboradorDetalhes): void => {
  try {
    const doc = new jsPDF();
    const pageWidth = doc.internal.pageSize.getWidth();
    const margin = 15;
    let yPos = margin;

    // Header com cor de marca
    doc.setFillColor(...hexToRgb(BRAND_PRIMARY));
    doc.rect(0, 0, pageWidth, 30, 'F');

    // Título
    doc.setTextColor(255, 255, 255);
    doc.setFontSize(20);
    doc.setFont('helvetica', 'bold');
    doc.text('Currículo do Colaborador', margin, 20);

    yPos = 40;

    // Nome e Cargo
    doc.setTextColor(...hexToRgb(TEXT_DARK));
    doc.setFontSize(18);
    doc.setFont('helvetica', 'bold');
    doc.text(detalhes.nome, margin, yPos);
    yPos += 8;

    doc.setFontSize(12);
    doc.setFont('helvetica', 'normal');
    doc.setTextColor(...hexToRgb(TEXT_MUTED));
    doc.text(detalhes.cargo, margin, yPos);
    yPos += 15;

    // Linha separadora
    doc.setDrawColor(...hexToRgb(BORDER_SOFT));
    doc.line(margin, yPos, pageWidth - margin, yPos);
    yPos += 10;

    // Informações Pessoais
    doc.setFontSize(14);
    doc.setFont('helvetica', 'bold');
    doc.setTextColor(...hexToRgb(TEXT_DARK));
    doc.text('Informações Pessoais', margin, yPos);
    yPos += 8;

    doc.setFontSize(10);
    doc.setFont('helvetica', 'normal');
    doc.setTextColor(...hexToRgb(TEXT_DARK));
    
    const infoPessoais = [
      ['Código:', `#${detalhes.codColaborador}`],
      ['Data de Nascimento:', formatData(detalhes.nascimento)],
      ['Telefone:', formatTelefone(detalhes.telefone)],
      ['E-mail:', detalhes.email],
      ['Cidadania:', detalhes.cidadania],
    ];

    infoPessoais.forEach(([label, value]) => {
      doc.setTextColor(...hexToRgb(TEXT_MUTED));
      doc.text(label, margin, yPos);
      doc.setTextColor(...hexToRgb(TEXT_DARK));
      doc.text(value, margin + 50, yPos);
      yPos += 6;
    });

    yPos += 5;

    // Verificar se precisa de nova página
    if (yPos > 250) {
      doc.addPage();
      yPos = margin;
    }

    // Endereço
    doc.setFontSize(14);
    doc.setFont('helvetica', 'bold');
    doc.setTextColor(...hexToRgb(TEXT_DARK));
    doc.text('Endereço', margin, yPos);
    yPos += 8;

    doc.setFontSize(10);
    doc.setFont('helvetica', 'normal');
    
    const enderecoCompleto = `${detalhes.endereco.rua}, ${detalhes.endereco.numero}${detalhes.endereco.complemento ? ` - ${detalhes.endereco.complemento}` : ''}`;
    const enderecoLinhas = [
      ['Rua:', enderecoCompleto],
      ['Bairro:', detalhes.endereco.bairro],
      ['Cidade:', detalhes.endereco.cidade],
      ['Estado:', detalhes.endereco.estado],
      ['CEP:', detalhes.endereco.cep],
    ];

    enderecoLinhas.forEach(([label, value]) => {
      doc.setTextColor(...hexToRgb(TEXT_MUTED));
      doc.text(label, margin, yPos);
      doc.setTextColor(...hexToRgb(TEXT_DARK));
      doc.text(value, margin + 50, yPos);
      yPos += 6;
    });

    yPos += 5;

    // Verificar se precisa de nova página
    if (yPos > 250) {
      doc.addPage();
      yPos = margin;
    }

    // Informações de Trabalho
    doc.setFontSize(14);
    doc.setFont('helvetica', 'bold');
    doc.setTextColor(...hexToRgb(TEXT_DARK));
    doc.text('Informações de Trabalho', margin, yPos);
    yPos += 8;

    doc.setFontSize(10);
    doc.setFont('helvetica', 'normal');
    
    const infoTrabalho = [
      ['Modelo:', detalhes.trabalho.modelo],
      ['Salário:', formatSalario(detalhes.trabalho.salario)],
      ['Saldo de Horas:', formatSaldoHoras(detalhes.trabalho.saldoHoras)],
      ['Tempo de Casa:', detalhes.trabalho.tempoCasa],
      ['Data de Admissão:', formatData(detalhes.trabalho.dataAdmissao)],
      ['Status:', detalhes.trabalho.status],
    ];

    infoTrabalho.forEach(([label, value]) => {
      doc.setTextColor(...hexToRgb(TEXT_MUTED));
      doc.text(label, margin, yPos);
      doc.setTextColor(...hexToRgb(TEXT_DARK));
      doc.text(value, margin + 50, yPos);
      yPos += 6;
    });

    yPos += 5;

    // Verificar se precisa de nova página
    if (yPos > 250) {
      doc.addPage();
      yPos = margin;
    }

    // Informações Financeiras
    doc.setFontSize(14);
    doc.setFont('helvetica', 'bold');
    doc.setTextColor(...hexToRgb(TEXT_DARK));
    doc.text('Informações Financeiras', margin, yPos);
    yPos += 8;

    doc.setFontSize(10);
    doc.setFont('helvetica', 'normal');
    
    const infoFinanceiras = [
      ['Banco:', detalhes.financeiro.banco],
      ['Agência:', detalhes.financeiro.agencia],
      ['Conta:', detalhes.financeiro.conta],
      ['Tipo de Conta:', detalhes.financeiro.tipoConta],
    ];

    infoFinanceiras.forEach(([label, value]) => {
      doc.setTextColor(...hexToRgb(TEXT_MUTED));
      doc.text(label, margin, yPos);
      doc.setTextColor(...hexToRgb(TEXT_DARK));
      doc.text(value, margin + 50, yPos);
      yPos += 6;
    });

    // Rodapé
    const totalPages = doc.getNumberOfPages();
    for (let i = 1; i <= totalPages; i++) {
      doc.setPage(i);
      doc.setFontSize(8);
      doc.setTextColor(...hexToRgb(TEXT_MUTED));
      doc.text(
        `Página ${i} de ${totalPages}`,
        pageWidth - margin - 30,
        doc.internal.pageSize.getHeight() - 10
      );
      doc.text(
        `Gerado em ${new Date().toLocaleDateString('pt-BR')}`,
        margin,
        doc.internal.pageSize.getHeight() - 10
      );
    }

    // Salvar PDF
    const fileName = `CV_${detalhes.nome.replace(/\s+/g, '_')}_${detalhes.codColaborador}.pdf`;
    doc.save(fileName);
  } catch (error) {
    console.error('Erro ao gerar PDF do CV:', error);
    throw new Error('Erro ao gerar PDF do CV. Tente novamente.');
  }
};

