import { useMemo, useEffect, useLayoutEffect, useState, useRef } from 'react';
import { useSearchParams, useLocation, useNavigate } from 'react-router-dom';
import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';
import * as z from 'zod';
import { container } from '@core/di/container';
import { GetVagaDetalhesPublicoUseCase } from '@domain/usecases/GetVagaDetalhesPublicoUseCase';
import { AddSkillToPerfil360UseCase } from '@domain/usecases/AddSkillToPerfil360UseCase';
import { RemoveSkillFromPerfil360UseCase } from '@domain/usecases/RemoveSkillFromPerfil360UseCase';
import { UpdateSkillLevelUseCase } from '@domain/usecases/UpdateSkillLevelUseCase';
import { BuscarDadosColaboradorUseCase } from '@domain/usecases/BuscarDadosColaboradorUseCase';
import { CadastrarUsuarioUseCase } from '@domain/usecases/CadastrarUsuarioUseCase';
import { RegistrarLgpdUseCase } from '@domain/usecases/RegistrarLgpdUseCase';
import { CandidatarSeUseCase } from '@domain/usecases/CandidatarSeUseCase';
import { AlterarFormularioColaboradorUseCase } from '@domain/usecases/AlterarFormularioColaboradorUseCase';
import { ListarModelosTrabalhoUseCase } from '@domain/usecases/ListarModelosTrabalhoUseCase';
import { ListarOpcoesContatoUseCase } from '@domain/usecases/ListarOpcoesContatoUseCase';
import { SincronizarCurriculoColaboradorUseCase } from '@domain/usecases/SincronizarCurriculoColaboradorUseCase';
import { GetSkillLevelsUseCase } from '@domain/usecases/GetSkillLevelsUseCase';
import type { MinhaJornadaSkillType } from '@domain/entities/MinhaJornadaSkill';
import type { VagaDetails } from '@domain/entities/VagaDetails';
import type { ModeloTrabalho } from '@domain/entities/GestaoVagasCandidatos';
import type { OpcaoContatoItem } from '@domain/entities/GestaoVagasCandidatos';
import type { BuscarDadosColaboradorResponse } from '@domain/entities/Profile360';
import type { AlterarFormularioColaboradorPayload } from '@domain/repositories/ColaboradoresRepository';
import { useAppDispatch, useAppSelector } from '@app/store/hooks';
import { sendLoginToken, validateLoginToken, clearLoginState, fetchShowmeProfile } from '@app/store/slices/authSlice';
import { validarCPF, formatCPF } from '@shared/utils/cpfUtils';
import { isCepComplete, formatCep, cepToApiFormat } from '@shared/utils/calculations';
import { useViaCep } from '@presentation/hooks/useViaCep';
import {
  groupSkills,
  skillsFaltantes,
  isCandidaturaJaExisteMessage,
  matchNivelPorDescricaoParaInclusao,
  parseBRLCurrencyInput,
} from '@presentation/utils/publicVagaUtils';
import { CANDIDATURA_EXTERNA_FLOW, CANDIDATURA_EXTERNA_ACTIONS } from '@shared/constants/candidaturaExternaAnalytics';
import { logUserAction } from '@shared/utils/firebaseAnalytics';
import { toast } from 'sonner';

export const ORG_ID_PUBLICO = 1;

const loginSchema = z.object({
  email: z.string().email('E-mail inválido'),
});

const cadastroSchema = z.object({
  nomeCompleto: z.string().min(3, 'Nome completo deve ter no mínimo 3 caracteres'),
  cpf: z
    .string()
    .min(1, 'CPF é obrigatório')
    .refine((v) => (v ?? '').replace(/\D/g, '').length >= 11, 'CPF deve ter 11 dígitos')
    .refine((v) => validarCPF((v ?? '').replace(/\D/g, '')), { message: 'CPF inválido' }),
  email: z.string().email('E-mail inválido'),
  senha: z.string().min(6, 'Senha deve ter no mínimo 6 caracteres'),
  confirmaSenha: z.string().min(6, 'Confirme a senha'),
}).refine((data) => data.senha === data.confirmaSenha, {
  message: 'As senhas não coincidem',
  path: ['confirmaSenha'],
});

const inscricaoSchema = z.object({
  cargoAtualUltimo: z.string().min(1, 'Informe o cargo'),
  salarioAtualUltimo: z.string().min(1, 'Informe o salário'),
  tipoContratoAtualUltimo: z.string().min(1, 'Selecione o tipo de contrato'),
  modalidadeAtualUltima: z.string().min(1, 'Selecione a modalidade'),
  contatoPrincipal: z.string().min(1, 'Informe o WhatsApp'),
  aceitaSugestoes: z.boolean(),
});

export type LoginFormData = z.infer<typeof loginSchema>;
export type CadastroFormData = z.infer<typeof cadastroSchema>;
export type InscricaoFormData = z.infer<typeof inscricaoSchema>;

export const TIPOS_CONTRATO = [
  { value: 'CLT', label: 'CLT' },
  { value: 'Cooperado', label: 'Cooperado' },
  { value: 'PJ', label: 'PJ' },
] as const;

export const MODALIDADES = [
  { value: '1', label: 'Presencial' },
  { value: '2', label: 'Híbrido' },
  { value: '3', label: 'Home Office' },
] as const;

export const MAX_FILE_CURRICULO_MB = 25;
export const ACCEPT_CURRICULO = '.png,.jpeg,.jpg,.doc,.docx,.pdf';
/** Mensagem exibida no campo de upload de currículo (step 2 vaga pública). */
export const CURRICULO_LABEL = `Formato .pdf de preferência o modelo exportado pelo LinkedIn (máx. ${MAX_FILE_CURRICULO_MB}MB)`;

export type PublicVagaLocationState = {
  detalhe?: VagaDetails | null;
  vagaId?: string;
  codigoVaga?: string | number;
} | null;

