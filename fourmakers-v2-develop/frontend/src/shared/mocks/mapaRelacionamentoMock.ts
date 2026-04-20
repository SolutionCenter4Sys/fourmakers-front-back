import type { ClienteMapaRelacionamento, NoMapaRelacionamento } from '@domain/entities/MapaRelacionamento'

export const mockClienteMapaRelacionamento: ClienteMapaRelacionamento = {
  id: '1',
  codigoCliente: 'CLI001',
  nomeCliente: 'Empresa Exemplo LTDA',
  qtdAlocados: 50,
  qtdGestoresSemPerfil: 5,
  qtdGestores: 10,
}

// Mock client list - includes the mock client that has the mock structure
export const mockClientesMapaRelacionamento: ClienteMapaRelacionamento[] = [
  mockClienteMapaRelacionamento,
  {
    id: '2',
    codigoCliente: '0245',
    nomeCliente: 'FOURMAKERS 3',
    qtdAlocados: 9,
    qtdGestoresSemPerfil: 0,
    qtdGestores: 3,
  },
  {
    id: '3',
    codigoCliente: 'CLI002',
    nomeCliente: 'Outra Empresa Exemplo',
    qtdAlocados: 25,
    qtdGestoresSemPerfil: 2,
    qtdGestores: 5,
  },
  {
    id: '4',
    codigoCliente: 'EMPTY001',
    nomeCliente: 'Startup Nova',
    qtdAlocados: 0,
    qtdGestoresSemPerfil: 0,
    qtdGestores: 0,
  },
]

export const mockMapaRelacionamentoEstrutura: NoMapaRelacionamento = {
  id: '1',
  profileId: 'P001',
  profileName: 'CEO',
  employeeId: 'C001',
  employeeName: 'João Silva',
  employeeEmail: 'joao.silva@exemplo.com',
  managerCode: 'G001',
  departmentId: 'D001',
  departmentName: 'Diretoria',
  isCLevel: true,
  wasConnected: true,
  children: [
    {
      id: '2',
      profileId: 'P002',
      profileName: 'CTO',
      employeeId: 'C002',
      employeeName: 'Maria Santos',
      employeeEmail: 'maria.santos@exemplo.com',
      managerCode: 'G002',
      departmentId: 'D002',
      departmentName: 'Tecnologia',
      isCLevel: true,
      wasConnected: true,
      children: [
        {
          id: '3',
          profileId: 'P003',
          profileName: 'Gerente de Engenharia',
          employeeId: 'C003',
          employeeName: 'Pedro Oliveira',
          employeeEmail: 'pedro.oliveira@exemplo.com',
          managerCode: 'G003',
          departmentId: 'D002',
          departmentName: 'Tecnologia',
          isCLevel: false,
          wasConnected: true,
          children: [
            {
              id: '4',
              profileId: 'P004',
              profileName: 'Desenvolvedor Sênior',
              employeeId: 'C004',
              employeeName: 'Ana Costa',
              employeeEmail: 'ana.costa@exemplo.com',
              managerCode: 'G003',
              departmentId: 'D002',
              departmentName: 'Tecnologia',
              isCLevel: false,
              wasConnected: true,
              children: [],
            },
            {
              id: '5',
              profileId: 'P005',
              profileName: 'Desenvolvedor Pleno',
              employeeId: 'C005',
              employeeName: 'Carlos Mendes',
              employeeEmail: 'carlos.mendes@exemplo.com',
              managerCode: 'G003',
              departmentId: 'D002',
              departmentName: 'Tecnologia',
              isCLevel: false,
              wasConnected: true,
              children: [],
            },
          ],
        },
        {
          id: '6',
          profileId: 'P006',
          profileName: 'Gerente de Produto',
          employeeId: 'C006',
          employeeName: 'Fernanda Lima',
          employeeEmail: 'fernanda.lima@exemplo.com',
          managerCode: 'G002',
          departmentId: 'D002',
          departmentName: 'Tecnologia',
          isCLevel: false,
          wasConnected: true,
          children: [],
        },
      ],
    },
    {
      id: '7',
      profileId: 'P007',
      profileName: 'CFO',
      employeeId: 'C007',
      employeeName: 'Roberto Alves',
      employeeEmail: 'roberto.alves@exemplo.com',
      managerCode: 'G001',
      departmentId: 'D003',
      departmentName: 'Financeiro',
      isCLevel: true,
      wasConnected: true,
      children: [
        {
          id: '8',
          profileId: 'P008',
          profileName: 'Controller',
          employeeId: 'C008',
          employeeName: 'Juliana Rocha',
          employeeEmail: 'juliana.rocha@exemplo.com',
          managerCode: 'G007',
          departmentId: 'D003',
          departmentName: 'Financeiro',
          isCLevel: false,
          wasConnected: true,
          children: [],
        },
      ],
    },
    {
      id: '9',
      profileId: 'P009',
      profileName: 'Diretor Comercial',
      employeeId: 'C009',
      employeeName: 'Lucas Ferreira',
      employeeEmail: 'lucas.ferreira@exemplo.com',
      managerCode: 'G001',
      departmentId: 'D004',
      departmentName: 'Comercial',
      isCLevel: true,
      wasConnected: true,
      children: [
        {
          id: '10',
          profileId: 'P010',
          profileName: 'Coordenador de Vendas',
          employeeId: 'C010',
          employeeName: 'Patricia Souza',
          employeeEmail: 'patricia.souza@exemplo.com',
          managerCode: 'G009',
          departmentId: 'D004',
          departmentName: 'Comercial',
          isCLevel: false,
          wasConnected: true,
          children: [
            {
              id: '11',
              profileId: 'P011',
              profileName: 'Vendedor',
              employeeId: 'vacant',
              employeeName: 'Vago',
              employeeEmail: undefined,
              managerCode: 'G010',
              departmentId: 'D004',
              departmentName: 'Comercial',
              isCLevel: false,
              wasConnected: false,
              children: [],
            },
          ],
        },
      ],
    },
  ],
}

