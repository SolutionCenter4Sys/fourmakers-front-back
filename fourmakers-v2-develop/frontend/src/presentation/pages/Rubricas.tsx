import { useState } from "react";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";
import { Switch } from "@/components/ui/switch";
import { Badge } from "@/components/ui/badge";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import {
  Command,
  CommandEmpty,
  CommandGroup,
  CommandInput,
  CommandItem,
  CommandList,
} from "@/components/ui/command";
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "@/components/ui/popover";
import { Calendar } from "@/components/ui/calendar";
import { format } from "date-fns";
import { ptBR } from "date-fns/locale";
import { CalendarIcon, Check, ChevronsUpDown, Plus, Receipt, Settings, Upload as UploadIcon, Trash2, FileText, Edit, RefreshCw } from "@/components/ui/system-icons";
import { cn } from "@/lib/utils";
import { SearchCard } from "@presentation/components/common/SearchCard";
import { DataTable } from "@presentation/components/common";
import type { Column } from "@/hooks/useColumnReorder";

// Mock data
const mockColaboradores = [
  { id: "1", nome: "João Silva" },
  { id: "2", nome: "Maria Santos" },
  { id: "3", nome: "Pedro Oliveira" },
];

const mockRubricas = [
  { id: "1", nome: "Salário Base" },
  { id: "2", nome: "Horas Extras" },
  { id: "3", nome: "Vale Transporte" },
];

const mockUnidades = [
  { id: "1", nome: "Matriz" },
  { id: "2", nome: "Filial SP" },
  { id: "3", nome: "Filial RJ" },
];

const mockCnpjUnidades = [
  { id: "1", cnpj: "12.345.678/0001-90", nome: "Matriz" },
  { id: "2", cnpj: "98.765.432/0001-10", nome: "Filial SP" },
  { id: "3", cnpj: "11.222.333/0001-44", nome: "Filial RJ" },
];

const mockCategorias = [
  { id: "1", nome: "Proventos" },
  { id: "2", nome: "Descontos" },
  { id: "3", nome: "Benefícios" },
];

const mockTiposCalculo = [
  { id: "1", nome: "Valor Fixo" },
  { id: "2", nome: "Porcentagem" },
  { id: "3", nome: "Quantidade" },
];

const mockRubricasConfig = [
  {
    id: "1",
    codigo: "VT001",
    nome: "Vale Transporte",
    categoria: "Benefícios",
    tipoCalculo: "Valor Fixo",
  },
  {
    id: "2",
    codigo: "HE050",
    nome: "Horas Extras 50%",
    categoria: "Proventos",
    tipoCalculo: "Porcentagem",
  },
  {
    id: "3",
    codigo: "INSS",
    nome: "INSS",
    categoria: "Descontos",
    tipoCalculo: "Porcentagem",
  },
];

const mockHistoricoImportacoes = [
  {
    id: 1,
    descricao: "[addDataMask(a)]",
    status: "Processado",
    competencia: "01/2024",
    registros: "150/150",
    nomeUsuario: "João Silva",
    dataImportacao: "15/01/2024 14:30",
  },
  {
    id: 2,
    descricao: "[addDataMask(a)]",
    status: "Erro",
    competencia: "12/2023",
    registros: "45/150",
    nomeUsuario: "Maria Santos",
    dataImportacao: "10/01/2024 09:15",
  },
  {
    id: 3,
    descricao: "[addDataMask(a)]",
    status: "Processando",
    competencia: "02/2024",
    registros: "80/150",
    nomeUsuario: "Pedro Oliveira",
    dataImportacao: "20/01/2024 16:45",
  },
];

const mockLancamentos = [
  {
    id: "1",
    colaborador: "João Silva",
    rubrica: "Salário Base",
    tipo: "Valor",
    valorPorcentagem: "R$ 5.000,00",
    vigencia: "01/2024",
    recorrencia: "Mensal",
  },
  {
    id: "2",
    colaborador: "Maria Santos",
    rubrica: "Horas Extras",
    tipo: "Porcentagem",
    valorPorcentagem: "50%",
    vigencia: "01/2024",
    recorrencia: "Eventual",
  },
];

