/**
 * Extrai o ID do vídeo de URLs do YouTube (watch, youtu.be, embed).
 */
const YOUTUBE_ID_REGEX =
  /(?:youtube\.com\/watch\?v=|youtu\.be\/|youtube\.com\/embed\/)([a-zA-Z0-9_-]{11})/i;

export function extractYoutubeVideoId(url: string): string | null {
  const match = url.match(YOUTUBE_ID_REGEX);
  return match ? match[1] : null;
}

/**
 * Gera o HTML seguro para embed do YouTube (iframe).
 */
function youtubeEmbedHtml(videoId: string): string {
  const embedUrl = `https://www.youtube.com/embed/${encodeURIComponent(videoId)}`;
  return `<div class="youtube-embed-wrapper my-3 rounded-lg overflow-hidden bg-black" style="position:relative;padding-bottom:56.25%;height:0;"><iframe src="${embedUrl}" class="absolute top-0 left-0 w-full h-full" frameborder="0" allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture" allowfullscreen title="YouTube"></iframe></div>`;
}

/**
 * Substitui URLs do YouTube no conteúdo HTML por embeds.
 * - Links <a href="...youtube...">...</a> são trocados pelo player.
 * - URLs soltas no texto também são trocadas pelo player.
 */
export function contentWithYoutubeEmbeds(html: string): string {
  if (!html || typeof html !== 'string') return html;

  let result = html;

  // 1) Substituir âncoras cujo href é YouTube pelo embed
  result = result.replace(
    /<a\s+[^>]*href\s*=\s*["'](https?:\/\/[^"']*youtube\.com\/watch\?v=([a-zA-Z0-9_-]{11})[^"']*|https?:\/\/youtu\.be\/([a-zA-Z0-9_-]{11})[^"']*)["'][^>]*>[\s\S]*?<\/a>/gi,
    (match, _url: string, idWatch?: string, idShort?: string) => {
      const id = idWatch || idShort;
      return id ? youtubeEmbedHtml(id) : match;
    },
  );

  // 2) Substituir URLs soltas (não dentro de atributos) por embed
  // Match URL que não está após href=" ou src=" (negative lookbehind) e não está dentro de < >
  result = result.replace(
    /(?<![="'])(https?:\/\/(?:www\.)?(?:youtube\.com\/watch\?v=|youtu\.be\/|youtube\.com\/embed\/)([a-zA-Z0-9_-]{11})(?:[^\s"']*)?)/gi,
    (_match, _fullUrl: string, id: string) => youtubeEmbedHtml(id),
  );

  return result;
}
