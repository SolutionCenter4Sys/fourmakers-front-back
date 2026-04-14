import { useState, useEffect, useMemo } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { useForm } from "react-hook-form";
import { ArrowLeft, Check, ChevronsUpDown, Plus } from "@/components/ui/system-icons";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Label } from "@/components/ui/label";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Checkbox } from "@/components/ui/checkbox";
import { Switch } from "@/components/ui/switch";
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/popover";
import { Command, CommandEmpty, CommandGroup, CommandInput, CommandItem, CommandList } from "@/components/ui/command";
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from "@/components/ui/alert-dialog";
import { cn } from "@/lib/utils";
import { PageBreadcrumb, PageHeader } from "@presentation/components/common";
import { useGestores } from "@/hooks/useGestores";
import { useDepartamentos } from "@/hooks/useDepartamentos";
import { useDiretorias } from "@/hooks/useDiretorias";
import { useEmpresasRelacionadas } from "@/hooks/useEmpresasRelacionadas";
import { useModelosContratacao } from "@/hooks/useModelosContratacao";
import { useParametros } from "@/hooks/useParametros";
import { useCargos } from "@/hooks/useCargos";
import type { Cargo } from "@domain/entities/Cargo";
import { useToast } from "@/hooks/use-toast";
import type { FormConfig, FormFieldConfig } from "@shared/types/formConfig";
import { useAppDispatch, useAppSelector } from "@app/store/hooks";
import { inserirColaborador, editarColaborador, fetchColaboradores } from "@app/store/slices/colaboradoresSlice";
import type { InserirColaboradorPayload, EditarColaboradorPayload } from "@domain/repositories/ColaboradoresRepository";
import { convertBackendDateToInputFormat } from "@shared/utils/formatUtils";

interface NovoColaboradorForm {
  // Dados Pessoais
  nomeColaborador: string;
  cpf: string;
  celularDDI: string;
  celular: string;
  residente: boolean;
  diretoria: string;
  departamento: string;
  cargo: string;
  gestorAdm: string;
  
  // Dados Contratuais
  codigoColaborador: string;
  dataInicio: string;
  emailCorporativo: string;
  empresaRelacionada: string;
  modeloContratacao: string;
  modeloTrabalho: string;
  modeloHibrido: string;
  valorHora: string;
  custoHora: string;
  baseHorasMes: string;
  custoTotalMes: string;
  
  // Status
  ativo: boolean;
  dataInativacao: string;
}

