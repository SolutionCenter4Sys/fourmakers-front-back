import { useState, useRef, useEffect, useCallback } from "react";
import { useNavigate } from "react-router-dom";
import { Card, CardContent } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Button } from "@/components/ui/button";
import { Textarea } from "@/components/ui/textarea";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/popover";
import { Command, CommandEmpty, CommandGroup, CommandInput, CommandItem, CommandList } from "@/components/ui/command";
import { Check, ChevronsUpDown } from "@/components/ui/system-icons";
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from "@/components/ui/tooltip";
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from "@/components/ui/alert-dialog";
import { Upload, ShoppingCart, Calculator, ArrowLeft, Edit, Trash2, Paperclip, Send, AlertTriangle, Loader2, CalendarIcon } from "@/components/ui/system-icons";
import { Calendar } from "@/components/ui/calendar";
import { PageBreadcrumb, DadosBancariosCard } from "@presentation/components/common";
import { format, differenceInDays, parse } from "date-fns";
import { ptBR } from "date-fns/locale";
import { cn } from "@/lib/utils";
import { useToast } from "@/hooks/use-toast";
import { useAppSelector } from "@/app/store/hooks";
import { useParametros } from "@/hooks/useParametros";
import { container } from "@/core/di/container";
import type { ProjetoColaborador, Verba, ArquivoSolicitacao } from "@/data/api/ReembolsoSolicitacaoApi";
import { GetProjetosColaboradorUseCase } from "@domain/usecases/GetProjetosColaboradorUseCase";
import { GetVerbasUseCase } from "@domain/usecases/GetVerbasUseCase";
import { AnalisarComprovantesUseCase } from "@domain/usecases/AnalisarComprovantesUseCase";
import { InserirSolicitacaoUseCase } from "@domain/usecases/InserirSolicitacaoUseCase";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import JSZip from "jszip";

// Mapeamento de extensões para tipos
const extensaoParaTipo: Record<string, number> = {
  pdf: 1,
  png: 2,
  jpg: 3,
  jpeg: 4,
  xlsx: 6,
};

// Função para converter arquivo para base64
const fileToBase64 = (file: File): Promise<string> => {
  return new Promise((resolve, reject) => {
    const reader = new FileReader();
    reader.readAsDataURL(file);
    reader.onload = () => {
      const result = reader.result as string;
      // Remover o prefixo data:*/*;base64,
      const base64 = result.split(",")[1];
      resolve(base64);
    };
    reader.onerror = (error) => reject(error);
  });
};

// Interface para item do carrinho
interface CarrinhoItem {
  objetivo: string;
  destino: string;
  dataInicio: string; // dd/mm/yyyy
  dataFim: string; // dd/mm/yyyy
  projetoId: string;
  clienteId: string;
  projetoLabel: string;
  verbaId: number;
  categoria: string;
  data: string;
  dataDate?: Date;
  quantidade: string;
  unidade: string;
  valorUnitario: string;
  valor: string;
  valorTotal: number;
  descricao: string;
  arquivos: ArquivoSolicitacao[];
  arquivosNomes: string[];
  /** Arquivos originais para montar o ZIP no envio (quando adicionado; vazio se editado e sem File) */
  arquivosFiles?: File[];
  exigirComprovante: boolean;
  tipoCodigo: number;
  comprovanteExcedido?: boolean;
}

