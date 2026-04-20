/**
 * Formata data enquanto o usuário digita no formato dd/mm/yyyy
 * @param valor Valor digitado pelo usuário
 * @returns String formatada no padrão dd/mm/yyyy
 */
export const formatarDataInput = (valor: string): string => {
  // Remove tudo que não é número
  const apenasNumeros = valor.replace(/\D/g, "");
  // Aplica a máscara
  if (apenasNumeros.length <= 2) {
    return apenasNumeros;
  } else if (apenasNumeros.length <= 4) {
    return `${apenasNumeros.slice(0, 2)}/${apenasNumeros.slice(2)}`;
  } else {
    return `${apenasNumeros.slice(0, 2)}/${apenasNumeros.slice(2, 4)}/${apenasNumeros.slice(4, 8)}`;
  }
};

/**
 * Formata telefone baseado no país selecionado
 * @param value Valor digitado
 * @param pais País selecionado (brasil, eua, portugal, etc)
 * @returns String formatada do telefone
 */
export const formatPhoneByCountry = (value: string, pais: string): string => {
  const digits = value.replace(/\D/g, "");
  switch (pais) {
    case "brasil":
      if (digits.length <= 2) return digits.length ? `(${digits}` : "";
      if (digits.length <= 7) return `(${digits.slice(0, 2)}) ${digits.slice(2)}`;
      return `(${digits.slice(0, 2)}) ${digits.slice(2, 7)}-${digits.slice(7, 11)}`;
    default:
      return digits;
  }
};

/**
 * Formata código postal baseado no país selecionado
 * @param value Valor digitado
 * @param pais País selecionado (brasil, eua, portugal, etc)
 * @returns String formatada do código postal
 */
export const formatPostalCodeByCountry = (value: string, pais: string): string => {
  const digits = value.replace(/\D/g, "");
  switch (pais) {
    case "brasil":
      if (digits.length <= 5) return digits;
      return `${digits.slice(0, 5)}-${digits.slice(5, 8)}`;
    default:
      return digits;
  }
};

/**
 * Converte data no formato dd/mm/yyyy para ISO string com horário zerado
 * @param dateString Data no formato dd/mm/yyyy
 * @returns String ISO com horário 00:00:00.000Z
 */
export const dateToISOWithZeroTime = (dateString: string): string => {
  if (!dateString || dateString.trim() === "") {
    return new Date().toISOString().split("T")[0] + "T00:00:00.000Z";
  }
  const [dia, mes, ano] = dateString.split("/").map(Number);
  if (dia && mes && ano) {
    const date = new Date(ano, mes - 1, dia);
    // Formatar como ISO string com horário zerado
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, "0");
    const day = String(date.getDate()).padStart(2, "0");
    return `${year}-${month}-${day}T00:00:00.000Z`;
  }
  return new Date().toISOString().split("T")[0] + "T00:00:00.000Z";
};

/**
 * Retorna data atual formatada como ISO string com horário zerado
 * @returns String ISO com horário 00:00:00.000Z
 */
export const getCurrentDateWithZeroTime = (): string => {
  return new Date().toISOString().split("T")[0] + "T00:00:00.000Z";
};

/**
 * Valida data no formato dd/mm/yyyy
 * @param dateString Data no formato dd/mm/yyyy
 * @returns true se a data é válida, false caso contrário
 */
export const validateDate = (dateString: string): boolean => {
  if (!dateString || dateString.trim() === "") return false;
  const dateRegex = /^(\d{2})\/(\d{2})\/(\d{4})$/;
  const match = dateString.match(dateRegex);
  if (!match) return false;
  
  const [, dia, mes, ano] = match.map(Number);
  if (dia < 1 || dia > 31 || mes < 1 || mes > 12 || ano < 1900 || ano > 2100) {
    return false;
  }
  
  const date = new Date(ano, mes - 1, dia);
  return (
    date.getDate() === dia &&
    date.getMonth() === mes - 1 &&
    date.getFullYear() === ano
  );
};

/**
 * Extrai o DDI do prefixo do telefone (remove o símbolo "+")
 * @param prefix Prefixo do telefone (ex: "+55", "+1", "+351")
 * @returns DDI sem o símbolo "+" ou string vazia se não houver prefixo
 */
export const extractDDIFromPrefix = (prefix: string): string => {
  if (!prefix) return "";
  return prefix.replace("+", "") || "";
};

/**
 * Remove todos os símbolos do telefone, deixando apenas números
 * @param telefone Telefone formatado (ex: "(11) 98765-4321")
 * @returns Telefone apenas com números (ex: "11987654321")
 */
