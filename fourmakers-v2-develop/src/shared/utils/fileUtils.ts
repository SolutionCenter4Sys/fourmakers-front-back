// Mapeamento de extensões para tipos conforme ArquivoTipoEnum
const extensaoParaTipo: Record<string, number> = {
  pdf: 1,
  png: 2,
  jpg: 3,
  jpeg: 4,
  csv: 5,
  xlsx: 6,
};

/**
 * Obtém o tipo de arquivo baseado na extensão conforme ArquivoTipoEnum
 * @param fileName Nome do arquivo com extensão
 * @returns Número correspondente ao tipo de arquivo (default: 1 para PDF)
 */
export const getArquivoTipo = (fileName: string): number => {
  const extensao = fileName.split(".").pop()?.toLowerCase() || "";
  return extensaoParaTipo[extensao] || 1; // Default para PDF (1) se não encontrar
};

/**
 * Converte um arquivo para base64
 * @param file Arquivo a ser convertido
 * @returns Promise com a string base64 do arquivo (sem o prefixo data:)
 */
export const fileToBase64 = (file: File): Promise<string> => {
  return new Promise((resolve, reject) => {
    const reader = new FileReader();
    reader.readAsDataURL(file);
    reader.onload = () => {
      const result = reader.result as string;
      const base64 = result.split(",")[1];
      resolve(base64);
    };
    reader.onerror = (error) => reject(error);
  });
};

