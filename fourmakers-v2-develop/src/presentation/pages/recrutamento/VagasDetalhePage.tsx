import { useEffect, useState } from 'react';
import { useParams, useNavigate, useLocation } from 'react-router-dom';
import { container } from '@core/di/container';
import { useAppSelector } from '@app/store/hooks';
import { GetVagaDetalhesUseCase } from '@domain/usecases/GetVagaDetalhesUseCase';
import { CandidatarOutraPessoaUseCase } from '@domain/usecases/CandidatarOutraPessoaUseCase';
import type { VagaDetails } from '@domain/entities/VagaDetails';
import { ArrowLeft, Share2, Copy } from '@/components/ui/system-icons';
import { Button } from '@/components/ui/button';
import { Card, CardContent } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { PageBreadcrumb } from '@presentation/components/common';
import { Spinner } from '@/components/ui/spinner';
import { InscreverTalentoModal } from '@presentation/components/gestao-vagas/InscreverTalentoModal';
import { InscreverCandidatoModal } from '@presentation/components/gestao-vagas/InscreverCandidatoModal';
import { InscricaoPreferenciasModal } from '@presentation/components/gestao-vagas/InscricaoPreferenciasModal';
import { toast } from 'sonner';

/** Agrupa skills por tipo: 1 = técnico (HardSkill), 2 = socioemocional (Softskill), outros = idioma/outros */
function groupSkills(skills: VagaDetails['skills']) {
  if (!Array.isArray(skills) || skills.length === 0) return { tecnicas: [], socioemocionais: [], outros: [] };
  const tecnicas = skills.filter((s) => s.tipoSkillId === 1);
  const socioemocionais = skills.filter((s) => s.tipoSkillId === 2);
  const outros = skills.filter((s) => s.tipoSkillId !== 1 && s.tipoSkillId !== 2);
  return { tecnicas, socioemocionais, outros };
}

/** State opcional ao navegar da listagem: id (guid) da vaga para contexto (ex.: modais). */
type VagasDetalheLocationState = { vagaId?: string; codigoVaga?: number } | null;

