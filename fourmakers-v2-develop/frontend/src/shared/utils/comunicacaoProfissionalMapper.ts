import type {
  ComunicacaoProfissionalApiItem,
  Professional,
} from '@domain/entities/comunicacao';

/**
 * Converte dataAniversario "DD/MM" da API para birthDate "MM-DD" (compatível com formatBirthDate/getMonthDay da view).
 */
function dataAniversarioToBirthDate(dataAniversario: string | null): string | undefined {
  if (!dataAniversario || dataAniversario.trim() === '') return undefined;
  const parts = dataAniversario.trim().split('/');
  if (parts.length !== 2) return undefined;
  const [dd, mm] = parts;
  if (!dd || !mm) return undefined;
  return `${mm.padStart(2, '0')}-${dd.padStart(2, '0')}`;
}

/**
 * Mapeia um item da API Profissionais para Professional (domínio).
 */
export function profissionalApiToProfessional(
  item: ComunicacaoProfissionalApiItem,
): Professional {
  const position = item.cargo?.trim() || item.codCargo?.trim() || '-';
  const unit = item.departamento?.trim() || item.codDepartamento?.trim() || '-';
  return {
    id: item.codigoColaboradorInterno,
    name: item.nomeCompleto ?? '',
    position,
    unit,
    avatar: item.urlFoto && item.urlFoto.trim() !== '' ? item.urlFoto : undefined,
    managerName: item.supervisor ?? undefined,
    about: item.sobre ?? undefined,
    email: item.email ?? undefined,
    phone: item.telefone ?? undefined,
    birthDate: dataAniversarioToBirthDate(item.dataAniversario),
    favoritado: item.favoritado ?? false,
  };
}
