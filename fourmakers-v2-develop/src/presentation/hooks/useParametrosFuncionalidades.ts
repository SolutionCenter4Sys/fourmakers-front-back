import { useState, useEffect, useMemo } from 'react';

// Módulo: Agrupa funcionalidades relacionadas (ex: APONTAMENTOS, MAPA_DE_ALOCACAO)
export interface Modulo {
  id: string;
  codigo: string;
  nome: string;
  descricao: string;
  createdAt: string;
  updatedAt: string;
}

// Funcionalidade: Ação específica do sistema (ex: Visualizar, Editar, Gerar Relatório)
export interface Funcionalidade {
  id: string;
  idNumerico?: number; // ID numérico da funcionalidade no sistema
  codigo: string;
  nome: string;
  descricao: string;
  moduloId: string; // Referência ao módulo
  tipo: 'visualizacao' | 'edicao' | 'criacao' | 'exclusao' | 'relatorio' | 'configuracao';
  createdAt: string;
  updatedAt: string;
}

// Parâmetro: Configuração que controla comportamento do sistema ou funcionalidade
export interface Parametro {
  id: string;
  codigo: string;
  descricao: string;
  valorPadrao: string;
  moduloId: string; // Parâmetros podem ser do módulo (aplicam a todas as funcionalidades)
  funcionalidadeId?: string; // Ou específicos de uma funcionalidade
  tipo: 'boolean' | 'string' | 'number' | 'json';
  createdAt: string;
  updatedAt: string;
}

const STORAGE_KEY_MODULOS = 'permissionamento_modulos';
const STORAGE_KEY_FUNCIONALIDADES = 'permissionamento_funcionalidades';
const STORAGE_KEY_PARAMETROS = 'permissionamento_parametros';

// Módulos iniciais
const modulosIniciais: Omit<Modulo, 'id' | 'createdAt' | 'updatedAt'>[] = [
  {
    codigo: 'APONTAMENTOS',
    nome: 'Apontamentos',
    descricao: 'Módulo para gestão de apontamentos de horas e timesheet',
  },
  {
    codigo: 'DADOS_CADASTRAIS',
    nome: 'Dados Cadastrais',
    descricao: 'Módulo para gestão de dados pessoais e cadastrais de colaboradores',
  },
  {
    codigo: 'MAPA_DE_ALOCACAO',
    nome: 'Mapa de Alocação',
    descricao: 'Módulo para gestão de alocações de colaboradores em projetos',
  },
  {
    codigo: 'GESTAO',
    nome: 'Gestão',
    descricao: 'Módulo para funcionalidades gerais de gestão',
  },
  {
    codigo: 'VAGAS',
    nome: 'Vagas',
    descricao: 'Módulo para gestão de vagas e recrutamento',
  },
  {
    codigo: 'COMPETENCIAS',
    nome: 'Competências',
    descricao: 'Módulo para gestão de competências e habilidades',
  },
  {
    codigo: 'BENEFICIOS',
    nome: 'Benefícios',
    descricao: 'Módulo para gestão de benefícios',
  },
  {
    codigo: 'PROJETOS',
    nome: 'Projetos',
    descricao: 'Módulo para gestão de projetos',
  },
  {
    codigo: 'PERMISSIONAMENTO',
    nome: 'Permissionamento',
    descricao: 'Módulo para gestão de acessos e permissões',
  },
  {
    codigo: 'GERAL',
    nome: 'Geral',
    descricao: 'Módulo para configurações gerais do sistema',
  },
  {
    codigo: 'TIMESHEET',
    nome: 'Timesheet',
    descricao: 'Módulo para gestão de timesheet e apontamentos de horas',
  },
  {
    codigo: 'LG',
    nome: 'LG',
    descricao: 'Módulo para integração com sistemas LG',
  },
];

