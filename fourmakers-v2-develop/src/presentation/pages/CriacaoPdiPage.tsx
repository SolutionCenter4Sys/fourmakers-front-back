import { useState, useCallback } from "react";
import { useParams, useNavigate, useLocation } from "react-router-dom";
import { useAppSelector } from "@app/store/hooks";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";
import { Input } from "@/components/ui/input";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "@/components/ui/popover";
import {
  ArrowLeft,
  User,
  Check,
  X,
  Search,
  Target,
  FileText,
} from "@/components/ui/system-icons";
import { ColaboradorPdiApi } from "@data/api/ColaboradorPdiApi";
import { SkillsApi } from "@data/api/SkillsApi";
import type { PdiSkillInputDTO } from "@shared/types/pdiApi";
import type { SkillSearchResult } from "@data/api/SkillsApi";
import type { MinhaJornadaSkillType } from "@domain/entities/MinhaJornadaSkill";
import { useColaboradorDesempenhoDetalhes } from "@/presentation/hooks/useGestaoDesempenho";
import { cn } from "@/lib/utils";
import avatarIan from "@/assets/avatar.png";

const WORKFLOW_STEPS = [
  "1 Criar o PDI",
  "2 Definição de planos",
  "3 Aprovação gestor",
  "4 Planos em andamento",
  "5 Planos concluídos",
];

const COMPETENCIA_PAI_OPTIONS: {
  value: MinhaJornadaSkillType;
  label: string;
}[] = [
  { value: "hard", label: "Hard skill" },
  { value: "soft", label: "Soft skill" },
  { value: "methodology", label: "Metodologia" },
  { value: "domain", label: "Domínio de negócio" },
  { value: "language", label: "Idioma" },
];

const api = new ColaboradorPdiApi();
const skillsApi = new SkillsApi();

/**
 * Tela de criação de PDI — um único componente para os dois fluxos:
 * - /pdi/novo (colaborador: "Meu PDI") → FourTalent oculto
 * - /gestao-desempenho-gestor/:codColaborador/novo-pdi (gestor criando para colaborador) → FourTalent exibido
 */
