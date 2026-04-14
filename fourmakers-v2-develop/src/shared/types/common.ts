export type StatusType = 
  | "Aprovado" 
  | "Aprovado Parcial" 
  | "Pendente" 
  | "Rejeitado" 
  | "Cancelado" 
  | "Proposta" 
  | "Desenvolvimento";

export interface StatusColorMap {
  [key: string]: string;
}

export interface FilterOptions {
  searchTerm?: string;
  startDate?: Date;
  endDate?: Date;
  status?: string;
}

export interface PaginationState {
  currentPage: number;
  itemsPerPage: number;
  totalItems: number;
}