// Funcionalidades iniciais organizadas por módulo
const funcionalidadesIniciais: Record<string, Omit<Funcionalidade, 'id' | 'moduloId' | 'createdAt' | 'updatedAt'>[]> = {
  APONTAMENTOS: [
    {
      idNumerico: 16,
      codigo: 'APONTAMENTO_HORAS_COLABORADOR',
      nome: 'Apontamento de Horas do Colaborador',
      descricao: 'Permite que usuários apontem horas em nome de qualquer colaborador, para lançamento de férias, por exemplo, e afastamentos',
      tipo: 'edicao',
    },
    {
      idNumerico: 11,
      codigo: 'RELATORIO_DE_APONTAMENTO',
      nome: 'Relatório de Apontamento',
      descricao: 'Gera relatórios de apontamento de horas, útil para acompanhamento de atividades',
      tipo: 'relatorio',
    },
    {
      idNumerico: 17,
      codigo: 'RELATORIO_FECHAMENTO_APONTAMENTO',
      nome: 'Relatório de Fechamento de Apontamento',
      descricao: 'Gera relatórios de fechamento de apontamentos, consolidando as horas trabalhadas',
      tipo: 'relatorio',
    },
    {
      idNumerico: 28,
      codigo: 'RELATORIO_DE_APONTAMENTO_SIMPLIFICADO',
      nome: 'Relatório de Apontamento Simplificado',
      descricao: 'Gera relatórios simplificados de apontamento de horas',
      tipo: 'relatorio',
    },
  ],
  DADOS_CADASTRAIS: [
    {
      idNumerico: 8,
      codigo: 'DADOS_COLABORADOR',
      nome: 'Dados do Colaborador',
      descricao: 'Acesso aos dados cadastrais dos colaboradores, como informações pessoais e profissionais',
      tipo: 'visualizacao',
    },
  ],
  MAPA_DE_ALOCACAO: [
    {
      idNumerico: 7,
      codigo: 'MAPA_DE_ALOCACAO',
      nome: 'Visualizar Mapa de Alocação',
      descricao: 'Visualização do Mapa de Alocação, mostrando a distribuição de colaboradores por projetos',
      tipo: 'visualizacao',
    },
    {
      idNumerico: 6,
      codigo: 'MAPA_DE_ALOCACAO_EDICAO',
      nome: 'Editar Mapa de Alocação',
      descricao: 'Permite a edição do Mapa de Alocação, onde são definidas as alocações de colaboradores em projetos',
      tipo: 'edicao',
    },
    {
      idNumerico: 12,
      codigo: 'RELACIONAR_PESSOAS_PROJETOS',
      nome: 'Relacionar Pessoas a Projetos',
      descricao: 'Permite relacionar colaboradores a projetos, facilitando a gestão de alocações',
      tipo: 'edicao',
    },
    {
      idNumerico: 14,
      codigo: 'RELATORIO_ALOCACOES',
      nome: 'Relatório de Alocações',
      descricao: 'Gera relatórios de alocações, mostrando a distribuição de colaboradores por projetos',
      tipo: 'relatorio',
    },
    {
      idNumerico: 13,
      codigo: 'CRIAR_RECURSO_TBD',
      nome: 'Criar Recurso TBD',
      descricao: 'Funcionalidade para criar recursos temporários ou pendentes de definição',
      tipo: 'criacao',
    },
  ],
  GESTAO: [
    {
      idNumerico: 15,
      codigo: 'CADASTRO_PESSOAS',
      nome: 'Cadastro de Pessoas',
      descricao: 'Permite o cadastro de novas pessoas no sistema, incluindo colaboradores e gestores',
      tipo: 'criacao',
    },
    {
      idNumerico: 19,
      codigo: 'INATIVAR_PESSOAS',
      nome: 'Inativar Pessoas',
      descricao: 'Permite inativar colaboradores ou outros usuários no sistema',
      tipo: 'exclusao',
    },
    {
      idNumerico: 24,
      codigo: 'LISTA_GESTORES',
      nome: 'Lista de Gestores',
      descricao: 'Permite listar gestores. Funcionalidade necessária para módulos que utilizam a hierarquia',
      tipo: 'visualizacao',
    },
    {
      idNumerico: 29,
      codigo: 'CADASTRO_GESTAO_ALOCADOS',
      nome: 'Cadastro - Gestão de Alocados',
      descricao: 'Permite o cadastro e gestão de perfis necessários para alocação em clientes',
      tipo: 'criacao',
    },
  ],
  COMPETENCIAS: [
    {
      idNumerico: 9,
      codigo: 'SUMARIO_DE_COMPETENCIA',
      nome: 'Sumário de Competência',
      descricao: 'Permite busca por competências dos colaboradores, facilitando a gestão de habilidades e busca por CVs',
      tipo: 'visualizacao',
    },
    {
      idNumerico: 10,
      codigo: 'CV_DIGITAL',
      nome: 'CV Digital',
      descricao: 'Permite a visualização do currículo digital dos colaboradores',
      tipo: 'visualizacao',
    },
    {
      idNumerico: 21,
      codigo: 'GESTAO_COMPETENCIAS',
      nome: 'Gestão de Competências',
      descricao: 'Facilita a gestão de competências, permitindo cadastro e acompanhamento de habilidades',
      tipo: 'edicao',
    },
  ],
  VAGAS: [
    {
      idNumerico: 32,
      codigo: 'VISUALIZAR_INDICACOES',
      nome: 'Visualizar Indicações',
      descricao: 'Permite visualizar indicações feitas por colaboradores em vagas disponíveis',
      tipo: 'visualizacao',
    },
    {
      idNumerico: 33,
      codigo: 'EDITAR_INDICACOES',
      nome: 'Editar Indicações',
      descricao: 'Permite editar indicações feitas por colaboradores em vagas',
      tipo: 'edicao',
    },
  ],
  PROJETOS: [
    {
      idNumerico: 18,
      codigo: 'CRIAR_PROJETOS',
      nome: 'Criar Projetos',
      descricao: 'Permite a criação de novos projetos no sistema',
      tipo: 'criacao',
    },
    {
      idNumerico: 31,
      codigo: 'CRIAR_PROPOSTAS',
      nome: 'Criar Propostas',
      descricao: 'Permite a criação de propostas para novos projetos ou iniciativas',
      tipo: 'criacao',
    },
  ],
  BENEFICIOS: [
    {
      idNumerico: 30,
      codigo: 'GESTAO_DE_BENEFICIOS',
      nome: 'Gestão de Benefícios',
      descricao: 'Facilita a gestão de benefícios oferecidos aos colaboradores',
      tipo: 'edicao',
    },
  ],
  PERMISSIONAMENTO: [
    {
      idNumerico: 22,
      codigo: 'CONTROLE_FUNCIONALIDADE_SISTEMA_USUARIO',
      nome: 'Controle de Funcionalidade - Usuário',
      descricao: 'Permite controlar quais Grupos de Acesso e funcionalidades estão disponíveis para cada usuário',
      tipo: 'configuracao',
    },
    {
      idNumerico: 23,
      codigo: 'CONTROLE_FUNCIONALIDADE_SISTEMA_GRUPO_ACESSO',
      nome: 'Controle de Funcionalidade - Grupo de Acesso',
      descricao: 'Permite controlar quais funcionalidades estão disponíveis para cada grupo de acesso',
      tipo: 'configuracao',
    },
    {
      idNumerico: 25,
      codigo: 'CADASTRO_PARAMETRO',
      nome: 'Cadastro de Parâmetro',
      descricao: 'Permite o cadastro de novos parâmetros no sistema',
      tipo: 'configuracao',
    },
    {
      idNumerico: 26,
      codigo: 'CADASTRO_PARAMETRO_CONFIGURACAO',
      nome: 'Cadastro de Parâmetro - Configuração',
      descricao: 'Permite configurar parâmetros específicos para organizações ou grupos',
      tipo: 'configuracao',
    },
    {
      idNumerico: 27,
      codigo: 'CADASTRO_PARAMETRO_CONFIGURACAO_OUTRAS_ORG',
      nome: 'Cadastro de Parâmetro - Configuração Outras Orgs',
      descricao: 'Permite configurar parâmetros para outras organizações',
      tipo: 'configuracao',
    },
  ],
};

