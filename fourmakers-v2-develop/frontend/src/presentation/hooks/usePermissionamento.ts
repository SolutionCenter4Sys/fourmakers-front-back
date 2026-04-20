import { useState, useEffect, useMemo } from 'react';
import { container } from 'tsyringe';
import { useAppSelector } from '@app/store/hooks';
import { ListarClientesGestaoAlocadosUseCase } from '@domain/usecases/ListarClientesGestaoAlocadosUseCase';

export type EscopoType = 
  | 'TODAS_ORGS_CLIENTES'
  | 'ORG_CLIENTES'
  | 'CLIENTES_ORG'
  | 'CLIENTES_ESPECIFICOS';

export interface ClienteSelecionado {
  id: string;
  nome: string;
}

export interface PerfilPermissionamento {
  id: string;
  nome: string;
  escopo: EscopoType[];
  clientesSelecionados: ClienteSelecionado[];
  status: 'Ativo' | 'Inativo';
  descricao?: string;
  createdAt: string;
  updatedAt: string;
}

const STORAGE_KEY = 'permissionamento_perfis';

// Função para carregar perfis do localStorage
const loadPerfisFromStorage = (): PerfilPermissionamento[] => {
  try {
    const stored = localStorage.getItem(STORAGE_KEY);
    if (!stored) {
      // Retornar perfis padrão se não houver dados salvos
      return [
        {
          id: '1',
          nome: 'Administrador',
          escopo: ['TODAS_ORGS_CLIENTES'],
          clientesSelecionados: [],
          status: 'Ativo',
          descricao: 'Acesso completo a todas as organizações e clientes',
          createdAt: new Date().toISOString(),
          updatedAt: new Date().toISOString(),
        },
        {
          id: '2',
          nome: 'Gestor Org',
          escopo: ['ORG_CLIENTES'],
          clientesSelecionados: [],
          status: 'Ativo',
          descricao: 'Pode ver clientes a nível de org e realizar ações administrativas da org',
          createdAt: new Date().toISOString(),
          updatedAt: new Date().toISOString(),
        },
        {
          id: '3',
          nome: 'Comercial',
          escopo: ['CLIENTES_ORG'],
          clientesSelecionados: [],
          status: 'Ativo',
          descricao: 'Pode ver clientes da organização',
          createdAt: new Date().toISOString(),
          updatedAt: new Date().toISOString(),
        },
        {
          id: '4',
          nome: 'Recrutador',
          escopo: ['CLIENTES_ESPECIFICOS'],
          clientesSelecionados: [],
          status: 'Ativo',
          descricao: 'Pode ver cliente específico com ações limitadas',
          createdAt: new Date().toISOString(),
          updatedAt: new Date().toISOString(),
        },
      ];
    }
    return JSON.parse(stored);
  } catch {
    return [];
  }
};

// Função para salvar perfis no localStorage
const savePerfisToStorage = (perfis: PerfilPermissionamento[]) => {
  try {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(perfis));
  } catch (error) {
    console.error('Erro ao salvar perfis no localStorage:', error);
  }
};

