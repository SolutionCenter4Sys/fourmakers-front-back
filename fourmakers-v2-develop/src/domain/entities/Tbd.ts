export interface Tbd {
  codTbdAlocado: number;
  descricao: string;
  cpfGestor: string | null;
  codDiretoria: string;
  descricaoDiretoria: string;
  codDepartamento: string;
  descricaoDepartamento: string;
  codGestor: string | null;
  orgId: number;
  dataCriacao: string;
  dataAlteracao: string;
}

export interface InserirTbdPayload {
  codGestor: string;
  descricao: string;
  codDiretoria: string;
  descricaoDiretoria: string;
  codDepartamento: string;
  descricaoDepartamento: string;
}

export interface InserirTbdResponse {
  codTbdAlocado: number;
  descricao: string;
  cpfGestor: string | null;
  codDiretoria: string;
  descricaoDiretoria: string;
  codDepartamento: string;
  descricaoDepartamento: string;
  codGestor: string | null;
  orgId: number;
  dataCriacao: string;
  dataAlteracao: string;
}

export interface AtualizarTbdPayload {
  codGestor: string;
  codTbdAlocado: number;
  descricao: string;
  codDiretoria: string;
  descricaoDiretoria: string;
  codDepartamento: string;
  descricaoDepartamento: string;
}

export interface AtualizarTbdResponse {
  codTbdAlocado: number;
  descricao: string;
  cpfGestor: string | null;
  codDiretoria: string;
  descricaoDiretoria: string;
  codDepartamento: string;
  descricaoDepartamento: string;
  codGestor: string | null;
  orgId: number;
  dataCriacao: string;
  dataAlteracao: string;
}