// Parâmetros iniciais organizados por módulo
const parametrosIniciais: Record<string, Omit<Parametro, 'id' | 'moduloId' | 'createdAt' | 'updatedAt'>[]> = {
  APONTAMENTOS: [
    {
      codigo: 'MOSTRAR_COLUNA_APROVADORES_TIMESHEET',
      descricao: 'Exibe a coluna de aprovadores na aba de aprovação do Timesheet',
      valorPadrao: 'true',
      tipo: 'boolean',
    },
    {
      codigo: 'LABEL_COLABORADORES_TIMESHEET',
      descricao: 'Define o texto da label "Colaboradores" no Timesheet',
      valorPadrao: 'Residentes',
      tipo: 'string',
    },
    {
      codigo: 'LABEL_COLABORADOR_TIMESHEET',
      descricao: 'Define o texto da label "Colaborador" no Timesheet',
      valorPadrao: 'Residente',
      tipo: 'string',
    },
  ],
  TIMESHEET: [
    {
      codigo: 'OBRIGA_RESUMOATIVIDADE_TIMESHEET',
      descricao: 'Torna o campo "Resumo das Atividades" obrigatório no Timesheet',
      valorPadrao: 'true',
      tipo: 'boolean',
    },
  ],
  DADOS_CADASTRAIS: [
    {
      codigo: 'MOSTRA_ABA_PERFIL',
      descricao: 'Habilita a exibição da aba "Perfil" dentro de "Dados Pessoais"',
      valorPadrao: 'true',
      tipo: 'boolean',
    },
    {
      codigo: 'MOSTRA_PASSAPORTE_VISTOS',
      descricao: 'Habilita a exibição da aba "Passaporte/Vistos" dentro de "Dados Pessoais"',
      valorPadrao: 'true',
      tipo: 'boolean',
    },
    {
      codigo: 'MOSTRA_ABA_CONTATO',
      descricao: 'Habilita a exibição da aba "Contato" dentro de "Dados Pessoais"',
      valorPadrao: 'true',
      tipo: 'boolean',
    },
    {
      codigo: 'MOSTRA_ABA_ENDERECO',
      descricao: 'Habilita a exibição da aba "Endereço" dentro de "Dados Pessoais"',
      valorPadrao: 'true',
      tipo: 'boolean',
    },
    {
      codigo: 'MOSTRA_ABA_SAUDE',
      descricao: 'Habilita a exibição da aba "Saúde" dentro de "Dados Pessoais"',
      valorPadrao: 'true',
      tipo: 'boolean',
    },
    {
      codigo: 'MOSTRA_ABA_DEPENDENTES',
      descricao: 'Habilita a exibição da aba "Dependentes" dentro de "Dados Pessoais"',
      valorPadrao: 'true',
      tipo: 'boolean',
    },
    {
      codigo: 'MOSTRA_ABA_COMPARTILHAMENTO',
      descricao: 'Habilita a exibição da aba "Compartilhamento" dentro de "Dados Pessoais"',
      valorPadrao: 'true',
      tipo: 'boolean',
    },
    {
      codigo: 'MOSTRA_ABA_ALTERAR_SENHA',
      descricao: 'Habilita a exibição da aba "Alterar Senha" dentro de "Dados Pessoais"',
      valorPadrao: 'true',
      tipo: 'boolean',
    },
  ],
  MAPA_DE_ALOCACAO: [
    {
      codigo: 'CONFIGURACAO_PERCENTUAL_ADERENCIA',
      descricao: 'Define os percentuais máximos para habilidades relevantes, desejáveis, disponibilidade, localidade e custos',
      valorPadrao: 'perc_max_skills_relevantes=40;perc_max_skills_desejaveis=20;...',
      tipo: 'json',
    },
    {
      codigo: 'ASSOCIACAO_AUTOMATICA_COLABORADOR_PROJETO',
      descricao: 'Permite associação automática de colaboradores a projetos',
      valorPadrao: 'true',
      tipo: 'boolean',
    },
    {
      codigo: 'PERMITIR_LANCAMENTO_MAIOR_24_HORAS',
      descricao: 'Permite lançar mais de 24 horas em um dia no Mapa de Alocação',
      valorPadrao: 'true',
      tipo: 'boolean',
    },
    {
      codigo: 'DEVE_PULAR_GERENCIA_CLIENTES_PROJETO',
      descricao: 'Ignora a gestão de clientes associados ao projeto ao criar/editar',
      valorPadrao: 'true',
      tipo: 'boolean',
    },
    {
      codigo: 'PERMITIR_SOBREPOSICAO_ALOCACOES',
      descricao: 'Permite sobrepor alocações no Mapa de Alocação',
      valorPadrao: 'true',
      tipo: 'boolean',
    },
    {
      codigo: 'DEVE_INCLUIR_INATIVOS_MAPA_ALOCACAO',
      descricao: 'Inclui colaboradores inativos no Mapa de Alocação',
      valorPadrao: 'true',
      tipo: 'boolean',
    },
    {
      codigo: 'CONFIGURACAO_PERCENTUAL_MATCH_REVERSO',
      descricao: 'Define valores de match reverso para aderência',
      valorPadrao: 'perc_max_skills_relevantes=44;perc_max_skills_desejaveis=24;...',
      tipo: 'json',
    },
  ],
  GESTAO: [
    {
      codigo: 'MOSTRA_MAPA_DEMOGRAFICO',
      descricao: 'Habilita a exibição do mapa demográfico',
      valorPadrao: 'true',
      tipo: 'boolean',
    },
    {
      codigo: 'CONFIGURACAO_ADERENCIA_ORG',
      descricao: 'Define quais organizações listar na aderência',
      valorPadrao: '1,2,7',
      tipo: 'string',
    },
    {
      codigo: 'CAMPOS_NAO_OBRIGATORIOS_CADASTRO_COLABORADOR',
      descricao: 'Define campos não obrigatórios no cadastro de colaboradores',
      valorPadrao: 'Email;Gestor;Diretoria;Departamento;DataAdmissao',
      tipo: 'string',
    },
  ],
  VAGAS: [
    {
      codigo: 'MOSTRA_VAGAS',
      descricao: 'Habilita a exibição de vagas no portal',
      valorPadrao: 'true',
      tipo: 'boolean',
    },
    {
      codigo: 'PODE_CANDIDATAR',
      descricao: 'Habilita a candidatura do usuário ao banco de talentos',
      valorPadrao: 'true',
      tipo: 'boolean',
    },
  ],
  GERAL: [
    {
      codigo: 'URL_HOME_DEFAULT',
      descricao: 'Define a URL padrão de redirecionamento para o portal',
      valorPadrao: 'vagas/portal',
      tipo: 'string',
    },
  ],
  LG: [
    {
      codigo: 'CONFIGURACAO_API_SOAP_LG',
      descricao: 'Configura integração com a API SOAP da LG',
      valorPadrao: 'Usuario=eng.fourmakers@foursys.com.br;Senha="nFmbRxd*fFZc";...',
      tipo: 'json',
    },
  ],
};