// Mock tree for CLI001 (Empresa Exemplo LTDA) - uses the detailed structure above
const mockTreeCLI001 = mockMapaRelacionamentoEstrutura

// Mock tree for 0245 (FOURMAKERS 3) - smaller startup structure
const mockTree0245: NoMapaRelacionamento = {
  id: 'fm-1',
  profileId: 'FM-P001',
  profileName: 'CEO & Founder',
  employeeId: 'FM-001',
  employeeName: 'Paulo Cymbaum',
  employeeEmail: 'paulo@fourmakers.com',
  departmentId: 'FM-D001',
  departmentName: 'Diretoria',
  isCLevel: true,
  wasConnected: true,
  children: [
    {
      id: 'fm-2',
      profileId: 'FM-P002',
      profileName: 'Tech Lead',
      employeeId: 'FM-002',
      employeeName: 'Maria Tech',
      employeeEmail: 'maria@fourmakers.com',
      departmentId: 'FM-D002',
      departmentName: 'Tecnologia',
      isCLevel: false,
      wasConnected: true,
      children: [
        {
          id: 'fm-3',
          profileId: 'FM-P003',
          profileName: 'Desenvolvedor Full Stack',
          employeeId: 'FM-003',
          employeeName: 'João Dev',
          employeeEmail: 'joao@fourmakers.com',
          departmentId: 'FM-D002',
          departmentName: 'Tecnologia',
          isCLevel: false,
          wasConnected: true,
          children: [],
        },
      ],
    },
    {
      id: 'fm-4',
      profileId: 'FM-P004',
      profileName: 'Product Manager',
      employeeId: 'FM-004',
      employeeName: 'Ana Product',
      employeeEmail: 'ana@fourmakers.com',
      departmentId: 'FM-D003',
      departmentName: 'Produto',
      isCLevel: false,
      wasConnected: true,
      children: [],
    },
  ],
}

// Mock tree for CLI002 (Outra Empresa Exemplo) - mid-size company
const mockTreeCLI002: NoMapaRelacionamento = {
  id: 'cli2-1',
  profileId: 'CLI2-P001',
  profileName: 'CEO',
  employeeId: 'CLI2-001',
  employeeName: 'Carlos Diretor',
  employeeEmail: 'carlos@outraempresa.com',
  departmentId: 'CLI2-D001',
  departmentName: 'Diretoria',
  isCLevel: true,
  wasConnected: true,
  children: [
    {
      id: 'cli2-2',
      profileId: 'CLI2-P002',
      profileName: 'Diretor de Operações',
      employeeId: 'CLI2-002',
      employeeName: 'Fernanda Ops',
      employeeEmail: 'fernanda@outraempresa.com',
      departmentId: 'CLI2-D002',
      departmentName: 'Operações',
      isCLevel: true,
      wasConnected: true,
      children: [
        {
          id: 'cli2-3',
          profileId: 'CLI2-P003',
          profileName: 'Coordenador de Logística',
          employeeId: 'CLI2-003',
          employeeName: 'Ricardo Log',
          employeeEmail: 'ricardo@outraempresa.com',
          departmentId: 'CLI2-D002',
          departmentName: 'Operações',
          isCLevel: false,
          wasConnected: true,
          children: [],
        },
      ],
    },
    {
      id: 'cli2-4',
      profileId: 'CLI2-P004',
      profileName: 'Diretor de RH',
      employeeId: 'CLI2-004',
      employeeName: 'Juliana RH',
      employeeEmail: 'juliana@outraempresa.com',
      departmentId: 'CLI2-D003',
      departmentName: 'Recursos Humanos',
      isCLevel: true,
      wasConnected: true,
      children: [
        {
          id: 'cli2-5',
          profileId: 'CLI2-P005',
          profileName: 'Analista de RH',
          employeeId: 'vacant',
          employeeName: 'Vago',
          departmentId: 'CLI2-D003',
          departmentName: 'Recursos Humanos',
          isCLevel: false,
          wasConnected: false,
          children: [],
        },
      ],
    },
  ],
}

// Map client codes to their mock trees
const mockTreesByClient: Record<string, NoMapaRelacionamento | null> = {
  'CLI001': mockTreeCLI001,
  '0245': mockTree0245,
  'CLI002': mockTreeCLI002,
  'EMPTY001': null, // Empty client for testing empty state
}

/**
 * Get mock tree structure for a given client code
 * @param codigoCliente - Client code (CLI001, 0245, CLI002, etc.)
 * @returns Mock tree structure or null if not found
 */
export function getMockTreeForClient(codigoCliente: string): NoMapaRelacionamento | null {
  // Check if client is in the map (returns undefined if not found)
  if (codigoCliente in mockTreesByClient) {
    return mockTreesByClient[codigoCliente]
  }
  return null
}

// LEGACY EXPORTS FOR BACKWARD COMPATIBILITY
// TODO: Remove after all files are updated
// Uppercase aliases for constants-style imports
export const MOCK_CLIENTES_MAPA_RELACIONAMENTO = mockClientesMapaRelacionamento
export const MOCK_ESTRUTURA_MAPA_RELACIONAMENTO = mockMapaRelacionamentoEstrutura