const NovoColaborador = () => {
  const navigate = useNavigate();
  const dispatch = useAppDispatch();
  const { codColaborador } = useParams<{ codColaborador?: string }>();
  const isEditMode = !!codColaborador;
  const { colaboradores } = useAppSelector((state) => state.colaboradores);
  const { user } = useAppSelector((state) => state.auth);
  const orgId = user?.colaboradorOrg?.orgId;
  
  const { register, handleSubmit, watch, setValue, formState: { errors }, reset } = useForm<NovoColaboradorForm>({
    defaultValues: {
      celularDDI: "+55",
      residente: false,
      valorHora: "0,00",
      custoHora: "0,00",
      custoTotalMes: "0,00",
      ativo: true,
      dataInativacao: "",
    }
  });

  const [openGestorCombo, setOpenGestorCombo] = useState(false);
  const [gestorSearch, setGestorSearch] = useState("");
  const [selectedGestor, setSelectedGestor] = useState<string>("");
  const [selectedDiretoria, setSelectedDiretoria] = useState<string>("");
  const [openEmpresaCombo, setOpenEmpresaCombo] = useState(false);
  const [empresaSearch, setEmpresaSearch] = useState("");
  const [selectedEmpresa, setSelectedEmpresa] = useState<string>("");
  const [cargosAdicionados, setCargosAdicionados] = useState<Cargo[]>([]);
  const [showNovoCargoInput, setShowNovoCargoInput] = useState(false);
  const [novoCargoValue, setNovoCargoValue] = useState("");
  const [cargoParaSelecionar, setCargoParaSelecionar] = useState<string | null>(null);
  const [showErrorDialog, setShowErrorDialog] = useState(false);
  const [errorMessage, setErrorMessage] = useState<string>("");
  const [errorList, setErrorList] = useState<string[]>([]);
  
  const { diretorias: diretoriasRaw, loading: loadingDiretorias } = useDiretorias();
  
  // Remove duplicatas de diretorias baseado no código
  const diretorias = useMemo(() => {
    if (!diretoriasRaw) return [];
    const seen = new Set<string>();
    return diretoriasRaw.filter(dir => {
      if (seen.has(dir.cod)) {
        return false;
      }
      seen.add(dir.cod);
      return true;
    });
  }, [diretoriasRaw]);
  const { gestores, loading: loadingGestores } = useGestores(gestorSearch);
  const { departamentos, loading: loadingDepartamentos } = useDepartamentos(selectedDiretoria);
  const { empresas, loading: loadingEmpresas } = useEmpresasRelacionadas(empresaSearch);
  const { modelos, loading: loadingModelos } = useModelosContratacao();
  const { cargos, loading: loadingCargos } = useCargos();
  const { getParametro } = useParametros();
  const { toast } = useToast();
  const { token } = useAppSelector((state) => state.auth);

  // Buscar colaborador específico se estiver em modo de edição e não estiver na lista
  useEffect(() => {
    if (isEditMode && token && orgId && orgId > 0 && codColaborador) {
      const colaboradorEncontrado = colaboradores.find(c => c.codColaborador === codColaborador);
      if (!colaboradorEncontrado) {
        // Buscar usando codExterno (codColaborador como string)
        void dispatch(fetchColaboradores({ 
          token, 
          orgId, 
          cursor: 0,
          limite: 100,
          nomeOuEmail: "",
          codExterno: codColaborador
        }));
      }
    }
  }, [isEditMode, token, orgId, codColaborador, colaboradores, dispatch]);

  // Busca e parseia a configuração de campos do formulário
  const formConfig = useMemo<FormConfig | null>(() => {
    const configParam = getParametro("CADASTRO_COLABORADOR_CONFIG_CAMPOS");
    if (!configParam) return null;
    
    try {
      const parsed = JSON.parse(configParam) as FormConfig;
      return parsed;
    } catch (error) {
      console.error("Erro ao parsear configuração de campos:", error);
      return null;
    }
  }, [getParametro]);

  // Cria um mapa de configurações por nome do campo para acesso rápido
  const fieldConfigMap = useMemo<Map<string, FormFieldConfig>>(() => {
    const map = new Map<string, FormFieldConfig>();
    if (formConfig?.fields) {
      formConfig.fields.forEach(field => {
        map.set(field.name, field);
      });
    }
    return map;
  }, [formConfig]);

  // Função helper para verificar se um campo é visível
  const isFieldVisible = (fieldName: string): boolean => {
    const config = fieldConfigMap.get(fieldName);
    return config?.visible ?? true; // Por padrão, campos são visíveis
  };

  // Função helper para verificar se um campo é obrigatório
  const isFieldRequired = (fieldName: string): boolean => {
    const config = fieldConfigMap.get(fieldName);
    return config?.required ?? false; // Por padrão, campos não são obrigatórios
  };

  // Função helper para obter regras de validação dinâmicas (para register)
  const getFieldValidation = (fieldName: string) => {
    const required = isFieldRequired(fieldName);
    return required ? { required: true } : {};
  };

  // Função helper para obter opções do setValue (para campos Select)
  const getSetValueOptions = () => {
    return { 
      shouldValidate: true, 
      shouldDirty: true 
    };
  };

  // Obtém label customizada para "Colaborador(a)" ou usa default
  const labelColaborador = getParametro("LABEL_COLABORADOR_TIMESHEET") || "Colaborador(a)";
  
  // Obtém label customizada para "Colaborador" (título/plural) ou usa default
  const labelColaboradores = getParametro("LABEL_COLABORADORES_TIMESHEET") || "Colaborador";

  const custoHora = watch("custoHora");
  const baseHorasMes = watch("baseHorasMes");
  const celularDDI = watch("celularDDI");

  // Calcula custo total mês (Custo hora * Base de horas mês)
  useEffect(() => {
    if (custoHora && baseHorasMes) {
      const custo = parseFloat(custoHora.replace(",", ".")) || 0;
      const horas = parseFloat(baseHorasMes) || 0;
      const total = custo * horas;
      setValue("custoTotalMes", total.toFixed(2).replace(".", ","));
    }
  }, [custoHora, baseHorasMes, setValue]);

  // Limpa e reformata o celular quando o DDI mudar
  useEffect(() => {
    const currentCelular = watch("celular");
    if (currentCelular && celularDDI === "+55") {
      const formatted = formatPhoneBR(currentCelular);
      setValue("celular", formatted);
    }
  }, [celularDDI]);

  const onSubmit = async (data: NovoColaboradorForm) => {
    if (!token) {
      toast({
        variant: "destructive",
        title: "Erro de autenticação",
        description: "Token de autenticação não encontrado. Por favor, faça login novamente.",
      });
      return;
    }

    try {
      // Buscar nomes da diretoria e departamento
      const diretoriaSelecionada = diretorias.find(dir => dir.cod === data.diretoria);
      const departamentoSelecionado = departamentos.find(dept => dept.cod === data.departamento);
      
      // Buscar código do cargo
      const cargoSelecionado = [...cargos, ...cargosAdicionados].find(
        c => (c.codigoCargo || c.cargo) === data.cargo
      );

      // Calcular diasPorSemana baseado no modeloHibrido
      let diasPorSemana: number | null = null;
      if (data.modeloTrabalho === "Híbrido" && data.modeloHibrido) {
        diasPorSemana = parseInt(data.modeloHibrido) || null;
      }

      // Converter valores monetários
      const valorHora = parseFloat(data.valorHora?.replace(",", ".") || "0") || 0;
      const custoHora = parseFloat(data.custoHora?.replace(",", ".") || "0") || 0;
      const baseHoraMes = parseInt(data.baseHorasMes || "0") || null;

      // Remover formatação do CPF do campo CPF do formulário
      const cpfSemFormatacao = data.cpf?.replace(/\D/g, "") || "";
      const documentoColaborador = cpfSemFormatacao;

      // Remover formatação do celular
      const contatoPrincipal = data.celular?.replace(/\D/g, "") || "";

      // Montar payload baseado nos campos visíveis e preenchidos
      const payload: any = {};

      // Em modo de edição, codColaborador é obrigatório
      if (isEditMode) {
        payload.codColaborador = codColaborador;
      } else if (isFieldVisible("codigoColaborador") && data.codigoColaborador) {
        payload.codColaborador = data.codigoColaborador;
      }

      if (isFieldVisible("nomeColaborador") && data.nomeColaborador) {
        payload.nomeColaborador = data.nomeColaborador;
      }
      
      // CPF (apenas em modo de edição) - pegar do campo cpf do retorno da API (UUID/GUID)
      // IMPORTANTE: O campo cpf do payload deve receber o valor do campo cpf do colaborador (UUID),
      // não o documentoColaborador (CPF numérico que aparece no formulário)
      if (isEditMode) {
        const colaboradorAtual = colaboradores.find(c => c.codColaborador === codColaborador);
        if (colaboradorAtual && colaboradorAtual.cpf) {
          payload.cpf = colaboradorAtual.cpf;
        }
      }
      
      if (isFieldVisible("dataInicio") && data.dataInicio) {
        // Garantir formato YYYY-MM-DD (ano-mês-dia)
        const dataFormatada = data.dataInicio.includes('T') 
          ? data.dataInicio.split('T')[0] 
          : data.dataInicio;
        payload.dataAdmissao = dataFormatada;
      }
      
      if (isFieldVisible("cpf") && documentoColaborador) {
        payload.documentoColaborador = documentoColaborador;
      }
      
      if (isFieldVisible("emailCorporativo") && data.emailCorporativo) {
        payload.email = data.emailCorporativo;
      }
      
      if (isFieldVisible("diretoria") && data.diretoria) {
        payload.codDiretoria = data.diretoria;
        payload.diretoria = diretoriaSelecionada?.diretoria || "";
      }
      
      if (isFieldVisible("departamento") && data.departamento) {
        payload.codDepartamento = data.departamento;
        payload.departamento = departamentoSelecionado?.departamento || "";
      }
      
      if (isFieldVisible("gestorAdm") && data.gestorAdm) {
        payload.codGestor = data.gestorAdm;
      }
      
      if (isFieldVisible("celular") && contatoPrincipal) {
        payload.contatoPrincipalDDI = data.celularDDI || "+55";
        payload.contatoPrincipal = contatoPrincipal;
      }
      
      if (isFieldVisible("modeloContratacao") && data.modeloContratacao) {
        payload.modeloContratacao = data.modeloContratacao;
      }
      
      if (isFieldVisible("empresaRelacionada") && data.empresaRelacionada) {
        payload.empresaRelacionada = data.empresaRelacionada;
      }
      
      if (isFieldVisible("modeloTrabalho") && data.modeloTrabalho) {
        payload.modeloTrabalho = data.modeloTrabalho;
      }
      
      // Dias por semana
      if (diasPorSemana !== null) {
        payload.diasPorSemana = diasPorSemana;
      }
      
      // Campos numéricos
      if (isFieldVisible("baseHorasMes")) {
        payload.baseHoraMes = baseHoraMes;
      }
      if (isFieldVisible("custoHora")) {
        payload.custoHora = custoHora;
      }
      if (isFieldVisible("valorHora")) {
        payload.valorHora = valorHora;
      }
      
      // Checkbox residente
      if (isFieldVisible("residente")) {
        payload.considerarBancoDeTalentos = data.residente || false;
      }
      
      // Cargo
      if (isFieldVisible("cargo") && data.cargo) {
        payload.cargo = cargoSelecionado?.cargo || data.cargo;
        if (isEditMode) {
          payload.codigoCargo = cargoSelecionado?.codigoCargo || data.cargo;
        } else {
          payload.codCargo = cargoSelecionado?.codigoCargo || data.cargo;
        }
      }

      // Em modo de edição, incluir campos adicionais
      if (isEditMode) {
        payload.ativo = data.ativo ?? true;
        
        // Formatar data de inativação
        if (data.dataInativacao) {
          const dataFormatada = data.dataInativacao.includes('T') 
            ? data.dataInativacao.split('T')[0] 
            : data.dataInativacao;
          payload.dataInativacao = dataFormatada;
        } else if (!data.ativo) {
          // Se está inativo mas não tem data, usar hoje
          const hoje = new Date();
          payload.dataInativacao = hoje.toISOString().split('T')[0];
        }
      }

      // Chamar Redux action
      let response;
      if (isEditMode) {
        response = await dispatch(editarColaborador({ token, payload: payload as EditarColaboradorPayload })).unwrap();
      } else {
        response = await dispatch(inserirColaborador({ token, payload: payload as InserirColaboradorPayload })).unwrap();
      }

      if (response.sucesso) {
        toast({
          title: "Sucesso",
          description: response.mensagem || (isEditMode ? "Colaborador editado com sucesso!" : "Colaborador cadastrado com sucesso!"),
        });
        navigate("/colaboradores");
      } else {
        // Exibir modal de erro com mensagem e erros
        setErrorMessage(response.mensagem || (isEditMode ? "Erro ao editar colaborador. Tente novamente." : "Erro ao cadastrar colaborador. Tente novamente."));
        setErrorList(Array.isArray(response.erros) ? response.erros : []);
        setShowErrorDialog(true);
      }
    } catch (error) {
      console.error(`Erro ao ${isEditMode ? 'editar' : 'cadastrar'} colaborador:`, error);
      // Exibir modal de erro
      const errorMessage = error instanceof Error 
        ? error.message 
        : (isEditMode ? "Erro ao editar colaborador. Tente novamente." : "Erro ao cadastrar colaborador. Tente novamente.");
      setErrorMessage(errorMessage);
      setErrorList([]);
      setShowErrorDialog(true);
    }
  };

  const handleVoltar = () => {
    navigate("/colaboradores");
  };

  // Formata CPF
  const formatCPF = (value: string) => {
    const cleaned = value.replace(/\D/g, "");
    if (cleaned.length <= 11) {
      return cleaned
        .replace(/(\d{3})(\d)/, "$1.$2")
        .replace(/(\d{3})(\d)/, "$1.$2")
        .replace(/(\d{3})(\d{1,2})/, "$1-$2");
    }
    return value;
  };

  // Formata moeda
  const formatCurrency = (value: string) => {
    const cleaned = value.replace(/\D/g, "");
    const number = parseFloat(cleaned) / 100;
    return number.toFixed(2).replace(".", ",");
  };

  // Formata telefone brasileiro
  const formatPhoneBR = (value: string) => {
    const cleaned = value.replace(/\D/g, "");
    
    // Celular: (XX) 9XXXX-XXXX (11 dígitos)
    if (cleaned.length === 11) {
      return cleaned.replace(/(\d{2})(\d{5})(\d{4})/, "($1) $2-$3");
    }
    
    // Telefone fixo: (XX) XXXX-XXXX (10 dígitos)
    if (cleaned.length === 10) {
      return cleaned.replace(/(\d{2})(\d{4})(\d{4})/, "($1) $2-$3");
    }
    
    // Formatação parcial enquanto digita
    if (cleaned.length > 10) {
      return cleaned.replace(/(\d{2})(\d{5})(\d{0,4}).*/, "($1) $2-$3");
    } else if (cleaned.length > 6) {
      return cleaned.replace(/(\d{2})(\d{4})(\d{0,4}).*/, "($1) $2-$3");
    } else if (cleaned.length > 2) {
      return cleaned.replace(/(\d{2})(\d{0,5})/, "($1) $2");
    }
    
    return cleaned;
  };

  // Adiciona novo cargo
  const handleAddCargo = () => {
    const cargoNome = novoCargoValue.trim();
    if (!cargoNome) return;

    // Valida se o cargo já existe na lista (API ou adicionados)
    const cargoExiste = [...cargos, ...cargosAdicionados].some(
      c => {
        const cargoNomeComparacao = (c.cargo || c.codigoCargo || "").trim().toLowerCase();
        const novoCargoComparacao = cargoNome.trim().toLowerCase();
        return cargoNomeComparacao === novoCargoComparacao && cargoNomeComparacao !== "";
      }
    );

    if (cargoExiste) {
      toast({
        variant: "destructive",
        title: "Cargo já existe",
        description: "Este cargo já está cadastrado na lista.",
      });
      return;
    }

    // Cria o novo cargo com codigoCargo igual ao cargo
    const novoCargo: Cargo = {
      cargo: cargoNome,
      codigoCargo: cargoNome
    };

    // Adiciona o novo cargo à lista
    const novosCargosAdicionados = [...cargosAdicionados, novoCargo];
    setCargosAdicionados(novosCargosAdicionados);
    
    // Marca o cargo para ser selecionado após a lista ser atualizada
    setCargoParaSelecionar(novoCargo.codigoCargo);
    
    setNovoCargoValue("");
    setShowNovoCargoInput(false);
  };

  // Combina cargos da API com cargos adicionados manualmente
  const todosCargos = useMemo(() => {
    const cargosApi = cargos
      .filter(c => c.cargo || c.codigoCargo) // Remove cargos vazios
      .map((c, index) => {
        const value = String(c.codigoCargo || c.cargo || '').trim();
        const label = String(c.cargo || c.codigoCargo || '').trim();
        return {
          label,
          value,
          uniqueKey: `api-${index}-${value}`
        };
      });
    
    const cargosAdicionadosFormatados = cargosAdicionados
      .filter(c => c.cargo || c.codigoCargo) // Remove cargos vazios
      .map((c, index) => {
        const value = String(c.codigoCargo || c.cargo || '').trim();
        const label = String(c.cargo || c.codigoCargo || '').trim();
        return {
          label,
          value,
          uniqueKey: `added-${index}-${value}`
        };
      });
    
    // Combina e remove duplicatas baseado no value, mantendo o primeiro
    const todos = [...cargosApi, ...cargosAdicionadosFormatados];
    const seenValues = new Set<string>();
    const result: { label: string; value: string; uniqueKey: string }[] = [];
    let globalIndex = 0;
    
    todos.forEach(cargo => {
      const value = String(cargo.value || '').trim();
      if (value && !seenValues.has(value)) {
        seenValues.add(value);
        // Garante uniqueKey único usando um índice global
        result.push({
          ...cargo,
          uniqueKey: `cargo-${globalIndex++}-${value}`
        });
      }
    });
    
    return result;
  }, [cargos, cargosAdicionados]);

  // Efeito para selecionar o cargo após ser adicionado à lista
  useEffect(() => {
    if (cargoParaSelecionar && todosCargos.some(c => c.value === cargoParaSelecionar)) {
      setValue("cargo", cargoParaSelecionar, { 
        shouldValidate: true, 
        shouldDirty: true 
      });
      setCargoParaSelecionar(null);
    }
  }, [cargoParaSelecionar, todosCargos, setValue]);

  // Efeito para definir a diretoria primeiro (dispara carregamento de departamentos)
  useEffect(() => {
    if (isEditMode && codColaborador && colaboradores.length > 0) {
      const colaborador = colaboradores.find(c => c.codColaborador === codColaborador);
      if (colaborador && colaborador.diretoriaColaborador?.cod) {
        const codDiretoria = colaborador.diretoriaColaborador.cod;
        if (selectedDiretoria !== codDiretoria) {
          setSelectedDiretoria(codDiretoria);
        }
      }
    }
  }, [isEditMode, codColaborador, colaboradores, selectedDiretoria]);

  // Efeito para preencher formulário em modo de edição
  useEffect(() => {
    if (isEditMode && codColaborador && colaboradores.length > 0) {
      const colaborador = colaboradores.find(c => c.codColaborador === codColaborador);
      if (colaborador) {

        // Formatar CPF do campo documentoColaborador
        const cpfFormatado = colaborador.documentoColaborador ? formatCPF(colaborador.documentoColaborador) : "";
        
        // Formatar data de admissão para YYYY-MM-DD
        const dataAdmissaoFormatada = convertBackendDateToInputFormat(colaborador.dataAdmissao);
        
        // Formatar valores monetários
        const valorHoraFormatado = colaborador.valorHora?.toFixed(2).replace(".", ",") || "0,00";
        const custoHoraFormatado = colaborador.custoHora?.toFixed(2).replace(".", ",") || "0,00";
        
        // Formatar telefone
        const telefoneFormatado = colaborador.contatoPrincipal || "";
        const ddi = colaborador.contatoPrincipalDDI || "+55";
        
        // Buscar código do cargo
        const codigoCargo = colaborador.cargoColaborador?.codigoCargo || colaborador.cargoColaborador?.cargo || "";
        
        // Resetar e preencher formulário
        reset({
          nomeColaborador: colaborador.nome || "",
          cpf: cpfFormatado,
          celularDDI: ddi,
          celular: telefoneFormatado,
          residente: colaborador.considerarBancoDeTalentos || false,
          diretoria: colaborador.diretoriaColaborador?.cod || "",
          departamento: colaborador.departamentoColaborador?.cod || "",
          cargo: codigoCargo,
          gestorAdm: colaborador.gestor?.codigoProfissional || "",
          codigoColaborador: colaborador.codColaborador || "",
          dataInicio: dataAdmissaoFormatada,
          emailCorporativo: colaborador.email || "",
          empresaRelacionada: colaborador.empresaRelacionada || "",
          modeloContratacao: colaborador.modeloContratacao || "",
          modeloTrabalho: colaborador.modeloTrabalho || "",
          modeloHibrido: colaborador.diasPorSemana?.toString() || "",
          valorHora: valorHoraFormatado,
          custoHora: custoHoraFormatado,
          baseHorasMes: colaborador.baseHoraMes?.toString() || "",
          custoTotalMes: "0,00",
          ativo: colaborador.ativo ?? true,
          dataInativacao: convertBackendDateToInputFormat(colaborador.dataInativacao),
        });

        // Configurar outros estados auxiliares
        if (colaborador.gestor?.codigoProfissional) {
          setSelectedGestor(colaborador.gestor.codigoProfissional);
        }
        if (colaborador.empresaRelacionada) {
          setSelectedEmpresa(colaborador.empresaRelacionada);
        }

      }
    }
  }, [isEditMode, codColaborador, colaboradores, reset, setValue]);

  // Efeito para preencher diretoria quando os dados estiverem carregados
  useEffect(() => {
    if (isEditMode && codColaborador && colaboradores.length > 0 && !loadingDiretorias && diretorias.length > 0) {
      const colaborador = colaboradores.find(c => c.codColaborador === codColaborador);
      if (colaborador && colaborador.diretoriaColaborador?.cod) {
        const codDiretoria = colaborador.diretoriaColaborador.cod;
        const diretoriaExiste = diretorias.some(dir => dir.cod === codDiretoria);
        if (diretoriaExiste && watch("diretoria") !== codDiretoria) {
          setValue("diretoria", codDiretoria, { 
            shouldValidate: true, 
            shouldDirty: true 
          });
        }
      }
    }
  }, [isEditMode, codColaborador, colaboradores, loadingDiretorias, diretorias, setValue, watch]);

  // Efeito para preencher cargo quando os dados estiverem carregados
  useEffect(() => {
    if (isEditMode && codColaborador && colaboradores.length > 0 && !loadingCargos && todosCargos.length > 0) {
      const colaborador = colaboradores.find(c => c.codColaborador === codColaborador);
      if (colaborador && colaborador.cargoColaborador) {
        const codigoCargo = colaborador.cargoColaborador.codigoCargo || colaborador.cargoColaborador.cargo || "";
        const cargoNome = colaborador.cargoColaborador.cargo || colaborador.cargoColaborador.codigoCargo || "";
        
        if (codigoCargo) {
          // Tentar encontrar o cargo pelo código ou pelo nome
          const cargoEncontrado = todosCargos.find(c => {
            const cargoValue = c.value.trim();
            const cargoLabel = c.label.trim();
            return cargoValue === codigoCargo.trim() || 
                   cargoValue === cargoNome.trim() ||
                   cargoLabel === codigoCargo.trim() ||
                   cargoLabel === cargoNome.trim();
          });
          
          if (cargoEncontrado && watch("cargo") !== cargoEncontrado.value) {
            setValue("cargo", cargoEncontrado.value, { 
              shouldValidate: true, 
              shouldDirty: true 
            });
          } else if (!cargoEncontrado) {
            // Se o cargo não existe na lista, adicionar à lista de cargos adicionados
            const novoCargo: Cargo = {
              cargo: cargoNome || codigoCargo,
              codigoCargo: codigoCargo
            };
            setCargosAdicionados(prev => {
              const jaExiste = prev.some(c => 
                (c.codigoCargo || c.cargo) === codigoCargo
              );
              if (!jaExiste) {
                return [...prev, novoCargo];
              }
              return prev;
            });
            // Aguardar um pouco para o cargo ser adicionado à lista
            setTimeout(() => {
              setValue("cargo", codigoCargo, { 
                shouldValidate: true, 
                shouldDirty: true 
              });
            }, 100);
          }
        }
      }
    }
  }, [isEditMode, codColaborador, colaboradores, loadingCargos, todosCargos, setValue, watch]);

  // Efeito para preencher modelo de contratação quando os dados estiverem carregados
  useEffect(() => {
    if (isEditMode && codColaborador && colaboradores.length > 0 && !loadingModelos && modelos.length > 0) {
      const colaborador = colaboradores.find(c => c.codColaborador === codColaborador);
      if (colaborador && colaborador.modeloContratacao) {
        const modeloExiste = modelos.some(m => m.codigoModeloContratacao === colaborador.modeloContratacao);
        if (modeloExiste && watch("modeloContratacao") !== colaborador.modeloContratacao) {
          setValue("modeloContratacao", colaborador.modeloContratacao, { 
            shouldValidate: true, 
            shouldDirty: true 
          });
        }
      }
    }
  }, [isEditMode, codColaborador, colaboradores, loadingModelos, modelos, setValue, watch]);

  // Efeito para preencher modelo de trabalho (valores fixos, não precisa aguardar carregamento)
  useEffect(() => {
    if (isEditMode && codColaborador && colaboradores.length > 0) {
      const colaborador = colaboradores.find(c => c.codColaborador === codColaborador);
      if (colaborador && colaborador.modeloTrabalho) {
        // Valores possíveis: "100% Remoto", "100% Presencial", "Híbrido"
        const valoresValidos = ["100% Remoto", "100% Presencial", "Híbrido"];
        const modeloTrabalho = colaborador.modeloTrabalho.trim();
        const modeloTrabalhoValido = valoresValidos.includes(modeloTrabalho);
        
        if (modeloTrabalhoValido) {
          // Aguardar um pouco para garantir que o reset foi aplicado
          const timer = setTimeout(() => {
            if (watch("modeloTrabalho") !== modeloTrabalho) {
              setValue("modeloTrabalho", modeloTrabalho, { 
                shouldValidate: true, 
                shouldDirty: true 
              });
            }
          }, 150);
          return () => clearTimeout(timer);
        }
      }
    }
  }, [isEditMode, codColaborador, colaboradores, setValue, watch]);

  // Efeito para preencher outros campos Select
  useEffect(() => {
    if (isEditMode && codColaborador && colaboradores.length > 0) {
      const colaborador = colaboradores.find(c => c.codColaborador === codColaborador);
      if (colaborador) {
        if (colaborador.gestor?.codigoProfissional && watch("gestorAdm") !== colaborador.gestor.codigoProfissional) {
          setValue("gestorAdm", colaborador.gestor.codigoProfissional, { 
            shouldValidate: true, 
            shouldDirty: true 
          });
        }
        if (colaborador.empresaRelacionada && watch("empresaRelacionada") !== colaborador.empresaRelacionada) {
          setValue("empresaRelacionada", colaborador.empresaRelacionada, { 
            shouldValidate: true, 
            shouldDirty: true 
          });
        }
      }
    }
  }, [isEditMode, codColaborador, colaboradores, setValue, watch]);

  // Efeito específico para selecionar departamento após os departamentos serem carregados
  useEffect(() => {
    if (isEditMode && codColaborador && colaboradores.length > 0 && selectedDiretoria) {
      const colaborador = colaboradores.find(c => c.codColaborador === codColaborador);
      if (colaborador && colaborador.departamentoColaborador?.cod) {
        // Se os departamentos já foram carregados e não estão mais carregando, selecionar
        if (!loadingDepartamentos && departamentos.length > 0) {
          const departamentoExiste = departamentos.some(dept => dept.cod === colaborador.departamentoColaborador.cod);
          if (departamentoExiste) {
            setValue("departamento", colaborador.departamentoColaborador.cod, { 
              shouldValidate: true, 
              shouldDirty: true 
            });
          }
        }
      }
    }
  }, [isEditMode, codColaborador, colaboradores, selectedDiretoria, loadingDepartamentos, departamentos, setValue]);

  return (
    <div className="container mx-auto p-4 pb-24 space-y-4">
      <PageBreadcrumb 
        items={[
          { label: labelColaboradores, href: '/colaboradores' },
          { label: isEditMode ? `Editar ${labelColaborador}` : `Adicionar ${labelColaborador}` }
        ]} 
      />
      
      {/* Header */}
      <div className="flex items-center gap-4">
        <Button
          variant="ghost"
          size="icon"
          onClick={handleVoltar}
          className="h-10 w-10"
        >
          <ArrowLeft className="h-5 w-5" />
        </Button>
        <PageHeader 
          title={isEditMode ? `Editar ${labelColaboradores}` : `Adicionar ${labelColaboradores}`}
          description={isEditMode ? `Edite os dados do ${labelColaboradores.toLowerCase()}` : `Preencha os dados do novo ${labelColaboradores.toLowerCase()}`}
        />
      </div>

      <form id="novo-colaborador-form" onSubmit={handleSubmit(onSubmit)} className="space-y-6">
        {/* Campos hidden para validação de Selects */}
        {isFieldVisible("diretoria") && (
          <input type="hidden" {...register("diretoria", getFieldValidation("diretoria"))} />
        )}
        {isFieldVisible("departamento") && (
          <input type="hidden" {...register("departamento", getFieldValidation("departamento"))} />
        )}
        {isFieldVisible("cargo") && (
          <input type="hidden" {...register("cargo", getFieldValidation("cargo"))} />
        )}
        {isFieldVisible("gestorAdm") && (
          <input type="hidden" {...register("gestorAdm", getFieldValidation("gestorAdm"))} />
        )}
        {isFieldVisible("empresaRelacionada") && (
          <input type="hidden" {...register("empresaRelacionada", getFieldValidation("empresaRelacionada"))} />
        )}
        {isFieldVisible("modeloContratacao") && (
          <input type="hidden" {...register("modeloContratacao", getFieldValidation("modeloContratacao"))} />
        )}
        {isFieldVisible("modeloTrabalho") && (
          <input type="hidden" {...register("modeloTrabalho", getFieldValidation("modeloTrabalho"))} />
        )}
        
        {/* Dados Pessoais */}
        <Card className="rounded-2xl shadow-none">
          <CardHeader>
            <CardTitle>Dados pessoais</CardTitle>
          </CardHeader>
          <CardContent className="space-y-6">
            <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
              {/* Nome do Colaborador */}
              {isFieldVisible("nomeColaborador") && (
                <div className="space-y-2">
                  <Label htmlFor="nomeColaborador">
                    Nome do {labelColaborador}{isFieldRequired("nomeColaborador") && "*"}
                  </Label>
                  <Input
                    id="nomeColaborador"
                    placeholder="Digite o nome..."
                    {...register("nomeColaborador", getFieldValidation("nomeColaborador"))}
                    className={errors.nomeColaborador ? "border-red-500" : ""}
                  />
                </div>
              )}

              {/* CPF */}
              {isFieldVisible("cpf") && (
                <div className="space-y-2">
                  <Label htmlFor="cpf">CPF{isFieldRequired("cpf") && "*"}</Label>
                  <Input
                    id="cpf"
                    placeholder="000.000.00-00"
                    {...register("cpf", getFieldValidation("cpf"))}
                    onChange={(e) => {
                      const formatted = formatCPF(e.target.value);
                      setValue("cpf", formatted);
                    }}
                    maxLength={14}
                    className={errors.cpf ? "border-red-500" : ""}
                  />
                </div>
              )}

              {/* Celular */}
              {isFieldVisible("celular") && (
                <div className="space-y-2">
                  <Label htmlFor="celular">Celular{isFieldRequired("celular") && "*"}</Label>
                  <div className="flex gap-2">
                    <Select
                      value={watch("celularDDI") || "+55"}
                      onValueChange={(value) => setValue("celularDDI", value)}
                    >
                      <SelectTrigger className="w-24">
                        <SelectValue />
                      </SelectTrigger>
                      <SelectContent>
                        <SelectItem value="+55">+55</SelectItem>
                        <SelectItem value="+1">+1</SelectItem>
                        <SelectItem value="+351">+351</SelectItem>
                      </SelectContent>
                    </Select>
                    <Input
                      id="celular"
                      placeholder={celularDDI === "+55" ? "(XX) XXXXX-XXXX" : "DDD + Número"}
                      {...register("celular", getFieldValidation("celular"))}
                      onChange={(e) => {
                        const value = e.target.value;
                        if (celularDDI === "+55") {
                          const formatted = formatPhoneBR(value);
                          setValue("celular", formatted);
                        } else {
                          setValue("celular", value);
                        }
                      }}
                      maxLength={celularDDI === "+55" ? 15 : undefined}
                      className={errors.celular ? "border-red-500" : ""}
                    />
                  </div>
                </div>
              )}
            </div>

            <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
              {/* Diretoria */}
              {isFieldVisible("diretoria") && (
                <div className="space-y-2">
                  <Label htmlFor="diretoria">Diretoria{isFieldRequired("diretoria") && "*"}</Label>
                  <Select 
                    value={watch("diretoria") || ""}
                    onValueChange={(value) => {
                      setValue("diretoria", value, getSetValueOptions());
                      setSelectedDiretoria(value);
                      setValue("departamento", ""); // Reset departamento quando mudar diretoria
                    }}
                    disabled={loadingDiretorias}
                  >
                    <SelectTrigger className={errors.diretoria ? "border-red-500" : ""}>
                      <SelectValue placeholder={loadingDiretorias ? "Carregando..." : "Selecione..."} />
                    </SelectTrigger>
                    <SelectContent>
                      {diretorias && diretorias.length > 0 ? (
                        diretorias.map((dir, index) => (
                          <SelectItem key={`diretoria-${dir.cod}-${index}`} value={dir.cod}>
                            {dir.diretoria}
                          </SelectItem>
                        ))
                      ) : (
                        <SelectItem value="none" disabled>Nenhuma diretoria disponível</SelectItem>
                      )}
                    </SelectContent>
                  </Select>
                </div>
              )}

              {/* Departamento */}
              {isFieldVisible("departamento") && (
                <div className="space-y-2">
                  <Label htmlFor="departamento">Departamento{isFieldRequired("departamento") && "*"}</Label>
                  <Select 
                    value={watch("departamento") || ""}
                    onValueChange={(value) => setValue("departamento", value, getSetValueOptions())}
                    disabled={!selectedDiretoria || loadingDepartamentos}
                  >
                    <SelectTrigger className={errors.departamento ? "border-red-500" : ""}>
                      <SelectValue placeholder={
                        loadingDepartamentos 
                          ? "Carregando..." 
                          : !selectedDiretoria 
                          ? "Selecione uma diretoria primeiro" 
                          : "Selecione..."
                      } />
                    </SelectTrigger>
                    <SelectContent>
                      {departamentos && departamentos.length > 0 ? (
                        departamentos.map((dept, index) => (
                          <SelectItem key={`departamento-${dept.cod}-${index}`} value={dept.cod}>
                            {dept.departamento}
                          </SelectItem>
                        ))
                      ) : (
                        <SelectItem value="none" disabled>
                          {!selectedDiretoria ? "Selecione uma diretoria primeiro" : "Nenhum departamento disponível"}
                        </SelectItem>
                      )}
                    </SelectContent>
                  </Select>
                </div>
              )}

              {/* Cargo */}
              {isFieldVisible("cargo") && (
                <div className="space-y-2">
                  <Label htmlFor="cargo">Cargo{isFieldRequired("cargo") && "*"}</Label>
                  <div className="flex gap-2">
                    <Select 
                      value={watch("cargo") || ""}
                      onValueChange={(value) => setValue("cargo", value, getSetValueOptions())}
                      disabled={loadingCargos}
                    >
                      <SelectTrigger className={errors.cargo ? "border-red-500" : ""}>
                        <SelectValue placeholder={loadingCargos ? "Carregando..." : "Selecione..."} />
                      </SelectTrigger>
                      <SelectContent>
                        {todosCargos.length > 0 ? (
                          todosCargos.map((cargo, index) => (
                            <SelectItem key={`${cargo.uniqueKey}-${index}`} value={cargo.value}>
                              {cargo.label}
                            </SelectItem>
                          ))
                        ) : (
                          <SelectItem value="none" disabled>
                            {loadingCargos ? "Carregando..." : "Nenhum cargo disponível"}
                          </SelectItem>
                        )}
                      </SelectContent>
                    </Select>
                    <Button
                      type="button"
                      variant="outline"
                      size="icon"
                      onClick={() => setShowNovoCargoInput(true)}
                      className="flex-shrink-0"
                    >
                      <Plus className="h-4 w-4" />
                    </Button>
                  </div>
                </div>
              )}
            </div>

            {/* Input para novo cargo */}
            {showNovoCargoInput && (
              <div className="flex gap-2 items-end">
                <div className="space-y-2 flex-1">
                  <Label htmlFor="novoCargo">Novo Cargo</Label>
                  <Input
                    id="novoCargo"
                    placeholder="Digite o nome do cargo..."
                    value={novoCargoValue}
                    onChange={(e) => setNovoCargoValue(e.target.value)}
                    onKeyDown={(e) => {
                      if (e.key === "Enter") {
                        e.preventDefault();
                        handleAddCargo();
                      }
                    }}
                    autoFocus
                  />
                </div>
                <Button
                  type="button"
                  onClick={handleAddCargo}
                  disabled={!novoCargoValue.trim()}
                >
                  Adicionar
                </Button>
                <Button
                  type="button"
                  variant="outline"
                  onClick={() => {
                    setShowNovoCargoInput(false);
                    setNovoCargoValue("");
                  }}
                >
                  Cancelar
                </Button>
              </div>
            )}

            <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
              {/* Gestor Adm */}
              {isFieldVisible("gestorAdm") && (
                <div className="space-y-2">
                  <Label htmlFor="gestorAdm">Gestor Adm{isFieldRequired("gestorAdm") && "*"}</Label>
                <Popover open={openGestorCombo} onOpenChange={setOpenGestorCombo}>
                  <PopoverTrigger asChild>
                    <Button
                      variant="outline"
                      role="combobox"
                      aria-expanded={openGestorCombo}
                      className={cn(
                        "w-full justify-between",
                        !selectedGestor && "text-muted-foreground",
                        errors.gestorAdm && "border-red-500"
                      )}
                    >
                      {selectedGestor
                        ? gestores.find((gestor) => gestor.cd_Profissional === selectedGestor)?.nm_Profissional
                        : "Selecione o gestor..."}
                      <ChevronsUpDown className="ml-2 h-4 w-4 shrink-0 opacity-50" />
                    </Button>
                  </PopoverTrigger>
                  <PopoverContent className="w-full p-0" align="start">
                    <Command>
                      <CommandInput 
                        placeholder="Buscar gestor..." 
                        value={gestorSearch}
                        onValueChange={setGestorSearch}
                      />
                      <CommandList>
                        <CommandEmpty>
                          {loadingGestores ? "Carregando..." : "Nenhum gestor encontrado."}
                        </CommandEmpty>
                        <CommandGroup>
                          {gestores && gestores.length > 0 ? (
                            gestores.map((gestor) => (
                              <CommandItem
                                key={gestor.cd_Profissional}
                                value={gestor.nm_Profissional}
                                onSelect={() => {
                                  setSelectedGestor(gestor.cd_Profissional);
                                  setValue("gestorAdm", gestor.cd_Profissional, getSetValueOptions());
                                  setOpenGestorCombo(false);
                                }}
                              >
                                <Check
                                  className={cn(
                                    "mr-2 h-4 w-4",
                                    selectedGestor === gestor.cd_Profissional ? "opacity-100" : "opacity-0"
                                  )}
                                />
                                {gestor.nm_Profissional}
                              </CommandItem>
                            ))
                          ) : null}
                        </CommandGroup>
                      </CommandList>
                    </Command>
                  </PopoverContent>
                </Popover>
                </div>
              )}
            </div>

            {/* Residente Checkbox */}
            {isFieldVisible("residente") && (
              <div className="flex items-center space-x-2">
                <Checkbox
                  id="residente"
                  checked={watch("residente")}
                  onCheckedChange={(checked) => setValue("residente", checked === true)}
                />
                <Label
                  htmlFor="residente"
                  className="text-sm font-medium leading-none peer-disabled:cursor-not-allowed peer-disabled:opacity-70 cursor-pointer"
                >
                  Residente{isFieldRequired("residente") && "*"}
                </Label>
              </div>
            )}
          </CardContent>
        </Card>

        {/* Dados Contratuais */}
        <Card className="rounded-2xl shadow-none">
          <CardHeader>
            <CardTitle>Dados contratuais</CardTitle>
          </CardHeader>
          <CardContent className="space-y-6">
            {/* Primeira linha */}
            <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
              {/* Código do Colaborador */}
              {isFieldVisible("codigoColaborador") && (
                <div className="space-y-2">
                  <Label htmlFor="codigoColaborador">Código do {labelColaborador}{isFieldRequired("codigoColaborador") && "*"}</Label>
                  <Input
                    id="codigoColaborador"
                    placeholder="Digite o id..."
                    {...register("codigoColaborador", getFieldValidation("codigoColaborador"))}
                    className={errors.codigoColaborador ? "border-red-500" : ""}
                  />
                </div>
              )}

              {/* Data de Início */}
              {isFieldVisible("dataInicio") && (
                <div className="space-y-2">
                  <Label htmlFor="dataInicio">Data de Início{isFieldRequired("dataInicio") && "*"}</Label>
                  <Input
                    id="dataInicio"
                    type="date"
                    placeholder="dd/mm/yyyy"
                    {...register("dataInicio", getFieldValidation("dataInicio"))}
                    className={errors.dataInicio ? "border-red-500" : ""}
                  />
                </div>
              )}

              {/* E-mail Corporativo */}
              {isFieldVisible("emailCorporativo") && (
                <div className="space-y-2">
                  <Label htmlFor="emailCorporativo">E-mail corporativo{isFieldRequired("emailCorporativo") && "*"}</Label>
                  <Input
                    id="emailCorporativo"
                    type="email"
                    placeholder="Digite o nome..."
                    {...register("emailCorporativo", getFieldValidation("emailCorporativo"))}
                    className={errors.emailCorporativo ? "border-red-500" : ""}
                  />
                </div>
              )}
            </div>

            {/* Segunda linha */}
            <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
              {/* Empresa Relacionada */}
              {isFieldVisible("empresaRelacionada") && (
                <div className="space-y-2">
                  <Label htmlFor="empresaRelacionada">Empresa relacionada{isFieldRequired("empresaRelacionada") && "*"}</Label>
                <Popover open={openEmpresaCombo} onOpenChange={setOpenEmpresaCombo}>
                  <PopoverTrigger asChild>
                    <Button
                      variant="outline"
                      role="combobox"
                      aria-expanded={openEmpresaCombo}
                      className={cn(
                        "w-full justify-between",
                        !selectedEmpresa && "text-muted-foreground",
                        errors.empresaRelacionada && "border-red-500"
                      )}
                    >
                      {selectedEmpresa || "Selecione a empresa..."}
                      <ChevronsUpDown className="ml-2 h-4 w-4 shrink-0 opacity-50" />
                    </Button>
                  </PopoverTrigger>
                  <PopoverContent className="w-full p-0" align="start">
                    <Command>
                      <CommandInput 
                        placeholder="Buscar empresa..." 
                        value={empresaSearch}
                        onValueChange={setEmpresaSearch}
                      />
                      <CommandList>
                        <CommandEmpty>
                          {loadingEmpresas ? "Carregando..." : "Nenhuma empresa encontrada."}
                        </CommandEmpty>
                        <CommandGroup>
                          {empresas && empresas.length > 0 ? (
                            empresas.map((empresa, index) => (
                              <CommandItem
                                key={`${empresa}-${index}`}
                                value={empresa}
                                onSelect={() => {
                                  setSelectedEmpresa(empresa);
                                  setValue("empresaRelacionada", empresa, getSetValueOptions());
                                  setOpenEmpresaCombo(false);
                                }}
                              >
                                <Check
                                  className={cn(
                                    "mr-2 h-4 w-4",
                                    selectedEmpresa === empresa ? "opacity-100" : "opacity-0"
                                  )}
                                />
                                {empresa}
                              </CommandItem>
                            ))
                          ) : null}
                        </CommandGroup>
                      </CommandList>
                    </Command>
                  </PopoverContent>
                </Popover>
                </div>
              )}

              {/* Modelo de Contratação */}
              {isFieldVisible("modeloContratacao") && (
                <div className="space-y-2">
                  <Label htmlFor="modeloContratacao">Modelo de contratação{isFieldRequired("modeloContratacao") && "*"}</Label>
                  <Select 
                    value={watch("modeloContratacao") || ""}
                    onValueChange={(value) => setValue("modeloContratacao", value, getSetValueOptions())}
                    disabled={loadingModelos}
                  >
                    <SelectTrigger className={errors.modeloContratacao ? "border-red-500" : ""}>
                      <SelectValue placeholder={loadingModelos ? "Carregando..." : "Selecione..."} />
                    </SelectTrigger>
                    <SelectContent>
                      {modelos && modelos.length > 0 ? (
                        modelos.map((modelo, index) => (
                          <SelectItem key={`modelo-${modelo.codigoModeloContratacao}-${index}`} value={modelo.codigoModeloContratacao}>
                            {modelo.descricao}
                          </SelectItem>
                        ))
                      ) : (
                        <SelectItem value="none" disabled>Nenhum modelo disponível</SelectItem>
                      )}
                    </SelectContent>
                  </Select>
                </div>
              )}

              {/* Modelo de Trabalho */}
              {isFieldVisible("modeloTrabalho") && (
                <div className="space-y-2">
                  <Label htmlFor="modeloTrabalho">Modelo de trabalho{isFieldRequired("modeloTrabalho") && "*"}</Label>
                <Select 
                  value={watch("modeloTrabalho") || ""}
                  onValueChange={(value) => {
                    setValue("modeloTrabalho", value, getSetValueOptions());
                    // Limpar modelo híbrido se não for híbrido
                    if (value !== "Híbrido") {
                      setValue("modeloHibrido", "");
                    }
                  }}>
                  <SelectTrigger className={errors.modeloTrabalho ? "border-red-500" : ""}>
                    <SelectValue placeholder="Selecione..." />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="100% Remoto">100% Remoto</SelectItem>
                    <SelectItem value="100% Presencial">100% Presencial</SelectItem>
                    <SelectItem value="Híbrido">Híbrido</SelectItem>
                  </SelectContent>
                </Select>
                </div>
              )}
            </div>

            {/* Terceira linha */}
            <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
              {/* Modelo Híbrido - Condicional */}
              {watch("modeloTrabalho") === "Híbrido" && (
                <div className="space-y-2">
                  <Label htmlFor="modeloHibrido">Modelo Híbrido*</Label>
                  <Select onValueChange={(value) => setValue("modeloHibrido", value)}>
                    <SelectTrigger className={errors.modeloHibrido ? "border-red-500" : ""}>
                      <SelectValue placeholder="Selecione..." />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value="1">1 dia presencial</SelectItem>
                      <SelectItem value="2">2 dias presencial</SelectItem>
                      <SelectItem value="3">3 dias presencial</SelectItem>
                      <SelectItem value="4">4 dias presencial</SelectItem>
                    </SelectContent>
                  </Select>
                </div>
              )}

              {/* Valor Hora */}
              {isFieldVisible("valorHora") && (
                <div className="space-y-2">
                  <Label htmlFor="valorHora">Valor hora{isFieldRequired("valorHora") && "*"}</Label>
                  <div className="relative">
                    <span className="absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground">
                      R$
                    </span>
                    <Input
                      id="valorHora"
                      placeholder="0,00"
                      {...register("valorHora", getFieldValidation("valorHora"))}
                      onChange={(e) => {
                        const formatted = formatCurrency(e.target.value);
                        setValue("valorHora", formatted);
                      }}
                      className={`pl-10 ${errors.valorHora ? "border-red-500" : ""}`}
                    />
                  </div>
                </div>
              )}

              {/* Custo Hora */}
              {isFieldVisible("custoHora") && (
                <div className="space-y-2">
                  <Label htmlFor="custoHora">Custo hora{isFieldRequired("custoHora") && "*"}</Label>
                  <div className="relative">
                    <span className="absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground">
                      R$
                    </span>
                    <Input
                      id="custoHora"
                      placeholder="0,00"
                      {...register("custoHora", getFieldValidation("custoHora"))}
                      onChange={(e) => {
                        const formatted = formatCurrency(e.target.value);
                        setValue("custoHora", formatted);
                      }}
                      className={`pl-10 ${errors.custoHora ? "border-red-500" : ""}`}
                    />
                  </div>
                </div>
              )}
            </div>

            {/* Quarta linha */}
            <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
              {/* Base de Horas Mês */}
              {isFieldVisible("baseHorasMes") && (
                <div className="space-y-2">
                  <Label htmlFor="baseHorasMes">Base de horas mês{isFieldRequired("baseHorasMes") && "*"}</Label>
                  <Input
                    id="baseHorasMes"
                    type="number"
                    placeholder="Digite as horas..."
                    {...register("baseHorasMes", getFieldValidation("baseHorasMes"))}
                    className={errors.baseHorasMes ? "border-red-500" : ""}
                  />
                </div>
              )}

              {/* Custo Total Mês */}
              {isFieldVisible("custoTotalMes") && (
                <div className="space-y-2">
                  <Label htmlFor="custoTotalMes">Custo total mês{isFieldRequired("custoTotalMes") && "*"}</Label>
                <div className="relative">
                  <span className="absolute left-3 top-1/2 -translate-y-1/2 text-red-500 font-medium">
                    R$
                  </span>
                  <Input
                    id="custoTotalMes"
                    placeholder="0,00"
                    {...register("custoTotalMes")}
                    readOnly
                    className="pl-10 bg-muted text-red-500 font-medium"
                  />
                </div>
                </div>
              )}
            </div>
          </CardContent>
        </Card>

        {/* Status do Colaborador - Apenas em modo de edição */}
        {isEditMode && (
          <Card className="rounded-2xl shadow-none">
            <CardHeader>
              <CardTitle>Status do colaborador</CardTitle>
            </CardHeader>
            <CardContent className="space-y-6">
              {/* Toggle Colaborador Ativo */}
              <div className="flex items-center justify-between">
                <div className="space-y-0.5">
                  <Label htmlFor="ativo" className="text-base">
                    Colaborador ativo
                  </Label>
                </div>
                <Switch
                  id="ativo"
                  checked={watch("ativo")}
                  onCheckedChange={(checked) => {
                    setValue("ativo", checked);
                    // Se desativar, definir data de inativação como hoje se não houver
                    if (!checked && !watch("dataInativacao")) {
                      const hoje = new Date();
                      const dataFormatada = hoje.toISOString().split('T')[0];
                      setValue("dataInativacao", dataFormatada);
                    } else if (checked) {
                      // Se ativar, limpar data de inativação
                      setValue("dataInativacao", "");
                    }
                  }}
                  className={cn(
                    !watch("ativo") && "data-[state=unchecked]:bg-gray-300 dark:data-[state=unchecked]:bg-gray-600"
                  )}
                />
              </div>

              {/* Data de Inativação */}
              {!watch("ativo") && (
                <div className="space-y-2">
                  <Label htmlFor="dataInativacao">Data de inativação</Label>
                  <Input
                    id="dataInativacao"
                    type="date"
                    placeholder="00/00/0000"
                    value={watch("dataInativacao") || ""}
                    onChange={(e) => {
                      setValue("dataInativacao", e.target.value);
                    }}
                    className={errors.dataInativacao ? "border-red-500" : ""}
                  />
                  <p className="text-sm text-muted-foreground">
                    Caso deixe em branco, hoje será a data de inativação.
                  </p>
                </div>
              )}
            </CardContent>
          </Card>
        )}

        {/* Botão Cancelar */}
        <div className="flex justify-end gap-4">
          <Button
            type="button"
            variant="outline"
            onClick={handleVoltar}
          >
            Cancelar
          </Button>
        </div>
      </form>

      {/* Botão Flutuante Salvar */}
      <div className="fixed bottom-6 right-6 z-50">
        <Button 
          type="button"
          onClick={() => handleSubmit(onSubmit)()}
          size="lg"
          className="shadow-lg"
        >
          {isEditMode ? 'Salvar alterações' : `Salvar ${labelColaboradores}`}
        </Button>
      </div>

      {/* Modal de Erro */}
      <AlertDialog open={showErrorDialog} onOpenChange={setShowErrorDialog}>
        <AlertDialogContent className="max-w-2xl">
          <AlertDialogHeader>
            <AlertDialogTitle>Erro ao cadastrar colaborador</AlertDialogTitle>
            <AlertDialogDescription className="space-y-4">
              {errorMessage && (
                <div>
                  <p className="font-medium text-foreground mb-2">Mensagem:</p>
                  <p className="text-sm text-muted-foreground">{errorMessage}</p>
                </div>
              )}
              {errorList && errorList.length > 0 && (
                <div>
                  <p className="font-medium text-foreground mb-2">Erros:</p>
                  <ul className="list-disc list-inside space-y-1 text-sm text-muted-foreground">
                    {errorList.map((erro, index) => (
                      <li key={index}>{erro}</li>
                    ))}
                  </ul>
                </div>
              )}
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogAction onClick={() => setShowErrorDialog(false)}>
              Fechar
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  );
};

export default NovoColaborador;

