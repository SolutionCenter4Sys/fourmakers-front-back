import { useState, useEffect, useCallback } from "react";
import { useSearchParams, useNavigate, useLocation } from "react-router-dom";
import { ProfileHero } from "@presentation/components/profile/ProfileHero";
import { AboutSection } from "@presentation/components/profile/AboutSection";
import { ExperienceSection } from "@presentation/components/profile/ExperienceSection";
import { CertificationSection } from "@presentation/components/profile/CertificationSection";
import { EducationSection } from "@presentation/components/profile/EducationSection";
import { SkillsSection } from "@presentation/components/profile/SkillsSection";
import { DocumentsSection } from "@presentation/components/profile/DocumentsSection";
import { ProfileHeroSkeleton } from "@presentation/components/profile/ProfileHeroSkeleton";
import { AboutSectionSkeleton } from "@presentation/components/profile/AboutSectionSkeleton";
import { ExperienceSectionSkeleton } from "@presentation/components/profile/ExperienceSectionSkeleton";
import { CertificationSectionSkeleton } from "@presentation/components/profile/CertificationSectionSkeleton";
import { EducationSectionSkeleton } from "@presentation/components/profile/EducationSectionSkeleton";
import { SkillsSectionSkeleton } from "@presentation/components/profile/SkillsSectionSkeleton";
import { DocumentsSectionSkeleton } from "@presentation/components/profile/DocumentsSectionSkeleton";
import { Button } from "@/components/ui/button";
import { ArrowLeft, Download } from "@/components/ui/system-icons";
import { container } from "@core/di/container";
import { BuscarDadosColaboradorUseCase } from "@domain/usecases/BuscarDadosColaboradorUseCase";
import { ListarEscolaridadeColaboradorUseCase } from "@domain/usecases/ListarEscolaridadeColaboradorUseCase";
import { useAppSelector } from "@app/store/hooks";
import type { BuscarDadosColaboradorResponse } from "@domain/entities/Profile360";
import type { EscolaridadeColaborador } from "@domain/entities/Escolaridade";
import { toast } from "sonner";
import { GeneratePdfModal } from "@presentation/components/profile/GeneratePdfModal";
import {
  generateProfile360PDF,
  type ProfilePdfNameMode,
} from "@shared/utils/generateProfile360PDF";
import { EditarDadosPessoaisModal } from "@presentation/components/gestao-vagas";
import { logUserAction } from "@shared/utils/firebaseAnalytics";

