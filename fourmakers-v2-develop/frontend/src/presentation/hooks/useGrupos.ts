import { useState, useEffect, useMemo } from 'react';
import { container } from '@core/di/container';
import { GetColaboradoresUseCase } from '@domain/usecases/GetColaboradoresUseCase';
import type { Colaborador } from '@domain/entities/Colaborador';
import { useAppSelector } from '@app/store/hooks';
import { useParametrosFuncionalidades } from './useParametrosFuncionalidades';
import type { EscopoType, ClienteSelecionado } from './usePermissionamento';

export interface Grupo {
  id: string;
  nome: string;
  isAdmin: boolean;
  membros: ColaboradorSelecionado[];
  perfisSelecionados: string[]; // IDs dos perfis selecionados (mantido para compatibilidade)
  funcionalidades: FuncionalidadeSelecionada[];
  parametros: ParametroSelecionado[]; // Parâmetros selecionados
  escopo: EscopoType[]; // Escopo do grupo
  clientesSelecionados: ClienteSelecionado[]; // Clientes selecionados quando escopo for CLIENTES_ESPECIFICOS
  createdAt: string;
  updatedAt: string;
}

export interface ColaboradorSelecionado {
  cpf: string;
  nome: string;
  email: string;
  codColaborador: string;
}

export interface FuncionalidadeSelecionada {
  funcionalidadeId: string;
  moduloId: string;
}

export interface ParametroSelecionado {
  parametroId: string;
  moduloId: string;
  funcionalidadeId?: string; // Opcional: se for parâmetro de funcionalidade
}

export interface AtribuicaoPerfil {
  id: string;
  perfilId: string;
  colaboradorCpf: string;
  colaboradorNome: string;
  colaboradorEmail: string;
  colaboradorCodigo: string;
  createdAt: string;
  updatedAt: string;
}

const STORAGE_KEY_GRUPOS = 'permissionamento_grupos';
const STORAGE_KEY_ATRIBUICOES = 'permissionamento_atribuicoes';

const loadGruposFromStorage = (): Grupo[] => {
  try {
    const stored = localStorage.getItem(STORAGE_KEY_GRUPOS);
    if (stored) {
      return JSON.parse(stored);
    }
    return [];
  } catch {
    return [];
  }
};

const saveGruposToStorage = (grupos: Grupo[]) => {
  try {
    localStorage.setItem(STORAGE_KEY_GRUPOS, JSON.stringify(grupos));
  } catch (error) {
    console.error('Erro ao salvar grupos:', error);
  }
};

const loadAtribuicoesFromStorage = (): AtribuicaoPerfil[] => {
  try {
    const stored = localStorage.getItem(STORAGE_KEY_ATRIBUICOES);
    if (stored) {
      return JSON.parse(stored);
    }
    return [];
  } catch {
    return [];
  }
};

const saveAtribuicoesToStorage = (atribuicoes: AtribuicaoPerfil[]) => {
  try {
    localStorage.setItem(STORAGE_KEY_ATRIBUICOES, JSON.stringify(atribuicoes));
  } catch (error) {
    console.error('Erro ao salvar atribuições:', error);
  }
};