const loadModulosFromStorage = (): Modulo[] => {
  try {
    const stored = localStorage.getItem(STORAGE_KEY_MODULOS);
    if (stored) {
      return JSON.parse(stored);
    }
    const modulos: Modulo[] = modulosIniciais.map((m) => ({
      ...m,
      id: Date.now().toString() + Math.random().toString(36).substring(2, 9),
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
    }));
    saveModulosToStorage(modulos);
    return modulos;
  } catch {
    return [];
  }
};

const saveModulosToStorage = (modulos: Modulo[]) => {
  try {
    localStorage.setItem(STORAGE_KEY_MODULOS, JSON.stringify(modulos));
  } catch (error) {
    console.error('Erro ao salvar módulos:', error);
  }
};

const loadFuncionalidadesFromStorage = (): Funcionalidade[] => {
  try {
    const stored = localStorage.getItem(STORAGE_KEY_FUNCIONALIDADES);
    if (stored) {
      return JSON.parse(stored);
    }
    return [];
  } catch {
    return [];
  }
};

const saveFuncionalidadesToStorage = (funcionalidades: Funcionalidade[]) => {
  try {
    localStorage.setItem(STORAGE_KEY_FUNCIONALIDADES, JSON.stringify(funcionalidades));
  } catch (error) {
    console.error('Erro ao salvar funcionalidades:', error);
  }
};