export function VagasDetalhePage() {
  const { vagaCodigo } = useParams<{ vagaCodigo?: string }>();
  const location = useLocation();
  const navigate = useNavigate();
  const token = useAppSelector((state) => state.auth.token);
  const state = location.state as VagasDetalheLocationState | undefined;
  /** Segmento da URL: código (ex: 541) ou id (guid) para o use case. */
  const idOrCode = vagaCodigo ?? state?.vagaId ?? null;
  const codigoVaga = state?.codigoVaga ?? (idOrCode && /^\d+$/.test(idOrCode) ? Number(idOrCode) : undefined);
  /** vagaId (guid) para modais: do state ao navegar da listagem ou id retornado pela API ao carregar por código. */
  const vagaIdParaModais = state?.vagaId ?? null;

  const [detalhe, setDetalhe] = useState<VagaDetails | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [inscreverTalentoModalOpen, setInscreverTalentoModalOpen] = useState(false);
  const [cadastrarTalentoModalOpen, setCadastrarTalentoModalOpen] = useState(false);
  const [preferenciasModalOpen, setPreferenciasModalOpen] = useState(false);
  const [codigoColaboradorInscrito, setCodigoColaboradorInscrito] = useState<string | null>(null);

  useEffect(() => {
    if (!token?.trim()) {
      setError('Faça login para visualizar a vaga.');
      setLoading(false);
      return;
    }
    if (!idOrCode?.trim()) {
      setError('Acesse esta página pelo link da vaga no Recrutamento.');
      setLoading(false);
      return;
    }
    setLoading(true);
    setError(null);
    const useCase = container.resolve(GetVagaDetalhesUseCase);
    useCase
      .execute(token, idOrCode.trim())
      .then(setDetalhe)
      .catch((err) => {
        setError(err?.message ?? 'Não foi possível carregar os detalhes da vaga.');
        setDetalhe(null);
      })
      .finally(() => setLoading(false));
  }, [idOrCode, token]);

  const handleVoltar = () => navigate('/recrutamento');
  const codigoParaLink = detalhe?.codigo ?? codigoVaga ?? (idOrCode && /^\d+$/.test(idOrCode) ? Number(idOrCode) : null);

  const handleCompartilhar = () => {
    if (codigoParaLink == null) {
      toast.error('Código da vaga não disponível para compartilhar.');
      return;
    }
    const query = `detalhes=${encodeURIComponent(String(codigoParaLink))}`;
    navigate(`/public/vaga?${query}`, { state: { detalhe: detalhe ?? undefined, codigoVaga: codigoParaLink } });
  };

  /** Abre a página de candidatura pública (se logado, vai direto para etapa 2). */
  const handleMeInscrever = () => {
    if (codigoParaLink == null) {
      toast.error('Código da vaga não disponível.');
      return;
    }
    const query = `detalhes=${encodeURIComponent(String(codigoParaLink))}`;
    navigate(`/public/vaga?${query}`, { state: { detalhe: detalhe ?? undefined, codigoVaga: codigoParaLink } });
  };

  const handleCopiarLink = async () => {
    if (codigoParaLink == null) {
      toast.error('Código da vaga não disponível para copiar o link.');
      return;
    }
    const urlPublica = `${window.location.origin}/public/vaga?detalhes=${encodeURIComponent(String(codigoParaLink))}`;
    try {
      await navigator.clipboard.writeText(urlPublica);
      toast.success('Link copiado para a área de transferência.');
    } catch {
      toast.error('Não foi possível copiar o link.');
    }
  };

  const codigoVagaNum = detalhe?.codigo ?? (codigoVaga ? Number(codigoVaga) : NaN);
  const handleAbrirInscreverTalento = () => {
    if (Number.isNaN(codigoVagaNum) || codigoVagaNum <= 0) {
      toast.error('Código da vaga inválido.');
      return;
    }
    setInscreverTalentoModalOpen(true);
  };

  const handleAbrirCadastrarTalento = () => {
    if (Number.isNaN(codigoVagaNum) || codigoVagaNum <= 0) {
      toast.error('Código da vaga inválido.');
      return;
    }
    setCadastrarTalentoModalOpen(true);
  };

  const handleCadastrarTalentoSuccess = (codigoColaborador: string) => {
    setCadastrarTalentoModalOpen(false);
    setCodigoColaboradorInscrito(codigoColaborador);
    setPreferenciasModalOpen(true);
  };

  const handlePreferenciasClose = (open: boolean) => {
    setPreferenciasModalOpen(open);
    if (!open) setCodigoColaboradorInscrito(null);
  };

  const handleInscreverTalento = async (codigoColaborador: string) => {
    if (!token || !codigoColaborador) return;
    const codigoVagaStr = String(codigoVagaNum);
    if (!codigoVagaStr || codigoVagaStr === 'NaN') {
      toast.error('Código da vaga não disponível.');
      return;
    }
    toast.info('Inscrevendo candidato. Por favor, aguarde...');
    try {
      const useCase = container.resolve(CandidatarOutraPessoaUseCase);
      await useCase.execute(token, {
        codigoVaga: codigoVagaStr,
        codigoColaborador,
        opcoesContatoIds: [],
      });
      toast.success('Candidato inscrito com sucesso!');
      setInscreverTalentoModalOpen(false);
    } catch (e) {
      console.error('Erro ao inscrever candidato', e);
      toast.error('Não foi possível inscrever o candidato.');
    }
  };

  const modalidade = detalhe?.modeloTrabalhoDescricao ?? detalhe?.cidade ?? 'Remoto';
  const estado = detalhe?.estado ? ` (${detalhe.estado})` : '';
  const { tecnicas, socioemocionais } = groupSkills(detalhe?.skills);

  return (
    <div className="container mx-auto p-4 space-y-6 pb-12">
      <PageBreadcrumb
        items={[
          { label: 'Recrutamento', href: '/recrutamento' },
          { label: detalhe?.titulo ?? (codigoVaga ? `Vaga ${codigoVaga}` : 'Detalhe') },
        ]}
      />

      <div className="flex items-center gap-2">
        <Button
          variant="ghost"
          size="icon"
          className="shrink-0 rounded-pillToken h-9 w-9 text-muted-foreground hover:text-foreground"
          onClick={handleVoltar}
          aria-label="Voltar para Recrutamento"
        >
          <ArrowLeft className="h-5 w-5" aria-hidden />
        </Button>
        <span className="text-muted-foreground font-medium">Recrutamento</span>
      </div>

      {loading && (
        <div className="flex justify-center py-12">
          <Spinner className="m-auto" />
        </div>
      )}

      {error && !loading && (
        <Card className="border-borderSoft">
          <CardContent className="p-6 text-center">
            <p className="text-destructive">{error}</p>
            <Button variant="outline" className="mt-4" onClick={handleVoltar}>
              Voltar
            </Button>
          </CardContent>
        </Card>
      )}

      {!loading && !error && detalhe && (
        <Card className="max-w-3xl border-borderSoft bg-card shadow-softToken overflow-hidden">
          <CardContent className="p-6 space-y-6">
            <div className="text-sm text-muted-foreground">
              Código: {detalhe.codigo ?? codigoVaga ?? '—'}
            </div>
            <h1 className="text-xl font-semibold text-primaryText">{detalhe.titulo ?? 'Sem título'}</h1>
            <div>
              <Badge variant="secondary" className="rounded-md bg-primary/10 text-primary">
                {modalidade}{estado}
              </Badge>
            </div>

            {tecnicas.length > 0 && (
              <div>
                <h3 className="text-sm font-medium text-muted-foreground mb-2">Habilidades Técnicas:</h3>
                <div className="flex flex-wrap gap-2">
                  {tecnicas.map((s) => (
                    <Badge
                      key={s.id}
                      variant="outline"
                      className="rounded-md border-borderSoft bg-surfaceSubtle text-primaryText font-normal"
                    >
                      {s.skillDescription} – {s.skillNivelDescription}
                    </Badge>
                  ))}
                </div>
              </div>
            )}

            {socioemocionais.length > 0 && (
              <div>
                <h3 className="text-sm font-medium text-muted-foreground mb-2">Habilidades Socioemocionais:</h3>
                <div className="flex flex-wrap gap-2">
                  {socioemocionais.map((s) => (
                    <Badge
                      key={s.id}
                      variant="outline"
                      className="rounded-md border-borderSoft bg-surfaceSubtle text-primaryText font-normal"
                    >
                      {s.skillDescription} – {s.skillNivelDescription}
                    </Badge>
                  ))}
                </div>
              </div>
            )}

            <div>
              <h3 className="text-sm font-medium text-muted-foreground mb-2">Descrição da vaga</h3>
              <p className="text-sm text-primaryText whitespace-pre-line leading-relaxed">
                {detalhe.descricao ?? 'Sem dados'}
              </p>
            </div>

            <div className="flex flex-wrap items-center gap-3 pt-4 border-t border-borderSoft">
              <Button variant="outline" size="sm" onClick={handleCompartilhar} className="gap-2">
                <Share2 className="h-4 w-4" aria-hidden />
                Compartilhar
              </Button>
              <Button variant="outline" size="sm" onClick={handleCopiarLink} className="gap-2">
                <Copy className="h-4 w-4" aria-hidden />
                Copiar link
              </Button>
              <Button variant="outline" size="sm" onClick={handleAbrirCadastrarTalento}>
                Cadastrar talento
              </Button>
              <Button size="sm" onClick={handleAbrirInscreverTalento}>
                Inscrever talento
              </Button>
              <Button size="sm" onClick={handleMeInscrever}>
                Me inscrever
              </Button>
            </div>
          </CardContent>
        </Card>
      )}

      {!loading && !error && !detalhe && codigoVaga && (
        <Card className="border-borderSoft">
          <CardContent className="p-6 text-center text-muted-foreground">
            Vaga não encontrada.
            <Button variant="outline" className="mt-4 ml-2" onClick={handleVoltar}>
              Voltar
            </Button>
          </CardContent>
        </Card>
      )}

      <InscreverTalentoModal
        open={inscreverTalentoModalOpen}
        onOpenChange={setInscreverTalentoModalOpen}
        token={token}
        onInscrever={handleInscreverTalento}
        vagaId={vagaIdParaModais ?? detalhe?.id ?? undefined}
        vagaTitle={detalhe?.titulo ?? (codigoVaga ? `Vaga ${codigoVaga}` : 'Vaga')}
        vagaRaw={detalhe ?? undefined}
      />

      <InscreverCandidatoModal
        open={cadastrarTalentoModalOpen}
        onOpenChange={setCadastrarTalentoModalOpen}
        token={token}
        onSuccess={handleCadastrarTalentoSuccess}
      />

      {(vagaIdParaModais ?? detalhe?.id) && codigoColaboradorInscrito && !Number.isNaN(codigoVagaNum) && codigoVagaNum > 0 && (
        <InscricaoPreferenciasModal
          open={preferenciasModalOpen}
          onOpenChange={handlePreferenciasClose}
          token={token}
          codigoVaga={codigoVagaNum}
          vagaId={vagaIdParaModais ?? detalhe?.id ?? ''}
          vagaTitle={detalhe?.titulo ?? (codigoVaga ? `Vaga ${codigoVaga}` : 'Vaga')}
          vagaRaw={detalhe ? (detalhe as unknown as Record<string, unknown>) : undefined}
          codigoColaborador={codigoColaboradorInscrito}
          onVerCandidaturas={() => handlePreferenciasClose(false)}
        />
      )}
    </div>
  );
}
