import { useMemo } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { useAppSelector } from '@app/store/hooks';
import { Button } from '@/components/ui/button';
import { Card, CardContent } from '@/components/ui/card';
import { PageBreadcrumb, PageHeader } from '@presentation/components/common';
import { GerarMatchPromptContent } from '@presentation/components/gestao-vagas/GerarMatchPromptContent';
import { useGestaoVagasCandidatos } from '@presentation/hooks/recrutamento'
import { ArrowLeft, Sparkles } from '@/components/ui/system-icons';

export default function GerarMatchPromptPage() {
  const navigate = useNavigate();
  const location = useLocation();
  const token = useAppSelector((state) => state.auth.token);

  const state = location.state as { vagaId?: string; vagaTitle?: string; vagaRaw?: Record<string, unknown> } | null | undefined;
  const vagaId = state?.vagaId ?? null;
  const vagaTitle = state?.vagaTitle ?? 'Candidatos';

  const { inscreverCandidato, candidatos } = useGestaoVagasCandidatos();

  const codigosInscritosNaVaga = useMemo(() => {
    const set = new Set<string>();
    if (!vagaId) return set;
    for (const c of candidatos ?? []) {
      const cod = c.codigo;
      if (cod != null && String(cod).trim() !== '') set.add(String(cod).trim());
      const interno = (c as { retornoMatch?: { codigoInternoColaborador?: string } }).retornoMatch?.codigoInternoColaborador;
      if (interno != null && String(interno).trim() !== '') set.add(String(interno).trim());
    }
    return set;
  }, [vagaId, candidatos]);

  const handleVoltar = () => {
    navigate('/recrutamento/candidatos', { state: { vagaId, vagaTitle, vagaRaw: state?.vagaRaw } });
  };

  if (!vagaId) {
    return (
      <div className="w-full max-w-none mx-auto p-4 space-y-4">
        <PageBreadcrumb
          items={[
            { label: 'Recrutamento', href: '/recrutamento' },
            { label: 'Gerar Match a partir de Prompt' },
          ]}
        />
        <Card>
          <CardContent className="p-8 text-center">
            <p className="text-muted-foreground">
              Nenhuma vaga selecionada. Acesse o Recrutamento, abra o painel de candidatos de uma vaga e use
              &quot;Gerar match&quot; para abrir esta tela.
            </p>
            <Button className="mt-4" variant="outline" onClick={() => navigate('/recrutamento')}>
              <ArrowLeft className="mr-2 h-4 w-4" />
              Voltar para Recrutamento
            </Button>
          </CardContent>
        </Card>
      </div>
    );
  }

  return (
    <div className="w-full max-w-none mx-auto p-4 space-y-4">
      <PageBreadcrumb
        items={[
          {
            label: 'Recrutamento',
            href: '/recrutamento',
            state: { vagaId, vagaTitle, vagaRaw: state?.vagaRaw },
          },
          {
            label: `Candidatos - ${vagaTitle}`,
            href: '/recrutamento/candidatos',
            state: { vagaId, vagaTitle, vagaRaw: state?.vagaRaw },
          },
          { label: 'Gerar Match a partir de Prompt' },
        ]}
      />

      <PageHeader
        title="Gerar Match a partir de Prompt"
        description="Descreva a vaga ou o perfil desejado e a IA buscará profissionais com cálculo de match."
        titlePrefix={
          <Button variant="ghost" size="icon" onClick={handleVoltar} aria-label="Voltar para Candidatos">
            <ArrowLeft className="h-5 w-5" />
          </Button>
        }
      />

      <div className="rounded-lg border border-border bg-card p-4 sm:p-6">
        <div className="flex items-center gap-3 mb-6">
          <div
            className="h-10 w-10 sm:h-12 sm:w-12 rounded-xl bg-primary/10 flex items-center justify-center shrink-0"
            aria-hidden
          >
            <Sparkles className="h-6 w-6 sm:h-7 sm:w-7 text-primary" />
          </div>
          <p className="text-sm text-muted-foreground">
            Use o campo abaixo para descrever a vaga ou o perfil desejado; em seguida clique em &quot;Gerar match&quot; para buscar perfis com cálculo de aderência.
          </p>
        </div>

        <GerarMatchPromptContent
          token={token}
          onInscrever={inscreverCandidato}
          codigosInscritosNaVaga={codigosInscritosNaVaga}
        />
      </div>
    </div>
  );
}
