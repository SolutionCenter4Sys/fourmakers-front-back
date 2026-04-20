import { Icon } from "@/components/ui/icon";

import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import {
  Accordion,
  AccordionContent,
  AccordionItem,
  AccordionTrigger,
} from "@/components/ui/accordion";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Separator } from "@/components/ui/separator";
import { DataTable } from "@presentation/components/common";
import type { Column } from "@/hooks/useColumnReorder";
import { componentsSpec } from "@presentation/components/documentation/DocComponents";
import {
  Tooltip,
  TooltipContent,
  TooltipProvider,
  TooltipTrigger,
} from "@/components/ui/tooltip";
import { useMemo, useState, useEffect } from "react";
import toolkitContent from "../../../public/design-toolkit.md?raw";

const paletteLight = [
  { name: "Primary", token: "--color-primary", value: "#000000" },
  { name: "Primary Soft", token: "--color-primary-soft", value: "#F1F5F9" },
  { name: "Accent", token: "--color-accent", value: "#9A1BFF" },
  { name: "Accent Soft", token: "--color-accent-soft", value: "#F4EBFF" },
  { name: "Surface", token: "--color-surface-elevated", value: "#FFFFFF" },
  { name: "Surface Subtle", token: "--color-surface-subtle", value: "#F9FAFB" },
  { name: "Border", token: "--color-border-default", value: "#E2E8F0" },
  { name: "Text", token: "--color-primary-text", value: "#0F172A" },
];

const paletteDark = [
  { name: "Primary", token: "--color-primary", value: "#F9FAFB" },
  { name: "Primary Soft", token: "--color-primary-soft", value: "#111827" },
  { name: "Accent", token: "--color-accent", value: "#C084FF" },
  { name: "Accent Soft", token: "--color-accent-soft", value: "#312E81" },
  { name: "Surface", token: "--color-surface-elevated", value: "#0B1120" },
  { name: "Surface Subtle", token: "--color-surface-subtle", value: "#020617" },
  { name: "Border", token: "--color-border-default", value: "#1E293B" },
  { name: "Text", token: "--color-primary-text", value: "#F9FAFB" },
];

const radiusTokens = [
  { name: "XS", token: "--radius-xs", value: "6px" },
  { name: "SM", token: "--radius-sm", value: "8px" },
  { name: "MD", token: "--radius-md", value: "12px" },
  { name: "LG", token: "--radius-lg", value: "20px" },
  { name: "Pill", token: "--radius-pill", value: "999px" },
];

const spacingTokens = [
  { name: "2XS", token: "--space-2xs", value: "4px" },
  { name: "XS", token: "--space-xs", value: "8px" },
  { name: "SM", token: "--space-sm", value: "12px" },
  { name: "MD", token: "--space-md", value: "16px" },
  { name: "LG", token: "--space-lg", value: "24px" },
  { name: "XL", token: "--space-xl", value: "32px" },
];

const shadowTokens = [
  { name: "Soft", token: "--elevation-soft", value: "0 10px 25px rgba(15,23,42,0.06)" },
  { name: "Hover", token: "--elevation-card-hover", value: "0 18px 45px rgba(15,23,42,0.10)" },
];

const principles = [
  { title: "Clareza e foco humano", text: "Interfaces limpas, copy direta, hierarquia visual forte para suportar decisões rápidas." },
  { title: "Energia da marca", text: "Roxo protagonista com verdes de ação. Gradientes diagonais e microbrilhos como acento." },
  { title: "Escalabilidade", text: "Tokens primeiro: toda cor, raio, sombra e espaçamento vivem em variáveis e classes utilitárias." },
];

// Mapear aliases de utilitários para tokens, garantindo que contagens incluam classes aplicadas em componentes.
const tokenAliasMap: Record<string, string[]> = {
  "radius-xs": ["radius-xs", "rounded-xsToken"],
  "radius-sm": ["radius-sm", "rounded-smToken"],
  "radius-md": ["radius-md", "rounded-mdToken"],
  "radius-lg": ["radius-lg", "rounded-lgToken"],
  "radius-pill": ["radius-pill", "rounded-pillToken"],
  "space-2xs": ["space-2xs", "p-2xs", "px-2xs", "py-2xs", "pt-2xs", "pr-2xs", "pb-2xs", "pl-2xs", "gap-2xs", "space-x-2xs", "space-y-2xs"],
  "space-xs": ["space-xs", "p-xs", "px-xs", "py-xs", "pt-xs", "pr-xs", "pb-xs", "pl-xs", "gap-xs", "space-x-xs", "space-y-xs"],
  "space-sm": ["space-sm", "p-sm", "px-sm", "py-sm", "pt-sm", "pr-sm", "pb-sm", "pl-sm", "gap-sm", "space-x-sm", "space-y-sm"],
  "space-md": ["space-md", "p-md", "px-md", "py-md", "pt-md", "pr-md", "pb-md", "pl-md", "gap-md", "space-x-md", "space-y-md"],
  "space-lg": ["space-lg", "p-lg", "px-lg", "py-lg", "pt-lg", "pr-lg", "pb-lg", "pl-lg", "gap-lg", "space-x-lg", "space-y-lg"],
  "space-xl": ["space-xl", "p-xl", "px-xl", "py-xl", "pt-xl", "pr-xl", "pb-xl", "pl-xl", "gap-xl", "space-x-xl", "space-y-xl"],
  "elevation-soft": ["elevation-soft", "shadow-softToken"],
  "elevation-card-hover": ["elevation-card-hover", "shadow-cardHoverToken"],
  "background-brand": ["background-brand", "bg-brand-gradient"],
};

// Alias para componentes (usos reais podem não usar a tag com mesmo nome do guia)
const componentAliasMap: Record<string, string[]> = {
  Dropdown: ["<Select ", "<SelectTrigger", "<SelectField"],
  Counter: ["dependentsIRPF", "variableExpenses", "increment", "decrement"],
  Icon: ["<Icon", "material-symbols-outlined"],
  Snackbar: ["toast(", "<Toast ", "<Toaster", "useToast("],
};

