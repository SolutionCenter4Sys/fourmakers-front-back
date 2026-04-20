export interface GestorProjeto {
  nomeGestor: string;
  codGestor: string;
}

export interface ProjetoAlocacao {
  codProjeto: string;
  nomeProjeto: string;
  codCliente: string;
  nomeCliente: string;
  labelClienteProjeto: string;
  statusProjeto: string;
  gestoresProjeto: GestorProjeto[];
}

export interface PerfilAlocacao {
  perfil: string;
  id: string;
  skills: string | null;
}

export interface AlocacaoColaboradorTbd {
  colaboradorAlocadoId: string | null;
  periodoAlocadoId: number;
  codDepartamento: string;
  nomeDepartamento: string;
  codColaborador: string;
  nomeColaborador: string;
  cpf: string;
  nomeGestorAdm: string;
  projeto: ProjetoAlocacao;
  dataInicio: string;
  dataFim: string;
  quantidadeDeHoras: number;
  percentual: number;
  tbd: boolean;
  prioridade: boolean;
  oportunidade: string;
  observacao: string;
  retroalimentaCv: boolean;
  incluiFimDeSemana?: boolean | null;
  perfil: PerfilAlocacao | null;
  habilidadesAlocacao: string[];
  habilidadesTecnicas: string[];
}

export interface ListarAlocacoesPayload {
  pesquisa?: string;
  codigoUnidade?: string | null;
  codigoDepartamento?: string | null;
  codigoGestorAdm?: string | null;
  listaCodigoColabOuTbd?: string[];
  filtroTipoProfissional?: number;
  codigoGestorProjeto?: string | null;
  listaCodigoClientes?: string[];
  apenasProjetosPrioritarios?: boolean;
  filtroPrioridade?: number; // 0 = Todos, 1 = Prioritários, 2 = Não prioritários
  listaCodigoProjetos?: string[];
  statusProjeto?: string | null;
}

export interface ListarAlocacoesResponse {
  retorno: AlocacaoColaboradorTbd[];
  sucesso: boolean;
  mensagem: string | null;
  erros: string[] | null;
}

export interface ColaboradorETbd {
  codProfissional: string;
  nomeProfissional: string;
  labelCodigoNome: string;
  cpf: string | null;
  ehTbd: boolean;
}

export interface ListarColaboradoresETbdsParams {
  codigoDiretoria?: number;
  codigoGestor?: number;
  filtroTipoProfissional?: number;
  codigoDepartamento?: string;
}

export interface ListarColaboradoresETbdsResponse {
  retorno: ColaboradorETbd[];
  sucesso?: boolean;
  mensagem?: string | null;
  erros?: string[] | null;
}

export interface ProjetoColaboradorMapa {
  projetos: string;
  codigoProjeto: string;
  codigoCliente: string;
}

export interface ListarProjetosColaboradorMapaParams {
  codigoProfissional?: string;
  ehTbd?: boolean | null;
  codigoGerenteProjeto?: string;
  listaCodigoCliente?: string[];
  status?: string;
  prioritarioFiltro?: number;
}

export interface ListarProjetosColaboradorMapaResponse {
  retorno: ProjetoColaboradorMapa[];
  Projetos?: ProjetoColaboradorMapa[];
  sucesso?: boolean;
  mensagem?: string | null;
  erros?: string[] | null;
}

export interface Gestor {
  codigoProfissional: string;
  nome: string;
}

export interface ListarNomesGestoresParams {
  codigoDiretoria?: number;
  codigoDepartamento?: string;
  codDiretoria?: number;
  codDepartamento?: string;
}

export interface ListarNomesGestoresResponse {
  retorno: Gestor[];
  Gestor?: Gestor[];
  sucesso?: boolean;
  mensagem?: string | null;
  erros?: string[] | null;
}

