/**
 * Sidebar reutilizável com currículo profissional (mesma lógica do /curriculoProfissional).
 * Header fixo (foto, nome, contato) + abas por seção (Sobre, Experiências, etc.).
 * Pode ser usado em Match Talentos, Gestão de Vagas e outras telas.
 */

import { useState, useEffect, useCallback } from 'react';
import { container } from '@core/di/container';
import { BuscarDadosColaboradorUseCase } from '@domain/usecases/BuscarDadosColaboradorUseCase';
import { ListarEscolaridadeColaboradorUseCase } from '@domain/usecases/ListarEscolaridadeColaboradorUseCase';
import type { BuscarDadosColaboradorResponse } from '@domain/entities/Profile360';
import type { EscolaridadeColaborador } from '@domain/entities/Escolaridade';
import { Sheet, SheetContent, SheetHeader, SheetTitle } from '@/components/ui/sheet';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar';
import { Spinner } from '@/components/ui/spinner';
import { Briefcase, Copy } from 'lucide-react';
import { toast } from 'sonner';
import logoFourmakers from '@/assets/logo-fourmakers.svg';
import { ProfileHeroSkeleton } from '@presentation/components/profile/ProfileHeroSkeleton';
import { AboutSection } from '@presentation/components/profile/AboutSection';
import { ExperienceSection } from '@presentation/components/profile/ExperienceSection';
import { CertificationSection } from '@presentation/components/profile/CertificationSection';
import { EducationSection } from '@presentation/components/profile/EducationSection';
import { SkillsSectionExpanded } from '@presentation/components/profile/SkillsSectionExpanded';
import { DocumentsSection } from '@presentation/components/profile/DocumentsSection';
import type { ColaboradorProfile360 } from '@domain/entities/Profile360';
import { ScrollArea } from '@/components/ui/scroll-area';

export interface CurriculoSidebarProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  /** CPF, código interno ou id (coluna Beta); enviado no param cpf da API BuscarDadosColaborador. */
  identificador: string | null;
  token: string | null;
  /** Título exibido no header do Sheet (ex.: "Perfil 360"). */
  title?: string;
}

