import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import * as z from "zod";
import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { toast } from "sonner";
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import {
  MapPin,
  Building2,
  Award,
  Upload,
  AlertTriangle,
  X,
  Sparkles,
  Loader2,
} from "@/components/ui/system-icons";
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from "@/components/ui/alert-dialog";
import { useAppSelector, useAppDispatch } from "../../app/store/hooks";
import { updateQuestionariosPreenchidos } from "../../app/store/slices/authSlice";
import { container } from "@core/di/container";
import { ColetaPerfilColaboradorCampanhaUseCase } from "@domain/usecases/ColetaPerfilColaboradorCampanhaUseCase";
import type { ColetaPerfilColaboradorCampanhaRequest } from "@domain/entities/PerfilColaboradorCampanha";
import { getArquivoTipo, fileToBase64 } from "@shared/utils/fileUtils";
import { formatarDataInput, formatPhoneByCountry, formatPostalCodeByCountry, dateToISOWithZeroTime, getCurrentDateWithZeroTime, validateDate, extractDDIFromPrefix, cleanPhoneNumber } from "@shared/utils/formatUtils";
import { getAddressConfig } from "@shared/utils/addressUtils";
import { useViaCep } from "@presentation/hooks/useViaCep";

// Schema Zod com todos os campos obrigatórios
const formSchema = z.object({
  // Endereço
  codigoPostal: z.string({ required_error: "CEP é obrigatório" }).min(1, "CEP é obrigatório"),
  endereco: z.string({ required_error: "Endereço é obrigatório" }).min(1, "Endereço é obrigatório"),
  numero: z.string({ required_error: "Número é obrigatório" }).min(1, "Número é obrigatório"),
  bairro: z.string().optional(), // Opcional porque Estados Unidos não usa bairro
  cidade: z.string({ required_error: "Cidade é obrigatória" }).min(1, "Cidade é obrigatória"),
  estado: z.string({ required_error: "Estado é obrigatório" }).min(1, "Estado é obrigatório"),
  complemento: z.string().optional(),
  telefone: z.string({ required_error: "Telefone é obrigatório" }).min(1, "Telefone é obrigatório"),
  comprovanteResidencia: z.custom<File>(
    (val) => val instanceof File,
    { message: "Comprovante de residência é obrigatório" }
  ),

  // Forma de Atuação
  modeloTrabalho: z.string({ required_error: "Modelo de trabalho é obrigatório" }).min(1, "Modelo de trabalho é obrigatório"),
  frequencia: z.string().optional(),
  frequenciaId: z.number().optional(),
  diasSemana: z.array(z.string()).optional(),
  localTrabalho: z.string().optional(), // Opcional - obrigatório apenas para presencial/híbrido
  clienteNome: z.string().optional(),
  clienteEndereco: z.string().optional(),

  // PCD
  pcd: z.string({ required_error: "PCD é obrigatório" }).min(1, "PCD é obrigatório"),

  // Dados Pessoais
  linkedin: z.string().url("URL do LinkedIn inválida").optional().or(z.literal("")).nullable(),
});

type FormData = z.infer<typeof formSchema>;

