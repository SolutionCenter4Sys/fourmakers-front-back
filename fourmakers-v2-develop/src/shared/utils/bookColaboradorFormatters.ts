export const formatSaldoHoras = (horas: number): string => {
  const horasInt = Math.floor(Math.abs(horas));
  const minutos = Math.floor((Math.abs(horas) - horasInt) * 60);
  const sinal = horas < 0 ? '-' : '+';
  return `${sinal}${horasInt}h ${minutos}m`;
};

export const formatSalario = (valor: number): string => {
  return new Intl.NumberFormat('pt-BR', {
    style: 'currency',
    currency: 'BRL',
  }).format(valor);
};

export const formatData = (data: string): string => {
  if (!data) return '-';
  try {
    const date = new Date(data);
    return date.toLocaleDateString('pt-BR');
  } catch {
    return data;
  }
};

export const formatTelefone = (telefone: string): string => {
  if (!telefone) return '-';
  const cleanPhone = telefone.replace(/\D/g, '');
  
  if (cleanPhone.length === 11) {
    return cleanPhone.replace(/(\d{2})(\d{5})(\d{4})/, '($1) $2-$3');
  }
  
  if (cleanPhone.length === 10) {
    return cleanPhone.replace(/(\d{2})(\d{4})(\d{4})/, '($1) $2-$3');
  }
  
  return telefone;
};

export const formatTempoCasaSimples = (tempo: string): string => {
  return tempo || '-';
};