export default function Rubricas() {
  const [competencia, setCompetencia] = useState<Date>();
  const [colaboradorOpen, setColaboradorOpen] = useState(false);
  const [colaboradorValue, setColaboradorValue] = useState("");
  const [rubrica, setRubrica] = useState("");
  const [unidade, setUnidade] = useState("");
  
  // Novo Lançamento Modal
  const [novoLancamentoOpen, setNovoLancamentoOpen] = useState(false);
  const [novoColaboradorOpen, setNovoColaboradorOpen] = useState(false);
  const [novoColaborador, setNovoColaborador] = useState("");
  const [novaRubrica, setNovaRubrica] = useState("");
  const [recorrencia, setRecorrencia] = useState("");
  const [dataInicio, setDataInicio] = useState<Date>();
  const [dataFinal, setDataFinal] = useState<Date>();
  const [valor, setValor] = useState("");
  const [observacoes, setObservacoes] = useState("");
  
  // Nova Rubrica Modal
  const [novaRubricaOpen, setNovaRubricaOpen] = useState(false);
  const [codigoRubrica, setCodigoRubrica] = useState("");
  const [nomeRubrica, setNomeRubrica] = useState("");
  const [categoriaRubrica, setCategoriaRubrica] = useState("");
  const [tipoCalculoRubrica, setTipoCalculoRubrica] = useState("");
  const [refeteContabilidade, setRefeteContabilidade] = useState(false);
  
  // Busca de rubricas
  const [buscaRubrica, setBuscaRubrica] = useState("");
  
  // Importação
  const [rubricaImportacao, setRubricaImportacao] = useState("");
  const [cnpjUnidadeImportacao, setCnpjUnidadeImportacao] = useState("");
  const [vigenciaImportacao, setVigenciaImportacao] = useState<Date>();
  const [arquivoImportacao, setArquivoImportacao] = useState<File | null>(null);
  
  const rubricasFiltradas = mockRubricasConfig.filter(
    (rub) =>
      rub.codigo.toLowerCase().includes(buscaRubrica.toLowerCase()) ||
      rub.nome.toLowerCase().includes(buscaRubrica.toLowerCase()) ||
      rub.categoria.toLowerCase().includes(buscaRubrica.toLowerCase())
  );

  const handleLimpar = () => {
    setCompetencia(undefined);
    setColaboradorValue("");
    setRubrica("");
    setUnidade("");
  };

  const handleBuscar = () => {
    console.log("Buscar lançamentos");
  };

  const handleConfirmarLancamento = () => {
    console.log("Confirmar lançamento");
    setNovoLancamentoOpen(false);
    // Reset form
    setNovoColaborador("");
    setNovaRubrica("");
    setRecorrencia("");
    setDataInicio(undefined);
    setDataFinal(undefined);
    setValor("");
    setObservacoes("");
  };

  const handleCancelarLancamento = () => {
    setNovoLancamentoOpen(false);
    // Reset form
    setNovoColaborador("");
    setNovaRubrica("");
    setRecorrencia("");
    setDataInicio(undefined);
    setDataFinal(undefined);
    setValor("");
    setObservacoes("");
  };

  const handleConfirmarRubrica = () => {
    console.log("Confirmar rubrica");
    setNovaRubricaOpen(false);
    // Reset form
    setCodigoRubrica("");
    setNomeRubrica("");
    setCategoriaRubrica("");
    setTipoCalculoRubrica("");
    setRefeteContabilidade(false);
  };

  const handleCancelarRubrica = () => {
    setNovaRubricaOpen(false);
    // Reset form
    setCodigoRubrica("");
    setNomeRubrica("");
    setCategoriaRubrica("");
    setTipoCalculoRubrica("");
    setRefeteContabilidade(false);
  };

  const handleUploadArquivo = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (file) {
      setArquivoImportacao(file);
    }
  };

  const handleProcessarImportacao = () => {
    console.log("Processar importação");
    // Reset form após processar
    setRubricaImportacao("");
    setCnpjUnidadeImportacao("");
    setVigenciaImportacao(undefined);
    setArquivoImportacao(null);
  };

  const getStatusColor = (status: string) => {
    switch (status) {
      case "Processado":
        return "bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200";
      case "Erro":
        return "bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200";
      case "Processando":
        return "bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-200";
      default:
        return "bg-gray-100 text-gray-800 dark:bg-gray-900 dark:text-gray-200";
    }
  };

  // Colunas e renderCell para Lançamentos
  const lancamentosColumns: Column[] = [
    { id: "colaborador", label: "Colaborador", sortable: true },
    { id: "rubrica", label: "Rubrica", sortable: true },
    { id: "tipo", label: "Tipo", sortable: true },
    { id: "valorPorcentagem", label: "Valor/Porcentagem", sortable: true },
    { id: "vigencia", label: "Vigência", sortable: true },
    { id: "recorrencia", label: "Recorrência", sortable: true },
    { id: "acoes", label: "Ações", sortable: false, width: "w-[80px]" },
  ];

  const renderLancamentoCell = (lancamento: typeof mockLancamentos[0], columnId: string) => {
    switch (columnId) {
      case "colaborador":
        return lancamento.colaborador;
      case "rubrica":
        return lancamento.rubrica;
      case "tipo":
        return lancamento.tipo;
      case "valorPorcentagem":
        return lancamento.valorPorcentagem;
      case "vigencia":
        return lancamento.vigencia;
      case "recorrencia":
        return lancamento.recorrencia;
      case "acoes":
        return (
          <Button variant="ghost" size="icon">
            <Trash2 className="h-4 w-4 text-destructive" />
          </Button>
        );
      default:
        return null;
    }
  };

  // Colunas e renderCell para Rubricas Cadastradas
  const rubricasColumns: Column[] = [
    { id: "codigo", label: "Código", sortable: true },
    { id: "nome", label: "Nome", sortable: true },
    { id: "categoria", label: "Categoria", sortable: true },
    { id: "tipoCalculo", label: "Tipo Cálculo", sortable: true },
    { id: "acoes", label: "Ações", sortable: false, width: "w-[100px]" },
  ];

  const renderRubricaCell = (rubricaItem: typeof mockRubricasConfig[0], columnId: string) => {
    switch (columnId) {
      case "codigo":
        return <span className="font-mono font-medium">{rubricaItem.codigo}</span>;
      case "nome":
        return rubricaItem.nome;
      case "categoria":
        return rubricaItem.categoria;
      case "tipoCalculo":
        return rubricaItem.tipoCalculo;
      case "acoes":
        return (
          <div className="flex justify-end gap-2">
            <Button variant="ghost" size="icon">
              <Edit className="h-4 w-4" />
            </Button>
            <Button variant="ghost" size="icon">
              <Trash2 className="h-4 w-4 text-destructive" />
            </Button>
          </div>
        );
      default:
        return null;
    }
  };

  return (
    <div className="container mx-auto p-6 space-y-6">
      <div className="flex items-center justify-between">
        <h1 className="page-title">Rubricas</h1>
      </div>

      <Tabs defaultValue="lancamentos" className="w-full">
        <div className="mb-6">
          <TabsList className="inline-flex h-auto bg-muted/30 backdrop-blur-sm border border-border/50 p-1.5 rounded-xl gap-2 w-auto shadow-sm">
            <TabsTrigger 
              value="lancamentos" 
              className="relative flex items-center gap-2.5 text-sm font-medium py-2.5 px-5 rounded-lg transition-all duration-200 data-[state=inactive]:text-muted-foreground data-[state=inactive]:hover:text-foreground data-[state=inactive]:hover:bg-muted/50 data-[state=active]:bg-primary data-[state=active]:text-primary-foreground data-[state=active]:shadow-md data-[state=active]:scale-[1.02]"
            >
              <Receipt className="h-4 w-4" />
              <span>Lançamentos</span>
            </TabsTrigger>
            <TabsTrigger 
              value="configuracoes" 
              className="relative flex items-center gap-2.5 text-sm font-medium py-2.5 px-5 rounded-lg transition-all duration-200 data-[state=inactive]:text-muted-foreground data-[state=inactive]:hover:text-foreground data-[state=inactive]:hover:bg-muted/50 data-[state=active]:bg-primary data-[state=active]:text-primary-foreground data-[state=active]:shadow-md data-[state=active]:scale-[1.02]"
            >
              <Settings className="h-4 w-4" />
              <span>Configurações</span>
            </TabsTrigger>
            <TabsTrigger 
              value="importacao" 
              className="relative flex items-center gap-2.5 text-sm font-medium py-2.5 px-5 rounded-lg transition-all duration-200 data-[state=inactive]:text-muted-foreground data-[state=inactive]:hover:text-foreground data-[state=inactive]:hover:bg-muted/50 data-[state=active]:bg-primary data-[state=active]:text-primary-foreground data-[state=active]:shadow-md data-[state=active]:scale-[1.02]"
            >
              <UploadIcon className="h-4 w-4" />
              <span>Importação</span>
            </TabsTrigger>
          </TabsList>
        </div>

        <TabsContent value="lancamentos" className="space-y-6">
          <div>
            <h2 className="text-2xl font-bold">Lançamentos</h2>
            <p className="text-muted-foreground mt-1">
              Visualize e gerencie todas as rubricas salariais dos colaboradores
            </p>
          </div>

          <Card>
            <CardContent className="pt-6">
              <div className="grid grid-cols-1 md:grid-cols-4 gap-4 mb-4">
                {/* Competência */}
                <div className="space-y-2">
                  <Label>Competência</Label>
                  <Popover>
                    <PopoverTrigger asChild>
                      <Button
                        variant="outline"
                        className={cn(
                          "w-full justify-start text-left font-normal",
                          !competencia && "text-muted-foreground"
                        )}
                      >
                        <CalendarIcon className="mr-2 h-4 w-4" />
                        {competencia ? format(competencia, "MM/yyyy", { locale: ptBR }) : "Selecione"}
                      </Button>
                    </PopoverTrigger>
                    <PopoverContent className="w-auto p-0" align="start">
                      <Calendar
                        mode="single"
                        selected={competencia}
                        onSelect={setCompetencia}
                        locale={ptBR}
                      />
                    </PopoverContent>
                  </Popover>
                </div>

                {/* Colaborador (Autocomplete) */}
                <div className="space-y-2">
                  <Label>Colaborador</Label>
                  <Popover open={colaboradorOpen} onOpenChange={setColaboradorOpen}>
                    <PopoverTrigger asChild>
                      <Button
                        variant="outline"
                        role="combobox"
                        aria-expanded={colaboradorOpen}
                        className="w-full justify-between"
                      >
                        {colaboradorValue
                          ? mockColaboradores.find((c) => c.id === colaboradorValue)?.nome
                          : "Selecione"}
                        <ChevronsUpDown className="ml-2 h-4 w-4 shrink-0 opacity-50" />
                      </Button>
                    </PopoverTrigger>
                    <PopoverContent className="w-[300px] p-0">
                      <Command>
                        <CommandInput placeholder="Buscar colaborador..." />
                        <CommandList>
                          <CommandEmpty>Nenhum colaborador encontrado.</CommandEmpty>
                          <CommandGroup>
                            {mockColaboradores.map((colaborador) => (
                              <CommandItem
                                key={colaborador.id}
                                value={colaborador.id}
                                onSelect={(currentValue) => {
                                  setColaboradorValue(currentValue === colaboradorValue ? "" : currentValue);
                                  setColaboradorOpen(false);
                                }}
                              >
                                <Check
                                  className={cn(
                                    "mr-2 h-4 w-4",
                                    colaboradorValue === colaborador.id ? "opacity-100" : "opacity-0"
                                  )}
                                />
                                {colaborador.nome}
                              </CommandItem>
                            ))}
                          </CommandGroup>
                        </CommandList>
                      </Command>
                    </PopoverContent>
                  </Popover>
                </div>

                {/* Rubrica (Dropdown) */}
                <div className="space-y-2">
                  <Label>Rubrica</Label>
                  <Select value={rubrica} onValueChange={setRubrica}>
                    <SelectTrigger>
                      <SelectValue placeholder="Selecione" />
                    </SelectTrigger>
                    <SelectContent>
                      {mockRubricas.map((r) => (
                        <SelectItem key={r.id} value={r.id}>
                          {r.nome}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </div>

                {/* Unidade (Dropdown) */}
                <div className="space-y-2">
                  <Label>Unidade</Label>
                  <Select value={unidade} onValueChange={setUnidade}>
                    <SelectTrigger>
                      <SelectValue placeholder="Selecione" />
                    </SelectTrigger>
                    <SelectContent>
                      {mockUnidades.map((u) => (
                        <SelectItem key={u.id} value={u.id}>
                          {u.nome}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </div>
              </div>

              <div className="flex gap-2">
                <Button variant="outline" onClick={handleLimpar}>
                  Limpar
                </Button>
                <Button onClick={handleBuscar}>
                  Buscar
                </Button>
              </div>
            </CardContent>
          </Card>

          <div className="flex justify-end">
            <Button onClick={() => setNovoLancamentoOpen(true)}>
              <Plus className="mr-2 h-4 w-4" />
              Novo Lançamento
            </Button>
          </div>

          <Card>
            <CardContent className="pt-6">
              <DataTable
                columns={lancamentosColumns}
                data={mockLancamentos}
                keyExtractor={(item) => item.id}
                renderCell={renderLancamentoCell}
                emptyMessage="Nenhum lançamento encontrado"
              />
            </CardContent>
          </Card>
        </TabsContent>

        <TabsContent value="configuracoes" className="mt-6">
          <div className="space-y-6">
            <div>
              <h2 className="text-2xl font-bold">Configurações de Rubricas</h2>
              <p className="text-muted-foreground mt-1">
                Gerencie e configure todas as rubricas do sistema
              </p>
            </div>

            <SearchCard 
              searchTerm={buscaRubrica}
              onSearchChange={setBuscaRubrica}
              placeholder="Buscar rubrica..."
              actionButton={
                <Button onClick={() => setNovaRubricaOpen(true)} className="gap-2">
                  <Plus className="h-4 w-4" />
                  Nova Rubrica
                </Button>
              }
            />

            {/* Tabela de Rubricas */}
            <Card>
              <CardContent className="p-6">
                <DataTable
                  columns={rubricasColumns}
                  data={rubricasFiltradas}
                  keyExtractor={(item) => item.id}
                  renderCell={renderRubricaCell}
                  emptyMessage="Nenhuma rubrica encontrada"
                />
              </CardContent>
            </Card>
          </div>
        </TabsContent>

        <TabsContent value="importacao" className="mt-6">
          <div className="space-y-6">
            {/* Filtros */}
            <Card>
              <CardHeader>
                <CardTitle>Filtros de Importação</CardTitle>
              </CardHeader>
              <CardContent>
                <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                  {/* Rubrica */}
                  <div className="space-y-2">
                    <Label>Rubrica</Label>
                    <Select value={rubricaImportacao} onValueChange={setRubricaImportacao}>
                      <SelectTrigger>
                        <SelectValue placeholder="Selecione" />
                      </SelectTrigger>
                      <SelectContent>
                        {mockRubricas.map((r) => (
                          <SelectItem key={r.id} value={r.id}>
                            {r.nome}
                          </SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                  </div>

                  {/* CNPJ/Unidade */}
                  <div className="space-y-2">
                    <Label>CNPJ/Unidade</Label>
                    <Select value={cnpjUnidadeImportacao} onValueChange={setCnpjUnidadeImportacao}>
                      <SelectTrigger>
                        <SelectValue placeholder="Selecione" />
                      </SelectTrigger>
                      <SelectContent>
                        {mockCnpjUnidades.map((u) => (
                          <SelectItem key={u.id} value={u.id}>
                            {u.cnpj} - {u.nome}
                          </SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                  </div>

                  {/* Vigência */}
                  <div className="space-y-2">
                    <Label>Vigência</Label>
                    <Popover>
                      <PopoverTrigger asChild>
                        <Button
                          variant="outline"
                          className={cn(
                            "w-full justify-start text-left font-normal",
                            !vigenciaImportacao && "text-muted-foreground"
                          )}
                        >
                          <CalendarIcon className="mr-2 h-4 w-4" />
                          {vigenciaImportacao ? format(vigenciaImportacao, "MM/yyyy", { locale: ptBR }) : "Selecione"}
                        </Button>
                      </PopoverTrigger>
                      <PopoverContent className="w-auto p-0" align="start">
                        <Calendar
                          mode="single"
                          selected={vigenciaImportacao}
                          onSelect={setVigenciaImportacao}
                          locale={ptBR}
                          className="pointer-events-auto"
                        />
                      </PopoverContent>
                    </Popover>
                  </div>
                </div>
              </CardContent>
            </Card>

            {/* Upload */}
            <Card>
              <CardHeader>
                <CardTitle>Upload de Arquivo</CardTitle>
              </CardHeader>
              <CardContent>
                <div className="space-y-4">
                  <div className="border-2 border-dashed rounded-lg p-8 text-center hover:border-primary transition-colors">
                    <input
                      type="file"
                      id="file-upload"
                      className="hidden"
                      accept=".xlsx,.xls,.csv"
                      onChange={handleUploadArquivo}
                    />
                    <label htmlFor="file-upload" className="cursor-pointer">
                      <UploadIcon className="mx-auto h-12 w-12 text-muted-foreground mb-4" />
                      <p className="text-sm font-medium mb-1">
                        {arquivoImportacao ? arquivoImportacao.name : "Clique para selecionar ou arraste um arquivo"}
                      </p>
                      <p className="text-xs text-muted-foreground">
                        Formatos aceitos: XLSX, XLS, CSV
                      </p>
                    </label>
                  </div>
                  <div className="flex justify-end">
                    <Button 
                      onClick={handleProcessarImportacao}
                      disabled={!arquivoImportacao || !rubricaImportacao || !cnpjUnidadeImportacao || !vigenciaImportacao}
                    >
                      Processar Importação
                    </Button>
                  </div>
                </div>
              </CardContent>
            </Card>

            {/* Histórico de Importações */}
            <Card>
              <CardHeader className="flex flex-row items-center justify-between">
                <div>
                  <CardTitle>Histórico de Importações</CardTitle>
                  <p className="text-sm text-muted-foreground mt-1">
                    Acompanhe o status das importações realizadas recentemente
                  </p>
                </div>
                <Button variant="ghost" size="icon">
                  <RefreshCw className="h-4 w-4" />
                </Button>
              </CardHeader>
              <CardContent>
                <div className="space-y-4">
                  {mockHistoricoImportacoes.map((importacao) => (
                    <Card key={importacao.id} className="p-4 hover:shadow-md transition-shadow">
                      <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
                        <div>
                          <p className="text-xs text-muted-foreground mb-1">STATUS</p>
                          <Badge className={cn("font-medium", getStatusColor(importacao.status))}>
                            {importacao.status}
                          </Badge>
                        </div>
                        <div>
                          <p className="text-xs text-muted-foreground mb-1">Competência</p>
                          <p className="font-medium">{importacao.competencia}</p>
                        </div>
                        <div>
                          <p className="text-xs text-muted-foreground mb-1">Registros</p>
                          <p className="font-medium">{importacao.registros}</p>
                        </div>
                        <div>
                          <p className="text-xs text-muted-foreground mb-1">Usuário</p>
                          <p className="font-medium">{importacao.nomeUsuario}</p>
                        </div>
                      </div>
                      <div className="mt-2 pt-2 border-t">
                        <p className="text-xs text-muted-foreground">{importacao.descricao}</p>
                      </div>
                    </Card>
                  ))}
                </div>
              </CardContent>
            </Card>
          </div>
        </TabsContent>
      </Tabs>

      {/* Modal Novo Lançamento */}
      <Dialog open={novoLancamentoOpen} onOpenChange={setNovoLancamentoOpen}>
        <DialogContent className="max-w-3xl max-h-[90vh] overflow-y-auto bg-white dark:bg-white">
          <DialogHeader>
            <div className="flex items-center gap-3">
              <div className="p-2 bg-primary/10 rounded-lg">
                <FileText className="h-6 w-6 text-primary" />
              </div>
              <DialogTitle className="text-2xl font-bold">Novo Lançamento</DialogTitle>
            </div>
          </DialogHeader>

          <div className="grid grid-cols-1 md:grid-cols-2 gap-4 py-4">
            {/* Colaborador */}
            <div className="space-y-2">
              <Label>
                Colaborador<span className="text-destructive">*</span>
              </Label>
              <Popover open={novoColaboradorOpen} onOpenChange={setNovoColaboradorOpen}>
                <PopoverTrigger asChild>
                  <Button
                    variant="outline"
                    role="combobox"
                    aria-expanded={novoColaboradorOpen}
                    className="w-full justify-between"
                  >
                    {novoColaborador
                      ? mockColaboradores.find((c) => c.id === novoColaborador)?.nome
                      : "Selecione o colaborador"}
                    <ChevronsUpDown className="ml-2 h-4 w-4 shrink-0 opacity-50" />
                  </Button>
                </PopoverTrigger>
                <PopoverContent className="w-full p-0">
                  <Command>
                    <CommandInput placeholder="Buscar colaborador..." />
                    <CommandList>
                      <CommandEmpty>Nenhum colaborador encontrado.</CommandEmpty>
                      <CommandGroup>
                        {mockColaboradores.map((colaborador) => (
                          <CommandItem
                            key={colaborador.id}
                            value={colaborador.id}
                            onSelect={(currentValue) => {
                              setNovoColaborador(currentValue === novoColaborador ? "" : currentValue);
                              setNovoColaboradorOpen(false);
                            }}
                          >
                            <Check
                              className={cn(
                                "mr-2 h-4 w-4",
                                novoColaborador === colaborador.id ? "opacity-100" : "opacity-0"
                              )}
                            />
                            {colaborador.nome}
                          </CommandItem>
                        ))}
                      </CommandGroup>
                    </CommandList>
                  </Command>
                </PopoverContent>
              </Popover>
            </div>

            {/* Recorrência */}
            <div className="space-y-2">
              <Label>
                Recorrência<span className="text-destructive">*</span>
              </Label>
              <Select value={recorrencia} onValueChange={setRecorrencia}>
                <SelectTrigger>
                  <SelectValue placeholder="Selecione a recorrência" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="mensal">Mensal</SelectItem>
                  <SelectItem value="eventual">Eventual</SelectItem>
                  <SelectItem value="unica">Única</SelectItem>
                </SelectContent>
              </Select>
            </div>

            {/* Rubrica */}
            <div className="space-y-2 md:col-span-2">
              <Label>
                Rubrica<span className="text-destructive">*</span>
              </Label>
              <Select value={novaRubrica} onValueChange={setNovaRubrica}>
                <SelectTrigger>
                  <SelectValue placeholder="Selecione a rubrica" />
                </SelectTrigger>
                <SelectContent>
                  {mockRubricas.map((r) => (
                    <SelectItem key={r.id} value={r.id}>
                      {r.nome}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>

            {/* Data de início */}
            <div className="space-y-2">
              <Label>
                Data de início<span className="text-destructive">*</span>
              </Label>
              <Popover>
                <PopoverTrigger asChild>
                  <Button
                    variant="outline"
                    className={cn(
                      "w-full justify-start text-left font-normal",
                      !dataInicio && "text-muted-foreground"
                    )}
                  >
                    <CalendarIcon className="mr-2 h-4 w-4" />
                    {dataInicio ? format(dataInicio, "MM/yyyy", { locale: ptBR }) : "mm/yyyy"}
                  </Button>
                </PopoverTrigger>
                <PopoverContent className="w-auto p-0" align="start">
                  <Calendar
                    mode="single"
                    selected={dataInicio}
                    onSelect={setDataInicio}
                    locale={ptBR}
                    className="pointer-events-auto"
                  />
                </PopoverContent>
              </Popover>
            </div>

            {/* Data final */}
            <div className="space-y-2">
              <Label>
                Data final<span className="text-destructive">*</span>
              </Label>
              <Popover>
                <PopoverTrigger asChild>
                  <Button
                    variant="outline"
                    className={cn(
                      "w-full justify-start text-left font-normal",
                      !dataFinal && "text-muted-foreground"
                    )}
                  >
                    <CalendarIcon className="mr-2 h-4 w-4" />
                    {dataFinal ? format(dataFinal, "MM/yyyy", { locale: ptBR }) : "mm/yyyy"}
                  </Button>
                </PopoverTrigger>
                <PopoverContent className="w-auto p-0" align="start">
                  <Calendar
                    mode="single"
                    selected={dataFinal}
                    onSelect={setDataFinal}
                    locale={ptBR}
                    className="pointer-events-auto"
                  />
                </PopoverContent>
              </Popover>
            </div>

            {/* Valor */}
            <div className="space-y-2 md:col-span-2">
              <Label>
                Valor<span className="text-destructive">*</span>
              </Label>
              <Input
                type="text"
                placeholder="R$ 0,00"
                value={valor}
                onChange={(e) => setValor(e.target.value)}
              />
            </div>

            {/* Observações */}
            <div className="space-y-2 md:col-span-2">
              <Label>Observações</Label>
              <Textarea
                placeholder="Obs"
                value={observacoes}
                onChange={(e) => setObservacoes(e.target.value)}
                rows={3}
              />
            </div>
          </div>

          <div className="flex justify-end gap-3 pt-4 border-t">
            <Button variant="outline" onClick={handleCancelarLancamento}>
              Cancelar
            </Button>
            <Button onClick={handleConfirmarLancamento}>
              Confirmar
            </Button>
          </div>
        </DialogContent>
      </Dialog>

      {/* Modal Nova Rubrica */}
      <Dialog open={novaRubricaOpen} onOpenChange={setNovaRubricaOpen}>
        <DialogContent className="max-w-2xl bg-white dark:bg-white">
          <DialogHeader>
            <div className="flex items-center gap-3">
              <div className="p-2 bg-primary/10 rounded-lg">
                <FileText className="h-6 w-6 text-primary" />
              </div>
              <DialogTitle className="text-2xl font-bold">Nova Rubrica</DialogTitle>
            </div>
          </DialogHeader>

          <div className="grid grid-cols-1 gap-4 py-4">
            {/* Código da Rubrica */}
            <div className="space-y-2">
              <Label>
                Código da Rubrica<span className="text-destructive">*</span>
              </Label>
              <Input
                type="text"
                placeholder="Ex: VT001"
                value={codigoRubrica}
                onChange={(e) => setCodigoRubrica(e.target.value)}
              />
            </div>

            {/* Nome */}
            <div className="space-y-2">
              <Label>
                Nome<span className="text-destructive">*</span>
              </Label>
              <Input
                type="text"
                placeholder="Ex: Vale Transporte"
                value={nomeRubrica}
                onChange={(e) => setNomeRubrica(e.target.value)}
              />
            </div>

            {/* Categoria */}
            <div className="space-y-2">
              <Label>
                Categoria<span className="text-destructive">*</span>
              </Label>
              <Select value={categoriaRubrica} onValueChange={setCategoriaRubrica}>
                <SelectTrigger>
                  <SelectValue placeholder="Selecione a categoria" />
                </SelectTrigger>
                <SelectContent>
                  {mockCategorias.map((cat) => (
                    <SelectItem key={cat.id} value={cat.id}>
                      {cat.nome}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>

            {/* Tipo de Cálculo */}
            <div className="space-y-2">
              <Label>
                Tipo de Cálculo<span className="text-destructive">*</span>
              </Label>
              <Select value={tipoCalculoRubrica} onValueChange={setTipoCalculoRubrica}>
                <SelectTrigger>
                  <SelectValue placeholder="Selecione o tipo" />
                </SelectTrigger>
                <SelectContent>
                  {mockTiposCalculo.map((tipo) => (
                    <SelectItem key={tipo.id} value={tipo.id}>
                      {tipo.nome}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>

            {/* Reflete na contabilidade */}
            <div className="flex items-center justify-between space-y-2">
              <Label htmlFor="reflete-contabilidade" className="cursor-pointer">
                Reflete na contabilidade?
              </Label>
              <Switch
                id="reflete-contabilidade"
                checked={refeteContabilidade}
                onCheckedChange={setRefeteContabilidade}
              />
            </div>
          </div>

          <div className="flex justify-end gap-3 pt-4 border-t">
            <Button variant="outline" onClick={handleCancelarRubrica}>
              Cancelar
            </Button>
            <Button onClick={handleConfirmarRubrica}>
              Confirmar
            </Button>
          </div>
        </DialogContent>
      </Dialog>
    </div>
  );
}