export function usePublicVagaDetalhe() {
  const [searchParams] = useSearchParams();
  const location = useLocation();
  const navigate = useNavigate();
  const codigoFromUrl = searchParams.get('detalhes');
  const state = location.state as PublicVagaLocationState | undefined;

  const [detalhePublico, setDetalhePublico] = useState<VagaDetails | null>(null);
  const [loadingPublico, setLoadingPublico] = useState(false);
  const detalhe = detalhePublico ?? state?.detalhe ?? null;

  const dispatch = useAppDispatch();
  const { token, user, loginStatus, loginError, orgIdFromResponse, status: authStatus } = useAppSelector((s) => s.auth);

  const [showCadastroForm, setShowCadastroForm] = useState(false);
  const [emailForToken, setEmailForToken] = useState('');
  const [showTokenModal, setShowTokenModal] = useState(false);
  const [otpValue, setOtpValue] = useState('');
  const [timeLeft, setTimeLeft] = useState(60);
  const [isResendEnabled, setIsResendEnabled] = useState(false);
  const [aceitaTermos, setAceitaTermos] = useState(false);
  const [isCadastrando, setIsCadastrando] = useState(false);
  const [showErrorDialog, setShowErrorDialog] = useState(false);

  const [dadosColaborador, setDadosColaborador] = useState<BuscarDadosColaboradorResponse | null>(null);
  const [loadingDados, setLoadingDados] = useState(false);
  const [stepPublico, setStepPublico] = useState<2 | 3 | 4>(2);
  const [inscricaoEnviada, setInscricaoEnviada] = useState(false);
  const [fileCurriculo, setFileCurriculo] = useState<File | null>(null);
  const [cpfInscricao, setCpfInscricao] = useState('');
  const [cepInscricao, setCepInscricao] = useState('');
  const [cidadeInscricao, setCidadeInscricao] = useState('');
  const [estadoInscricao, setEstadoInscricao] = useState('');
  const { buscarCep, endereco: enderecoViaCep, loading: loadingCep } = useViaCep();
  const [submittingInscricao, setSubmittingInscricao] = useState(false);
  const [submittingCandidatura, setSubmittingCandidatura] = useState(false);
  const [pretensaoSalarialDisplay, setPretensaoSalarialDisplay] = useState('');
  const [modeloTrabalhoId, setModeloTrabalhoId] = useState('');
  const [modelosTrabalho, setModelosTrabalho] = useState<ModeloTrabalho[]>([]);
  const [opcoesContato, setOpcoesContato] = useState<OpcaoContatoItem[]>([]);
  const [opcoesContatoSelectedIds, setOpcoesContatoSelectedIds] = useState<string[]>([]);
  const [loadingModelos, setLoadingModelos] = useState(false);
  const [loadingOpcoes, setLoadingOpcoes] = useState(false);
  const [skillsSelecionadas, setSkillsSelecionadas] = useState<Set<string>>(new Set());
  /** Por skill id: opção (incluir / não incluir / PDI), nível editável (id + descrição). Só opcao === 'incluir' dispara inclusão no perfil e o visual verde. */
  const [skillOpcoes, setSkillOpcoes] = useState<Record<string, { opcao?: 'incluir' | 'nao_incluir' | 'pdi'; nivelId?: number; nivelDescricao: string }>>({});
  /** Níveis de senioridade por tipoSkillId (1=hard, 2=soft, etc.) para o select no card. */
  const [niveisByTipo, setNiveisByTipo] = useState<Record<number, { id: number; descricao: string }[]>>({});
  const [loadingNiveis, setLoadingNiveis] = useState(false);
  const [candidaturaJaExiste, setCandidaturaJaExiste] = useState(false);
  const [syncPdfError, setSyncPdfError] = useState(false);
  const [curriculoAtualizadoComSucesso, setCurriculoAtualizadoComSucesso] = useState(false);
  const [mobileSheetOpen, setMobileSheetOpen] = useState(false);
  const [isLg, setIsLg] = useState(true);
  /** Skill id em que a chamada de adicionar/atualizar está em andamento (loading no card). */
  const [addingOrUpdatingSkillId, setAddingOrUpdatingSkillId] = useState<string | null>(null);
  /** Skills para as quais já disparámos a chamada de incluir no perfil (evita duplicar; limpo ao desmarcar). */
  const skillsIncluirChamadosRef = useRef<Set<string>>(new Set());
  /** Evita logar VisualizarStepDadosVaga mais de uma vez por sessão. */
  const stepDadosVagaLogadoRef = useRef(false);

  const getDetalhesPublico = useMemo(
    () => container.resolve(GetVagaDetalhesPublicoUseCase),
    []
  );
  const buscarDadosColaboradorUseCase = useMemo(
    () => container.resolve(BuscarDadosColaboradorUseCase),
    []
  );
  const candidatarSeUseCase = useMemo(
    () => container.resolve(CandidatarSeUseCase),
    []
  );
  const alterarFormularioColaboradorUseCase = useMemo(
    () => container.resolve(AlterarFormularioColaboradorUseCase),
    []
  );
  const listarModelosTrabalhoUseCase = useMemo(
    () => container.resolve(ListarModelosTrabalhoUseCase),
    []
  );
  const listarOpcoesContatoUseCase = useMemo(
    () => container.resolve(ListarOpcoesContatoUseCase),
    []
  );
  const sincronizarCurriculoUseCase = useMemo(
    () => container.resolve(SincronizarCurriculoColaboradorUseCase),
    []
  );
  const getSkillLevelsUseCase = useMemo(
    () => container.resolve(GetSkillLevelsUseCase),
    []
  );

  /** Mapeia tipoSkillId da vaga para MinhaJornadaSkillType (API de níveis: ListarNivelCompetencia, ListarNivelSoftskill, etc.). */
  const tipoSkillIdToSkillType = (tipoSkillId: number): MinhaJornadaSkillType => {
    if (tipoSkillId === 1) return 'hard';
    if (tipoSkillId === 2 || tipoSkillId === 8) return 'soft';
    if (tipoSkillId === 3) return 'methodology';
    if (tipoSkillId === 4) return 'domain';
    if (tipoSkillId === 9) return 'language';
    return 'hard';
  };

  /** Mapeia tipoSkillId para o tipo do repositório (adicionar/atualizar no perfil 360). */
  const tipoSkillIdToTipoPerfil360 = (tipoSkillId: number): 'hard' | 'soft' | 'metodologia' | 'dominio' | 'idioma' => {
    if (tipoSkillId === 1) return 'hard';
    if (tipoSkillId === 2 || tipoSkillId === 8) return 'soft';
    if (tipoSkillId === 3) return 'metodologia';
    if (tipoSkillId === 4) return 'dominio';
    if (tipoSkillId === 9) return 'idioma';
    return 'hard';
  };

  /**
   * Modelos de trabalho: sempre ListarModelosTrabalho no passo 3 com token + usuário hidratado
   * (mesmo contexto org/sessão que BuscarDadosColaborador). Evita IDs de outra org ou mock HML.
   */
  useEffect(() => {
    if (stepPublico !== 3 || !token || !user?.cpf || showTokenModal) return;
    let cancelled = false;
    setLoadingModelos(true);
    listarModelosTrabalhoUseCase
      .execute(token)
      .then((list) => {
        if (cancelled) return;
        const arr = Array.isArray(list) ? list : [];
        setModelosTrabalho(arr);
        setModeloTrabalhoId((prev) => {
          if (!prev?.trim()) return prev;
          return arr.some((m) => String(m.id) === String(prev)) ? prev : '';
        });
      })
      .catch(() => {
        if (!cancelled) {
          setModelosTrabalho([]);
          toast.error('Não foi possível carregar os modelos de trabalho. Tente atualizar a página.');
        }
      })
      .finally(() => {
        if (!cancelled) setLoadingModelos(false);
      });
    return () => {
      cancelled = true;
    };
  }, [stepPublico, token, user?.cpf, showTokenModal, listarModelosTrabalhoUseCase]);

  /** Opções de contato: mesmo gatilho do passo 3 para alinhar sessão/org ao formulário de inscrição. */
  useEffect(() => {
    if (stepPublico !== 3 || !token || !user?.cpf || showTokenModal) return;
    let cancelled = false;
    setLoadingOpcoes(true);
    listarOpcoesContatoUseCase
      .execute(token)
      .then((list) => {
        if (cancelled) return;
        const arr = Array.isArray(list) ? list : [];
        setOpcoesContato(arr);
        setOpcoesContatoSelectedIds((prev) =>
          prev.filter((id) => arr.some((o) => String(o.id) === String(id)))
        );
      })
      .catch(() => {
        if (!cancelled) {
          setOpcoesContato([]);
          toast.error('Não foi possível carregar as opções de contato.');
        }
      })
      .finally(() => {
        if (!cancelled) setLoadingOpcoes(false);
      });
    return () => {
      cancelled = true;
    };
  }, [stepPublico, token, user?.cpf, showTokenModal, listarOpcoesContatoUseCase]);

  useEffect(() => {
    const mql = window.matchMedia('(min-width: 1024px)');
    const update = () => setIsLg(mql.matches);
    update();
    mql.addEventListener('change', update);
    return () => mql.removeEventListener('change', update);
  }, []);

  useEffect(() => {
    const codigo = codigoFromUrl ? Number(codigoFromUrl) : NaN;
    if (Number.isNaN(codigo) || codigo <= 0) {
      setDetalhePublico(null);
      setLoadingPublico(false);
      return;
    }
    const detalheFromState = state?.detalhe;
    const codigoFromState = state?.codigoVaga != null ? Number(state.codigoVaga) : detalheFromState?.codigo;
    if (detalheFromState && (codigoFromState === codigo || detalheFromState.codigo === codigo)) {
      setDetalhePublico(detalheFromState);
      setLoadingPublico(false);
      return;
    }
    let cancelled = false;
    setLoadingPublico(true);
    setDetalhePublico(null);
    getDetalhesPublico
      .execute(codigo)
      .then((result) => {
        if (!cancelled) setDetalhePublico(result ?? null);
      })
      .catch(() => {
        if (!cancelled) {
          setDetalhePublico(null);
          toast.error('Não foi possível carregar os detalhes da vaga.');
        }
      })
      .finally(() => {
        if (!cancelled) setLoadingPublico(false);
      });
    return () => {
      cancelled = true;
    };
  }, [codigoFromUrl, getDetalhesPublico, state?.detalhe, state?.codigoVaga]);

  useEffect(() => {
    if (showTokenModal && timeLeft > 0) {
      const t = setTimeout(() => setTimeLeft((prev) => prev - 1), 1000);
      return () => clearTimeout(t);
    }
    if (timeLeft === 0) setIsResendEnabled(true);
  }, [showTokenModal, timeLeft]);

  useEffect(() => {
    if (loginError && loginStatus === 'failed') setShowErrorDialog(true);
  }, [loginError, loginStatus]);

  /**
   * /public/vaga não passa pelo MainLayout — com token em localStorage o Redux inicia com user=null.
   * Dispara fetchShowmeProfile para hidratar o perfil (igual MainLayout) e permitir ir direto ao passo 2.
   */
  useLayoutEffect(() => {
    if (!token || user || showTokenModal) return;
    if (authStatus === 'loading' || authStatus === 'failed') return;
    void dispatch(fetchShowmeProfile());
  }, [token, user, showTokenModal, authStatus, dispatch]);

  // Só buscar dados do colaborador quando tivermos token e o modal de token já foi fechado
  useEffect(() => {
    if (!token || !user?.cpf || showTokenModal) {
      setDadosColaborador(null);
      return;
    }
    setLoadingDados(true);
    setDadosColaborador(null);
    buscarDadosColaboradorUseCase
      .execute(token, user.cpf)
      .then(setDadosColaborador)
      .catch(() => {
        setDadosColaborador(null);
        toast.error('Não foi possível carregar seus dados.');
      })
      .finally(() => setLoadingDados(false));
  }, [token, user?.cpf, showTokenModal, buscarDadosColaboradorUseCase]);

  // Pular para "Dados para vaga" (step 3) quando dados do colaborador carregarem — passo 2 (Dados complementares) fica oculto
  useEffect(() => {
    if (token && dadosColaborador && stepPublico === 2) {
      setStepPublico(3);
      setPretensaoSalarialDisplay('');
      setSkillsSelecionadas(new Set());
      setSkillOpcoes({});
      const colab = dadosColaborador.colaborador;
      setCpfInscricao(colab?.documentoColaborador ? formatCPF(colab.documentoColaborador) : '');
      const end = colab?.endereco;
      if (end?.cep) setCepInscricao(formatCep(end.cep));
      if (end?.cidade) setCidadeInscricao(end.cidade);
      if (end?.estado) setEstadoInscricao(end.estado);
    }
  }, [token, dadosColaborador, stepPublico]);

  // Analytics: logar uma vez quando o usuário visualiza o passo "Dados para vaga" (formulário de inscrição)
  useEffect(() => {
    if (stepPublico === 3 && token && dadosColaborador && !stepDadosVagaLogadoRef.current) {
      stepDadosVagaLogadoRef.current = true;
      logUserAction(CANDIDATURA_EXTERNA_FLOW, CANDIDATURA_EXTERNA_ACTIONS.VisualizarStepDadosVaga, {}, user ?? undefined);
    }
  }, [stepPublico, token, dadosColaborador, user]);

  // Preencher cidade e estado quando ViaCEP retornar (não limpar por useEffect para não apagar dados carregados do colaborador)
  useEffect(() => {
    if (enderecoViaCep) {
      setCidadeInscricao(enderecoViaCep.cidade ?? '');
      setEstadoInscricao(enderecoViaCep.estado ?? '');
    }
  }, [enderecoViaCep]);

  const loginForm = useForm<LoginFormData>({
    resolver: zodResolver(loginSchema),
    defaultValues: { email: '' },
  });

  const cadastroForm = useForm<CadastroFormData>({
    resolver: zodResolver(cadastroSchema),
    mode: 'onChange',
    defaultValues: {
      nomeCompleto: '',
      cpf: '',
      email: '',
      senha: '',
      confirmaSenha: '',
    },
  });

  const inscricaoForm = useForm<InscricaoFormData>({
    resolver: zodResolver(inscricaoSchema),
    defaultValues: {
      cargoAtualUltimo: '',
      salarioAtualUltimo: '',
      tipoContratoAtualUltimo: '',
      modalidadeAtualUltima: '',
      contatoPrincipal: '',
      aceitaSugestoes: false,
    },
  });

  useEffect(() => {
    const col = dadosColaborador?.colaborador;
    const cand = col?.candidato;
    if (!col) return;
    inscricaoForm.reset({
      cargoAtualUltimo: cand?.cargoAtualUltimo ?? '',
      salarioAtualUltimo: cand?.salarioAtualUltimo != null ? String(cand.salarioAtualUltimo) : '',
      tipoContratoAtualUltimo: cand?.tipoContratoAtualUltimo ?? '',
      modalidadeAtualUltima: cand?.modalidadeAtualUltima ?? '',
      contatoPrincipal: col.contatoPrincipal ?? '',
      aceitaSugestoes: cand?.aceitaSugestoes ?? false,
    });
  }, [dadosColaborador]);

  const handleLoginSubmit = async (data: LoginFormData) => {
    dispatch(clearLoginState());
    const result = await dispatch(sendLoginToken({ email: data.email, orgId: ORG_ID_PUBLICO }));
    if (sendLoginToken.fulfilled.match(result)) {
      if (result.payload.tipoAcesso === 0) {
        logUserAction(CANDIDATURA_EXTERNA_FLOW, CANDIDATURA_EXTERNA_ACTIONS.SolicitarCodigoLogin, {}, undefined);
        setEmailForToken(data.email);
        setShowTokenModal(true);
        setTimeLeft(60);
        setIsResendEnabled(false);
        setOtpValue('');
      } else {
        toast.info('Verifique seu e-mail e siga as instruções.');
      }
    } else {
      setShowErrorDialog(true);
    }
  };

  const handleCadastroSubmit = async (data: CadastroFormData) => {
    if (!aceitaTermos) {
      toast.error('Aceite os termos de uso para continuar.');
      return;
    }
    const cpfDigits = (data.cpf ?? '').replace(/\D/g, '');
    if (!validarCPF(data.cpf ?? '')) {
      cadastroForm.setError('cpf', { type: 'manual', message: 'CPF inválido' });
      return;
    }
    setIsCadastrando(true);
    try {
      const cadastroUseCase = container.resolve(CadastrarUsuarioUseCase);
      const response = await cadastroUseCase.execute({
        cpf: cpfDigits,
        email: data.email,
        nome_completo: data.nomeCompleto,
        senha: data.senha,
      });
      if (!response.sucesso) {
        toast.error(response.mensagem ?? 'Erro ao cadastrar. Tente novamente.');
        return;
      }
      const lgpdUseCase = container.resolve(RegistrarLgpdUseCase);
      await lgpdUseCase.execute({
        cpfPessoa: cpfDigits,
        nomePessoa: data.nomeCompleto,
        isColab: false,
        unidade: 'Externo',
        contrato: 'Termos de uso - Fourmakers',
      });
      logUserAction(CANDIDATURA_EXTERNA_FLOW, CANDIDATURA_EXTERNA_ACTIONS.Cadastrar, {}, undefined);
      dispatch(clearLoginState());
      const result = await dispatch(sendLoginToken({ email: data.email, orgId: ORG_ID_PUBLICO }));
      if (sendLoginToken.fulfilled.match(result)) {
        if (result.payload.tipoAcesso === 0) {
          setEmailForToken(data.email);
          setShowTokenModal(true);
          setTimeLeft(60);
          setIsResendEnabled(false);
          setOtpValue('');
          setShowCadastroForm(false);
          cadastroForm.reset();
          setAceitaTermos(false);
        } else {
          toast.success('Cadastro realizado. Verifique seu e-mail.');
        }
      } else {
        setShowErrorDialog(true);
      }
    } catch (err: unknown) {
      const msg = err instanceof Error ? err.message : 'Erro ao cadastrar. Tente novamente.';
      toast.error(msg);
    } finally {
      setIsCadastrando(false);
    }
  };

  const handleResendCode = async () => {
    if (!emailForToken) return;
    dispatch(clearLoginState());
    const result = await dispatch(sendLoginToken({ email: emailForToken, orgId: ORG_ID_PUBLICO }));
    if (sendLoginToken.fulfilled.match(result) && result.payload.tipoAcesso === 0) {
      setTimeLeft(60);
      setIsResendEnabled(false);
      setOtpValue('');
    }
  };

  const handleOTPComplete = async (code: string) => {
    if (!emailForToken) return;
    const orgId = orgIdFromResponse ?? ORG_ID_PUBLICO;
    const result = await dispatch(
      validateLoginToken({ email: emailForToken, token: code, orgId })
    );
    if (validateLoginToken.fulfilled.match(result)) {
      const payloadToken = result.payload?.token;
      const payloadUsuario = result.payload?.usuario;
      if (!payloadToken) {
        setOtpValue('');
        setShowErrorDialog(true);
        toast.error('Resposta da API sem token. Tente novamente.');
        return;
      }
      // Redundante com validateLoginToken.fulfilled (já grava token no state e no localStorage), mas garante persistência se o fluxo mudar.
      if (typeof localStorage !== 'undefined') {
        localStorage.setItem('authToken', payloadToken);
      }
      if (!payloadUsuario) {
        await dispatch(fetchShowmeProfile());
      }
      setShowTokenModal(false);
      setOtpValue('');
      logUserAction(CANDIDATURA_EXTERNA_FLOW, CANDIDATURA_EXTERNA_ACTIONS.Entrar, {}, undefined);
      toast.success('Login realizado! Preencha os dados abaixo para continuar.');
    } else {
      setOtpValue('');
      setShowErrorDialog(true);
    }
  };

  const handleInscricaoSubmit = async (_data: InscricaoFormData) => {
    if (!token || !dadosColaborador?.colaborador) return;
    setSyncPdfError(false);
    setSubmittingInscricao(true);
    try {
      if (fileCurriculo) {
        const syncRes = await sincronizarCurriculoUseCase.execute(token, fileCurriculo);
        if (!syncRes.sucesso) {
          setSyncPdfError(true);
          toast.error(
            syncRes.mensagem ||
              'Não foi possível extrair os dados do PDF. Prefira o formato exportado pelo LinkedIn. Você pode enviar novamente ou avançar.'
          );
          return;
        }
      }
      const atualizado = await buscarDadosColaboradorUseCase.execute(token, user!.cpf);
      setDadosColaborador(atualizado);
      setFileCurriculo(null);
      setStepPublico(3);
      setPretensaoSalarialDisplay('');
      setSkillsSelecionadas(new Set());
      setSkillOpcoes({});
      toast.success('Dados atualizados. Preencha a pretensão salarial e confirme as competências.');
    } catch (err) {
      if (fileCurriculo) {
        setSyncPdfError(true);
        toast.error(
          'Não foi possível extrair os dados do PDF. Prefira o formato exportado pelo LinkedIn. Você pode enviar novamente ou avançar.'
        );
      } else {
        const msg = err instanceof Error ? err.message : 'Erro ao atualizar dados. Tente novamente.';
        toast.error(msg);
      }
    } finally {
      setSubmittingInscricao(false);
    }
  };

  const handleAvançarSemPdf = async () => {
    if (!token || !dadosColaborador?.colaborador) return;
    setSyncPdfError(false);
    setSubmittingInscricao(true);
    try {
      const atualizado = await buscarDadosColaboradorUseCase.execute(token, user!.cpf);
      setDadosColaborador(atualizado);
      setFileCurriculo(null);
      setStepPublico(3);
      setPretensaoSalarialDisplay('');
      setSkillsSelecionadas(new Set());
      setSkillOpcoes({});
      toast.success('Preencha a pretensão salarial e confirme as competências.');
    } catch (err) {
      const msg = err instanceof Error ? err.message : 'Erro ao carregar dados. Tente novamente.';
      toast.error(msg);
    } finally {
      setSubmittingInscricao(false);
    }
  };

  /** Atualiza perfil com PDF no step 4 (após inscrição). */
  const handleSyncCurriculoPdf = async () => {
    if (!token || !fileCurriculo || !user?.cpf) return;
    setSyncPdfError(false);
    setSubmittingInscricao(true);
    try {
      const syncRes = await sincronizarCurriculoUseCase.execute(token, fileCurriculo);
      if (!syncRes.sucesso) {
        setSyncPdfError(true);
        toast.error(syncRes.mensagem ?? 'Não foi possível extrair os dados do PDF. Prefira o formato exportado pelo LinkedIn.');
        return;
      }
      const atualizado = await buscarDadosColaboradorUseCase.execute(token, user.cpf);
      setDadosColaborador(atualizado);
      setFileCurriculo(null);
      setCurriculoAtualizadoComSucesso(true);
      toast.success('Perfil atualizado com sucesso.');
    } catch (err) {
      setSyncPdfError(true);
      toast.error(err instanceof Error ? err.message : 'Erro ao atualizar perfil. Tente novamente.');
    } finally {
      setSubmittingInscricao(false);
    }
  };

  const handleRealizarInscricao = async () => {
    if (!token || !detalhe?.codigo) return;
    const codigoVaga = Number(detalhe.codigo) || Number(codigoFromUrl);
    if (!Number.isFinite(codigoVaga) || codigoVaga <= 0) {
      toast.error('Código da vaga inválido.');
      return;
    }
    const cpfDigits = (cpfInscricao ?? '').replace(/\D/g, '');
    if (!validarCPF(cpfDigits)) {
      toast.error('Informe um CPF válido.');
      return;
    }
    if (!isCepComplete(cepInscricao)) {
      toast.error('Informe um CEP válido.');
      return;
    }
    if (!cidadeInscricao.trim() || !estadoInscricao.trim()) {
      toast.error('Preencha o CEP para carregar cidade e estado.');
      return;
    }
    const pretencaoNum = parseBRLCurrencyInput(pretensaoSalarialDisplay);
    if (pretencaoNum == null || pretencaoNum <= 0) {
      toast.error('Informe a pretensão de remuneração líquida.');
      return;
    }
    if (!modeloTrabalhoId.trim()) {
      toast.error('Selecione o modelo de trabalho.');
      return;
    }
    if (opcoesContatoSelectedIds.length === 0) {
      toast.error('Selecione ao menos uma preferência de contato.');
      return;
    }
    setSubmittingCandidatura(true);
    setCandidaturaJaExiste(false);
    try {
      const colab = dadosColaborador?.colaborador;
      const docOriginal = (colab?.documentoColaborador ?? '').replace(/\D/g, '');
      const cpfNorm = cpfDigits;
      const cepNorm = cepToApiFormat(cepInscricao);
      const cepOriginal = colab?.endereco?.cep?.replace(/\D/g, '') ?? '';
      const documentoAlterado = cpfNorm !== docOriginal;
      const enderecoAlterado =
        cepNorm !== cepOriginal ||
        (cidadeInscricao.trim() !== (colab?.endereco?.cidade ?? '')) ||
        (estadoInscricao.trim() !== (colab?.endereco?.estado ?? ''));

      if (documentoAlterado || enderecoAlterado) {
        // Montar payload completo com todos os dados recebidos e só sobrescrever CPF e endereço, para não apagar outros dados no backend
        const base = colab?.endereco;
        const enderecoAtualizado = enderecoAlterado
          ? {
              ...base,
              cep: cepNorm,
              endereco: enderecoViaCep?.endereco ?? base?.endereco ?? '',
              complemento: base?.complemento ?? '',
              numero: base?.numero ?? 0,
              bairro: enderecoViaCep?.bairro ?? base?.bairro ?? '',
              cidade: cidadeInscricao.trim(),
              estado: estadoInscricao.trim(),
              comQuemMora: base?.comQuemMora ?? '',
            }
          : undefined;
        const fullPayload: AlterarFormularioColaboradorPayload = {
          ...colab,
          ...(documentoAlterado && { documentoColaborador: cpfInscricao.trim() }),
          ...(enderecoAtualizado && { endereco: enderecoAtualizado }),
        };
        const alterRes = await alterarFormularioColaboradorUseCase.execute(token, fullPayload);
        if (alterRes?.sucesso === false && alterRes?.mensagem) {
          toast.error(alterRes.mensagem);
          return;
        }
        const atualizado = await buscarDadosColaboradorUseCase.execute(token, user!.cpf);
        setDadosColaborador(atualizado);
      }

      await candidatarSeUseCase.execute(token, {
        codigoVaga,
        opcoesContatoIds: opcoesContatoSelectedIds.filter((id) => id != null && String(id).trim() !== ''),
        pretencaoSalarial: pretencaoNum.toFixed(2),
        modeloTrabalhoId: modeloTrabalhoId.trim(),
      });
      logUserAction(CANDIDATURA_EXTERNA_FLOW, CANDIDATURA_EXTERNA_ACTIONS.RealizarInscricao, { codigoVaga }, user ?? undefined);
      setInscricaoEnviada(true);
      setStepPublico(4);
      toast.success('Inscrição realizada com sucesso!');
    } catch (err) {
      const msg = err instanceof Error ? err.message : 'Não foi possível realizar a inscrição. Tente novamente.';
      logUserAction(CANDIDATURA_EXTERNA_FLOW, CANDIDATURA_EXTERNA_ACTIONS.RealizarInscricaoErro, { codigoVaga, erro: msg }, user ?? undefined);
      if (isCandidaturaJaExisteMessage(msg)) {
        setCandidaturaJaExiste(true);
        toast.info(msg);
      } else {
        toast.error(msg);
      }
    } finally {
      setSubmittingCandidatura(false);
    }
  };

  const toggleSkill = (skillId: string, checked: boolean) => {
    setSkillsSelecionadas((prev) => {
      const next = new Set(prev);
      if (checked) next.add(skillId);
      else next.delete(skillId);
      return next;
    });
  };

  const { tecnicas, socioemocionais, outros: outrosSkills } = groupSkills(detalhe?.skills ?? []);
  const listaSkillsFaltantes = useMemo(
    () => skillsFaltantes(detalhe?.skills ?? [], dadosColaborador),
    [detalhe?.skills, dadosColaborador]
  );
  const { tecnicas: faltantesTecnicas, socioemocionais: faltantesSocio, outros: faltantesOutros } = useMemo(() => {
    const t = listaSkillsFaltantes.filter((s) => s.tipoSkillId === 1);
    const s = listaSkillsFaltantes.filter((sk) => sk.tipoSkillId === 2);
    const o = listaSkillsFaltantes.filter((sk) => sk.tipoSkillId !== 1 && sk.tipoSkillId !== 2);
    return { tecnicas: t, socioemocionais: s, outros: o };
  }, [listaSkillsFaltantes]);

  useEffect(() => {
    if (stepPublico !== 3 || !token || listaSkillsFaltantes.length === 0) return;
    const tipos = [...new Set(listaSkillsFaltantes.map((s) => s.tipoSkillId))];
    let cancelled = false;
    setLoadingNiveis(true);
    const next: Record<number, { id: number; descricao: string }[]> = {};
    Promise.all(
      tipos.map(async (tipo) => {
        const skillType = tipoSkillIdToSkillType(tipo);
        const list = await getSkillLevelsUseCase.execute({ skillType, token });
        return { tipo, list };
      })
    )
      .then((results) => {
        if (cancelled) return;
        const nivelADefinir = (d: string) => (d ?? '').trim().toLowerCase() === 'a definir';
        results.forEach(({ tipo, list }) => {
          next[tipo] = (list ?? [])
            .filter((n) => !nivelADefinir(n.descricao ?? ''))
            .map((n) => ({ id: n.id, descricao: n.descricao ?? '' }));
        });
        setNiveisByTipo((prev) => ({ ...prev, ...next }));
      })
      .catch(() => {
        if (!cancelled) setNiveisByTipo({});
      })
      .finally(() => {
        if (!cancelled) setLoadingNiveis(false);
      });
    return () => {
      cancelled = true;
    };
  }, [stepPublico, token, listaSkillsFaltantes, getSkillLevelsUseCase]);

  const setSkillOpcao = (skillId: string, opcao: 'incluir' | 'nao_incluir' | 'pdi', nivelDescricao?: string, nivelId?: number) => {
    setSkillOpcoes((prev) => {
      const current = prev[skillId];
      const descricao = nivelDescricao ?? current?.nivelDescricao ?? '';
      const resolvedNivelId = nivelId ?? current?.nivelId;
      const next = { ...prev, [skillId]: { opcao, nivelId: resolvedNivelId, nivelDescricao: descricao } };
      return next;
    });
    setSkillsSelecionadas((prev) => {
      const next = new Set(prev);
      if (opcao === 'incluir') next.add(skillId);
      else next.delete(skillId);
      return next;
    });
  };

  /** Atualiza apenas o nível da skill. Não altera opcao nem aspecto do card: só quando o usuário marcar "Sim, já possuo" é que opcao vira 'incluir'. */
  const setSkillNivel = (skillId: string, nivelId: number, nivelDescricao: string) => {
    setSkillOpcoes((prev) => {
      const current = prev[skillId];
      const nextEntry = current
        ? { ...current, nivelId, nivelDescricao }
        : { nivelId, nivelDescricao };
      return { ...prev, [skillId]: nextEntry };
    });
  };

  const addSkillToPerfil360UseCase = useMemo(
    () => container.resolve(AddSkillToPerfil360UseCase),
    []
  );
  const removeSkillFromPerfil360UseCase = useMemo(
    () => container.resolve(RemoveSkillFromPerfil360UseCase),
    []
  );
  const updateSkillLevelUseCase = useMemo(
    () => container.resolve(UpdateSkillLevelUseCase),
    []
  );

  /**
   * Adiciona skill ao perfil 360 (hard, soft, metodologia, domínio, idioma).
   * Ref reservada antes de await; nível alinhado ao texto do card (vaga/popover).
   */
  const adicionarHardSkillPublico = async (skillId: string, nivelId?: number) => {
    if (skillsIncluirChamadosRef.current.has(skillId)) return;
    if (!token || !user?.cpf) {
      toast.error('Faça login para incluir habilidades no seu perfil.');
      return;
    }
    const skill = listaSkillsFaltantes.find(
      (s) =>
        s.id === skillId ||
        String(s.skillId) === String(skillId) ||
        (s.id || `skill-${s.skillId}-${s.tipoSkillId}`) === skillId
    );
    if (!skill) {
      toast.error('Habilidade não encontrada. Atualize a página e tente novamente.');
      return;
    }
    skillsIncluirChamadosRef.current.add(skillId);

    const op = skillOpcoes[skillId];
    const descricaoNoCard = (op?.nivelDescricao ?? skill.skillNivelDescription ?? '').trim();

    let listaNiveis: { id: number; descricao: string }[] = [...(niveisByTipo[skill.tipoSkillId] ?? [])];
    if (!listaNiveis.length) {
      try {
        const skillType = tipoSkillIdToSkillType(skill.tipoSkillId);
        const list = await getSkillLevelsUseCase.execute({ skillType, token });
        listaNiveis = (list ?? [])
          .filter((n) => (n.descricao ?? '').trim().toLowerCase() !== 'a definir')
          .map((n) => ({ id: n.id, descricao: n.descricao ?? '' }));
      } catch {
        skillsIncluirChamadosRef.current.delete(skillId);
        toast.error('Não foi possível carregar os níveis. Tente novamente.');
        return;
      }
    }

    const porDescricaoCard = descricaoNoCard
      ? matchNivelPorDescricaoParaInclusao(listaNiveis, descricaoNoCard)
      : null;
    let nivelIdResolvido = porDescricaoCard?.id ?? null;
    let nivelDescricaoResolvido = porDescricaoCard?.descricao ?? '';

    if (nivelIdResolvido == null && nivelId != null) {
      const porArg = listaNiveis.find((n) => n.id === nivelId);
      if (porArg) {
        nivelIdResolvido = porArg.id;
        nivelDescricaoResolvido = porArg.descricao;
      }
    }
    if (nivelIdResolvido == null && op?.nivelId != null) {
      const porOp = listaNiveis.find((n) => n.id === op.nivelId);
      if (porOp) {
        nivelIdResolvido = porOp.id;
        nivelDescricaoResolvido = porOp.descricao;
      }
    }
    if (nivelIdResolvido == null && listaNiveis[0]) {
      nivelIdResolvido = listaNiveis[0].id;
      nivelDescricaoResolvido = listaNiveis[0].descricao;
    }
    if (nivelIdResolvido == null) {
      skillsIncluirChamadosRef.current.delete(skillId);
      toast.error('Nível da habilidade não disponível. Tente novamente.');
      return;
    }

    const tipo = tipoSkillIdToTipoPerfil360(skill.tipoSkillId);
    setAddingOrUpdatingSkillId(skillId);
    try {
      await addSkillToPerfil360UseCase.execute({
        tipo,
        items: [
          {
            id: Number(skill.skillId),
            descricao: skill.skillDescription ?? '',
            nivelId: nivelIdResolvido,
            cpf: user.cpf,
            gestorExternoPerfil: '',
            minhaJornada: false,
          },
        ],
        token,
      });
      setSkillOpcoes((prev) => {
        const current = prev[skillId];
        return {
          ...prev,
          [skillId]: {
            ...current,
            opcao: 'incluir',
            nivelId: nivelIdResolvido,
            nivelDescricao: nivelDescricaoResolvido || (current?.nivelDescricao ?? ''),
          },
        };
      });
      logUserAction(CANDIDATURA_EXTERNA_FLOW, CANDIDATURA_EXTERNA_ACTIONS.IncluirSkillPerfil, { skillId: Number(skill.skillId) }, user ?? undefined);
      toast.success('Habilidade foi inserida com sucesso.');
    } catch {
      skillsIncluirChamadosRef.current.delete(skillId);
      toast.error('Não foi possível adicionar a habilidade. Tente novamente.');
    } finally {
      setAddingOrUpdatingSkillId(null);
    }
  };

  /** Única fonte de chamada: quando o card passa para "incluir", dispara a inclusão no perfil. Evita duplicidade e rechamadas usando skillsIncluirChamadosRef. */
  useEffect(() => {
    if (stepPublico !== 3 || !token || !user?.cpf || listaSkillsFaltantes.length === 0) return;
    listaSkillsFaltantes.forEach((s) => {
      const skillKey = s.id || `skill-${s.skillId}-${s.tipoSkillId}`;
      const opcao = skillOpcoes[skillKey]?.opcao;
      if (opcao === 'incluir') {
        if (!skillsIncluirChamadosRef.current.has(skillKey)) {
          const nivelId = skillOpcoes[skillKey]?.nivelId;
          void adicionarHardSkillPublico(skillKey, nivelId);
        }
      } else {
        skillsIncluirChamadosRef.current.delete(skillKey);
      }
    });
  }, [skillOpcoes, stepPublico, token, user?.cpf, listaSkillsFaltantes]);

  /** Remove skill do perfil 360 (portal público). Chamado ao desmarcar o checkbox. Usa skill.skillId como id/competenciaId conforme o tipo. */
  const removerSkillPublico = async (skillId: string): Promise<void> => {
    if (!token || !user?.cpf) {
      toast.error('Faça login para alterar seu perfil.');
      return;
    }
    const skill = listaSkillsFaltantes.find(
      (s) =>
        s.id === skillId ||
        String(s.skillId) === String(skillId) ||
        (s.id || `skill-${s.skillId}-${s.tipoSkillId}`) === skillId
    );
    if (!skill) {
      toast.error('Habilidade não encontrada.');
      return;
    }
    const tipo = tipoSkillIdToTipoPerfil360(skill.tipoSkillId);
    setAddingOrUpdatingSkillId(skillId);
    try {
      await removeSkillFromPerfil360UseCase.execute({
        tipo,
        idOuCompetenciaId: Number(skill.skillId),
        cpf: user.cpf,
        token,
      });
      skillsIncluirChamadosRef.current.delete(skillId);
      logUserAction(CANDIDATURA_EXTERNA_FLOW, CANDIDATURA_EXTERNA_ACTIONS.RemoverSkillPerfil, { skillId: Number(skill.skillId) }, user ?? undefined);
      toast.success('Habilidade removida do seu perfil.');
    } catch {
      toast.error('Não foi possível remover a habilidade. Tente novamente.');
    } finally {
      setAddingOrUpdatingSkillId(null);
    }
  };

  /** Atualiza nível da skill no perfil 360 (portal público). Usa Use Case; chamada por categoria no repositório. */
  const atualizarNivelHardSkillPublico = async (skillId: string, nivelId: number) => {
    if (!token || !user?.cpf) return;
    const skill = listaSkillsFaltantes.find(
      (s) =>
        s.id === skillId ||
        String(s.skillId) === String(skillId) ||
        (s.id || `skill-${s.skillId}-${s.tipoSkillId}`) === skillId
    );
    if (!skill) return;
    const tipo = tipoSkillIdToTipoPerfil360(skill.tipoSkillId);
    setAddingOrUpdatingSkillId(skillId);
    try {
      await updateSkillLevelUseCase.execute({
        id: Number(skill.skillId),
        nivelId,
        cpf: user.cpf,
        tipo,
        gestorExternoPerfil: '',
        minhaJornada: false,
        token,
      });
      logUserAction(CANDIDATURA_EXTERNA_FLOW, CANDIDATURA_EXTERNA_ACTIONS.AtualizarNivelSkillPerfil, { skillId: Number(skill.skillId), nivelId }, user ?? undefined);
      toast.success('Nível da habilidade atualizado.');
    } catch {
      toast.error('Não foi possível atualizar o nível. Tente novamente.');
    } finally {
      setAddingOrUpdatingSkillId(null);
    }
  };

  const toggleOpcaoContato = (id: string) => {
    setOpcoesContatoSelectedIds((prev) =>
      prev.includes(id) ? prev.filter((x) => x !== id) : [...prev, id]
    );
  };

  const modalidade = detalhe?.modeloTrabalhoDescricao ?? detalhe?.cidade ?? 'Remoto';
  const estado = detalhe?.estado ? ` (${detalhe.estado})` : '';
  /** Token persistido válido em restauração OU usuário já hidratado (não exibir login/cadastro). */
  const validandoSessaoPersistida = Boolean(
    token && !user && !showTokenModal && authStatus !== 'failed'
  );
  const logado = Boolean(user) || validandoSessaoPersistida;
  const canSubmitCandidatura =
    validarCPF((cpfInscricao ?? '').replace(/\D/g, '')) &&
    isCepComplete(cepInscricao) &&
    cidadeInscricao.trim().length > 0 &&
    estadoInscricao.trim().length > 0 &&
    pretensaoSalarialDisplay.trim().length > 0 &&
    parseBRLCurrencyInput(pretensaoSalarialDisplay) != null &&
    (parseBRLCurrencyInput(pretensaoSalarialDisplay) ?? 0) > 0 &&
    modeloTrabalhoId.trim().length > 0 &&
    opcoesContatoSelectedIds.length > 0;

  return {
    codigoFromUrl,
    detalhe,
    loadingPublico,
    token,
    user,
    loginStatus,
    showCadastroForm,
    setShowCadastroForm,
    emailForToken,
    showTokenModal,
    setShowTokenModal,
    otpValue,
    setOtpValue,
    timeLeft,
    isResendEnabled,
    aceitaTermos,
    setAceitaTermos,
    isCadastrando,
    showErrorDialog,
    setShowErrorDialog,
    dadosColaborador,
    loadingDados,
    stepPublico,
    setStepPublico,
    inscricaoEnviada,
    cpfInscricao,
    setCpfInscricao,
    cepInscricao,
    setCepInscricao,
    cidadeInscricao,
    setCidadeInscricao,
    estadoInscricao,
    setEstadoInscricao,
    buscarCep,
    loadingCep,
    fileCurriculo,
    setFileCurriculo,
    syncPdfError,
    setSyncPdfError,
    curriculoAtualizadoComSucesso,
    handleAvançarSemPdf,
    submittingInscricao,
    submittingCandidatura,
    pretensaoSalarialDisplay,
    setPretensaoSalarialDisplay,
    modeloTrabalhoId,
    setModeloTrabalhoId,
    modelosTrabalho,
    opcoesContato,
    opcoesContatoSelectedIds,
    loadingModelos,
    loadingOpcoes,
    toggleOpcaoContato,
    canSubmitCandidatura,
    skillsSelecionadas,
    skillOpcoes,
    niveisByTipo,
    loadingNiveis,
    setSkillOpcao,
    setSkillNivel,
    adicionarHardSkillPublico,
    atualizarNivelHardSkillPublico,
    removerSkillPublico,
    addingOrUpdatingSkillId,
    candidaturaJaExiste,
    mobileSheetOpen,
    setMobileSheetOpen,
    isLg,
    loginForm,
    cadastroForm,
    inscricaoForm,
    handleLoginSubmit,
    handleCadastroSubmit,
    handleResendCode,
    handleOTPComplete,
    handleInscricaoSubmit,
    handleRealizarInscricao,
    handleSyncCurriculoPdf,
    toggleSkill,
    navigate,
    modalidade,
    estado,
    tecnicas,
    socioemocionais,
    outrosSkills,
    listaSkillsFaltantes,
    faltantesTecnicas,
    faltantesSocio,
    faltantesOutros,
    logado,
    validandoSessaoPersistida,
    loginError,
  };
}