export default function Documentacao() {
  const [openExamples, setOpenExamples] = useState(false);
  const [openTokens, setOpenTokens] = useState(false);
  // Referência de mercado: Glassdoor (React Dev SP ~R$8k-9k/mês) ≈ R$50-60/h base; com encargos/produtividade usamos R$90/h.
  const ROI_RATE = 90;
  const COMPONENT_DEV_HOURS = 3; // horas para construir um componente do zero
  const TOKEN_DEV_HOURS = 1; // horas para criar/refatorar estilos sem tokens

  const formatCurrency = (value: number) =>
    value.toLocaleString("pt-BR", { style: "currency", currency: "BRL", maximumFractionDigits: 0 });

  const buildRoiTitle = (baseHours: number, usageCount: number) => {
    const baseCost = baseHours * ROI_RATE;
    const roiHours = baseHours * usageCount;
    const roiCost = roiHours * ROI_RATE;
    return `Usos no projeto: - $ Dev: (${baseHours}h/${formatCurrency(baseCost)}) - $ ROI: (${roiHours}h/${formatCurrency(roiCost)})`;
  };

  const [copyMessage, setCopyMessage] = useState("");

  const tokenGuide = useMemo(
    () => [
      {
        grupo: "Cores",
        tipo: "Semântico",
        nota: "Nomes refletem propósito (ação, superfície, texto).",
        sections: [
          {
            label: "Cores de Marca e Ação",
            tokens: [
              { id: "primary", label: "Primary", value: "#000000" },
              { id: "primarySoft", label: "Primary Soft", value: "#F1F5F9" },
              { id: "primaryStrong", label: "Primary Strong", value: "#000000" },
              { id: "accent", label: "Accent (Destaque)", value: "#9A1BFF" },
              { id: "accentSoft", label: "Accent Soft", value: "#F4EBFF" },
            ],
          },
          {
            label: "Cores de Componentes",
            tokens: [
              { id: "primaryBackground", label: "Fundo Primário", value: "#F0F5FA" },
              { id: "secondaryBackground", label: "Fundo Secundário", value: "#FFFFFF" },
              { id: "surfaceElevated", label: "Superfície Elevada", value: "#FFFFFF" },
              { id: "surfaceSubtle", label: "Superfície Sutil", value: "#F9FAFB" },
              { id: "borderDefault", label: "Borda Default", value: "#E2E8F0" },
              { id: "borderSoft", label: "Borda Soft", value: "#EEF2FF" },
              { id: "field001", label: "Campo Base", value: "#F9FAFB" },
              { id: "btnPrimary", label: "Botão Primário", value: "#000000" },
              { id: "btnPrimaryHover", label: "Botão Primário Hover", value: "#111827" },
              { id: "btnSecondary", label: "Botão Secundário", value: "#FFFFFF" },
              { id: "btnSecondaryText", label: "Texto Botão Secundário", value: "#0F172A" },
              { id: "btnGhostHover", label: "Botão Ghost Hover", value: "#E5E7EB" },
              { id: "background-brand", label: "Gradiente Brand", value: "linear-gradient(135deg, #9A1BFF 0%, #7B1CE5 40%, #4F46E5 100%)" },
            ],
          },
          {
            label: "Cores de Texto",
            tokens: [
              { id: "primaryText", label: "Texto Primário", value: "#0F172A" },
              { id: "secondaryText", label: "Texto Secundário", value: "#64748B" },
              { id: "placeholder", label: "Placeholder", value: "#9CA3AF" },
              { id: "inverseText", label: "Texto Inverso", value: "#FFFFFF" },
            ],
          },
          {
            label: "Cores Auxiliares (Estados)",
            tokens: [
              { id: "success", label: "Sucesso", value: "#16A34A" },
              { id: "warning", label: "Alerta", value: "#F59E0B" },
              { id: "error", label: "Erro", value: "#DC2626" },
              { id: "info", label: "Info", value: "#2563EB" },
            ],
          },
        ],
      },
      {
        grupo: "Raios",
        tipo: "Estrutural",
        nota: "Escala fixa para consistência entre cards, inputs e botões.",
        tokens: [
          { id: "radius-xs", label: "XS", value: "6px" },
          { id: "radius-sm", label: "SM", value: "8px" },
          { id: "radius-md", label: "MD", value: "12px" },
          { id: "radius-lg", label: "LG", value: "20px" },
          { id: "radius-pill", label: "Pill", value: "999px" },
        ],
      },
      {
        grupo: "Espaçamentos",
        tipo: "Estrutural",
        nota: "Usados em padding/margens para manter ritmo.",
        tokens: [
          { id: "space-2xs", label: "2XS", value: "4px" },
          { id: "space-xs", label: "XS", value: "8px" },
          { id: "space-sm", label: "SM", value: "12px" },
          { id: "space-md", label: "MD", value: "16px" },
          { id: "space-lg", label: "LG", value: "24px" },
          { id: "space-xl", label: "XL", value: "32px" },
        ],
      },
      {
        grupo: "Sombras",
        tipo: "Semântico",
        nota: "Controla profundidade sem alterar cores.",
        tokens: [
          { id: "elevation-soft", label: "Soft", value: "0 10px 25px rgba(15, 23, 42, 0.06)" },
          { id: "elevation-card-hover", label: "Card Hover", value: "0 18px 45px rgba(15, 23, 42, 0.10)" },
        ],
      },
      {
        grupo: "Gradientes",
        tipo: "Marca",
        nota: "Assinatura roxo→verde para heros e CTAs amplos.",
        tokens: [
          {
            id: "background-brand",
            label: "Brand Gradient",
            value: "linear-gradient(135deg, #9A1BFF 0%, #7B1CE5 40%, #4F46E5 100%)",
          },
        ],
      },
    ],
    [],
  );


  const tokenUsageFallback: Record<
    string,
    { total: number; items: Array<{ label: string; count: number }> }
  > = useMemo(
    () => ({
      primary: { total: 0, items: [] },
      primarySoft: { total: 7, items: [{ label: "Página Documentação", count: 5 }, { label: "Cards Gerais", count: 2 }] },
      primaryStrong: { total: 0, items: [] },
      accent: { total: 47, items: [{ label: "Botões CTA", count: 15 }, { label: "Badges", count: 10 }, { label: "Tabela/DataTable", count: 12 }, { label: "Formulários", count: 10 }] },
      accentSoft: { total: 0, items: [] },
      primaryText: { total: 30, items: [{ label: "Tipografia global", count: 10 }, { label: "Documentação", count: 12 }, { label: "Simulador", count: 8 }] },
      secondaryText: { total: 28, items: [{ label: "Subtítulos", count: 12 }, { label: "Tooltips", count: 6 }, { label: "Listas", count: 10 }] },
      placeholder: { total: 0, items: [] },
      surfaceElevated: { total: 25, items: [{ label: "Cards", count: 15 }, { label: "Modais", count: 5 }, { label: "Listagens", count: 5 }] },
      surfaceSubtle: { total: 14, items: [{ label: "Banners/Chips", count: 6 }, { label: "Acordeões", count: 4 }, { label: "Estados vazios", count: 4 }] },
      borderDefault: { total: 12, items: [{ label: "Inputs", count: 5 }, { label: "Cards", count: 3 }, { label: "Tabela", count: 4 }] },
      borderSoft: { total: 38, items: [{ label: "Cards", count: 15 }, { label: "Modais", count: 8 }, { label: "Tabelas", count: 10 }, { label: "Pills", count: 5 }] },
      field001: { total: 8, items: [{ label: "Inputs", count: 8 }] },
      "radius-xs": { total: 0, items: [] },
      "radius-sm": { total: 0, items: [] },
      "radius-md": { total: 0, items: [] },
      "radius-lg": { total: 0, items: [] },
      "radius-pill": { total: 10, items: [{ label: "Botões", count: 6 }, { label: "Pills", count: 4 }] },
      "space-2xs": { total: 0, items: [] },
      "space-xs": { total: 0, items: [] },
      "space-sm": { total: 0, items: [] },
      "space-md": { total: 11, items: [{ label: "Cards", count: 5 }, { label: "Listas", count: 6 }] },
      "space-lg": { total: 0, items: [] },
      "space-xl": { total: 0, items: [] },
      "elevation-soft": { total: 16, items: [{ label: "Cards", count: 10 }, { label: "Modais", count: 6 }] },
      "elevation-card-hover": { total: 5, items: [{ label: "Cards Hover", count: 5 }] },
      "background-brand": { total: 0, items: [] },
    }),
    [],
  );

  const allComponentsList = useMemo(
    () =>
      [
        "Accordion",
        "AlertDialog",
        "Avatar",
        "Badge",
        "Breadcrumb",
        "Button",
        "Calendar",
        "Card",
        "Checkbox",
        "Collapsible",
        "Command",
        "Dialog",
        "Icon",
        "Input",
        "Popover",
        "Select",
        "Sheet",
        "Slider",
        "Switch",
        "Table/DataTable",
        "Tabs",
        "Textarea",
        "Snackbar",
        "Tooltip",
      ].sort((a, b) => a.localeCompare(b)),
    [],
  );

  const componentIdsInDoc = useMemo(
    () =>
      new Set(
        componentsSpec.map((item) => item.name),
      ),
    [],
  );

  const componentsSpecOrdered = useMemo(
    () => [...componentsSpec].sort((a, b) => a.name.localeCompare(b.name)),
    [],
  );

  const sourceFiles = useMemo(
    () => import.meta.glob<string>("/src/**/*.{ts,tsx,css}", { as: "raw", eager: true }),
    [],
  );

  const [tokenUsageCounts, setTokenUsageCounts] = useState<Record<string, number>>({});
  const [componentUsageCounts, setComponentUsageCounts] = useState<Record<string, number>>({});
  const [componentUsageDetails, setComponentUsageDetails] = useState<
    Record<string, { total: number; items: Array<{ label: string; count: number }> }>
  >({});
  const [tokenUsageDetails, setTokenUsageDetails] = useState<
    Record<string, { total: number; items: Array<{ label: string; count: number }> }>
  >({});

  const countOccurrences = (text: string, needle: string) => {
    if (!text || !needle) return 0;
    const regex = new RegExp(needle.replace(/[.*+?^${}()|[\]\\]/g, "\\$&"), "g");
    return (text.match(regex) || []).length;
  };

  useEffect(() => {
    if (!openTokens && !openExamples) return;
    const fileEntries = Object.entries(sourceFiles);
    const systemIconsFile = sourceFiles["/src/components/ui/system-icons.tsx"] ?? "";
    const systemIconNames = Array.from(
      systemIconsFile.matchAll(/export const (?<name>[A-Za-z0-9_]+)\s*=\s*createSystemIcon\(/g),
    )
      .map((match) => match.groups?.name)
      .filter((name): name is string => Boolean(name));

    // Tokens
    const tokenMap: Record<string, number> = {};
    const tokenDetailMap: Record<
      string,
      { total: number; items: Array<{ label: string; count: number }> }
    > = {};
    tokenGuide.forEach((group) => {
      const sections = (group as any).sections || [{ tokens: group.tokens }];
      sections.forEach((section: any) => {
        section.tokens.forEach((token: any) => {
          let total = 0;
          const items: Array<{ label: string; count: number }> = [];
          const patterns = tokenAliasMap[token.id] ?? [token.id];
          fileEntries.forEach(([path, file]) => {
            const count = patterns.reduce((acc, pattern) => acc + countOccurrences(file, pattern), 0);
            if (count > 0) {
              const trimmed = path.replace("/src/", "");
              const simplified = trimmed.split("/").slice(1).join("/") || trimmed;
              items.push({ label: simplified, count });
              total += count;
            }
          });
          tokenMap[token.id] = total;
          tokenDetailMap[token.id] = { total, items };
        });
      });
    });
    setTokenUsageCounts(tokenMap);
    setTokenUsageDetails(tokenDetailMap);

    // Component usage (busca por tags JSX)
    const componentMap: Record<string, number> = {};
    const componentDetailMap: Record<
      string,
      { total: number; items: Array<{ label: string; count: number }> }
    > = {};
    componentsSpec.forEach((comp) => {
      const baseNeedles = componentAliasMap[comp.name] ?? [`<${comp.name}`];
      const needles =
        comp.name === "Icon"
          ? Array.from(new Set([...baseNeedles, ...systemIconNames.map((name) => `<${name}`)]))
          : baseNeedles;
      let total = 0;
      const items: Array<{ label: string; count: number }> = [];
      fileEntries.forEach(([path, file]) => {
        const count = needles.reduce((acc, pattern) => acc + countOccurrences(file, pattern), 0);
        if (count > 0) {
          const trimmed = path.replace("/src/", "");
          const simplified = trimmed.split("/").slice(1).join("/") || trimmed;
          items.push({ label: simplified, count });
          total += count;
        }
      });
      componentMap[comp.name] = total;
      componentDetailMap[comp.name] = { total, items };
    });
    setComponentUsageCounts(componentMap);
    setComponentUsageDetails(componentDetailMap);
  }, [openTokens, openExamples, sourceFiles, tokenGuide, componentsSpec]);

  const mockData: Array<{ id: string; jornada: string; status: string }> = [
    { id: "1", jornada: "Onboarding", status: "Em andamento" },
    { id: "2", jornada: "Pagamento", status: "Publicado" },
  ];

  const columns: Column[] = [
    { id: "jornada", label: "Jornada", sortable: true },
    { id: "status", label: "Status", sortable: true },
  ];

  const renderCell = (item: Record<string, string>, columnId: string) => item[columnId];

  const heroHighlights = [
    {
      tag: "Tokens",
      description: "Cores, espaçamentos, sombras e raios em variáveis CSS.",
    },
    {
      tag: "Componentização",
      description: "Botões, cartões, pills e tabelas já estilizados.",
    },
    {
      tag: "Dark mode",
      description: "Aplicação automática via html.dark.",
    },
  ];

  return (
    <div className="min-h-screen bg-primaryBackground text-primaryText">
      <div className="container mx-auto px-4 lg:px-10 py-10 space-y-10">
        {/* Hero */}
        <section className="relative overflow-hidden rounded-lgToken bg-brand-gradient text-inverseText shadow-softToken">
          <div className="absolute inset-0 opacity-30" aria-hidden>
            <div className="absolute -left-16 top-12 h-40 w-40 rounded-full bg-white blur-3xl" />
            <div className="absolute right-10 bottom-10 h-28 w-28 rounded-full bg-accent blur-3xl" />
          </div>
          <div className="relative grid gap-8 lg:grid-cols-[2fr,1fr] items-center px-6 md:px-10 py-10">
            <div className="space-y-4">
	              <div className="inline-flex items-center gap-2 rounded-pillToken bg-white/10 px-3 py-2 text-sm font-semibold backdrop-blur">
	                <Icon name="auto_awesome" size={16} className="text-inverseText" />
	                Design System Fourmakers
	              </div>
              <h1 className="text-3xl md:text-4xl font-bold leading-tight">
                Visual Fourmakers.io conectado por tokens, dark mode e componentes padronizados.
              </h1>
              <p className="text-sm md:text-base text-white/80 max-w-3xl">
                Tudo neste projeto deriva de variáveis semânticas: paleta roxo/verde, superfícies claras e profundas,
                raios suaves e sombras flutuantes. Use sempre os tokens para garantir consistência e velocidade.
              </p>
	              <div className="flex flex-wrap items-center gap-3">
	                <Button size="lg" variant="primary" className="shadow-cardHoverToken" onClick={() => setOpenExamples(true)}>
	                  <Icon name="auto_fix_high" size={20} className="text-inverseText" />
	                  Guia de Componentes
	                </Button>
	                <Button size="lg" variant="secondary" onClick={() => setOpenTokens(true)}>
	                  <Icon name="code" size={20} />
	                  Guia de Tokens
	                </Button>
                <Badge variant="accent" className="bg-white/10 text-white border-white/20">
                  100% Tailwind + CSS vars
                </Badge>
              </div>
            </div>
	            <Card className="bg-white/10 text-inverseText border-white/20 shadow-cardHoverToken">
	              <CardHeader>
	                <CardTitle className="text-inverseText flex items-center gap-2">
	                  <Icon name="palette" size={20} className="text-inverseText" />
	                  Prontos para escalar
	                </CardTitle>
                <CardDescription className="text-white/80">
                  Tokens, componentes e páginas com o look & feel do fourmakers.io
                </CardDescription>
              </CardHeader>
              <CardContent className="space-y-4 text-sm">
                {heroHighlights.map((item, idx) => (
                  <div key={item.tag} className="space-y-3">
                    <div className="flex flex-col gap-2 md:flex-row md:items-center md:gap-3">
                      <Badge
                        variant="default"
                        className="bg-white/15 text-white border-white/20 md:w-[170px] justify-center whitespace-nowrap"
                      >
                        {item.tag}
                      </Badge>
                      <p className="text-white/80 leading-relaxed">
                        {item.tag === "Dark mode" ? (
                          <>
                            Aplicação automática via <span className="font-mono">html.dark</span>.
                          </>
                        ) : (
                          item.description
                        )}
                      </p>
                    </div>
                    {idx < heroHighlights.length - 1 && <Separator className="bg-white/20" />}
                  </div>
                ))}
              </CardContent>
            </Card>
          </div>
        </section>

        {/* Princípios */}
	        <section className="space-y-3">
	          <div className="flex items-center gap-2">
	            <Icon name="menu_book" size={20} className="text-primary" />
	            <h2 className="section-title">Princípios da marca</h2>
	          </div>
          <div className="grid gap-4 md:grid-cols-3">
            {principles.map((item) => (
              <Card key={item.title}>
                <CardHeader>
                  <CardTitle className="text-lg">{item.title}</CardTitle>
                  <CardDescription>{item.text}</CardDescription>
                </CardHeader>
              </Card>
            ))}
          </div>
        </section>

        {/* Toolkit para prototipação */}
	        <section className="space-y-3">
	          <div className="flex items-center gap-2">
	            <Icon name="auto_awesome" size={20} className="text-primary" />
	            <h2 className="section-title">Starter Toolkit para protótipos (IA)</h2>
	          </div>
          <Card className="bg-surfaceElevated border border-borderSoft shadow-softToken">
            <CardHeader className="flex flex-col gap-3 lg:flex-row lg:items-start lg:gap-6">
              <div className="space-y-2 lg:flex-1">
                <CardTitle className="text-lg">Design Toolkit</CardTitle>
                <CardDescription>
                  Use este guia rápido ao gerar telas: tokens, cores, raio padrão, botões (primary/secondary/outline/ghost), dark mode e estrutura de arquivos.
                </CardDescription>
	                <div className="inline-flex items-center gap-2 rounded-pillToken bg-primarySoft text-primary px-3 py-1 text-xs font-semibold">
	                  <Icon name="lightbulb" size={16} className="text-primary" />
	                  Dica: entregue esse arquivo à IA antes de gerar protótipos para minimizar refações.
	                </div>
              </div>
              <div className="flex flex-col gap-2 lg:w-[320px]">
                <Button variant="outline" asChild className="whitespace-nowrap gap-2 w-full">
	                  <a href="/design-toolkit.md" download className="flex items-center justify-center gap-2">
	                    <Icon name="download" size={16} className="text-primaryText" />
	                    Orientação Design Toolkit
	                  </a>
	                </Button>
                <div className="text-xs text-muted-foreground text-center">ou</div>
                <Button
                  variant="ghost"
                  className="whitespace-nowrap gap-2 w-full"
                  onClick={() => {
                    navigator.clipboard.writeText(toolkitContent);
                    setCopyMessage("Orientações copiadas para a área de transferência.");
                  }}
	                >
	                  <Icon name="content_copy" size={16} className="text-primaryText" />
	                  Copiar Comandos Toolkit
	                </Button>
                {copyMessage && (
                  <div className="text-xs text-primary bg-primarySoft border border-primary/20 rounded-lgToken px-3 py-2">
                    {copyMessage}
                  </div>
                )}
              </div>
            </CardHeader>
            <CardContent className="space-y-2 text-sm text-secondaryText">
              <p>
                • Paleta e tokens completos (light/dark), radius LG/pill, sombras soft; componentes base mapeados (Button, Input, Card, Tooltip, Table).
              </p>
              <p>
                • Hierarquia de botões: Primary (verde), Secondary sem borda, Outline com borda, Ghost transparente, Brand Gradient apenas para hero.
              </p>
              <p>
                • Estrutura sugerida: páginas só orquestram UI; lógica em hooks/view-model; usar layout padrão; aliases simulados para imports; responsividade e A11y incluídas.
              </p>
            </CardContent>
          </Card>
        </section>

        {/* Arquitetura e Boas Práticas */}
        <section className="space-y-4">
          <div className="flex items-center gap-2">
            <Icon name="architecture" size={20} className="text-primary" />
            <h2 className="section-title">Arquitetura e Boas Práticas</h2>
          </div>
          <div className="grid gap-4 lg:grid-cols-2">
            <Card>
              <CardHeader>
                <CardTitle>Clean Architecture</CardTitle>
                <CardDescription>Separação clara de responsabilidades por camadas</CardDescription>
              </CardHeader>
              <CardContent className="space-y-2 text-sm text-secondaryText">
                <p>• <strong>Presentation</strong>: Pages orquestram UI, Components reutilizáveis, Hooks centralizam lógica</p>
                <p>• <strong>Domain</strong>: Entities, Repository interfaces, UseCases com lógica de negócio</p>
                <p>• <strong>Data</strong>: APIs (usando httpClient), Repository implementations</p>
                <p>• <strong>Shared</strong>: Utils, Constants, Types compartilhados</p>
                <p>• <strong>Regra</strong>: Pages não fazem fetch direto, sempre usar UseCases</p>
              </CardContent>
            </Card>
            <Card>
              <CardHeader>
                <CardTitle>Organização de Código</CardTitle>
                <CardDescription>Estrutura e padrões para manter o frontend limpo</CardDescription>
              </CardHeader>
              <CardContent className="space-y-2 text-sm text-secondaryText">
                <p>• <strong>Hooks</strong>: Lógica de apresentação e estado (`use[Feature].ts` ou `use[Feature]ViewModel.ts`)</p>
                <p>• <strong>Utils</strong>: Funções puras reutilizáveis (`[feature]Utils.ts` em `@shared/utils/`)</p>
                <p>• <strong>Componentes</strong>: Reutilizáveis e desacoplados, usando componentes base do DS</p>
                <p>• <strong>APIs</strong>: Sempre usar `httpClient` de `@data/api/httpClient` (nunca `fetch()` direto)</p>
                <p>• <strong>Imports</strong>: Usar aliases (`@/`, `@presentation/`, `@domain/`, etc.)</p>
              </CardContent>
            </Card>
          </div>
          <Card className="bg-accentSoft/30 border-accent/20">
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <Icon name="menu_book" size={20} className="text-accent" />
                Guia Completo: Design Toolkit
              </CardTitle>
              <CardDescription>
                Consulte o arquivo <code className="bg-surfaceElevated px-2 py-1 rounded text-xs">public/design-toolkit.md</code> para orientações completas sobre design system, arquitetura, organização de código, padrões de componentes e checklist para novas features.
              </CardDescription>
            </CardHeader>
            <CardContent className="space-y-3">
              <div className="grid gap-2 text-sm text-secondaryText md:grid-cols-2">
                <div>
                  <p className="font-semibold text-primaryText mb-1">Design System</p>
                  <p>• Tokens (cores, spacing, radius, sombras)</p>
                  <p>• Componentes base e padrões</p>
                  <p>• Acessibilidade e dark mode</p>
                </div>
                <div>
                  <p className="font-semibold text-primaryText mb-1">Arquitetura</p>
                  <p>• Clean Architecture e camadas</p>
                  <p>• Fluxo de dados e dependências</p>
                  <p>• Padrões de Page, Hook, Componente, API</p>
                </div>
              </div>
              <div className="pt-2 border-t border-borderDefault">
                <p className="text-xs text-secondaryText">
                  📋 Use o toolkit para: prototipação, desenvolvimento, code review e onboarding. Ele é o padrão oficial do projeto.
                </p>
              </div>
            </CardContent>
          </Card>
        </section>

        {/* Paleta */}
	        <section className="space-y-4">
	          <div className="flex items-center gap-2">
	            <Icon name="palette" size={20} className="text-primary" />
	            <h2 className="section-title">Paleta Fourmakers (light & dark)</h2>
	          </div>
          <div className="grid gap-4 lg:grid-cols-2">
            <Card>
	              <CardHeader>
	                <CardTitle className="flex items-center gap-2">
	                  <Icon name="light_mode" size={20} className="text-primary" />
	                  Light mode
	                </CardTitle>
                <CardDescription>Usado como default. Superfícies claras e textos de alto contraste.</CardDescription>
              </CardHeader>
              <CardContent className="grid gap-3 md:grid-cols-2">
                {paletteLight.map((color) => (
                  <div
                    key={color.token}
                    className="flex items-center justify-between rounded-lgToken border border-borderSoft bg-surfaceElevated p-3 shadow-softToken"
                  >
                    <div className="flex items-center gap-3">
                      <span
                        className="h-10 w-10 rounded-md border border-borderDefault"
                        style={{ background: color.value }}
                      />
                      <div>
                        <p className="text-sm font-semibold">{color.name}</p>
                        <p className="text-xs text-secondaryText">{color.token}</p>
                      </div>
                    </div>
                    <span className="text-xs font-mono text-secondaryText">{color.value}</span>
                  </div>
                ))}
              </CardContent>
            </Card>
            <Card>
	              <CardHeader>
	                <CardTitle className="flex items-center gap-2">
	                  <Icon name="dark_mode" size={20} className="text-primary" />
	                  Dark mode
	                </CardTitle>
                <CardDescription>Ativado via <code className="font-mono">html.dark</code>. Gradiente roxo + verde.</CardDescription>
              </CardHeader>
              <CardContent className="grid gap-3 md:grid-cols-2">
                {paletteDark.map((color) => (
                  <div
                    key={color.token}
                    className="flex items-center justify-between rounded-lgToken border border-borderSoft bg-surfaceElevated p-3 shadow-softToken"
                  >
                    <div className="flex items-center gap-3">
                      <span
                        className="h-10 w-10 rounded-md border border-borderDefault"
                        style={{ background: color.value }}
                      />
                      <div>
                        <p className="text-sm font-semibold">{color.name}</p>
                        <p className="text-xs text-secondaryText">{color.token}</p>
                      </div>
                    </div>
                    <span className="text-xs font-mono text-secondaryText">{color.value}</span>
                  </div>
                ))}
              </CardContent>
            </Card>
          </div>
        </section>

        {/* Tokens */}
        <section className="space-y-4">
          <div className="flex items-center gap-2">
            <Icon name="palette" size={20} className="text-primary" />
            <h2 className="section-title">Tokens disponíveis</h2>
          </div>
          <div className="grid gap-4 lg:grid-cols-3">
            <Card>
              <CardHeader>
                <CardTitle>Radius</CardTitle>
                <CardDescription>Use sempre classes <code className="font-mono">rounded-*-Token</code>.</CardDescription>
              </CardHeader>
              <CardContent className="space-y-3">
                {radiusTokens.map((radius) => (
                  <div key={radius.token} className="flex items-center justify-between rounded-lgToken border p-3">
                    <div className="flex items-center gap-3">
                      <span
                        className="block h-12 w-12 bg-primary"
                        style={{ borderRadius: radius.value }}
                        aria-hidden
                      />
                      <div>
                        <p className="text-sm font-semibold">{radius.name}</p>
                        <p className="text-xs text-secondaryText">{radius.token}</p>
                      </div>
                    </div>
                    <span className="text-xs font-mono text-secondaryText">{radius.value}</span>
                  </div>
                ))}
              </CardContent>
            </Card>
            <Card>
              <CardHeader>
                <CardTitle>Spacing</CardTitle>
                <CardDescription>Mapeados em <code className="font-mono">px-[token]</code> e <code className="font-mono">py-[token]</code>.</CardDescription>
              </CardHeader>
              <CardContent className="space-y-3">
                {spacingTokens.map((space) => (
                  <div key={space.token} className="flex items-center justify-between rounded-lgToken border p-3">
                    <div className="flex items-center gap-3">
                      <div
                        className="rounded-md bg-primary"
                        style={{ width: space.value, height: space.value }}
                        aria-hidden
                      />
                      <div>
                        <p className="text-sm font-semibold">{space.name}</p>
                        <p className="text-xs text-secondaryText">{space.token}</p>
                      </div>
                    </div>
                    <span className="text-xs font-mono text-secondaryText">{space.value}</span>
                  </div>
                ))}
              </CardContent>
            </Card>
            <Card>
              <CardHeader>
                <CardTitle>Shadows & superfícies</CardTitle>
                <CardDescription>Elevação suave e hover consistente.</CardDescription>
              </CardHeader>
              <CardContent className="space-y-3">
                {shadowTokens.map((shadow) => (
                  <div
                    key={shadow.token}
                    className="rounded-lgToken border border-borderSoft bg-surfaceElevated p-4"
                    style={{ boxShadow: shadow.value }}
                  >
                    <p className="text-sm font-semibold">{shadow.name}</p>
                    <p className="text-xs text-secondaryText">{shadow.token}</p>
                    <p className="text-xs text-secondaryText font-mono mt-1">{shadow.value}</p>
                  </div>
                ))}
                <Separator />
                <p className="text-sm">
                  Superfícies: <code className="font-mono">bg-surfaceElevated</code>,{" "}
                  <code className="font-mono">bg-surfaceSubtle</code>,{" "}
                  <code className="font-mono">bg-primaryBackground</code>.
                </p>
              </CardContent>
            </Card>
          </div>
        </section>

        {/* Tailwind usage */}
        <section className="grid gap-4 lg:grid-cols-[1.1fr,0.9fr]">
	          <Card>
	            <CardHeader>
	              <CardTitle className="flex items-center gap-2">
	                <Icon name="code" size={20} className="text-primary" />
	                Tailwind com tokens
	              </CardTitle>
              <CardDescription>Use as classes semânticas mapeadas no <code className="font-mono">tailwind.config.ts</code>.</CardDescription>
            </CardHeader>
            <CardContent className="space-y-4">
              <pre className="rounded-lgToken bg-surfaceSubtle p-4 text-xs text-primaryText border border-borderSoft overflow-x-auto">
{`// Botão CTA verde
<Button variant="primary" className="shadow-cardHoverToken">
  Enviar
</Button>

// Card com superfície elevada
<div className="bg-surfaceElevated border border-borderSoft rounded-lgToken shadow-softToken p-lg">
  Conteúdo
</div>

// Input padrão
<input
  className="w-full rounded-lgToken border border-borderDefault bg-field001 px-md py-sm
             text-primaryText placeholder:text-placeholder focus:border-primary focus:ring-2 focus:ring-primary" />`}
              </pre>
              <div className="grid gap-3 md:grid-cols-2">
                <div className="rounded-lgToken border border-borderSoft bg-surfaceElevated p-4">
                  <p className="text-sm font-semibold mb-2">Gradiente de marca</p>
                  <div className="h-16 rounded-md bg-brand-gradient shadow-softToken" />
                </div>
                <div className="rounded-lgToken border border-borderSoft bg-surfaceElevated p-4">
                  <p className="text-sm font-semibold mb-2">Dark mode</p>
                  <p className="text-sm text-secondaryText">
                    Aplique <code className="font-mono">document.documentElement.classList.add("dark")</code> para ativar.
                  </p>
                </div>
              </div>
            </CardContent>
          </Card>
          <Card>
	            <CardHeader>
	              <CardTitle className="flex items-center gap-2">
	                <Icon name="widgets" size={20} className="text-primary" />
	                Componentes base
	              </CardTitle>
              <CardDescription>Botões, cards e pills refletem o look fourmakers.io.</CardDescription>
            </CardHeader>
            <CardContent className="space-y-4">
              <div className="flex flex-wrap gap-3">
                <Button variant="primary" size="sm">
                  CTA
                </Button>
                <Button variant="secondary" size="sm">
                  Secundário
                </Button>
                <Button variant="ghost" size="sm">
                  Ghost
                </Button>
                <Button variant="outline" size="sm">
                  Outline
                </Button>
              </div>
              <div className="flex flex-wrap gap-2">
	                <span className="inline-flex items-center gap-2 bg-primarySoft text-primaryText px-md py-2xs rounded-pillToken border border-borderSoft">
	                  Jornada Fast
	                  <Icon name="verified" size={16} className="text-primary" />
	                </span>
	                <span className="inline-flex items-center gap-2 bg-accentSoft text-accent px-md py-2xs rounded-pillToken border border-borderSoft">
	                  Automação
	                  <Icon name="bolt" size={16} className="text-accent" />
	                </span>
              </div>
              <div className="rounded-lgToken border border-borderSoft bg-surfaceElevated p-4 shadow-softToken space-y-3">
                <p className="text-sm font-semibold">Card padrão</p>
                <p className="text-sm text-secondaryText">
                  Sempre use <code className="font-mono">bg-surfaceElevated</code> +{" "}
                  <code className="font-mono">border-borderSoft</code> +{" "}
                  <code className="font-mono">shadow-softToken</code>.
                </p>
              </div>
            </CardContent>
          </Card>
        </section>

        {/* Layout & componentes de página */}
        <section className="grid gap-4 lg:grid-cols-3">
	          <Card>
	            <CardHeader>
	              <CardTitle className="flex items-center gap-2">
	                <Icon name="dashboard" size={20} className="text-primary" />
	                Layouts & Hero
	              </CardTitle>
              <CardDescription>Hero usa <code className="font-mono">bg-brand-gradient</code>, textos brancos e pills.</CardDescription>
            </CardHeader>
            <CardContent className="space-y-3 text-sm text-secondaryText">
              <p>Header: fundo branco, borda inferior, CTA verde e link de demonstração.</p>
              <p>Seções: cards com espaçamento <code className="font-mono">p-lg</code> e radius <code className="font-mono">rounded-lgToken</code>.</p>
              <p>Sidebar: use cores <code className="font-mono">sidebar.*</code> do Tailwind para consistência.</p>
            </CardContent>
          </Card>
	          <Card>
	            <CardHeader>
	              <CardTitle className="flex items-center gap-2">
	                <Icon name="view_quilt" size={20} className="text-primary" />
	                Inputs & formulários
	              </CardTitle>
              <CardDescription>Campos brancos no light, superfícies profundas no dark.</CardDescription>
            </CardHeader>
            <CardContent className="space-y-3">
              <input
                aria-label="Input padrão"
                className="w-full rounded-lgToken border border-borderDefault bg-field001 px-md py-sm text-sm text-primaryText placeholder:text-placeholder focus:border-primary focus:ring-2 focus:ring-primary"
                placeholder="Placeholder neutro"
              />
              <div className="rounded-lgToken border border-borderSoft bg-primarySoft p-3 text-sm text-primaryText">
                <strong>Regra:</strong> nunca force <code className="font-mono">bg-white</code> ou{" "}
                <code className="font-mono">dark:bg-white</code>. Use <code className="font-mono">bg-field001</code>.
              </div>
            </CardContent>
          </Card>
	          <Card>
	            <CardHeader>
	              <CardTitle className="flex items-center gap-2">
	                <Icon name="table_chart" size={20} className="text-primary" />
	                DataTable padrão
	              </CardTitle>
              <CardDescription>Ordenação, drag & drop e responsividade já embutidos.</CardDescription>
            </CardHeader>
            <CardContent className="space-y-3 text-sm text-secondaryText">
              <p>
                Importar de <code className="font-mono">@presentation/components/common</code> e usar{" "}
                <code className="font-mono">columns</code> + <code className="font-mono">renderCell</code>.
              </p>
              <pre className="rounded-lgToken bg-surfaceSubtle p-3 text-[11px] border border-borderSoft overflow-x-auto">
{`<DataTable
  columns={columns}
  data={data}
  keyExtractor={(item) => item.id.toString()}
  renderCell={(item, columnId) => item[columnId]}
/>`}
              </pre>
            </CardContent>
          </Card>
        </section>

        {/* Dark mode & acessibilidade */}
        <section className="grid gap-4 lg:grid-cols-2">
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <Icon name="dark_mode" size={20} className="text-primary" />
                Dark mode
              </CardTitle>
              <CardDescription>Toda a paleta reage à classe <code className="font-mono">dark</code> no html.</CardDescription>
            </CardHeader>
            <CardContent className="space-y-3 text-sm text-secondaryText">
              <ul className="list-disc list-inside space-y-2">
                <li>Gradiente de marca alterna para roxo profundo + verde.</li>
                <li>Superfícies: <code className="font-mono">bg-surfaceElevated</code> → <code className="font-mono">#0B1120</code>.</li>
                <li>Botão primário continua verde com hover reforçado.</li>
                <li>Nunca force cores com <code className="font-mono">dark:bg-white</code>. Use tokens.</li>
              </ul>
              <div className="inline-flex items-center gap-2 rounded-pillToken bg-primarySoft px-md py-2xs text-xs text-primaryText">
                <Icon name="light_mode" size={16} className="text-primary" />
                <span className="font-semibold">Toggle</span>
                <span className="text-secondaryText">document.documentElement.classList.toggle("dark")</span>
              </div>
            </CardContent>
          </Card>
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <Icon name="accessibility" size={20} className="text-primary" />
                Acessibilidade & motion
              </CardTitle>
              <CardDescription>Contraste alto e transições suaves de 200ms.</CardDescription>
            </CardHeader>
            <CardContent className="space-y-3 text-sm text-secondaryText">
              <div className="flex items-center gap-2">
                <Icon name="verified_user" size={16} className="text-accent" />
                <p>Textos usam <code className="font-mono">--color-primary-text</code> para contraste.</p>
              </div>
              <div className="flex items-center gap-2">
                <Icon name="bolt" size={16} className="text-primary" />
                <p>Transições globais configuradas no <code className="font-mono">index.css</code>.</p>
              </div>
              <div className="flex items-center gap-2">
                <Icon name="rocket_launch" size={16} className="text-accent" />
                <p>Focus-visible com anel roxo em todos os botões e links.</p>
              </div>
              <div className="rounded-lgToken border border-borderSoft bg-surfaceSubtle p-3">
                Motion: prefira entradas de fade/translate curtas (200–300ms) e sombras <code className="font-mono">shadow-softToken</code>.
              </div>
            </CardContent>
          </Card>
        </section>

        {/* Guia de contribuição */}
        <section className="grid gap-4 lg:grid-cols-[1.2fr,0.8fr]">
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <Icon name="menu_book" size={20} className="text-primary" />
                Como contribuir
              </CardTitle>
              <CardDescription>Checklist para novas telas ou componentes.</CardDescription>
            </CardHeader>
            <CardContent className="grid gap-3 md:grid-cols-2 text-sm text-secondaryText">
              <div className="rounded-lgToken border border-borderSoft bg-surfaceElevated p-3 space-y-2">
                <p className="font-semibold text-primaryText">Antes de codar</p>
                <ul className="list-disc list-inside space-y-1">
                  <li>Importe <code className="font-mono">Button</code>, <code className="font-mono">Card</code> e <code className="font-mono">Badge</code>.</li>
                  <li>Defina tokens no <code className="font-mono">index.css</code> caso precise de novos.</li>
                  <li>Esboce variantes e estados (hover, disabled, focus).</li>
                </ul>
              </div>
              <div className="rounded-lgToken border border-borderSoft bg-surfaceElevated p-3 space-y-2">
                <p className="font-semibold text-primaryText">Durante o desenvolvimento</p>
                <ul className="list-disc list-inside space-y-1">
                  <li>Use utilitários: <code className="font-mono">bg-surfaceElevated</code>, <code className="font-mono">text-primaryText</code>.</li>
                  <li>Evite hardcode de cores, mesmo em SVGs.</li>
                  <li>Testar light/dark: <code className="font-mono">documentElement.classList.add("dark")</code>.</li>
                </ul>
              </div>
              <div className="rounded-lgToken border border-borderSoft bg-surfaceElevated p-3 space-y-2">
                <p className="font-semibold text-primaryText">Depois</p>
                <ul className="list-disc list-inside space-y-1">
                  <li>Rodar lint e garantir responsividade.</li>
                  <li>Documentar no Story/Docs quando aplicável.</li>
                  <li>Adicionar exemplos na seção de componentes, se for novo padrão.</li>
                </ul>
              </div>
              <div className="rounded-lgToken border border-borderSoft bg-surfaceElevated p-3 space-y-2">
                <p className="font-semibold text-primaryText">Roadmap rápido</p>
                <ul className="list-disc list-inside space-y-1">
                  <li>Estados para formulários complexos.</li>
                  <li>Ícones proprietários na paleta de marca.</li>
                  <li>Biblioteca de motion e skeletons.</li>
                </ul>
              </div>
            </CardContent>
          </Card>
	          <Card>
	            <CardHeader>
	              <CardTitle className="flex items-center gap-2">
	                <Icon name="table_chart" size={20} className="text-primary" />
	                Exemplo visual (DataTable)
	              </CardTitle>
              <CardDescription>Tokens aplicados em tabela viva.</CardDescription>
            </CardHeader>
            <CardContent>
              <DataTable
                columns={columns}
                data={mockData}
                keyExtractor={(item) => item.id.toString()}
                renderCell={renderCell}
                emptyMessage="Nenhum dado encontrado"
              />
            </CardContent>
          </Card>
        </section>
      </div>

      {/* Modal: guia de componentes */}
      <Dialog open={openExamples} onOpenChange={setOpenExamples}>
        <DialogContent className="max-w-5xl max-h-[90vh] overflow-visible">
	          <DialogHeader className="sticky top-0 z-10 pb-4">
	            <DialogTitle className="flex items-center gap-2 text-primaryText">
	              <Icon name="auto_fix_high" size={20} className="text-primary" />
	              Guia de Componentes
	            </DialogTitle>
            <DialogDescription>
              Visual, specs, usos e snippets prontos para colar em qualquer página.
            </DialogDescription>
          </DialogHeader>
          <div className="space-y-4 max-h-[70vh] overflow-y-auto pr-1 relative">
            <div className="sticky top-0 z-10 bg-surfaceElevated pb-3">
              <Accordion type="single" collapsible defaultValue="todos">
                <AccordionItem value="todos">
                  <AccordionTrigger className="px-3 py-3 rounded-lgToken border border-borderSoft bg-surfaceSubtle">
                    Lista completa de componentes
                  </AccordionTrigger>
                  <AccordionContent className="px-3 pb-4 pt-2">
                    <div className="flex flex-wrap gap-2 text-sm text-primaryText max-h-[120px] overflow-y-auto pr-1">
                      {allComponentsList
                        .filter((comp) => componentIdsInDoc.has(comp))
                        .map((comp) => (
                          <button
                            key={comp}
                            className="rounded-pillToken border border-borderSoft bg-surfaceElevated px-3 py-1 hover:bg-primarySoft transition"
                            onClick={() => {
                              const el = document.getElementById(`comp-${comp}`);
                              if (el) {
                                el.scrollIntoView({ behavior: "smooth", block: "start" });
                              }
                            }}
                            type="button"
                            title={`Ir para ${comp}`}
                          >
                            {comp}
                          </button>
                        ))}
                    </div>
                  </AccordionContent>
                </AccordionItem>
              </Accordion>
            </div>
            {componentsSpecOrdered.map((item) => (
              <Accordion key={item.name} type="single" collapsible>
                <AccordionItem value={item.name}>
                  <AccordionTrigger id={`comp-${item.name}`} className="px-4 py-3">
                    <div className="flex items-center justify-between w-full gap-3">
                      <div className="text-left">
                        <p className="text-lg font-semibold text-primaryText">{item.name}</p>
                        <p className="text-sm text-secondaryText">{item.description}</p>
                      </div>
                      <div className="flex items-center gap-2">
                        <TooltipProvider>
                          <Tooltip>
                            <TooltipTrigger asChild>
                              <Badge variant="secondary" className="text-xs cursor-help">Token-first</Badge>
                            </TooltipTrigger>
                            <TooltipContent className="text-xs max-w-xs">
                              Usa apenas tokens semânticos de cor, raio, sombra e espaçamento. Altere tokens/temas e o componente se adapta sem refatorar.
                            </TooltipContent>
                          </Tooltip>
                        </TooltipProvider>
                        <TooltipProvider>
                          <Tooltip>
	                            <TooltipTrigger asChild>
	                              <div className="flex items-center gap-1 text-xs text-secondaryText cursor-help">
	                                <Icon name="check" size={14} className="text-primary" />
	                                <span>{componentUsageCounts[item.name] ?? 0} usos</span>
	                              </div>
	                            </TooltipTrigger>
                              <TooltipContent className="text-xs p-0 overflow-hidden min-w-[24rem] max-w-[44rem]">
                                <div className="sticky top-0 z-10 bg-surfaceElevated border-b border-borderSoft px-3 py-2">
                                  <p className="font-semibold text-primaryText leading-tight text-[11px] sm:text-xs">
                                    {buildRoiTitle(COMPONENT_DEV_HOURS, componentUsageCounts[item.name] ?? 0)}
                                  </p>
                                </div>
                                <div className="max-h-[600px] overflow-y-auto px-3 py-2 space-y-1">
                                  {[...(componentUsageDetails[item.name]?.items || [])]
                                    .sort((a, b) => b.count - a.count)
                                    .map((use) => (
                                    <p key={use.label} className="flex justify-between gap-2">
                                      <span className="truncate">{use.label}</span>
                                      <span className="font-semibold">{use.count}</span>
                                    </p>
                                  ))}
                                {(!componentUsageDetails[item.name]?.items ||
                                  componentUsageDetails[item.name]?.items.length === 0) && (
                                  <p className="text-secondaryText">Sem usos mapeados nesta build.</p>
                                )}
                                </div>
                            </TooltipContent>
                          </Tooltip>
                        </TooltipProvider>
                      </div>
                    </div>
                  </AccordionTrigger>
                  <AccordionContent className="px-4 pb-4 space-y-3">
                    <div className="rounded-lgToken border border-borderSoft bg-surfaceSubtle p-4">{item.element}</div>
                    <div className="rounded-lgToken border border-borderSoft bg-surfaceElevated p-4">
                      <p className="text-sm font-semibold text-primaryText mb-2">Specs rápidas</p>
                      <div className="grid gap-2 text-xs text-secondaryText sm:grid-cols-3">
                        <div>
                          <p className="font-semibold text-primaryText">Cores</p>
                          <p>Superfícies: surfaceElevated / Subtle</p>
                          <p>Ação: primary / accent / borderSoft</p>
                        </div>
                        <div>
                          <p className="font-semibold text-primaryText">Espaçamento/Raio</p>
                          <p>Padding: sm/md</p>
                          <p>Raio: mdToken ou pillToken</p>
                        </div>
                        <div>
                          <p className="font-semibold text-primaryText">Sombras</p>
                          <p>shadow-softToken + hover cardHoverToken</p>
                        </div>
                      </div>
                    </div>
	                    <div className="rounded-lgToken border border-borderSoft bg-surfaceSubtle p-3 flex items-start gap-2">
	                      <Icon name="info" size={16} className="text-primary" />
	                      <div>
	                        <p className="text-sm font-semibold text-primaryText">Acessibilidade</p>
	                        <p className="text-xs text-secondaryText">{item.accessibility}</p>
	                      </div>
	                    </div>
                    <div className="rounded-lgToken border border-borderSoft bg-[#0B1120] text-white p-3 relative">
                      <p className="text-sm font-semibold mb-1 flex items-center justify-between">
                        <span>Snippet</span>
	                        <Button
	                          variant="ghost"
	                          size="sm"
	                          className="text-xs text-white hover:bg-white/10"
	                          onClick={() => navigator.clipboard.writeText(item.snippet)}
	                        >
		                          <Icon name="content_copy" size={14} className="text-current" /> Copiar
		                        </Button>
	                      </p>
	                      <pre className="text-xs whitespace-pre-wrap font-mono">{item.snippet}</pre>
	                    </div>
                  </AccordionContent>
                </AccordionItem>
              </Accordion>
            ))}
          </div>
        </DialogContent>
      </Dialog>

      {/* Modal: guia de tokens */}
      <Dialog open={openTokens} onOpenChange={setOpenTokens}>
        <DialogContent className="max-w-3xl max-h-[90vh] overflow-visible">
	          <DialogHeader className="sticky top-0 z-10 pb-4">
	            <DialogTitle className="flex items-center gap-2 text-primaryText">
	              <Icon name="code" size={20} className="text-primary" />
	              Guia rápido de tokens
	            </DialogTitle>
            <DialogDescription>
              Estrutura semântica, grupos e lógica de nomenclatura dos tokens Fourmakers.
            </DialogDescription>
          </DialogHeader>
          <div className="space-y-4 max-h-[70vh] overflow-y-auto pr-1">
            <div className="rounded-lgToken border border-borderSoft bg-primarySoft p-3 text-sm text-primaryText">
              <strong>Semântica:</strong> nomes refletem propósito (ex: <code className="font-mono text-xs">primary</code> = cor de ação,
              <code className="font-mono text-xs">surfaceElevated</code> = fundo de cards). Evite nomes descritivos genéricos como
              <code className="font-mono text-xs">blue-500</code>; sempre prefira tokens.
            </div>
            <Accordion type="multiple" className="space-y-3">
              {tokenGuide.map((item) => (
                <AccordionItem
                  key={item.grupo}
                  value={item.grupo}
                  className="rounded-lgToken border border-borderSoft bg-surfaceElevated shadow-softToken"
                >
	                  <AccordionTrigger className="px-4 py-3">
	                    <div className="flex items-center gap-2">
	                      <Icon name="info" size={16} className="text-primary" />
	                      <p className="text-sm font-semibold text-primaryText">{item.grupo}</p>
	                      <Badge variant="secondary" className="text-[11px]">{item.tipo}</Badge>
	                    </div>
	                  </AccordionTrigger>
                  <AccordionContent className="px-4 pb-4 pt-0">
                    <p className="text-xs text-secondaryText mb-3">{item.nota}</p>
                    <div className="grid gap-4">
                      {((item as any).sections || [{ label: item.grupo, tokens: (item as any).tokens || [] }]).map(
                        (section: any, idx: number, arr: any[]) => (
                          <div key={section.label} className="space-y-2">
                            <div className="text-xs font-semibold text-primaryText flex items-center gap-2">
                              <span>{section.label}</span>
                            </div>
                            <div className="grid gap-2">
                              {section.tokens.map((token: any) => {
                                const usageCount =
                                  tokenUsageCounts[token.id] ??
                                  tokenUsageDetails[token.id]?.total ??
                                  tokenUsageFallback[token.id]?.total ??
                                  0;
                                const isColor =
                                  token.value.startsWith("#") ||
                                  token.value.startsWith("rgb") ||
                                  token.value.startsWith("hsl") ||
                                  token.value.toLowerCase().includes("gradient");
                                const isSpacing = token.id.includes("space");
                                const isRadius = token.id.includes("radius");
                                const numeric = parseFloat(token.value);
                                const size = Number.isFinite(numeric) ? Math.max(12, Math.min(48, numeric)) : 16;
                                const swatchStyle = isSpacing
                                  ? { width: `${size}px`, height: `${size}px`, background: "var(--color-primary)", borderRadius: "4px" }
                                  : isRadius
                                    ? { width: "48px", height: "48px", background: "var(--color-primary)", borderRadius: token.value }
                                    : isColor
                                      ? { background: token.value }
                                      : { background: "var(--color-surface-subtle)" };
                                return (
                                  <div
                                    key={token.id}
                                    className="flex items-center justify-between rounded-md border border-borderSoft bg-surfaceSubtle px-3 py-2"
                                  >
                                    <div className="flex items-center gap-3">
                                      <div
                                        className="h-9 w-9 rounded-md border border-borderDefault shadow-softToken flex-shrink-0"
                                        style={swatchStyle}
                                        aria-hidden
                                        title={token.value}
                                      />
                                      <div className="space-y-0.5">
                                        <p className="text-sm font-semibold text-primaryText">{token.label}</p>
                                        <p className="text-xs text-secondaryText">{token.id}</p>
                                      </div>
                                    </div>
                                    <div className="flex items-center gap-3">
                                      <TooltipProvider>
                                        <Tooltip>
                                          <TooltipTrigger asChild>
                                            <div className="text-xs font-mono text-primaryText flex items-center gap-1">
                                              <span>{token.value}</span>
                                              <button
                                                type="button"
                                                className="p-1 rounded-md hover:bg-primarySoft text-primary"
	                                                onClick={() => navigator.clipboard.writeText(token.value)}
	                                                title="Copiar valor"
	                                              >
	                                                <Icon name="content_copy" size={14} />
	                                              </button>
	                                            </div>
                                          </TooltipTrigger>
                                          <TooltipContent className="text-xs">Clique para copiar o valor do token</TooltipContent>
                                        </Tooltip>
                                      </TooltipProvider>
                                      <TooltipProvider>
                                        <Tooltip>
	                                          <TooltipTrigger asChild>
	                                            <div className="flex items-center gap-1 text-xs text-secondaryText cursor-help">
	                                              <Icon name="check" size={14} className="text-primary" />
	                                              <span>{usageCount} usos</span>
	                                            </div>
	                                          </TooltipTrigger>
                                          <TooltipContent className="text-xs p-0 overflow-hidden min-w-[20rem] max-w-[40rem]">
                                            <div className="sticky top-0 z-10 bg-surfaceElevated border-b border-borderSoft px-3 py-2">
                                              <p className="font-semibold text-primaryText leading-tight text-[11px] sm:text-xs">
                                                {buildRoiTitle(
                                                  TOKEN_DEV_HOURS,
                                                  tokenUsageCounts[token.id] ??
                                                    tokenUsageDetails[token.id]?.total ??
                                                    tokenUsageFallback[token.id]?.total ??
                                                    0,
                                                )}
                                              </p>
                                            </div>
                                            <div className="max-h-[600px] overflow-y-auto px-3 py-2 space-y-1">
                                              {[...(tokenUsageDetails[token.id]?.items || tokenUsageFallback[token.id]?.items || [])]
                                                .sort((a, b) => b.count - a.count)
                                                .map((use) => (
                                                <p key={use.label} className="flex justify-between gap-2">
                                                  <span>{use.label}</span>
                                                  <span className="font-semibold">{use.count}</span>
                                                </p>
                                              ))}
                                              {((!tokenUsageDetails[token.id]?.items || tokenUsageDetails[token.id]?.items.length === 0) &&
                                                (!tokenUsageFallback[token.id]?.items || tokenUsageFallback[token.id]?.items.length === 0)) && (
                                                <p className="text-secondaryText">Sem usos mapeados.</p>
                                              )}
                                            </div>
                                          </TooltipContent>
                                        </Tooltip>
                                      </TooltipProvider>
                                    </div>
                                  </div>
                                );
                              })}
                            </div>
                            {idx < arr.length - 1 && <Separator className="my-2" />}
                          </div>
                        ),
                      )}
                    </div>
                  </AccordionContent>
                </AccordionItem>
              ))}
            </Accordion>
            <div className="rounded-lgToken border border-borderSoft bg-surfaceSubtle p-3 text-xs text-secondaryText space-y-1">
              <p><strong>Estrutural:</strong> spacing, radius e sombras garantem ritmo e hierarquia sem depender de cor.</p>
              <p><strong>Funcional:</strong> cores de ação, estados e superfícies comunicam interação e contexto.</p>
              <p><strong>Marca:</strong> gradiente brand e acentos verdes mantêm a assinatura visual Fourmakers.</p>
            </div>
          </div>
        </DialogContent>
      </Dialog>

      {/* Acessibilidade geral */}
      <section className="container mx-auto px-4 lg:px-10 pb-12">
          <div className="space-y-4">
            <div className="flex items-center gap-2">
            <Icon name="accessibility" className="h-5 w-5 text-primary" />
            <h2 className="section-title">Boas práticas de Acessibilidade (A11y)</h2>
            </div>
          <div className="grid gap-4 lg:grid-cols-2">
            <Card>
              <CardHeader>
                <CardTitle>Fundamentos</CardTitle>
                <CardDescription>Aplicar em todas as telas e componentes.</CardDescription>
              </CardHeader>
              <CardContent className="space-y-2 text-sm text-secondaryText">
                <p>• Contraste mínimo 4.5:1 para texto normal; usar tokens de cor garante consistência.</p>
                <p>• Foco visível sempre (já presente em buttons/inputs); não remover outline.</p>
                <p>• Labels claros em inputs e selects; placeholder não é label.</p>
                <p>• Ordem de tabulação lógica; evite tabindex manual exceto para correções.</p>
                <p>• Ícones precisam de texto alternativo quando forem essenciais (aria-label ou texto visível).</p>
              </CardContent>
            </Card>
            <Card>
              <CardHeader>
                <CardTitle>Padrões práticos</CardTitle>
                <CardDescription>Dicas rápidas por componente.</CardDescription>
              </CardHeader>
              <CardContent className="space-y-2 text-sm text-secondaryText">
                <p>• Botões: use tipo adequado (button/submit) e textos imperativos.</p>
                <p>• Formularios: agrupe campos com legendas quando fizer sentido e informe erros com texto.</p>
                <p>• Tabelas: preserve <code className="font-mono text-xs">thead</code>/<code className="font-mono text-xs">th</code> e use títulos de coluna claros.</p>
                <p>• Accordion/Dialog: Radix já entrega ARIA; mantenha títulos e descrições objetivos.</p>
                <p>• Dark mode: evite saturação baixa demais; tokens já cuidam de contraste, não force cores.</p>
              </CardContent>
            </Card>
          </div>
        </div>
      </section>
    </div>
  );
}
