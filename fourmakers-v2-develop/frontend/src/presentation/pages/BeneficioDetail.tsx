import { useState, useEffect, useCallback } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { ArrowLeft, Star, Gift, ExternalLink, Paperclip } from 'lucide-react';
import { container } from '@core/di/container';
import { DiTokens } from '@core/di/tokens';
import type { BeneficioXanoApi } from '@data/api/BeneficioXanoApi';
import type { BeneficioXanoDetail } from '@domain/entities/BeneficioXano';
import { Card, CardContent } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Separator } from '@/components/ui/separator';
import { Spinner } from '@/components/ui/spinner';

const BADGE_CLASS = 'bg-muted text-muted-foreground border border-border/60';

export default function BeneficioDetail() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const api = container.resolve<BeneficioXanoApi>(DiTokens.beneficioXanoApi);

  const [beneficio, setBeneficio] = useState<BeneficioXanoDetail | null>(null);
  const [loading, setLoading] = useState(true);

  const loadDetail = useCallback(async () => {
    const numId = id ? Number(id) : NaN;
    if (Number.isNaN(numId)) {
      setBeneficio(null);
      setLoading(false);
      return;
    }
    setLoading(true);
    try {
      const data = await api.obterPorId(numId);
      setBeneficio(data);
    } catch {
      setBeneficio(null);
    } finally {
      setLoading(false);
    }
  }, [api, id]);

  useEffect(() => {
    loadDetail();
  }, [loadDetail]);

  if (loading) {
    return (
      <div className="flex items-center justify-center min-h-[300px]">
        <Spinner className="h-8 w-8 text-muted-foreground" />
      </div>
    );
  }

  if (!beneficio) {
    return (
      <div className="p-4 space-y-4">
        <Button variant="ghost" onClick={() => navigate(-1)} className="gap-2">
          <ArrowLeft className="w-4 h-4" />
          Voltar
        </Button>
        <p className="text-muted-foreground">Benefício não encontrado.</p>
      </div>
    );
  }

  const dest = beneficio.url_destino;
  const media = typeof beneficio.media === 'number' ? beneficio.media : 0;
  const hasDestino =
    dest && (dest.titulo || dest.descricao || (dest.label_botao1 && dest.url1) || (dest.label_botao2 && dest.url2));

  return (
    <div className="min-h-screen bg-background">
      <main className="container mx-auto px-4 py-6 max-w-4xl">
        <div className="transition-opacity">
          <Button
            variant="ghost"
            onClick={() => navigate(-1)}
            className="mb-6 gap-2"
          >
            <ArrowLeft className="w-4 h-4" />
            Voltar
          </Button>

        <div className="space-y-4">
        {/* Hero card */}
        <Card className="shadow-softToken rounded-lgToken overflow-hidden">
          {/* Logo à esquerda + nome à direita + tag no canto superior direito */}
          <CardContent className="p-0">
            <div className="relative flex items-center gap-4 p-4">
              {/* Tag tipo — canto superior direito */}
              <span
                className={`absolute top-3 right-3 inline-flex items-center text-xs font-medium px-2.5 py-0.5 rounded-full ${BADGE_CLASS}`}
              >
                {beneficio.tipo || 'Benefício'}
              </span>

              {/* Logo — sem border-radius */}
              <div className="w-20 h-20 bg-muted/40 flex items-center justify-center overflow-hidden flex-shrink-0 border border-border/40">
                {beneficio.miniatura?.url ? (
                  <img
                    src={beneficio.miniatura.url}
                    alt={beneficio.nome}
                    className="w-full h-full object-contain p-2"
                  />
                ) : (
                  <Gift className="w-10 h-10 text-muted-foreground/30" />
                )}
              </div>

              {/* Informações — nome, segmento, avaliação */}
              <div className="flex-1 min-w-0 pr-20 space-y-1.5">
                <h1 className="text-lg font-bold text-primaryText leading-tight">
                  {beneficio.nome}
                </h1>
                {beneficio.segmento && (
                  <p className="text-sm text-muted-foreground">{beneficio.segmento}</p>
                )}
                {media > 0 && (
                  <div className="flex items-center gap-1">
                    {[1, 2, 3, 4, 5].map((star) => (
                      <Star
                        key={star}
                        className={`w-3.5 h-3.5 ${
                          star <= Math.round(media)
                            ? 'fill-amber-400 text-amber-400'
                            : 'text-muted-foreground/30'
                        }`}
                      />
                    ))}
                    <span className="text-xs text-muted-foreground ml-0.5">{media.toFixed(1)}</span>
                  </div>
                )}
              </div>
            </div>
          </CardContent>
        </Card>

        {/* FAQ */}
        {beneficio.perguntas && beneficio.perguntas.length > 0 && (
          <Card className="shadow-softToken rounded-lgToken overflow-hidden">
            <div className="px-5 py-4 border-b border-border/50">
              <h2 className="text-sm font-semibold text-primaryText">Informações</h2>
            </div>
            <CardContent className="p-0">
              {beneficio.perguntas.map((p, idx) => (
                <div key={idx}>
                  {idx > 0 && <Separator />}
                  <div className="px-5 py-4 space-y-2">
                    <p className="text-sm font-semibold text-primaryText">{p.pergunta}</p>
                    <p className="text-sm text-muted-foreground leading-relaxed whitespace-pre-wrap">
                      {p.resposta}
                    </p>
                    {p.link && (
                      <a
                        href={p.link}
                        target="_blank"
                        rel="noopener noreferrer"
                        className="inline-flex items-center gap-1.5 text-sm text-primary hover:underline font-medium"
                      >
                        <ExternalLink className="w-3.5 h-3.5" />
                        Ver link
                      </a>
                    )}
                    {p.anexo?.url && (
                      <div className="mt-2">
                        {p.anexo.mime?.startsWith('image/') ? (
                          <img
                            src={p.anexo.url}
                            alt={p.anexo.name || 'Anexo'}
                            className="max-w-full rounded-lg border border-border/50"
                          />
                        ) : (
                          <a
                            href={p.anexo.url}
                            target="_blank"
                            rel="noopener noreferrer"
                            className="inline-flex items-center gap-1.5 text-sm text-primary hover:underline font-medium"
                          >
                            <Paperclip className="w-3.5 h-3.5" />
                            {p.anexo.name || 'Baixar anexo'}
                          </a>
                        )}
                      </div>
                    )}
                  </div>
                </div>
              ))}
            </CardContent>
          </Card>
        )}

        {/* Como aderir / URL destino */}
        {hasDestino && (
          <Card className="shadow-softToken rounded-lgToken overflow-hidden border-primary/20">
            <div className="px-5 py-4 bg-primarySoft/50 border-b border-primary/15">
              <h2 className="text-sm font-semibold text-primaryText">
                {dest.titulo || 'Como aderir?'}
              </h2>
            </div>
            <CardContent className="p-5 space-y-4">
              {dest.descricao && (
                <p className="text-sm text-muted-foreground leading-relaxed whitespace-pre-wrap">
                  {dest.descricao}
                </p>
              )}
              {(dest.label_botao1 && dest.url1) || (dest.label_botao2 && dest.url2) ? (
                <div className="flex flex-wrap gap-2 pt-1">
                  {dest.label_botao1 && dest.url1 && (
                    <Button asChild>
                      <a href={dest.url1} target="_blank" rel="noopener noreferrer" className="gap-1.5">
                        <ExternalLink className="w-3.5 h-3.5" />
                        {dest.label_botao1}
                      </a>
                    </Button>
                  )}
                  {dest.label_botao2 && dest.url2 && (
                    <Button variant="secondary" asChild>
                      <a href={dest.url2} target="_blank" rel="noopener noreferrer" className="gap-1.5">
                        <ExternalLink className="w-3.5 h-3.5" />
                        {dest.label_botao2}
                      </a>
                    </Button>
                  )}
                </div>
              ) : null}
            </CardContent>
          </Card>
        )}
        </div>
        </div>
      </main>
    </div>
  );
}
