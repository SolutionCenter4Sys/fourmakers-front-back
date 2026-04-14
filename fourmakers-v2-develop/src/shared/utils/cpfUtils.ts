/**
 * Utilitário para validação e formatação de CPF
 */

/**
 * Valida se um CPF é válido verificando os dígitos verificadores
 * @param cpf CPF com ou sem formatação
 * @returns true se o CPF é válido, false caso contrário
 */
export const validarCPF = (cpf: string): boolean => {
  // Remove formatação
  const cpfLimpo = cpf.replace(/\D/g, '');
  
  // Verifica se tem 11 dígitos
  if (cpfLimpo.length !== 11) return false;
  
  // Verifica se todos os dígitos são iguais (CPF inválido)
  if (/^(\d)\1{10}$/.test(cpfLimpo)) return false;
  
  // Valida primeiro dígito verificador
  let soma = 0;
  for (let i = 0; i < 9; i++) {
    soma += parseInt(cpfLimpo.charAt(i)) * (10 - i);
  }
  let digito = 11 - (soma % 11);
  if (digito >= 10) digito = 0;
  if (digito !== parseInt(cpfLimpo.charAt(9))) return false;
  
  // Valida segundo dígito verificador
  soma = 0;
  for (let i = 0; i < 10; i++) {
    soma += parseInt(cpfLimpo.charAt(i)) * (11 - i);
  }
  digito = 11 - (soma % 11);
  if (digito >= 10) digito = 0;
  if (digito !== parseInt(cpfLimpo.charAt(10))) return false;
  
  return true;
};

/**
 * Formata um CPF adicionando pontos e hífen
 * @param value Valor do CPF (com ou sem formatação)
 * @returns CPF formatado (000.000.000-00)
 */
export const formatCPF = (value: string): string => {
  const numbers = value.replace(/\D/g, '');
  if (numbers.length <= 11) {
    return numbers
      .replace(/(\d{3})(\d)/, '$1.$2')
      .replace(/(\d{3})(\d)/, '$1.$2')
      .replace(/(\d{3})(\d{1,2})$/, '$1-$2');
  }
  return value;
};
