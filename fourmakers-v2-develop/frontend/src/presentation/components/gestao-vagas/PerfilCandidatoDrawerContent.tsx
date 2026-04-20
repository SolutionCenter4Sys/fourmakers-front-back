import { useState, useEffect } from 'react';
import { container } from '@core/di/container';
import { Button } from '@/components/ui/button';
import { ArrowLeft } from '@/components/ui/system-icons';
import { ProfileHero } from '@presentation/components/profile/ProfileHero';
import { AboutSection } from '@presentation/components/profile/AboutSection';
import { ExperienceSection } from '@presentation/components/profile/ExperienceSection';
import { CertificationSection } from '@presentation/components/profile/CertificationSection';
import { EducationSection } from '@presentation/components/profile/EducationSection';
import { SkillsSection } from '@presentation/components/profile/SkillsSection';
import { DocumentsSection } from '@presentation/components/profile/DocumentsSection';
import { ProfileHeroSkeleton } from '@presentation/components/profile/ProfileHeroSkeleton';
import { AboutSectionSkeleton } from '@presentation/components/profile/AboutSectionSkeleton';
import { ExperienceSectionSkeleton } from '@presentation/components/profile/ExperienceSectionSkeleton';
import { CertificationSectionSkeleton } from '@presentation/components/profile/CertificationSectionSkeleton';
import { EducationSectionSkeleton } from '@presentation/components/profile/EducationSectionSkeleton';
import { SkillsSectionSkeleton } from '@presentation/components/profile/SkillsSectionSkeleton';
import { DocumentsSectionSkeleton } from '@presentation/components/profile/DocumentsSectionSkeleton';
import { BuscarDadosColaboradorUseCase } from '@domain/usecases/BuscarDadosColaboradorUseCase';
import { ListarEscolaridadeColaboradorUseCase } from '@domain/usecases/ListarEscolaridadeColaboradorUseCase';
import type { BuscarDadosColaboradorResponse } from '@domain/entities/Profile360';
import type { EscolaridadeColaborador } from '@domain/entities/Escolaridade';

export interface PerfilCandidatoDrawerContentProps {
  codigoInternoColaborador: string | null;
  token: string | null;
  onClose: () => void;
}

export function PerfilCandidatoDrawerContent({
  codigoInternoColaborador,
  token,
  onClose,
}: PerfilCandidatoDrawerContentProps) {
  const [dados, setDados] = useState<BuscarDadosColaboradorResponse | null>(null);
  const [escolaridades, setEscolaridades] = useState<EscolaridadeColaborador[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!codigoInternoColaborador || !token) {
      setLoading(false);
      return;
    }
    setLoading(true);
    setError(null);
    setDados(null);

    const carregarDados = async () => {
      try {
        const useCase = container.resolve(BuscarDadosColaboradorUseCase);
        const response = await useCase.execute(token, codigoInternoColaborador);
        if (response.sucesso) {
          setDados(response);
        } else {
          setError(response.mensagem || 'Erro ao carregar dados do colaborador');
        }
        try {
          const escolaridadeUseCase = container.resolve(ListarEscolaridadeColaboradorUseCase);
          const escolaridadeResponse = await escolaridadeUseCase.execute(token, {
            busca: codigoInternoColaborador,
            cursor: 0,
            limite: 500,
          });
          if (escolaridadeResponse.sucesso) {
            setEscolaridades(escolaridadeResponse.escolaridade);
          }
        } catch {
          // não bloquear
        }
      } catch (err: unknown) {
        setError(err instanceof Error ? err.message : 'Erro ao carregar dados do colaborador');
      } finally {
        setLoading(false);
      }
    };

    carregarDados();
  }, [token, codigoInternoColaborador]);

  if (!codigoInternoColaborador) {
    return (
      <div className="p-4 text-sm text-muted-foreground">
        Selecione um candidato para ver o perfil.
      </div>
    );
  }

  if (loading) {
    return (
      <div className="flex flex-col h-full overflow-auto bg-primaryBackground p-4">
        <Button variant="ghost" onClick={onClose} className="mb-2 self-start">
          <ArrowLeft className="h-4 w-4 mr-2" />
          Fechar
        </Button>
        <div className="rounded-2xl border bg-card p-6 shadow-sm">
          <ProfileHeroSkeleton />
        </div>
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-6 mt-6">
          <div className="lg:col-span-2 space-y-6">
            <div className="rounded-2xl border bg-card p-6 shadow-sm space-y-8">
              <AboutSectionSkeleton />
              <div className="border-t" />
              <ExperienceSectionSkeleton />
              <div className="border-t" />
              <CertificationSectionSkeleton />
              <div className="border-t" />
              <EducationSectionSkeleton />
            </div>
          </div>
          <div className="lg:col-span-1 space-y-6">
            <div className="rounded-2xl border bg-card p-6 shadow-sm space-y-8">
              <SkillsSectionSkeleton />
              <div className="border-t" />
              <DocumentsSectionSkeleton />
            </div>
          </div>
        </div>
      </div>
    );
  }

  if (error || !dados) {
    return (
      <div className="p-4">
        <Button variant="ghost" onClick={onClose} className="mb-2">
          <ArrowLeft className="h-4 w-4 mr-2" />
          Fechar
        </Button>
        <p className="text-sm text-destructive">{error || 'Erro ao carregar perfil.'}</p>
      </div>
    );
  }

  return (
    <div className="flex flex-col h-full overflow-auto bg-primaryBackground p-4">
      <Button variant="ghost" onClick={onClose} className="mb-2 self-start">
        <ArrowLeft className="h-4 w-4 mr-2" />
        Fechar
      </Button>
      <div className="space-y-6">
        <div className="rounded-2xl border bg-card p-6 shadow-sm">
          <ProfileHero dados={dados.colaborador} readOnly />
        </div>
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
          <div className="lg:col-span-2 space-y-6">
            <div className="rounded-2xl border bg-card p-6 shadow-sm space-y-8">
              <AboutSection sobre={dados.colaborador.sobre} readOnly />
              <div className="border-t" />
              <ExperienceSection experiencias={dados.perfilProfissional.experienciaEmpresas} readOnly />
              <div className="border-t" />
              <CertificationSection certificados={dados.perfilProfissional.certificados} readOnly />
              <div className="border-t" />
              <EducationSection escolaridades={escolaridades} escolaridade={dados.colaborador.escolaridade} readOnly />
            </div>
          </div>
          <div className="lg:col-span-1 space-y-6">
            <div className="rounded-2xl border bg-card p-6 shadow-sm space-y-8">
              <SkillsSection
                competencias={dados.perfilProfissional.competencias}
                dominios={dados.perfilProfissional.dominios}
                metodologias={dados.perfilProfissional.metodologias}
                softskills={dados.perfilProfissional.softskills}
                idiomas={dados.perfilProfissional.idiomas}
              />
              <div className="border-t" />
              <DocumentsSection
                vistos={dados.colaborador.vistos}
                passaportes={dados.colaborador.passaportes}
                readOnly
              />
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