export default function CriacaoPdiPage() {
  const { codColaborador: codColaboradorParam } = useParams<{
    codColaborador?: string;
  }>();
  const navigate = useNavigate();
  const location = useLocation();
  const pathname = location.pathname || "";
  const { token, codColaborador: authCodColaborador, user } = useAppSelector(
    (state) => state.auth,
  );
  const isMeuPdi = pathname.startsWith("/pdi/novo") || !codColaboradorParam;
  const codColaborador = codColaboradorParam || authCodColaborador || "";
  const stateNome = (location.state as { colaboradorNome?: string })
    ?.colaboradorNome;
  const { detalhes } = useColaboradorDesempenhoDetalhes(
    codColaboradorParam || "",
  );
  const colaboradorNome = isMeuPdi
    ? "Você"
    : (stateNome ?? detalhes?.nome ?? "");
  const nomeBanner = isMeuPdi
    ? (user?.nomeColaborador?.trim().split(/\s+/)[0] || "você")
    : (stateNome ?? detalhes?.nome ?? "você");

  const [tituloPdi, setTituloPdi] = useState("");
  const [definicaoPdi, setDefinicaoPdi] = useState("");
  /** Previsão para conclusão no formato yyyy-MM-dd (igual ao input type="date" do step 2). */
  const [previsaoConclusao, setPrevisaoConclusao] = useState<string>("");
  const [competenciaPai, setCompetenciaPai] = useState<
    MinhaJornadaSkillType | ""
  >("");
  const [skillsSelecionadas, setSkillsSelecionadas] = useState<
    PdiSkillInputDTO[]
  >([]);
  const [buscaCompetencia, setBuscaCompetencia] = useState("");
  const [sugestoesCompetencia, setSugestoesCompetencia] = useState<
    SkillSearchResult[]
  >([]);
  const [popoverCompetenciaOpen, setPopoverCompetenciaOpen] = useState(false);
  const [loadingSugestoes, setLoadingSugestoes] = useState(false);
  const [enviando, setEnviando] = useState(false);

  const handleVoltar = () => {
    if (isMeuPdi) {
      // Voltar para gestão de desempenho do colaborador já na aba PDI
      navigate('/gestao-desempenho-colaborador', { state: { openTab: 'pdi' } });
      return;
    }
    navigate(`/gestao-desempenho-gestor/${codColaboradorParam}`, {
      state: { ...(location.state as object), openTab: 'pdi' },
    });
  };

  const buscarCompetencias = useCallback(
    async (termo: string) => {
      if (!token || !competenciaPai) {
        setSugestoesCompetencia([]);
        return;
      }
      setLoadingSugestoes(true);
      try {
        const lista = await skillsApi.searchSkillsByType(
          token,
          competenciaPai,
          termo,
        );
        const jaAdicionadas = new Set(
          skillsSelecionadas.map((s) => s.codigoSkill),
        );
        setSugestoesCompetencia(
          lista.filter((item) => !jaAdicionadas.has(String(item.id))),
        );
      } catch {
        setSugestoesCompetencia([]);
      } finally {
        setLoadingSugestoes(false);
      }
    },
    [token, competenciaPai, skillsSelecionadas],
  );

  const adicionarSkill = (item: SkillSearchResult) => {
    if (skillsSelecionadas.some((s) => s.codigoSkill === String(item.id)))
      return;
    setSkillsSelecionadas((prev) => [
      ...prev,
      { nomeSkill: item.nome, codigoSkill: String(item.id) },
    ]);
    setBuscaCompetencia("");
    setSugestoesCompetencia([]);
    setPopoverCompetenciaOpen(false);
  };

  const removerSkill = (codigoSkill: string) => {
    setSkillsSelecionadas((prev) =>
      prev.filter((s) => s.codigoSkill !== codigoSkill),
    );
  };

  const handleEnviarPdi = async () => {
    if (!token) {
      toast.error("Sessão inválida. Faça login novamente.");
      return;
    }
    // No fluxo gestor (novo-pdi para colaborador) precisamos do cod para redirect; no "meu PDI" o backend identifica o usuário pelo JWT.
    const codParaRedirect = codColaboradorParam || authCodColaborador;
    if (!isMeuPdi && !codParaRedirect) {
      toast.error("Sessão inválida. Faça login novamente.");
      return;
    }
    const titulo = tituloPdi.trim().slice(0, 200) || "";
    const descricao = definicaoPdi.trim();
    if (!titulo) {
      toast.error("Preencha o título do PDI.");
      return;
    }
    if (!descricao) {
      toast.error("Preencha a definição do PDI.");
      return;
    }
    if (skillsSelecionadas.length === 0) {
      toast.error("Selecione ao menos uma competência.");
      return;
    }

    setEnviando(true);
    try {
      // Previsão para conclusão: input type="date" retorna yyyy-MM-dd; enviar como deadLine no formato datetime .NET com tempo zerado
      const previsaoIso = previsaoConclusao?.trim() || undefined;
      const deadLine = previsaoIso ? `${previsaoIso}T00:00:00.000Z` : undefined;
      // Código do criador: quando o gestor cria para o colaborador, enviar para o backend persistir "Criado por: Gestor"
      const codigoCriador = authCodColaborador || user?.cpf;
      // Não enviar plano de ação automático: o usuário define os planos na etapa 2.
      const payload = {
        titulo: titulo.slice(0, 200),
        descricao,
        skills: skillsSelecionadas,
        actionPlans: [],
        ...(deadLine && { deadLine }),
        ...(!isMeuPdi && codigoCriador && { codigoInternoColaboradorCriacao: codigoCriador }),
      };
      const res = await api.criarPdi(token, payload, isMeuPdi ? undefined : codColaborador);
      toast.success("PDI criado. Agora defina os planos de ação (etapa 2).");
      if (isMeuPdi) {
        navigate(`/pdi/${res.id}`, {
          state: {
            from: "/gestao-desempenho-colaborador",
            fromCriacao: true,
            isMeuPdi: true,
            previsaoConclusao: previsaoIso,
          },
        });
      } else {
        navigate(`/gestao-desempenho-gestor/${codParaRedirect}/pdi/${res.id}`, {
          state: {
            colaboradorNome,
            from: location.state,
            fromCriacao: true,
            isMeuPdi: false,
            criadoPorGestor: true,
            previsaoConclusao: previsaoIso,
          },
        });
      }
    } catch (e: unknown) {
      const msg = e instanceof Error ? e.message : "Erro ao criar PDI.";
      if (msg.includes("404")) {
        toast.error(
          "Rota não encontrada (404). Verifique se a API Gestão de Pessoa/Pdi está disponível.",
        );
      } else {
        toast.error(msg);
      }
    } finally {
      setEnviando(false);
    }
  };

  return (
    <div className="container mx-auto p-4 w-full space-y-6">
      <Button variant="ghost" onClick={handleVoltar} className="mb-2">
        <ArrowLeft className="h-4 w-4 mr-2" />
        Voltar
      </Button>

      {/* Banner — mesmo padrão das outras steps do PDI (Ian + mensagem) */}
      <div
        className="relative overflow-hidden rounded-2xl h-[200px] flex items-center gap-4 sm:gap-6 p-0 pr-6 text-white"
        style={{
          backgroundImage:
            "linear-gradient(135deg, rgba(15, 118, 110, 0.95) 0%, rgba(88, 28, 135, 0.9) 45%, rgba(49, 46, 129, 0.95) 100%), url(/profile-hero-bg.jpg)",
          backgroundSize: "cover",
          backgroundPosition: "center",
        }}
      >
        <div className="self-end">
          <img
            src={avatarIan}
            alt="Ian"
            className="h-32 sm:h-40 w-auto object-contain object-bottom block"
          />
        </div>
        <div className="flex-1 min-w-0 flex items-center py-4">
          <div className="rounded-xl bg-indigo-900/80 backdrop-blur-sm border border-white/10 shadow-lg px-5 py-4 text-left max-w-xl">
            {isMeuPdi ? (
              <p className="text-lg sm:text-xl font-medium leading-snug text-white">
                Crie metas vinculando competências que podem ser evoluídas.
              </p>
            ) : (
              <p className="text-lg sm:text-xl font-medium leading-snug text-white">
                <span className="font-bold text-white bg-white/20 px-1.5 py-0.5 rounded">
                  Olá!
                </span>
                <span className="text-white/95">
                  {" "}
                  Você está criando um PDI para {nomeBanner}. Defina a meta e as
                  competências abaixo.
                </span>
              </p>
            )}
          </div>
        </div>
      </div>

      {/* Stepper — ocupa a largura da área de conteúdo, igual ao restante do sistema */}
      <div className="flex flex-wrap gap-2 p-3 rounded-xl bg-muted/40 w-full">
        {WORKFLOW_STEPS.map((label, i) => (
          <span
            key={label}
            className={cn(
              "px-4 py-2 rounded-lg text-sm font-medium transition-colors",
              i === 0
                ? "bg-primary text-primary-foreground shadow-sm"
                : "bg-background/80 text-muted-foreground border border-border/50",
            )}
          >
            {label}
          </span>
        ))}
      </div>

      {/* FourTalent: só exibe quando for pelo gestor (novo-pdi do colaborador) */}
      {!isMeuPdi && (
        <Card className="mb-6 border-2 border-primary/30 bg-gradient-to-r from-primary/5 to-transparent overflow-hidden">
          <CardContent className="p-4 flex items-center gap-4">
            <div className="rounded-full bg-primary/20 p-3">
              <User className="h-6 w-6 text-primary shrink-0" />
            </div>
            <div>
              <Label className="text-xs font-medium text-muted-foreground uppercase tracking-wide">
                FourTalent
              </Label>
              <p className="font-semibold text-foreground mt-0.5">
                {colaboradorNome || "Carregando..."}
              </p>
            </div>
          </CardContent>
        </Card>
      )}

      {/* Bloco PDI */}
      <Card className="border-2 border-border/50 shadow-sm overflow-hidden">
        <CardContent className="p-6 space-y-6">
          <div className="flex items-start gap-3">
            <div className="rounded-lg bg-primary/10 p-2 shrink-0">
              <Target className="h-5 w-5 text-primary" />
            </div>
            <div>
              <h2 className="text-xl font-semibold text-foreground">PDI</h2>
              <p className="text-sm text-muted-foreground mt-1">
                {isMeuPdi
                  ? "Defina abaixo o plano de desenvolvimento individual."
                  : "Defina abaixo o plano de desenvolvimento individual deste FourTalent."}
              </p>
            </div>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div className="space-y-2 md:col-span-2">
              <Label className="text-sm font-medium">Título *</Label>
              <Input
                placeholder="Ex.: Nome ou tema da sua meta de desenvolvimento"
                value={tituloPdi}
                onChange={(e) => setTituloPdi(e.target.value)}
                maxLength={200}
                className="border-2 focus-visible:ring-2 focus-visible:ring-primary/20"
              />
              <p className="text-xs text-muted-foreground">{tituloPdi.length}/200</p>
            </div>
            <div className="space-y-2 md:col-span-2">
              <Label className="text-sm font-medium">Definição do PDI *</Label>
              <Textarea
                placeholder="Ex.: Desenvolver habilidades em gestão de projetos e liderança para atuar em entregas de maior complexidade."
                value={definicaoPdi}
                onChange={(e) => setDefinicaoPdi(e.target.value)}
                className="min-h-[140px] resize-none border-2 focus-visible:ring-2 focus-visible:ring-primary/20"
              />
            </div>
            <div className="space-y-2">
              <Label className="text-sm font-medium">
                Previsão para conclusão
              </Label>
              <Input
                type="date"
                value={previsaoConclusao}
                onChange={(e) => setPrevisaoConclusao(e.target.value)}
                className="h-11 border-2 border-borderSoft"
              />
            </div>
          </div>

          {/* Competências */}
          <div className="space-y-3 pt-2 border-t border-border/50">
            <div className="flex items-center gap-2">
              <FileText className="h-4 w-4 text-muted-foreground shrink-0" />
              <Label className="text-sm font-medium">
                Selecione a competência *
              </Label>
            </div>
            <p className="text-xs text-muted-foreground">
              Escolha o tipo e pesquise para adicionar uma ou mais competências.
            </p>
            <div className="flex flex-wrap gap-2 items-center">
              {/* Select competência pai */}
              <Select
                value={competenciaPai || "placeholder"}
                onValueChange={(v) => {
                  setCompetenciaPai(
                    v === "placeholder" ? "" : (v as MinhaJornadaSkillType),
                  );
                  setBuscaCompetencia("");
                  setSugestoesCompetencia([]);
                  setPopoverCompetenciaOpen(false);
                }}
              >
                <SelectTrigger className="w-[200px] border-primary/50">
                  <SelectValue placeholder="Selecione" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="placeholder">Selecione</SelectItem>
                  {COMPETENCIA_PAI_OPTIONS.map((opt) => (
                    <SelectItem key={opt.value} value={opt.value}>
                      {opt.label}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>

              {/* Pesquisar competências (filtra pela competência pai) */}
              <Popover
                open={popoverCompetenciaOpen}
                onOpenChange={(open) => {
                  setPopoverCompetenciaOpen(open);
                  if (open && competenciaPai)
                    buscarCompetencias(buscaCompetencia);
                }}
              >
                <PopoverTrigger asChild>
                  <Input
                    placeholder="Pesquisar competências"
                    className="w-[280px]"
                    value={buscaCompetencia}
                    disabled={!competenciaPai}
                    onChange={(e) => {
                      const v = e.target.value;
                      setBuscaCompetencia(v);
                      if (competenciaPai) buscarCompetencias(v);
                    }}
                    onFocus={() => {
                      if (competenciaPai) buscarCompetencias(buscaCompetencia);
                    }}
                  />
                </PopoverTrigger>
                <PopoverContent
                  className="w-[320px] p-0 max-h-[360px] flex flex-col"
                  align="start"
                >
                  {!competenciaPai && (
                    <div className="p-3 text-sm text-muted-foreground">
                      Selecione primeiro o tipo de competência.
                    </div>
                  )}
                  {competenciaPai && (
                    <>
                      <div className="p-2 border-b bg-muted/30">
                        <div className="relative">
                          <Search className="absolute left-2.5 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground pointer-events-none" />
                          <Input
                            placeholder="Buscar competência"
                            className="pl-8 h-9 bg-background"
                            value={buscaCompetencia}
                            onChange={(e) => {
                              const v = e.target.value;
                              setBuscaCompetencia(v);
                              buscarCompetencias(v);
                            }}
                            onKeyDown={(e) => e.stopPropagation()}
                            autoFocus
                          />
                        </div>
                      </div>
                      <div className="overflow-auto max-h-[280px] min-h-[80px]">
                        {loadingSugestoes && (
                          <div className="p-3 text-sm text-muted-foreground">
                            Buscando...
                          </div>
                        )}
                        {!loadingSugestoes &&
                          sugestoesCompetencia.length === 0 && (
                            <div className="p-3 text-sm text-muted-foreground">
                              {buscaCompetencia.trim()
                                ? "Nenhuma competência encontrada."
                                : "Digite para buscar."}
                            </div>
                          )}
                        {!loadingSugestoes &&
                          sugestoesCompetencia.slice(0, 50).map((item) => (
                            <button
                              key={item.id}
                              type="button"
                              className="flex w-full items-center gap-2 px-3 py-2 text-left text-sm hover:bg-muted rounded-sm"
                              onClick={() => adicionarSkill(item)}
                            >
                              <Check className="h-4 w-4 text-primary shrink-0" />
                              {item.nome}
                            </button>
                          ))}
                      </div>
                    </>
                  )}
                </PopoverContent>
              </Popover>
            </div>

            {/* Badges das competências selecionadas (estilo FF) */}
            {skillsSelecionadas.length > 0 && (
              <div className="flex flex-wrap gap-2 mt-2">
                {skillsSelecionadas.map((s) => (
                  <span
                    key={s.codigoSkill}
                    className="inline-flex items-center gap-1 rounded-md bg-blue-500 px-2.5 py-1 text-sm font-medium text-white"
                  >
                    {s.nomeSkill}
                    <button
                      type="button"
                      aria-label="Remover"
                      className="ml-0.5 rounded hover:bg-blue-600 p-0.5"
                      onClick={() => removerSkill(s.codigoSkill)}
                    >
                      <X className="h-3.5 w-3.5" />
                    </button>
                  </span>
                ))}
              </div>
            )}
          </div>

          <div className="flex justify-end pt-6 border-t border-border/50">
            <Button
              variant="primary"
              size="lg"
              className="min-w-[180px] shadow-sm"
              onClick={handleEnviarPdi}
              disabled={
                enviando ||
                !tituloPdi.trim() ||
                !definicaoPdi.trim() ||
                skillsSelecionadas.length === 0
              }
            >
              {enviando ? "Enviando..." : "Enviar PDI"}
            </Button>
          </div>
        </CardContent>
      </Card>
    </div>
  );
}
