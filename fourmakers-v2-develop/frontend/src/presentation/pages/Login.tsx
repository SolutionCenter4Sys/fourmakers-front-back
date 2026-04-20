import { useMemo } from "react";
import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import * as z from "zod";
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
import { Label } from "@/components/ui/label";
import { Card, CardContent } from "@/components/ui/card";
import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { InputOTP, InputOTPGroup, InputOTPSlot } from "@/components/ui/input-otp";
import { CheckCircle2 } from "@/components/ui/system-icons";
import { useState, useEffect } from "react";
import { Badge } from "@/components/ui/badge";
// Adicionar imports para AlertDialog
import { AlertDialog, AlertDialogAction, AlertDialogContent, AlertDialogDescription, AlertDialogFooter, AlertDialogHeader, AlertDialogTitle } from "@/components/ui/alert-dialog";

// ADICIONE ESTA IMPORTAÇÃO DO REDUX
import { useAppDispatch, useAppSelector } from '@app/store/hooks';
import { sendLoginToken, validateLoginToken, clearLoginState, fetchShowmeProfile } from '@app/store/slices/authSlice';
// ADICIONE ESTA IMPORTAÇÃO:
import { useNavigate, useParams, useSearchParams } from "react-router-dom";
// Importar configurações de organização
import { orgLoginConfigs } from '@shared/constants/orgConfig';
import type { OrgLoginConfig } from '@shared/constants/orgConfig';
import { getSsoUrlForCurrentEnv } from '@shared/utils/envUtils';
import { clearFlutterFlowStorageIframe } from '@shared/utils/clearFlutterFlowStorage';
import { Checkbox } from "@/components/ui/checkbox";
import { toast } from 'sonner';
import { format } from 'date-fns';
import { ptBR } from 'date-fns/locale';
import { container } from '@core/di/container';
import { CadastrarUsuarioUseCase } from '@domain/usecases/CadastrarUsuarioUseCase';
import { validarCPF, formatCPF } from '@shared/utils/cpfUtils';
import { Spinner } from '@/components/ui/spinner';

import logoFourmakers from "@/assets/logo-fourmakers.svg";
import loginHero1 from "@/assets/login-hero.jpg";
import loginHero2 from "@/assets/login-hero-2.jpg";
import loginHero3 from "@/assets/login-hero-3.jpg";
import loginHero4 from "@/assets/login-hero-4.jpg";
import loginHero5 from "@/assets/login-hero-5.jpg";
import loginHero6 from "@/assets/login-hero-6.jpg";
import loginHero7 from "@/assets/login-hero-7.jpg";
import loginHero8 from "@/assets/login-hero-8.jpg";
import loginHero9 from "@/assets/login-hero-9.jpg";
import loginHero10 from "@/assets/login-hero-10.jpg";


const heroImages = [
  loginHero1, loginHero2, loginHero3, loginHero4, loginHero5,
  loginHero6, loginHero7, loginHero8, loginHero9, loginHero10
];

const formSchema = z.object({
  email: z.string().email("Email inválido"),
});

type FormData = z.infer<typeof formSchema>;

// Schema para cadastro
const cadastroSchema = z.object({
  nomeCompleto: z.string().min(3, "Nome completo deve ter no mínimo 3 caracteres"),
  cpf: z.string()
    .optional()
    .refine((cpf) => {
      // Se CPF foi preenchido, deve ser válido
      if (!cpf || cpf.trim() === '') return true; // Opcional, então vazio é válido
      return validarCPF(cpf);
    }, {
      message: "CPF inválido",
    }),
  email: z.string().email("Email inválido"),
  senha: z.string().min(6, "Senha deve ter no mínimo 6 caracteres"),
  confirmaSenha: z.string().min(6, "Confirmação de senha é obrigatória"),
}).refine((data) => data.senha === data.confirmaSenha, {
  message: "As senhas não coincidem",
  path: ["confirmaSenha"],
});

type CadastroFormData = z.infer<typeof cadastroSchema>;

// Componente de Força de Senha
interface PasswordStrengthProps {
  password: string;
}

const PasswordStrength = ({ password }: PasswordStrengthProps) => {
  const calcularForca = (senha: string): { nivel: number; label: string; cor: string } => {
    if (!senha) {
      return { nivel: 0, label: '', cor: '' };
    }

    let forca = 0;
    const criterios = {
      comprimento: senha.length >= 8,
      minuscula: /[a-z]/.test(senha),
      maiuscula: /[A-Z]/.test(senha),
      numero: /[0-9]/.test(senha),
      especial: /[^a-zA-Z0-9]/.test(senha),
    };

    if (criterios.comprimento) forca++;
    if (criterios.minuscula) forca++;
    if (criterios.maiuscula) forca++;
    if (criterios.numero) forca++;
    if (criterios.especial) forca++;

    if (forca <= 1) {
      return { nivel: 1, label: 'Muito fraca', cor: 'bg-red-500' };
    } else if (forca <= 2) {
      return { nivel: 2, label: 'Fraca', cor: 'bg-orange-500' };
    } else if (forca <= 3) {
      return { nivel: 3, label: 'Média', cor: 'bg-yellow-500' };
    } else if (forca <= 4) {
      return { nivel: 4, label: 'Forte', cor: 'bg-green-500' };
    } else {
      return { nivel: 5, label: 'Muito forte', cor: 'bg-green-600' };
    }
  };

  const forca = calcularForca(password);

  if (!password) return null;

  return (
    <div className="space-y-2 mt-2">
      <div className="flex items-center gap-2">
        <div className="flex-1 h-2 bg-muted rounded-full overflow-hidden">
          <div
            className={`h-full transition-all duration-300 ${forca.cor}`}
            style={{ width: `${(forca.nivel / 5) * 100}%` }}
          />
        </div>
        {forca.label && (
          <span className={`text-xs font-medium ${
            forca.nivel <= 2 ? 'text-red-500' :
            forca.nivel === 3 ? 'text-yellow-500' :
            'text-green-500'
          }`}>
            {forca.label}
          </span>
        )}
      </div>
      <div className="text-xs text-muted-foreground space-y-1">
        <p className="font-medium">A senha deve conter:</p>
        <ul className="list-disc list-inside space-y-0.5 ml-2">
          <li className={password.length >= 8 ? 'text-green-600' : ''}>
            Pelo menos 8 caracteres
          </li>
          <li className={/[a-z]/.test(password) ? 'text-green-600' : ''}>
            Uma letra minúscula
          </li>
          <li className={/[A-Z]/.test(password) ? 'text-green-600' : ''}>
            Uma letra maiúscula
          </li>
          <li className={/[0-9]/.test(password) ? 'text-green-600' : ''}>
            Um número
          </li>
          <li className={/[^a-zA-Z0-9]/.test(password) ? 'text-green-600' : ''}>
            Um caractere especial
          </li>
        </ul>
      </div>
    </div>
  );
};

