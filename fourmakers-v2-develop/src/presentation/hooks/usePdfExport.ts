
import { jsPDF } from 'jspdf';
import autoTable from 'jspdf-autotable';
import type { SimulationData, CalculationResult } from '@domain/entities/SimulatorTypes';
import { formatCurrency } from '@shared/utils/calculations';
import { trackEvent } from '@shared/utils/analytics';

const LOGO_DARK = "https://storage.googleapis.com/flutterflow-io-6f20.appspot.com/teams/tSBuz6sMYZEZ5mtSVNyg/assets/kmsewlynomr4/logo_fourmakers_white.png";

const BRAND_PRIMARY = '#9A1BFF';
const BRAND_PRIMARY_STRONG = '#7B1CE5';
// const BRAND_ACCENT = '#18C964'; // Não utilizado no momento
const BRAND_ACCENT_SOFT = '#E6FDF1';
const SURFACE_SOFT = '#F4EBFF';
const SURFACE_BASE = '#F0F5FA';
const TEXT_DARK = '#0F172A';
const BORDER_SOFT = '#E2E8F0';

const hexToRgb = (hex: string): [number, number, number] => {
  const clean = hex.replace('#', '');
  const bigint = parseInt(clean, 16);
  return [(bigint >> 16) & 255, (bigint >> 8) & 255, bigint & 255];
};

