import type { ReleaseContentsByMenuCode } from '@shared/types/releaseContent';

/**
 * Conteúdos de release por código do recurso no menu.
 * Ordenação: mais recente primeiro (dataInsercao desc).
 * Para novas features, adicione uma entrada com o menu code e o array de releases.
 */
export const releaseContentsByMenuCode: ReleaseContentsByMenuCode = {
  agendas_comerciais: [
    {
      titulo: 'Maverick agora também na Web',
      dataInsercao: '2025-03-09',
      conteudo: `O Maverick, módulo de gestão de relacionamento comercial do Fourmakers, agora também pode ser acessado pela versão web da plataforma.
Com essa evolução, os encontros comerciais deixam de depender apenas do aplicativo e passam a poder ser organizados e acompanhados diretamente pelo computador, ampliando o registro e o acompanhamento das interações com clientes.

✅ O que já está disponível na Web
Agora você pode:
• Visualizar agendas comerciais
• Filtrar agendas por período, cliente ou colaboradores
• Confirmar presença em reuniões
• Criar novas agendas - sem integração com teams
• Editar agendas existentes
• Confirmar participação nas agendas

Assim, as interações comerciais podem ser registradas e acompanhadas em qualquer dispositivo, mantendo o histórico de relacionamento centralizado no Fourmakers.`,
      midiaUrl: 'https://www.loom.com/share/be98d9c3dcaf446782536b0b72aafe88',
      midiaLabel: 'Ver vídeo',
    },
  ],
  mapa_relacionamento: [
    {
      titulo: 'Mapa de Relacionamento | Visão 360° do Stakeholder',
      dataInsercao: '2025-03-09',
      conteudo: `Agora o Maverick permite acessar rapidamente o histórico de relacionamento com cada gestor do cliente.

Ao abrir um gestor no Mapa de Relacionamento, é possível visualizar uma Visão 360° do stakeholder, reunindo em um único lugar:

🧭 Contexto
Dores e oportunidades registradas para aquele gestor.

📅 Agenda
Histórico de reuniões e agendas vinculadas ao stakeholder.

💬 Interações
Registros de interações comerciais realizadas com o gestor.

Cada registro apresenta:
• data
• assunto
• responsável
• status

Os itens são exibidos do mais recente para o mais antigo, facilitando a leitura do histórico.

Caso não existam registros, o sistema exibirá "Sem registros recentes".

📱 Também é possível abrir diretamente a agenda relacionada.

💡 Com isso você consegue entender rapidamente o histórico daquele relacionamento sem precisar navegar por diferentes módulos do Fourmakers.`,
      midiaUrl: 'https://www.loom.com/share/c79fd09be5f94b73a71ccbabcdddb8c6',
      midiaLabel: 'Ver vídeo',
    },
  ],
};
