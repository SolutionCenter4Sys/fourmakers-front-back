import type { HomeConfig } from '@shared/types/homeBuilder'

let counter = 0
function genId(): string {
  return `section_${Date.now()}_${++counter}`
}

export function createDefaultConfig(): HomeConfig {
  return {
    version: '1.0',
    sections: [
      {
        id: genId(),
        visible: true,
        columnSpan: 'full',
        background: { type: 'none' },
        content: {
          type: 'banner',
          height: 'lg',
          autoPlay: true,
          interval: 5,
          showDots: true,
          showArrows: true,
          slides: [
            {
              id: genId(),
              background: {
                type: 'gradient',
                gradient: { from: '#9A1BFF', to: '#4F46E5', direction: 'to-br' },
              },
              title: 'Juntos, fazemos mais!',
              subtitle: 'Acesse tudo que você precisa em um só lugar.',
              textColor: 'light',
              textAlign: 'left',
              textVerticalAlign: 'center',
              button: {
                label: 'Explorar',
                variant: 'secondary',
                link: { type: 'none', href: '', target: '_self' },
              },
            },
            {
              id: genId(),
              background: {
                type: 'gradient',
                gradient: { from: '#0F172A', to: '#1E293B', direction: 'to-r' },
              },
              title: 'Conecte-se com sua equipe',
              subtitle: 'Veja as novidades no feed de comunicação.',
              textColor: 'light',
              textAlign: 'center',
              textVerticalAlign: 'center',
              button: {
                label: 'Abrir Feed',
                variant: 'primary',
                link: { type: 'internal', href: '/comunicacao', target: '_self' },
              },
            },
          ],
        },
      },
      {
        id: genId(),
        visible: true,
        columnSpan: 'half',
        background: { type: 'none' },
        content: {
          type: 'welcome',
          greeting: 'Olá, {nomeColaborador}!',
          subtitle: 'Seja bem-vindo ao Fourmakers',
          showUserName: true,
        },
      },
      {
        id: genId(),
        visible: true,
        columnSpan: 'half',
        background: { type: 'none' },
        content: {
          type: 'shortcuts',
          title: 'Acesso rápido',
          items: [
            {
              id: genId(),
              iconName: 'User',
              label: 'Dados pessoais',
              link: { type: 'internal', href: '/page/dadospessoais', target: '_self' },
            },
            {
              id: genId(),
              iconName: 'TrendingUp',
              label: 'Perfil 360',
              link: { type: 'internal', href: '/page/cvdigital', target: '_self' },
            },
            {
              id: genId(),
              iconName: 'ArrowUpDown',
              label: 'Reembolso',
              link: { type: 'internal', href: '/reembolso', target: '_self' },
            },
          ],
          columns: 3,
        },
      },
      {
        id: genId(),
        visible: true,
        columnSpan: 'half',
        background: { type: 'none' },
        content: {
          type: 'feed',
          title: 'Feed',
          showFilters: true,
          compactMode: false,
        },
      },
      {
        id: genId(),
        visible: true,
        columnSpan: 'half',
        background: { type: 'none' },
        content: {
          type: 'aniversariantes',
          title: 'Aniversariantes da semana',
          height: 'md',
        },
      },
      {
        id: genId(),
        visible: true,
        columnSpan: 'half',
        background: {
          type: 'gradient',
          gradient: { from: '#9A1BFF', to: '#4F46E5', direction: 'to-br' },
        },
        content: {
          type: 'action-card',
          title: 'Título do card',
          textColor: 'light',
          minHeight: 'md',
          button: {
            label: 'Saiba mais',
            variant: 'secondary',
            link: { type: 'internal', href: '#', target: '_self' },
          },
        },
      },
      {
        id: genId(),
        visible: true,
        columnSpan: 'half',
        background: {
          type: 'gradient',
          gradient: { from: '#9A1BFF', to: '#4F46E5', direction: 'to-br' },
        },
        content: {
          type: 'action-card',
          title: 'Está preparado para a próxima grande oportunidade?',
          textColor: 'light',
          minHeight: 'md',
          button: {
            label: 'Atualize seu currículo e destaque-se',
            variant: 'secondary',
            link: { type: 'internal', href: '/page/cvdigital', target: '_self' },
          },
        },
      },
      {
        id: genId(),
        visible: true,
        columnSpan: 'full',
        background: { type: 'none' },
        content: {
          type: 'canal-denuncias',
        },
      },
    ],
  }
}

export const defaultConfig = createDefaultConfig()