export const usePdfExport = () => {
  const generatePDF = async (data: SimulationData, results: CalculationResult, type: 'proposed' | 'desired', viewMode: 'recruiter' | 'collaborator') => {
    const startTime = performance.now();
    const titleSuffix = type === 'proposed' ? 'Proposta' : 'Pretendida';
    
    const pdfData = viewMode === 'recruiter' 
        ? { ...data, variableExpenses: [] } 
        : data;

    trackEvent('export_pdf', { status: 'start', section: 'Footer', candidate: pdfData.candidateName, type });

    try {
        const doc = new jsPDF();
        const headerHeight = 42;

        // Header roxo brand
        doc.setFillColor(...hexToRgb(BRAND_PRIMARY));
        doc.rect(0, 0, 210, headerHeight, 'F');
        
        try {
            const logoUrl = LOGO_DARK;
            const response = await fetch(logoUrl);
            const blob = await response.blob();
            const base64 = await new Promise<string>((resolve) => {
                const reader = new FileReader();
                reader.onloadend = () => resolve(reader.result as string);
                reader.readAsDataURL(blob);
            });
            
            const imgProps = await new Promise<{width: number, height: number, ratio: number}>((resolve) => {
                const img = new Image();
                img.onload = () => {
                    resolve({ width: img.width, height: img.height, ratio: img.width / img.height });
                };
                img.src = base64;
            });

            const pdfLogoHeight = 12; 
            const pdfLogoWidth = pdfLogoHeight * imgProps.ratio;
            const pdfLogoY = (headerHeight - pdfLogoHeight) / 2;
            const pdfLogoX = 210 - 14 - pdfLogoWidth;

            doc.addImage(base64, 'PNG', pdfLogoX, pdfLogoY, pdfLogoWidth, pdfLogoHeight); 
        } catch (imgErr) {
            console.error("Failed to load logo for PDF", imgErr);
        }

        doc.setTextColor(255, 255, 255);
        doc.setFontSize(22);
        doc.text(`Remuneração Salarial ${titleSuffix}`, 14, 24);
        doc.setFontSize(10);
        doc.text('Simulação Fourmakers', 14, 32);

        let yPos = 52;

        doc.setFillColor(...hexToRgb(SURFACE_BASE));
        doc.roundedRect(10, yPos - 8, 190, 24, 3, 3, 'F');
        doc.setTextColor(...hexToRgb(TEXT_DARK));
        doc.setFontSize(13);
        doc.text(`Candidato: ${pdfData.candidateName || 'N/A'}`, 14, yPos);
        doc.setFontSize(11);
        doc.text(`Cargo: ${pdfData.role}`, 14, yPos + 8);
        
        yPos += 26;

        const tableData: any[] = [
          ['Salário Bruto', formatCurrency(pdfData.grossSalary)],
          ['INSS (Estimado)', `-${formatCurrency(results.inss)}`],
          ['IRRF (Estimado)', `-${formatCurrency(results.irrf)}`],
          ['Mobilidade', formatCurrency(results.mobilityCost)],
          ['Outros Benefícios', formatCurrency(results.monthlyBenefits - results.mobilityCost)],
          ['Salário Líquido em Conta', { content: formatCurrency(results.netSalary), styles: { fontStyle: 'bold' as const, fillColor: hexToRgb(BRAND_ACCENT_SOFT), textColor: hexToRgb(TEXT_DARK) } }],
          ['Ganho Real Mensal (Liq + Ben)', { content: formatCurrency(results.netSalary + results.monthlyBenefits), styles: { fontStyle: 'bold' as const, fillColor: hexToRgb(SURFACE_SOFT), textColor: hexToRgb(TEXT_DARK) } }],
        ];

        if (viewMode === 'collaborator' && results.totalVariableExpenses > 0) {
             tableData.splice(3, 0, ['Gastos Variáveis', { content: `-${formatCurrency(results.totalVariableExpenses)}`, styles: { textColor: [200, 50, 50] as [number, number, number] } }]);
        }

        autoTable(doc, {
          startY: yPos,
          head: [['Descrição', 'Valor']],
          body: tableData,
          theme: 'grid',
          headStyles: { fillColor: hexToRgb(BRAND_PRIMARY_STRONG), textColor: [255, 255, 255] }, 
          styles: { fontSize: 11, cellPadding: 3, lineColor: hexToRgb(BORDER_SOFT), lineWidth: 0.2, textColor: hexToRgb(TEXT_DARK) },
          alternateRowStyles: { fillColor: hexToRgb('#F9FAFB') },
        });
        
        if (viewMode === 'collaborator' && results.totalVariableExpenses > 0) {
            yPos = (doc as any).lastAutoTable.finalY + 10;
            doc.setFontSize(12);
            doc.text('Detalhamento Gastos Variáveis (Descontos)', 14, yPos);
            yPos += 5;
            
            const varExpensesData = pdfData.variableExpenses.map(v => [v.description || 'Sem descrição', `-${formatCurrency(v.value)}`]);
            
            autoTable(doc, {
                startY: yPos,
                head: [['Item', 'Valor']],
                body: varExpensesData,
                theme: 'plain',
                headStyles: { fillColor: hexToRgb(SURFACE_SOFT), textColor: hexToRgb(TEXT_DARK) },
                styles: { fontSize: 10, textColor: hexToRgb(TEXT_DARK), lineColor: hexToRgb(BORDER_SOFT), lineWidth: 0.2 }
            });
        }

        yPos = (doc as any).lastAutoTable.finalY + 15;
        if (yPos > 250) { doc.addPage(); yPos = 20; }
        
        doc.setFontSize(14);
        doc.text('Previsão de Recebimentos (13 Meses)', 14, yPos);
        yPos += 5;
        
        const forecastData = results.forecast.map(f => [
            f.month,
            f.note || '-',
            formatCurrency(f.salaryPayment),
            formatCurrency(f.benefitsPayment),
            formatCurrency(f.total)
        ]);

        autoTable(doc, {
            startY: yPos,
            head: [['Mês', 'Obs', 'Líquido em Conta', 'Benefícios', 'Total']],
            body: forecastData,
            theme: 'striped',
            headStyles: { fillColor: hexToRgb(BRAND_PRIMARY), textColor: [255, 255, 255] },
            styles: { textColor: hexToRgb(TEXT_DARK), lineColor: hexToRgb(BORDER_SOFT), lineWidth: 0.2, fontSize: 10 },
            columnStyles: { 0: { cellWidth: 35 }, 1: { cellWidth: 40 } },
            alternateRowStyles: { fillColor: hexToRgb('#F9FAFB') },
        });

        doc.save(`Proposta_${titleSuffix}_${pdfData.candidateName || 'Candidato'}.pdf`);

        const duration = performance.now() - startTime;
        trackEvent('export_pdf', { status: 'success', duration_ms: Math.round(duration), section: 'Footer' });
    } catch (e) {
        console.error(e);
        trackEvent('export_pdf', { status: 'error', error: String(e), section: 'Footer' });
    }
  };

  return { generatePDF };
};