export const cleanPhoneNumber = (telefone: string): string => {
  if (!telefone) return "";
  return telefone.replace(/\D/g, "");
};

/**
 * Extrai apenas a parte da data (sem hora) de uma string de data do backend
 * Remove hora, minutos, segundos e timezone, retornando apenas a data
 * @param dateString Data do backend no formato "10/30/2024 03:00:00" ou "2024-10-30T03:00:00"
 * @returns Apenas a parte da data (ex: "10/30/2024" ou "2024-10-30") ou string vazia se inválida
 */
export const extractDateOnly = (dateString: string | null | undefined): string => {
  if (!dateString) return '';
  // Se contém espaço, pegar apenas a parte da data (formato "10/30/2024 03:00:00")
  if (dateString.includes(' ')) {
    return dateString.split(' ')[0];
  }
  // Se contém 'T', pegar apenas a parte da data (formato ISO "2024-10-30T03:00:00")
  if (dateString.includes('T')) {
    return dateString.split('T')[0];
  }
  // Se não tem separador de hora, retornar como está
  return dateString;
};

/**
 * Formata data do backend para exibição no formato brasileiro (DD/MM/YYYY)
 * A data vem do backend no formato "10/30/2024 03:00:00" (MM/DD/YYYY HH:mm:ss)
 * @param dateString Data do backend ou null
 * @returns Data formatada no padrão brasileiro (DD/MM/YYYY) ou '-' se inválida
 */
export const formatDateFromBackend = (dateString: string | null): string => {
  if (!dateString) return '-';
  try {
    // Extrair apenas a parte da data (sem hora)
    const dataPart = extractDateOnly(dateString);
    
    // Se está no formato MM/DD/YYYY, converter para Date corretamente
    if (dataPart.includes('/')) {
      const partes = dataPart.split('/');
      // Verificar se é MM/DD/YYYY (primeiro número > 12 indica DD/MM/YYYY)
      const primeiroNumero = parseInt(partes[0], 10);
      if (primeiroNumero > 12) {
        // Formato DD/MM/YYYY
        const [dia, mes, ano] = partes;
        const date = new Date(parseInt(ano, 10), parseInt(mes, 10) - 1, parseInt(dia, 10));
        return date.toLocaleDateString('pt-BR');
      } else {
        // Formato MM/DD/YYYY (padrão do backend)
        const [mes, dia, ano] = partes;
        const date = new Date(parseInt(ano, 10), parseInt(mes, 10) - 1, parseInt(dia, 10));
        return date.toLocaleDateString('pt-BR');
      }
    } else if (dataPart.includes('T')) {
      // Se está no formato ISO, usar diretamente
      const date = new Date(dataPart);
      return date.toLocaleDateString('pt-BR');
    } else {
      // Tentar parsear como Date padrão
      const date = new Date(dateString);
      return date.toLocaleDateString('pt-BR');
    }
  } catch {
    // Se falhar, retornar apenas a parte da data sem hora
    return extractDateOnly(dateString) || '-';
  }
};

/**
 * Converte data do backend para formato de input (YYYY-MM-DD)
 * A data vem do backend no formato "10/30/2024 03:00:00" (MM/DD/YYYY HH:mm:ss)
 * @param dateString Data do backend
 * @returns Data no formato YYYY-MM-DD ou string vazia se inválida
 */
export const convertBackendDateToInputFormat = (dateString: string | null | undefined): string => {
  if (!dateString) return '';
  try {
    // Extrair apenas a parte da data (sem hora)
    const dataPart = extractDateOnly(dateString);
    
    // Se está no formato MM/DD/YYYY, converter para YYYY-MM-DD
    // Prefer handling explicit ISO-like formats first to avoid Date/timezone parsing
    const trimmed = dataPart.trim();

    // If ISO-like (YYYY-MM-DD or YYYY-MM-DDTHH:MM:SS...), extract the leading date portion
    const isoMatch = trimmed.match(/^(\d{4})-(\d{2})-(\d{2})/);
    if (isoMatch) {
      return `${isoMatch[1]}-${isoMatch[2]}-${isoMatch[3]}`;
    }

    // If contains slashes, likely MM/DD/YYYY or DD/MM/YYYY (or with time after a space)
    if (trimmed.includes('/')) {
      const primeiraParte = trimmed.split(' ')[0];
      const partes = primeiraParte.split('/');
      if (partes.length === 3) {
        const primeiroNumero = parseInt(partes[0], 10);
        // If first number > 12 it's DD/MM/YYYY, otherwise treat as MM/DD/YYYY
        if (primeiroNumero > 12) {
          const [dia, mes, ano] = partes;
          return `${ano}-${mes.padStart(2, '0')}-${dia.padStart(2, '0')}`;
        } else {
          const [mes, dia, ano] = partes;
          return `${ano}-${mes.padStart(2, '0')}-${dia.padStart(2, '0')}`;
        }
      }
    }

    // Fallback: try to capture numeric groups like YYYYMMDD or YYYY.MM.DD
    const fallbackMatch = trimmed.match(/(\d{4}).?(\d{1,2}).?(\d{1,2})/);
    if (fallbackMatch) {
      const y = fallbackMatch[1];
      const m = String(fallbackMatch[2]).padStart(2, '0');
      const d = String(fallbackMatch[3]).padStart(2, '0');
      return `${y}-${m}-${d}`;
    }

    return '';
  } catch {
    return '';
  }
};

