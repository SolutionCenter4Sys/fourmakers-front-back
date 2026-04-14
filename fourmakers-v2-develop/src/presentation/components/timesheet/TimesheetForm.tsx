import { useState, useEffect } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import { Checkbox } from "@/components/ui/checkbox";
import { Card, CardContent } from "@/components/ui/card";
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/popover";
import {
  Command,
  CommandEmpty,
  CommandGroup,
  CommandInput,
  CommandItem,
  CommandList,
} from "@/components/ui/command";
import { Check, ChevronsUpDown } from "@/components/ui/system-icons";
import { cn } from "@/lib/utils";
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from "@/components/ui/alert-dialog";
import { useAppSelector } from "@app/store/hooks";
import { useToast } from "@/hooks/use-toast";
import { useParametros } from "@presentation/hooks/useParametros";
import { TimesheetComponentesApi, type ProjetoComAtividades, type Atividade } from "@data/api/TimesheetComponentesApi";

interface TimesheetFormProps {
  onEnvioSucesso?: () => void;
  cpfColaborador?: string;
}

export const TimesheetForm = ({ onEnvioSucesso, cpfColaborador }: TimesheetFormProps) => {
  const { token } = useAppSelector((state) => state.auth);
  const { toast } = useToast();
  const { isEnabled } = useParametros();
  const obrigaResumoAtividade = isEnabled("OBRIGA_RESUMOATIVIDADE_TIMESHEET");
  const [includeSaturday, setIncludeSaturday] = useState(false);
  const [includeSunday, setIncludeSunday] = useState(false);
  const [includeHoliday, setIncludeHoliday] = useState(false);
  const [projetos, setProjetos] = useState<ProjetoComAtividades[]>([]);
  const [selectedProjeto, setSelectedProjeto] = useState<string>("");
  const [atividades, setAtividades] = useState<Atividade[]>([]);
  const [selectedAtividade, setSelectedAtividade] = useState<string>("");
  const [loadingProjetos, setLoadingProjetos] = useState(true);
  const [projetoOpen, setProjetoOpen] = useState(false);
  const [atividadeOpen, setAtividadeOpen] = useState(false);
  const [showConfirmDialog, setShowConfirmDialog] = useState(false);
  const [showErrorDialog, setShowErrorDialog] = useState(false);
  const [errorMessage, setErrorMessage] = useState<string>("");
  const [enviando, setEnviando] = useState(false);
  
  const formatarDataAtual = (): string => {
    const hoje = new Date();
    const ano = hoje.getFullYear();
    const mes = String(hoje.getMonth() + 1).padStart(2, '0');
    const dia = String(hoje.getDate()).padStart(2, '0');
    return `${ano}-${mes}-${dia}`;
  };

  const [dataInicio, setDataInicio] = useState<string>(formatarDataAtual());
  const [dataFim, setDataFim] = useState<string>(formatarDataAtual());
  const [resumoAtividades, setResumoAtividades] = useState<string>("");
  const [horasPorDia, setHorasPorDia] = useState<string>("");
  const [minutosPorDia, setMinutosPorDia] = useState<string>("00");

  // Verificar se todos os campos obrigatórios estão preenchidos (resumo obrigatório apenas se OBRIGA_RESUMOATIVIDADE_TIMESHEET=true)
  const isFormValid =
    selectedProjeto !== "" &&
    selectedAtividade !== "" &&
    dataInicio !== "" &&
    dataFim !== "" &&
    (!obrigaResumoAtividade || resumoAtividades.trim() !== "") &&
    (horasPorDia !== "" || minutosPorDia !== "00");

  // Converter horas HH:MM para minutos
  const converterHorasParaMinutos = (horas: string, minutos: string): number => {
    const h = horas === "" ? 0 : parseInt(horas, 10) || 0;
    const m = minutos === "" ? 0 : parseInt(minutos, 10) || 0;
    return h * 60 + m;
  };

  // Obter CPF do colaborador - usa o parâmetro da URL se disponível
  const getCpfColaborador = (): string => {
    return cpfColaborador || "";
  };

  const handleEnviarHoras = async () => {
    if (!isFormValid || !token) return;

    setEnviando(true);
    try {
      const api = new TimesheetComponentesApi();
      const horasEmMinutos = converterHorasParaMinutos(horasPorDia, minutosPorDia);

      const response = await api.apontarHorasEmLote(
        token,
        {
          projetoId: selectedProjeto,
          atividadeId: selectedAtividade,
          cpfColaborador: getCpfColaborador(),
          horas: horasEmMinutos,
          diaQuebraSemana: 0, // Sempre 0 conforme especificação
          deveSomarApontamentoDia: true, // Sempre true conforme especificação
          dataInicio: dataInicio,
          dataFim: dataFim,
          incluirSabado: includeSaturday,
          incluirDomingo: includeSunday,
          incluirFeriado: includeHoliday,
          observacao: resumoAtividades,
        },
        'pt-BR'
      );

      // Verificar se houve erro na resposta
      if (!response.sucesso) {
        setErrorMessage(response.mensagem || "Erro ao enviar horas.");
        setShowErrorDialog(true);
        setShowConfirmDialog(false);
        return;
      }

      // Fechar modal e limpar formulário
      setShowConfirmDialog(false);
      
      // Limpar campos
      setSelectedProjeto("");
      setSelectedAtividade("");
      setResumoAtividades("");
      setHorasPorDia("");
      setMinutosPorDia("00");
      setIncludeSaturday(false);
      setIncludeSunday(false);
      setIncludeHoliday(false);

      // Exibir toast de sucesso
      toast({
        title: "Sucesso",
        description: "Horas lançadas com sucesso!",
      });

      // Chamar callback para refetch
      if (onEnvioSucesso) {
        onEnvioSucesso();
      }
    } catch (err) {
      console.error("Erro ao enviar horas:", err);
      setErrorMessage("Erro inesperado ao enviar horas. Tente novamente.");
      setShowErrorDialog(true);
      setShowConfirmDialog(false);
    } finally {
      setEnviando(false);
    }
  };

  useEffect(() => {
    const loadProjetos = async () => {
      if (!token) {
        setLoadingProjetos(false);
        return;
      }

      try {
        setLoadingProjetos(true);
        const api = new TimesheetComponentesApi();
        const cpfParaEnvio = cpfColaborador || '';
        const response = await api.listarProjetosComAtividades(token, cpfParaEnvio);
        
        if (response.projetos_atividades && response.projetos_atividades.length > 0) {
          const ordenados = [...response.projetos_atividades].sort((a, b) =>
            (a.nome || "").localeCompare(b.nome || "", "pt-BR")
          );
          setProjetos(ordenados);
        }
      } catch (err) {
        console.error("Erro ao carregar projetos:", err);
      } finally {
        setLoadingProjetos(false);
      }
    };

    loadProjetos();
  }, [token, cpfColaborador]);

  useEffect(() => {
    if (selectedProjeto) {
      const projetoSelecionado = projetos.find((p) => p.id === selectedProjeto);
      if (projetoSelecionado) {
        setAtividades(projetoSelecionado.atividades || []);
        setSelectedAtividade(""); // Reset atividade quando projeto muda
      } else {
        setAtividades([]);
      }
    } else {
      setAtividades([]);
      setSelectedAtividade("");
    }
  }, [selectedProjeto, projetos]);

  const formatarLabelProjeto = (projeto: ProjetoComAtividades): string => {
    return `${projeto.id} - ${projeto.nome} / ${projeto.codigoCliente} - ${projeto.nomeCliente}`;
  };

  return (
    <Card>
      <CardContent className="p-4 space-y-4">
        {/* Projeto e Atividade */}
        <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
          <div className="space-y-1.5">
            <label className="text-sm font-medium text-foreground">Projeto</label>
            <Popover open={projetoOpen} onOpenChange={setProjetoOpen}>
              <PopoverTrigger asChild>
                <Button
                  variant="outline"
                  role="combobox"
                  aria-expanded={projetoOpen}
                  className="h-9 w-full justify-between font-normal"
                  disabled={loadingProjetos}
                >
                  {selectedProjeto
                    ? projetos.find((p) => p.id === selectedProjeto)
                      ? formatarLabelProjeto(projetos.find((p) => p.id === selectedProjeto)!)
                      : "Escolha o seu projeto de atuação..."
                    : loadingProjetos
                    ? "Carregando..."
                    : "Escolha o seu projeto de atuação..."}
                  <ChevronsUpDown className="ml-2 h-4 w-4 shrink-0 opacity-50" />
                </Button>
              </PopoverTrigger>
              <PopoverContent className="w-[var(--radix-popover-trigger-width)] p-0" align="start">
                <Command>
                  <CommandInput placeholder="Buscar projeto..." />
                  <CommandList>
                    <CommandEmpty>Nenhum projeto encontrado.</CommandEmpty>
                    <CommandGroup>
                      {projetos.map((projeto) => {
                        const label = formatarLabelProjeto(projeto);
                        return (
                          <CommandItem
                            key={projeto.id}
                            value={label}
                            onSelect={() => {
                              setSelectedProjeto(selectedProjeto === projeto.id ? "" : projeto.id);
                              setProjetoOpen(false);
                            }}
                          >
                            <Check
                              className={cn(
                                "mr-2 h-4 w-4",
                                selectedProjeto === projeto.id ? "opacity-100" : "opacity-0"
                              )}
                            />
                            {label}
                          </CommandItem>
                        );
                      })}
                    </CommandGroup>
                  </CommandList>
                </Command>
              </PopoverContent>
            </Popover>
          </div>

          <div className="space-y-1.5">
            <label className="text-sm font-medium text-foreground">Atividade</label>
            <Popover open={atividadeOpen} onOpenChange={setAtividadeOpen}>
              <PopoverTrigger asChild>
                <Button
                  variant="outline"
                  role="combobox"
                  aria-expanded={atividadeOpen}
                  className="h-9 w-full justify-between font-normal"
                  disabled={!selectedProjeto || atividades.length === 0}
                >
                  {selectedAtividade
                    ? atividades.find((a) => a.id === selectedAtividade)
                      ? atividades.find((a) => a.id === selectedAtividade)!.descricao
                      : "Selecione a atividade..."
                    : !selectedProjeto
                    ? "Selecione um projeto primeiro..."
                    : "Selecione a atividade..."}
                  <ChevronsUpDown className="ml-2 h-4 w-4 shrink-0 opacity-50" />
                </Button>
              </PopoverTrigger>
              <PopoverContent className="w-[var(--radix-popover-trigger-width)] p-0" align="start">
                <Command>
                  <CommandInput placeholder="Buscar atividade..." />
                  <CommandList>
                    <CommandEmpty>Nenhuma atividade encontrada.</CommandEmpty>
                    <CommandGroup>
                      {atividades.map((atividade) => (
                        <CommandItem
                          key={atividade.id}
                          value={atividade.descricao}
                          onSelect={() => {
                            setSelectedAtividade(selectedAtividade === atividade.id ? "" : atividade.id);
                            setAtividadeOpen(false);
                          }}
                        >
                          <Check
                            className={cn(
                              "mr-2 h-4 w-4",
                              selectedAtividade === atividade.id ? "opacity-100" : "opacity-0"
                            )}
                          />
                          {atividade.descricao}
                        </CommandItem>
                      ))}
                    </CommandGroup>
                  </CommandList>
                </Command>
              </PopoverContent>
            </Popover>
          </div>
        </div>

        {/* Datas, Horas e Checkboxes */}
        <div className="flex flex-wrap items-end gap-4">
          <div className="space-y-1.5">
            <label className="text-sm font-medium text-foreground">Data início</label>
            <Input 
              type="date" 
              value={dataInicio} 
              onChange={(e) => setDataInicio(e.target.value)}
              className="h-9 w-40" 
            />
          </div>

          <div className="space-y-1.5">
            <label className="text-sm font-medium text-foreground">Data fim</label>
            <Input 
              type="date" 
              value={dataFim} 
              onChange={(e) => setDataFim(e.target.value)}
              className="h-9 w-40" 
            />
          </div>

          <div className="space-y-1.5">
            <label className="text-sm font-medium text-foreground">Horas por dia</label>
            <div className="flex items-center gap-1.5">
              <Input
                type="number"
                min="0"
                max="23"
                value={horasPorDia}
                placeholder="00"
                onChange={(e) => {
                  const value = e.target.value;
                  // Permitir digitação livre, sem padding durante a digitação
                  setHorasPorDia(value);
                }}
                onBlur={(e) => {
                  const value = e.target.value;
                  // Aplicar padding apenas no blur se necessário
                  if (value === "") {
                    setHorasPorDia("");
                  } else {
                    const numValue = parseInt(value, 10);
                    if (!isNaN(numValue) && numValue >= 0 && numValue <= 23) {
                      setHorasPorDia(String(numValue).padStart(2, '0'));
                    } else {
                      setHorasPorDia("00");
                    }
                  }
                }}
                className="w-16 h-9 text-center [appearance:textfield] [&::-webkit-outer-spin-button]:appearance-none [&::-webkit-inner-spin-button]:appearance-none"
              />
              <span className="text-sm font-medium text-muted-foreground">:</span>
              <Input
                type="number"
                min="0"
                max="59"
                value={minutosPorDia}
                onChange={(e) => {
                  const value = e.target.value;
                  // Permitir digitação livre, sem padding durante a digitação
                  setMinutosPorDia(value);
                }}
                onBlur={(e) => {
                  const value = e.target.value;
                  // Aplicar padding apenas no blur se necessário
                  if (value === "") {
                    setMinutosPorDia("00");
                  } else {
                    const numValue = parseInt(value, 10);
                    if (!isNaN(numValue) && numValue >= 0 && numValue <= 59) {
                      setMinutosPorDia(String(numValue).padStart(2, '0'));
                    } else {
                      setMinutosPorDia("00");
                    }
                  }
                }}
                className="w-16 h-9 text-center [appearance:textfield] [&::-webkit-outer-spin-button]:appearance-none [&::-webkit-inner-spin-button]:appearance-none"
              />
            </div>
          </div>

          <div className="flex items-center space-x-2">
            <Checkbox
              id="saturday"
              checked={includeSaturday}
              onCheckedChange={(checked) => setIncludeSaturday(checked as boolean)}
            />
            <label
              htmlFor="saturday"
              className="text-sm font-medium leading-none cursor-pointer peer-disabled:cursor-not-allowed peer-disabled:opacity-70"
            >
              Incluir Sábado
            </label>
          </div>

          <div className="flex items-center space-x-2">
            <Checkbox
              id="sunday"
              checked={includeSunday}
              onCheckedChange={(checked) => setIncludeSunday(checked as boolean)}
            />
            <label
              htmlFor="sunday"
              className="text-sm font-medium leading-none cursor-pointer peer-disabled:cursor-not-allowed peer-disabled:opacity-70"
            >
              Incluir Domingo
            </label>
          </div>

          <div className="flex items-center space-x-2">
            <Checkbox
              id="holiday"
              checked={includeHoliday}
              onCheckedChange={(checked) => setIncludeHoliday(checked as boolean)}
            />
            <label
              htmlFor="holiday"
              className="text-sm font-medium leading-none cursor-pointer peer-disabled:cursor-not-allowed peer-disabled:opacity-70"
            >
              Incluir Feriado
            </label>
          </div>
        </div>

        {/* Resumo das Atividades e Botão Enviar */}
        <div className="space-y-1.5">
          <label className="text-sm font-medium text-foreground">
            Resumo das Atividades
            {obrigaResumoAtividade && <span className="text-destructive"> *</span>}
          </label>
          <div className="flex flex-col sm:flex-row gap-3 sm:items-end">
            <div className="flex-1 w-full">
              <Textarea
                placeholder="Digite aqui..."
                value={resumoAtividades}
                onChange={(e) => setResumoAtividades(e.target.value)}
                className="min-h-[60px] resize-none w-full"
              />
            </div>
            <Button 
              className="h-9 px-5 font-medium whitespace-nowrap w-full sm:w-auto"
              disabled={!isFormValid || enviando}
              onClick={() => setShowConfirmDialog(true)}
            >
              {enviando ? "Enviando..." : "Enviar horas"}
            </Button>
          </div>
        </div>
      </CardContent>

      {/* Modal de Confirmação */}
      <AlertDialog open={showConfirmDialog} onOpenChange={setShowConfirmDialog}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Confirmar envio de horas</AlertDialogTitle>
            <AlertDialogDescription>
              Tem certeza que deseja enviar as horas apontadas? Esta ação não pode ser desfeita.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel disabled={enviando}>Cancelar</AlertDialogCancel>
            <AlertDialogAction 
              onClick={handleEnviarHoras}
              disabled={enviando}
            >
              {enviando ? "Enviando..." : "Confirmar"}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>

      {/* Modal de Erro */}
      <AlertDialog open={showErrorDialog} onOpenChange={setShowErrorDialog}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle className="text-destructive">Erro ao enviar horas</AlertDialogTitle>
            <AlertDialogDescription>
              {errorMessage}
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogAction onClick={() => setShowErrorDialog(false)}>
              Entendi
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </Card>
  );
};