export default function InserirReembolso() {
  const navigate = useNavigate();
  const { toast } = useToast();
  const token = useAppSelector((state) => state.auth.token);
  const user = useAppSelector((state) => state.auth.user);
  const codigoProfissional = user?.colaboradorOrg?.codColaborador || "";
  const { isEnabled } = useParametros();
  const permitirSolicitacaoSemProjeto = isEnabled("REEMBOLSO_PERMITIR_SOLICITACAO_SEM_PROJETO");

  // Estados de carregamento
  const [loadingProjetos, setLoadingProjetos] = useState(true);
  const [loadingVerbas, setLoadingVerbas] = useState(false);
  const [, setLoadingParametros] = useState(true);
  const [loadingAnalise, setLoadingAnalise] = useState(false);
  const [enviando, setEnviando] = useState(false);

  // Dados carregados da API
  const [projetos, setProjetos] = useState<ProjetoColaborador[]>([]);
  const [verbas, setVerbas] = useState<Verba[]>([]);
  const [validadeComprovanteDias, setValidadeComprovanteDias] = useState<number>(30);

  // Estados do formulário - Seção superior
  const [objetivo, setObjetivo] = useState("");
  const [destino, setDestino] = useState("");
  const [dataInicio, setDataInicio] = useState<Date | undefined>(undefined);
  const [dataFim, setDataFim] = useState<Date | undefined>(undefined);

  // Estados do formulário
  const [clienteProjetoValue, setClienteProjetoValue] = useState("");
  const [projetoSelecionado, setProjetoSelecionado] = useState<ProjetoColaborador | null>(null);
  const [clienteProjetoOpen, setClienteProjetoOpen] = useState(false);
  const [dataInicioPickerOpen, setDataInicioPickerOpen] = useState(false);
  const [dataFimPickerOpen, setDataFimPickerOpen] = useState(false);
  const [dataDespesaPickerOpen, setDataDespesaPickerOpen] = useState(false);
  const [date, setDate] = useState<Date | undefined>(undefined);
  const [categoriaSelecionada, setCategoriaSelecionada] = useState<Verba | null>(null);
  const [unidade, setUnidade] = useState("");
  const [quantidade, setQuantidade] = useState("1");
  const [valorUnitario, setValorUnitario] = useState("0");
  const [valor, setValor] = useState("");
  const [descricao, setDescricao] = useState("");
  const [anexos, setAnexos] = useState<File[]>([]);
  const [anexosBase64, setAnexosBase64] = useState<ArquivoSolicitacao[]>([]);
  const [itemEditando, setItemEditando] = useState<number | null>(null);
  const [verbaIdParaSelecionar, setVerbaIdParaSelecionar] = useState<number | null>(null);
  const [valorParaRestaurar, setValorParaRestaurar] = useState<string | null>(null);
  const fileInputRef = useRef<HTMLInputElement>(null);

  // Estados de validação visual
  const [comprovanteExcedido, setComprovanteExcedido] = useState(false);
  const [diasExcedidos, setDiasExcedidos] = useState<number>(0);
  const [valorExcedido, setValorExcedido] = useState(false);

  // Estados de erro para validação
  const [erros, setErros] = useState<{
    objetivo?: boolean;
    dataInicio?: boolean;
    dataFim?: boolean;
    clienteProjeto?: boolean;
    categoria?: boolean;
    data?: boolean;
    quantidade?: boolean;
    valor?: boolean;
    descricao?: boolean;
    comprovante?: boolean;
  }>({});

  // Modal de sucesso
  const [modalSucesso, setModalSucesso] = useState(false);
  const [modalErro, setModalErro] = useState(false);
  const [errosApi, setErrosApi] = useState<string[]>([]);

  // Carrinho de solicitações
  const [carrinho, setCarrinho] = useState<CarrinhoItem[]>([]);

  // Use Cases
  const getProjetosColaboradorUseCase = container.resolve(GetProjetosColaboradorUseCase);
  const getVerbasUseCase = container.resolve(GetVerbasUseCase);
  const analisarComprovantesUseCase = container.resolve(AnalisarComprovantesUseCase);
  const inserirSolicitacaoUseCase = container.resolve(InserirSolicitacaoUseCase);


  // Carregar projetos no loading da página
  useEffect(() => {
    const carregarProjetos = async () => {
      if (!token || !codigoProfissional) return;

      setLoadingProjetos(true);
      try {
        const response = await getProjetosColaboradorUseCase.execute(token, codigoProfissional);
        if (response.sucesso && response.retorno) {
          setProjetos(response.retorno);
        } else {
          toast({
            title: "Erro",
            description: response.mensagem || "Erro ao carregar projetos",
            variant: "destructive",
          });
        }
      } catch (error) {
        toast({
          title: "Erro",
          description: "Erro ao carregar projetos",
          variant: "destructive",
        });
      } finally {
        setLoadingProjetos(false);
      }
    };

    carregarProjetos();
  }, [token, codigoProfissional]);

  // Carregar parâmetros gerais no loading da página
  useEffect(() => {
    const carregarParametros = async () => {
      if (!token) return;

      setLoadingParametros(true);
      try {
        const response = await getVerbasUseCase.obterVerbasEParametroValidacao(token);
        if (response.sucesso && response.retorno) {
          setValidadeComprovanteDias(response.retorno.parametroReembolso?.validadeComprovanteDias || 30);
        }
      } catch (error) {
        console.error("Erro ao carregar parâmetros:", error);
      } finally {
        setLoadingParametros(false);
      }
    };

    carregarParametros();
  }, [token]);

  // Carregar verbas quando projeto for selecionado ou ao iniciar
  useEffect(() => {
    const carregarVerbas = async () => {
      if (!token) {
        setVerbas([]);
        return;
      }

      setLoadingVerbas(true);
      try {
        // Se não houver projeto selecionado, enviar strings vazias
        const codigoProjeto = projetoSelecionado?.codigoProjeto || "";
        const codigoCliente = projetoSelecionado?.codigoCliente || "";
        
        const response = await getVerbasUseCase.listarVerbasComExcecao(
          token,
          codigoProjeto,
          codigoCliente
        );
        if (response.sucesso && response.retorno) {
          setVerbas(response.retorno);
        } else {
          toast({
            title: "Erro",
            description: response.mensagem || "Erro ao carregar categorias",
            variant: "destructive",
          });
        }
      } catch (error) {
        toast({
          title: "Erro",
          description: "Erro ao carregar categorias",
          variant: "destructive",
        });
      } finally {
        setLoadingVerbas(false);
      }
    };

    carregarVerbas();
  }, [token, projetoSelecionado, permitirSolicitacaoSemProjeto]);

  // Selecionar categoria quando verbas forem carregadas e houver verbaId para selecionar
  useEffect(() => {
    if (verbaIdParaSelecionar !== null && verbas.length > 0) {
      const verbaEncontrada = verbas.find(
        (v) => (v.verbaId ?? v.id) === verbaIdParaSelecionar
      );
      if (verbaEncontrada) {
        setCategoriaSelecionada(verbaEncontrada);
        setUnidade(verbaEncontrada.unidade || "");
        setValorUnitario(verbaEncontrada.valor?.toString() || "0");
        // Se for tipoCodigo = 2, calcular valor automaticamente
        if (verbaEncontrada.tipoCodigo === 2) {
          const novoValorTotal = Number(quantidade) * Number(verbaEncontrada.valor || 0);
          setValor(novoValorTotal > 0 ? formatarValorNumerico(novoValorTotal) : "");
          } else {
          // Se for tipoCodigo = 1, restaurar valor se houver um valor para restaurar
          if (valorParaRestaurar !== null) {
            const valorFormatado = valorParaRestaurar ? formatarValorNumerico(valorParaRestaurar) : "";
            setValor(valorFormatado);
            setValorParaRestaurar(null);
          } else {
            setValor("");
          }
          }
        setVerbaIdParaSelecionar(null); // Limpar após selecionar
      }
    }
  }, [verbas, verbaIdParaSelecionar, quantidade, valorParaRestaurar]);

  // Validar data do comprovante
  const validarDataComprovante = useCallback((dataComprovante: Date | string | undefined) => {
    // Se o campo estiver vazio, não validar
    if (!dataComprovante) {
      setComprovanteExcedido(false);
      setDiasExcedidos(0);
      return;
    }

    // Se for string, verificar se não está vazia
    if (typeof dataComprovante === 'string' && dataComprovante.trim() === '') {
      setComprovanteExcedido(false);
      setDiasExcedidos(0);
      return;
    }

    const dataDate = typeof dataComprovante === 'string' ? stringParaDate(dataComprovante) : dataComprovante;
    if (!dataDate) {
      setComprovanteExcedido(false);
      setDiasExcedidos(0);
      return;
    }

    const hoje = new Date();
    const diasDiferenca = differenceInDays(hoje, dataDate);
    const excedeu = diasDiferenca > validadeComprovanteDias;
    setComprovanteExcedido(excedeu);
    if (excedeu) {
      setDiasExcedidos(diasDiferenca - validadeComprovanteDias);
    } else {
      setDiasExcedidos(0);
    }
  }, [validadeComprovanteDias]);

  // Função para formatar valor monetário enquanto digita (recebe string com números e formata como centavos)
  const formatarValorMonetario = (value: string): string => {
    // Remove tudo que não é número
    const apenasNumeros = value.replace(/\D/g, '');
    
    // Se estiver vazio, retorna vazio
    if (apenasNumeros === '') return '';
    
    // Converte para número e divide por 100 para ter centavos
    const numero = parseFloat(apenasNumeros) / 100;
    
    // Formata como moeda brasileira
    return numero.toLocaleString('pt-BR', {
      minimumFractionDigits: 2,
      maximumFractionDigits: 2,
    });
  };

  // Função para formatar um valor numérico já desformatado (usado ao restaurar do carrinho)
  const formatarValorNumerico = (value: string | number): string => {
    // Se estiver vazio, retorna vazio
    if (!value || value === '' || value === '0') return '';
    
    // Converte para número
    const numero = typeof value === 'string' ? parseFloat(value) : value;
    
    // Se não for um número válido, retorna vazio
    if (isNaN(numero) || numero === 0) return '';
    
    // Formata como moeda brasileira (sem dividir por 100, pois já está no formato correto)
    return numero.toLocaleString('pt-BR', {
      minimumFractionDigits: 2,
      maximumFractionDigits: 2,
    });
  };

  // Função para converter valor formatado de volta para número
  const desformatarValorMonetario = (value: string): string => {
    // Remove tudo que não é número
    const apenasNumeros = value.replace(/\D/g, '');
    
    // Se estiver vazio, retorna vazio
    if (apenasNumeros === '') return '';
    
    // Converte para número dividindo por 100 (pois formatamos como centavos)
    const numero = parseFloat(apenasNumeros) / 100;
    
    // Se não for um número válido, retorna string vazia
    if (isNaN(numero)) return '';
    
    // Retorna como string para manter compatibilidade
    return numero.toString();
  };

  // Função para formatar valor monetário para exibição (com separadores de milhares)
  const formatarValorParaExibicao = (valor: number): string => {
    return valor.toLocaleString('pt-BR', {
      minimumFractionDigits: 2,
      maximumFractionDigits: 2,
    });
  };

  // Validar valor máximo (para tipoCodigo = 1 ou tipoCodigo = 2)
  const validarValorMaximo = useCallback((valorDigitado: string) => {
    if (!categoriaSelecionada) {
      setValorExcedido(false);
      return;
    }

    const valorNumerico = Number(valorDigitado) || 0;
    // Para tipoCodigo = 2, valida o valor unitário
    // Para tipoCodigo = 1, valida o valor total digitado
    if (categoriaSelecionada.tipoCodigo === 2) {
      setValorExcedido(valorNumerico > categoriaSelecionada.valor);
    } else {
      // Para tipoCodigo = 1, valida o valor total
      setValorExcedido(valorNumerico > categoriaSelecionada.valor);
    }
  }, [categoriaSelecionada]);

  // Analisar comprovantes fiscais quando arquivos forem adicionados
  const analisarComprovantes = useCallback(async (arquivos: File[]) => {
    if (!token || arquivos.length === 0) return;

    setLoadingAnalise(true);
    try {
      // Converter arquivos para base64
      const arquivosParaAnalise = await Promise.all(
        arquivos.map(async (arquivo) => {
          const base64 = await fileToBase64(arquivo);
          const extensao = arquivo.name.split(".").pop()?.toLowerCase() || "";
          const tipo = extensaoParaTipo[extensao] || 1;
          return { base64, tipo };
        })
      );

      // Salvar base64 para envio posterior
      setAnexosBase64(arquivosParaAnalise);

      // Enviar para análise OCR
      const response = await analisarComprovantesUseCase.execute(token, arquivosParaAnalise);
      if (response.sucesso && response.retorno) {
        // Preencher data da despesa se retornada
        if (response.retorno.data) {
          try {
            // A data pode vir no formato ISO com hora ou apenas data
            let dataAnalisada: Date;
            if (response.retorno.data.includes('T')) {
              dataAnalisada = new Date(response.retorno.data);
            } else {
              dataAnalisada = parse(response.retorno.data, "yyyy-MM-dd", new Date());
            }
            // Verificar se a data é válida
            if (dataAnalisada && !isNaN(dataAnalisada.getTime())) {
              setDate(dataAnalisada);
              validarDataComprovante(dataAnalisada);
            }
          } catch {
            // Ignorar erro de parse
          }
        }

        // Preencher quantidade se retornada
        if (response.retorno.quantidade && response.retorno.quantidade > 0) {
          setQuantidade(response.retorno.quantidade.toString());
        }

        // Preencher valor se retornado
        if (response.retorno.valor && response.retorno.valor > 0) {
          setValor(response.retorno.valor.toString());
          validarValorMaximo(response.retorno.valor.toString());
        }
      }
    } catch (error) {
      console.error("Erro ao analisar comprovantes:", error);
    } finally {
      setLoadingAnalise(false);
    }
  }, [token, validarDataComprovante, validarValorMaximo]);

  // Calcular valor total
  const mostrarCamposQuantidadeValor = categoriaSelecionada?.tipoCodigo === 2;
  const valorTotal = mostrarCamposQuantidadeValor
    ? Number(quantidade) * Number(valorUnitario)
    : (valor ? Number(desformatarValorMonetario(valor)) : 0);
  
  // Validar se valor total excede o teto da verba (apenas para tipoCodigo = 1)
  const valorTotalExcedido = categoriaSelecionada && categoriaSelecionada.tipoCodigo === 1 && valorTotal > (categoriaSelecionada.valor || 0);

  // Quando tipoCodigo = 2, o valor é calculado automaticamente (quantidade * valorUnitario)
  // Não precisamos setar o campo valor nesse caso, pois ele não é visível
  // O valor total é mostrado no card verde abaixo

  // Função para converter string dd/mm/yyyy ou dd/mm/yy para Date
  // Aceita ano com 2 dígitos: 00-29 → 20xx, 30-99 → 19xx
  const stringParaDate = (dataStr: string): Date | null => {
    if (!dataStr) return null;
    const parts = dataStr.split("/");
    if (parts.length !== 3) return null;
    const dia = parseInt(parts[0], 10);
    const mes = parseInt(parts[1], 10);
    let ano = parseInt(parts[2], 10);
    if (Number.isNaN(dia) || Number.isNaN(mes) || Number.isNaN(ano)) return null;
    // Expandir ano de 2 para 4 dígitos
    if (ano >= 0 && ano <= 99) {
      ano = ano <= 29 ? 2000 + ano : 1900 + ano;
    }
    if (!dia || !mes || ano < 1000 || ano > 9999) return null;
    const data = new Date(ano, mes - 1, dia);
    if (data.getDate() === dia && data.getMonth() === mes - 1 && data.getFullYear() === ano) {
      return data;
    }
    return null;
  };

  // Limpar formulário
  const handleLimpar = () => {
    setObjetivo("");
    setDestino("");
    setDataInicio(undefined);
    setDataFim(undefined);
    setClienteProjetoValue("");
    setProjetoSelecionado(null);
    setDate(undefined);
    setCategoriaSelecionada(null);
    setUnidade("");
    setQuantidade("1");
    setValorUnitario("0");
    setValor("");
    setDescricao("");
    setAnexos([]);
    setAnexosBase64([]);
    setItemEditando(null);
    setErros({});
    setComprovanteExcedido(false);
    setValorExcedido(false);
    setVerbas([]);
    setVerbaIdParaSelecionar(null);
    setValorParaRestaurar(null);
    if (fileInputRef.current) {
      fileInputRef.current.value = "";
    }
  };

  // Adicionar ao carrinho
  const handleAdicionarCarrinho = () => {
    const novosErros: typeof erros = {};
    let temErro = false;

    // Validar campos da seção superior
    if (!objetivo || objetivo.trim() === "") {
      novosErros.objetivo = true;
      temErro = true;
    }

    if (!dataInicio) {
      novosErros.dataInicio = true;
      temErro = true;
    }

    // Data final é opcional; se preenchida, já é Date válido

    // Validar CLIENTE/PROJETO apenas se o parâmetro não permitir solicitação sem projeto
    if (!permitirSolicitacaoSemProjeto && !projetoSelecionado) {
      novosErros.clienteProjeto = true;
      temErro = true;
    }

    if (!categoriaSelecionada) {
      novosErros.categoria = true;
      temErro = true;
    }

    if (!date) {
      novosErros.data = true;
      temErro = true;
    }

    // Validar quantidade e valor unitário somente se tipoCodigo = 2
    if (mostrarCamposQuantidadeValor) {
      const quantidadeNumerica = Number(quantidade);
      if (!quantidade || quantidadeNumerica <= 0) {
        novosErros.quantidade = true;
        temErro = true;
      }

      const valorUnitarioNumerico = Number(valorUnitario);
      if (!valorUnitario || valorUnitarioNumerico <= 0) {
        novosErros.valor = true;
        temErro = true;
      }
    } else {
      // Se não é tipoCodigo 2, validar campo valor
      const valorNumerico = Number(valor);
      if (!valor || valorNumerico <= 0) {
        novosErros.valor = true;
        temErro = true;
      }
    }

    if (!descricao || descricao.trim() === "") {
      novosErros.descricao = true;
      temErro = true;
    }

    // Validar comprovante obrigatório
    if (categoriaSelecionada?.exigirComprovante && anexos.length === 0) {
      novosErros.comprovante = true;
      temErro = true;
    }

    setErros(novosErros);

    if (temErro) {
      toast({
        title: "Campos obrigatórios",
        description: "Por favor, preencha todos os campos obrigatórios.",
        variant: "destructive",
      });
      return;
    }

    const novoItem: CarrinhoItem = {
      objetivo: objetivo.trim(),
      destino: destino.trim(),
      dataInicio: dataInicio ? format(dataInicio, "dd/MM/yyyy", { locale: ptBR }) : "",
      dataFim: dataFim ? format(dataFim, "dd/MM/yyyy", { locale: ptBR }) : "",
      projetoId: projetoSelecionado?.codigoProjeto || "",
      clienteId: projetoSelecionado?.codigoCliente || "",
      projetoLabel: projetoSelecionado?.projetos || "",
      verbaId: categoriaSelecionada!.verbaId ?? categoriaSelecionada!.id ?? 0,
      categoria: categoriaSelecionada!.categoria,
      data: date ? format(date, "dd/MM/yyyy", { locale: ptBR }) : "",
      dataDate: date ?? undefined,
      quantidade,
      unidade,
      valorUnitario,
      valor: valor ? desformatarValorMonetario(valor) : (Number(quantidade) * Number(valorUnitario)).toString(),
      valorTotal,
      descricao,
      arquivos: anexosBase64,
      arquivosNomes: anexos.map((a) => a.name),
      arquivosFiles: itemEditando !== null && carrinho[itemEditando]?.arquivosFiles?.length === anexos.length
        ? carrinho[itemEditando].arquivosFiles
        : [...anexos],
      exigirComprovante: categoriaSelecionada!.exigirComprovante,
      tipoCodigo: categoriaSelecionada!.tipoCodigo,
      comprovanteExcedido,
    };

    if (itemEditando !== null) {
      const novoCarrinho = [...carrinho];
      novoCarrinho[itemEditando] = novoItem;
      setCarrinho(novoCarrinho);
      setItemEditando(null);
    } else {
      setCarrinho([...carrinho, novoItem]);
    }

    // Limpar apenas os campos do item, mantendo os campos da seção superior
    setClienteProjetoValue("");
    setProjetoSelecionado(null);
    setDate(undefined);
    setCategoriaSelecionada(null);
    setUnidade("");
    setQuantidade("1");
    setValorUnitario("0");
    setValor("");
    setDescricao("");
    setAnexos([]);
    setAnexosBase64([]);
    setErros({});
    setComprovanteExcedido(false);
    setValorExcedido(false);
    setVerbas([]);
    setVerbaIdParaSelecionar(null);
    setValorParaRestaurar(null);
    if (fileInputRef.current) {
      fileInputRef.current.value = "";
    }
  };

  // Editar item do carrinho
  const handleEditarItem = async (index: number) => {
    const item = carrinho[index];

    // Restaurar campos da seção superior
    setObjetivo(item.objetivo);
    setDestino(item.destino);
    setDataInicio(item.dataInicio ? stringParaDate(item.dataInicio) ?? undefined : undefined);
    setDataFim(item.dataFim ? stringParaDate(item.dataFim) ?? undefined : undefined);

    // Encontrar projeto
    const projeto = projetos.find(
      (p) => p.codigoProjeto === item.projetoId && p.codigoCliente === item.clienteId
    );
    if (projeto) {
      setClienteProjetoValue(`${projeto.codigoProjeto}-${projeto.codigoCliente}`);
      setProjetoSelecionado(projeto);
      // Armazenar verbaId para selecionar quando as verbas forem carregadas
      setVerbaIdParaSelecionar(item.verbaId);
    } else {
      // Se não houver projeto, limpar categoria
      setCategoriaSelecionada(null);
      setVerbas([]);
      setVerbaIdParaSelecionar(null);
    }

    // Restaurar data
    setDate(item.dataDate ?? (item.data ? stringParaDate(item.data) ?? undefined : undefined));

    setQuantidade(item.quantidade);
    setUnidade(item.unidade);
    setValorUnitario(item.valorUnitario);
    // Restaurar valor - se tipoCodigo = 2, calcular, senão usar valor salvo
    if (item.tipoCodigo === 2) {
      // Para tipoCodigo = 2, o valor é calculado (quantidade * valorUnitario)
      const valorCalculado = Number(item.quantidade) * Number(item.valorUnitario);
      setValor(valorCalculado > 0 ? formatarValorNumerico(valorCalculado) : "");
    } else {
      // Para tipoCodigo = 1, armazenar valor para restaurar quando categoria for selecionada
      setValorParaRestaurar(item.valor || "");
      // Se a categoria já estiver selecionada, restaurar imediatamente
      if (categoriaSelecionada && categoriaSelecionada.tipoCodigo === 1) {
        const valorFormatado = item.valor ? formatarValorNumerico(item.valor) : "";
        setValor(valorFormatado);
      }
    }
    setDescricao(item.descricao);
    
    // Restaurar comprovantes
    setAnexosBase64(item.arquivos);
    // Criar objetos File simulados a partir dos nomes salvos para exibição
    if (item.arquivosNomes && item.arquivosNomes.length > 0) {
      const arquivosSimulados = item.arquivosNomes.map((nome, idx) => {
        // Estimar tamanho baseado no tipo (aproximado)
        const tamanhoEstimado = item.arquivos[idx]?.tipo === 1 ? 500000 : 200000; // PDF ~500KB, imagem ~200KB
        const tipoMime = item.arquivos[idx]?.tipo === 1 ? 'application/pdf' : 
                        nome.toLowerCase().endsWith('.png') ? 'image/png' :
                        nome.toLowerCase().endsWith('.jpg') || nome.toLowerCase().endsWith('.jpeg') ? 'image/jpeg' :
                        'application/octet-stream';
        
        // Criar um File mínimo com o nome e tamanho para exibição
        const blob = new Blob([''], { type: tipoMime });
        const file = new File([blob], nome, { 
          type: tipoMime,
          lastModified: Date.now()
        });
        // Sobrescrever a propriedade size para exibição
        Object.defineProperty(file, 'size', {
          value: tamanhoEstimado,
          writable: false
        });
        return file;
      });
      setAnexos(arquivosSimulados);
    } else {
      setAnexos([]);
    }
    
    setItemEditando(index);
    setErros({});
    setComprovanteExcedido(item.comprovanteExcedido || false);

    // Scroll para o formulário
    window.scrollTo({ top: 0, behavior: "smooth" });
  };

  // Excluir item do carrinho
  const handleExcluirItem = (index: number) => {
    const novoCarrinho = carrinho.filter((_, i) => i !== index);
    setCarrinho(novoCarrinho);
  };

  // Enviar solicitações (ZIP com dados.json + archive_N)
  const handleEnviarSolicitacoes = async () => {
    if (carrinho.length === 0) {
      toast({
        title: "Carrinho vazio",
        description: "Adicione pelo menos uma solicitação ao carrinho.",
        variant: "destructive",
      });
      return;
    }

    if (!token) {
      toast({
        title: "Erro de autenticação",
        description: "Por favor, faça login novamente.",
        variant: "destructive",
      });
      return;
    }

    const primeiroItem = carrinho[0];
    if (!primeiroItem.objetivo || primeiroItem.objetivo.trim() === "") {
      toast({
        title: "Campo obrigatório",
        description: "O campo Objetivo é obrigatório. Por favor, edite o item no carrinho.",
        variant: "destructive",
      });
      return;
    }

    const dataInicioDate = stringParaDate(primeiroItem.dataInicio);
    if (!dataInicioDate) {
      toast({
        title: "Erro",
        description: "Data de início inválida. Por favor, verifique a data informada.",
        variant: "destructive",
      });
      return;
    }
    const dataInicioFormatada = format(dataInicioDate, "yyyy-MM-dd");
    const dataFimDate = primeiroItem.dataFim ? stringParaDate(primeiroItem.dataFim) : null;
    const dataFimFormatada = dataFimDate ? format(dataFimDate, "yyyy-MM-dd") : "";
    if (primeiroItem.dataFim && !dataFimDate) {
      toast({
        title: "Erro",
        description: "Data final inválida. Por favor, verifique a data informada.",
        variant: "destructive",
      });
      return;
    }

    // Montar lista de nomes archive_N por solicitação e lista de conteúdos na mesma ordem
    let globalIndex = 0;
    const solicitacoesComNomes: Array<{
      projetoId: string;
      clienteId: string;
      verbaId: number;
      descricao: string;
      dataDespesa: string;
      valor: number;
      valorUnidade?: number;
      quantidade?: number;
      unidade?: string;
      arquivos: string[];
    }> = [];
    const arquivosParaZip: Array<{ nomeZip: string; blob: Blob }> = [];

    for (const item of carrinho) {
      const nomesArquivos: string[] = [];
      const n = item.arquivos?.length ?? 0;
      for (let j = 0; j < n; j++) {
        globalIndex += 1;
        const ext = item.arquivosFiles?.[j]?.name?.split(".").pop()?.toLowerCase()
          ?? (item.arquivosNomes?.[j]?.split(".").pop()?.toLowerCase())
          ?? (item.arquivos?.[j]?.tipo === 1 ? "pdf" : "png");
        const nomeRef = `archive_${globalIndex}`;
        nomesArquivos.push(nomeRef);
        const nomeZip = `${nomeRef}.${ext}`;
        if (item.arquivosFiles?.[j]) {
          arquivosParaZip.push({ nomeZip, blob: item.arquivosFiles[j] });
        } else if (item.arquivos?.[j]?.base64) {
          const binary = atob(item.arquivos[j].base64);
          const bytes = new Uint8Array(binary.length);
          for (let i = 0; i < binary.length; i++) bytes[i] = binary.charCodeAt(i);
          arquivosParaZip.push({ nomeZip, blob: new Blob([bytes]) });
        }
      }
      const dataDespesaDate = item.data ? stringParaDate(item.data) : null;
      solicitacoesComNomes.push({
        projetoId: item.projetoId,
        clienteId: item.clienteId,
        verbaId: item.verbaId,
        descricao: item.descricao,
        dataDespesa: dataDespesaDate ? format(dataDespesaDate, "yyyy-MM-dd") : "",
        valor: item.valorTotal,
        valorUnidade: item.tipoCodigo === 2 ? Number(item.valorUnitario) : undefined,
        quantidade: item.tipoCodigo === 2 ? Number(item.quantidade) : undefined,
        unidade: item.tipoCodigo === 2 ? item.unidade : undefined,
        arquivos: nomesArquivos,
      });
    }

    const payloadZip = {
      objetivo: primeiroItem.objetivo.trim(),
      destino: primeiroItem.destino.trim() || "",
      dataInicio: dataInicioFormatada,
      dataFim: dataFimFormatada || dataInicioFormatada,
      solicitacoes: solicitacoesComNomes,
    };

    setEnviando(true);
    try {
      const zip = new JSZip();
      zip.file("dados.json", JSON.stringify(payloadZip));
      for (const { nomeZip, blob } of arquivosParaZip) {
        zip.file(nomeZip, blob);
      }
      const zipBlob = await zip.generateAsync({ type: "blob" });
      const zipFile = new File([zipBlob], "reembolso.zip", { type: "application/zip" });

      const response = await inserirSolicitacaoUseCase.executeZip(token, zipFile);

      if (response.sucesso) {
        setModalSucesso(true);
      } else {
        if (response.erros && Array.isArray(response.erros) && response.erros.length > 0) {
          setErrosApi(response.erros);
          setModalErro(true);
        } else {
          toast({
            title: "Erro ao enviar",
            description: response.mensagem || "Erro ao enviar solicitações",
            variant: "destructive",
          });
        }
      }
    } catch (error) {
      toast({
        title: "Erro",
        description: "Erro ao enviar solicitações",
        variant: "destructive",
      });
    } finally {
      setEnviando(false);
    }
  };

  // Fechar modal e redirecionar
  const handleFecharModalSucesso = () => {
    setModalSucesso(false);
    navigate("/reembolso");
  };

  const totalItens = carrinho.length;
  const valorTotalCarrinho = carrinho.reduce((acc, item) => acc + item.valorTotal, 0);


  return (
    <TooltipProvider>
      <div className="container mx-auto p-4 md:p-6 pb-24 space-y-4">
        <PageBreadcrumb 
          items={[
            { label: 'Reembolso', href: '/reembolso' },
            { label: 'Inserir Reembolso' }
          ]} 
        />
        
        <div className="flex items-center gap-4">
          <Button
            variant="ghost"
            size="icon"
            onClick={() => navigate(-1)}
            className="h-10 w-10"
          >
            <ArrowLeft className="h-5 w-5" />
          </Button>
          <h1 className="page-title">Nova Solicitação</h1>
        </div>

        {/* Seção de Dados Bancários */}
        <DadosBancariosCard />

        <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
          <div className="lg:col-span-2 space-y-6">
            {/* Nova seção no topo - Informações Gerais */}
            <Card className="rounded-2xl shadow-none">
              <CardContent className="p-6 space-y-6">
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div className="space-y-2">
                    <Label htmlFor="objetivo">
                      Objetivo<span className="text-destructive">*</span>
                    </Label>
                    <Input
                      id="objetivo"
                      placeholder="Objetivo"
                      value={objetivo}
                      onChange={(e) => {
                        setObjetivo(e.target.value);
                        setErros((prev) => ({ ...prev, objetivo: false }));
                      }}
                      className={erros.objetivo ? "border-red-500" : ""}
                    />
                  </div>

                  <div className="space-y-2">
                    <Label htmlFor="destino">Destino</Label>
                    <Input
                      id="destino"
                      placeholder="Destino"
                      value={destino}
                      onChange={(e) => setDestino(e.target.value)}
                    />
                  </div>
                </div>

                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div className="space-y-2">
                    <Label htmlFor="dataInicio">
                      Data de início<span className="text-destructive">*</span>
                    </Label>
                    <Popover open={dataInicioPickerOpen} onOpenChange={setDataInicioPickerOpen}>
                      <PopoverTrigger asChild>
                        <Button
                          id="dataInicio"
                          variant="outline"
                          className={cn(
                            "w-full justify-start text-left font-normal",
                            !dataInicio && "text-muted-foreground",
                            erros.dataInicio && "border-red-500"
                          )}
                        >
                          <CalendarIcon className="mr-2 h-4 w-4" />
                          {dataInicio ? (
                            format(dataInicio, "dd/MM/yyyy", { locale: ptBR })
                          ) : (
                            <span>Selecione a data</span>
                          )}
                        </Button>
                      </PopoverTrigger>
                      <PopoverContent className="w-auto p-0" align="start">
                        <Calendar
                          mode="single"
                          selected={dataInicio}
                          onSelect={(d) => {
                            setDataInicio(d);
                            setErros((prev) => ({ ...prev, dataInicio: false }));
                            setDataInicioPickerOpen(false);
                          }}
                          locale={ptBR}
                          initialFocus
                          className="pointer-events-auto"
                        />
                      </PopoverContent>
                    </Popover>
                  </div>

                  <div className="space-y-2">
                    <Label htmlFor="dataFim">Data final</Label>
                    <Popover open={dataFimPickerOpen} onOpenChange={setDataFimPickerOpen}>
                      <PopoverTrigger asChild>
                        <Button
                          id="dataFim"
                          variant="outline"
                          className={cn(
                            "w-full justify-start text-left font-normal",
                            !dataFim && "text-muted-foreground",
                            erros.dataFim && "border-red-500"
                          )}
                        >
                          <CalendarIcon className="mr-2 h-4 w-4" />
                          {dataFim ? (
                            format(dataFim, "dd/MM/yyyy", { locale: ptBR })
                          ) : (
                            <span>Selecione a data</span>
                          )}
                        </Button>
                      </PopoverTrigger>
                      <PopoverContent className="w-auto p-0" align="start">
                        <Calendar
                          mode="single"
                          selected={dataFim}
                          onSelect={(d) => {
                            setDataFim(d);
                            setErros((prev) => ({ ...prev, dataFim: false }));
                            setDataFimPickerOpen(false);
                          }}
                          locale={ptBR}
                          initialFocus
                          className="pointer-events-auto"
                        />
                      </PopoverContent>
                    </Popover>
                  </div>
                </div>
              </CardContent>
            </Card>

            {/* Formulário existente */}
            <Card className="rounded-2xl shadow-none">
              <CardContent className="p-6 space-y-6">
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div className="space-y-2">
                    <Label>
                      Cliente/Projeto
                      {!permitirSolicitacaoSemProjeto && <span className="text-destructive">*</span>}
                    </Label>
                    <Popover open={clienteProjetoOpen} onOpenChange={setClienteProjetoOpen}>
                      <PopoverTrigger asChild>
                        <Button
                          variant="outline"
                          role="combobox"
                          aria-expanded={clienteProjetoOpen}
                          className={cn(
                            "w-full justify-between min-w-0",
                            erros.clienteProjeto && "border-red-500",
                            !clienteProjetoValue && "text-muted-foreground"
                          )}
                          disabled={loadingProjetos}
                        >
                          <span className="truncate text-left flex-1 mr-2 min-w-0">
                            {clienteProjetoValue
                              ? projetos.find(
                                  (p) =>
                                    `${p.codigoProjeto}-${p.codigoCliente}` === clienteProjetoValue
                                )?.projetos || "Selecione o projeto"
                              : loadingProjetos
                              ? "Carregando..."
                              : "Selecione o projeto"}
                          </span>
                          <ChevronsUpDown className="ml-2 h-4 w-4 shrink-0 opacity-50 flex-shrink-0" />
                        </Button>
                      </PopoverTrigger>
                      <PopoverContent className="w-[var(--radix-popover-trigger-width)] p-0" align="start">
                        <Command>
                          <CommandInput placeholder="Buscar cliente/projeto..." />
                          <CommandList>
                            <CommandEmpty>
                              {loadingProjetos ? "Carregando..." : "Nenhum projeto encontrado."}
                            </CommandEmpty>
                            <CommandGroup>
                              {projetos.length > 0 ? (
                                projetos.map((projeto) => {
                                  const value = `${projeto.codigoProjeto}-${projeto.codigoCliente}`;
                                  return (
                                    <CommandItem
                                      key={value}
                                      value={projeto.projetos}
                                      onSelect={() => {
                                        setClienteProjetoValue(value);
                                        setProjetoSelecionado(projeto);
                                        setCategoriaSelecionada(null);
                                        setErros((prev) => ({ ...prev, clienteProjeto: false }));
                                        setClienteProjetoOpen(false);
                                      }}
                                    >
                                      <Check
                                        className={cn(
                                          "mr-2 h-4 w-4",
                                          clienteProjetoValue === value ? "opacity-100" : "opacity-0"
                                        )}
                                      />
                                      {projeto.projetos}
                                    </CommandItem>
                                  );
                                })
                              ) : (
                                <CommandItem value="none" disabled>
                                  Nenhum projeto disponível
                                </CommandItem>
                              )}
                            </CommandGroup>
                          </CommandList>
                        </Command>
                      </PopoverContent>
                    </Popover>
                  </div>

                  <div className="space-y-2">
                    <Label>Categoria<span className="text-destructive">*</span></Label>
                    <Select
                      value={categoriaSelecionada ? String(categoriaSelecionada.verbaId ?? categoriaSelecionada.id ?? "") : ""}
                      onValueChange={(value) => {
                        const verba = verbas.find((v) => String(v.verbaId ?? v.id ?? "") === value);
                        setCategoriaSelecionada(verba || null);
                        if (verba) {
                          // Preencher unidade (readonly)
                          setUnidade(verba.unidade || "");
                          // Preencher valor unitário (readonly)
                          const valorUnitarioStr = verba.valor?.toString() || "0";
                          setValorUnitario(valorUnitarioStr);
                          // Se for tipoCodigo = 2, calcular valor automaticamente (quantidade * valorUnitario)
                          if (verba.tipoCodigo === 2) {
                            const novoValorTotal = Number(quantidade) * Number(valorUnitarioStr);
                            setValor(novoValorTotal > 0 ? novoValorTotal.toString() : "");
                            validarValorMaximo(valorUnitarioStr);
                          } else {
                            // Se for tipoCodigo = 1, limpar o campo valor (deve nascer zerado)
                            // Mas se houver valorParaRestaurar (edição), restaurar
                            if (valorParaRestaurar !== null) {
                              const valorFormatado = valorParaRestaurar ? formatarValorNumerico(valorParaRestaurar) : "";
                              setValor(valorFormatado);
                              setValorParaRestaurar(null);
                            } else {
                              setValor("");
                            }
                          }
                          // Limpar valorParaRestaurar quando categoria for selecionada manualmente
                          setValorParaRestaurar(null);
                        } else {
                          // Limpar campos quando categoria for desmarcada
                          setValor("");
                          setValorUnitario("0");
                          setUnidade("");
                        }
                        setErros((prev) => ({ ...prev, categoria: false }));
                      }}
                      disabled={loadingVerbas}
                    >
                      <SelectTrigger className={erros.categoria ? "border-red-500" : ""}>
                        <SelectValue
                          placeholder={
                            loadingVerbas
                              ? "Carregando..."
                              : "Selecione a categoria"
                          }
                        />
                      </SelectTrigger>
                      <SelectContent>
                        {verbas.length > 0 ? (
                          verbas.map((verba, index) => {
                            const verbaKey = verba.verbaId ?? verba.id ?? index;
                            return (
                              <SelectItem key={verbaKey} value={String(verbaKey)}>
                                {verba.categoria}
                              </SelectItem>
                            );
                          })
                        ) : (
                          <SelectItem value="none" disabled>
                            Nenhuma categoria disponível
                          </SelectItem>
                        )}
                      </SelectContent>
                    </Select>
                  </div>
                </div>

                <div className="space-y-2">
                  <div className="flex items-center gap-2">
                    <Label>
                      Comprovantes
                      {categoriaSelecionada?.exigirComprovante && (
                        <span className="text-destructive">*</span>
                      )}
                    </Label>
                    {loadingAnalise && (
                      <Loader2 className="h-4 w-4 animate-spin text-muted-foreground" />
                    )}
                  </div>
                  <div
                    className={cn(
                      "border-2 border-dashed rounded-lg p-6 hover:border-primary/50 transition-colors",
                      erros.comprovante ? "border-red-500" : "border-border"
                    )}
                  >
                    <input
                      ref={fileInputRef}
                      type="file"
                      multiple
                      accept=".png,.jpg,.jpeg,.pdf,image/png,image/jpeg,application/pdf"
                      onChange={async (e) => {
                        const files = Array.from(e.target.files || []);
                        if (files.length === 0) return;

                        const formatosPermitidos = ["image/png", "image/jpeg", "image/jpg", "application/pdf"];
                        const arquivosValidos = files.filter((file) => {
                          const tipo = file.type.toLowerCase();
                          const extensao = file.name.split(".").pop()?.toLowerCase();
                          return (
                            formatosPermitidos.includes(tipo) ||
                            (extensao && ["png", "jpg", "jpeg", "pdf"].includes(extensao))
                          );
                        });

                        if (arquivosValidos.length !== files.length) {
                          toast({
                            title: "Formato inválido",
                            description: "Apenas arquivos PNG, JPG e PDF são permitidos.",
                            variant: "destructive",
                          });
                        }

                        if (arquivosValidos.length > 0) {
                          setAnexos((prev) => [...prev, ...arquivosValidos]);
                          setErros((prev) => ({ ...prev, comprovante: false }));
                          // Analisar comprovantes (OCR)
                          await analisarComprovantes(arquivosValidos);
                        }

                        if (e.target) {
                          e.target.value = "";
                        }
                      }}
                      className="hidden"
                      id="file-upload"
                    />
                    <label htmlFor="file-upload" className="cursor-pointer block">
                      <div className="text-center py-4">
                        <Upload className="mx-auto h-12 w-12 text-muted-foreground mb-4" />
                        <p className="text-foreground font-medium mb-1">Clique ou arraste os comprovantes</p>
                        <p className="text-sm text-muted-foreground">Apenas PNG, JPG e PDF até 10MB cada</p>
                      </div>
                    </label>

                    {anexos.length > 0 && (
                      <div className="mt-4 space-y-2 border-t pt-4">
                        <p className="text-sm font-medium text-foreground mb-2">Arquivos selecionados:</p>
                        <div className="space-y-2">
                          {anexos.map((arquivo, index) => (
                            <div
                              key={index}
                              className="flex items-center justify-between p-2 bg-muted/50 rounded-lg hover:bg-muted transition-colors"
                            >
                              <div className="flex items-center gap-2 flex-1 min-w-0">
                                <Paperclip className="h-4 w-4 text-muted-foreground shrink-0" />
                                <span className="text-sm text-foreground truncate" title={arquivo.name}>
                                  {arquivo.name}
                                </span>
                                <span className="text-xs text-muted-foreground shrink-0">
                                  ({(arquivo.size / 1024 / 1024).toFixed(2)} MB)
                                </span>
                              </div>
                              <Button
                                variant="ghost"
                                size="icon"
                                className="h-8 w-8 text-destructive hover:text-destructive hover:bg-destructive/10 shrink-0"
                                onClick={(e) => {
                                  e.preventDefault();
                                  e.stopPropagation();
                                  setAnexos((prev) => prev.filter((_, i) => i !== index));
                                  setAnexosBase64((prev) => prev.filter((_, i) => i !== index));
                                }}
                              >
                                <Trash2 className="h-4 w-4" />
                              </Button>
                            </div>
                          ))}
                        </div>
                      </div>
                    )}
                  </div>
                </div>

                <div className="space-y-2">
                  <div className="flex items-center gap-2">
                    <Label>Data da Despesa<span className="text-destructive">*</span></Label>
                    {comprovanteExcedido && date && (
                      <Tooltip>
                        <TooltipTrigger asChild>
                          <AlertTriangle className="h-4 w-4 text-amber-500" />
                        </TooltipTrigger>
                        <TooltipContent>
                          <p>A data do comprovante excedeu o limite permitido de {validadeComprovanteDias} dia(s). Foram excedidos {diasExcedidos} dia(s).</p>
                        </TooltipContent>
                      </Tooltip>
                    )}
                  </div>
                  <Popover open={dataDespesaPickerOpen} onOpenChange={setDataDespesaPickerOpen}>
                    <PopoverTrigger asChild>
                      <Button
                        variant="outline"
                        className={cn(
                          "w-full justify-start text-left font-normal",
                          !date && "text-muted-foreground",
                          erros.data && "border-red-500",
                          comprovanteExcedido && date && "border-amber-500"
                        )}
                      >
                        <CalendarIcon className="mr-2 h-4 w-4" />
                        {date ? (
                          format(date, "dd/MM/yyyy", { locale: ptBR })
                        ) : (
                          <span>Selecione a data</span>
                        )}
                      </Button>
                    </PopoverTrigger>
                    <PopoverContent className="w-auto p-0" align="start">
                      <Calendar
                        mode="single"
                        selected={date}
                        onSelect={(d) => {
                          setDate(d);
                          setErros((prev) => ({ ...prev, data: false }));
                          if (d) {
                            validarDataComprovante(d);
                          } else {
                            setComprovanteExcedido(false);
                            setDiasExcedidos(0);
                          }
                          setDataDespesaPickerOpen(false);
                        }}
                        locale={ptBR}
                        initialFocus
                        className="pointer-events-auto"
                      />
                    </PopoverContent>
                  </Popover>
                </div>

                {/* Campos condicionais: mostrar apenas se tipoCodigo = 2 */}
                {mostrarCamposQuantidadeValor && (
                  <div className="grid grid-cols-3 gap-4">
                    <div className="space-y-2">
                      <Label>Quantidade<span className="text-destructive">*</span></Label>
                      <Input
                        type="number"
                        value={quantidade}
                        onChange={(e) => {
                          setQuantidade(e.target.value);
                          setErros((prev) => ({ ...prev, quantidade: false }));
                          // Quando tipoCodigo = 2, recalcular valor total automaticamente
                          if (categoriaSelecionada?.tipoCodigo === 2) {
                            const novaQuantidade = Number(e.target.value) || 0;
                            const novoValorTotal = novaQuantidade * Number(valorUnitario);
                            setValor(novoValorTotal > 0 ? formatarValorNumerico(novoValorTotal) : "");
                          }
                        }}
                        className={erros.quantidade ? "border-red-500" : ""}
                        min="1"
                      />
                    </div>

                    <div className="space-y-2">
                      <Label>Unidade</Label>
                      <Input
                        value={unidade}
                        readOnly
                        disabled
                        className="bg-muted"
                      />
                    </div>

                    <div className="space-y-2">
                      <div className="flex items-center gap-2">
                        <Label>Valor Unitário<span className="text-destructive">*</span></Label>
                        {valorExcedido && (
                          <Tooltip>
                            <TooltipTrigger asChild>
                              <AlertTriangle className="h-4 w-4 text-amber-500" />
                            </TooltipTrigger>
                            <TooltipContent>
                              <p>O valor máximo permitido para este reembolso é de R$ {categoriaSelecionada?.valor?.toFixed(2)}.</p>
                            </TooltipContent>
                          </Tooltip>
                        )}
                      </div>
                      <div className="relative">
                        <span className="absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground">R$</span>
                        <Input
                          type="number"
                          value={valorUnitario}
                          readOnly
                          disabled
                          className={cn("pl-10 bg-muted", erros.valor && "border-red-500", valorExcedido && "border-amber-500")}
                          step="0.01"
                          min="0"
                        />
                      </div>
                    </div>
                  </div>
                )}

                {/* Campo Valor (sempre visível quando não é tipoCodigo = 2) */}
                {!mostrarCamposQuantidadeValor && (
                  <div className="space-y-2">
                    <div className="flex items-center gap-2">
                      <Label>Valor<span className="text-destructive">*</span></Label>
                      {valorExcedido && (
                        <Tooltip>
                          <TooltipTrigger asChild>
                            <AlertTriangle className="h-4 w-4 text-amber-500" />
                          </TooltipTrigger>
                          <TooltipContent>
                            <p>O valor máximo permitido para este reembolso é de R$ {categoriaSelecionada?.valor?.toFixed(2)}.</p>
                          </TooltipContent>
                        </Tooltip>
                      )}
                    </div>
                    <div className="relative">
                      <span className="absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground">R$</span>
                      <Input
                        type="text"
                        value={valor}
                        onChange={(e) => {
                          const valorFormatado = formatarValorMonetario(e.target.value);
                          setValor(valorFormatado);
                          setErros((prev) => ({ ...prev, valor: false }));
                          // Validar se excede o teto (para tipoCodigo = 1)
                          if (categoriaSelecionada?.tipoCodigo === 1) {
                            const valorNumerico = desformatarValorMonetario(valorFormatado);
                            validarValorMaximo(valorNumerico);
                          }
                        }}
                        className={cn("pl-10", erros.valor && "border-red-500", valorExcedido && "border-amber-500")}
                        placeholder="0,00"
                      />
                    </div>
                  </div>
                )}

                <div className="bg-green-50 dark:bg-green-950/20 border border-green-200 dark:border-green-800 rounded-lg p-4 flex items-center justify-between">
                  <div className="flex items-center gap-2">
                    <Calculator className="h-5 w-5 text-green-600 dark:text-green-400" />
                    <span className="font-semibold text-green-700 dark:text-green-300">Valor Total:</span>
                    {valorTotalExcedido && (
                      <Tooltip>
                        <TooltipTrigger asChild>
                          <AlertTriangle className="h-4 w-4 text-amber-500" />
                        </TooltipTrigger>
                        <TooltipContent>
                          <p>O valor total foi excedido. O teto permitido é de R$ {categoriaSelecionada?.valor ? formatarValorParaExibicao(categoriaSelecionada.valor) : '0,00'}.</p>
                        </TooltipContent>
                      </Tooltip>
                    )}
                  </div>
                  <span className="text-2xl font-bold text-green-600 dark:text-green-400">
                    R$ {formatarValorParaExibicao(valorTotal)}
                  </span>
                </div>

                <div className="space-y-2">
                  <Label>Descrição<span className="text-destructive">*</span></Label>
                  <Textarea
                    placeholder="Descreva a despesa (ex: Almoço de negócios com cliente)"
                    value={descricao}
                    onChange={(e) => {
                      setDescricao(e.target.value);
                      setErros((prev) => ({ ...prev, descricao: false }));
                    }}
                    className={erros.descricao ? "border-red-500" : ""}
                    rows={4}
                  />
                </div>

                <div className="flex gap-4 pt-4">
                  <Button variant="outline" className="flex-1" onClick={handleLimpar}>
                    Limpar
                  </Button>
                  <Button className="flex-1" onClick={handleAdicionarCarrinho}>
                    {itemEditando !== null ? "Atualizar Item" : "Adicionar ao Carrinho"}
                  </Button>
                </div>
              </CardContent>
            </Card>
          </div>

          <div>
            <Card className="rounded-2xl shadow-none">
              <CardContent className="p-6">
                <div className="flex items-center gap-2 mb-6">
                  <ShoppingCart className="h-5 w-5" />
                  <h2 className="text-lg font-semibold">Carrinho de Solicitações</h2>
                </div>

                {carrinho.length === 0 ? (
                  <div className="text-center py-12">
                    <ShoppingCart className="h-16 w-16 mx-auto text-muted-foreground mb-4" />
                    <p className="text-muted-foreground">Nenhuma solicitação no carrinho</p>
                  </div>
                ) : (
                  <>
                    <div className="space-y-4 mb-6">
                      {carrinho.map((item, index) => (
                        <Card key={index} className="hover:shadow-md transition-all duration-200 border-border/50 bg-surfaceElevated">
                          <CardContent className="p-5">
                            <div className="flex items-start justify-between gap-4">
                              <div className="flex-1 space-y-3">
                                {/* Header do card */}
                                <div className="flex items-start justify-between gap-3">
                                  <div className="flex-1">
                                    <div className="flex items-center gap-2 mb-2">
                                      <h3 className="font-semibold text-base text-primaryText">{item.categoria}</h3>
                                      {item.comprovanteExcedido && (
                                        <Tooltip>
                                          <TooltipTrigger asChild>
                                            <AlertTriangle className="h-4 w-4 text-amber-500 shrink-0" />
                                          </TooltipTrigger>
                                          <TooltipContent>
                                            <p>Data do comprovante excedida</p>
                                          </TooltipContent>
                                        </Tooltip>
                                      )}
                                    </div>
                                    {item.data && (
                                      <div className="flex items-center gap-2 text-xs text-secondaryText mb-2">
                                        <span className="px-2 py-0.5 bg-muted/50 rounded-md">{item.data}</span>
                                      </div>
                                    )}
                                  </div>
                                  <div className="flex items-center gap-1">
                                    <Button
                                      variant="ghost"
                                      size="icon"
                                      className="h-8 w-8 hover:bg-primarySoft"
                                      onClick={() => handleEditarItem(index)}
                                    >
                                      <Edit className="h-4 w-4" />
                                    </Button>
                                    <Button
                                      variant="ghost"
                                      size="icon"
                                      className="h-8 w-8 text-destructive hover:text-destructive hover:bg-destructive/10"
                                      onClick={() => handleExcluirItem(index)}
                                    >
                                      <Trash2 className="h-4 w-4" />
                                    </Button>
                                  </div>
                                </div>

                                {/* Descrição */}
                                {item.descricao && (
                                  <p className="text-sm text-secondaryText line-clamp-2">{item.descricao}</p>
                                )}

                                {/* Footer do card */}
                                <div className="flex items-center justify-between pt-2 border-t border-border/50">
                                  <div className="flex items-center gap-4">
                                    <div>
                                      <span className="text-xs text-secondaryText">Valor</span>
                                      <p className="text-lg font-bold text-green-600 dark:text-green-400">
                                        R$ {formatarValorParaExibicao(item.valorTotal)}
                                      </p>
                                    </div>
                                    {item.tipoCodigo === 2 && (
                                      <div className="text-xs text-secondaryText">
                                        <span className="block">Qtd: {item.quantidade}</span>
                                        <span className="block">R$ {formatarValorParaExibicao(Number(item.valorUnitario))} un.</span>
                                      </div>
                                    )}
                                  </div>
                                  {item.arquivos.length > 0 && (
                                    <div className="flex items-center gap-1.5 px-2.5 py-1.5 bg-primarySoft/50 rounded-md">
                                      <Paperclip className="h-3.5 w-3.5 text-primary" />
                                      <span className="text-xs font-medium text-primary">{item.arquivos.length}</span>
                                    </div>
                                  )}
                                </div>
                              </div>
                            </div>
                          </CardContent>
                        </Card>
                      ))}
                    </div>

                    <div className="space-y-3 mb-6">
                      <div className="bg-muted/50 rounded-lg p-3 flex items-center justify-between">
                        <span className="text-sm text-foreground">Total de itens:</span>
                        <span className="text-lg font-bold text-foreground">{totalItens}</span>
                      </div>
                      <div className="bg-muted/50 rounded-lg p-3 flex items-center justify-between">
                        <span className="text-sm text-foreground">Valor total:</span>
                        <span className="text-lg font-bold text-green-600 dark:text-green-400">
                          R$ {formatarValorParaExibicao(valorTotalCarrinho)}
                        </span>
                      </div>
                    </div>

                    <Button
                      className="w-full"
                      onClick={handleEnviarSolicitacoes}
                      disabled={enviando}
                    >
                      {enviando ? (
                        <>
                          <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                          Enviando...
                        </>
                      ) : (
                        <>
                          <Send className="mr-2 h-4 w-4" />
                          Enviar solicitações
                        </>
                      )}
                    </Button>
                  </>
                )}
              </CardContent>
            </Card>
          </div>
        </div>

        {/* Modal de Sucesso */}
        <AlertDialog open={modalSucesso} onOpenChange={setModalSucesso}>
          <AlertDialogContent>
            <AlertDialogHeader>
              <AlertDialogTitle>Sucesso!</AlertDialogTitle>
              <AlertDialogDescription>
                Reembolso enviado com sucesso!
              </AlertDialogDescription>
            </AlertDialogHeader>
            <AlertDialogFooter>
              <AlertDialogAction onClick={handleFecharModalSucesso}>
                OK
              </AlertDialogAction>
            </AlertDialogFooter>
          </AlertDialogContent>
        </AlertDialog>


        {/* Modal de Erro */}
        <Dialog open={modalErro} onOpenChange={setModalErro}>
          <DialogContent>
            <DialogHeader>
              <DialogTitle className="flex items-center gap-2 text-destructive">
                <AlertTriangle className="h-5 w-5" />
                Erro ao enviar solicitações
              </DialogTitle>
              <DialogDescription>
                Ocorreram os seguintes erros ao processar sua solicitação:
              </DialogDescription>
            </DialogHeader>
            <div className="py-4">
              <ul className="list-disc list-inside space-y-2">
                {errosApi.map((erro, index) => (
                  <li key={index} className="text-sm text-foreground">
                    {erro}
                  </li>
                ))}
              </ul>
            </div>
            <DialogFooter>
              <Button onClick={() => setModalErro(false)}>
                Fechar
              </Button>
            </DialogFooter>
          </DialogContent>
        </Dialog>
      </div>
    </TooltipProvider>
  );
}