function CurriculoSidebarHeader({ colaborador }: { colaborador: ColaboradorProfile360 }) {
  const iniciais = colaborador?.nomeCompleto
    ?.split(' ')
    .map((n) => n[0])
    .join('')
    .toUpperCase()
    .slice(0, 2) ?? '?';
  const status = colaborador?.flagAtivo ? 'Ativo' : 'Inativo';
  const urlLinkedin = colaborador?.urlLinkedin?.trim();

  /** Exibe sempre no formato linkedin.com/in/[perfil]. */
  const linkedinDisplay = (() => {
    if (!urlLinkedin) return '';
    const normalized = urlLinkedin.replace(/^https?:\/\//i, '').replace(/^www\./i, '');
    const match = normalized.match(/linkedin\.com\/in\/(.+)/i);
    const slug = match ? match[1].replace(/\/$/, '') : normalized.replace(/^linkedin\.com\/?/i, '');
    return slug ? `linkedin.com/in/${slug}` : normalized || urlLinkedin;
  })();

  const linkedinHref = urlLinkedin
    ? (urlLinkedin.startsWith('http') ? urlLinkedin : `https://${urlLinkedin}`)
    : '';

  const handleCopyLinkedin = useCallback(() => {
    if (!linkedinHref) return;
    void navigator.clipboard.writeText(linkedinHref).then(() => {
      toast.success('Link do LinkedIn copiado.');
    });
  }, [linkedinHref]);

  return (
    <div className="relative overflow-hidden rounded-xl bg-gradient-to-br from-violet-700 via-indigo-600 to-blue-600 p-5 shadow-lg">
      {/* Logo */}
      <div className="absolute top-4 right-4">
        <img
          src={logoFourmakers}
          alt="FourMakers"
          className="h-7 w-auto brightness-0 invert opacity-90"
        />
      </div>

      <div className="flex gap-4 items-start">
        <Avatar className="h-20 w-20 border-2 border-white/30 shadow-md shrink-0">
          <AvatarImage src={colaborador?.urlFotoThumb ?? colaborador?.urlFoto} />
          <AvatarFallback className="text-xl font-semibold text-white bg-white/20">
            {iniciais}
          </AvatarFallback>
        </Avatar>

        <div className="min-w-0 flex-1 space-y-2 pr-8">
          <div className="flex flex-wrap items-center gap-2">
            <h3 className="text-lg font-bold tracking-tight text-white truncate">
              {colaborador?.nomeCompleto ?? '—'}
            </h3>
            <span className="inline-flex items-center rounded-full bg-white/20 px-2.5 py-0.5 text-xs font-medium text-white border border-white/30">
              {status}
            </span>
          </div>

          {urlLinkedin && linkedinDisplay && (
            <div className="flex items-center gap-2 text-white/90 text-sm">
              <Briefcase className="h-4 w-4 shrink-0 opacity-90" />
              <a
                href={linkedinHref}
                target="_blank"
                rel="noopener noreferrer"
                className="truncate hover:underline flex-1 min-w-0"
              >
                {linkedinDisplay}
              </a>
              <button
                type="button"
                onClick={handleCopyLinkedin}
                className="shrink-0 p-1 rounded hover:bg-white/20 transition-colors"
                aria-label="Copiar link do LinkedIn"
              >
                <Copy className="h-4 w-4" />
              </button>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}

const TAB_KEYS = ['sobre', 'experiencias', 'certificacoes', 'educacao', 'habilidades', 'documentos'] as const;

export function CurriculoSidebar({
  open,
  onOpenChange,
  identificador,
  token,
  title = 'Currículo profissional',
}: CurriculoSidebarProps) {
  const [dados, setDados] = useState<BuscarDadosColaboradorResponse | null>(null);
  const [escolaridades, setEscolaridades] = useState<EscolaridadeColaborador[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!open || !identificador || !token) {
      setDados(null);
      setEscolaridades([]);
      setError(null);
      return;
    }

    let cancelled = false;

    const load = async () => {
      setLoading(true);
      setError(null);
      try {
        const useCase = container.resolve(BuscarDadosColaboradorUseCase);
        const response = await useCase.execute(token, identificador.trim());
        if (cancelled) return;
        if (response.sucesso) {
          setDados(response);
        } else {
          setError(response.mensagem ?? 'Erro ao carregar dados.');
        }

        if (response.sucesso && response.colaborador) {
          const escUseCase = container.resolve(ListarEscolaridadeColaboradorUseCase);
          const escResponse = await escUseCase.execute(token, {
            busca: identificador.trim(),
            cursor: 0,
            limite: 500,
          });
          if (!cancelled && escResponse.sucesso && escResponse.escolaridade) {
            setEscolaridades(escResponse.escolaridade);
          }
        }
      } catch (err) {
        if (!cancelled) {
          setError((err as Error)?.message ?? 'Erro ao carregar currículo.');
        }
      } finally {
        if (!cancelled) setLoading(false);
      }
    };

    load();
    return () => {
      cancelled = true;
    };
  }, [open, identificador, token]);

  return (
    <Sheet open={open} onOpenChange={onOpenChange}>
      <SheetContent
        side="right"
        className="w-full sm:max-w-xl p-0 flex flex-col overflow-hidden"
        aria-labelledby="curriculo-sidebar-title"
      >
        <SheetHeader className="shrink-0 px-6 pt-6 pb-2">
          <SheetTitle id="curriculo-sidebar-title">{title}</SheetTitle>
        </SheetHeader>

        {loading && (
          <div className="flex-1 flex flex-col items-center justify-center gap-4 px-6 py-8">
            <Spinner size={32} className="text-primary" />
            <p className="text-sm text-muted-foreground">Carregando currículo...</p>
            <div className="w-full max-w-sm rounded-lg border bg-card p-6">
              <ProfileHeroSkeleton />
            </div>
          </div>
        )}

        {error && !loading && (
          <div className="flex-1 flex items-center justify-center px-6">
            <p className="text-sm text-destructive text-center">{error}</p>
          </div>
        )}

        {dados && dados.colaborador && !loading && (
          <>
            <div className="shrink-0 px-4 pt-2">
              <CurriculoSidebarHeader colaborador={dados.colaborador} />
            </div>
            <Tabs defaultValue={TAB_KEYS[0]} className="flex-1 flex flex-col min-h-0">
              <div className="shrink-0 mx-4 mt-3 border-b overflow-x-auto scrollbar-thin">
                <TabsList className="w-max min-w-full justify-start rounded-none border-0 bg-transparent p-0 h-auto flex flex-nowrap gap-2 pb-2">
                  <TabsTrigger value={TAB_KEYS[0]} variant="primary" className="rounded-full shrink-0 px-4 py-1.5 text-sm">
                    Sobre
                  </TabsTrigger>
                  <TabsTrigger value={TAB_KEYS[1]} variant="primary" className="rounded-full shrink-0 px-4 py-1.5 text-sm">
                    Experiências
                  </TabsTrigger>
                  <TabsTrigger value={TAB_KEYS[2]} variant="primary" className="rounded-full shrink-0 px-4 py-1.5 text-sm">
                    Certificações
                  </TabsTrigger>
                  <TabsTrigger value={TAB_KEYS[3]} variant="primary" className="rounded-full shrink-0 px-4 py-1.5 text-sm">
                    Educação
                  </TabsTrigger>
                  <TabsTrigger value={TAB_KEYS[4]} variant="primary" className="rounded-full shrink-0 px-4 py-1.5 text-sm">
                    Habilidades
                  </TabsTrigger>
                  <TabsTrigger value={TAB_KEYS[5]} variant="primary" className="rounded-full shrink-0 px-4 py-1.5 text-sm">
                    Documentos
                  </TabsTrigger>
                </TabsList>
              </div>
              <ScrollArea className="flex-1 min-h-0 px-6 pb-6">
                <TabsContent value={TAB_KEYS[0]} className="mt-4 focus-visible:outline-none">
                  <AboutSection sobre={dados.colaborador.sobre} readOnly />
                </TabsContent>
                <TabsContent value={TAB_KEYS[1]} className="mt-4 focus-visible:outline-none">
                  <ExperienceSection experiencias={dados.perfilProfissional.experienciaEmpresas} readOnly />
                </TabsContent>
                <TabsContent value={TAB_KEYS[2]} className="mt-4 focus-visible:outline-none">
                  <CertificationSection certificados={dados.perfilProfissional.certificados} readOnly />
                </TabsContent>
                <TabsContent value={TAB_KEYS[3]} className="mt-4 focus-visible:outline-none">
                  <EducationSection
                    escolaridades={escolaridades}
                    escolaridade={dados.colaborador.escolaridade}
                    readOnly
                  />
                </TabsContent>
                <TabsContent value={TAB_KEYS[4]} className="mt-4 focus-visible:outline-none">
                  <SkillsSectionExpanded
                    competencias={dados.perfilProfissional.competencias}
                    dominios={dados.perfilProfissional.dominios}
                    metodologias={dados.perfilProfissional.metodologias}
                    softskills={dados.perfilProfissional.softskills}
                    idiomas={dados.perfilProfissional.idiomas}
                  />
                </TabsContent>
                <TabsContent value={TAB_KEYS[5]} className="mt-4 focus-visible:outline-none">
                  <DocumentsSection
                    vistos={dados.colaborador.vistos}
                    passaportes={dados.colaborador.passaportes}
                    readOnly
                  />
                </TabsContent>
              </ScrollArea>
            </Tabs>
          </>
        )}
      </SheetContent>
    </Sheet>
  );
}