export const usePermissionamento = () => {
  const { token } = useAppSelector((state) => state.auth);
  const listarClientesUseCase = container.resolve(ListarClientesGestaoAlocadosUseCase);
  const [perfis, setPerfis] = useState<PerfilPermissionamento[]>([]);
  const [search, setSearch] = useState('');
  const [clients, setClients] = useState<{ id: string; name: string }[]>([]);
  const [clientSearch, setClientSearch] = useState('');
  const [clientsLoading, setClientsLoading] = useState(false);

  // Carregar perfis do localStorage ao montar
  useEffect(() => {
    const loaded = loadPerfisFromStorage();
    setPerfis(loaded);
  }, []);

  // Salvar perfis no localStorage sempre que mudarem
  useEffect(() => {
    if (perfis.length > 0) {
      savePerfisToStorage(perfis);
    }
  }, [perfis]);

  // Buscar clientes apenas quando o popover estiver aberto
  const [shouldLoadClients, setShouldLoadClients] = useState(false);

  useEffect(() => {
    const loadClients = async () => {
      // Só busca se o popover estiver aberto
      if (!token || !shouldLoadClients) {
        if (!shouldLoadClients) {
          setClients([]);
        }
        return;
      }

      // Quando o popover abre, busca com string vazia para carregar lista completa
      // Quando o usuário digita, busca com o termo de busca
      const searchTerm = clientSearch.trim();

      setClientsLoading(true);
      try {
        const response = await listarClientesUseCase.execute(token, searchTerm);
        const lista = response?.retorno ?? [];
        setClients(
          lista.map((c) => ({
            id: c.codigoCliente,
            name: c.nomeCliente,
          }))
        );
      } catch (error) {
        console.error('Erro ao buscar clientes:', error);
        // Em caso de erro (incluindo CORS), apenas limpa a lista
        setClients([]);
      } finally {
        setClientsLoading(false);
      }
    };

    // Se o popover acabou de abrir, busca imediatamente
    // Se o usuário está digitando, aguarda um pouco para evitar muitas chamadas
    const delay = shouldLoadClients && !clientSearch.trim() ? 0 : 300;
    
    const timeoutId = setTimeout(() => {
      void loadClients();
    }, delay);

    return () => clearTimeout(timeoutId);
  }, [token, clientSearch, shouldLoadClients]);

  // Filtrar perfis pela busca
  const perfisFiltrados = useMemo(() => {
    if (!search.trim()) return perfis;
    
    const searchLower = search.toLowerCase();
    return perfis.filter(
      (perfil) =>
        perfil.nome.toLowerCase().includes(searchLower) ||
        perfil.descricao?.toLowerCase().includes(searchLower)
    );
  }, [perfis, search]);

  // Função para obter texto do escopo
  const getEscopoText = (perfil: PerfilPermissionamento): string => {
    const { escopo, clientesSelecionados } = perfil;
    
    if (escopo.includes('TODAS_ORGS_CLIENTES')) {
      return 'Todas as Orgs e Clientes';
    }
    if (escopo.includes('ORG_CLIENTES')) {
      return 'Org e Clientes da Org';
    }
    if (escopo.includes('CLIENTES_ORG')) {
      return 'Clientes da Org';
    }
    if (escopo.includes('CLIENTES_ESPECIFICOS')) {
      const count = clientesSelecionados?.length || 0;
      return `Clientes Específicos (${count} selecionado${count !== 1 ? 's' : ''})`;
    }
    return 'Não definido';
  };

  // Função para obter lista de clientes selecionados (para tooltip)
  const getClientesSelecionadosText = (perfil: PerfilPermissionamento): string => {
    if (!perfil.escopo.includes('CLIENTES_ESPECIFICOS')) {
      return '';
    }
    const clientes = perfil.clientesSelecionados || [];
    if (clientes.length === 0) {
      return 'Nenhum cliente selecionado';
    }
    return clientes.map((c) => c.nome).join(', ');
  };

  const criarPerfil = (perfil: Omit<PerfilPermissionamento, 'id' | 'createdAt' | 'updatedAt'>) => {
    const novoPerfil: PerfilPermissionamento = {
      ...perfil,
      id: Date.now().toString(),
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
    };
    setPerfis((prev) => [...prev, novoPerfil]);
    return novoPerfil;
  };

  const atualizarPerfil = (id: string, updates: Partial<PerfilPermissionamento>) => {
    setPerfis((prev) =>
      prev.map((perfil) =>
        perfil.id === id
          ? { ...perfil, ...updates, updatedAt: new Date().toISOString() }
          : perfil
      )
    );
  };

  const removerPerfil = (id: string) => {
    setPerfis((prev) => prev.filter((perfil) => perfil.id !== id));
  };

  const toggleStatusPerfil = (id: string) => {
    setPerfis((prev) =>
      prev.map((perfil) =>
        perfil.id === id
          ? {
              ...perfil,
              status: perfil.status === 'Ativo' ? 'Inativo' : 'Ativo',
              updatedAt: new Date().toISOString(),
            }
          : perfil
      )
    );
  };

  return {
    perfis: perfisFiltrados,
    todosPerfis: perfis, // Para edição, precisamos dos perfis não filtrados
    search,
    setSearch,
    clients,
    clientSearch,
    setClientSearch,
    clientsLoading,
    setShouldLoadClients,
    getEscopoText,
    getClientesSelecionadosText,
    criarPerfil,
    atualizarPerfil,
    removerPerfil,
    toggleStatusPerfil,
  };
};
