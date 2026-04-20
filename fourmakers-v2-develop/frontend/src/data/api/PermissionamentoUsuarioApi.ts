import { httpClient } from './httpClient';

/**
 * Resposta mínima do endpoint SumarioCompetenciasListarColaboradores
 * usada apenas para obter usuarioId por CPF (contorno até o back retornar usuarioId na listagem).
 * Endpoint de competências; duplicamos a chamada aqui para não depender da feature de competências.
 */
interface RespostaListarColaboradoresSumario {
  listaColaboradoresSumario?: { id: number }[];
  sucesso?: boolean;
  mensagem?: string | null;
  erros?: unknown;
}

/**
 * Obtém o usuarioId do colaborador a partir do CPF.
 * Contorno: a listagem de colaboradores (ColaboradoresApi) não retorna usuarioId;
 * esta chamada usa o mesmo endpoint do sumário de competências apenas para obter o id.
 * Manter dentro da feature permissionamento para não impactar a feature de competências.
 *
 * @returns usuarioId ou null se não encontrado
 */
export async function obterUsuarioIdPorCpf(
  token: string,
  cpf: string
): Promise<number | null> {
  const res = await httpClient.post<RespostaListarColaboradoresSumario>(
    '/api/Competencia/SumarioCompetenciasListarColaboradores',
    {
      unidadeid: ['0'],
      perfilId: [],
      Competencia: [],
      cpf: [cpf],
      limite: 50,
      cursor: 0,
    },
    { token, headers: { 'Content-Type': 'application/json' } }
  );
  const primeiro = res?.listaColaboradoresSumario?.[0];
  const id = primeiro?.id;
  if (id == null || !Number.isFinite(id)) return null;
  return id;
}