const Login = () => {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const { '*': slugRota } = useParams<{ '*': string }>();
  const [searchParams, setSearchParams] = useSearchParams();
  
  // Estados do Redux
  const { loginStatus, loginError, tipoAcesso, orgIdFromResponse } = useAppSelector((state) => state.auth);
  
  // Estados locais da UI (específicos da interface)
  const [showCodeConfirmationModal, setShowCodeConfirmationModal] = useState(false);
  const [showEmailInstructionsModal, setShowEmailInstructionsModal] = useState(false);
  const [timeLeft, setTimeLeft] = useState(60);
  const [isResendEnabled, setIsResendEnabled] = useState(false);
  const [otpValue, setOtpValue] = useState("");
  const [showErrorDialog, setShowErrorDialog] = useState(false);
  
  // Estados para cadastro
  const [showCadastroForm, setShowCadastroForm] = useState(false);
  const [showTermosModal, setShowTermosModal] = useState(false);
  const [aceitaTermos, setAceitaTermos] = useState(false);
  const [cadastroSucesso, setCadastroSucesso] = useState(false);
  const [isCadastrando, setIsCadastrando] = useState(false);
  const [senhaAtual, setSenhaAtual] = useState("");

  // Verificar se está na rota /login (raiz, sem slug)
  const isLoginRaiz = !slugRota || slugRota === '';

  const handleSSOClick = () => {
    try {
      clearFlutterFlowStorageIframe();
    } catch {
      // Não bloquear fluxo SSO
    }
    if (currentOrgConfig.possuiSSO) {
      const ssoUrl = getSsoUrlForCurrentEnv(currentOrgConfig);
      if (ssoUrl) {
        localStorage.setItem('ssoOrgId', currentOrgConfig.orgId.toString());
        setTimeout(() => {
          window.location.href = ssoUrl;
        }, 150);
      } else {
        console.error("[Login][SSO] URL de SSO não configurada para o ambiente atual.", {
          mode: import.meta.env.MODE,
          slugRota,
          orgId: currentOrgConfig.orgId,
          orgConfig: currentOrgConfig,
        });
      }
    }
  };

  // Lógica para obter a configuração de organização baseada no slug da rota
  const currentOrgConfig: OrgLoginConfig = useMemo(() => {
    if (slugRota) {
      console.log(`[Login] slugRota: ${slugRota}`);
      const config = orgLoginConfigs.find(config => config.slugRota === slugRota);
      if (config) {
        console.log(`[Login] Usando configuração para slugRota: ${slugRota}`, config);
        return config;
      }
    }
    // Retorna a configuração padrão se não houver slug ou se o slug não for encontrado
    const defaultConfig = orgLoginConfigs.find(config => config.slugRota === "default");
    console.log(`[Login] Usando configuração padrão: ${defaultConfig?.slugRota}`, defaultConfig);
    // Para rota /login raiz (sem slug), usar orgId: 1
    const orgIdParaUsar = (!slugRota || slugRota === '') ? 1 : (defaultConfig?.orgId || 8);
    return defaultConfig 
      ? { ...defaultConfig, orgId: orgIdParaUsar }
      : { slugRota: "default", orgId: orgIdParaUsar, possuiSSO: false, urlSSoDev: null, urlSSoHomolog: null, urlSSoProd: null, exibeFormularioEmail: true, nomeEmpresa: null };
  }, [slugRota]);

  console.log(`[Login] orgId em uso: ${currentOrgConfig.orgId}`);

  // Seleciona uma imagem aleatória que permanece durante toda a sessão da página
  const randomHeroImage = useMemo(() => {
    return heroImages[Math.floor(Math.random() * heroImages.length)];
  }, []);

  // Verificar se há parâmetro unauthorized na URL e mostrar modal
  useEffect(() => {
    const unauthorized = searchParams.get('unauthorized')
    if (unauthorized === 'true') {
      setShowErrorDialog(true)
      // Remover o parâmetro da URL após mostrar o modal
      searchParams.delete('unauthorized')
      setSearchParams(searchParams, { replace: true })
    }
  }, [searchParams, setSearchParams])

  // Efeito para abrir formulário de cadastro via URL
  useEffect(() => {
    const cadastroParam = searchParams.get('cadastro');
    if ((cadastroParam === 'true' || cadastroParam === '1') && !showCadastroForm) {
      // Abre o formulário apenas se ainda não estiver aberto
      setShowCadastroForm(true);
      cadastroForm.reset({
        nomeCompleto: "",
        cpf: "",
        email: "",
        senha: "",
        confirmaSenha: "",
      });
      setSenhaAtual("");
      setAceitaTermos(false);
    }
  }, [searchParams]); // Executa quando searchParams mudar

  // Novo useEffect para o countdown
  useEffect(() => {
    let timer: NodeJS.Timeout;
    if (showCodeConfirmationModal && timeLeft > 0) {
      timer = setTimeout(() => {
        setTimeLeft(timeLeft - 1);
      }, 1000);
    } else if (timeLeft === 0) {
      setIsResendEnabled(true);
    }
    return () => clearTimeout(timer);
  }, [showCodeConfirmationModal, timeLeft]);

  // Função para reenvio do código
  const handleResendCode = async () => {
    const emailData = form.getValues("email");
    if (emailData) {
      dispatch(clearLoginState());
      // Para rota /login raiz, usar null para chamar EnviaTokenAcessoEmailSemOrg
      const orgIdParaLogin = isLoginRaiz ? null : currentOrgConfig.orgId;
      const result = await dispatch(sendLoginToken({ email: emailData, orgId: orgIdParaLogin }));
      if (sendLoginToken.fulfilled.match(result) && result.payload.tipoAcesso === 0) {
        setTimeLeft(60);
        setIsResendEnabled(false);
        setOtpValue("");
      }
    }
  };
  
  // Função para lidar com a validação do OTP
  const handleOTPComplete = async (token: string) => {
    const emailData = form.getValues("email");
    if (!emailData) {
      setShowErrorDialog(true);
      return;
    }

    // Para rota raiz, priorizar orgIdFromResponse se disponível
    // Caso contrário, usar a lógica padrão (verificar se currentOrgConfig.orgId === 0 || null)
    const orgIdToUse = isLoginRaiz && orgIdFromResponse !== null && orgIdFromResponse !== undefined
      ? orgIdFromResponse
      : (currentOrgConfig.orgId === 0 || currentOrgConfig.orgId === null)
        ? (orgIdFromResponse || currentOrgConfig.orgId)
        : currentOrgConfig.orgId;

    const result = await dispatch(
      validateLoginToken({
        email: emailData,
        token: token,
        orgId: orgIdToUse,
      })
    );

    if (validateLoginToken.fulfilled.match(result)) {
      // Se o usuário não foi retornado na resposta, buscar o perfil
      if (!result.payload.usuario) {
        await dispatch(fetchShowmeProfile());
      }
      setShowCodeConfirmationModal(false);
      setOtpValue("");
      navigate('/dashboard');
    } else {
      setOtpValue("");
      setShowErrorDialog(true);
    }
  };
  
  const form = useForm<FormData>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      email: "",
    },
  });

  const cadastroForm = useForm<CadastroFormData>({
    resolver: zodResolver(cadastroSchema),
    mode: "onChange",
    defaultValues: {
      nomeCompleto: "",
      cpf: "",
      email: "",
      senha: "",
      confirmaSenha: "",
    },
  });


  // Função para submeter cadastro
  const onSubmitCadastro = async (data: CadastroFormData) => {
    if (!aceitaTermos) {
      toast.error("Você deve aceitar os termos de uso para continuar");
      return;
    }

    // Validar CPF se foi preenchido
    if (data.cpf && data.cpf.trim() !== '') {
      if (!validarCPF(data.cpf)) {
        toast.error("CPF inválido. Por favor, verifique o CPF informado.");
        cadastroForm.setError("cpf", {
          type: "manual",
          message: "CPF inválido"
        });
        return;
      }
    }

    setIsCadastrando(true);
    try {
      // Usar Use Case seguindo a arquitetura: Page → Use Case → Repository → API
      const useCase = container.resolve(CadastrarUsuarioUseCase);
      
      // Remover formatação do CPF
      const cpfSemFormatacao = data.cpf?.replace(/\D/g, "") || "";
      
      const payload = {
        cpf: cpfSemFormatacao,
        email: data.email,
        nome_completo: data.nomeCompleto,
        senha: data.senha,
      };

      await useCase.execute(payload);

      // Sucesso - ocultar formulário, mostrar feedback e exibir login
      setCadastroSucesso(true);
      setShowCadastroForm(false);
      
      // Limpar formulário
      cadastroForm.reset();
      setAceitaTermos(false);
      setSenhaAtual("");
      
      // Ocultar feedback após 3 segundos e mostrar formulário de login
      setTimeout(() => {
        setCadastroSucesso(false);
      }, 3000);
    } catch (error: any) {
      console.error('Erro ao cadastrar:', error);
      toast.error(error.message || "Erro ao realizar cadastro. Tente novamente.");
    } finally {
      setIsCadastrando(false);
    }
  };

  const onSubmit = async (data: FormData) => {
    try {
      clearFlutterFlowStorageIframe();
    } catch {
      // Não bloquear fluxo de login
    }
    dispatch(clearLoginState());
    // Para rota /login raiz, usar null para chamar EnviaTokenAcessoEmailSemOrg
    const orgIdParaLogin = isLoginRaiz ? null : currentOrgConfig.orgId;
    const result = await dispatch(
      sendLoginToken({ email: data.email, orgId: orgIdParaLogin })
    );

    if (sendLoginToken.fulfilled.match(result)) {
      // Se tipoAcesso === 0, abrir modal de confirmação de código
      // Se tipoAcesso !== 0, abrir modal de instruções de email
      if (result.payload.tipoAcesso === 0) {
        setShowCodeConfirmationModal(true);
        setTimeLeft(60);
        setIsResendEnabled(false);
      } else {
        setShowEmailInstructionsModal(true);
      }
    } else {
      setShowErrorDialog(true);
    }
  };

  // Efeito para monitorar erros do Redux
  useEffect(() => {
    if (loginError && loginStatus === 'failed') {
      setShowErrorDialog(true);
    }
  }, [loginError, loginStatus]);

  // Efeito para redirecionar após login bem-sucedido
  useEffect(() => {
    if (loginStatus === 'succeeded' && tipoAcesso !== null && !showCodeConfirmationModal) {
      // Login foi validado com sucesso, redirecionar
      navigate('/dashboard');
    }
  }, [loginStatus, tipoAcesso, showCodeConfirmationModal, navigate]);

  return (
    <>
    <div className="min-h-screen flex">
      {/* Coluna Esquerda - Imagem com Película Roxa */}
      <div className="hidden lg:flex lg:w-1/2 relative overflow-hidden bg-gradient-to-br from-purple-900 to-indigo-900">
        <img 
          src={randomHeroImage} 
          alt="Cultura Fourmakers" 
          className="absolute inset-0 w-full h-full object-cover"
        />
        <div className="absolute inset-0 bg-gradient-to-br from-purple-600/80 via-purple-700/75 to-indigo-700/80" />
        
        {/* Marca d'água - Logo */}
        <div className="absolute -bottom-0 -right-0 opacity-20">
          <img 
            src="/fourmakers_favicon.svg" 
            alt="" 
            className="w-[600px] h-[600px] [filter:brightness(0)_saturate(100%)_invert(35%)_sepia(91%)_saturate(3458%)_hue-rotate(257deg)_brightness(92%)_contrast(103%)]"
          />
        </div>
        
        <div className="relative z-10 flex flex-col justify-between h-full pb-24 px-12 text-white">
          {/* Conteúdo superior - Novidades */}
          <div className="pt-24">
            <div className="bg-white/10 backdrop-blur-sm rounded-lg p-6 border border-white/20">
              <h2 className="text-xl font-bold mb-4 flex items-center gap-2">
                Novidades na plataforma 🚀
              </h2>
              <p className="text-sm leading-relaxed opacity-95 mb-4">
                Nossa plataforma passou por uma atualização importante!
                Modernizamos a tecnologia e ajustamos o visual para oferecer uma experiência mais fluida, atual e eficiente.
              </p>
              
              <div className="space-y-3 text-sm">
                <div>
                  <p className="font-semibold mb-2">👉 O que muda?</p>
                  <ul className="list-disc list-inside space-y-1 opacity-90 ml-2">
                    <li>Performance e estabilidade aprimoradas</li>
                    <li>Interface mais moderna e intuitiva</li>
                  </ul>
                </div>
                
                <div>
                  <p className="font-semibold mb-2">👉 O que permanece igual?</p>
                  <ul className="list-disc list-inside space-y-1 opacity-90 ml-2">
                    <li>Todas as funcionalidades continuam as mesmas</li>
                    <li>Seus dados, fluxos e acessos não foram alterados</li>
                  </ul>
                </div>
              </div>
              
              <p className="text-sm leading-relaxed opacity-90 mt-4">
                Essa evolução foi pensada para apoiar você no dia a dia, com mais qualidade e conforto na navegação.
                Qualquer dúvida, nosso time está à disposição. 😊
              </p>
            </div>
          </div>

          {/* Conteúdo inferior - Mensagem original */}
          <div>
            <h1 className="text-[1.75rem] font-bold mb-3 leading-tight">
              Chegou o Fourmakers!
            </h1>
            <p className="text-base leading-snug opacity-95">
              Uma nova era na gestão de talentos, conectando<br />
              empresas e profissionais para transformar o futuro.
            </p>
          </div>
        </div>
      </div>

      {/* Coluna Direita - Formulário de Login */}
      <div className="w-full lg:w-1/2 flex items-center justify-center bg-background p-8">
        <div className="w-full max-w-md">
          {/* Card de Login */}
          <Card className="shadow-xl border-0">
            <CardContent className="pt-8">
              {/* Logo */}
              <div className="flex flex-col items-center mb-8">
                <img 
                  src={logoFourmakers} 
                  alt="FourMakers" 
                  className="h-12 w-auto dark:invert"
                />
                {currentOrgConfig.nomeEmpresa && (
                  <Badge variant="outline" className="mt-4 text-sm">
                    {currentOrgConfig.nomeEmpresa}
                  </Badge>
                )}
              </div>

              {/* Feedback de sucesso do cadastro */}
              {cadastroSucesso && (
                <div className="mb-6 p-4 bg-green-50 dark:bg-green-900/20 border border-green-200 dark:border-green-800 rounded-lg">
                  <div className="flex items-center gap-2 text-green-700 dark:text-green-400">
                    <CheckCircle2 className="h-5 w-5" />
                    <p className="text-sm font-medium">
                      Cadastro realizado com sucesso! Verifique seu e-mail para continuar.
                    </p>
                  </div>
                </div>
              )}

              {/* Formulário de Cadastro */}
              {showCadastroForm && !cadastroSucesso ? (
                <Form {...cadastroForm}>
                  <form onSubmit={cadastroForm.handleSubmit(onSubmitCadastro)} className="space-y-4">
                    <div className="space-y-2">
                      <Label htmlFor="nomeCompleto">Nome completo *</Label>
                      <Input 
                        id="nomeCompleto"
                        type="text"
                        placeholder="Seu nome completo"
                        {...cadastroForm.register("nomeCompleto")}
                      />
                      {cadastroForm.formState.errors.nomeCompleto && (
                        <p className="text-sm font-medium text-destructive">
                          {cadastroForm.formState.errors.nomeCompleto.message}
                        </p>
                      )}
                    </div>

                    <FormField
                      control={cadastroForm.control}
                      name="cpf"
                      render={({ field }) => (
                        <FormItem>
                          <FormLabel>CPF</FormLabel>
                          <FormControl>
                            <Input 
                              placeholder="000.000.000-00"
                              maxLength={14}
                              {...field}
                              onChange={(e) => {
                                const formatted = formatCPF(e.target.value);
                                field.onChange(formatted);
                              }}
                            />
                          </FormControl>
                          <FormMessage />
                        </FormItem>
                      )}
                    />

                    <FormField
                      control={cadastroForm.control}
                      name="email"
                      render={({ field }) => (
                        <FormItem>
                          <FormLabel>E-mail *</FormLabel>
                          <FormControl>
                            <Input 
                              type="email"
                              placeholder="seu.email@exemplo.com" 
                              {...field} 
                            />
                          </FormControl>
                          <FormMessage />
                        </FormItem>
                      )}
                    />

                    <FormField
                      control={cadastroForm.control}
                      name="senha"
                      render={({ field }) => (
                        <FormItem>
                          <FormLabel>Senha *</FormLabel>
                          <FormControl>
                            <Input 
                              type="password"
                              placeholder="Mínimo 8 caracteres" 
                              {...field}
                              onChange={(e) => {
                                field.onChange(e);
                                setSenhaAtual(e.target.value);
                              }}
                            />
                          </FormControl>
                          <FormMessage />
                        </FormItem>
                      )}
                    />

                    <FormField
                      control={cadastroForm.control}
                      name="confirmaSenha"
                      render={({ field }) => (
                        <FormItem>
                          <FormLabel>Confirme a senha *</FormLabel>
                          <FormControl>
                            <Input 
                              type="password"
                              placeholder="Digite a senha novamente" 
                              {...field} 
                            />
                          </FormControl>
                          <FormMessage />
                        </FormItem>
                      )}
                    />

                    {/* Componente de Força de Senha */}
                    <PasswordStrength password={senhaAtual} />

                    <div className="flex items-start space-x-2 pt-2">
                      <Checkbox
                        id="aceita-termos"
                        checked={aceitaTermos}
                        onCheckedChange={(checked) => setAceitaTermos(checked === true)}
                      />
                      <label
                        htmlFor="aceita-termos"
                        className="text-sm leading-none peer-disabled:cursor-not-allowed peer-disabled:opacity-70 cursor-pointer"
                      >
                        <span
                          className="text-primary underline hover:no-underline"
                          onClick={(e) => {
                            e.preventDefault();
                            setShowTermosModal(true);
                          }}
                        >
                          Li e Aceito os Termos de Uso e Condições
                        </span>
                      </label>
                    </div>

                    <div className="flex gap-2 pt-2">
                      <Button 
                        type="button"
                        variant="outline"
                        className="flex-1"
                        onClick={() => {
                          setShowCadastroForm(false);
                          cadastroForm.reset();
                          setAceitaTermos(false);
                        }}
                      >
                        Cancelar
                      </Button>
                      <Button 
                        type="submit" 
                        className="flex-1 h-11 text-base" 
                        disabled={!aceitaTermos || isCadastrando}
                      >
                        {isCadastrando && (
                          <Spinner className="mr-2 text-current" size={16} />
                        )}
                        Cadastrar
                      </Button>
                    </div>
                  </form>
                </Form>
              ) : (
                /* Formulário de Login (existente) */
                <Form {...form}>
                  {currentOrgConfig.exibeFormularioEmail && (
                    <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
                      <FormField
                        control={form.control}
                        name="email"
                        render={({ field }) => (
                          <FormItem>
                            <FormLabel>Email</FormLabel>
                            <FormControl>
                              <Input 
                                type="email"
                                placeholder="seu.email@empresa.com" 
                                {...field} 
                              />
                            </FormControl>
                            <FormMessage />
                          </FormItem>
                        )}
                      />

                      <Button 
                        type="submit" 
                        className="w-full h-11 text-base" 
                        disabled={loginStatus === 'sending'}
                      >
                        <span className="flex items-center justify-center gap-2">
                          {loginStatus === 'sending' && (
                            <Spinner className="h-4 w-4 text-current" size={16} />
                          )}
                          <span>Entrar</span>
                        </span>
                      </Button>
                    </form>
                  )}
                </Form>
              )}

              {/* Botão Cadastre-se - só aparece na rota /login (raiz) */}
              {!showCadastroForm && isLoginRaiz && currentOrgConfig.exibeFormularioEmail && (
                <>
                  <div className="flex items-center gap-4 my-6">
                    <div className="flex-1 h-px bg-border"></div>
                    <span className="text-sm text-muted-foreground">ou</span>
                    <div className="flex-1 h-px bg-border"></div>
                  </div>
                  <Button 
                    type="button"
                    variant="outline"
                    className="w-full h-11 text-base"
                    onClick={() => {
                      // Adiciona o parâmetro cadastro=true na URL
                      const newSearchParams = new URLSearchParams(searchParams);
                      newSearchParams.set('cadastro', 'true');
                      const currentPath = slugRota ? `/login/${slugRota}` : '/login';
                      navigate(`${currentPath}?${newSearchParams.toString()}`, { replace: false });
                      
                      // Abre o formulário de cadastro
                      setShowCadastroForm(true);
                      cadastroForm.reset({
                        nomeCompleto: "",
                        cpf: "",
                        email: "",
                        senha: "",
                        confirmaSenha: "",
                      });
                      setSenhaAtual("");
                      setAceitaTermos(false);
                    }}
                  >
                    Cadastre-se
                  </Button>
                </>
              )}

              {currentOrgConfig.exibeFormularioEmail && currentOrgConfig.possuiSSO && (
                <div className="flex items-center gap-4 my-6">
                  <div className="flex-1 h-px bg-border"></div>
                  <span className="text-sm text-muted-foreground">ou</span>
                  <div className="flex-1 h-px bg-border"></div>
                </div>
              )}

              {currentOrgConfig.possuiSSO && (
                <Button 
                  type="button" 
                  className="w-full h-11 text-base bg-[#0078D4] hover:bg-[#0063B1] text-white border-0"
                  onClick={handleSSOClick}
                >
                  <svg className="w-5 h-5 mr-2" viewBox="0 0 88 88" fill="none" xmlns="http://www.w3.org/2000/svg">
                    <path d="M0 0H41.9048V41.9048H0V0Z" fill="white"/>
                    <path d="M46.0952 0H88V41.9048H46.0952V0Z" fill="white"/>
                    <path d="M0 46.0952H41.9048V88H0V46.0952Z" fill="white"/>
                    <path d="M46.0952 46.0952H88V88H46.0952V46.0952Z" fill="white"/>
                  </svg>
                  Com SSO Microsoft
                </Button>
              )}
            </CardContent>
          </Card>

          {/* Footer */}
          <p className="text-center text-sm text-muted-foreground mt-6">
            © 2025 FourMakers. Todos os direitos reservados.
          </p>
        </div>
      </div>
    </div>

      {/* Modal de Confirmação de Código */}
      <Dialog open={showCodeConfirmationModal} onOpenChange={setShowCodeConfirmationModal}>
        <DialogContent className="sm:max-w-lg top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2"> {/* Removido 'relative', adicionadas classes de centralização */}
          {loginStatus === 'validating' && (
            <div className="absolute inset-0 flex items-center justify-center bg-background/80 z-50 rounded-lg">
              <Spinner className="h-12 w-12 text-primary" />
            </div>
          )}
          <DialogHeader className="flex flex-row items-center justify-start gap-2">
            <CheckCircle2 className="h-8 w-8 text-green-500" />
            <DialogTitle className="text-2xl font-bold m-0">E-mail enviado</DialogTitle>
          </DialogHeader>
          <DialogDescription className="text-base text-muted-foreground max-w-sm text-left">
            Foi enviado um e-mail com instruções para acessar a plataforma Fourmakers.
            {tipoAcesso === 0 && "Informe o código enviado para seu e-mail."}
          </DialogDescription>
          {tipoAcesso === 0 && (
            <div className="grid gap-4 py-4">
              <div className="text-left">
                <h4 className="text-lg font-medium mb-4">Código</h4>
                <InputOTP maxLength={6} value={otpValue} onChange={(value) => setOtpValue(value)} onComplete={handleOTPComplete}>
                  <InputOTPGroup className="flex justify-center w-full gap-4">
                    <InputOTPSlot index={0} className="w-16 h-16 text-2xl rounded-md border border-gray-300 focus:border-gray-700" />
                    <InputOTPSlot index={1} className="w-16 h-16 text-2xl rounded-md border border-gray-300 focus:border-gray-700" />
                    <InputOTPSlot index={2} className="w-16 h-16 text-2xl rounded-md border border-gray-300 focus:border-gray-700" />
                    <InputOTPSlot index={3} className="w-16 h-16 text-2xl rounded-md border border-gray-300 focus:border-gray-700" />
                    <InputOTPSlot index={4} className="w-16 h-16 text-2xl rounded-md border border-gray-300 focus:border-gray-700" />
                    <InputOTPSlot index={5} className="w-16 h-16 text-2xl rounded-md border border-gray-300 focus:border-gray-700" />
                  </InputOTPGroup>
                </InputOTP>
              </div>
              <div className="text-left text-sm text-muted-foreground mt-4">
                {!isResendEnabled ? (
                  <span>Não recebeu? Solicite um novo código em 00:{timeLeft < 10 ? `0${timeLeft}` : timeLeft}</span>
                ) : (
                  <>
                    Está com problemas?{" "}
                    <Button variant="link" className="p-0 h-auto" onClick={handleResendCode}>
                      Reenviar código
                    </Button>
                  </>
                )}
              </div>
            </div>
          )}
        </DialogContent>
      </Dialog>

      {/* Modal de Instruções de Email */}
      <Dialog open={showEmailInstructionsModal} onOpenChange={setShowEmailInstructionsModal}>
        <DialogContent className="sm:max-w-lg top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2">
          <DialogHeader className="flex flex-row items-center justify-start gap-2">
            <CheckCircle2 className="h-8 w-8 text-green-500" />
            <DialogTitle className="text-2xl font-bold m-0">E-mail enviado</DialogTitle>
          </DialogHeader>
          <DialogDescription className="text-base text-muted-foreground max-w-sm text-left">
            Foi enviado um e-mail com instruções para acessar a plataforma Fourmakers.
            Por favor, verifique sua caixa de entrada e siga as instruções contidas no e-mail.
          </DialogDescription>
          <div className="flex justify-end mt-4">
            <Button onClick={() => setShowEmailInstructionsModal(false)}>
              Fechar
            </Button>
          </div>
        </DialogContent>
      </Dialog>

      {/* Dialog de Erro - Renderização condicional */}
      {showErrorDialog && (
        <AlertDialog open={showErrorDialog} onOpenChange={setShowErrorDialog}>
          <AlertDialogContent>
            <AlertDialogHeader>
              <AlertDialogTitle>Sessão expirada</AlertDialogTitle>
              <AlertDialogDescription>
                {loginError || "Sua sessão expirou. Por favor, faça login novamente."}
              </AlertDialogDescription>
            </AlertDialogHeader>
            <AlertDialogFooter>
              <AlertDialogAction onClick={() => setShowErrorDialog(false)}>Fechar</AlertDialogAction>
            </AlertDialogFooter>
          </AlertDialogContent>
        </AlertDialog>
      )}

      {/* Modal de Termos de Uso */}
      <Dialog open={showTermosModal} onOpenChange={setShowTermosModal}>
        <DialogContent className="sm:max-w-4xl max-h-[90vh] overflow-y-auto">
          <DialogHeader>
            <DialogTitle>Termos e Condições para o Tratamento de Dados Pessoais e Sensíveis</DialogTitle>
            <DialogDescription>
              Termos e Condições para o Tratamento de Dados Pessoais e Sensíveis de Candidatos a Vagas de Trabalho disponibilizadas pela FOURSYS com base no Art. 7º e 11, da Lei Geral de Proteção de Dados
            </DialogDescription>
          </DialogHeader>
          <div className="py-4 space-y-4 text-sm text-muted-foreground">
            <p>
              O CANDIDATO (Titular de dados) declara estar ciente e concorda que ao efetuar o cadastro site www.foursys.com.br na área de Recrutamento e Seleção anexando o seu currículo e/ou documentos, faz automaticamente a adesão e confere o consentimento de forma livre, informada e inequívoca por prazo indeterminado e mesmo após encerramento do processo seletivo, concordando com os termos e condições para o tratamento de seus dados pessoais para as finalidades específicas a seguir mencionadas, em conformidade com a Lei nº 13.709/2018 (Lei Geral de Proteção de Dados Pessoais - LGPD) para a empresa FOURSYS PROJETOS E SISTEMAS EM INFORMÁTICA LTDA., inscrita no CNPJ nº 03.808.125/0001-30, localizada na Av. Copacabana, 190, 9º andar, Empresarial 18 do Forte, CEP 06472-001, Barueri, SP, Telefone: (011) 4134-2222, e-mail dpo@foursys.com.br doravante denominada como FOURSYS (Controladora).
            </p>
            
            <p>
              A FOURSYS como controladora de dados pode tomar as decisões referentes ao tratamento de seus dados pessoais do CANDIDATO, bem como realizar o tratamento de seus dados pessoais, envolvendo operações como as que se referem a coleta, produção, recepção, classificação, utilização, acesso, reprodução, transmissão, distribuição, processamento, arquivamento, armazenamento, eliminação, avaliação ou controle da informação, modificação, comunicação, transferência, difusão ou extração para atendimento da finalidade do tratamento.
            </p>

            <div className="space-y-3">
              <h4 className="font-semibold text-foreground text-base">I – DADOS PESSOAIS DO CANDIDATO</h4>
              <p>
                A FOURSYS fica autorizada a tomar decisões referentes ao tratamento e a realizar o tratamento dos seguintes dados pessoais do CANDIDATO:
              </p>
              <ul className="list-disc list-inside space-y-1 ml-4">
                <li>Nome completo e do(a) cônjuge;</li>
                <li>Certidão de Nascimento e/ou Casamento;</li>
                <li>Estado civil;</li>
                <li>Endereço completo;</li>
                <li>Números de telefone, WhatsApp e endereços de e-mail.</li>
                <li>Data de nascimento;</li>
                <li>Nome dos pais e dos filhos e – Certidão de nascimento dos filhos menores de 14 anos, Carteira de vacinação dos menores de 7 anos, e atestado de matrícula e frequência escolar semestral dos maiores de 4 anos;</li>
                <li>Número e imagem da Carteira de Identidade (RG);</li>
                <li>Número e imagem do Cadastro de Pessoas Físicas (CPF);</li>
                <li>Número e imagem da Carteira Nacional de Habilitação (CNH);</li>
                <li>Número e imagem da Carteira de Trabalho e da Previdência Social (CTPS);</li>
                <li>Número e imagem da PIS;</li>
                <li>Número e imagem do Título de Eleitor;</li>
                <li>Fotografia 3x4;</li>
                <li>Nível de instrução ou escolaridade;</li>
                <li>Atestado de Saúde e Exames especialmente admissionais, periódicos, incluídos de retorno por afastamento superior a 30 dias em caso de doença, acidente ou parto, de mudança de função, demissionais e ainda aqueles que atestem doença ou acidente;</li>
                <li>Documento de filiação a Sindicato;</li>
                <li>Certidões e Declarações de regularidade financeira, fiscal e jurídica;</li>
                <li>Banco, agência e número de contas bancárias.</li>
                <li>Demais dados que venham a ser necessários para avaliação como para eventual contratação;</li>
                <li>Comunicação, verbal e escrita, mantida entre o Titular e o Controlador;</li>
                <li>Gravação de áudio e/ou vídeo de entrevistas para avaliação.</li>
              </ul>
              <p>
                Em caso de futuras vagas abertas pela FOURSYS, o CANDIDATO poderá ser ou não contatado para atualização de cadastro e/ou de currículo por meio dos contatos disponibilizados por ele a FOURSYS, sendo certo que a FOURSYS se exime de responsabilidade caso tais contatos estejam desatualizados.
              </p>
            </div>

            <div className="space-y-3">
              <h4 className="font-semibold text-foreground text-base">II – FINALIDADES, ADEQUAÇÃO E NECESSIDADE DO TRATAMENTO DOS DADOS</h4>
              <p>
                O tratamento de dados pessoais e sensíveis do CANDIDATO atenderão aos princípios da boa-fé, finalidade, adequação e necessidade, livre acesso, qualidade dos dados, transparência, segurança, prevenção, não discriminação, responsabilização e prestação de contas previstos no artigo 6º, da LGPD.
              </p>
              <p>
                O tratamento dos dados pessoais e sensíveis listados nestes Termos e Condições visa atender 3 finalidades previstas no artigo 7º, da LGPD:
              </p>
              <ul className="list-disc list-inside space-y-2 ml-4">
                <li>
                  <strong>Cadastro para se candidatar a vagas anunciadas ou futura vagas</strong> (art. 7º, I e IX, e 11, da LGPD – Consentimento e Legítimo Interesse) – a FOURSYS como prestadora de serviços no setor de tecnologia prioriza como política de gestão de capital humano a atração de talentos, e para isto necessita de banco de dados organizado de profissionais habilitados de acordo com as suas competências para atendimento da demanda de seus clientes. O cadastro, os dados, as entrevistas e as gravações do CANDIDATO garantem e o beneficia para que em vagas anunciadas ou futura vagas, este possa participar do processo de recrutamento e seleção;
                </li>
                <li>
                  <strong>Cadastro para procedimentos preliminares de contratação</strong> (art. 7º, V, e 11, da LGPD – Pré-contrato ou Contrato de Trabalho) – os dados e informações tanto do CANDIDATO como de sua filiação, visam atender a pré-contratação ou necessários a contratação, visando facilitar o cumprimento das leis trabalhistas, previdenciárias, fundiárias, tributárias, contábeis, sindicais, bancários, securitárias e de segurança e de saúde;
                </li>
                <li>
                  <strong>Cadastro para cumprimento de obrigação legal ou regulatória</strong> (art. 7º, II, e 11, da LGPD - Lei e Regulatório) – os dados e informações tanto do CANDIDATO como de sua filiação, visam atender ao cumprimento das leis trabalhistas, previdenciárias, fundiárias, tributárias, contábeis, sindicais e bancários. Além disso, a FOURSYS atende clientes do mercado financeiro e de capitais, as quais estão submetidas as normas do Banco Central do Brasil, Comissão de Valores Mobiliário, BOVESPA, Basileia I, II e III, entre outros órgãos reguladores, os quais podem exigir determinados cadastros para garantir a segurança e obrigações a serem assumidas pelo CANDIDATO.
                </li>
              </ul>
            </div>

            <div className="space-y-3">
              <h4 className="font-semibold text-foreground text-base">III – COMPARTILHAMENTO DOS DADOS DO CANDIDATO</h4>
              <p>
                A FOURSYS não utilizará, compartilhará, comercializará, ou atribuirá qualquer outra finalidade aos dados pessoais e sensíveis coletados do CANDIDATO, que seja diferente das finalidades aqui especificadas para o integral cumprimento do objeto destes Termos e Condições de Tratamento de Dados podendo manter armazenados em seus bancos de dados para preenchimento de vagas anunciadas ou futuras vagas.
              </p>
              <p>
                Quando o CANDIDATO efetuar o cadastro e envio de currículo, dados e outros documentos, a FOURSYS fica autorizada automaticamente a compartilhar os dados pessoais e sensíveis do CANDIDATO com o(s) cliente(s) da FOURSYS em que poderão ser envolvidos nos projetos para avaliação do CANDIDATO quanto ao atendimento dos requisitos técnicos, de habilidade e adequações as regras de compliance e de regulação com a finalidade.
              </p>
              <p>
                Após a escolha do CANDIDATO para preenchimento da vaga, a FOURSYS fica autorizada automaticamente a compartilhar os dados pessoais e sensíveis do CANDIDATO com órgãos e agentes públicos e fiscalizadores tais como Receita Federal, Previdência Social, Sindicato, Caixa Econômica Federal, Banco Central, outros previstos em lei, bem como os operadores de dados autorizados por lei como o contabilista da FOURSYS, e seus funcionários, advogados, auditores, consultores legalmente autorizados, planos de saúde e odontológico, benefícios (vale refeição, vale transporte, vale combustível, seguro de vida, entre outros) com o objetivo de contratação caso seja escolhido para preenchimento da vaga, bem como para cadastros em sistemas do(s) cliente(s) da FOURSYS sempre de acordo com a necessidade e atendimento da finalidade contratual e regulatória perante o cliente da FOURSYS.
              </p>
              <p>
                O CANDIDATO declara ainda estar ciente e concordar que seus dados pessoais e sensíveis poderão ser objeto de transferência e/ou compartilhamento internacional a depender da vaga a ser preenchida e dentro dos limites dos princípios da boa-fé, finalidade, adequação e necessidade previstos no artigo 6º, da LGPD, em decorrência da FOURSYS possuir escritórios, filiais e agentes fora do Brasil, de modo que a intervenção desta pode vir ser necessária para a decisão de preenchimento de vagas específicas, bem como clientes estrangeiros.
              </p>
              <p>
                Caso seja necessário o compartilhamento de dados com terceiros que não se relacionem com as finalidades acima mencionadas, a FOURSYS compromete-se a requerer do CANDIDATO um termo de consentimento específico para este fim (§ 6° do artigo 8° e § 2° do artigo 9° da Lei n° 13.709/2018).
              </p>
            </div>

            <div className="space-y-3">
              <h4 className="font-semibold text-foreground text-base">IV – SEGURANÇA DOS DADOS DO CANDIDATO</h4>
              <p>
                A FOURSYS mantém medidas de segurança, técnicas e administrativas suficientes a proteger os dados pessoais e sensíveis do CANDIDATO e à Autoridade Nacional de Proteção de Dados (ANPD), utilizando de serviços de infraestrutura de nuvem em estruturas e certificações de garantia de segurança reconhecidas mundialmente, incluindo ISO/IEC 27001, ISO/IEC 27017, ISO/IEC 27018, PCI DSS Level 1 e SOC 1, 2 e 3. Essas medidas de segurança técnica e organizacional são validadas por auditores externos independentes e são projetadas para impedir o acesso não autorizado ao conteúdo ou a divulgação não autorizada desse conteúdo.
              </p>
              <p>
                A FOURSYS manterá registro das operações de tratamento de dados pessoais e sensíveis que realizar em decorrência desses Termos e Condições de Tratamento de Dados, contendo no mínimo a descrição dos tipos de dados coletados, bem como a metodologia e mecanismos de mitigação de riscos adotados para a garantia da segurança das informações, dentro dos padrões internacionais e certificações ISO 27001:2013 e ISO 9001:2015, o qual a FOURSYS mantem atualizadas e auditadas.
              </p>
              <p>
                Muito embora, a FOURSYS adote as melhores práticas de governança e conta com os serviços de empresas renomadas para o tratamento de dados, esta não tem como garantir que os dados do CANDIDATO venham a ser vazados em decorrência da utilização de tecnologia de quebra e violação de códigos por criminosos digitais e grupos (hackers, crackers, etc.), porém em caso de ocorrência de incidente de segurança a FOURSYS, em atenção ao art. 48 da Lei nº 13.709/2018, comunicará ao CANDIDATO e à Autoridade Nacional de Proteção de Dados (ANPD) sobre o ocorrido, prestando as informações necessárias e esclarecendo quais medidas foram adotadas para solucionar o ocorrido, inclusive no que tange a investigações por meio dos agentes públicos.
              </p>
            </div>

            <div className="space-y-3">
              <h4 className="font-semibold text-foreground text-base">V - TÉRMINO DO TRATAMENTO DOS DADOS</h4>
              <p>
                À FOURSYS, é permitida manter e utilizar os dados pessoais do CANDIDATO durante todo o processo seletivo e em caso de contratação durante todo período contratualmente firmado para as finalidades relacionadas nesses Termos e Condições de Tratamento de Dados, e ainda após o término da contratação para cumprimento de obrigação legal ou impostas por órgãos de fiscalização, nos termos do artigo 16 da Lei n° 13.709/2018.
              </p>
              <p>
                A FOURSYS poderá adotar ainda medidas seguras de anonimização, de criptografia ou de alteração de ambiente tecnológico para armazenamento dos dados do CANDIDATO podendo manter por tempo indefinido mesmo após o término do tratamento, mas sempre respeitando a finalidade, adequação e a necessidade, previstos no art. 6º, da LGPD.
              </p>
            </div>

            <div className="space-y-3">
              <h4 className="font-semibold text-foreground text-base">VI - TEMPO DE PERMANÊNCIA E EXCLUSÃO DOS DADOS DO CANDIDATO</h4>
              <p>
                O CANDIDATO fica ciente de que a FOURSYS deverá permanecer com os seus dados pelo período mínimo de guarda de documentos trabalhistas, previdenciários, tributários e funciários, bem como os relacionados à segurança e saúde no trabalho, mesmo após o encerramento do vínculo empregatício.
              </p>
              <p>
                O CANDIDATO, caso queira solicitar a exclusão de seus dados pessoais e sensíveis, poderá fazê-lo somente por meio de manifestação expressa que deverá ser direcionada no e-mail dpo@foursys.com.br, conforme o artigo 8°, § 5°, da Lei n° 13.709/2018.
              </p>
              <p>
                A FOURSYS avaliará o pedido de exclusão dos dados do CANDIDATO e responderá por e-mail a respeito da solicitação, sendo que dependendo das finalidades do tratamento, o CANDIDATO já fica ciente de que a FOURSYS poderá permanecer utilizando os dados para as seguintes finalidades:
              </p>
              <ul className="list-disc list-inside space-y-1 ml-4">
                <li>Para cumprimento de obrigações decorrentes da legislação trabalhista e previdenciária, incluindo o disposto em Acordo ou Convenção Coletiva da categoria da FOURSYS;</li>
                <li>Para procedimentos de admissão e execução do contrato de trabalho, inclusive após seu término;</li>
                <li>Para cumprimento, pela FOURSYS, de obrigações impostas por órgãos de fiscalização e de regulação, e/ou de obrigações contratuais perante o(s) cliente(s) da FOURSYS;</li>
                <li>Para o exercício regular de direitos em processo judicial, administrativo ou arbitral;</li>
                <li>Para a proteção da vida ou da incolumidade física do titular ou de terceiros;</li>
                <li>Para a tutela da saúde, exclusivamente, em procedimento realizado por profissionais de saúde, serviços de saúde ou autoridade sanitária;</li>
                <li>Quando necessário para atender aos interesses legítimos da FOURSYS ou de terceiros, exceto no caso de prevalecerem direitos e liberdades fundamentais do titular que exijam a proteção dos dados pessoais.</li>
              </ul>
            </div>

            <div className="space-y-3">
              <h4 className="font-semibold text-foreground text-base">VII – VIGÊNCIA DOS TERMOS E CONDIÇÕES DE TRATAMENTO DE DADOS</h4>
              <p>
                As regras, termos e condições de Tratamento de Dados Pessoais e Sensíveis da FOURSYS espelham as medidas que sempre foram adotadas pela FOURSYS no recrutamento e seleção com base nas boas práticas de governança de dados dos candidatos a vagas, e baseadas nos padrões internacionais e certificações ISO 27001:2013 e ISO 9001:2015, e estão em vigor desde a vigência da Lei Geral de Proteção de Dados.
              </p>
              <p>
                Caso a Lei de Proteção Geral de Proteção de Dados venha a ser alterada ou revogada, ou melhor regulamentada pela Autoridade Nacional de Proteção de Dados (ANPD) de modo que exista uma influência direta nos termos aqui estabelecidos, adequar-se-á às novas regras vigentes quanto ao tratamento e proteção de dados pessoais.
              </p>
            </div>

            <div className="space-y-3 pt-4 border-t">
              <p className="text-xs">
                Barueri, {format(new Date(), "d 'de' MMMM 'de' yyyy", { locale: ptBR })}.
              </p>
              <div className="text-xs space-y-1">
                <p className="font-semibold text-foreground">FOURSYS PROJETOS E SISTEMAS EM INFORMÁTICA LTDA.</p>
                <p>CNPJ nº 03.808.125/0001-30</p>
                <p>Av. Copacabana, 190, 9º andar, Empresarial 18 do Forte, CEP 06472-001, Barueri, SP</p>
                <p>Telefone: (011) 4134-2222</p>
                <p>Encarregado de Dados: Nome e Sobrenome</p>
                <p>E-mail: dpo@foursys.com.br</p>
                <p>Responsáveis pelo Recrutamento e Seleção: Nome e Sobrenome (Unidade 1) e Nome e Sobrenome (Unidade 2)</p>
                <p>E-mail: rh@foursys.com.br</p>
              </div>
            </div>
          </div>
          <div className="flex justify-end mt-4">
            <Button onClick={() => setShowTermosModal(false)}>
              Fechar
            </Button>
          </div>
        </DialogContent>
      </Dialog>
    </>
  );
};

export default Login;
