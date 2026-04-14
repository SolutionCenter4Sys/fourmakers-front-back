import { useState, type ReactElement } from "react";
import {
  Accordion,
  AccordionContent,
  AccordionItem,
  AccordionTrigger,
} from "@/components/ui/accordion";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Checkbox } from "@/components/ui/checkbox";
import { Slider } from "@/components/ui/slider";
import { Switch } from "@/components/ui/switch";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Textarea } from "@/components/ui/textarea";
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from "@/components/ui/tooltip";
import {
  Select,
  SelectContent,
  SelectGroup,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { toast } from "@/hooks/use-toast";
import { Icon, type IconVariant } from "@/components/ui/icon";

type ComponentSpec = {
  name: string;
  description: string;
  snippet: string;
  element: ReactElement;
  accessibility: string;
};

const DocExampleButton = () => (
  <div className="flex items-center gap-3">
    <Button variant="primary">Primário</Button>
    <Button variant="secondary">Secundário</Button>
    <Button variant="outline">Outline</Button>
    <Button variant="ghost">Ghost</Button>
  </div>
);

const DocExampleInput = () => (
  <div className="space-y-4">
    <div className="space-y-2">
      <Label className="text-sm text-primaryText">Campo normal</Label>
      <Input
        placeholder="Digite algo..."
        className="bg-field001 border-borderDefault text-primaryText placeholder:text-placeholder rounded-lgToken"
      />
    </div>
    <div className="space-y-2">
      <Label className="text-sm text-primaryText">Campo com erro</Label>
      <Input
        placeholder="Ex.: valor inválido"
        error
        className="bg-field001 text-primaryText placeholder:text-placeholder rounded-lgToken"
      />
    </div>
  </div>
);

const DocExampleTextarea = () => (
  <Textarea
    placeholder="Descreva o contexto..."
    className="bg-field001 border-borderDefault text-primaryText placeholder:text-placeholder rounded-lgToken"
    rows={4}
  />
);

const DocExampleCheckbox = () => (
  <label className="flex items-center gap-2 text-sm text-primaryText">
    <Checkbox defaultChecked />
    <span>Ativar notificações</span>
  </label>
);

const DocExampleDropdown = () => (
  <Select defaultValue="hoje">
    <SelectTrigger className="w-[220px] bg-field001 border-borderDefault rounded-lgToken">
      <SelectValue placeholder="Selecione" />
    </SelectTrigger>
    <SelectContent>
      <SelectGroup>
        <SelectItem value="hoje">Hoje</SelectItem>
        <SelectItem value="semana">Última semana</SelectItem>
        <SelectItem value="mes">Último mês</SelectItem>
      </SelectGroup>
    </SelectContent>
  </Select>
);

const DocExampleCounter = () => {
  const [value, setValue] = useState(1);
  return (
    <div className="inline-flex items-center gap-2 rounded-pillToken border border-borderSoft bg-surfaceSubtle px-md py-2xs">
      <Button variant="ghost" size="icon" onClick={() => setValue((v) => Math.max(0, v - 1))}>
        -
      </Button>
      <span className="min-w-[28px] text-center font-semibold text-primaryText">{value}</span>
      <Button variant="ghost" size="icon" onClick={() => setValue((v) => v + 1)}>
        +
      </Button>
    </div>
  );
};

const DocExampleSlider = () => (
  <div className="w-full max-w-sm space-y-2">
    <div className="flex items-center justify-between text-xs text-secondaryText">
      <span>Senioridade</span>
      <span>70%</span>
    </div>
    <Slider defaultValue={[70]} max={100} step={5} className="text-primary" />
  </div>
);

const DocExampleSwitch = () => (
  <div className="flex items-center gap-2 text-sm">
    <Switch defaultChecked />
    <span className="text-primaryText">Habilitar dark mode automático</span>
  </div>
);

const DocExampleTabs = () => (
  <Tabs defaultValue="tab1" className="w-full max-w-md">
    <TabsList className="grid w-full grid-cols-3">
      <TabsTrigger value="tab1">Geral</TabsTrigger>
      <TabsTrigger value="tab2">Custo</TabsTrigger>
      <TabsTrigger value="tab3">Logs</TabsTrigger>
    </TabsList>
    <TabsContent value="tab1" className="text-sm text-secondaryText p-3 border border-borderSoft rounded-lgToken">
      Tokens de cores e raios aplicados nos inputs.
    </TabsContent>
    <TabsContent value="tab2" className="text-sm text-secondaryText p-3 border border-borderSoft rounded-lgToken">
      Resumo financeiro com badges.
    </TabsContent>
    <TabsContent value="tab3" className="text-sm text-secondaryText p-3 border border-borderSoft rounded-lgToken">
      Histórico de alterações.
    </TabsContent>
  </Tabs>
);

const DocExampleTooltip = () => (
  <TooltipProvider>
    <Tooltip>
      <TooltipTrigger asChild>
        <Button variant="ghost" size="sm">
          Hover para dicas
        </Button>
      </TooltipTrigger>
      <TooltipContent className="text-xs max-w-xs">
        Usa tokens de borda, sombra e superfície. Evite tooltips para textos longos.
      </TooltipContent>
    </Tooltip>
  </TooltipProvider>
);

const DocExampleSnackbar = () => (
  <div className="flex flex-wrap items-center gap-3">
    <Button
      variant="secondary"
      onClick={() =>
        toast({
          title: "Notificação",
          description: "Snackbar default (fundo de card e texto padrão).",
        })
      }
    >
      Snackbar Default
    </Button>
    <Button
      variant="secondary"
      onClick={() =>
        toast({
          title: "Informação",
          description: "Snackbar informativo com borda e texto semânticos.",
          variant: "info",
        })
      }
    >
      Snackbar Info
    </Button>
    <Button
      variant="secondary"
      onClick={() =>
        toast({
          title: "Sucesso",
          description: "Ação concluída com sucesso.",
          variant: "success",
        })
      }
    >
      Snackbar Sucesso
    </Button>
    <Button
      variant="secondary"
      onClick={() =>
        toast({
          title: "Erro",
          description: "Ocorreu um erro ao processar sua solicitação.",
          variant: "destructive",
        })
      }
    >
      Snackbar Erro
    </Button>
  </div>
);

const DocExampleIcon = () => {
  const [iconName, setIconName] = useState("dashboard");
  const [variant, setVariant] = useState<IconVariant>("outlined");
  const [size, setSize] = useState(20);
  const [wght, setWght] = useState(400);
  const [opsz, setOpsz] = useState(20);
  const [grad, setGrad] = useState(0);
  const fill: 0 | 1 = variant === "outlined" ? 0 : 1;

  const resolvedIconName = (() => {
    const trimmed = iconName.trim();
    const isValidLigatureName = (name: string) => /^[a-z0-9_]+$/i.test(name);
    return trimmed && isValidLigatureName(trimmed) ? trimmed : "help";
  })();

  const iconCode = `<Icon
  name="${resolvedIconName}"
  variant="${variant}"
  size={${size}}
  axes={{ wght: ${wght}, opsz: ${opsz}, grad: ${grad} }}
/>`;

  const handleCopyIconCode = async () => {
    try {
      await navigator.clipboard.writeText(iconCode);
      toast({ title: "Código copiado", description: "Snippet do Icon copiado para a área de transferência." });
    } catch {
      toast({ title: "Não foi possível copiar", description: "Copie manualmente o snippet abaixo.", variant: "destructive" });
    }
  };

  return (
    <div className="space-y-4">
      <div className="flex items-center gap-3 rounded-lgToken border border-borderSoft bg-surfaceElevated px-4 py-3">
        <Icon
          name={iconName}
          variant={variant}
          size={size}
          axes={{ wght, opsz, grad }}
          className="text-primary"
          title={iconName}
        />
        <div className="min-w-0">
          <p className="text-sm font-semibold text-primaryText leading-tight truncate">{iconName || "—"}</p>
          <p className="text-xs text-secondaryText">
            {`variant=${variant} • size=${size}px • FILL=${fill} • wght=${wght} • opsz=${opsz} • GRAD=${grad}`}
          </p>
        </div>
      </div>

      <div className="grid gap-4 lg:grid-cols-2">
        <div className="space-y-2">
          <Label>Nome do ícone (ligature)</Label>
          <Input
            value={iconName}
            onChange={(e) => setIconName(e.target.value)}
            placeholder="Ex: dashboard, settings, logout"
            className="bg-field001 border-borderDefault rounded-lgToken"
          />
          <p className="text-xs text-secondaryText">
            Se o nome for inválido/vazio, cai no fallback <code className="font-mono">help</code>.
          </p>
        </div>

        <div className="space-y-2">
          <Label>Variante</Label>
          <Select value={variant} onValueChange={(v) => setVariant(v as IconVariant)}>
            <SelectTrigger className="bg-field001 border-borderDefault rounded-lgToken">
              <SelectValue />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="default">Default</SelectItem>
              <SelectItem value="outlined">Outlined</SelectItem>
            </SelectContent>
          </Select>
          <p className="text-xs text-secondaryText">FILL é automático: Default = 1 • Outlined = 0.</p>
        </div>
      </div>

      <div className="grid gap-4 md:grid-cols-2">
        <div className="space-y-2">
          <div className="flex items-center justify-between text-xs text-secondaryText">
            <span>Tamanho</span>
            <span className="font-semibold text-primaryText">{size}px</span>
          </div>
          <Slider value={[size]} min={12} max={48} step={1} onValueChange={([v]) => setSize(v)} />
        </div>
        <div className="space-y-2">
          <div className="flex items-center justify-between text-xs text-secondaryText">
            <span>Peso</span>
            <span className="font-semibold text-primaryText">{wght}</span>
          </div>
          <Slider value={[wght]} min={100} max={700} step={50} onValueChange={([v]) => setWght(v)} />
        </div>
        <div className="space-y-2">
          <div className="flex items-center justify-between text-xs text-secondaryText">
            <div className="flex items-center gap-1.5">
              <span>Tamanho óptico</span>
              <TooltipProvider>
                <Tooltip>
                  <TooltipTrigger asChild>
                    <button type="button" className="inline-flex items-center justify-center text-secondaryText hover:text-primaryText">
                      <Icon name="info" variant="outlined" className="h-4 w-4" />
                    </button>
                  </TooltipTrigger>
                  <TooltipContent className="max-w-xs text-xs">
                    <p>
                      <strong>opsz</strong> ajusta o desenho do ícone para o tamanho óptico: em tamanhos menores, melhora legibilidade e
                      detalhes; em tamanhos maiores, preserva proporções.
                    </p>
                  </TooltipContent>
                </Tooltip>
              </TooltipProvider>
            </div>
            <span className="font-semibold text-primaryText">{opsz}</span>
          </div>
          <Slider value={[opsz]} min={20} max={48} step={1} onValueChange={([v]) => setOpsz(v)} />
        </div>
        <div className="space-y-2">
          <div className="flex items-center justify-between text-xs text-secondaryText">
            <div className="flex items-center gap-1.5">
              <span>Grade</span>
              <TooltipProvider>
                <Tooltip>
                  <TooltipTrigger asChild>
                    <button type="button" className="inline-flex items-center justify-center text-secondaryText hover:text-primaryText">
                      <Icon name="info" variant="outlined" className="h-4 w-4" />
                    </button>
                  </TooltipTrigger>
                  <TooltipContent className="max-w-xs text-xs">
                    <p>
                      <strong>GRAD</strong> controla a “graduação”/contraste do traço: valores negativos deixam mais leve; positivos deixam o
                      traço mais forte, sem trocar o peso (wght).
                    </p>
                  </TooltipContent>
                </Tooltip>
              </TooltipProvider>
            </div>
            <span className="font-semibold text-primaryText">{grad}</span>
          </div>
          <Slider value={[grad]} min={-50} max={200} step={10} onValueChange={([v]) => setGrad(v)} />
        </div>
      </div>

      <div className="rounded-lgToken border border-borderSoft bg-[#0B1120] text-white p-3 relative">
        <p className="text-sm font-semibold mb-1 flex items-center justify-between">
          <span>Código do ícone</span>
          <Button
            variant="ghost"
            size="sm"
            className="text-xs text-white hover:bg-white/10"
            type="button"
            onClick={handleCopyIconCode}
          >
            <Icon name="content_copy" size={14} className="text-current" /> Copiar
          </Button>
        </p>
        <pre className="text-xs whitespace-pre-wrap font-mono">{iconCode}</pre>
      </div>

      <div className="space-y-2">
        <p className="text-xs font-semibold text-primaryText">Atalhos (exemplos)</p>
        <div className="flex flex-wrap gap-2">
          {["dashboard", "settings", "search", "person", "logout", "notifications", "menu", "help"].map((preset) => (
            <Button
              key={preset}
              variant="outline"
              size="sm"
              onClick={() => setIconName(preset)}
              className="rounded-pillToken"
              type="button"
            >
              {preset}
            </Button>
          ))}
        </div>
      </div>
    </div>
  );
};

const DocExampleTable = () => (
  <div className="border border-borderSoft rounded-lgToken bg-surfaceElevated shadow-softToken">
    <Table>
      <TableHeader>
        <TableRow>
          <TableHead>Jornada</TableHead>
          <TableHead>Status</TableHead>
          <TableHead>Ações</TableHead>
        </TableRow>
      </TableHeader>
      <TableBody>
        <TableRow>
          <TableCell>Onboarding</TableCell>
          <TableCell>
            <Badge variant="accent">Ativo</Badge>
          </TableCell>
          <TableCell>
            <Button variant="ghost" size="sm">
              Ver
            </Button>
          </TableCell>
        </TableRow>
        <TableRow>
          <TableCell>Faturamento</TableCell>
          <TableCell>
            <Badge variant="secondary">Rascunho</Badge>
          </TableCell>
          <TableCell>
            <Button variant="ghost" size="sm">
              Ver
            </Button>
          </TableCell>
        </TableRow>
      </TableBody>
    </Table>
  </div>
);

const DocExampleCard = () => (
  <Card className="max-w-md">
    <CardHeader>
      <CardTitle>Card padrão</CardTitle>
      <CardDescription>Uso de superfícies elevadas e bordas suaves.</CardDescription>
    </CardHeader>
    <CardContent className="space-y-3">
      <p className="text-sm text-secondaryText">
        Utilize <code className="font-mono text-xs">bg-surfaceElevated</code>,{" "}
        <code className="font-mono text-xs">border-borderSoft</code> e{" "}
        <code className="font-mono text-xs">shadow-softToken</code> para consistência.
      </p>
      <Button variant="primary" size="sm">
        Ação
      </Button>
    </CardContent>
  </Card>
);

const DocExampleAccordion = () => (
  <Accordion type="single" collapsible className="w-full rounded-lgToken border border-borderSoft bg-surfaceSubtle">
    <AccordionItem value="item-1">
      <AccordionTrigger className="px-md">Accordion tokenizado</AccordionTrigger>
      <AccordionContent className="px-md pb-md text-sm text-secondaryText">
        Use <code className="font-mono text-xs">border-borderSoft</code> e superfícies neutras para conteúdos colapsáveis.
      </AccordionContent>
    </AccordionItem>
  </Accordion>
);

export const componentsSpec: ComponentSpec[] = [
  {
    name: "Icon",
    description: "Ícone do design system (Material Symbols). Suporta axes do variable font e fallback para help.",
    element: <DocExampleIcon />,
    snippet: `import { Icon } from "@/components/ui/icon";

<Icon
  name="dashboard"
  variant="outlined"
  size={20}
  axes={{ wght: 500, opsz: 24, grad: 0, fill: 0 }}
/>`,
    accessibility: "Use title/aria-label quando o ícone for o único conteúdo de um botão; não dependa só de ícone para transmitir significado.",
  },
  {
    name: "Button",
    description: "Variantes primária, secundária e ghost usando tokens de cor e raio pill.",
    element: <DocExampleButton />,
    snippet: `<Button variant="primary">Primário</Button>
<Button variant="secondary">Secundário</Button>
<Button variant="outline">Outline</Button>
<Button variant="ghost">Ghost</Button>`,
    accessibility: "Use texto descritivo, mantenha foco visível (já aplicado) e declare tipo=“button” quando não for submit.",
  },
  {
    name: "Input",
    description: "Campos neutros com fundo field001 e borda default. Suporta estado de erro (prop error) com borda destrutiva e aria-invalid.",
    element: <DocExampleInput />,
    snippet: `<Input
  className="bg-field001 border-borderDefault rounded-lgToken
             text-primaryText placeholder:text-placeholder"
  placeholder="Digite algo..."
/>
<Input error placeholder="Campo com erro" />`,
    accessibility: "Associe sempre label visível ou aria-label. Use a prop error para validação; o componente define aria-invalid automaticamente.",
  },
  {
    name: "Textarea",
    description: "Área de texto com borda default e placeholder neutro.",
    element: <DocExampleTextarea />,
    snippet: `<Textarea
  className="bg-field001 border-borderDefault rounded-lgToken
             text-primaryText placeholder:text-placeholder"
  rows={4}
  placeholder="Descreva o contexto..."
/>`,
    accessibility: "Inclua label clara e descreva limites de caracteres se houver.",
  },
  {
    name: "Dropdown",
    description: "Select com superfícies neutras e foco roxo.",
    element: <DocExampleDropdown />,
    snippet: `<Select defaultValue="hoje">
  <SelectTrigger className="bg-field001 border-borderDefault rounded-lgToken">
    <SelectValue placeholder="Selecione" />
  </SelectTrigger>
  <SelectContent>
    <SelectItem value="hoje">Hoje</SelectItem>
    <SelectItem value="semana">Última semana</SelectItem>
  </SelectContent>
  </Select>`,
    accessibility: "Mantenha labels claros e use valores de opção compreensíveis. Foco já é gerenciado pelo Radix.",
  },
  {
    name: "Checkbox",
    description: "Seleção binária com realce no foco/hover.",
    element: <DocExampleCheckbox />,
    snippet: `<label className="flex items-center gap-2 text-sm">
  <Checkbox defaultChecked />
  <span>Ativar notificações</span>
</label>`,
    accessibility: "Relacione com label visível; use aria-checked quando customizar além do padrão.",
  },
  {
    name: "Counter",
    description: "Incremento/decremento com botões ghost e raio pill.",
    element: <DocExampleCounter />,
    snippet: `<div className="inline-flex items-center gap-2 rounded-pillToken border border-borderSoft bg-surfaceSubtle px-md py-2xs">
  <Button variant="ghost" size="icon">-</Button>
  <span className="min-w-[28px] text-center font-semibold">1</span>
  <Button variant="ghost" size="icon">+</Button>
</div>`,
  accessibility: "Inclua aria-label nos botões de adicionar/remover para leitores de tela.",
  },
  {
    name: "Slider",
    description: "Controle contínuo para selecionar intervalos ou porcentagens.",
    element: <DocExampleSlider />,
    snippet: `<Slider defaultValue={[70]} max={100} step={5} className="text-primary" />`,
    accessibility: "Use aria-label ou aria-labelledby; mantenha contraste no handle e track.",
  },
  {
    name: "Switch",
    description: "Alternância rápida de estados on/off.",
    element: <DocExampleSwitch />,
    snippet: `<div className="flex items-center gap-2">
  <Switch defaultChecked />
  <span>Habilitar dark mode automático</span>
</div>`,
    accessibility: "Forneça label clara e estado textual quando necessário.",
  },
  {
    name: "Table",
    description: "Tabela leve com badges tokenizados e botões ghost.",
    element: <DocExampleTable />,
    snippet: `<div className="border border-borderSoft rounded-lgToken bg-surfaceElevated shadow-softToken">
  <Table>...</Table>
</div>`,
    accessibility: "Use cabeçalhos semânticos <th>, forneça legendas quando necessário e preserve ordem de tabulação.",
  },
  {
    name: "Card",
    description: "Superfície elevada com sombra suave e botão CTA.",
    element: <DocExampleCard />,
    snippet: `<Card className="bg-surfaceElevated border-borderSoft shadow-softToken">
  <CardHeader>...</CardHeader>
  <CardContent>...</CardContent>
</Card>`,
    accessibility: "Mantenha contraste de texto >= 4.5:1, especialmente para descrições.",
  },
  {
    name: "Accordion",
    description: "Conteúdo expansível com bordas suaves e fundo sutil.",
    element: <DocExampleAccordion />,
    snippet: `<Accordion type="single" collapsible className="border-borderSoft bg-surfaceSubtle rounded-lgToken">
  <AccordionItem value="item-1">
    <AccordionTrigger>Accordion tokenizado</AccordionTrigger>
    <AccordionContent>Conteúdo</AccordionContent>
  </AccordionItem>
</Accordion>`,
    accessibility: "Radix já entrega ARIA; mantenha ordem lógica e textos de gatilho descritivos.",
  },
  {
    name: "Tabs",
    description: "Navegação entre seções sem recarregar a página.",
    element: <DocExampleTabs />,
    snippet: `<Tabs defaultValue="tab1">
  <TabsList>
    <TabsTrigger value="tab1">Geral</TabsTrigger>
    <TabsTrigger value="tab2">Custo</TabsTrigger>
  </TabsList>
  <TabsContent value="tab1">Conteúdo</TabsContent>
  <TabsContent value="tab2">Conteúdo</TabsContent>
</Tabs>`,
    accessibility: "Radix provê ARIA; mantenha rótulos curtos e claros.",
  },
  {
    name: "Tooltip",
    description: "Suporte contextual curto em hover/foco.",
    element: <DocExampleTooltip />,
    snippet: `<TooltipProvider>
  <Tooltip>
    <TooltipTrigger asChild>
      <Button variant="ghost" size="sm">Hover para dicas</Button>
    </TooltipTrigger>
    <TooltipContent>Texto curto de ajuda.</TooltipContent>
  </Tooltip>
</TooltipProvider>`,
    accessibility: "Evite textos longos; use para dicas curtas e garanta foco acessível.",
  },
  {
    name: "Snackbar",
    description: "Snackbar com mensagens informativas e feedbacks do sistema para o usuário.",
    element: <DocExampleSnackbar />,
    snippet: `import { toast } from "@/hooks/use-toast";

toast({
  title: "Sucesso",
  description: "Perfil salvo com sucesso.",
  variant: "success",
});`,
    accessibility: "Use mensagens curtas e acionáveis; evite spam. Para erros, inclua o próximo passo.",
  },
];
