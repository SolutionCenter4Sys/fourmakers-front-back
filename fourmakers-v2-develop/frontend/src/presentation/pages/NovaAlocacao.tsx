import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import * as z from "zod";
import { Card, CardContent } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { Checkbox } from "@/components/ui/checkbox";

import { toast } from "sonner";
import { Calendar } from "@/components/ui/calendar";
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "@/components/ui/popover";
import { CalendarIcon, Info, Check, ChevronsUpDown, ArrowLeft } from "@/components/ui/system-icons";
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from "@/components/ui/tooltip";
import { Command, CommandEmpty, CommandGroup, CommandInput, CommandItem, CommandList } from "@/components/ui/command";
import { format } from "date-fns";
import { ptBR } from "date-fns/locale";
import { cn } from "@/lib/utils";
import { useNavigate } from "react-router-dom";
import { useState, useEffect, useCallback, useMemo } from "react";
import { useAppSelector } from "@/app/store/hooks";
import { container } from "@/core/di/container";
import { ListarNomesGestoresUseCase } from "@domain/usecases/ListarNomesGestoresUseCase";
import { ListarDepartamentosUseCase } from "@domain/usecases/ListarDepartamentosUseCase";
import { ListarColaboradoresETbdsUseCase } from "@domain/usecases/ListarColaboradoresETbdsUseCase";
import { ListarProjetosColaboradorMapaUseCase } from "@domain/usecases/ListarProjetosColaboradorMapaUseCase";
import { CadastrarMapaAlocacaoUseCase } from "@domain/usecases/CadastrarMapaAlocacaoUseCase";
import { ListarPerfilAlocacaoUseCase } from "@domain/usecases/ListarPerfilAlocacaoUseCase";
import type { CadastrarMapaAlocacaoPayload, PerfilSkill, PerfilAlocacaoItem } from "@domain/entities/MapaAlocacao";
import type { Gestor } from "@domain/entities/MapaAlocacao";
import type { Departamento } from "@domain/entities/Departamento";
import type { ColaboradorETbd, ProjetoColaboradorMapa } from "@domain/entities/MapaAlocacao";

const formSchema = z.object({
  tipoFiltro: z.enum(["gestor", "departamento"]).optional(),
  gestor: z.string().optional(),
  departamento: z.string().optional(),
  colaborador: z.string({ required_error: "Colaborador é obrigatório" }),
  clienteProjeto: z.string({ required_error: "Cliente/Projeto é obrigatório" }),
  dataInicio: z.date({ required_error: "Data de início é obrigatória" }),
  dataFinal: z.date({ required_error: "Data final é obrigatória" }),
  horasPorDia: z.string().min(1, "Horas por dia é obrigatório").refine(
    (val) => {
      if (!val || val.trim() === "") return false;
      const normalized = val.replace(",", ".");
      const num = parseFloat(normalized);
      return !isNaN(num) && num > 0 && /^\d+(\.\d{1})?$/.test(normalized);
    },
    { message: "Deve ser um número positivo com até uma casa decimal (ex: 8.5)" }
  ),
  percentual: z.string().optional(),
  incluirSabDomingo: z.boolean().default(false),
  observacao: z.string().optional(),
  oportunidade: z.string().optional(),
  perfilAtuacao: z.string().optional(),
  habilidades: z.string().optional(),
  prioritaria: z.boolean().default(false),
  retroalimentarPerfil: z.boolean().default(false),
});

type FormData = z.infer<typeof formSchema>;

