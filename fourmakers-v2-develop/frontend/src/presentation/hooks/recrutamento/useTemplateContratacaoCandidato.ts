import { useEffect, useState, useCallback } from 'react';
import { container } from '@core/di/container';
import { ListarColaboradoresOrgUseCase } from '@domain/usecases/ListarColaboradoresOrgUseCase';
import { ListarDiretoriosSistemasGruposUseCase } from '@domain/usecases/ListarDiretoriosSistemasGruposUseCase';
import { ListarEquipamentosPadroesAninhadosUseCase } from '@domain/usecases/ListarEquipamentosPadroesAninhadosUseCase';
import { ObterTemplateContratacaoPorCandidaturaUseCase } from '@domain/usecases/ObterTemplateContratacaoPorCandidaturaUseCase';
import { SalvarEBaixarTemplateUseCase } from '@domain/usecases/SalvarEBaixarTemplateUseCase';
import type {
  CandidaturaDetalhesVaga,
  DiretorioContratacaoItem,
  EquipamentosPadroesAninhadosGrupo,
  GrupoEmailContratacaoItem,
  SistemaLiberadoContratacaoItem,
  TemplateContratacaoData,
} from '@domain/entities/TemplateContratacao';
import { GRUPO_EMAIL_CONTRATO_DEFAULT } from '@domain/entities/TemplateContratacao';
import { toast } from 'sonner';

const TEMPLATE_ID_CRIACAO = '00000000-0000-0000-0000-000000000000';

/** Rótulos dos campos para exibir na mensagem de validação (campos com *). */
const ROTULOS_CAMPOS_OBRIGATORIOS: Record<string, string> = {
  cargo: 'Cargo',
  dataInicio: 'Data de Início',
  colaboradorCodigoInternoColaboradorAnalista: 'Analista de R&S',
  grupoAreaEquipamentoPadraoCargoFuncao: 'Grupo Área / Equipamento',
  quantidadeDiasPresencial: 'Dias no presencial',
  documentoColaborador: 'CPF',
  rgColaborador: 'RG',
  dataNascimento: 'Data de Nascimento',
  dddColaborador: 'DDD',
  telefone: 'Telefone',
  emailPessoal: 'E-mail Pessoal',
  cep: 'CEP',
  enderecoNumero: 'Número (Endereço)',
  enderecoBairro: 'Bairro',
  enderecoCidade: 'Cidade',
  enderecoEstado: 'Estado',
  tipoLoginRede: 'Tipo de Login Rede',
  colaboradorCodigoInternoColaboradorSuperiorImediato: 'Colaborador Superior Imediato',
  observacoesAcessoUsuario: 'Observações Acesso Usuário',
  equipamentoPadraoCargoFuncaoId: 'Cargo x Máquina',
};

export interface UseTemplateContratacaoCandidatoResult {
  template: TemplateContratacaoData | null;
  candidatura: CandidaturaDetalhesVaga | null;
  loading: boolean;
  saving: boolean;
  formErrors: Record<string, string>;
  isModoCriacao: boolean;
  updateTemplate: (updates: Partial<TemplateContratacaoData>) => void;
  updateEndereco: (updates: Partial<NonNullable<TemplateContratacaoData['endereco']>>) => void;
  updateSaude: (updates: Partial<NonNullable<TemplateContratacaoData['saude']>>) => void;
  /** Salva o template (Criar ou Atualizar conforme id). Se fornecido, onSuccess(retorno) é chamado após sucesso; use para abrir modal de envio e gerar PDF. */
  handleSalvarValidar: (onSuccess?: (retorno: unknown) => void) => void;
  setFormErrors: (errors: Record<string, string>) => void;
  /** Dados para Select Tipo de equipamento + Cargo x Máquina (carregados ao montar). */
  equipamentosAninhados: EquipamentosPadroesAninhadosGrupo[];
  loadingEquipamentos: boolean;
  /** Listas da API para Acessos do Usuário (vazias se API falhar; usar constantes como fallback). */
  listasDiretorios: DiretorioContratacaoItem[];
  listasSistemasLiberados: SistemaLiberadoContratacaoItem[];
  listasGruposEmails: GrupoEmailContratacaoItem[];
}