const loadParametrosFromStorage = (): Parametro[] => {
  try {
    const stored = localStorage.getItem(STORAGE_KEY_PARAMETROS);
    if (stored) {
      return JSON.parse(stored);
    }
    return [];
  } catch {
    return [];
  }
};

const saveParametrosToStorage = (parametros: Parametro[]) => {
  try {
    localStorage.setItem(STORAGE_KEY_PARAMETROS, JSON.stringify(parametros));
  } catch (error) {
    console.error('Erro ao salvar parâmetros:', error);
  }
};

// Função para inferir o tipo do parâmetro baseado no valor padrão
const inferirTipoParametro = (valorPadrao: string): Parametro['tipo'] => {
  if (!valorPadrao || valorPadrao.trim() === '') {
    return 'string';
  }
  
  const valorLower = valorPadrao.toLowerCase().trim();
  
  // Boolean
  if (valorLower === 'true' || valorLower === 'false') {
    return 'boolean';
  }
  
  // Number
  if (!isNaN(Number(valorPadrao)) && valorPadrao.trim() !== '') {
    return 'number';
  }
  
  // JSON (contém estruturas como key=value; ou objetos JSON)
  if (valorPadrao.includes('=') || valorPadrao.includes('{') || valorPadrao.includes('[') || valorPadrao.includes(';')) {
    return 'json';
  }
  
  // String (padrão)
  return 'string';
};