/**
 * Formata valor monetário enquanto o usuário digita no formato brasileiro (R$ 0,00)
 * @param valor Valor digitado pelo usuário (pode estar formatado ou não)
 * @returns String formatada no padrão R$ 0.000,00
 */
export const formatarValorMonetario = (valor: string): string => {
  // Remove tudo que não é número
  const apenasNumeros = valor.replace(/\D/g, '');
  
  // Se estiver vazio, retorna vazio
  if (apenasNumeros === '') return '';
  
  // Converte para número dividindo por 100 (centavos)
  const numero = parseFloat(apenasNumeros) / 100;
  
  // Formata como moeda brasileira
  return numero.toLocaleString('pt-BR', {
    style: 'currency',
    currency: 'BRL',
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  });
};

/**
 * Remove formatação monetária e retorna apenas o valor numérico
 * @param valor Valor formatado (ex: "R$ 1.234,56")
 * @returns Valor numérico como string (ex: "1234.56")
 */
export const desformatarValorMonetario = (valor: string): string => {
  // Remove tudo que não é número ou ponto
  const apenasNumeros = valor.replace(/[^\d,]/g, '').replace(',', '.');
  
  if (apenasNumeros === '' || apenasNumeros === '.') return '';
  
  // Converte para número
  const numero = parseFloat(apenasNumeros);
  
  if (isNaN(numero)) return '';
  
  return numero.toString();
};

/**
 * Formata uma data para o formato YYYY-MM-DD.
 * Aceita vários formatos de entrada e converte para YYYY-MM-DD.
 * Se a data já estiver nesse formato, retorna-a diretamente.
 * Se a data for inválida ou vazia, retorna string vazia.
 * @param dateString A string da data a ser formatada (pode ser YYYY-MM-DD, DD/MM/YYYY, MM/DD/YYYY, ou ISO string)
 * @returns A data formatada como YYYY-MM-DD ou string vazia se inválida
 */
export const formatarDataParaYYYYMMDD = (dateString: string): string => {
  if (!dateString || dateString.trim() === '') return '';

  // Verifica se já está no formato YYYY-MM-DD
  if (/^\d{4}-\d{2}-\d{2}$/.test(dateString)) {
    return dateString;
  }

  try {
    // Tentar parsear como Date
    let date: Date;
    
    // Se contém barras, pode ser DD/MM/YYYY ou MM/DD/YYYY
    if (dateString.includes('/')) {
      const partes = dateString.split('/');
      if (partes.length === 3) {
        const primeiroNumero = parseInt(partes[0], 10);
        // Se primeiro número > 12, provavelmente é DD/MM/YYYY
        if (primeiroNumero > 12) {
          const [dia, mes, ano] = partes;
          date = new Date(parseInt(ano, 10), parseInt(mes, 10) - 1, parseInt(dia, 10));
        } else {
          // Caso contrário, assume MM/DD/YYYY
          const [mes, dia, ano] = partes;
          date = new Date(parseInt(ano, 10), parseInt(mes, 10) - 1, parseInt(dia, 10));
        }
      } else {
        return '';
      }
    } else {
      // Tentar parsear diretamente
      date = new Date(dateString);
    }

    // Verificar se a data é válida
    if (isNaN(date.getTime())) {
      return '';
    }

    // Formatar como YYYY-MM-DD
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    
    return `${year}-${month}-${day}`;
  } catch (error) {
    console.error('Erro ao formatar data para YYYY-MM-DD:', error);
    return '';
  }
};