const ORG_ID_ESPECIAL = 9;

function isOrgTemplateContratacaoEspecial(orgId: number | null | undefined): boolean {
  if (orgId === undefined || orgId === null) return false;
  return Number(orgId) === ORG_ID_ESPECIAL;
}

export function useTemplateContratacaoCandidato(
  idCandidatura: string | undefined,
  token: string | null,
  orgId?: number | null
): UseTemplateContratacaoCandidatoResult {
  const isOrg9 = isOrgTemplateContratacaoEspecial(orgId ?? undefined);
  const [template, setTemplate] = useState<TemplateContratacaoData | null>(null);
  const [templateInicial, setTemplateInicial] = useState<TemplateContratacaoData | null>(null);
  const [candidatura, setCandidatura] = useState<CandidaturaDetalhesVaga | null>(null);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [formErrors, setFormErrors] = useState<Record<string, string>>({});
  const [equipamentosAninhados, setEquipamentosAninhados] = useState<EquipamentosPadroesAninhadosGrupo[]>([]);
  const [loadingEquipamentos, setLoadingEquipamentos] = useState(false);
  const [listasDiretorios, setListasDiretorios] = useState<DiretorioContratacaoItem[]>([]);
  const [listasSistemasLiberados, setListasSistemasLiberados] = useState<SistemaLiberadoContratacaoItem[]>([]);
  const [listasGruposEmails, setListasGruposEmails] = useState<GrupoEmailContratacaoItem[]>([]);

  const isModoCriacao =
    template?.id === TEMPLATE_ID_CRIACAO || template?.id == null;

  useEffect(() => {
    if (!idCandidatura?.trim() || !token) {
      setLoading(false);
      return;
    }
    let cancelled = false;
    const useCase = container.resolve(ObterTemplateContratacaoPorCandidaturaUseCase);

    useCase
      .execute(token, idCandidatura)
      .then((result) => {
        if (cancelled) return;
        setTemplate(result.template);
        setTemplateInicial(result.template);
        setCandidatura(result.candidatura);
      })
      .catch((err) => {
        if (!cancelled) {
          toast.error(err?.message ?? 'Erro ao carregar dados');
          setTemplate(null);
          setTemplateInicial(null);
          setCandidatura(null);
        }
      })
      .finally(() => {
        if (!cancelled) setLoading(false);
      });

    return () => {
      cancelled = true;
    };
  }, [idCandidatura, token]);

  useEffect(() => {
    if (!token) return;
    let cancelled = false;
    setLoadingEquipamentos(true);
    const useCase = container.resolve(ListarEquipamentosPadroesAninhadosUseCase);
    useCase
      .execute(token)
      .then((lista) => {
        if (!cancelled && Array.isArray(lista)) setEquipamentosAninhados(lista);
      })
      .catch(() => {
        if (!cancelled) setEquipamentosAninhados([]);
      })
      .finally(() => {
        if (!cancelled) setLoadingEquipamentos(false);
      });
    return () => {
      cancelled = true;
    };
  }, [token]);

  useEffect(() => {
    if (!token) return;
    let cancelled = false;
    const useCase = container.resolve(ListarDiretoriosSistemasGruposUseCase);
    useCase
      .execute(token)
      .then((result) => {
        if (cancelled) return;
        setListasDiretorios(Array.isArray(result.diretorios) ? result.diretorios : []);
        setListasSistemasLiberados(
          Array.isArray(result.sistemasLiberados) ? result.sistemasLiberados : []
        );
        setListasGruposEmails(Array.isArray(result.gruposEmails) ? result.gruposEmails : []);
      })
      .catch(() => {
        if (!cancelled) {
          setListasDiretorios([]);
          setListasSistemasLiberados([]);
          setListasGruposEmails([]);
        }
      });
    return () => {
      cancelled = true;
    };
  }, [token]);

  /** Ao recarregar a tela: busca lista de colaboradores e preenche nm_Profissional para Analista e Superior Imediato pelo codigoColaboradorInterno. */
  useEffect(() => {
    if (!token || !template) return;
    const codigoAnalista = (template.colaboradorCodigoInternoColaboradorAnalista ?? '').trim();
    const codigoSuperior = (template.colaboradorCodigoInternoColaboradorSuperiorImediato ?? '').trim();
    const precisaAnalista = codigoAnalista && !(template.nomeColaboradorAnalista ?? '').trim();
    const precisaSuperior = codigoSuperior && !(template.nomeColaboradorSuperiorImediato ?? '').trim();
    if (!precisaAnalista && !precisaSuperior) return;

    let cancelled = false;
    const useCase = container.resolve(ListarColaboradoresOrgUseCase);
    useCase
      .execute(token, { busca: '', cursor: 0, limite: 50000 })
      .then((res) => {
        if (cancelled) return;
        const lista = res?.ColaboradoresCchResult ?? [];
        if (!Array.isArray(lista)) return;

        const analista = codigoAnalista
          ? lista.find((c) => (c.codigoColaboradorInterno ?? '').trim() === codigoAnalista)
          : null;
        const superior = codigoSuperior
          ? lista.find((c) => (c.codigoColaboradorInterno ?? '').trim() === codigoSuperior)
          : null;

        const nomeAnalista = analista?.nm_Profissional?.trim() ?? '';
        const nomeSuperior = superior?.nm_Profissional?.trim() ?? '';
        if (!nomeAnalista && !nomeSuperior) return;

        setTemplate((prev) => {
          if (!prev) return null;
          const next = { ...prev };
          if (nomeAnalista) next.nomeColaboradorAnalista = nomeAnalista;
          if (nomeSuperior) next.nomeColaboradorSuperiorImediato = nomeSuperior;
          return next;
        });
      })
      .catch(() => {
        /* falha silenciosa: mantém template como está */
      });

    return () => {
      cancelled = true;
    };
  }, [token, template?.colaboradorCodigoInternoColaboradorAnalista, template?.colaboradorCodigoInternoColaboradorSuperiorImediato, template?.nomeColaboradorAnalista, template?.nomeColaboradorSuperiorImediato]);

  const updateTemplate = useCallback((updates: Partial<TemplateContratacaoData>) => {
    setTemplate((prev) => (prev ? { ...prev, ...updates } : null));
  }, []);

  const updateEndereco = useCallback(
    (updates: Partial<NonNullable<TemplateContratacaoData['endereco']>>) => {
      setTemplate((prev) => {
        if (!prev) return null;
        const endereco = { ...(prev.endereco ?? {}), ...updates };
        return { ...prev, endereco };
      });
    },
    []
  );

  const updateSaude = useCallback(
    (updates: Partial<NonNullable<TemplateContratacaoData['saude']>>) => {
      setTemplate((prev) => {
        if (!prev) return null;
        const saude = { ...(prev.saude ?? {}), ...updates };
        return { ...prev, saude };
      });
    },
    []
  );

  // Limpa erros de campos obrigatórios assim que o valor fica válido.
  useEffect(() => {
    if (Object.keys(formErrors).length === 0) return;
    const nextErrors = { ...formErrors };

    const checks: Record<string, boolean> = {
      cargo: !!(template?.cargo ?? '').trim(),
      dataInicio: !!(template?.dataInicio ?? '').trim(),
      colaboradorCodigoInternoColaboradorAnalista: !!(template?.colaboradorCodigoInternoColaboradorAnalista ?? '').trim(),
      grupoAreaEquipamentoPadraoCargoFuncao:
        (template?.grupoAreaEquipamentoPadraoCargoFuncao ?? '').toString().trim() !== '',
      documentoColaborador: (template?.documentoColaborador ?? '').replace(/\D/g, '').length === 11,
      rgColaborador: !!(template?.rgColaborador ?? '').trim(),
      dataNascimento: !!(template?.dataNascimento ?? '').trim(),
      dddColaborador: (template?.dddColaborador ?? '').replace(/\D/g, '').length === 2,
      telefone: (() => {
        const len = (template?.telefone ?? '').replace(/\D/g, '').length;
        return len >= 8 && len <= 9;
      })(),
      emailPessoal: !!(template?.emailPessoal ?? '').trim(),
      cep: (template?.endereco?.cep ?? '').replace(/\D/g, '').length === 8,
      enderecoNumero:
        template?.endereco?.numero !== undefined &&
        template?.endereco?.numero !== null &&
        String(template.endereco.numero).trim() !== '',
      enderecoBairro: !!(template?.endereco?.bairro ?? '').trim(),
      enderecoCidade: !!(template?.endereco?.cidade ?? '').trim(),
      enderecoEstado: !!(template?.endereco?.estado ?? '').trim(),
      tipoLoginRede: !!(template?.tipoLoginRede ?? '').trim(),
      colaboradorCodigoInternoColaboradorSuperiorImediato:
        !!(template?.colaboradorCodigoInternoColaboradorSuperiorImediato ?? '').trim(),
      observacoesAcessoUsuario: !!(template?.observacoesAcessoUsuario ?? '').trim(),
      quantidadeDiasPresencial: (() => {
        const desc = (template?.modeloTrabalhoDescricao ?? '').toString().toLowerCase();
        if (!desc.includes('híbrido')) return true;
        const n = template?.quantidadeDiasPresencial;
        return n != null && n >= 1 && n <= 5;
      })(),
    };

    let changed = false;
    for (const [field, isValid] of Object.entries(checks)) {
      if (isValid && nextErrors[field]) {
        delete nextErrors[field];
        changed = true;
      }
    }

    if (changed) setFormErrors(nextErrors);
  }, [template, formErrors]);

  const handleSalvarValidar = useCallback(
    (onSuccess?: (retorno: unknown) => void) => {
      setFormErrors({});
      const errors: Record<string, string> = {};
      const merged = { ...templateInicial, ...template } as TemplateContratacaoData | null;

      // Dados da Vaga
      if (!(merged?.cargo ?? template?.cargo ?? '').trim()) {
        errors.cargo = 'Campo obrigatório';
      }
      if (!(template?.dataInicio ?? merged?.dataInicio ?? '').trim()) {
        errors.dataInicio = 'Campo obrigatório';
      }
      // Horário / Jornada: não é obrigatório; valor default é salvo quando vazio
      if (!(merged?.colaboradorCodigoInternoColaboradorAnalista ?? template?.colaboradorCodigoInternoColaboradorAnalista ?? '').trim()) {
        errors.colaboradorCodigoInternoColaboradorAnalista = 'Campo obrigatório';
      }
      if (!isOrg9) {
        const grupoEquipamentoPreenchido =
          (merged?.grupoAreaEquipamentoPadraoCargoFuncao ?? template?.grupoAreaEquipamentoPadraoCargoFuncao ?? '').toString().trim() !== '';
        if (!grupoEquipamentoPreenchido) {
          errors.grupoAreaEquipamentoPadraoCargoFuncao = 'Campo obrigatório';
        }
      }

      // Dias no presencial: obrigatório apenas quando modelo de trabalho é Híbrido
      const descricaoModelo = (merged?.modeloTrabalhoDescricao ?? template?.modeloTrabalhoDescricao ?? '').toString().toLowerCase();
      const isModeloHibrido = descricaoModelo.includes('híbrido');
      if (isModeloHibrido) {
        const dias = merged?.quantidadeDiasPresencial ?? template?.quantidadeDiasPresencial;
        if (dias == null || dias < 1 || dias > 5) {
          errors.quantidadeDiasPresencial = 'Campo obrigatório';
        }
      }

      // Informações Pessoais
      const cpfVal = (template?.documentoColaborador ?? merged?.documentoColaborador ?? '').replace(/\D/g, '');
      if (cpfVal.length !== 11) {
        errors.documentoColaborador = 'CPF deve ter 11 dígitos';
      }
      if (!(merged?.rgColaborador ?? template?.rgColaborador ?? '').trim()) {
        errors.rgColaborador = 'Campo obrigatório';
      }
      if (!(template?.dataNascimento ?? merged?.dataNascimento ?? '').trim()) {
        errors.dataNascimento = 'Campo obrigatório';
      }
      const ddd = (template?.dddColaborador ?? merged?.dddColaborador ?? '').replace(/\D/g, '');
      if (ddd.length !== 2) {
        errors.dddColaborador = 'DDD deve ter 2 dígitos';
      }
      const telDigits = (template?.telefone ?? merged?.telefone ?? '').replace(/\D/g, '');
      if (telDigits.length < 8 || telDigits.length > 9) {
        errors.telefone = 'Telefone deve ter 8 ou 9 dígitos';
      }
      if (!(merged?.emailPessoal ?? template?.emailPessoal ?? '').trim()) {
        errors.emailPessoal = 'Campo obrigatório';
      }

      // Endereço
      const enderecoMerge = template?.endereco ?? merged?.endereco;
      const cepDigits = (enderecoMerge?.cep ?? '').replace(/\D/g, '');
      if (cepDigits.length !== 8) {
        errors.cep = 'CEP deve ter 8 dígitos';
      }
      if (enderecoMerge?.numero === undefined || enderecoMerge?.numero === null || String(enderecoMerge.numero).trim() === '') {
        errors.enderecoNumero = 'Campo obrigatório';
      }
      if (!(enderecoMerge?.bairro ?? '').trim()) {
        errors.enderecoBairro = 'Campo obrigatório';
      }
      if (!(enderecoMerge?.cidade ?? '').trim()) {
        errors.enderecoCidade = 'Campo obrigatório';
      }
      if (!(enderecoMerge?.estado ?? '').trim()) {
        errors.enderecoEstado = 'Campo obrigatório';
      }

      // Acessos do Usuário (obrigatórios apenas quando orgId !== 9)
      if (!isOrg9) {
        if (!(merged?.tipoLoginRede ?? template?.tipoLoginRede ?? '').trim()) {
          errors.tipoLoginRede = 'Campo obrigatório';
        }
        // Grupo de E-mail - Contrato: não é obrigatório; valor default é usado ao salvar quando vazio
        if (!(merged?.colaboradorCodigoInternoColaboradorSuperiorImediato ?? template?.colaboradorCodigoInternoColaboradorSuperiorImediato ?? '').trim()) {
          errors.colaboradorCodigoInternoColaboradorSuperiorImediato = 'Campo obrigatório';
        }
        if (!(merged?.observacoesAcessoUsuario ?? template?.observacoesAcessoUsuario ?? '').trim()) {
          errors.observacoesAcessoUsuario = 'Campo obrigatório';
        }
      }

      if (Object.keys(errors).length > 0) {
        setFormErrors(errors);
        const nomesCampos = Object.keys(errors)
          .map((key) => ROTULOS_CAMPOS_OBRIGATORIOS[key] ?? key)
          .filter(Boolean);
        const mensagem =
          nomesCampos.length > 0
            ? `Preencha os campos obrigatórios: ${nomesCampos.join(', ')}.`
            : 'Preencha os campos obrigatórios.';
        toast.error(mensagem, { duration: 6000 });
        return;
      }
      if (!idCandidatura?.trim() || !token) {
        toast.error('Dados da candidatura ou autenticação ausentes.');
        return;
      }
      if (!merged && !template) {
        toast.error('Template não carregado.');
        return;
      }

      const t = merged ?? template!;
      const cpfDigitsPayload = (t.documentoColaborador ?? '').replace(/\D/g, '').slice(0, 11);
      const dddDigitsPayload = (t.dddColaborador ?? '').replace(/\D/g, '').slice(0, 2);
      const telefoneDigitsPayload = (t.telefone ?? '').replace(/\D/g, '').slice(0, 9);
      const contatoPrincipalNormalizado =
        [dddDigitsPayload, telefoneDigitsPayload].filter(Boolean).join('') ||
        (t.contatoPrincipal ?? '').replace(/\D/g, '') ||
        '';

      const templateIdRaw = (t.id ?? '').trim();
      const isCriacao = templateIdRaw === '' || templateIdRaw === TEMPLATE_ID_CRIACAO;
      // Não usar idCandidatura em payload.id: salvarTemplate decide POST vs PUT só pelo id.
      // candidatoVagaId já associa o template à candidatura no corpo do CriarTemplate.
      const idPayload = isCriacao ? TEMPLATE_ID_CRIACAO : templateIdRaw;
      const codColaboradorNormalizado =
        (t.colaboradorCodigoInternoColaborador ?? '').trim() ||
        ((candidatura?.codColaborador as string | undefined) ?? '').trim() ||
        '';
      const candidatoVagaIdNormalizado = (t.candidatoVagaId ?? '').trim() || idCandidatura.trim();

      const sistemasLiberadosPayload = (t.sistemasLiberados ?? []).map((s) => ({
        id: s.id ?? '',
        descricao: s.descricao ?? '',
        liberado: true as const,
      }));
      const rawOutros = t.outrosGrupos ?? [];
      const outrosGruposPayload = rawOutros
        .map((item) => ({
          emailGrupo: typeof item === 'string' ? item : (item as { emailGrupo?: string })?.emailGrupo ?? '',
        }))
        .filter((o) => o.emailGrupo.length > 0);
      const gruposEmailsPayload = (t.gruposEmails ?? []).map((g) => ({
        id: g.id ?? '',
        descricao: g.descricao ?? '',
      }));
      const diretoriosPayload = (t.diretorios ?? []).map((d) => ({
        id: d.id ?? '',
        descricao: d.descricao ?? '',
        leitura: !!d.leitura,
        escrita: !!d.escrita,
      }));

      const enderecoPayload = t.endereco
        ? {
            cep: (t.endereco.cep ?? '').replace(/\D/g, '').slice(0, 8),
            endereco: t.endereco.endereco ?? '',
            complemento: t.endereco.complemento ?? '',
            numero: String(t.endereco.numero ?? '').trim() || '0',
            bairro: t.endereco.bairro ?? '',
            cidade: t.endereco.cidade ?? '',
            estado: t.endereco.estado ?? '',
          }
        : undefined;

      const payload: Record<string, unknown> = {
        endereco: enderecoPayload,
        id: idPayload,
        colaboradorCodigoInternoColaboradorAnalista: (t.colaboradorCodigoInternoColaboradorAnalista ?? '').trim() || undefined,
        colaboradorCodigoInternoColaborador: codColaboradorNormalizado || undefined,
        candidatoVagaId: candidatoVagaIdNormalizado,
        cargo: (t.cargo ?? '').trim() || undefined,
        equipamentoPadraoCargoFuncaoId: (t.equipamentoPadraoCargoFuncaoId ?? '').trim() || undefined,
        dataInicio: (t.dataInicio ?? '').trim() || undefined,
        horarioJornada: (t.horarioJornada ?? '').trim() || 'SEG A SEXTA 09:00 12:00/13:00 18:00',
        tipoHorarioJornada: (t.tipoHorarioJornada ?? '').toString().trim() || 'true',
        documentoColaborador: cpfDigitsPayload || undefined,
        rgColaborador: (t.rgColaborador ?? '').trim() || undefined,
        dataNascimento: (t.dataNascimento ?? '').trim() || undefined,
        contatoPrincipal: contatoPrincipalNormalizado || undefined,
        nomeCompleto: (t.nomeCompleto ?? '').trim() || undefined,
        tamanhoCamiseta: (t.tamanhoCamiseta ?? '').trim() || undefined,
        hardware: (t.hardware ?? '').trim() || '',
        softwaresNecessarios: (t.softwaresNecessarios ?? '').trim() || '',
        softwaresEc: (t.softwaresEc ?? '').trim() || '',
        colaboradorCodigoInternoColaboradorSuperiorImediato: isOrg9
          ? (t.colaboradorCodigoInternoColaboradorAnalista ?? '').trim() || undefined
          : (t.colaboradorCodigoInternoColaboradorSuperiorImediato ?? '').trim() || undefined,
        emailPessoal: (t.emailPessoal ?? '').trim() || undefined,
        emailCorporativo: (t.emailCorporativo ?? '').trim() || undefined,
        loginRede: (t.loginRede ?? '').trim() || undefined,
        tipoMaquina: (t.tipoMaquina ?? '').trim() || undefined,
        observacoesAcessoUsuario: (t.observacoesAcessoUsuario ?? '').trim() || undefined,
        grupoEmailContrato: (t.grupoEmailContrato ?? '').trim() || GRUPO_EMAIL_CONTRATO_DEFAULT,
        observacoesAprovadorAcessos: (t.observacoesAprovadorAcessos ?? '').trim() || undefined,
        salario: String(t.salario ?? 0).trim() || '0',
        custoHora: String(t.custoHora ?? 0).trim() || '0',
        vr: String(t.vr ?? 0).trim() || '0',
        va: String(t.va ?? 0).trim() || '0',
        assistenciaMedica: String(t.assistenciaMedica ?? 0).trim() || '0',
        ajudaDeCusto: String(t.ajudaDeCusto ?? 0).trim() || '0',
        mobilidade: String(t.mobilidade ?? 0).trim() || '0',
        educacao: String(t.educacao ?? 0).trim() || '0',
        remuneracaoTotal: String(t.remuneracaoTotal ?? 0).trim() || '0',
        tipoLoginRede: (t.tipoLoginRede ?? '').trim() || undefined,
        celular: !!t.celular,
        planoDados: !!t.planoDados,
        quantidadeMinutosPlanoDados: Number(t.quantidadeMinutosPlanoDados) || 0,
        cartaoVisitas: !!t.cartaoVisitas,
        quantidadeCartaoVisitas: Number(t.quantidadeCartaoVisitas) || 0,
        outrosEquipamentos: (t.outrosEquipamentos ?? '').trim() || '',
        cargoConfianca: !!t.cargoConfianca,
        modeloTrabalhoId: (t.modeloTrabalhoId ?? '').trim() || undefined,
        quantidadeDiasPresencial: (() => {
          const desc = (t.modeloTrabalhoDescricao ?? '').toString().toLowerCase();
          const isHibrido = desc.includes('híbrido');
          if (!isHibrido) return 5;
          const n = t.quantidadeDiasPresencial;
          return n != null && n >= 1 && n <= 5 ? n : 5;
        })(),
        valorAdicionalCargoConfianca: String(t.valorAdicionalCargoConfianca ?? 0).trim() || '0',
        sistemasLiberados: sistemasLiberadosPayload,
        diretorios: diretoriosPayload,
        gruposEmails: gruposEmailsPayload,
        outrosGrupos: outrosGruposPayload,
      };

      setSaving(true);
      const useCase = container.resolve(SalvarEBaixarTemplateUseCase);
      useCase
        .execute(token, idCandidatura, payload)
        .then((response) => {
          if (response?.sucesso === false && response?.mensagem) {
            toast.error(response.mensagem);
            return;
          }
          const retorno = response?.retorno;
          if (retorno && typeof retorno === 'object') {
            setTemplate((prev) => (prev ? { ...prev, ...(retorno as Record<string, unknown>) } : null));
          }
          toast.success(response?.mensagem ?? 'Template salvo com sucesso.');
          onSuccess?.(retorno);
        })
        .catch((err: Error) => {
          toast.error(err?.message ?? 'Erro ao salvar o template. Verifique os endpoints de criar/atualizar.');
        })
        .finally(() => {
          setSaving(false);
        });
    },
    [template, templateInicial, candidatura, idCandidatura, token, isOrg9]
  );

  return {
    template,
    candidatura,
    loading,
    saving,
    formErrors,
    isModoCriacao,
    updateTemplate,
    updateEndereco,
    updateSaude,
    handleSalvarValidar,
    setFormErrors,
    equipamentosAninhados,
    loadingEquipamentos,
    listasDiretorios,
    listasSistemasLiberados,
    listasGruposEmails,
  };
}
