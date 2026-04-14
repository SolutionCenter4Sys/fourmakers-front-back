export interface FuncionalidadeSistemaItem {
  id: number;
  descricao: string;
  ativo: boolean;
  dataCriacao: string;
  dataAlteracao: string;
}

export interface ListarFuncionalidadesSistemaResponse {
  retorno?: FuncionalidadeSistemaItem[];
  sucesso?: boolean;
  mensagem?: string | null;
  erros?: string[] | null;
}