export interface SubstituirDadosAlocacoesPorPeriodoPayload {
  codigoColaboradorNovo?: string;
  codigoProjetoNovo?: string;
  ehTbd?: boolean;
  periodosIdSubstituidos: number[];
}

export interface SubstituirDadosAlocacoesPorPeriodoResponse {
  sucesso: boolean;
  mensagem: string | null;
  erros: string[] | null;
  retorno?: AlocacaoColaboradorTbd[];
}

export interface EditarAlocacaoPayload {
  periodoAlocacaoId: number;
  dataInicio?: string;
  dataFim?: string;
  quantidadeDeHoras?: number;
  quantidadeHoras?: number; // Mantido para compatibilidade
  percentual?: number;
  prioritario?: number;
  prioridade?: number; // Mantido para compatibilidade
  oportunidade?: string;
  observacao?: string;
  incluiFimDeSemana?: boolean | null;
  flagRetroalimentaCV?: boolean | null;
  idPerfilAlocacao?: string;
  perfilSkills?: PerfilSkill[];
}

export interface EditarAlocacaoResponse {
  AlocacaoEditada: AlocacaoColaboradorTbd;
  sucesso: boolean;
  mensagem: string | null;
  erros: string[] | null;
}

export interface RemoverAlocacoesEmLotePayload {
  idsPeriodoAlocacao: string[];
}

export interface RemoverAlocacoesEmLoteResponse {
  sucesso: boolean;
  mensagem: string | null;
  erros: string[] | null;
}

export interface ExportarRelatorioAlocacoesPayload {
  pesquisa?: string;
  codigoUnidade?: string | null;
  codigoDepartamento?: string | null;
  codigoGestorAdm?: string | null;
  listaCodigoColabOuTbd?: string[];
  filtroTipoProfissional?: number;
  codigoGestorProjeto?: string | null;
  listaCodigoClientes?: string[];
  apenasProjetosPrioritarios?: boolean;
  filtroPrioridade?: number;
  listaCodigoProjetos?: string[];
  statusProjeto?: string | null;
}

export interface PerfilSkill {
  idSkill: number;
  descricaoSkill: string;
  tipoSkill: string;
  senioridade: string;
  nivelId: number;
}

export interface CadastrarMapaAlocacaoPayload {
  cpfColaborador: string;
  codigoTbd: string;
  codigoColaborador: string;
  codigoGestor: string;
  codigoProjeto: string;
  nomeGestor: string;
  nomeProjeto: string;
  dataInicio: string;
  dataFim: string;
  incluiFimDeSemana: boolean;
  quantidadeHoras: number;
  oportunidade: string;
  observacao: string;
  prioritario: number;
  percentual: number;
  flagRetroalimentaCV: boolean;
  idPerfilAlocacao: string;
  nomePerfilAlocacao: string;
  perfilSkills: PerfilSkill[];
}

export interface CadastrarMapaAlocacaoResponse {
  sucesso: boolean;
  mensagem: string | null;
  erros: string[] | null;
}

export interface PerfilAlocacaoItem {
  perfil: string;
  id: string;
  skills: string[] | null;
}

export interface ListarPerfilAlocacaoParams {
  codProjeto?: string;
  ocultarSkill?: boolean;
}

export interface ListarPerfilAlocacaoResponse {
  retorno: PerfilAlocacaoItem[];
  sucesso?: boolean;
  mensagem?: string | null;
  erros?: string[] | null;
}

export interface AlteraPerfilAlocacaoPayload {
  periodoAlocacaoId: number;
  perfilId: string;
}

export interface AlteraPerfilAlocacaoResponse {
  AlocacaoEditada: AlocacaoColaboradorTbd;
  sucesso: boolean;
  mensagem: string | null;
  erros: string[] | null;
}

export interface ItemPerfil {
  descricao: string;
  id: number;
}

export interface SkillPerfil {
  descricao: string;
  id: number;
}

export interface NivelPerfil {
  descricao: string;
  id: number;
}