// Função para garantir que um parâmetro tenha um tipo válido
const garantirTipoParametro = (param: Omit<Parametro, 'id' | 'moduloId' | 'createdAt' | 'updatedAt'> | Parametro): Parametro['tipo'] => {
  if (param.tipo && ['boolean', 'string', 'number', 'json'].includes(param.tipo)) {
    return param.tipo;
  }
  return inferirTipoParametro(param.valorPadrao);
};

export const useParametrosFuncionalidades = () => {
  const [modulos, setModulos] = useState<Modulo[]>([]);
  const [funcionalidades, setFuncionalidades] = useState<Funcionalidade[]>([]);
  const [parametros, setParametros] = useState<Parametro[]>([]);
  const [search, setSearch] = useState('');

  // Inicializar dados
  useEffect(() => {
    let mods = loadModulosFromStorage();
    
    // Se não há módulos salvos, inicializar com dados padrão
    if (mods.length === 0) {
      mods = modulosIniciais.map((m) => ({
        ...m,
        id: Date.now().toString() + Math.random().toString(36).substring(2, 9),
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString(),
      }));
      saveModulosToStorage(mods);
    }
    
    setModulos(mods);

    const funcs = loadFuncionalidadesFromStorage();
    const params = loadParametrosFromStorage();

    // Se não há funcionalidades salvas, inicializar com dados padrão
    if (funcs.length === 0 && mods.length > 0) {
      const funcionalidadesIniciaisCompletas: Funcionalidade[] = [];
      mods.forEach((modulo) => {
        const funcsDoModulo = funcionalidadesIniciais[modulo.codigo] || [];
        funcsDoModulo.forEach((func) => {
          funcionalidadesIniciaisCompletas.push({
            ...func,
            id: Date.now().toString() + Math.random().toString(36).substring(2, 9),
            moduloId: modulo.id,
            createdAt: new Date().toISOString(),
            updatedAt: new Date().toISOString(),
          });
        });
      });
      setFuncionalidades(funcionalidadesIniciaisCompletas);
      saveFuncionalidadesToStorage(funcionalidadesIniciaisCompletas);
    } else {
      // Verificar se há funcionalidades faltando para módulos existentes
      const codigosFuncionalidadesExistentes = new Set(funcs.map(f => f.codigo));
      const funcionalidadesParaAdicionar: Funcionalidade[] = [];
      let funcionalidadesAtualizadas = [...funcs];
      let precisaAtualizar = false;
      
      // Para cada módulo, verificar se suas funcionalidades iniciais existem
      mods.forEach((modulo) => {
        const funcsDoModulo = funcionalidadesIniciais[modulo.codigo] || [];
        funcsDoModulo.forEach((func) => {
          // Verificar se a funcionalidade já existe (por código)
          if (!codigosFuncionalidadesExistentes.has(func.codigo)) {
            funcionalidadesParaAdicionar.push({
              ...func,
              id: Date.now().toString() + Math.random().toString(36).substring(2, 9),
              moduloId: modulo.id,
              createdAt: new Date().toISOString(),
              updatedAt: new Date().toISOString(),
            });
          } else {
            // Se a funcionalidade existe mas está associada a um módulo diferente, atualizar o moduloId
            const funcionalidadeExistente = funcs.find(f => f.codigo === func.codigo);
            if (funcionalidadeExistente && funcionalidadeExistente.moduloId !== modulo.id) {
              funcionalidadesAtualizadas = funcionalidadesAtualizadas.map(f => 
                f.id === funcionalidadeExistente.id 
                  ? { ...f, moduloId: modulo.id, updatedAt: new Date().toISOString() }
                  : f
              );
              precisaAtualizar = true;
            }
          }
        });
      });
      
      if (funcionalidadesParaAdicionar.length > 0 || precisaAtualizar) {
        const novasFuncs = [...funcionalidadesAtualizadas, ...funcionalidadesParaAdicionar];
        setFuncionalidades(novasFuncs);
        saveFuncionalidadesToStorage(novasFuncs);
      } else {
        setFuncionalidades(funcs);
      }
    }

    // Se não há parâmetros salvos, inicializar com dados padrão
    if (params.length === 0 && mods.length > 0) {
      const parametrosIniciaisCompletos: Parametro[] = [];
      mods.forEach((modulo) => {
        const paramsDoModulo = parametrosIniciais[modulo.codigo] || [];
        paramsDoModulo.forEach((param) => {
          parametrosIniciaisCompletos.push({
            ...param,
            tipo: garantirTipoParametro(param),
            id: Date.now().toString() + Math.random().toString(36).substring(2, 9),
            moduloId: modulo.id,
            createdAt: new Date().toISOString(),
            updatedAt: new Date().toISOString(),
          });
        });
      });
      setParametros(parametrosIniciaisCompletos);
      saveParametrosToStorage(parametrosIniciaisCompletos);
    } else {
      // Garantir que todos os parâmetros carregados tenham tipo válido
      let parametrosAtualizados = params.map(p => ({
        ...p,
        tipo: garantirTipoParametro(p),
      }));
      let precisaAtualizar = parametrosAtualizados.some((p, index) => p.tipo !== params[index].tipo);
      
      // Verificar se há parâmetros faltando para módulos existentes
      // Verificar quais parâmetros já existem (por código do parâmetro)
      const codigosParametrosExistentes = new Set(params.map(p => p.codigo));
      const parametrosParaAdicionar: Parametro[] = [];
      
      // Para cada módulo, verificar se seus parâmetros iniciais existem
      mods.forEach((modulo) => {
        const paramsDoModulo = parametrosIniciais[modulo.codigo] || [];
        paramsDoModulo.forEach((param) => {
          // Verificar se o parâmetro já existe (por código)
          if (!codigosParametrosExistentes.has(param.codigo)) {
            parametrosParaAdicionar.push({
              ...param,
              tipo: garantirTipoParametro(param),
              id: Date.now().toString() + Math.random().toString(36).substring(2, 9),
              moduloId: modulo.id,
              createdAt: new Date().toISOString(),
              updatedAt: new Date().toISOString(),
            });
          } else {
            // Se o parâmetro existe mas está associado a um módulo diferente, atualizar o moduloId
            const parametroExistente = params.find(p => p.codigo === param.codigo);
            if (parametroExistente && parametroExistente.moduloId !== modulo.id) {
              // Atualizar o moduloId do parâmetro existente
              parametrosAtualizados = parametrosAtualizados.map(p => 
                p.id === parametroExistente.id 
                  ? { ...p, moduloId: modulo.id, updatedAt: new Date().toISOString() }
                  : p
              );
              precisaAtualizar = true;
            }
          }
        });
      });
      
      if (parametrosParaAdicionar.length > 0 || precisaAtualizar) {
        const novosParams = [...parametrosAtualizados, ...parametrosParaAdicionar];
        setParametros(novosParams);
        saveParametrosToStorage(novosParams);
      } else {
        setParametros(params);
      }
    }
  }, []);

  // Salvar quando mudarem
  useEffect(() => {
    if (modulos.length > 0) {
      saveModulosToStorage(modulos);
    }
  }, [modulos]);

  useEffect(() => {
    if (funcionalidades.length > 0) {
      saveFuncionalidadesToStorage(funcionalidades);
    }
  }, [funcionalidades]);

  useEffect(() => {
    if (parametros.length > 0) {
      saveParametrosToStorage(parametros);
    }
  }, [parametros]);

  // Filtrar módulos pela busca
  const modulosFiltrados = useMemo(() => {
    if (!search.trim()) return modulos;
    
    const searchLower = search.toLowerCase();
    return modulos.filter(
      (mod) =>
        mod.nome.toLowerCase().includes(searchLower) ||
        mod.codigo.toLowerCase().includes(searchLower) ||
        mod.descricao.toLowerCase().includes(searchLower)
    );
  }, [modulos, search]);

  // Obter funcionalidades de um módulo
  const getFuncionalidadesByModulo = (moduloId: string): Funcionalidade[] => {
    return funcionalidades.filter((f) => f.moduloId === moduloId);
  };

  // Obter parâmetros de um módulo (inclui todos os parâmetros do módulo, mesmo os que têm funcionalidade associada)
  const getParametrosByModulo = (moduloId: string): Parametro[] => {
    return parametros.filter((p) => p.moduloId === moduloId);
  };

  const getParametrosByFuncionalidade = (funcionalidadeId: string): Parametro[] => {
    return parametros.filter((p) => p.funcionalidadeId === funcionalidadeId);
  };

  // CRUD Módulos
  const criarModulo = (modulo: Omit<Modulo, 'id' | 'createdAt' | 'updatedAt'>) => {
    const novoModulo: Modulo = {
      ...modulo,
      id: Date.now().toString() + Math.random().toString(36).substring(2, 9),
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
    };
    setModulos((prev) => [...prev, novoModulo]);
  };

  const atualizarModulo = (id: string, updates: Partial<Modulo>) => {
    setModulos((prev) =>
      prev.map((m) =>
        m.id === id
          ? { ...m, ...updates, updatedAt: new Date().toISOString() }
          : m
      )
    );
  };

  const removerModulo = (id: string) => {
    // Remover módulo e suas funcionalidades e parâmetros relacionados
    setModulos((prev) => prev.filter((m) => m.id !== id));
    setFuncionalidades((prev) => prev.filter((f) => f.moduloId !== id));
    setParametros((prev) => prev.filter((p) => p.moduloId !== id));
  };

  // CRUD Funcionalidades
  const criarFuncionalidade = (funcionalidade: Omit<Funcionalidade, 'id' | 'createdAt' | 'updatedAt'>) => {
    const novaFuncionalidade: Funcionalidade = {
      ...funcionalidade,
      id: Date.now().toString() + Math.random().toString(36).substring(2, 9),
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
    };
    setFuncionalidades((prev) => [...prev, novaFuncionalidade]);
  };

  const atualizarFuncionalidade = (id: string, updates: Partial<Funcionalidade>) => {
    setFuncionalidades((prev) =>
      prev.map((f) =>
        f.id === id
          ? { ...f, ...updates, updatedAt: new Date().toISOString() }
          : f
      )
    );
  };

  const removerFuncionalidade = (id: string) => {
    // Remover funcionalidade e seus parâmetros específicos
    setFuncionalidades((prev) => prev.filter((f) => f.id !== id));
    setParametros((prev) => prev.filter((p) => p.funcionalidadeId !== id));
  };

  // CRUD Parâmetros
  const criarParametro = (parametro: Omit<Parametro, 'id' | 'createdAt' | 'updatedAt'>) => {
    const novoParametro: Parametro = {
      ...parametro,
      id: Date.now().toString() + Math.random().toString(36).substring(2, 9),
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
    };
    setParametros((prev) => {
      const existe = prev.some((p) => p.codigo === novoParametro.codigo && p.moduloId === novoParametro.moduloId);
      if (existe) {
        return prev;
      }
      return [...prev, novoParametro];
    });
  };

  const atualizarParametro = (id: string, updates: Partial<Parametro>) => {
    setParametros((prev) =>
      prev.map((p) =>
        p.id === id
          ? { ...p, ...updates, updatedAt: new Date().toISOString() }
          : p
      )
    );
  };

  const removerParametro = (id: string) => {
    setParametros((prev) => prev.filter((p) => p.id !== id));
  };

  return {
    modulos: modulosFiltrados,
    todosModulos: modulos,
    funcionalidades,
    parametros,
    search,
    setSearch,
    getFuncionalidadesByModulo,
    getParametrosByModulo,
    getParametrosByFuncionalidade,
    criarModulo,
    atualizarModulo,
    removerModulo,
    criarFuncionalidade,
    atualizarFuncionalidade,
    removerFuncionalidade,
    criarParametro,
    atualizarParametro,
    removerParametro,
  };
};