export const useGrupos = () => {
  const token = useAppSelector((state) => state.auth.token);
  const user = useAppSelector((state) => state.auth.user);
  const orgId = user?.orgId || user?.colaboradorOrg?.orgId || 0;
  const { todosModulos, funcionalidades } = useParametrosFuncionalidades();
  
  const [grupos, setGrupos] = useState<Grupo[]>([]);
  const [atribuicoes, setAtribuicoes] = useState<AtribuicaoPerfil[]>([]);
  const [search, setSearch] = useState('');
  const [colaboradores, setColaboradores] = useState<Colaborador[]>([]);
  const [colaboradoresLoading, setColaboradoresLoading] = useState(false);
  const [colaboradoresSearch, setColaboradoresSearch] = useState('');

  // Carregar grupos e atribuições do localStorage
  useEffect(() => {
    const gruposCarregados = loadGruposFromStorage();
    setGrupos(gruposCarregados);
    const atribuicoesCarregadas = loadAtribuicoesFromStorage();
    setAtribuicoes(atribuicoesCarregadas);
  }, []);

  // Salvar grupos quando mudarem
  useEffect(() => {
    if (grupos.length > 0) {
      saveGruposToStorage(grupos);
    }
  }, [grupos]);

  // Salvar atribuições quando mudarem
  useEffect(() => {
    if (atribuicoes.length > 0) {
      saveAtribuicoesToStorage(atribuicoes);
    }
  }, [atribuicoes]);

  // Buscar colaboradores
  const fetchColaboradores = async (nomeOuEmail: string = '') => {
    if (!token || !orgId || orgId === 0) {
      console.warn('Token ou orgId não disponível para buscar colaboradores', { token: !!token, orgId });
      return;
    }
    
    try {
      setColaboradoresLoading(true);
      const useCase = container.resolve(GetColaboradoresUseCase);
      const response = await useCase.execute(token, {
        cursor: 0,
        limite: 100,
        nomeOuEmail,
        org: orgId,
      });
      
      // Filtrar apenas colaboradores ativos
      const colaboradoresAtivos = (response.retorno || []).filter((col) => col.ativo);
      setColaboradores(colaboradoresAtivos);
    } catch (error) {
      console.error('Erro ao buscar colaboradores:', error);
      setColaboradores([]);
    } finally {
      setColaboradoresLoading(false);
    }
  };

  // Carregar colaboradores quando necessário
  const loadColaboradores = (nomeOuEmail: string = '') => {
    fetchColaboradores(nomeOuEmail);
  };

  // Filtrar grupos pela busca
  const gruposFiltrados = useMemo(() => {
    if (!search.trim()) return grupos;
    
    const searchLower = search.toLowerCase();
    return grupos.filter(
      (grupo) =>
        grupo.nome.toLowerCase().includes(searchLower)
    );
  }, [grupos, search]);

  // CRUD Grupos
  const criarGrupo = (grupo: Omit<Grupo, 'id' | 'createdAt' | 'updatedAt'>) => {
    const novoGrupo: Grupo = {
      ...grupo,
      perfisSelecionados: grupo.perfisSelecionados || [], // Manter compatibilidade
      escopo: grupo.escopo || [],
      clientesSelecionados: grupo.clientesSelecionados || [],
      parametros: grupo.parametros || [],
      id: Date.now().toString() + Math.random().toString(36).substring(2, 9),
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
    };
    setGrupos((prev) => [...prev, novoGrupo]);
  };

  const atualizarGrupo = (id: string, grupo: Omit<Grupo, 'id' | 'createdAt' | 'updatedAt'>) => {
    setGrupos((prev) =>
      prev.map((g) =>
        g.id === id
          ? { ...g, ...grupo, updatedAt: new Date().toISOString() }
          : g
      )
    );
  };

  const removerGrupo = (id: string) => {
    setGrupos((prev) => prev.filter((g) => g.id !== id));
  };

  // Obter funcionalidades por módulo
  const getFuncionalidadesByModulo = (moduloId: string) => {
    return funcionalidades.filter((f) => f.moduloId === moduloId);
  };

  // Obter colaboradores por perfil
  const getColaboradoresByPerfil = (perfilId: string): ColaboradorSelecionado[] => {
    return atribuicoes
      .filter((atrib) => atrib.perfilId === perfilId)
      .map((atrib) => ({
        cpf: atrib.colaboradorCpf,
        nome: atrib.colaboradorNome,
        email: atrib.colaboradorEmail,
        codColaborador: atrib.colaboradorCodigo,
      }));
  };

  // Adicionar atribuição de perfil a colaborador
  const adicionarAtribuicao = (
    perfilId: string,
    colaborador: Colaborador
  ) => {
    const novaAtribuicao: AtribuicaoPerfil = {
      id: Date.now().toString() + Math.random().toString(36).substring(2, 9),
      perfilId,
      colaboradorCpf: colaborador.cpf,
      colaboradorNome: colaborador.nome,
      colaboradorEmail: colaborador.email,
      colaboradorCodigo: colaborador.codColaborador,
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
    };
    setAtribuicoes((prev) => {
      // Evitar duplicatas
      const existe = prev.some(
        (a) => a.perfilId === perfilId && a.colaboradorCpf === colaborador.cpf
      );
      if (existe) return prev;
      return [...prev, novaAtribuicao];
    });
  };

  // Remover atribuição de perfil a colaborador
  const removerAtribuicao = (perfilId: string, colaboradorCpf: string) => {
    setAtribuicoes((prev) =>
      prev.filter(
        (a) => !(a.perfilId === perfilId && a.colaboradorCpf === colaboradorCpf)
      )
    );
  };

  return {
    grupos: gruposFiltrados,
    todosGrupos: grupos,
    colaboradores,
    colaboradoresLoading,
    colaboradoresSearch,
    setColaboradoresSearch,
    loadColaboradores,
    search,
    setSearch,
    todosModulos,
    funcionalidades,
    getFuncionalidadesByModulo,
    criarGrupo,
    atualizarGrupo,
    removerGrupo,
    atribuicoes,
    getColaboradoresByPerfil,
    adicionarAtribuicao,
    removerAtribuicao,
  };
};
