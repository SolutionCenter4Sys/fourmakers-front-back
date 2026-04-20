/**
 * Conteúdo de release/novidade exibido em modal ao acessar uma tela.
 * Taggeado pelo código do recurso no menu (ex.: agendas_comerciais).
 */

export interface ReleaseContentItem {
  /** Título da novidade */
  titulo: string;
  /** Data de inserção (YYYY-MM-DD); o mais recente é exibido primeiro */
  dataInsercao: string;
  /** Texto/descrição da feature (pode conter quebras de linha) */
  conteudo: string;
  /** URL de mídia (vídeo, etc.). Se vazio, o botão de mídia não é exibido */
  midiaUrl?: string;
  /** Label do botão de mídia (ex.: "Ver vídeo") */
  midiaLabel?: string;
}

/** Mapa: código do recurso no menu -> lista de releases (mais recente primeiro) */
export type ReleaseContentsByMenuCode = Record<string, ReleaseContentItem[]>;