export interface SkillPerfilGestorExterno {
  itemPerfil: ItemPerfil;
  skill: SkillPerfil;
  nivel: NivelPerfil;
  dataCriacao: string;
  relevante: boolean;
}

export interface ListarSkillsPerfilGestorExternoPorIdParams {
  perfilId: string;
}

export interface ListarSkillsPerfilGestorExternoPorIdResponse {
  retorno: SkillPerfilGestorExterno[];
  sucesso?: boolean;
  mensagem?: string | null;
  erros?: string[] | null;
}

// Visão Gerencial - Resumo
export interface MesResumo {
  ociosidadeMes: string; // Formato "HHHMM:SS"
  forca: number; // Percentual (0-1)
  nomeMesAtual: string; // Ex: "jan./2026"
}

export interface ResumoAlocacao {
  ociosidadeResumo: number; // Percentual (0-1)
  meses: MesResumo[];
}

export interface GetMapaAlocacaoResumoParams {
  trimestral: boolean;
  mesInicial: number;
  anoInicial: number;
}

export interface GetMapaAlocacaoResumoResponse {
  retorno: {
    resumo: ResumoAlocacao;
  };
  sucesso: boolean;
  mensagem: string | null;
  erros: string[] | null;
}

// Visão Gerencial - Recursos
export interface AlocacaoMes {
  mes: string; // Ex: "jan/2026"
  horas: string; // Ex: "0:00", "44:00"
  mesAlocacao: number;
  anoAlocacao: number;
  statusHorasRecursoFiltro: number; // 1=Horas pendentes, 2=Todas as horas alocadas, 3=Horas alocadas a mais
}

export interface AlocacaoRecurso {
  nome: string;
  ehTbd: boolean;
  cpfColaborador: string | null;
  codigoColaborador: string | null;
  codigoTbd: number | null;
  hardSkills: string | null;
  idioma: string | null;
  diretoria: string;
  codigoDiretoria: string;
  alocacao: AlocacaoMes[];
}

export interface GetMapaAlocacaoRecursoParams {
  cursor?: number;
  limite?: number;
  trimestral: boolean;
  hardskill?: string;
  idioma?: string;
  statusHorasFiltro?: number; // 0=Todos, 1=Horas pendentes, 2=Todas as horas alocadas, 3=Horas alocadas a mais
  gestorFiltro?: string;
  codigoDiretoria?: string;
  colaboradorOuTbdFiltro?: string;
}

export interface GetMapaAlocacaoRecursoResponse {
  retorno: {
    recurso: AlocacaoRecurso[];
  };
  sucesso: boolean;
  mensagem: string | null;
  erros: string[] | null;
}

// Visão Gerencial - Projetos
export interface ProjetoHoras {
  codigo_projeto: string;
  nome_projeto: string;
  proposta: string;
  cliente: string | null;
  codigo_cliente: string;
  horas_tecnicas: number;
  horas_alocadas: number;
  horas_disponiveis: number;
  quantidade_alocados: number;
  status_projeto: string;
  data_inicio: string;
  data_fim: string;
  nome_gerente_projeto: string;
  cpf_gerente_projeto: string;
  codigo_gerente_projeto: string;
  quantidadeDeGestores: number;
  quantidadeDeTBDS: number;
}

export interface ConsultaProjetoHorasParams {
  cursor?: number;
  limite?: number;
  cdProjeto?: string;
  disponibilidadeHorario?: number; // 0=Todos, 1=Horas pendentes, 2=Todas as horas alocadas, 3=Horas alocadas a mais
  cdStatusProjeto?: number;
  nomeProjeto?: string;
  codDiretoria?: number;
  cliente?: string;
  gestorProjeto?: string;
  cdCliente?: string;
}

export interface ConsultaProjetoHorasResponse {
  retorno: ProjetoHoras[];
  sucesso: boolean;
  mensagem: string | null;
  erros: string[] | null;
}
