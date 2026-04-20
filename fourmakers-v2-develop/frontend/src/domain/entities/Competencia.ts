export interface Skill {
  id: number;
  descricao: string;
  tipo: string;
}

export interface Nivel {
  id: number;
  descricao: string;
  tipo: string;
}

export interface ListarSkillsSumarioParams {
  cursor?: number;
  limite?: number;
  descricao?: string;
}

export interface ListarSkillsSumarioResponse {
  skills: Skill[];
  niveis: Nivel[];
  sucesso: boolean;
  mensagem: string | null;
  erros: string[] | null;
}

export interface SkillColaborador {
  id: number;
  descricao: string;
  nivelId: number;
  tipo: string;
  nivel: string;
  codigoInternoColaborador: string;
}

export interface ListarTodasSkillsColaboradorParams {
  codigoInternoColaborador: string;
}

export interface ListarTodasSkillsColaboradorResponse {
  retorno: SkillColaborador[];
  sucesso?: boolean;
  mensagem?: string | null;
  erros?: string[] | null;
}

/** Body da API SumarioCompetenciasListarColaboradores (contorno até o back retornar usuarioId na listagem). */
export interface SumarioCompetenciasListarColaboradoresParams {
  unidadeid: string[];
  perfilId: unknown[];
  Competencia: unknown[];
  cpf: string[];
  limite: number;
  cursor: number;
}

export interface ColaboradorSumarioItem {
  id: number;
  nomeCompleto: string;
  cpf: string;
  unidade?: string;
  imagePath?: string;
}

export interface SumarioCompetenciasListarColaboradoresResponse {
  listaColaboradoresSumario: ColaboradorSumarioItem[];
  sucesso: boolean;
  mensagem: string | null;
  erros: unknown;
}
