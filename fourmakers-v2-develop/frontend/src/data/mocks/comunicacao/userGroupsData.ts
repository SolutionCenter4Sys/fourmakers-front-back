import type { UserGroup } from '@domain/entities/comunicacao';

// ============================================
// MOCK DATA - GRUPOS DE USUÁRIOS
// ============================================

export const mockUserGroups: UserGroup[] = [
  {
    id: 'ug1',
    name: 'Departamento de TI',
    description: 'Equipe de Tecnologia da Informação',
    status: 'active',
    members: [
      { id: 'ugm1', userId: 'u1', userName: 'Ana Silva', userEmail: 'ana.silva@email.com', addedAt: '2024-01-15' },
      { id: 'ugm2', userId: 'u2', userName: 'Carlos Santos', userEmail: 'carlos.santos@email.com', addedAt: '2024-01-16' },
      { id: 'ugm3', userId: 'u3', userName: 'Maria Oliveira', userEmail: 'maria.oliveira@email.com', addedAt: '2024-01-20' },
    ],
    createdAt: '2024-01-15',
    updatedAt: '2024-01-20',
    createdBy: 'admin',
    requiresApproval: false,
  },
  {
    id: 'ug2',
    name: 'Recursos Humanos',
    description: 'Equipe de RH e Gestão de Pessoas',
    status: 'active',
    members: [
      { id: 'ugm4', userId: 'u4', userName: 'João Pereira', userEmail: 'joao.pereira@email.com', addedAt: '2024-01-18' },
      { id: 'ugm5', userId: 'u5', userName: 'Fernanda Costa', userEmail: 'fernanda.costa@email.com', addedAt: '2024-01-19' },
    ],
    createdAt: '2024-01-18',
    updatedAt: '2024-01-19',
    createdBy: 'admin',
    requiresApproval: true,
  },
  {
    id: 'ug3',
    name: 'Filial São Paulo',
    description: 'Colaboradores da unidade de São Paulo',
    status: 'active',
    members: [
      { id: 'ugm6', userId: 'u6', userName: 'Ricardo Lima', userEmail: 'ricardo.lima@email.com', addedAt: '2024-02-01' },
      { id: 'ugm7', userId: 'u7', userName: 'Lucas Ferreira', userEmail: 'lucas.ferreira@email.com', addedAt: '2024-02-02' },
      { id: 'ugm8', userId: 'u8', userName: 'Patricia Souza', userEmail: 'patricia.souza@email.com', addedAt: '2024-02-03' },
    ],
    createdAt: '2024-02-01',
    updatedAt: '2024-02-03',
    createdBy: 'admin',
    requiresApproval: false,
  },
  {
    id: 'ug4',
    name: 'Diretoria',
    description: 'Diretores e gerentes executivos',
    status: 'active',
    members: [
      { id: 'ugm9', userId: 'u1', userName: 'Ana Silva', userEmail: 'ana.silva@email.com', addedAt: '2024-01-10' },
      { id: 'ugm10', userId: 'u2', userName: 'Carlos Santos', userEmail: 'carlos.santos@email.com', addedAt: '2024-01-10' },
    ],
    createdAt: '2024-01-10',
    updatedAt: '2024-01-10',
    createdBy: 'admin',
    requiresApproval: true,
  },
];