const NovaAlocacao = () => {
  const navigate = useNavigate();
  const { token } = useAppSelector((state) => state.auth);
  
  const [gestores, setGestores] = useState<Gestor[]>([]);
  const [departamentos, setDepartamentos] = useState<Departamento[]>([]);
  const [colaboradores, setColaboradores] = useState<ColaboradorETbd[]>([]);
  const [projetos, setProjetos] = useState<ProjetoColaboradorMapa[]>([]);
  const [loadingGestores, setLoadingGestores] = useState(false);
  const [loadingDepartamentos, setLoadingDepartamentos] = useState(false);
  const [loadingColaboradores, setLoadingColaboradores] = useState(false);
  const [loadingProjetos, setLoadingProjetos] = useState(false);
  const [perfisDisponiveis, setPerfisDisponiveis] = useState<PerfilAlocacaoItem[]>([]);
  const [loadingPerfis, setLoadingPerfis] = useState(false);
  const [perfilAtuacaoOpen, setPerfilAtuacaoOpen] = useState(false);
  const [perfilAtuacaoSearch, setPerfilAtuacaoSearch] = useState("");
  const [colaboradorOpen, setColaboradorOpen] = useState(false);
  const [colaboradorSearch, setColaboradorSearch] = useState("");
  const [clienteProjetoOpen, setClienteProjetoOpen] = useState(false);
  const [clienteProjetoSearch, setClienteProjetoSearch] = useState("");
  const [departamentoOpen, setDepartamentoOpen] = useState(false);
  const [departamentoSearch, setDepartamentoSearch] = useState("");
  const [gestorOpen, setGestorOpen] = useState(false);
  const [gestorSearch, setGestorSearch] = useState("");
  
  const form = useForm<FormData>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      tipoFiltro: undefined,
      gestor: undefined,
      departamento: undefined,
      colaborador: "",
      clienteProjeto: "",
      horasPorDia: "",
      percentual: "",
      incluirSabDomingo: false,
      observacao: "",
      oportunidade: "",
      perfilAtuacao: "",
      habilidades: "",
      prioritaria: false,
      retroalimentarPerfil: false,
    },
  });

  const tipoFiltro = form.watch("tipoFiltro");
  const gestorSelecionado = form.watch("gestor");
  const departamentoSelecionado = form.watch("departamento");
  const colaboradorSelecionado = form.watch("colaborador");
  const horasPorDia = form.watch("horasPorDia");
  const percentual = form.watch("percentual");

  // Carregar gestores
  const carregarGestores = useCallback(async () => {
    if (!token) return;
    setLoadingGestores(true);
    try {
      const useCase = container.resolve(ListarNomesGestoresUseCase);
      const response = await useCase.execute(token, {});
      // A API pode retornar response.Gestor ou response.retorno
      const gestoresData = (response as any).Gestor || response.retorno || [];
      // Remover duplicatas
      const seen = new Set<string>();
      const gestoresUnicos = (gestoresData as Gestor[]).filter((gestor) => {
        if (seen.has(gestor.codigoProfissional)) return false;
        seen.add(gestor.codigoProfissional);
        return true;
      });
      setGestores(gestoresUnicos);
    } catch (error) {
      console.error("Erro ao carregar gestores:", error);
      toast.error("Erro ao carregar gestores");
    } finally {
      setLoadingGestores(false);
    }
  }, [token]);

  // Carregar departamentos
  const carregarDepartamentos = useCallback(async () => {
    if (!token) return;
    setLoadingDepartamentos(true);
    try {
      const useCase = container.resolve(ListarDepartamentosUseCase);
      const response = await useCase.execute(token, { codigoDiretoria: "" });
      const departamentosData = response.retorno || [];
      // Remover duplicatas
      const seen = new Set<string>();
      const departamentosUnicos = departamentosData.filter((dept) => {
        if (seen.has(dept.cod)) return false;
        seen.add(dept.cod);
        return true;
      });
      setDepartamentos(departamentosUnicos);
    } catch (error) {
      console.error("Erro ao carregar departamentos:", error);
      toast.error("Erro ao carregar departamentos");
    } finally {
      setLoadingDepartamentos(false);
    }
  }, [token]);

  // Carregar colaboradores baseado nos filtros
  const carregarColaboradores = useCallback(async () => {
    if (!token) return;
    setLoadingColaboradores(true);
    try {
      const useCase = container.resolve(ListarColaboradoresETbdsUseCase);
      const response = await useCase.execute(token, {
        codigoDiretoria: 0,
        codigoGestor: gestorSelecionado ? Number(gestorSelecionado) : 0,
        filtroTipoProfissional: 0,
        codigoDepartamento: departamentoSelecionado || "",
      });
      const colaboradoresData = response.retorno || [];
      // Remover duplicatas
      const seen = new Set<string>();
      const colaboradoresUnicos = colaboradoresData.filter((colab) => {
        if (seen.has(colab.codProfissional)) return false;
        seen.add(colab.codProfissional);
        return true;
      });
      setColaboradores(colaboradoresUnicos);
    } catch (error) {
      console.error("Erro ao carregar colaboradores:", error);
      toast.error("Erro ao carregar colaboradores");
    } finally {
      setLoadingColaboradores(false);
    }
  }, [token, gestorSelecionado, departamentoSelecionado]);

  // Carregar projetos quando selecionar colaborador/TBD
  const carregarProjetos = useCallback(async (codigoProfissional: string, ehTbd: boolean) => {
    if (!token || !codigoProfissional) return;
    setLoadingProjetos(true);
    try {
      const useCase = container.resolve(ListarProjetosColaboradorMapaUseCase);
      const response = await useCase.execute(token, {
        codigoProfissional,
        ehTbd,
        codigoGerenteProjeto: "",
        listaCodigoCliente: [],
        status: "",
        prioritarioFiltro: 0,
      });
      const projetosData = response.Projetos || [];
      // Remover duplicatas
      const seen = new Set<string>();
      const projetosUnicos = projetosData.filter((projeto) => {
        if (seen.has(projeto.codigoProjeto)) return false;
        seen.add(projeto.codigoProjeto);
        return true;
      });
      setProjetos(projetosUnicos);
      // Limpar seleção de projeto se não houver projetos
      if (projetosUnicos.length === 0) {
        form.setValue("clienteProjeto", "");
      }
    } catch (error) {
      console.error("Erro ao carregar projetos:", error);
      toast.error("Erro ao carregar projetos");
      setProjetos([]);
    } finally {
      setLoadingProjetos(false);
    }
  }, [token, form]);

  // Carregar perfis de alocação
  const carregarPerfis = useCallback(async () => {
    if (!token) return;

    try {
      setLoadingPerfis(true);
      const useCase = container.resolve(ListarPerfilAlocacaoUseCase);
      const response = await useCase.execute(token, {
        codProjeto: "",
        ocultarSkill: true,
      });

      if (response.retorno) {
        setPerfisDisponiveis(response.retorno);
      } else {
        setPerfisDisponiveis([]);
      }
    } catch (error) {
      console.error("Erro ao carregar perfis:", error);
      toast.error("Erro ao carregar perfis disponíveis");
      setPerfisDisponiveis([]);
    } finally {
      setLoadingPerfis(false);
    }
  }, [token]);

  // Filtrar perfis baseado na busca
  const perfisFiltrados = useMemo(() => {
    if (!perfilAtuacaoSearch.trim()) {
      return perfisDisponiveis;
    }
    const searchLower = perfilAtuacaoSearch.toLowerCase();
    return perfisDisponiveis.filter(perfil => 
      perfil.perfil && perfil.perfil.toLowerCase().includes(searchLower)
    );
  }, [perfisDisponiveis, perfilAtuacaoSearch]);

  // Filtrar colaboradores baseado na busca
  const colaboradoresFiltrados = useMemo(() => {
    if (!colaboradorSearch.trim()) {
      return colaboradores;
    }
    const searchLower = colaboradorSearch.toLowerCase();
    return colaboradores.filter(colab => 
      (colab.labelCodigoNome && colab.labelCodigoNome.toLowerCase().includes(searchLower)) ||
      (colab.nomeProfissional && colab.nomeProfissional.toLowerCase().includes(searchLower))
    );
  }, [colaboradores, colaboradorSearch]);

  // Filtrar projetos baseado na busca
  const projetosFiltrados = useMemo(() => {
    if (!clienteProjetoSearch.trim()) {
      return projetos;
    }
    const searchLower = clienteProjetoSearch.toLowerCase();
    return projetos.filter(projeto => 
      projeto.projetos && projeto.projetos.toLowerCase().includes(searchLower)
    );
  }, [projetos, clienteProjetoSearch]);

  // Filtrar departamentos baseado na busca
  const departamentosFiltrados = useMemo(() => {
    if (!departamentoSearch.trim()) {
      return departamentos;
    }
    const searchLower = departamentoSearch.toLowerCase();
    return departamentos.filter(dept => 
      dept.departamento && dept.departamento.toLowerCase().includes(searchLower)
    );
  }, [departamentos, departamentoSearch]);

  // Filtrar gestores baseado na busca
  const gestoresFiltrados = useMemo(() => {
    if (!gestorSearch.trim()) {
      return gestores;
    }
    const searchLower = gestorSearch.toLowerCase();
    return gestores.filter(gestor => 
      gestor.nome && gestor.nome.toLowerCase().includes(searchLower)
    );
  }, [gestores, gestorSearch]);

  // Carregar dados iniciais
  useEffect(() => {
    if (token) {
      carregarGestores();
      carregarDepartamentos();
      carregarPerfis();
    }
  }, [token, carregarGestores, carregarDepartamentos, carregarPerfis]);

  // Recarregar colaboradores quando filtros mudarem
  useEffect(() => {
    if (token && (gestorSelecionado || departamentoSelecionado || tipoFiltro)) {
      carregarColaboradores();
    } else if (!gestorSelecionado && !departamentoSelecionado) {
      setColaboradores([]);
    }
  }, [token, gestorSelecionado, departamentoSelecionado, tipoFiltro, carregarColaboradores]);

  // Carregar projetos quando selecionar colaborador
  useEffect(() => {
    if (colaboradorSelecionado) {
      // Aguardar colaboradores serem carregados
      if (colaboradores.length > 0) {
        const colaborador = colaboradores.find(c => c.codProfissional === colaboradorSelecionado);
        if (colaborador) {
          carregarProjetos(colaborador.codProfissional, colaborador.ehTbd);
        }
      }
    } else {
      setProjetos([]);
      form.setValue("clienteProjeto", "");
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [colaboradorSelecionado, colaboradores]);

  // Flag para evitar loops infinitos
  const [calculandoPercentual, setCalculandoPercentual] = useState(false);
  const [calculandoHoras, setCalculandoHoras] = useState(false);

  // Calcular percentual quando horasPorDia mudar
  useEffect(() => {
    if (calculandoHoras || !horasPorDia || horasPorDia.trim() === "") return;
    
    const horas = parseFloat(horasPorDia.replace(",", "."));
    if (!isNaN(horas) && horas > 0) {
      setCalculandoPercentual(true);
      // 8 horas = 100%, então: percentual = (horas / 8) * 100
      const novoPercentual = ((horas / 8) * 100).toFixed(2);
      form.setValue("percentual", novoPercentual.replace(".", ","));
      setTimeout(() => setCalculandoPercentual(false), 100);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [horasPorDia]);

  // Calcular horasPorDia quando percentual mudar
  useEffect(() => {
    if (calculandoPercentual || !percentual || percentual.trim() === "") return;
    
    const percent = parseFloat(percentual.replace(",", "."));
    if (!isNaN(percent) && percent > 0) {
      setCalculandoHoras(true);
      // 100% = 8 horas, então: horas = (percentual / 100) * 8
      const novasHoras = ((percent / 100) * 8).toFixed(1);
      form.setValue("horasPorDia", novasHoras.replace(".", ","));
      setTimeout(() => setCalculandoHoras(false), 100);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [percentual]);

  const onSubmit = async (data: FormData) => {
    if (!token) {
      toast.error("Token de autenticação não encontrado");
      return;
    }

    try {
      // Buscar dados do colaborador selecionado
      const colaborador = colaboradores.find(c => c.codProfissional === data.colaborador);
      if (!colaborador) {
        toast.error("Colaborador não encontrado");
        return;
      }

      // Buscar dados do projeto selecionado
      const projeto = projetos.find(p => p.codigoProjeto === data.clienteProjeto);
      if (!projeto) {
        toast.error("Projeto não encontrado");
        return;
      }

      // Buscar dados do gestor se selecionado
      let nomeGestor = "";
      let codigoGestor = "";
      if (data.gestor) {
        const gestor = gestores.find(g => g.codigoProfissional === data.gestor);
        if (gestor) {
          nomeGestor = gestor.nome;
          codigoGestor = gestor.codigoProfissional;
        }
      }

      // Formatar datas - validar se são datas válidas
      if (!data.dataInicio || !(data.dataInicio instanceof Date) || isNaN(data.dataInicio.getTime())) {
        toast.error("Data de início inválida");
        return;
      }
      const dataInicio = format(data.dataInicio, "yyyy-MM-dd");
      
      // Data final é obrigatória
      if (!data.dataFinal || !(data.dataFinal instanceof Date) || isNaN(data.dataFinal.getTime())) {
        toast.error("Data final é obrigatória e deve ser uma data válida");
        return;
      }
      const dataFim = format(data.dataFinal, "yyyy-MM-dd");

      // Converter horas por dia para número
      const quantidadeHoras = parseFloat(data.horasPorDia.replace(",", ".")) || 0;

      // Converter percentual para número
      const percentual = parseFloat(data.percentual?.replace(",", ".") || "0") || 0;

      // PerfilSkills vazio (funcionalidade de habilidades removida)
      const perfilSkills: PerfilSkill[] = [];

      // Buscar nome do perfil selecionado
      const perfilSelecionado = perfisDisponiveis.find(p => p.id === data.perfilAtuacao);
      const nomePerfilAlocacao = perfilSelecionado?.perfil || "";

      // Montar payload
      const payload: CadastrarMapaAlocacaoPayload = {
        cpfColaborador: colaborador.cpf || colaborador.codProfissional || "",
        codigoTbd: colaborador.ehTbd ? colaborador.codProfissional : "0",
        codigoColaborador: colaborador.ehTbd ? "0" : colaborador.codProfissional,
        codigoGestor: codigoGestor,
        codigoProjeto: projeto.codigoProjeto,
        nomeGestor: nomeGestor,
        nomeProjeto: projeto.projetos || "",
        dataInicio: dataInicio,
        dataFim: dataFim,
        incluiFimDeSemana: data.incluirSabDomingo,
        quantidadeHoras: quantidadeHoras,
        oportunidade: data.oportunidade || "",
        observacao: data.observacao || "",
        prioritario: data.prioritaria ? 1 : 0,
        percentual: percentual,
        flagRetroalimentaCV: data.retroalimentarPerfil,
        idPerfilAlocacao: data.perfilAtuacao || "",
        nomePerfilAlocacao: nomePerfilAlocacao,
        perfilSkills: perfilSkills,
      };

      // Chamar use case
      const useCase = container.resolve(CadastrarMapaAlocacaoUseCase);
      const response = await useCase.execute(token, payload);

      if (response.sucesso) {
        toast.success("Alocação criada com sucesso!");
        navigate("/mapa-alocacao");
      } else {
        const mensagemErro = response.mensagem || "Erro ao criar alocação";
        toast.error(mensagemErro);
      }
    } catch (error) {
      console.error("Erro ao criar alocação:", error);
      const mensagemErro = error instanceof Error ? error.message : "Erro ao criar alocação";
      toast.error(mensagemErro);
    }
  };

  const handleLimpar = () => {
    form.reset();
    setColaboradores([]);
    setProjetos([]);
  };

  return (
    <div className="container max-w-5xl mx-auto py-8 px-4">
      <div className="mb-8 flex items-center gap-4">
        <Button
          type="button"
          variant="ghost"
          size="icon"
          onClick={() => navigate(-1)}
          className="h-9 w-9"
        >
          <ArrowLeft className="h-5 w-5" />
        </Button>
        <h1 className="page-title mb-2">Nova Alocação</h1>
      </div>

      <Form {...form}>
        <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-6">
          {/* Seção: Informações Básicas */}
          <Card>
            <CardContent className="p-6 space-y-6">
              <h3 className="text-lg font-semibold border-b pb-2">Informações Básicas</h3>
              
              {/* Filtro inicial: Gestor ou Departamento */}
              <FormField
                control={form.control}
                name="tipoFiltro"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel className="flex items-center gap-2">
                      Filtrar por
                    </FormLabel>
                    <Select 
                      onValueChange={(value) => {
                        field.onChange(value);
                        // Limpar seleções quando mudar o tipo de filtro
                        form.setValue("gestor", "");
                        form.setValue("departamento", "");
                        form.setValue("colaborador", "");
                        form.setValue("clienteProjeto", "");
                        setColaboradores([]);
                        setProjetos([]);
                      }} 
                      value={field.value}
                    >
                      <FormControl>
                        <SelectTrigger>
                          <SelectValue placeholder="Selecione o tipo de filtro" />
                        </SelectTrigger>
                      </FormControl>
                      <SelectContent>
                        <SelectItem value="gestor">Gestor</SelectItem>
                        <SelectItem value="departamento">Departamento</SelectItem>
                      </SelectContent>
                    </Select>
                    <FormMessage />
                  </FormItem>
                )}
              />

              {/* Gestor ou Departamento baseado no filtro selecionado */}
              {tipoFiltro === "gestor" && (
                <FormField
                  control={form.control}
                  name="gestor"
                  render={({ field }) => {
                    const gestorSelecionadoObj = gestores.find(g => g.codigoProfissional === field.value);
                    return (
                      <FormItem>
                        <FormLabel>
                          Gestor<span className="text-destructive">*</span>
                        </FormLabel>
                        <Popover open={gestorOpen} onOpenChange={setGestorOpen}>
                          <PopoverTrigger asChild>
                            <FormControl>
                              <Button
                                variant="outline"
                                role="combobox"
                                className={cn(
                                  "w-full justify-between",
                                  !field.value && "text-muted-foreground"
                                )}
                                disabled={loadingGestores}
                              >
                                {gestorSelecionadoObj?.nome || (loadingGestores ? "Carregando..." : "Selecione o Gestor")}
                                <ChevronsUpDown className="ml-2 h-4 w-4 shrink-0 opacity-50" />
                              </Button>
                            </FormControl>
                          </PopoverTrigger>
                          <PopoverContent className="w-[var(--radix-popover-trigger-width)] p-0" align="start">
                            <Command shouldFilter={false}>
                              <CommandInput
                                placeholder="Buscar gestor..."
                                value={gestorSearch}
                                onValueChange={setGestorSearch}
                              />
                              <CommandList>
                                <CommandEmpty>
                                  {loadingGestores 
                                    ? "Carregando..." 
                                    : gestorSearch.trim()
                                      ? "Nenhum gestor encontrado"
                                      : "Nenhum gestor disponível"}
                                </CommandEmpty>
                                <CommandGroup>
                                  {gestoresFiltrados.map((gestor) => (
                                    <CommandItem
                                      key={`gestor-${gestor.codigoProfissional}`}
                                      value={gestor.codigoProfissional}
                                      onSelect={() => {
                                        field.onChange(gestor.codigoProfissional);
                                        form.setValue("colaborador", "");
                                        form.setValue("clienteProjeto", "");
                                        setColaboradores([]);
                                        setProjetos([]);
                                        setGestorOpen(false);
                                        setGestorSearch("");
                                      }}
                                    >
                                      <Check
                                        className={cn(
                                          "mr-2 h-4 w-4",
                                          field.value === gestor.codigoProfissional ? "opacity-100" : "opacity-0"
                                        )}
                                      />
                                      {gestor.nome}
                                    </CommandItem>
                                  ))}
                                </CommandGroup>
                              </CommandList>
                            </Command>
                          </PopoverContent>
                        </Popover>
                        <FormMessage />
                      </FormItem>
                    );
                  }}
                />
              )}

              {tipoFiltro === "departamento" && (
                <FormField
                  control={form.control}
                  name="departamento"
                  render={({ field }) => {
                    const departamentoSelecionadoObj = departamentos.find(d => d.cod === field.value);
                    return (
                      <FormItem>
                        <FormLabel>
                          Departamento<span className="text-destructive">*</span>
                        </FormLabel>
                        <Popover open={departamentoOpen} onOpenChange={setDepartamentoOpen}>
                          <PopoverTrigger asChild>
                            <FormControl>
                              <Button
                                variant="outline"
                                role="combobox"
                                className={cn(
                                  "w-full justify-between",
                                  !field.value && "text-muted-foreground"
                                )}
                                disabled={loadingDepartamentos}
                              >
                                {departamentoSelecionadoObj?.departamento || (loadingDepartamentos ? "Carregando..." : "Selecione o Departamento")}
                                <ChevronsUpDown className="ml-2 h-4 w-4 shrink-0 opacity-50" />
                              </Button>
                            </FormControl>
                          </PopoverTrigger>
                          <PopoverContent className="w-[var(--radix-popover-trigger-width)] p-0" align="start">
                            <Command shouldFilter={false}>
                              <CommandInput
                                placeholder="Buscar departamento..."
                                value={departamentoSearch}
                                onValueChange={setDepartamentoSearch}
                              />
                              <CommandList>
                                <CommandEmpty>
                                  {loadingDepartamentos 
                                    ? "Carregando..." 
                                    : departamentoSearch.trim()
                                      ? "Nenhum departamento encontrado"
                                      : "Nenhum departamento disponível"}
                                </CommandEmpty>
                                <CommandGroup>
                                  {departamentosFiltrados.map((dept) => (
                                    <CommandItem
                                      key={`dept-${dept.cod}`}
                                      value={dept.cod}
                                      onSelect={() => {
                                        field.onChange(dept.cod);
                                        form.setValue("colaborador", "");
                                        form.setValue("clienteProjeto", "");
                                        setColaboradores([]);
                                        setProjetos([]);
                                        setDepartamentoOpen(false);
                                        setDepartamentoSearch("");
                                      }}
                                    >
                                      <Check
                                        className={cn(
                                          "mr-2 h-4 w-4",
                                          field.value === dept.cod ? "opacity-100" : "opacity-0"
                                        )}
                                      />
                                      {dept.departamento}
                                    </CommandItem>
                                  ))}
                                </CommandGroup>
                              </CommandList>
                            </Command>
                          </PopoverContent>
                        </Popover>
                        <FormMessage />
                      </FormItem>
                    );
                  }}
                />
              )}

              {/* Colaborador e Cliente/Projeto */}
              <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                <FormField
                  control={form.control}
                  name="colaborador"
                  render={({ field }) => {
                    const colaboradorSelecionadoObj = colaboradores.find(c => c.codProfissional === field.value);
                    return (
                      <FormItem>
                        <FormLabel>
                          Colaborador(a)/TBD<span className="text-destructive">*</span>
                        </FormLabel>
                        <Popover open={colaboradorOpen} onOpenChange={setColaboradorOpen}>
                          <PopoverTrigger asChild>
                            <FormControl>
                              <Button
                                variant="outline"
                                role="combobox"
                                className={cn(
                                  "w-full justify-between",
                                  !field.value && "text-muted-foreground"
                                )}
                                disabled={!tipoFiltro || (!gestorSelecionado && !departamentoSelecionado) || loadingColaboradores}
                              >
                                {colaboradorSelecionadoObj?.labelCodigoNome || (
                                  !tipoFiltro 
                                    ? "Selecione primeiro um filtro" 
                                    : loadingColaboradores 
                                      ? "Carregando..." 
                                      : colaboradores.length === 0
                                        ? "Nenhum colaborador encontrado"
                                        : "Selecione o Colaborador(a)/TBD"
                                )}
                                <ChevronsUpDown className="ml-2 h-4 w-4 shrink-0 opacity-50" />
                              </Button>
                            </FormControl>
                          </PopoverTrigger>
                          <PopoverContent className="w-[var(--radix-popover-trigger-width)] p-0" align="start">
                            <Command shouldFilter={false}>
                              <CommandInput
                                placeholder="Buscar colaborador..."
                                value={colaboradorSearch}
                                onValueChange={setColaboradorSearch}
                              />
                              <CommandList>
                                <CommandEmpty>
                                  {loadingColaboradores 
                                    ? "Carregando..." 
                                    : colaboradorSearch.trim()
                                      ? "Nenhum colaborador encontrado"
                                      : colaboradores.length === 0
                                        ? "Nenhum colaborador disponível"
                                        : "Digite para buscar"}
                                </CommandEmpty>
                                <CommandGroup>
                                  {colaboradoresFiltrados.map((colab) => (
                                    <CommandItem
                                      key={`colab-${colab.codProfissional}`}
                                      value={colab.codProfissional}
                                      onSelect={() => {
                                        field.onChange(colab.codProfissional);
                                        setColaboradorOpen(false);
                                        setColaboradorSearch("");
                                      }}
                                    >
                                      <Check
                                        className={cn(
                                          "mr-2 h-4 w-4",
                                          field.value === colab.codProfissional ? "opacity-100" : "opacity-0"
                                        )}
                                      />
                                      {colab.labelCodigoNome}
                                    </CommandItem>
                                  ))}
                                </CommandGroup>
                              </CommandList>
                            </Command>
                          </PopoverContent>
                        </Popover>
                        <FormMessage />
                      </FormItem>
                    );
                  }}
                />

                <FormField
                  control={form.control}
                  name="clienteProjeto"
                  render={({ field }) => {
                    const projetoSelecionadoObj = projetos.find(p => p.codigoProjeto === field.value);
                    return (
                      <FormItem>
                        <FormLabel>
                          Cliente/Projeto<span className="text-destructive">*</span>
                        </FormLabel>
                        <Popover open={clienteProjetoOpen} onOpenChange={setClienteProjetoOpen}>
                          <PopoverTrigger asChild>
                            <FormControl>
                              <Button
                                variant="outline"
                                role="combobox"
                                className={cn(
                                  "w-full justify-between",
                                  !field.value && "text-muted-foreground"
                                )}
                                disabled={!colaboradorSelecionado || loadingProjetos}
                              >
                                {projetoSelecionadoObj?.projetos || (
                                  !colaboradorSelecionado
                                    ? "Selecione primeiro um colaborador"
                                    : loadingProjetos
                                      ? "Carregando projetos..."
                                      : projetos.length === 0
                                        ? "Nenhum projeto encontrado"
                                        : "Selecione o projeto"
                                )}
                                <ChevronsUpDown className="ml-2 h-4 w-4 shrink-0 opacity-50" />
                              </Button>
                            </FormControl>
                          </PopoverTrigger>
                          <PopoverContent className="w-[var(--radix-popover-trigger-width)] p-0" align="start">
                            <Command shouldFilter={false}>
                              <CommandInput
                                placeholder="Buscar projeto..."
                                value={clienteProjetoSearch}
                                onValueChange={setClienteProjetoSearch}
                              />
                              <CommandList>
                                <CommandEmpty>
                                  {loadingProjetos 
                                    ? "Carregando..." 
                                    : clienteProjetoSearch.trim()
                                      ? "Nenhum projeto encontrado"
                                      : projetos.length === 0
                                        ? "Nenhum projeto disponível"
                                        : "Digite para buscar"}
                                </CommandEmpty>
                                <CommandGroup>
                                  {projetosFiltrados.map((projeto) => (
                                    <CommandItem
                                      key={`projeto-${projeto.codigoProjeto}`}
                                      value={projeto.codigoProjeto}
                                      onSelect={() => {
                                        field.onChange(projeto.codigoProjeto);
                                        setClienteProjetoOpen(false);
                                        setClienteProjetoSearch("");
                                      }}
                                    >
                                      <Check
                                        className={cn(
                                          "mr-2 h-4 w-4",
                                          field.value === projeto.codigoProjeto ? "opacity-100" : "opacity-0"
                                        )}
                                      />
                                      {projeto.projetos}
                                    </CommandItem>
                                  ))}
                                </CommandGroup>
                              </CommandList>
                            </Command>
                          </PopoverContent>
                        </Popover>
                        <FormMessage />
                      </FormItem>
                    );
                  }}
                />
              </div>

              {/* Datas e Oportunidade */}
              <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
                <FormField
                  control={form.control}
                  name="dataInicio"
                  render={({ field }) => (
                    <FormItem className="flex flex-col">
                      <FormLabel>
                        Data de início<span className="text-destructive">*</span>
                      </FormLabel>
                      <Popover>
                        <PopoverTrigger asChild>
                          <FormControl>
                            <Button
                              variant="outline"
                              className={cn(
                                "pl-3 text-left font-normal",
                                !field.value && "text-muted-foreground"
                              )}
                            >
                              {field.value ? (
                                format(field.value, "dd/MM/yyyy")
                              ) : (
                                <span>dd/mm/yyyy</span>
                              )}
                              <CalendarIcon className="ml-auto h-4 w-4 opacity-50" />
                            </Button>
                          </FormControl>
                        </PopoverTrigger>
                        <PopoverContent className="w-auto p-0" align="start">
                          <Calendar
                            mode="single"
                            selected={field.value}
                            onSelect={field.onChange}
                            locale={ptBR}
                            initialFocus
                            className="pointer-events-auto"
                          />
                        </PopoverContent>
                      </Popover>
                      <FormMessage />
                    </FormItem>
                  )}
                />

                <FormField
                  control={form.control}
                  name="dataFinal"
                  render={({ field }) => (
                    <FormItem className="flex flex-col">
                      <FormLabel>
                        Data final<span className="text-destructive">*</span>
                      </FormLabel>
                      <Popover>
                        <PopoverTrigger asChild>
                          <FormControl>
                            <Button
                              variant="outline"
                              className={cn(
                                "pl-3 text-left font-normal",
                                !field.value && "text-muted-foreground"
                              )}
                            >
                              {field.value ? (
                                format(field.value, "dd/MM/yyyy")
                              ) : (
                                <span>dd/mm/yyyy</span>
                              )}
                              <CalendarIcon className="ml-auto h-4 w-4 opacity-50" />
                            </Button>
                          </FormControl>
                        </PopoverTrigger>
                        <PopoverContent className="w-auto p-0" align="start">
                          <Calendar
                            mode="single"
                            selected={field.value}
                            onSelect={field.onChange}
                            locale={ptBR}
                            initialFocus
                            className="pointer-events-auto"
                          />
                        </PopoverContent>
                      </Popover>
                      <FormMessage />
                    </FormItem>
                  )}
                />

                <FormField
                  control={form.control}
                  name="oportunidade"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Oportunidade</FormLabel>
                      <FormControl>
                        <Input placeholder="Cód. Oportunidade" {...field} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />
              </div>

              {/* Horas, Percentual e Checkbox em linha */}
              <div className="grid grid-cols-1 md:grid-cols-[1fr_1fr_auto] gap-6 items-end">
                <FormField
                  control={form.control}
                  name="horasPorDia"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel className="flex items-center gap-2">
                        Horas por dia<span className="text-destructive">*</span>
                        <Info className="h-4 w-4 text-muted-foreground" />
                      </FormLabel>
                      <FormControl>
                        <Input
                          type="text"
                          inputMode="decimal"
                          placeholder="Ex: 8 ou 8.5"
                          {...field}
                          onChange={(e) => {
                            let value = e.target.value;
                            // Remove tudo exceto números e ponto/vírgula
                            value = value.replace(/[^\d.,]/g, "");
                            // Substitui vírgula por ponto
                            value = value.replace(",", ".");
                            // Permite apenas um ponto decimal
                            const parts = value.split(".");
                            if (parts.length > 2) {
                              value = parts[0] + "." + parts.slice(1).join("");
                            }
                            // Limita a uma casa decimal
                            if (parts.length === 2 && parts[1].length > 1) {
                              value = parts[0] + "." + parts[1].slice(0, 1);
                            }
                            field.onChange(value);
                          }}
                        />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />

                <FormField
                  control={form.control}
                  name="percentual"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Percentual</FormLabel>
                      <FormControl>
                        <Input
                          type="text"
                          inputMode="decimal"
                          placeholder="0"
                          {...field}
                          onChange={(e) => {
                            let value = e.target.value;
                            // Remove tudo exceto números e ponto/vírgula
                            value = value.replace(/[^\d.,]/g, "");
                            // Substitui vírgula por ponto
                            value = value.replace(",", ".");
                            // Permite apenas um ponto decimal
                            const parts = value.split(".");
                            if (parts.length > 2) {
                              value = parts[0] + "." + parts.slice(1).join("");
                            }
                            // Limita a duas casas decimais
                            if (parts.length === 2 && parts[1].length > 2) {
                              value = parts[0] + "." + parts[1].slice(0, 2);
                            }
                            field.onChange(value);
                          }}
                        />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />

                <FormField
                  control={form.control}
                  name="incluirSabDomingo"
                  render={({ field }) => (
                    <FormItem className="flex flex-row items-center space-x-3 space-y-0 pb-1">
                      <FormControl>
                        <Checkbox
                          checked={field.value}
                          onCheckedChange={field.onChange}
                        />
                      </FormControl>
                      <FormLabel className="!mt-0 cursor-pointer">
                        Incluir Sáb e Domingo
                      </FormLabel>
                    </FormItem>
                  )}
                />
              </div>

              {/* Observação em linha separada */}
              <FormField
                control={form.control}
                name="observacao"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Observação</FormLabel>
                    <FormControl>
                      <Textarea
                        placeholder="Deixe uma observação"
                        className="resize-none min-h-[100px]"
                        {...field}
                      />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </CardContent>
          </Card>

          {/* Realizações */}
          <Card>
            <CardContent className="p-6 space-y-6">
              <div className="flex items-center gap-2 border-b pb-2">
                <h3 className="text-lg font-semibold">Realizações</h3>
                <div className="h-6 w-6 rounded-full bg-green-500 flex items-center justify-center">
                  <span className="text-white text-xs">✓</span>
                </div>
              </div>

              <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                <FormField
                  control={form.control}
                  name="perfilAtuacao"
                  render={({ field }) => {
                    const perfilSelecionado = perfisDisponiveis.find(p => p.id === field.value);
                    return (
                      <FormItem>
                        <FormLabel>Perfil de Atuação</FormLabel>
                        <Popover open={perfilAtuacaoOpen} onOpenChange={setPerfilAtuacaoOpen}>
                          <PopoverTrigger asChild>
                            <FormControl>
                              <Button
                                variant="outline"
                                role="combobox"
                                className={cn(
                                  "w-full justify-between",
                                  !field.value && "text-muted-foreground"
                                )}
                                disabled={loadingPerfis}
                              >
                                {perfilSelecionado?.perfil || (loadingPerfis ? "Carregando..." : "Selecione o perfil de atuação")}
                                <ChevronsUpDown className="ml-2 h-4 w-4 shrink-0 opacity-50" />
                              </Button>
                            </FormControl>
                          </PopoverTrigger>
                          <PopoverContent className="w-[var(--radix-popover-trigger-width)] p-0" align="start">
                            <Command shouldFilter={false}>
                              <CommandInput
                                placeholder="Buscar perfil..."
                                value={perfilAtuacaoSearch}
                                onValueChange={setPerfilAtuacaoSearch}
                              />
                              <CommandList>
                                <CommandEmpty>
                                  {loadingPerfis 
                                    ? "Carregando..." 
                                    : perfilAtuacaoSearch.trim()
                                      ? "Nenhum perfil encontrado"
                                      : "Nenhum perfil disponível"}
                                </CommandEmpty>
                                <CommandGroup>
                                  {perfisFiltrados.map((perfil) => (
                                    <CommandItem
                                      key={perfil.id}
                                      value={perfil.id}
                                      onSelect={() => {
                                        field.onChange(perfil.id);
                                        setPerfilAtuacaoOpen(false);
                                        setPerfilAtuacaoSearch("");
                                      }}
                                    >
                                      <Check
                                        className={cn(
                                          "mr-2 h-4 w-4",
                                          field.value === perfil.id ? "opacity-100" : "opacity-0"
                                        )}
                                      />
                                      {perfil.perfil}
                                    </CommandItem>
                                  ))}
                                </CommandGroup>
                              </CommandList>
                            </Command>
                          </PopoverContent>
                        </Popover>
                        <FormMessage />
                      </FormItem>
                    );
                  }}
                />

                {/* Campo Habilidades oculto - removido conforme solicitação */}
              </div>

              <div className="flex items-start justify-between gap-4">
                <FormField
                  control={form.control}
                  name="prioritaria"
                  render={({ field }) => (
                    <FormItem className="flex flex-row items-center space-x-3 space-y-0">
                      <FormControl>
                        <Checkbox
                          checked={field.value}
                          onCheckedChange={field.onChange}
                        />
                      </FormControl>
                      <div className="flex items-center gap-2">
                        <FormLabel className="!mt-0 cursor-pointer">
                          Marcar como prioritária
                        </FormLabel>
                        <TooltipProvider>
                          <Tooltip>
                            <TooltipTrigger asChild>
                              <Info className="h-4 w-4 text-muted-foreground cursor-help" />
                            </TooltipTrigger>
                            <TooltipContent className="max-w-xs">
                              <p>Este campo é opcional e serve para organização e filtros em relatórios, indicando se esta alocação tem prioridade</p>
                            </TooltipContent>
                          </Tooltip>
                        </TooltipProvider>
                      </div>
                    </FormItem>
                  )}
                />
                <FormField
                  control={form.control}
                  name="retroalimentarPerfil"
                  render={({ field }) => (
                    <FormItem className="flex flex-col items-end space-y-2">
                      <div className="flex flex-row items-center space-x-3 space-y-0">
                        <FormControl>
                          <Checkbox
                            checked={field.value}
                            onCheckedChange={field.onChange}
                          />
                        </FormControl>
                        <FormLabel className="!mt-0 cursor-pointer">
                          Retroalimentar perfil
                        </FormLabel>
                      </div>
                      <p className="text-xs text-muted-foreground text-right max-w-[200px]">
                        Ao marcar, o perfil do colaborador será retroalimentado com essas realizações
                      </p>
                    </FormItem>
                  )}
                />
              </div>
            </CardContent>
          </Card>

          {/* Botões de Ação */}
          <div className="flex justify-end gap-4">
            <Button
              type="button"
              variant="outline"
              onClick={handleLimpar}
              className="min-w-32"
            >
              Limpar
            </Button>
            <Button
              type="submit"
              className="min-w-32"
            >
              Confirmar
            </Button>
          </div>
        </form>
      </Form>
    </div>
  );
};

export default NovaAlocacao;