export default function AtualizarPerfil() {
  const navigate = useNavigate();
  const dispatch = useAppDispatch();
  const { token } = useAppSelector((state) => state.auth);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [showHeader] = useState(true);

  // Estados do formulário
  const [uploadedFile, setUploadedFile] = useState<string | null>(null);
  const [cepMismatch] = useState(false);
  const [paisSelecionado, setPaisSelecionado] = useState<string>("brasil");
  const [telefone, setTelefone] = useState<string>("");
  const [codigoPostal, setCodigoPostal] = useState<string>("");
  const [endereco, setEndereco] = useState<string>("");
  const [numero, setNumero] = useState<string>("");
  const [bairro, setBairro] = useState<string>("");
  const [complemento, setComplemento] = useState<string>("");
  const [cidade, setCidade] = useState<string>("");
  const [estado, setEstado] = useState<string>("");
  const [comprovanteFile, setComprovanteFile] = useState<File | null>(null);

  const [pcdResposta, setPcdResposta] = useState<string>("");

  const [modeloTrabalho, setModeloTrabalho] = useState<string>("");
  const [frequencia, setFrequencia] = useState<string>("");
  const [frequenciaId, setFrequenciaId] = useState<number>(0);
  const [diasSemana, setDiasSemana] = useState<string[]>([]);
  const [localTrabalho, setLocalTrabalho] = useState<string>("");
  const [clienteBusca, setClienteBusca] = useState("");
  const [clienteSelecionado, setClienteSelecionado] = useState(false);

  const [possuiCertificadoAWS, setPossuiCertificadoAWS] = useState<string>("");
  const [certificadosAWS, setCertificadosAWS] = useState<{
    technical: {
      status: string;
      file: File | null;
      nome: string;
      dataEmissao: string;
      emissor: string;
      cargaHoraria: string;
      previsaoConclusao: string;
    };
    foundational: {
      status: string;
      file: File | null;
      nome: string;
      dataEmissao: string;
      emissor: string;
      cargaHoraria: string;
      previsaoConclusao: string;
    };
    accredited: {
      status: string;
      file: File | null;
      nome: string;
      dataEmissao: string;
      emissor: string;
      cargaHoraria: string;
      previsaoConclusao: string;
    };
  }>({
    technical: {
      status: "",
      file: null,
      nome: "",
      dataEmissao: "",
      emissor: "",
      cargaHoraria: "",
      previsaoConclusao: "",
    },
    foundational: {
      status: "",
      file: null,
      nome: "",
      dataEmissao: "",
      emissor: "",
      cargaHoraria: "",
      previsaoConclusao: "",
    },
    accredited: {
      status: "",
      file: null,
      nome: "",
      dataEmissao: "",
      emissor: "",
      cargaHoraria: "",
      previsaoConclusao: "",
    },
  });

  const [linkedin, setLinkedin] = useState<string>("");
  const [showErrorDialog, setShowErrorDialog] = useState(false);
  const [errorMessage, setErrorMessage] = useState<string>("");
  const [errorList, setErrorList] = useState<string[]>([]);

  const form = useForm<FormData>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      diasSemana: [],
    },
  });

  const handlePhoneChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const formatted = formatPhoneByCountry(e.target.value, paisSelecionado);
    setTelefone(formatted);
  };

  // Hook para buscar CEP via ViaCEP
  const { buscarCep, limpar: limparCep, loading: loadingCep, error: errorCep, endereco: enderecoViaCep } = useViaCep();

  const handlePostalCodeChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const formatted = formatPostalCodeByCountry(e.target.value, paisSelecionado);
    setCodigoPostal(formatted);
  };

  // Buscar CEP automaticamente quando o código postal mudar e o país for Brasil
  useEffect(() => {
    const cepLimpo = codigoPostal.replace(/\D/g, '');
    
    // Só busca se o país for Brasil e o CEP tiver 8 dígitos
    if (paisSelecionado === 'brasil' && cepLimpo.length === 8 && !loadingCep) {
      buscarCep(codigoPostal);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [codigoPostal, paisSelecionado]);

  // Limpar dados do ViaCEP quando o país mudar para algo diferente de Brasil
  useEffect(() => {
    if (paisSelecionado !== 'brasil') {
      limparCep();
    }
  }, [paisSelecionado, limparCep]);

  // Preencher campos automaticamente quando o endereço for encontrado
  useEffect(() => {
    if (enderecoViaCep && paisSelecionado === 'brasil') {
      setEndereco(enderecoViaCep.endereco);
      setBairro(enderecoViaCep.bairro);
      setCidade(enderecoViaCep.cidade);
      setEstado(enderecoViaCep.estado);
      
      // Atualizar os campos do formulário
      form.setValue('endereco', enderecoViaCep.endereco);
      form.setValue('bairro', enderecoViaCep.bairro);
      form.setValue('cidade', enderecoViaCep.cidade);
      form.setValue('estado', enderecoViaCep.estado);
    }
  }, [enderecoViaCep, paisSelecionado, form]);

  const { clienteCampanha: clientesList, escritoriosEnderecos, frequenciaMap } = useAppSelector((state) => state.clienteCampanha);
  const addressConfig = getAddressConfig(paisSelecionado);
  const clientesFiltrados = clientesList.filter((cliente) =>
    cliente.nome.toLowerCase().includes(clienteBusca.toLowerCase())
  );
  const clienteSelecionadoData = clientesList.find((c) => c.nome === clienteBusca);

  const handleSubmit = async (_data: FormData) => {
    // _data é validado pelo Zod antes de chegar aqui - usando underscore para indicar que não é usado diretamente
    if (!token) {
      toast.error("Token não encontrado. Faça login novamente.");
      return;
    }

    // Validação de campos obrigatórios
    const errosCamposObrigatorios: string[] = [];
    
    // Validação de Bairro/Freguesia (obrigatório para Brasil e Portugal)
    if ((paisSelecionado === "brasil" || paisSelecionado === "portugal") && (!bairro || bairro.trim() === "")) {
      const labelBairro = paisSelecionado === "brasil" ? "Bairro" : "Freguesia";
      errosCamposObrigatorios.push(`${labelBairro} é obrigatório`);
    }
    
    if (!pcdResposta || pcdResposta.trim() === "") {
      errosCamposObrigatorios.push("PCD é obrigatório");
    }
    
    // Validação opcional do LinkedIn - apenas se preenchido
    if (linkedin && linkedin.trim() !== "" && !linkedin.match(/^https?:\/\/.+/)) {
      errosCamposObrigatorios.push("URL do LinkedIn inválida. Deve começar com http:// ou https://");
    }
    
    if (!possuiCertificadoAWS || possuiCertificadoAWS.trim() === "") {
      errosCamposObrigatorios.push("Certificação AWS é obrigatória");
    }

    if (errosCamposObrigatorios.length > 0) {
      toast.error("Preencha todos os campos obrigatórios");
      setErrorMessage("Campos obrigatórios não preenchidos");
      setErrorList(errosCamposObrigatorios);
      setShowErrorDialog(true);
      return;
    }

    // Validação de Forma de Atuação
    const errosFormaAtuacao: string[] = [];
    
    if (!modeloTrabalho || modeloTrabalho.trim() === "") {
      errosFormaAtuacao.push("Modelo de trabalho é obrigatório");
    }

    if (modeloTrabalho === "hibrido") {
      if (!frequenciaId || frequenciaId === 0) {
        errosFormaAtuacao.push("Frequência no escritório é obrigatória para modelo híbrido");
      }
      if (!localTrabalho || localTrabalho.trim() === "") {
        errosFormaAtuacao.push("Local de trabalho é obrigatório para modelo híbrido");
      }
      if (localTrabalho === "cliente" && !clienteSelecionado) {
        errosFormaAtuacao.push("Selecione um cliente quando o local de trabalho for 'Cliente'");
      }
      if (!diasSemana || diasSemana.length === 0) {
        errosFormaAtuacao.push("Selecione pelo menos um dia da semana para modelo híbrido");
      }
      if (frequenciaId > 0 && diasSemana.length < frequenciaId) {
        errosFormaAtuacao.push(`Selecione ${frequenciaId} dia${frequenciaId > 1 ? "s" : ""} da semana conforme a frequência selecionada`);
      }
    }

    if (modeloTrabalho === "presencial") {
      if (!localTrabalho || localTrabalho.trim() === "") {
        errosFormaAtuacao.push("Local de trabalho é obrigatório para modelo presencial");
      }
      if (localTrabalho === "cliente" && !clienteSelecionado) {
        errosFormaAtuacao.push("Selecione um cliente quando o local de trabalho for 'Cliente'");
      }
    }

    if (errosFormaAtuacao.length > 0) {
      toast.error("Preencha todos os campos obrigatórios da Forma de Atuação");
      setErrorMessage("Campos obrigatórios não preenchidos na Forma de Atuação");
      setErrorList(errosFormaAtuacao);
      setShowErrorDialog(true);
      return;
    }

    // Validação de certificados AWS quando status é "concluido"
    if (possuiCertificadoAWS === "sim") {
      const certificadosComErro: string[] = [];
      
      const certificadosParaValidar = [
        { key: "technical" as const, label: "Technical Certified" },
        { key: "foundational" as const, label: "Foundational Certified" },
        { key: "accredited" as const, label: "Accredited Individuals" },
      ];

      certificadosParaValidar.forEach((cert) => {
        const certData = certificadosAWS[cert.key];
        if (certData.status === "concluido") {
          const erros: string[] = [];
          
          if (!certData.file) {
            erros.push(`Upload do certificado é obrigatório para ${cert.label}`);
          }
          if (!certData.nome || certData.nome.trim() === "") {
            erros.push(`Nome do certificado é obrigatório para ${cert.label}`);
          }
          if (!certData.dataEmissao || certData.dataEmissao.trim() === "") {
            erros.push(`Data de emissão é obrigatória para ${cert.label}`);
          } else if (!validateDate(certData.dataEmissao)) {
            erros.push(`Data de emissão inválida para ${cert.label}. Use o formato dd/mm/aaaa`);
          }
          if (!certData.emissor || certData.emissor.trim() === "") {
            erros.push(`Nome do emissor é obrigatório para ${cert.label}`);
          }
          if (!certData.cargaHoraria || certData.cargaHoraria.trim() === "") {
            erros.push(`Carga horária é obrigatória para ${cert.label}`);
          }
          
          if (erros.length > 0) {
            certificadosComErro.push(...erros);
          }
        }
        
        // Validar previsão de conclusão se status é "cursando"
        if (certData.status === "cursando" && certData.previsaoConclusao && certData.previsaoConclusao.trim() !== "") {
          if (!validateDate(certData.previsaoConclusao)) {
            certificadosComErro.push(`Previsão de conclusão inválida para ${cert.label}. Use o formato dd/mm/aaaa`);
          }
        }
      });

      if (certificadosComErro.length > 0) {
        toast.error("Preencha todos os campos obrigatórios dos certificados AWS selecionados como 'Concluído'");
        setErrorMessage("Campos obrigatórios não preenchidos nos certificados AWS");
        setErrorList(certificadosComErro);
        setShowErrorDialog(true);
        return;
      }
    }

    // Validações movidas para o useCase
    const useCase = container.resolve(ColetaPerfilColaboradorCampanhaUseCase);
    const validationError = useCase.validate({
      paisSelecionado,
      bairro,
      modeloTrabalho,
      localTrabalho,
      clienteSelecionado,
      frequencia,
      frequenciaId,
      diasSemana,
      possuiCertificadoAWS,
      certificadosAWS: {
        technical: { status: certificadosAWS.technical.status },
        foundational: { status: certificadosAWS.foundational.status },
        accredited: { status: certificadosAWS.accredited.status },
      },
      comprovanteFile,
    });

    if (validationError) {
      toast.error(validationError.message);
      return;
    }

    setIsSubmitting(true);

    try {
      // Converter arquivos para base64
      if (!comprovanteFile) {
        toast.error("É necessário anexar o comprovante de residência.");
        setIsSubmitting(false);
        return;
      }

      const comprovanteBase64 = await fileToBase64(comprovanteFile);
      const comprovanteTipo = getArquivoTipo(comprovanteFile.name);

      // Função auxiliar para preparar certificado AWS
      const prepararCertificadoAWS = async (certData: {
        status: string;
        file: File | null;
        nome: string;
        dataEmissao: string;
        emissor: string;
        cargaHoraria: string;
        previsaoConclusao: string;
      }) => {
        // Se não tem status, retorna null
        if (!certData.status || certData.status.trim() === "") {
          return null;
        }

        // Se status é "concluido", precisa ter arquivo
        if (certData.status === "concluido") {
          if (certData.file) {
            return {
              arquivoBase64: {
                base64: await fileToBase64(certData.file),
                tipo: getArquivoTipo(certData.file.name),
              },
              root: {
                status: certData.status.charAt(0).toUpperCase() + certData.status.slice(1).toLowerCase(),
                filePath: "",
                nome: certData.nome || "",
                dataEmissao: certData.dataEmissao
                  ? dateToISOWithZeroTime(certData.dataEmissao)
                  : getCurrentDateWithZeroTime(),
                emissor: certData.emissor || "",
                cargaHoraria: parseInt(certData.cargaHoraria) || 0,
                previsaoConclusao: certData.previsaoConclusao
                  ? dateToISOWithZeroTime(certData.previsaoConclusao)
                  : getCurrentDateWithZeroTime(),
              },
            };
          }
          // Se está concluído mas não tem arquivo, retorna null (não deveria acontecer devido à validação)
          return null;
        }

        // Para status "cursando" ou "interesse", envia apenas o status e previsão se houver
        return {
          arquivoBase64: { base64: "", tipo: 1 },
          root: {
            status: certData.status.charAt(0).toUpperCase() + certData.status.slice(1).toLowerCase(),
            filePath: "",
            nome: "",
            dataEmissao: getCurrentDateWithZeroTime(),
            emissor: "",
            cargaHoraria: 0,
            previsaoConclusao: certData.previsaoConclusao && certData.previsaoConclusao.trim() !== ""
              ? dateToISOWithZeroTime(certData.previsaoConclusao)
              : getCurrentDateWithZeroTime(),
          },
        };
      };

      // Preparar certificações AWS
      const awsCertificacoes = {
        technical: await prepararCertificadoAWS(certificadosAWS.technical),
        foundational: await prepararCertificadoAWS(certificadosAWS.foundational),
        accredited: await prepararCertificadoAWS(certificadosAWS.accredited),
      };

      // Função auxiliar para garantir que sempre envie o status quando existir
      const garantirCertificadoComStatus = (
        certificadoPreparado: any,
        certData: { status: string; previsaoConclusao: string }
      ) => {
        // Se o certificado foi preparado (tem status), usa ele
        if (certificadoPreparado) {
          return certificadoPreparado;
        }
        
        // Se não foi preparado mas tem status, cria objeto com o status
        if (certData.status && certData.status.trim() !== "") {
          return {
            arquivoBase64: { base64: "", tipo: 1 },
            root: {
              status: certData.status.charAt(0).toUpperCase() + certData.status.slice(1).toLowerCase(),
              filePath: "",
              nome: "",
              dataEmissao: getCurrentDateWithZeroTime(),
              emissor: "",
              cargaHoraria: 0,
              previsaoConclusao: certData.previsaoConclusao && certData.previsaoConclusao.trim() !== ""
                ? dateToISOWithZeroTime(certData.previsaoConclusao)
                : getCurrentDateWithZeroTime(),
            },
          };
        }
        
        // Se não tem status, retorna objeto vazio
        return {
          arquivoBase64: { base64: "", tipo: 1 },
          root: {
            status: "",
            filePath: "",
            nome: "",
            dataEmissao: getCurrentDateWithZeroTime(),
            emissor: "",
            cargaHoraria: 0,
            previsaoConclusao: getCurrentDateWithZeroTime(),
          },
        };
      };

      // Determinar clienteNome e clienteEndereco
      let clienteNome = "";
      let clienteEndereco = "";
      if (localTrabalho === "cliente" && clienteSelecionadoData) {
        clienteNome = clienteSelecionadoData.nome;
        clienteEndereco = clienteSelecionadoData.endereco;
      } else if (localTrabalho && escritoriosEnderecos[localTrabalho]) {
        clienteNome = localTrabalho;
        clienteEndereco = escritoriosEnderecos[localTrabalho];
      }

      // Extrair DDI do prefixo baseado no país (removendo o "+")
      const ddi = extractDDIFromPrefix(addressConfig.phone.prefix);

      // Extrair apenas números do telefone (sem símbolos)
      const telefoneLimpo = cleanPhoneNumber(telefone);

      // Montar payload
      const payload: ColetaPerfilColaboradorCampanhaRequest = {
        endereco: {
          arquivoBase64: {
            base64: comprovanteBase64,
            tipo: comprovanteTipo,
          },
          root: {
            codigoPostal: codigoPostal.replace(/\D/g, ""),
            endereco,
            numero: parseInt(numero) || 0,
            bairro,
            cidade,
            estado,
            complemento: complemento || "",
            comprovanteResidenciaPath: "",
          },
        },
        formaAtuacao: {
          modeloTrabalho,
          frequencia: frequencia || "",
          frequenciaId: frequenciaId || 0,
          diasSemana: diasSemana.map((dia) => {
            const diasMap: Record<string, string> = {
              "Segunda-feira": "segunda",
              "Terça-feira": "terca",
              "Quarta-feira": "quarta",
              "Quinta-feira": "quinta",
              "Sexta-feira": "sexta",
              "Sábado": "sabado",
              "Domingo": "domingo",
            };
            return diasMap[dia] || dia.toLowerCase();
          }),
          localTrabalho,
          clienteNome,
          clienteEndereco,
        },
        awsTechnical: garantirCertificadoComStatus(
          awsCertificacoes.technical,
          certificadosAWS.technical
        ),
        awsTechnicalFoundational: garantirCertificadoComStatus(
          awsCertificacoes.foundational,
          certificadosAWS.foundational
        ),
        awsTechnicalAccredited: garantirCertificadoComStatus(
          awsCertificacoes.accredited,
          certificadosAWS.accredited
        ),
        dadosPessoais: {
          linkedin: linkedin && linkedin.trim() !== "" ? linkedin : null,
          pdc: pcdResposta || "",
          telefone: telefoneLimpo,
          ddi: ddi,
        },
      };

      const response = await useCase.execute(token, payload);

      if (response.sucesso) {
        // Atualizar questionariosPreenchidos no Redux store
        dispatch(updateQuestionariosPreenchidos('2025_01_COLETA_PERFIL_COLABORADOR'));
        toast.success(response.mensagem || "Perfil atualizado com sucesso!");
        navigate("/page/cvdigital");
      } else {
        // Exibir modal de erro com mensagem e erros
        const mensagemErro = response.mensagem && response.mensagem.trim() !== ""
          ? response.mensagem
          : "Erro ao atualizar perfil. Tente novamente.";
        setErrorMessage(mensagemErro);
        setErrorList(Array.isArray(response.erros) ? response.erros : []);
        setShowErrorDialog(true);
      }
    } catch (error) {
      console.error("Erro ao atualizar perfil:", error);
      // Tratar erros de rede e outros erros
      let errorMessage = "Erro ao atualizar perfil. Tente novamente.";
      
      if (error instanceof Error) {
        // Não mostrar "Failed to fetch" ou mensagens técnicas de rede
        if (error.message.toLowerCase().includes("failed to fetch") || 
            error.message.toLowerCase().includes("network error") ||
            error.message.toLowerCase().includes("networkerror")) {
          errorMessage = "Erro de conexão. Verifique sua internet e tente novamente.";
        } else if (error.message.trim() !== "") {
          // Se tiver mensagem válida e não for erro de rede, usar a mensagem
          errorMessage = error.message;
        }
      }
      
      setErrorMessage(errorMessage);
      setErrorList([]);
      setShowErrorDialog(true);
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="min-h-screen  p-2 sm:p-4 overflow-y-auto">
      <div className="space-y-4 w-full max-w-5xl mx-auto">
        {showHeader && (
          <div className="pb-4">
            <div className="flex items-start gap-3">
              <div className="w-8 h-8 sm:w-10 sm:h-10 rounded-lg bg-primary/10 flex items-center justify-center flex-shrink-0">
                <Sparkles className="w-4 h-4 sm:w-5 sm:h-5 text-primary" />
              </div>
              <div className="flex-1">
                <h1 className="text-base sm:text-lg font-semibold text-foreground">
                  Atualize seu Perfil
                </h1>
              </div>
            </div>
            <p className="text-sm sm:text-base font-semibold text-foreground mt-3 sm:mt-4">
              Seu próximo passo de carreira começa pelo seu perfil
            </p>
            <p className="text-xs sm:text-sm text-muted-foreground">
              Atualize agora, inclua seus certificados (especialmente os de AWS) e deixe suas
              oportunidades encontrarem você.
            </p>
          </div>
        )}

        <Form {...form}>
          <form onSubmit={form.handleSubmit(handleSubmit)} className="space-y-4">
        {/* Endereço de Residência */}
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2 text-foreground">
              <MapPin className="w-4 h-4" />
              <span className="font-medium text-sm">Endereço de Residência</span>
            </CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">

          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-5 gap-3">
            <div className="space-y-1.5">
              <Label className="text-xs text-muted-foreground">País</Label>
              <Select
                value={paisSelecionado}
                onValueChange={(value) => {
                  setPaisSelecionado(value);
                  setTelefone("");
                  setCodigoPostal("");
                  // Limpar bairro quando mudar para Estados Unidos
                  if (value === "eua") {
                    setBairro("");
                  }
                }}
              >
                <SelectTrigger className="bg-background border-border h-10">
                  <SelectValue placeholder="Selecione o país" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="brasil">Brasil</SelectItem>
                  <SelectItem value="portugal">Portugal</SelectItem>
                  <SelectItem value="eua">Estados Unidos</SelectItem>
                </SelectContent>
              </Select>
            </div>
            {paisSelecionado && (
              <>
                <FormField
                  control={form.control}
                  name="codigoPostal"
                  render={({ field }) => (
                    <FormItem className="space-y-1.5">
                      <FormLabel className="text-xs text-muted-foreground">
                        {addressConfig.postalCode.label} <span className="text-destructive">*</span>
                        {paisSelecionado === 'brasil' && loadingCep && (
                          <Loader2 className="inline-block ml-2 h-3 w-3 animate-spin text-primary" />
                        )}
                      </FormLabel>
                      <FormControl>
                        <div className="relative">
                          <Input
                            placeholder={addressConfig.postalCode.placeholder}
                            className={`bg-background border-border h-10 ${
                              paisSelecionado === 'brasil' && errorCep ? 'border-destructive' : ''
                            }`}
                            value={codigoPostal}
                            onChange={(e) => {
                              handlePostalCodeChange(e);
                              field.onChange(e.target.value);
                            }}
                            disabled={paisSelecionado === 'brasil' && loadingCep}
                          />
                          {paisSelecionado === 'brasil' && loadingCep && (
                            <div className="absolute right-3 top-1/2 -translate-y-1/2">
                              <Loader2 className="h-4 w-4 animate-spin text-primary" />
                            </div>
                          )}
                        </div>
                      </FormControl>
                      <FormMessage />
                      {paisSelecionado === 'brasil' && errorCep && (
                        <p className="text-xs text-destructive">{errorCep}</p>
                      )}
                      {paisSelecionado === 'brasil' && enderecoViaCep && !errorCep && (
                        <p className="text-xs text-green-600 dark:text-green-400">
                          ✓ Endereço encontrado
                        </p>
                      )}
                    </FormItem>
                  )}
                />
                <FormField
                  control={form.control}
                  name="endereco"
                  render={({ field }) => (
                    <FormItem className="space-y-1.5 lg:col-span-2">
                      <FormLabel className="text-xs text-muted-foreground">
                        {addressConfig.street.label} <span className="text-destructive">*</span>
                      </FormLabel>
                      <FormControl>
                        <Input
                          placeholder={addressConfig.street.placeholder}
                          className="bg-background border-border h-10"
                          {...field}
                          value={endereco}
                          onChange={(e) => {
                            setEndereco(e.target.value);
                            field.onChange(e.target.value);
                          }}
                        />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />
                <FormField
                  control={form.control}
                  name="numero"
                  render={({ field }) => (
                    <FormItem className="space-y-1.5">
                      <FormLabel className="text-xs text-muted-foreground">
                        {addressConfig.number.label} <span className="text-destructive">*</span>
                      </FormLabel>
                      <FormControl>
                        <Input
                          placeholder={addressConfig.number.placeholder}
                          className="bg-background border-border h-10"
                          {...field}
                          value={numero}
                          onChange={(e) => {
                            setNumero(e.target.value);
                            field.onChange(e.target.value);
                          }}
                        />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />
              </>
            )}
          </div>

          {paisSelecionado && (
            <>
              <div
                className={`grid grid-cols-1 sm:grid-cols-2 ${
                  addressConfig.neighborhood ? "lg:grid-cols-4" : "lg:grid-cols-3"
                } gap-3`}
              >
                {addressConfig.neighborhood && (
                  <FormField
                    control={form.control}
                    name="bairro"
                    render={({ field, fieldState }) => (
                      <FormItem className="space-y-1.5">
                        <FormLabel className="text-xs text-muted-foreground">
                          {addressConfig.neighborhood?.label}
                          {(paisSelecionado === "brasil" || paisSelecionado === "portugal") && (
                            <span className="text-destructive"> *</span>
                          )}
                        </FormLabel>
                        <FormControl>
                          <Input
                            placeholder={addressConfig.neighborhood?.placeholder}
                            className={`bg-background border-border h-10 ${
                              (paisSelecionado === "brasil" || paisSelecionado === "portugal") && 
                              (!bairro || bairro.trim() === "") && 
                              (fieldState.error || fieldState.isTouched)
                                ? "border-destructive"
                                : ""
                            }`}
                            {...field}
                            value={bairro}
                            onChange={(e) => {
                              setBairro(e.target.value);
                              field.onChange(e.target.value);
                            }}
                          />
                        </FormControl>
                        <FormMessage />
                        {(paisSelecionado === "brasil" || paisSelecionado === "portugal") && 
                         (!bairro || bairro.trim() === "") && 
                         (fieldState.isTouched || fieldState.error) && (
                          <p className="text-xs text-destructive">
                            {paisSelecionado === "brasil" ? "Bairro é obrigatório" : "Freguesia é obrigatória"}
                          </p>
                        )}
                      </FormItem>
                    )}
                  />
                )}
                <FormField
                  control={form.control}
                  name="complemento"
                  render={({ field }) => (
                    <FormItem className="space-y-1.5">
                      <FormLabel className="text-xs text-muted-foreground">
                        {addressConfig.complement.label}
                      </FormLabel>
                      <FormControl>
                        <Input
                          placeholder={addressConfig.complement.placeholder}
                          className="bg-background border-border h-10"
                          {...field}
                          value={complemento}
                          onChange={(e) => {
                            setComplemento(e.target.value);
                            field.onChange(e.target.value);
                          }}
                        />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />
                <FormField
                  control={form.control}
                  name="cidade"
                  render={({ field }) => (
                    <FormItem className="space-y-1.5">
                      <FormLabel className="text-xs text-muted-foreground">
                        {addressConfig.city.label} <span className="text-destructive">*</span>
                      </FormLabel>
                      <FormControl>
                        <Input
                          placeholder={addressConfig.city.placeholder}
                          className="bg-background border-border h-10"
                          {...field}
                          value={cidade}
                          onChange={(e) => {
                            setCidade(e.target.value);
                            field.onChange(e.target.value);
                          }}
                        />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />
                <FormField
                  control={form.control}
                  name="estado"
                  render={({ field }) => (
                    <FormItem className="space-y-1.5">
                      <FormLabel className="text-xs text-muted-foreground">
                        {addressConfig.state.label} <span className="text-destructive">*</span>
                      </FormLabel>
                      <FormControl>
                        <Input
                          placeholder={addressConfig.state.placeholder}
                          className="bg-background border-border h-10"
                          {...field}
                          value={estado}
                          onChange={(e) => {
                            setEstado(e.target.value);
                            field.onChange(e.target.value);
                          }}
                        />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />
              </div>

              <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-3">
                <FormField
                  control={form.control}
                  name="telefone"
                  render={({ field }) => (
                    <FormItem className="space-y-1.5 sm:col-span-1">
                      <FormLabel className="text-xs text-muted-foreground">
                        {addressConfig.phone.label} <span className="text-destructive">*</span>
                      </FormLabel>
                      <FormControl>
                        <div className="flex gap-2">
                          {addressConfig.phone.prefix && (
                            <div className="flex items-center justify-center px-3 bg-muted border border-border rounded-lg h-10 text-sm text-muted-foreground whitespace-nowrap">
                              {addressConfig.phone.prefix}
                            </div>
                          )}
                          <Input
                            placeholder={addressConfig.phone.placeholder}
                            className="bg-background border-border h-10 flex-1 min-w-[160px]"
                            value={telefone}
                            onChange={(e) => {
                              handlePhoneChange(e);
                              field.onChange(e.target.value);
                            }}
                          />
                        </div>
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />
              </div>
            </>
          )}

          <FormField
            control={form.control}
            name="comprovanteResidencia"
            render={({ field: { onChange, value, ...field } }) => (
              <FormItem className="space-y-2">
                <FormLabel className="text-xs text-muted-foreground">
                  Comprovante de Residência <span className="text-destructive">*</span>
                </FormLabel>
                <FormControl>
                  {!uploadedFile ? (
                    <label className="border-2 border-dashed border-border rounded-lg p-4 bg-background cursor-pointer hover:bg-muted/30 hover:border-primary/50 transition-all block text-center">
                      <input
                        type="file"
                        accept=".pdf,.jpg,.jpeg,.png"
                        className="hidden"
                        onChange={(e) => {
                          const file = e.target.files?.[0];
                          if (file) {
                            setUploadedFile(file.name);
                            setComprovanteFile(file);
                            onChange(file);
                          }
                        }}
                        {...field}
                      />
                <Upload className="w-6 h-6 text-muted-foreground mx-auto mb-2" />
                <p className="text-sm text-muted-foreground">Clique aqui para escolher o arquivo</p>
                <p className="text-xs text-muted-foreground mt-1">
                  Formatos aceitos: PDF, JPG, PNG
                </p>
              </label>
            ) : (
              <div className="flex items-center justify-between bg-secondary rounded-lg p-3">
                <div className="flex items-center gap-2 text-secondary-foreground">
                  <div className="w-8 h-8 rounded-lg bg-primary/10 flex items-center justify-center">
                    <Upload className="w-4 h-4 text-primary" />
                  </div>
                  <span className="text-sm font-medium">{uploadedFile}</span>
                </div>
                <button
                  type="button"
                  onClick={() => {
                    setUploadedFile(null);
                    setComprovanteFile(null);
                  }}
                  className="w-8 h-8 rounded-full hover:bg-destructive/10 flex items-center justify-center transition-colors group"
                >
                  <X className="w-4 h-4 text-muted-foreground group-hover:text-destructive" />
                </button>
              </div>
            )}
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />

          {uploadedFile && cepMismatch && (
            <div className="flex items-start gap-2 bg-destructive/10 text-destructive rounded-lg p-3">
              <AlertTriangle className="w-4 h-4 mt-0.5 flex-shrink-0" />
              <p className="text-sm">
                Os dados do comprovante não correspondem ao CEP informado.
              </p>
            </div>
          )}
          </CardContent>
        </Card>

        {/* PCD - Pessoa com Deficiência */}
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2 text-foreground">
              <span className="w-4 h-4 flex items-center justify-center">♿</span>
              <span className="font-medium text-sm">Pessoa com Deficiência (PCD)</span>
            </CardTitle>
          </CardHeader>
          <CardContent className="space-y-3">

          <FormField
            control={form.control}
            name="pcd"
            render={({ field, fieldState }) => (
              <FormItem>
                <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-3">
                  <div className="space-y-1.5">
                    <FormLabel className="text-xs text-muted-foreground">
                      Você é uma pessoa com deficiência? <span className="text-destructive">*</span>
                    </FormLabel>
                    <Select 
                      value={pcdResposta} 
                      onValueChange={(value) => {
                        setPcdResposta(value);
                        field.onChange(value);
                      }}
                    >
                <SelectTrigger className={`bg-background border-border h-10 ${
                      fieldState.error ? "border-destructive" : ""
                    }`}>
                  <SelectValue placeholder="Selecione uma opção" />
                </SelectTrigger>
                <SelectContent className="bg-background border-border">
                  <SelectItem value="sim">Sim</SelectItem>
                  <SelectItem value="nao">Não</SelectItem>
                </SelectContent>
                    </Select>
                    <FormMessage />
                  </div>
                </div>
              </FormItem>
            )}
          />
          </CardContent>
        </Card>

        {/* Forma de Atuação */}
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2 text-foreground">
              <Building2 className="w-4 h-4" />
              <span className="font-medium text-sm">Forma de Atuação</span>
            </CardTitle>
          </CardHeader>
          <CardContent className="space-y-3">

          <FormField
            control={form.control}
            name="modeloTrabalho"
            render={({ field }) => (
              <FormItem>
                <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-3">
                  <div className="space-y-1.5">
                    <FormLabel className="text-xs text-muted-foreground">
                      Modelo de Trabalho <span className="text-destructive">*</span>
                    </FormLabel>
                    <Select
                      value={modeloTrabalho}
                      onValueChange={(value) => {
                        setModeloTrabalho(value);
                        field.onChange(value);
                      }}
                    >
                <SelectTrigger className="bg-background border-border h-10">
                  <SelectValue placeholder="Selecione" />
                </SelectTrigger>
                <SelectContent className="bg-background border-border">
                  <SelectItem value="remoto">Remoto</SelectItem>
                  <SelectItem value="hibrido">Híbrido</SelectItem>
                  <SelectItem value="presencial">Presencial</SelectItem>
                </SelectContent>
                    </Select>
                  </div>
                </div>
                <FormMessage />
              </FormItem>
            )}
          />

          {modeloTrabalho === "hibrido" && (
            <>
              <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-3 pt-2 border-t border-border">
                <div className="space-y-1.5">
                  <Label className="text-xs text-muted-foreground">
                    Frequência no Escritório <span className="text-destructive">*</span>
                  </Label>
                  <Select
                    value={frequenciaId > 0 ? frequenciaId.toString() : ""}
                    onValueChange={(value) => {
                      const id = parseInt(value);
                      setFrequenciaId(id);
                      setFrequencia(frequenciaMap[id] || "");
                      setDiasSemana([]);
                    }}
                  >
                    <SelectTrigger className={`bg-background border-border h-10 ${
                      !frequenciaId ? "border-destructive" : ""
                    }`}>
                      <SelectValue placeholder="Quantas vezes por semana?" />
                    </SelectTrigger>
                    <SelectContent className="bg-background border-border">
                      <SelectItem value="1">1 vez por semana</SelectItem>
                      <SelectItem value="2">2 vezes por semana</SelectItem>
                      <SelectItem value="3">3 vezes por semana</SelectItem>
                      <SelectItem value="4">4 vezes por semana</SelectItem>
                      <SelectItem value="5">5 vezes por semana</SelectItem>
                    </SelectContent>
                  </Select>
                  {!frequenciaId && (
                    <p className="text-xs text-destructive">
                      Frequência é obrigatória para modelo híbrido
                    </p>
                  )}
                </div>

            <FormField
              control={form.control}
              name="localTrabalho"
              render={({ field }) => (
                <FormItem className="space-y-1.5">
                  <FormLabel className="text-xs text-muted-foreground">
                    Local de Trabalho <span className="text-destructive">*</span>
                  </FormLabel>
                  <Select
                    value={localTrabalho}
                    onValueChange={(value) => {
                      setLocalTrabalho(value);
                      field.onChange(value);
                    }}
                  >
                    <SelectTrigger className={`bg-background border-border h-10 ${
                      !localTrabalho ? "border-destructive" : ""
                    }`}>
                      <SelectValue placeholder="Selecione o local de trabalho" />
                    </SelectTrigger>
                    <SelectContent className="bg-background border-border">
                      <SelectItem value="alphaville">Alphaville</SelectItem>
                      <SelectItem value="paulista">Paulista</SelectItem>
                      <SelectItem value="rio">Escritório Rio de Janeiro</SelectItem>
                      <SelectItem value="curitiba">Escritório Curitiba</SelectItem>
                      <SelectItem value="cliente">Cliente</SelectItem>
                    </SelectContent>
                      </Select>
                      <FormMessage />
                      {!localTrabalho && (
                        <p className="text-xs text-destructive">
                          Local de trabalho é obrigatório para modelo híbrido
                        </p>
                      )}
                    </FormItem>
                  )}
                />

                {localTrabalho === "cliente" && (
                  <div className="space-y-1.5">
                    <Label className="text-xs text-muted-foreground">Buscar Cliente <span className="text-destructive">*</span></Label>
                    <div className="relative">
                      <Input
                        placeholder="Digite o nome do cliente..."
                        value={clienteBusca}
                        onChange={(e) => {
                          setClienteBusca(e.target.value);
                          setClienteSelecionado(false);
                        }}
                        className="bg-background border-border h-10"
                      />
                      {clienteBusca &&
                        clientesFiltrados.length > 0 &&
                        !clienteSelecionado && (
                          <div className="absolute top-full left-0 right-0 mt-1 bg-background border border-border rounded-lg shadow-lg z-50 max-h-48 overflow-y-auto">
                            {clientesFiltrados.map((cliente) => (
                              <button
                                key={cliente.nome}
                                type="button"
                                onClick={() => {
                                  setClienteBusca(cliente.nome);
                                  setClienteSelecionado(true);
                                }}
                                className="w-full text-left px-3 py-2 text-sm hover:bg-muted transition-colors first:rounded-t-lg last:rounded-b-lg"
                              >
                                {cliente.nome}
                              </button>
                            ))}
                          </div>
                        )}
                    </div>
                  </div>
                )}
              </div>

              {localTrabalho && localTrabalho !== "cliente" && escritoriosEnderecos[localTrabalho] && (
                <div className="p-3 bg-primary/10 rounded-lg border border-border">
                  <p className="text-xs text-muted-foreground mb-1">Endereço do escritório:</p>
                  <p className="text-sm text-foreground font-medium">
                    {escritoriosEnderecos[localTrabalho]}
                  </p>
                </div>
              )}

              {localTrabalho === "cliente" && clienteSelecionado && clienteSelecionadoData && (
                <div className="p-3 bg-primary/10 rounded-lg border border-border">
                  <p className="text-xs text-muted-foreground mb-1">Endereço do cliente:</p>
                  <p className="text-sm text-foreground font-medium">
                    {clienteSelecionadoData.endereco}
                  </p>
                </div>
              )}

              <div className="space-y-2">
                <Label className="text-xs text-muted-foreground">
                  Dia(s) da Semana <span className="text-destructive">*</span>
                </Label>
                <div className="grid grid-cols-2 sm:grid-cols-4 lg:grid-cols-7 gap-2">
                  {[
                    "Segunda-feira",
                    "Terça-feira",
                    "Quarta-feira",
                    "Quinta-feira",
                    "Sexta-feira",
                    "Sábado",
                    "Domingo",
                  ].map((dia) => {
                    const maxDias = frequenciaId || 0;
                    const isDisabled =
                      !diasSemana.includes(dia) && diasSemana.length >= maxDias;
                    return (
                      <label
                        key={dia}
                        className={`flex items-center justify-center gap-2 p-2 rounded-lg border text-xs sm:text-sm cursor-pointer transition-all ${
                          diasSemana.includes(dia)
                            ? "bg-primary text-primary-foreground border-primary"
                            : isDisabled
                              ? "bg-muted/50 text-muted-foreground border-border cursor-not-allowed opacity-50"
                              : "bg-background border-border hover:border-primary/50"
                        }`}
                      >
                        <input
                          type="checkbox"
                          className="sr-only"
                          checked={diasSemana.includes(dia)}
                          disabled={isDisabled}
                          onChange={(e) => {
                            if (e.target.checked) {
                              setDiasSemana([...diasSemana, dia]);
                            } else {
                              setDiasSemana(diasSemana.filter((d) => d !== dia));
                            }
                          }}
                        />
                        {dia.slice(0, 3)}
                      </label>
                    );
                  })}
                </div>
                {frequenciaId > 0 && (
                  <p className={`text-xs ${
                    diasSemana.length < frequenciaId ? "text-destructive" : "text-muted-foreground"
                  }`}>
                    Selecione {frequenciaId} dia{frequenciaId > 1 ? "s" : ""} (
                    {diasSemana.length}/{frequenciaId} selecionado
                    {diasSemana.length !== 1 ? "s" : ""})
                    {diasSemana.length < frequenciaId && " - Selecione todos os dias obrigatórios"}
                  </p>
                )}
                {frequenciaId > 0 && diasSemana.length === 0 && (
                  <p className="text-xs text-destructive">
                    Selecione pelo menos {frequenciaId} dia{frequenciaId > 1 ? "s" : ""} da semana
                  </p>
                )}
              </div>
            </>
          )}

          {modeloTrabalho === "presencial" && (
            <>
              <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-3 pt-2 border-t border-border">
                <FormField
                  control={form.control}
                  name="localTrabalho"
                  render={({ field }) => (
                    <FormItem className="space-y-1.5">
                      <FormLabel className="text-xs text-muted-foreground">
                        Local de Trabalho <span className="text-destructive">*</span>
                      </FormLabel>
                      <Select
                        value={localTrabalho}
                        onValueChange={(value) => {
                          setLocalTrabalho(value);
                          field.onChange(value);
                          if (value !== "cliente") {
                            setClienteBusca("");
                            setClienteSelecionado(false);
                          }
                        }}
                      >
                    <SelectTrigger className={`bg-background border-border h-10 ${
                      !localTrabalho ? "border-destructive" : ""
                    }`}>
                      <SelectValue placeholder="Selecione o local de trabalho" />
                    </SelectTrigger>
                    <SelectContent className="bg-background border-border">
                      <SelectItem value="alphaville">Alphaville</SelectItem>
                      <SelectItem value="paulista">Paulista</SelectItem>
                      <SelectItem value="rio">Escritório Rio de Janeiro</SelectItem>
                      <SelectItem value="curitiba">Escritório Curitiba</SelectItem>
                      <SelectItem value="cliente">Cliente</SelectItem>
                    </SelectContent>
                      </Select>
                      <FormMessage />
                      {!localTrabalho && (
                        <p className="text-xs text-destructive">
                          Local de trabalho é obrigatório para modelo presencial
                        </p>
                      )}
                    </FormItem>
                  )}
                />

                {localTrabalho === "cliente" && (
                  <div className="space-y-1.5">
                    <Label className="text-xs text-muted-foreground">
                      Buscar Cliente <span className="text-destructive">*</span>
                    </Label>
                    <div className="relative">
                      <Input
                        placeholder="Digite o nome do cliente..."
                        value={clienteBusca}
                        onChange={(e) => {
                          setClienteBusca(e.target.value);
                          setClienteSelecionado(false);
                        }}
                        className={`bg-background border-border h-10 ${
                          localTrabalho === "cliente" && !clienteSelecionado ? "border-destructive" : ""
                        }`}
                      />
                      {clienteBusca &&
                        clientesFiltrados.length > 0 &&
                        !clienteSelecionado && (
                          <div className="absolute top-full left-0 right-0 mt-1 bg-background border border-border rounded-lg shadow-lg z-50 max-h-48 overflow-y-auto">
                            {clientesFiltrados.map((cliente) => (
                              <button
                                key={cliente.nome}
                                type="button"
                                onClick={() => {
                                  setClienteBusca(cliente.nome);
                                  setClienteSelecionado(true);
                                }}
                                className="w-full text-left px-3 py-2 text-sm hover:bg-muted transition-colors first:rounded-t-lg last:rounded-b-lg"
                              >
                                {cliente.nome}
                              </button>
                            ))}
                          </div>
                        )}
                    </div>
                    {localTrabalho === "cliente" && !clienteSelecionado && (
                      <p className="text-xs text-destructive">
                        Selecione um cliente da lista
                      </p>
                    )}
                  </div>
                )}
              </div>

              {localTrabalho && localTrabalho !== "cliente" && escritoriosEnderecos[localTrabalho] && (
                <div className="p-3 bg-primary/10 rounded-lg border border-border">
                  <p className="text-xs text-muted-foreground mb-1">Endereço do escritório:</p>
                  <p className="text-sm text-foreground font-medium">
                    {escritoriosEnderecos[localTrabalho]}
                  </p>
                </div>
              )}

              {localTrabalho === "cliente" && clienteSelecionado && clienteSelecionadoData && (
                <div className="p-3 bg-primary/10 rounded-lg border border-border">
                  <p className="text-xs text-muted-foreground mb-1">Endereço do cliente:</p>
                  <p className="text-sm text-foreground font-medium">
                    {clienteSelecionadoData.endereco}
                  </p>
                </div>
              )}
            </>
          )}
          </CardContent>
        </Card>

        {/* Certificação AWS */}
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2 text-foreground">
              <Award className="w-4 h-4" />
              <span className="font-medium text-sm">Certificação AWS</span>
            </CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">

          <div className="space-y-1.5">
            <Label className="text-xs text-muted-foreground">
              Você possui ou tem interesse em certificações AWS? <span className="text-destructive">*</span>
            </Label>
            <Select
              value={possuiCertificadoAWS}
              onValueChange={(value) => {
                setPossuiCertificadoAWS(value);
                if (value === "nao") {
                  setCertificadosAWS({
                    technical: {
                      status: "",
                      file: null,
                      nome: "",
                      dataEmissao: "",
                      emissor: "",
                      cargaHoraria: "",
                      previsaoConclusao: "",
                    },
                    foundational: {
                      status: "",
                      file: null,
                      nome: "",
                      dataEmissao: "",
                      emissor: "",
                      cargaHoraria: "",
                      previsaoConclusao: "",
                    },
                    accredited: {
                      status: "",
                      file: null,
                      nome: "",
                      dataEmissao: "",
                      emissor: "",
                      cargaHoraria: "",
                      previsaoConclusao: "",
                    },
                  });
                }
              }}
            >
              <SelectTrigger className={`bg-background border-border h-10 max-w-[200px] ${
                !possuiCertificadoAWS ? "border-destructive" : ""
              }`}>
                <SelectValue placeholder="Selecione uma opção" />
              </SelectTrigger>
              <SelectContent className="bg-background border-border">
                <SelectItem value="sim">Sim</SelectItem>
                <SelectItem value="nao">Não</SelectItem>
              </SelectContent>
            </Select>
            {!possuiCertificadoAWS && (
              <p className="text-xs text-destructive">
                Certificação AWS é obrigatória
              </p>
            )}
          </div>

          {possuiCertificadoAWS === "sim" && (
            <div className="space-y-4">
              <div className="bg-secondary rounded-lg p-3">
                <p className="text-sm text-secondary-foreground">
                  Selecione abaixo os tipos de certificação e informe o status de cada um.
                </p>
              </div>

              {[
                { key: "technical" as const, label: "Technical Certified" },
                { key: "foundational" as const, label: "Foundational Certified" },
                { key: "accredited" as const, label: "Accredited Individuals" },
              ].map((cert) => (
                <div
                  key={cert.key}
                  className="border border-border rounded-lg p-4 space-y-3 bg-primary/10"
                >
                  <div className="flex items-center justify-between">
                    <span className="font-medium text-sm text-foreground">{cert.label}</span>
                    <Select
                      value={certificadosAWS[cert.key].status}
                      onValueChange={(value) => {
                        setCertificadosAWS((prev) => ({
                          ...prev,
                          [cert.key]: {
                            ...prev[cert.key],
                            status: value,
                            ...(value !== "concluido" && {
                              file: null,
                              nome: "",
                              dataEmissao: "",
                              emissor: "",
                              cargaHoraria: "",
                            }),
                          },
                        }));
                      }}
                    >
                      <SelectTrigger className="bg-background border-border h-9 w-48">
                        <SelectValue placeholder="Selecione o status" />
                      </SelectTrigger>
                      <SelectContent className="bg-background border-border">
                        <SelectItem value="concluido">Concluído</SelectItem>
                        <SelectItem value="cursando">Estou Cursando</SelectItem>
                        <SelectItem value="interesse">Tenho Interesse</SelectItem>
                      </SelectContent>
                    </Select>
                  </div>

                  {certificadosAWS[cert.key].status === "concluido" && (
                    <div className="space-y-3 pt-2 border-t border-border">
                      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-3">
                        <div className="space-y-2 lg:col-span-2">
                          <Label className="text-xs text-muted-foreground">
                            Upload do Certificado <span className="text-destructive">*</span>
                          </Label>

                          {!certificadosAWS[cert.key].file ? (
                            <label className="border-2 border-dashed border-border rounded-lg p-3 bg-background cursor-pointer hover:bg-muted/30 hover:border-primary/50 transition-all block text-center">
                              <input
                                type="file"
                                accept=".pdf,.jpg,.jpeg,.png"
                                className="hidden"
                                onChange={(e) => {
                                  const file = e.target.files?.[0];
                                  if (file) {
                                    setCertificadosAWS((prev) => ({
                                      ...prev,
                                      [cert.key]: {
                                        ...prev[cert.key],
                                        file,
                                      },
                                    }));
                                  }
                                }}
                              />
                              <Upload className="w-4 h-4 mx-auto mb-1 text-muted-foreground" />
                              <p className="text-xs text-muted-foreground">Clique para enviar</p>
                            </label>
                          ) : (
                            <div className="flex items-center justify-between bg-secondary rounded-lg p-2">
                              <div className="flex items-center gap-2 text-secondary-foreground">
                                <div className="w-6 h-6 rounded bg-primary/10 flex items-center justify-center">
                                  <Upload className="w-3 h-3 text-primary" />
                                </div>
                                <span className="text-xs font-medium truncate max-w-[120px]">
                                  {certificadosAWS[cert.key].file?.name}
                                </span>
                              </div>
                              <button
                                type="button"
                                onClick={() =>
                                  setCertificadosAWS((prev) => ({
                                    ...prev,
                                    [cert.key]: {
                                      ...prev[cert.key],
                                      file: null,
                                    },
                                  }))
                                }
                                className="w-6 h-6 rounded-full hover:bg-destructive/10 flex items-center justify-center transition-colors group"
                              >
                                <X className="w-3 h-3 text-muted-foreground group-hover:text-destructive" />
                              </button>
                            </div>
                          )}
                        </div>

                        <div className="space-y-1.5">
                          <Label className="text-xs text-muted-foreground">
                            Nome do Certificado <span className="text-destructive">*</span>
                          </Label>
                          <Input
                            placeholder="Nome do certificado"
                            className="bg-background border-border h-9 text-sm"
                            value={certificadosAWS[cert.key].nome}
                            onChange={(e) =>
                              setCertificadosAWS((prev) => ({
                                ...prev,
                                [cert.key]: {
                                  ...prev[cert.key],
                                  nome: e.target.value,
                                },
                              }))
                            }
                          />
                        </div>

                        <div className="space-y-1.5">
                          <Label className="text-xs text-muted-foreground">
                            Data de emissão <span className="text-destructive">*</span>
                          </Label>
                          <Input
                            placeholder="dd/mm/aaaa"
                            className={`bg-background border-border h-9 text-sm ${
                              certificadosAWS[cert.key].dataEmissao && 
                              !validateDate(certificadosAWS[cert.key].dataEmissao)
                                ? "border-destructive"
                                : ""
                            }`}
                            value={certificadosAWS[cert.key].dataEmissao}
                            maxLength={10}
                            onChange={(e) => {
                              const valorFormatado = formatarDataInput(e.target.value);
                              setCertificadosAWS((prev) => ({
                                ...prev,
                                [cert.key]: {
                                  ...prev[cert.key],
                                  dataEmissao: valorFormatado,
                                },
                              }));
                            }}
                          />
                          {certificadosAWS[cert.key].dataEmissao && 
                           !validateDate(certificadosAWS[cert.key].dataEmissao) && (
                            <p className="text-xs text-destructive">
                              Data inválida. Use o formato dd/mm/aaaa
                            </p>
                          )}
                        </div>
                      </div>

                      <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
                        <div className="space-y-1.5">
                          <Label className="text-xs text-muted-foreground">
                            Nome do emissor <span className="text-destructive">*</span>
                          </Label>
                          <Input
                            placeholder="Instituição ou empresa emissora"
                            className="bg-background border-border h-9 text-sm"
                            value={certificadosAWS[cert.key].emissor}
                            onChange={(e) =>
                              setCertificadosAWS((prev) => ({
                                ...prev,
                                [cert.key]: {
                                  ...prev[cert.key],
                                  emissor: e.target.value,
                                },
                              }))
                            }
                          />
                        </div>

                        <div className="space-y-1.5">
                          <Label className="text-xs text-muted-foreground">
                            Carga horária <span className="text-destructive">*</span>
                          </Label>
                          <Input
                            placeholder="Informe a carga horária"
                            className="bg-background border-border h-9 text-sm"
                            value={certificadosAWS[cert.key].cargaHoraria}
                            maxLength={5}
                            onChange={(e) => {
                              // Permite apenas números
                              const apenasNumeros = e.target.value.replace(/\D/g, "");
                              // Limita a 5 caracteres
                              const valorLimitado = apenasNumeros.slice(0, 5);
                              setCertificadosAWS((prev) => ({
                                ...prev,
                                [cert.key]: {
                                  ...prev[cert.key],
                                  cargaHoraria: valorLimitado,
                                },
                              }));
                            }}
                          />
                        </div>
                      </div>
                    </div>
                  )}

                  {certificadosAWS[cert.key].status === "cursando" && (
                    <div className="pt-3 border-t border-border space-y-3">
                      <div className="flex items-start gap-2">
                        <span className="text-sm">📚</span>
                        <p className="text-xs text-muted-foreground italic">
                          Você está cursando esta certificação. Quando concluir, atualize o status
                          para enviar o certificado.
                        </p>
                      </div>
                      <div className="space-y-1.5">
                        <Label className="text-xs text-muted-foreground">
                          Previsão de conclusão
                        </Label>
                        <Input
                          placeholder="dd/mm/aaaa"
                          className="bg-background border-border h-9 text-sm max-w-[200px]"
                          value={certificadosAWS[cert.key].previsaoConclusao}
                          maxLength={10}
                          onChange={(e) => {
                            const valorFormatado = formatarDataInput(e.target.value);
                            setCertificadosAWS((prev) => ({
                              ...prev,
                              [cert.key]: {
                                ...prev[cert.key],
                                previsaoConclusao: valorFormatado,
                              },
                            }));
                          }}
                        />
                        {certificadosAWS[cert.key].previsaoConclusao && 
                         !validateDate(certificadosAWS[cert.key].previsaoConclusao) && (
                          <p className="text-xs text-destructive">
                            Data inválida. Use o formato dd/mm/aaaa
                          </p>
                        )}
                      </div>
                    </div>
                  )}

                  {certificadosAWS[cert.key].status === "interesse" && (
                    <div className="pt-2 border-t border-border">
                      <p className="text-xs text-muted-foreground italic">
                        ✨ Você tem interesse nesta certificação. Entraremos em contato com
                        oportunidades.
                      </p>
                    </div>
                  )}
                </div>
              ))}
            </div>
          )}
          </CardContent>
        </Card>

        {/* Dados Pessoais e Profissionais */}
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2 text-foreground">
              <span className="w-4 h-4 flex items-center justify-center">💼</span>
              <span className="font-medium text-sm">Dados Pessoais e Profissionais</span>
            </CardTitle>
          </CardHeader>
          <CardContent className="space-y-3">

          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-3">
            <FormField
              control={form.control}
              name="linkedin"
              render={({ field, fieldState }) => (
                <FormItem className="space-y-1.5">
                  <FormLabel className="text-xs text-muted-foreground">
                    URL do LinkedIn
                  </FormLabel>
                  <FormControl>
                    <Input
                      placeholder="https://www.linkedin.com/in/seu-perfil"
                      value={linkedin}
                      onChange={(e) => {
                        setLinkedin(e.target.value);
                        field.onChange(e.target.value);
                      }}
                      className={`bg-background border-border h-10 ${
                        fieldState.error ? "border-destructive" : ""
                      }`}
                    />
                  </FormControl>
                  <FormMessage />
                  {!linkedin && !fieldState.error && (
                    <p className="text-xs text-muted-foreground">
                      Cole o link completo do seu perfil do LinkedIn
                    </p>
                  )}
                </FormItem>
              )}
            />
          </div>
          </CardContent>
        </Card>

        {/* Buttons */}
        <div className="flex flex-col sm:flex-row gap-3 pt-2">
          <Button
            variant="outline"
            size="lg"
            className="flex-1"
            onClick={() => navigate("/dashboard")}
            disabled={isSubmitting}
          >
            Cancelar
          </Button>
          <Button
            type="submit"
            variant="primary"
            size="lg"
            className="flex-1"
            disabled={isSubmitting}
          >
            {isSubmitting ? (
              <>
                <Loader2 className="w-4 h-4 mr-2 animate-spin" />
                Salvando...
              </>
            ) : (
              "Salvar e ir para o Perfil 360"
            )}
          </Button>
        </div>
          </form>
        </Form>
      </div>

      {/* Modal de Erro */}
      <AlertDialog open={showErrorDialog} onOpenChange={setShowErrorDialog}>
        <AlertDialogContent className="max-w-2xl">
          <AlertDialogHeader>
            <AlertDialogTitle>Erro ao atualizar perfil</AlertDialogTitle>
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
}