const Profile360 = () => {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const location = useLocation();
  const { token, user } = useAppSelector((state) => state.auth);
  // Aceitar tanto "colaborador" quanto "cpf" como parâmetro
  const colaboradorCpf = searchParams.get("colaborador") || searchParams.get("cpf");
  
  const [dados, setDados] = useState<BuscarDadosColaboradorResponse | null>(null);
  const [escolaridades, setEscolaridades] = useState<EscolaridadeColaborador[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [pdfModalOpen, setPdfModalOpen] = useState(false);
  const [editarDadosOpen, setEditarDadosOpen] = useState(false);

  // Obter rota de origem do state, com fallback.
  // Mantemos o restante do state para voltar com o contexto completo (vaga selecionada no kanban).
  const backState = (location.state as Record<string, unknown> | null) ?? null;
  const rotaOrigem = (backState?.from as string | undefined) || "/gestao-desempenho-gestor";

  const handleGoBack = () => {
    if (!backState) {
      navigate(rotaOrigem);
      return;
    }
    const nextState = { ...backState };
    delete (nextState as { from?: string }).from;
    navigate(rotaOrigem, { state: nextState });
  };

  const carregarDados = useCallback(async () => {
    if (!token) {
      setError("Token de autenticação não encontrado");
      setLoading(false);
      return;
    }
    
    if (!colaboradorCpf) {
      setError("CPF ou código do colaborador não fornecido");
      setLoading(false);
      return;
    }

    setLoading(true);
    try {
      const useCase = container.resolve(BuscarDadosColaboradorUseCase);
      const response = await useCase.execute(token, colaboradorCpf);
      
      if (response.sucesso) {
        setDados(response);
      } else {
        setError(response.mensagem || "Erro ao carregar dados do colaborador");
      }

      // Carregar escolaridade
      try {
        const escolaridadeUseCase = container.resolve(ListarEscolaridadeColaboradorUseCase);
        const escolaridadeResponse = await escolaridadeUseCase.execute(token, {
          busca: colaboradorCpf,
          cursor: 0,
          limite: 500,
        });

        if (escolaridadeResponse.sucesso) {
          setEscolaridades(escolaridadeResponse.escolaridade);
        }
      } catch (escolaridadeErr) {
        console.error("Erro ao buscar escolaridade:", escolaridadeErr);
      }
    } catch (err: unknown) {
      console.error("Erro ao buscar dados do colaborador:", err);
      const message = err instanceof Error ? err.message : "Erro ao carregar dados do colaborador";
      setError(message);
    } finally {
      setLoading(false);
    }
  }, [token, colaboradorCpf]);

  useEffect(() => {
    carregarDados();
  }, [carregarDados]);

  if (loading) {
    return (
      <div className="min-h-screen bg-primaryBackground p-6">
        <div className="mx-auto max-w-7xl space-y-6">
          <div className="mb-2 flex items-center justify-between gap-3">
            <Button variant="ghost" size="icon" onClick={handleGoBack} aria-label="Voltar">
              <ArrowLeft className="h-5 w-5" />
            </Button>
            <Button variant="ghost" disabled>
              <Download className="h-4 w-4 mr-2" />
              Baixar PDF
            </Button>
          </div>

          {/* Hero Section Skeleton */}
          <div className="rounded-2xl border bg-card p-6 shadow-sm">
            <ProfileHeroSkeleton />
          </div>

          {/* Two Column Layout Skeleton */}
          <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
            {/* Left Column - 2/3 width */}
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

            {/* Right Column - 1/3 width */}
            <div className="lg:col-span-1 space-y-6">
              <div className="rounded-2xl border bg-card p-6 shadow-sm space-y-8">
                <SkillsSectionSkeleton />
                <div className="border-t" />
                <DocumentsSectionSkeleton />
              </div>
            </div>
          </div>
        </div>
      </div>
    );
  }

  if (error || !dados) {
    return (
      <div className="min-h-screen bg-primaryBackground p-6">
        <div className="mx-auto max-w-7xl">
          <div className="mb-2">
            <Button variant="ghost" size="icon" onClick={handleGoBack} aria-label="Voltar">
              <ArrowLeft className="h-5 w-5" />
            </Button>
          </div>
          <div className="text-center py-8 text-destructive">
            {error || "Erro ao carregar dados do colaborador"}
          </div>
        </div>
      </div>
    );
  }

  const handleDownloadPdf = async (nameMode: ProfilePdfNameMode) => {
    try {
      await generateProfile360PDF({
        dados,
        escolaridades,
        nameMode,
      });
      setPdfModalOpen(false);
      logUserAction("Profile360", "GerarPDFCurriculo", { nameMode }, user);
      toast.success("PDF gerado com sucesso.");
    } catch (downloadError) {
      console.error("Erro ao gerar PDF do perfil", downloadError);
      logUserAction(
        "Profile360",
        "GerarPDFCurriculoErro",
        { nameMode, error: downloadError instanceof Error ? downloadError.message : "unknown_error" },
        user,
      );
      toast.error("Não foi possível gerar o PDF do perfil.");
    }
  };

  const handleOpenPdfModal = () => {
    logUserAction("Profile360", "AbrirModalGerarPDF", { colaboradorCpf }, user);
    setPdfModalOpen(true);
  };

  return (
    <div className="min-h-screen bg-primaryBackground p-6">
      <div className="mx-auto max-w-7xl space-y-6">
        <div className="mb-2 flex items-center justify-between gap-3">
          <Button variant="ghost" size="icon" onClick={handleGoBack} aria-label="Voltar">
            <ArrowLeft className="h-5 w-5" />
          </Button>
          <Button variant="ghost" onClick={handleOpenPdfModal}>
            <Download className="h-4 w-4 mr-2" />
            Baixar PDF
          </Button>
        </div>

        {/* Hero Section */}
        <div className="rounded-2xl border bg-card p-6 shadow-sm">
          <ProfileHero
            dados={dados.colaborador}
            token={token}
            readOnly
            onEditData={() => setEditarDadosOpen(true)}
          />
        </div>

        {/* Two Column Layout */}
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
          {/* Left Column - 2/3 width */}
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

          {/* Right Column - 1/3 width */}
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

      <GeneratePdfModal
        open={pdfModalOpen}
        onOpenChange={setPdfModalOpen}
        onDownload={handleDownloadPdf}
      />
      <EditarDadosPessoaisModal
        open={editarDadosOpen}
        onOpenChange={setEditarDadosOpen}
        token={token}
        codigoInternoColaborador={dados?.colaborador?.cpf ?? null}
        nomeCandidato={dados?.colaborador?.nomeCompleto}
        onSaved={carregarDados}
      />
    </div>
  );
};

export default Profile360;
