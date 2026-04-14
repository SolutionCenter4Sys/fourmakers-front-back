export interface Vaga {
  id: string | number;
  codigo?: number;
  titulo: string;
  descricao?: string;
  dataCriacao: string;
  dataUltimaAlteracao?: string;
  dataEnd?: string;
  status: string;
  statusVagaCod?: string;
  nomeCliente?: string;
  unidadeCliente?: string | null;
  nomeGestor?: string;
  nomeUsuarioAlterador?: string | null;
  nomeRecrutadorVaga?: string | null;
  nomeUsuarioCriador?: string;
  recrutadorVaga?: {
    id: number;
    nome: string;
  };
  totalPerfis?: number;
  totalAderentes?: number;
  numeroDeVagas?: number;
  /** Quantidade de candidatos por estágio (soma das quantidades = total de candidatos inscritos) */
  quantidadeCandidatosPorEstagio?: Array<{ idStatus?: number; descricaoStatus?: string; quantidade?: number }>;
  /** ID do perfil gerador (gestor externo perfil) para edição na tela de perfil de atuação */
  idPerfilGerador?: string;
}

export interface PerfilVaga {
  id: number;
  nome: string;
  email?: string;
  telefone?: string;
  status?: string;
  vagaId?: number;
}

export interface StatusVaga {
  id: string;
  nome: string;
  ativo: boolean;
  ordem?: number;
}
