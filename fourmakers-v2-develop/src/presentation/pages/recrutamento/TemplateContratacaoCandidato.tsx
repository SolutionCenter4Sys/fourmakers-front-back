import { Link, useLocation, useParams } from 'react-router-dom';
import { Card, CardContent } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Checkbox } from '@/components/ui/checkbox';
import { Switch } from '@/components/ui/switch';
import { Textarea } from '@/components/ui/textarea';
import { Spinner } from '@/components/ui/spinner';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import { PageBreadcrumb, PageHeader } from '@presentation/components/common';
import { useAppSelector } from '@app/store/hooks';
import { useEffect, useState, useCallback, useMemo, useRef } from 'react';
import {
  useTemplateContratacaoCandidato,
  formatDatePtBr,
  toInputDate,
  dateInputValueToIsoLocal,
  formatCurrencyBRL,
  parseBRLCurrencyInput,
  formatFrequenciaPresencial,
} from '@presentation/hooks/recrutamento'
import { useModelosTrabalho } from '@presentation/hooks/useModelosTrabalho';
import { useViaCep } from '@presentation/hooks/useViaCep';
import { formatCPF } from '@shared/utils/cpfUtils';
import { ArrowLeft, Plus, Trash2, ChevronDown, Download, Info } from '@/components/ui/system-icons';
import { Collapsible, CollapsibleContent, CollapsibleTrigger } from '@/components/ui/collapsible';
import { toast } from 'sonner';
import { cn } from '@/lib/utils';
import {
  TIPO_JORNADA_OPCOES,
  DIAS_PRESENCIAL_OPCOES,
  DIAS_PRESENCIAL_DEFAULT_OCULTO,
  TAMANHO_CAMISETA_OPCOES,
  UF_ESTADOS_BR,
  TIPO_DEFICIENCIA_OPCOES,
  PROPRIETARIO_MAQUINA_OPCOES,
  TIPO_ACESSO_OPCOES,
  SISTEMAS_LIBERADOS_LABELS,
  DIRETORIOS_REDE_LABELS,
  GRUPOS_EMAIL_LABELS,
} from './TemplateContratacaoCandidato.constants';
import { ColaboradorSearchField } from '@presentation/components/template-contratacao/ColaboradorSearchField';
import { EnvioTemplateContratacaoModal, type TipoEnvio } from '@presentation/components/template-contratacao/EnvioTemplateContratacaoModal';
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from '@/components/ui/tooltip';
import { Popover, PopoverContent, PopoverTrigger } from '@/components/ui/popover';
import { generateTemplateContratacaoPDF } from '@shared/utils/generateTemplateContratacaoPDF';
import { container } from '@core/di/container';
import { GRUPO_EMAIL_CONTRATO_DEFAULT, type TemplateContratacaoData } from '@domain/entities/TemplateContratacao';
import { EnviarEmailTemplateCandidatoUseCase } from '@domain/usecases/EnviarEmailTemplateCandidatoUseCase';

/**
 * Itens pendentes / não processados nesta entrega (ajustes posteriores):
 * - Endpoints de processamento para "Salvar e Validar Template" (criação vs edição).
 * - Estrutura exata do retorno de ObterCandidaturaPorId (campos podem variar).
 * - Dropdowns com dados de outras APIs: Modelo de Trabalho (id), Colaborador superior imediato (lookup).
 * - Sistemas Liberados e Diretórios: binding de leitura/escrita ao estado e ao submit.
 * - Botão "+" do Grupo de Email Contrato: adicionar à lista e persistir.
 * - Validação completa de todos os campos obrigatórios.
 */

const ORG_ID_ESPECIAL = 9;

export default function TemplateContratacaoCandidato() {
  const { idCandidatura } = useParams<{ idCandidatura: string }>();
  const location = useLocation();
  const token = useAppSelector((state) => state.auth.token);
  const orgIdFromRedux = useAppSelector((state) => state.auth.user?.colaboradorOrg?.orgId);
  const orgIdFromStorage = useMemo(() => {
    if (orgIdFromRedux != null) return undefined;
    const v = typeof localStorage !== 'undefined' ? localStorage.getItem('lastOrgId') : null;
    if (v == null || v === '') return undefined;
    const n = parseInt(v, 10);
    return Number.isNaN(n) ? undefined : n;
  }, [orgIdFromRedux]);
  const orgId = orgIdFromRedux ?? orgIdFromStorage;
  /** orgId pode vir como string (ex.: localStorage); normalizar para regras da org 9. */
  const isOrg9 = orgId != null && Number(orgId) === ORG_ID_ESPECIAL;
  const locationState = location.state as {
    vagaId?: string;
    vagaTitle?: string;
    vagaRaw?: unknown;
    /** Exibir bloco Benefícios e Lançamento Complementar (do item ListarCandidatosInscritos que originou o clique). */
    exibirRemuneracao?: boolean;
  } | null;
  const vagaIdFromState = locationState?.vagaId;
  const vagaTitleFromState = locationState?.vagaTitle ?? 'Candidatos';
  const exibirRemuneracao = locationState?.exibirRemuneracao === true;
  /** Dados da vaga vindos da lista de Candidatos inscritos na vaga (tela de origem). Usado no bloco "Dados da Vaga (origem)". */
  const dadosVagaOrigem = locationState?.vagaRaw as Record<string, unknown> | null | undefined;
  const candidatosHref =
    vagaIdFromState != null
      ? `/recrutamento/candidatos?vagaId=${encodeURIComponent(vagaIdFromState)}&nomeVaga=${encodeURIComponent(vagaTitleFromState)}`
      : '/recrutamento/candidatos';
  const backToCandidatos = candidatosHref;

  const breadcrumbBase = useMemo(
    () => [
      { label: 'Recrutamento', href: '/recrutamento' },
      vagaIdFromState
        ? { label: `Candidatos - ${vagaTitleFromState}`, href: candidatosHref }
        : { label: 'Candidatos', href: '/recrutamento/candidatos' },
    ],
    [vagaIdFromState, vagaTitleFromState, candidatosHref]
  );

  const {
    template,
    loading,
    saving,
    formErrors,
    setFormErrors,
    isModoCriacao,
    updateTemplate,
    updateEndereco,
    handleSalvarValidar,
    equipamentosAninhados,
    loadingEquipamentos,
    listasDiretorios,
    listasSistemasLiberados,
    listasGruposEmails,
  } = useTemplateContratacaoCandidato(idCandidatura ?? undefined, token ?? null, orgId ?? undefined);

  const [gerarPdfOpen, setGerarPdfOpen] = useState(false);

  const handleEnviarEmailTemplate = useCallback(
    async (payload: { tipoEnvio: TipoEnvio | null; emails: string[]; arquivo?: File }) => {
      if (!idCandidatura?.trim() || !token) {
        toast.error('Dados da candidatura ou autenticação ausentes.');
        return;
      }
      try {
        const useCase = container.resolve(EnviarEmailTemplateCandidatoUseCase);
        await useCase.execute(
          token,
          {
            idCandidatura: idCandidatura.trim(),
            emailsAdicionais: payload.emails,
            ocultarValores: payload.tipoEnvio === 'sem_remuneracao',
          },
          payload.arquivo
        );
        toast.success('Sucesso ao enviar os emails.');
      } catch (err) {
        toast.error(err instanceof Error ? err.message : 'Erro ao enviar os e-mails.');
        throw err;
      }
    },
    [idCandidatura, token]
  );

  const { list: modelosTrabalhoList, loading: loadingModelosTrabalho } = useModelosTrabalho(token);

  const modeloSelecionado = useMemo(
    () => modelosTrabalhoList.find((m) => m.id === template?.modeloTrabalhoId),
    [modelosTrabalhoList, template?.modeloTrabalhoId]
  );
  const isModeloHibrido = useMemo(() => {
    const desc = (modeloSelecionado?.descricao ?? template?.modeloTrabalhoDescricao ?? '').toString().toLowerCase();
    return desc.includes('híbrido');
  }, [modeloSelecionado?.descricao, template?.modeloTrabalhoDescricao]);

  useEffect(() => {
    if (!template || !modelosTrabalhoList.length) return;
    const desc = (modeloSelecionado?.descricao ?? template.modeloTrabalhoDescricao ?? '').toString().toLowerCase();
    if (!desc.includes('híbrido')) {
      const current = template.quantidadeDiasPresencial;
      if (current == null || current < 1 || current > 5) {
        updateTemplate({ quantidadeDiasPresencial: DIAS_PRESENCIAL_DEFAULT_OCULTO });
      }
    }
  }, [template?.modeloTrabalhoId, template?.quantidadeDiasPresencial, template?.modeloTrabalhoDescricao, modeloSelecionado?.descricao, modelosTrabalhoList.length, updateTemplate]);

  /** Pré-preenche campos do template a partir da vaga (lista de candidatos) uma vez por candidatura; vaga só sobrescreve quando tem valor. */
  const prefillVagaAplicadoRef = useRef(false);
  useEffect(() => {
    prefillVagaAplicadoRef.current = false;
    return () => {
      prefillVagaAplicadoRef.current = false;
    };
  }, [idCandidatura]);

  useEffect(() => {
    if (!template || loading || dadosVagaOrigem == null || prefillVagaAplicadoRef.current) return;
    const v = dadosVagaOrigem;
    const temModeloNaVaga = v.modeloTrabalhoId != null && String(v.modeloTrabalhoId).trim() !== '';
    if (temModeloNaVaga && modelosTrabalhoList.length === 0 && loadingModelosTrabalho) return;

    const updates: Partial<TemplateContratacaoData> = {};

    const cargoVaga = v.cargo;
    if (cargoVaga != null && String(cargoVaga).trim() !== '') {
      const s = String(cargoVaga).trim();
      if (template.cargo !== s) updates.cargo = s;
    }

    const templateJaDefiniuModelo =
      template.modeloTrabalhoId != null && String(template.modeloTrabalhoId).trim() !== '';
    if (!templateJaDefiniuModelo && temModeloNaVaga && modelosTrabalhoList.length > 0) {
      const mid = String(v.modeloTrabalhoId).trim();
      const item =
        modelosTrabalhoList.find((m) => m.id === mid) ??
        modelosTrabalhoList.find((m) => m.id.toLowerCase() === mid.toLowerCase());
      if (item && template.modeloTrabalhoId !== item.id) {
        updates.modeloTrabalhoId = item.id;
        updates.modeloTrabalhoDescricao = item.descricao ?? null;
      }
    }

    const templateJaDefiniuDias =
      template.quantidadeDiasPresencial != null &&
      typeof template.quantidadeDiasPresencial === 'number' &&
      template.quantidadeDiasPresencial >= 1 &&
      template.quantidadeDiasPresencial <= 5;
    const freqRaw = v.frequencia;
    if (
      !templateJaDefiniuDias &&
      freqRaw != null &&
      String(freqRaw).trim() !== ''
    ) {
      const n = parseInt(String(freqRaw), 10);
      if (Number.isFinite(n) && n >= 1 && n <= 5 && template.quantidadeDiasPresencial !== n) {
        updates.quantidadeDiasPresencial = n;
      }
    }

    prefillVagaAplicadoRef.current = true;
    if (Object.keys(updates).length > 0) {
      updateTemplate(updates);
    }
  }, [
    template,
    loading,
    dadosVagaOrigem,
    modelosTrabalhoList,
    loadingModelosTrabalho,
    idCandidatura,
    updateTemplate,
  ]);

  const grupoAreaSelecionado = template?.grupoAreaEquipamentoPadraoCargoFuncao ?? null;
  const cargosDoGrupo =
    grupoAreaSelecionado != null
      ? equipamentosAninhados.find((g) => g.grupoArea === grupoAreaSelecionado)?.cargosFuncoes ?? []
      : [];

  const { buscarCep, endereco: enderecoViaCep, loading: loadingCep } = useViaCep();

  useEffect(() => {
    if (!enderecoViaCep) return;
    updateEndereco({
      endereco: enderecoViaCep.endereco ?? '',
      bairro: enderecoViaCep.bairro ?? '',
      cidade: enderecoViaCep.cidade ?? '',
      estado: enderecoViaCep.estado ?? '',
      complemento: enderecoViaCep.complemento ?? '',
    });
  }, [enderecoViaCep]);

  const formatarCep = (valor: string) => {
    const digitos = valor.replace(/\D/g, '').slice(0, 8);
    if (digitos.length <= 5) return digitos;
    return `${digitos.slice(0, 5)}-${digitos.slice(5)}`;
  };
  const cepSoDigitos = (valor: string) => valor.replace(/\D/g, '').slice(0, 8);
  /** Formata apenas o número (8–9 dígitos) para o campo Telefone sem DDD. */
  const formatarApenasNumero = (valor: string) => {
    const d = valor.replace(/\D/g, '').slice(0, 9);
    if (d.length <= 4) return d;
    if (d.length <= 8) return `${d.slice(0, 4)}-${d.slice(4)}`;
    return `${d.slice(0, 5)}-${d.slice(5)}`;
  };

  const [outroGrupoEmailInput, setOutroGrupoEmailInput] = useState('');
  const rawOutrosGrupos = Array.isArray(template?.outrosGrupos) ? template.outrosGrupos : [];
  const emailDeItem = (item: string | { emailGrupo?: string }): string =>
    typeof item === 'string' ? item : (item?.emailGrupo ?? '');
  const outrosGruposList = rawOutrosGrupos.map(emailDeItem).filter(Boolean);

  const handleAdicionarOutroGrupo = () => {
    const email = outroGrupoEmailInput.trim();
    if (!email) return;
    if (outrosGruposList.includes(email)) {
      toast.info('Este e-mail já está na lista.');
      return;
    }
    const novoItem = { emailGrupo: email };
    updateTemplate({ outrosGrupos: [...rawOutrosGrupos, novoItem] });
    setOutroGrupoEmailInput('');
    toast.success('Email adicionado');
  };

  const handleRemoverOutroGrupo = (email: string) => {
    const lista = rawOutrosGrupos.filter((e) => emailDeItem(e) !== email);
    updateTemplate({ outrosGrupos: lista });
  };

  const onSelectAnalista = useCallback(
    (codigo: string, displayName: string) => {
      updateTemplate({
        colaboradorCodigoInternoColaboradorAnalista: codigo,
        nomeColaboradorAnalista: displayName,
      });
    },
    [updateTemplate]
  );

  const onSelectSuperior = useCallback(
    (codigo: string, displayName: string) => {
      updateTemplate({
        colaboradorCodigoInternoColaboradorSuperiorImediato: codigo,
        nomeColaboradorSuperiorImediato: displayName,
      });
    },
    [updateTemplate]
  );

  const clearFormError = useCallback(
    (field: string) => {
      if (!formErrors[field]) return;
      const next = { ...formErrors };
      delete next[field];
      setFormErrors(next);
    },
    [formErrors, setFormErrors]
  );

  const [modalEnvioOpen, setModalEnvioOpen] = useState(false);

  if (loading) {
    return (
      <div className="space-y-4 p-4">
        <PageBreadcrumb items={[...breadcrumbBase, { label: 'Template de Contratação' }]} />
        <div className="flex items-center justify-center py-12 text-muted-foreground">
          <div className="inline-flex items-center gap-2">
            <Spinner size={16} />
            <span>Carregando...</span>
          </div>
        </div>
      </div>
    );
  }

  if (!idCandidatura?.trim()) {
    return (
      <div className="space-y-4 p-4">
        <PageBreadcrumb items={[...breadcrumbBase, { label: 'Template de Contratação' }]} />
        <p className="text-destructive">Candidatura não informada.</p>
        <Button asChild variant="outline">
          <Link to={backToCandidatos}>Voltar</Link>
        </Button>
      </div>
    );
  }

  if (!template && !loading) {
    return (
      <div className="space-y-4 p-4">
        <PageBreadcrumb items={[...breadcrumbBase, { label: 'Template de Contratação' }]} />
        <p className="text-destructive">Não foi possível obter um template para esta candidatura. Tente novamente mais tarde.</p>
        <Button asChild variant="outline">
          <Link to={backToCandidatos}>Voltar</Link>
        </Button>
      </div>
    );
  }

  const nomeCandidatoCompleto = (template?.nomeCompleto ?? '').trim();
  const LIMITE_CARACTERES_NOME_TITULO = 28;
  const nomeCandidatoTituloExibicao =
    nomeCandidatoCompleto.length > LIMITE_CARACTERES_NOME_TITULO
      ? `${nomeCandidatoCompleto.slice(0, LIMITE_CARACTERES_NOME_TITULO)}…`
      : nomeCandidatoCompleto;

  const tituloPaginaHeader = (
    <>
      <span className="shrink-0">Template de Contratação</span>
      {nomeCandidatoCompleto ? (
        <>
          {' '}
          <TooltipProvider delayDuration={250}>
            <Tooltip>
              <TooltipTrigger asChild>
                <span
                  className="min-w-0 cursor-default border-b border-dotted border-muted-foreground/50"
                  tabIndex={0}
                >
                  {nomeCandidatoTituloExibicao}
                </span>
              </TooltipTrigger>
              <TooltipContent side="bottom" align="start" className="max-w-sm">
                <p className="text-sm break-words">{nomeCandidatoCompleto}</p>
              </TooltipContent>
            </Tooltip>
          </TooltipProvider>
        </>
      ) : null}
    </>
  );

  const endereco = template?.endereco ?? {};
  const saude = template?.saude ?? {};
  const sistemasLiberados = template?.sistemasLiberados ?? [];
  const diretorios = template?.diretorios ?? [];
  const gruposEmails = template?.gruposEmails ?? [];

  /** Opções para Sistemas Liberados: da API ou constantes (fallback). */
  const opcoesSistemas: { id?: string; descricao: string }[] =
    listasSistemasLiberados.length > 0
      ? listasSistemasLiberados.map((s) => ({ id: s.id, descricao: s.descricao ?? '' }))
      : SISTEMAS_LIBERADOS_LABELS.map((d) => ({ descricao: d }));

  /** Opções para Diretórios de Rede: da API ou constantes (fallback). */
  const opcoesDiretorios: { id?: string; descricao: string }[] =
    listasDiretorios.length > 0
      ? listasDiretorios.map((d) => ({ id: d.id, descricao: d.descricao ?? '' }))
      : DIRETORIOS_REDE_LABELS.map((d) => ({ descricao: d }));

  /** Opções para Grupos de E-mail: da API ou constantes (fallback). */
  const opcoesGruposEmails: { id?: string; descricao: string }[] =
    listasGruposEmails.length > 0
      ? listasGruposEmails.map((g) => ({ id: g.id, descricao: g.descricao ?? '' }))
      : GRUPOS_EMAIL_LABELS.map((l) => ({ descricao: l }));

  type ItemDesc = { id?: string; descricao: string };

  const isSistemaChecked = (item: ItemDesc) =>
    sistemasLiberados.some(
      (s) =>
        (item.id && s.id === item.id) ||
        (s.descricao ?? '').trim() === (item.descricao ?? '').trim()
    );
  const setSistemaChecked = (item: ItemDesc, checked: boolean) => {
    const next = checked
      ? [...sistemasLiberados, { id: item.id, descricao: item.descricao }]
      : sistemasLiberados.filter(
          (s) =>
            !((item.id && s.id === item.id) || (s.descricao ?? '').trim() === (item.descricao ?? '').trim())
        );
    updateTemplate({ sistemasLiberados: next });
  };

  const getDiretorioItem = (item: ItemDesc) =>
    diretorios.find(
      (d) =>
        (item.id && d.id === item.id) ||
        (d.descricao ?? d.nome ?? '').trim() === (item.descricao ?? '').trim()
    );
  const setDiretorioPerm = (item: ItemDesc, perm: 'leitura' | 'escrita', checked: boolean) => {
    const current = getDiretorioItem(item);
    const rest = diretorios.filter(
      (d) =>
        !((item.id && d.id === item.id) || (d.descricao ?? d.nome ?? '').trim() === (item.descricao ?? '').trim())
    );
    let leituraNew: boolean;
    let escritaNew: boolean;
    if (perm === 'leitura') {
      leituraNew = checked;
      escritaNew = checked ? (current?.escrita ?? false) : false; // ao desmarcar Leitura, desmarca Escrita
    } else {
      escritaNew = checked;
      leituraNew = checked ? true : (current?.leitura ?? false); // ao marcar Escrita, marca Leitura
    }
    const updated = {
      id: item.id,
      descricao: item.descricao,
      leitura: leituraNew,
      escrita: escritaNew,
    };
    updateTemplate({ diretorios: [...rest, updated] });
  };

  const isGrupoEmailChecked = (item: ItemDesc) =>
    gruposEmails.some(
      (g) =>
        (item.id && g.id === item.id) ||
        (g.descricao ?? g.nome ?? '').trim() === (item.descricao ?? '').trim()
    );
  const setGrupoEmailChecked = (item: ItemDesc, checked: boolean) => {
    const next = checked
      ? [...gruposEmails, { id: item.id, descricao: item.descricao }]
      : gruposEmails.filter(
          (g) =>
            !((item.id && g.id === item.id) || (g.descricao ?? g.nome ?? '').trim() === (item.descricao ?? '').trim())
        );
    updateTemplate({ gruposEmails: next });
  };

  return (
    <div className="space-y-6 pb-24">
      <PageBreadcrumb items={[...breadcrumbBase, { label: 'Template de Contratação' }]} />
      <PageHeader
        title={tituloPaginaHeader}
        titlePrefix={
          <Link
            to={backToCandidatos}
            className="flex items-center text-foreground hover:text-foreground/80 focus:outline-none focus-visible:ring-2 focus-visible:ring-ring"
            aria-label="Voltar"
          >
            <ArrowLeft className="h-5 w-5 shrink-0" />
          </Link>
        }
        actions={
          <div className="flex items-center gap-2">
            {!isModoCriacao && template && (
              <Popover open={gerarPdfOpen} onOpenChange={setGerarPdfOpen}>
                <PopoverTrigger asChild>
                  <Button variant="secondary" className="gap-2">
                    <Download className="h-4 w-4" />
                    Gerar PDF
                  </Button>
                </PopoverTrigger>
                <PopoverContent className="w-56 p-2" align="end">
                  <div className="flex flex-col gap-1">
                    <Button
                      variant="ghost"
                      className="justify-start font-normal"
                      onClick={() => {
                        if (isOrg9 && !(template?.emailPessoal ?? '').trim()) {
                          toast.error('Preencha o campo Email Pessoal para prosseguir com o envio.');
                          return;
                        }
                        generateTemplateContratacaoPDF(template, 'com_remuneracao', { modoOrg9: isOrg9 });
                        setGerarPdfOpen(false);
                        setModalEnvioOpen(true);
                      }}
                    >
                      PDF com remuneração
                    </Button>
                    <Button
                      variant="ghost"
                      className="justify-start font-normal"
                      onClick={() => {
                        if (isOrg9 && !(template?.emailPessoal ?? '').trim()) {
                          toast.error('Preencha o campo Email Pessoal para prosseguir com o envio.');
                          return;
                        }
                        generateTemplateContratacaoPDF(template, 'sem_remuneracao', { modoOrg9: isOrg9 });
                        setGerarPdfOpen(false);
                        setModalEnvioOpen(true);
                      }}
                    >
                      PDF sem remuneração
                    </Button>
                  </div>
                </PopoverContent>
              </Popover>
            )}
          </div>
        }
      />

      {template?.exColaborador === true && (
        <div
          className="rounded-md border border-destructive/50 bg-destructive/10 px-4 py-3 text-center text-sm font-medium text-destructive"
          role="alert"
        >
          O Candidato é um Ex-Colaborador.
        </div>
      )}

      {/* Bloco: Informações sobre a vaga — dados vindos da lista de Candidatos inscritos na vaga. */}
      <Card>
        <CardContent className="pt-6">
          <Collapsible defaultOpen={false}>
            <CollapsibleTrigger className="flex w-full items-center justify-between py-1 -mx-1 rounded hover:bg-muted/50 px-1 data-[state=open]:mb-4">
              <h2 className="text-base font-semibold">Informações sobre a vaga</h2>
              <ChevronDown className="h-4 w-4 shrink-0 transition-transform duration-200 data-[state=open]:rotate-180" />
            </CollapsibleTrigger>
            <CollapsibleContent>
              {dadosVagaOrigem == null ? (
                <p className="text-sm text-muted-foreground">
                  Nenhum dado de vaga recebido (acesso não foi pela lista de Candidatos da vaga).
                </p>
              ) : (
                (() => {
                  const fmt = (v: unknown): string =>
                    v == null || v === '' ? 'Não informado' : String(v).trim() || 'Não informado';
                  const localizacao =
                    [dadosVagaOrigem.estado, dadosVagaOrigem.cidade]
                      .filter((v) => v != null && String(v).trim() !== '')
                      .join(' / ') || 'Não informado';
                  const frequenciaStr = (() => {
                    const f = formatFrequenciaPresencial(dadosVagaOrigem.frequencia as string | number | null | undefined);
                    return f === '—' ? 'Não informado' : f;
                  })();
                  const custoStr =
                    dadosVagaOrigem.custoProfissional != null && dadosVagaOrigem.custoProfissional !== ''
                      ? formatCurrencyBRL(dadosVagaOrigem.custoProfissional)
                      : 'Não informado';
                  return (
                    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                      <div className="space-y-3">
                        <div className="space-y-1">
                          <p className="text-sm font-medium text-muted-foreground">Código</p>
                          <p className="text-sm">{fmt(dadosVagaOrigem.codigo)}</p>
                        </div>
                        <div className="space-y-1">
                          <p className="text-sm font-medium text-muted-foreground">Cliente</p>
                          <p className="text-sm">{fmt(dadosVagaOrigem.nomeCliente)}</p>
                        </div>
                        <div className="space-y-1">
                          <p className="text-sm font-medium text-muted-foreground">Gestor</p>
                          <p className="text-sm">{fmt(dadosVagaOrigem.nomeGestor)}</p>
                        </div>
                        <div className="space-y-1">
                          <p className="text-sm font-medium text-muted-foreground">Abertura em</p>
                          <p className="text-sm">{formatDatePtBr(dadosVagaOrigem.dataCriacao as string, 'Não informado')}</p>
                        </div>
                        <div className="space-y-1">
                          <p className="text-sm font-medium text-muted-foreground">Contratação em</p>
                          <p className="text-sm">{formatDatePtBr(dadosVagaOrigem.dataAlteracao as string, 'Não informado')}</p>
                        </div>
                        <div className="space-y-1">
                          <p className="text-sm font-medium text-muted-foreground">Observações internas</p>
                          <p className="text-sm">{fmt(dadosVagaOrigem.observacoesInternas)}</p>
                        </div>
                      </div>
                      <div className="space-y-3">
                        <div className="space-y-1">
                          <p className="text-sm font-medium text-muted-foreground">Proposta</p>
                          <p className="text-sm">{fmt(dadosVagaOrigem.propostaCrm)}</p>
                        </div>
                        <div className="space-y-1">
                          <p className="text-sm font-medium text-muted-foreground">Tipo de contratação</p>
                          <p className="text-sm">{fmt(dadosVagaOrigem.tipoContratacaoId)}</p>
                        </div>
                        <div className="space-y-1">
                          <p className="text-sm font-medium text-muted-foreground">Custo</p>
                          <p className="text-sm">{custoStr}</p>
                        </div>
                      </div>
                      <div className="space-y-3">
                        <div className="space-y-1">
                          <p className="text-sm font-medium text-muted-foreground">Modelo de trabalho</p>
                          <p className="text-sm">{fmt(dadosVagaOrigem.modeloTrabalhoDescricao)}</p>
                        </div>
                        <div className="space-y-1">
                          <p className="text-sm font-medium text-muted-foreground">Localização</p>
                          <p className="text-sm">{localizacao}</p>
                        </div>
                        <div className="space-y-1">
                          <p className="text-sm font-medium text-muted-foreground">Frequência</p>
                          <p className="text-sm">{frequenciaStr}</p>
                        </div>
                      </div>
                    </div>
                  );
                })()
              )}
            </CollapsibleContent>
          </Collapsible>
        </CardContent>
      </Card>

      {/* Dados da Vaga — formulário */}
      <Card>
        <CardContent className="pt-6">
          <Collapsible defaultOpen>
            <CollapsibleTrigger className="flex w-full items-center justify-between py-1 -mx-1 rounded hover:bg-muted/50 px-1 data-[state=open]:mb-4">
              <h2 className="text-base font-semibold">Dados da Vaga</h2>
              <ChevronDown className="h-4 w-4 shrink-0 transition-transform duration-200 data-[state=open]:rotate-180" />
            </CollapsibleTrigger>
            <CollapsibleContent>
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
            <div className="space-y-2">
              <Label>Esta vaga é do Tipo:</Label>
              <Input
                readOnly
                value={template?.descricaoTipoVaga?.trim() ? template.descricaoTipoVaga : 'Não informado'}
                placeholder="Não informado"
                className="bg-muted cursor-not-allowed"
                aria-readonly
              />
            </div>
            <div className="space-y-2">
              <ColaboradorSearchField
                label="Analista de R&S *"
                token={token}
                valueCodigo={template?.colaboradorCodigoInternoColaboradorAnalista}
                valueDisplayName={template?.nomeColaboradorAnalista}
                onSelect={onSelectAnalista}
                placeholder="Pesquise o nome do analista"
              />
              {formErrors.colaboradorCodigoInternoColaboradorAnalista && (
                <p className="text-xs text-destructive">{formErrors.colaboradorCodigoInternoColaboradorAnalista}</p>
              )}
            </div>
            <div className="space-y-2">
              <Label>Cargo</Label>
              <Input
                value={template?.cargo ?? ''}
                onChange={(e) => {
                  const value = e.target.value.slice(0, 100);
                  updateTemplate({ cargo: value });
                  if (value.trim()) clearFormError('cargo');
                }}
                placeholder="Cargo"
                maxLength={100}
                className={cn(formErrors.cargo && 'border-destructive')}
              />
              {formErrors.cargo && (
                <p className="text-xs text-destructive">{formErrors.cargo}</p>
              )}
            </div>
            <div className="space-y-2">
              <Label>Data de Início*</Label>
              <Input
                type="date"
                value={toInputDate(template?.dataInicio)}
                onChange={(e) =>
                  updateTemplate({
                    dataInicio: e.target.value ? dateInputValueToIsoLocal(e.target.value) : null,
                  })
                }
                className={cn(formErrors.dataInicio && 'border-destructive')}
                placeholder="dd/mm/yyyy"
              />
              {formErrors.dataInicio && (
                <p className="text-xs text-destructive">{formErrors.dataInicio}</p>
              )}
            </div>
            <div className="space-y-2">
              <Label>Horário / Jornada</Label>
              <Input
                readOnly
                value={(template?.horarioJornada ?? '').trim() || 'SEG A SEXTA 09:00 12:00/13:00 18:00'}
                className={cn('bg-muted cursor-not-allowed', formErrors.horarioJornada && 'border-destructive')}
              />
              {formErrors.horarioJornada && (
                <p className="text-xs text-destructive">{formErrors.horarioJornada}</p>
              )}
            </div>
            <div className="space-y-2">
              <Label>Tipo de Jornada*</Label>
              <Select
                value={template?.tipoHorarioJornada === 'true' ? 'true' : 'false'}
                onValueChange={(v) => updateTemplate({ tipoHorarioJornada: v })}
              >
                <SelectTrigger><SelectValue placeholder="Selecionar" /></SelectTrigger>
                <SelectContent>
                  {TIPO_JORNADA_OPCOES.map((o) => (
                    <SelectItem key={o.value} value={o.value}>
                      {o.label}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            <div className="space-y-2">
              <Label>Modelo de trabalho *</Label>
              <Select
                value={template?.modeloTrabalhoId && modelosTrabalhoList.some((m) => m.id === template.modeloTrabalhoId) ? template.modeloTrabalhoId : undefined}
                onValueChange={(v) => {
                  const item = modelosTrabalhoList.find((m) => m.id === v);
                  const desc = (item?.descricao ?? '').toString().toLowerCase();
                  const novoEhHibrido = desc.includes('híbrido');
                  updateTemplate({
                    modeloTrabalhoId: v || null,
                    modeloTrabalhoDescricao: item?.descricao ?? null,
                    quantidadeDiasPresencial: novoEhHibrido
                      ? (template?.quantidadeDiasPresencial != null && template.quantidadeDiasPresencial >= 1 && template.quantidadeDiasPresencial <= 5
                        ? template.quantidadeDiasPresencial
                        : null)
                      : DIAS_PRESENCIAL_DEFAULT_OCULTO,
                  });
                }}
                disabled={loadingModelosTrabalho}
              >
                <SelectTrigger><SelectValue placeholder="Selecionar" /></SelectTrigger>
                <SelectContent>
                  {modelosTrabalhoList.map((m) => (
                    <SelectItem key={m.id} value={m.id}>
                      {m.descricao}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            {isModeloHibrido && (
            <div className="space-y-2">
              <Label>Dias no presencial *</Label>
              <Select
                value={
                  template?.quantidadeDiasPresencial != null && template.quantidadeDiasPresencial >= 1 && template.quantidadeDiasPresencial <= 5
                    ? String(template.quantidadeDiasPresencial)
                    : undefined
                }
                onValueChange={(v) => updateTemplate({ quantidadeDiasPresencial: v === '' ? null : Number(v) })}
              >
                <SelectTrigger className={cn(formErrors.quantidadeDiasPresencial && 'border-destructive')}>
                  <SelectValue placeholder="Selecionar" />
                </SelectTrigger>
                <SelectContent>
                  {DIAS_PRESENCIAL_OPCOES.map((o) => (
                    <SelectItem key={o.value} value={o.value}>
                      {o.label}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
              {formErrors.quantidadeDiasPresencial && (
                <p className="text-xs text-destructive">{formErrors.quantidadeDiasPresencial}</p>
              )}
            </div>
            )}
          </div>
            </CollapsibleContent>
          </Collapsible>
        </CardContent>
      </Card>

      {/* Informações Pessoais */}
      <Card>
        <CardContent className="pt-6">
          <Collapsible defaultOpen>
            <CollapsibleTrigger className="flex w-full items-center justify-between py-1 -mx-1 rounded hover:bg-muted/50 px-1 data-[state=open]:mb-4">
              <h2 className="text-base font-semibold">Informações Pessoais</h2>
              <ChevronDown className="h-4 w-4 shrink-0 transition-transform duration-200 data-[state=open]:rotate-180" />
            </CollapsibleTrigger>
            <CollapsibleContent>
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
            <div className="space-y-2">
              <Label>CPF*</Label>
              <Input
                value={formatCPF(template?.documentoColaborador ?? '')}
                onChange={(e) => updateTemplate({ documentoColaborador: e.target.value.replace(/\D/g, '').slice(0, 11) })}
                placeholder="000.000.000-00"
                maxLength={14}
                inputMode="numeric"
                className={cn(formErrors.documentoColaborador && 'border-destructive')}
              />
              {formErrors.documentoColaborador && (
                <p className="text-xs text-destructive">{formErrors.documentoColaborador}</p>
              )}
            </div>
            <div className="space-y-2">
              <Label>RG*</Label>
              <Input
                value={template?.rgColaborador ?? ''}
                onChange={(e) => updateTemplate({ rgColaborador: e.target.value.slice(0, 15) })}
                maxLength={15}
                className={cn(formErrors.rgColaborador && 'border-destructive')}
              />
              {formErrors.rgColaborador && (
                <p className="text-xs text-destructive">{formErrors.rgColaborador}</p>
              )}
            </div>
            <div className="space-y-2">
              <Label>Data Nascimento*</Label>
              <Input
                type="date"
                value={toInputDate(template?.dataNascimento)}
                onChange={(e) =>
                  updateTemplate({
                    dataNascimento: e.target.value ? dateInputValueToIsoLocal(e.target.value) : null,
                  })
                }
                className={cn(formErrors.dataNascimento && 'border-destructive')}
              />
              {formErrors.dataNascimento && (
                <p className="text-xs text-destructive">{formErrors.dataNascimento}</p>
              )}
            </div>
            <div className="space-y-2">
              <Label>DDD *</Label>
              <Input
                value={template?.dddColaborador ?? ''}
                onChange={(e) => updateTemplate({ dddColaborador: e.target.value.replace(/\D/g, '').slice(0, 2) })}
                placeholder="DDD"
                maxLength={2}
                inputMode="numeric"
                className={cn(formErrors.dddColaborador && 'border-destructive')}
              />
              {formErrors.dddColaborador && (
                <p className="text-xs text-destructive">{formErrors.dddColaborador}</p>
              )}
            </div>
            <div className="space-y-2">
              <Label>Telefone *</Label>
              <Input
                value={formatarApenasNumero(template?.telefone ?? '')}
                onChange={(e) => {
                  const dig = e.target.value.replace(/\D/g, '').slice(0, 9);
                  updateTemplate({ telefone: dig });
                }}
                placeholder="Digite o telefone"
                maxLength={10}
                inputMode="tel"
                className={cn(formErrors.telefone && 'border-destructive')}
              />
              {formErrors.telefone && (
                <p className="text-xs text-destructive">{formErrors.telefone}</p>
              )}
            </div>
            {!isOrg9 && (
            <div className="space-y-2">
              <Label>Tamanho Camiseta</Label>
              <Select
                value={template?.tamanhoCamiseta && TAMANHO_CAMISETA_OPCOES.includes(template.tamanhoCamiseta as (typeof TAMANHO_CAMISETA_OPCOES)[number]) ? template.tamanhoCamiseta ?? undefined : undefined}
                onValueChange={(v) => updateTemplate({ tamanhoCamiseta: v || null })}
              >
                <SelectTrigger className={cn(formErrors.tamanhoCamiseta && 'border-destructive')}>
                  <SelectValue placeholder="Selecionar" />
                </SelectTrigger>
                <SelectContent>
                  {TAMANHO_CAMISETA_OPCOES.map((t) => (
                    <SelectItem key={t} value={t}>
                      {t}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
              {formErrors.tamanhoCamiseta && (
                <p className="text-xs text-destructive">{formErrors.tamanhoCamiseta}</p>
              )}
            </div>
            )}
            <div className="space-y-2">
              <Label>Email Pessoal*</Label>
              <Input
                type="email"
                value={template?.emailPessoal ?? ''}
                onChange={(e) => updateTemplate({ emailPessoal: e.target.value.slice(0, 200) })}
                maxLength={200}
                className={cn(formErrors.emailPessoal && 'border-destructive')}
              />
              {formErrors.emailPessoal && (
                <p className="text-xs text-destructive">{formErrors.emailPessoal}</p>
              )}
            </div>
          </div>
          <div className="mt-6">
            <h3 className="text-sm font-medium mb-3">Endereço</h3>
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
              <div className="space-y-2">
                <Label>CEP*</Label>
                <Input
                  value={formatarCep(endereco.cep ?? '')}
                  onChange={(e) => {
                    const valor = e.target.value;
                    const apenasNumeros = valor.replace(/\D/g, '');
                    if (apenasNumeros.length > 8) return;
                    const formatado = formatarCep(valor);
                    updateEndereco({ cep: formatado });
                    const digitos = cepSoDigitos(valor);
                    if (digitos.length === 8) {
                      buscarCep(digitos);
                    }
                  }}
                  placeholder="00000-000"
                  inputMode="numeric"
                  maxLength={9}
                  className={cn(formErrors.cep && 'border-destructive')}
                />
                {formErrors.cep && (
                  <p className="text-xs text-destructive">{formErrors.cep}</p>
                )}
                {loadingCep && (
                  <p className="text-xs text-muted-foreground">Buscando endereço...</p>
                )}
              </div>
              <div className="space-y-2 md:col-span-2">
                <Label>Rua*</Label>
                <Input
                  value={endereco.endereco ?? ''}
                  onChange={(e) => updateEndereco({ endereco: e.target.value.slice(0, 100) })}
                  maxLength={100}
                />
              </div>
              <div className="space-y-2">
                <Label>Número*</Label>
                <Input
                  type="number"
                  value={endereco.numero !== undefined && endereco.numero !== null ? String(endereco.numero) : ''}
                  onChange={(e) => {
                    const v = e.target.value.slice(0, 5);
                    updateEndereco({ numero: v === '' ? undefined : Number(v) });
                  }}
                  className={cn(formErrors.enderecoNumero && 'border-destructive')}
                />
                {formErrors.enderecoNumero && (
                  <p className="text-xs text-destructive">{formErrors.enderecoNumero}</p>
                )}
              </div>
              <div className="space-y-2">
                <Label>Complemento</Label>
                <Input
                  value={endereco.complemento ?? ''}
                  onChange={(e) => updateEndereco({ complemento: e.target.value.slice(0, 50) })}
                  maxLength={50}
                />
              </div>
              <div className="space-y-2">
                <Label>Bairro*</Label>
                <Input
                  value={endereco.bairro ?? ''}
                  onChange={(e) => updateEndereco({ bairro: e.target.value.slice(0, 50) })}
                  maxLength={50}
                  className={cn(formErrors.enderecoBairro && 'border-destructive')}
                />
                {formErrors.enderecoBairro && (
                  <p className="text-xs text-destructive">{formErrors.enderecoBairro}</p>
                )}
              </div>
              <div className="space-y-2">
                <Label>Cidade*</Label>
                <Input
                  value={endereco.cidade ?? ''}
                  onChange={(e) => updateEndereco({ cidade: e.target.value.slice(0, 50) })}
                  maxLength={50}
                  className={cn(formErrors.enderecoCidade && 'border-destructive')}
                />
                {formErrors.enderecoCidade && (
                  <p className="text-xs text-destructive">{formErrors.enderecoCidade}</p>
                )}
              </div>
              <div className="space-y-2">
                <Label>Estado*</Label>
                <Select
                  value={endereco.estado && UF_ESTADOS_BR.includes(endereco.estado as (typeof UF_ESTADOS_BR)[number]) ? endereco.estado ?? undefined : undefined}
                  onValueChange={(v) => updateEndereco({ estado: v })}
                >
                  <SelectTrigger className={cn(formErrors.enderecoEstado && 'border-destructive')}>
                    <SelectValue placeholder="UF" />
                  </SelectTrigger>
                  <SelectContent>
                    {UF_ESTADOS_BR.map((uf) => (
                      <SelectItem key={uf} value={uf}>
                        {uf}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
                {formErrors.enderecoEstado && (
                  <p className="text-xs text-destructive">{formErrors.enderecoEstado}</p>
                )}
              </div>
              <div className="space-y-2">
                <Label>Com quem mora</Label>
                <Input
                  value={endereco.com_quem_mora ?? ''}
                  onChange={(e) => updateEndereco({ com_quem_mora: e.target.value.slice(0, 100) })}
                  maxLength={100}
                />
              </div>
            </div>
          </div>
            </CollapsibleContent>
          </Collapsible>
        </CardContent>
      </Card>

      {/* Benefícios e Lançamento Complementar: exibido apenas quando exibirRemuneracao === true (item ListarCandidatosInscritos do card clicado). */}
      {exibirRemuneracao && (
      <Card>
        <CardContent className="pt-6">
          <Collapsible defaultOpen>
            <CollapsibleTrigger className="flex w-full items-center justify-between py-1 -mx-1 rounded hover:bg-muted/50 px-1 data-[state=open]:mb-4">
              <h2 className="text-base font-semibold">Benefícios e Lançamento Complementar</h2>
              <ChevronDown className="h-4 w-4 shrink-0 transition-transform duration-200 data-[state=open]:rotate-180" />
            </CollapsibleTrigger>
            <CollapsibleContent>
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
            <div className="space-y-2">
              <Label>Salário / Remuneração Base*</Label>
              <Input
                value={formatCurrencyBRL(template?.salario ?? 0)}
                onChange={(e) => {
                  const n = parseBRLCurrencyInput(e.target.value);
                  updateTemplate({ salario: n != null ? n : null });
                }}
                placeholder="R$ 0,00"
                inputMode="decimal"
              />
            </div>
            <div className="space-y-2 flex flex-col justify-end">
              <div className="flex items-center gap-2">
                <Switch
                  id="cargoConfianca"
                  checked={template?.cargoConfianca ?? false}
                  onCheckedChange={(checked) => updateTemplate({ cargoConfianca: !!checked })}
                />
                <Label htmlFor="cargoConfianca" className="cursor-pointer">Cargo de Confiança?</Label>
              </div>
            </div>
            <div className="space-y-2">
              <Label>Valor adicional.</Label>
              <Input
                value={formatCurrencyBRL(template?.valorAdicionalCargoConfianca ?? 0)}
                onChange={(e) => {
                  const n = parseBRLCurrencyInput(e.target.value);
                  updateTemplate({ valorAdicionalCargoConfianca: n != null ? n : null });
                }}
                placeholder="R$ 0,00"
                inputMode="decimal"
              />
            </div>
            <div className="space-y-2">
              <Label>Vale Alimentação</Label>
              <Input
                value={formatCurrencyBRL(template?.va ?? 0)}
                onChange={(e) => {
                  const n = parseBRLCurrencyInput(e.target.value);
                  updateTemplate({ va: n != null ? n : null });
                }}
                placeholder="R$ 0,00"
                inputMode="decimal"
              />
            </div>
            <div className="space-y-2">
              <Label>Vale Refeição</Label>
              <Input
                value={formatCurrencyBRL(template?.vr ?? 0)}
                onChange={(e) => {
                  const n = parseBRLCurrencyInput(e.target.value);
                  updateTemplate({ vr: n != null ? n : null });
                }}
                placeholder="R$ 0,00"
                inputMode="decimal"
              />
            </div>
            <div className="space-y-2">
              <Label>Ajuda de Custo</Label>
              <Input
                value={formatCurrencyBRL(template?.ajudaDeCusto ?? 0)}
                onChange={(e) => {
                  const n = parseBRLCurrencyInput(e.target.value);
                  updateTemplate({ ajudaDeCusto: n != null ? n : null });
                }}
                placeholder="R$ 0,00"
                inputMode="decimal"
              />
            </div>
            <div className="space-y-2">
              <Label>Assistência Médica</Label>
              <Input
                value={formatCurrencyBRL(template?.assistenciaMedica ?? 0)}
                onChange={(e) => {
                  const n = parseBRLCurrencyInput(e.target.value);
                  updateTemplate({ assistenciaMedica: n != null ? n : null });
                }}
                placeholder="R$ 0,00"
                inputMode="decimal"
              />
            </div>
            <div className="space-y-2">
              <Label>Assistência Educacional</Label>
              <Input
                value={formatCurrencyBRL(template?.educacao ?? 0)}
                onChange={(e) => {
                  const n = parseBRLCurrencyInput(e.target.value);
                  updateTemplate({ educacao: n != null ? n : null });
                }}
                placeholder="R$ 0,00"
                inputMode="decimal"
              />
            </div>
            <div className="space-y-2">
              <Label>Mobilidade / Transporte</Label>
              <Input
                value={formatCurrencyBRL(template?.mobilidade ?? 0)}
                onChange={(e) => {
                  const n = parseBRLCurrencyInput(e.target.value);
                  updateTemplate({ mobilidade: n != null ? n : null });
                }}
                placeholder="R$ 0,00"
                inputMode="decimal"
              />
            </div>
            <div className="space-y-2">
              <Label>Custo Hora Profissional</Label>
              <Input
                value={formatCurrencyBRL(template?.custoHora ?? 0)}
                onChange={(e) => {
                  const n = parseBRLCurrencyInput(e.target.value);
                  updateTemplate({ custoHora: n != null ? n : null });
                }}
                placeholder="R$ 0,00"
                inputMode="decimal"
              />
            </div>
            <div className="space-y-2">
              <Label>Remuneração Total</Label>
              <Input
                value={formatCurrencyBRL(template?.remuneracaoTotal ?? 0)}
                onChange={(e) => {
                  const n = parseBRLCurrencyInput(e.target.value);
                  updateTemplate({ remuneracaoTotal: n != null ? n : null });
                }}
                placeholder="R$ 0,00"
                inputMode="decimal"
              />
            </div>
          </div>
            </CollapsibleContent>
          </Collapsible>
        </CardContent>
      </Card>
      )}

      {/* Saúde do Candidato */}
      <Card>
        <CardContent className="pt-6">
          <Collapsible defaultOpen>
            <CollapsibleTrigger className="flex w-full items-center justify-between py-1 -mx-1 rounded hover:bg-muted/50 px-1 data-[state=open]:mb-4">
              <span className="flex flex-col items-start gap-0.5 text-left">
                <span className="flex items-center gap-2">
                  <h2 className="text-base font-semibold">Saúde do Candidato</h2>
                  <TooltipProvider>
                    <Tooltip>
                      <TooltipTrigger asChild>
                        <span className="inline-flex h-5 w-5 items-center justify-center rounded-full bg-destructive/10 text-destructive">
                          <Info className="h-3 w-3" />
                        </span>
                      </TooltipTrigger>
                      <TooltipContent side="right" className="max-w-xs">
                        <p className="text-sm">Informações sobre saúde e adaptações do candidato.</p>
                      </TooltipContent>
                    </Tooltip>
                  </TooltipProvider>
                </span>
                <p className="text-xs font-normal text-muted-foreground">Apenas visualização</p>
              </span>
              <ChevronDown className="h-4 w-4 shrink-0 transition-transform duration-200 data-[state=open]:rotate-180" />
            </CollapsibleTrigger>
            <CollapsibleContent>
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            <div className="space-y-2">
              <Label>Candidato PCD?</Label>
              <Select value={saude.pcd ?? 'Não'} onValueChange={() => {}}>
                <SelectTrigger disabled className="bg-muted/50 cursor-not-allowed opacity-100"><SelectValue placeholder="Selecionar" /></SelectTrigger>
                <SelectContent>
                  <SelectItem value="Sim">Sim</SelectItem>
                  <SelectItem value="Não">Não</SelectItem>
                </SelectContent>
              </Select>
            </div>
            <div className="space-y-2">
              <Label>Tipo de deficiência</Label>
              <Select
                value={
                  saude.enumPCD != null && saude.enumPCD >= 0 && saude.enumPCD <= 6
                    ? String(saude.enumPCD)
                    : undefined
                }
                onValueChange={() => {}}
              >
                <SelectTrigger disabled className="bg-muted/50 cursor-not-allowed opacity-100"><SelectValue placeholder="Selecionar" /></SelectTrigger>
                <SelectContent>
                  {TIPO_DEFICIENCIA_OPCOES.map((o) => (
                    <SelectItem key={o.value} value={o.value}>
                      {o.label}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            <div className="space-y-2 md:col-span-3">
              <Label>Necessita de adaptação?</Label>
              <Textarea
                value={template?.restricaoAdaptacao ?? ''}
                readOnly
                rows={2}
                placeholder="Descreva se necessário"
                className="bg-muted/50 cursor-not-allowed resize-none"
              />
            </div>
          </div>
            </CollapsibleContent>
          </Collapsible>
        </CardContent>
      </Card>

      {!isOrg9 && (
      <>
      {/* Acessórios Foursys */}
      <Card>
        <CardContent className="pt-6">
          <Collapsible defaultOpen>
            <CollapsibleTrigger className="flex w-full items-center justify-between py-1 -mx-1 rounded hover:bg-muted/50 px-1 data-[state=open]:mb-4">
              <h2 className="text-base font-semibold">Acessórios Foursys</h2>
              <ChevronDown className="h-4 w-4 shrink-0 transition-transform duration-200 data-[state=open]:rotate-180" />
            </CollapsibleTrigger>
            <CollapsibleContent>
          <div className="flex flex-wrap gap-6">
            <div className="flex items-center space-x-2">
              <Checkbox
                id="celular"
                checked={template?.celular ?? false}
                onCheckedChange={(c) => updateTemplate({ celular: !!c })}
              />
              <Label htmlFor="celular">Celular</Label>
            </div>
            <div className="flex items-center space-x-2">
              <Checkbox
                id="planoDados"
                checked={template?.planoDados ?? false}
                onCheckedChange={(c) => updateTemplate({ planoDados: !!c })}
              />
              <Label htmlFor="planoDados">Plano de Dados</Label>
            </div>
            <div className="flex items-center space-x-2">
              <Checkbox
                id="cartaoVisitas"
                checked={template?.cartaoVisitas ?? false}
                onCheckedChange={(c) => updateTemplate({ cartaoVisitas: !!c })}
              />
              <Label htmlFor="cartaoVisitas">Cartão de Visitas</Label>
            </div>
          </div>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4 mt-4">
            <div className="space-y-2">
              <Label>Quantidade minutos Plano de Dados</Label>
              <Input
                type="number"
                min={0}
                value={String(template?.quantidadeMinutosPlanoDados ?? 0)}
                onChange={(e) => updateTemplate({ quantidadeMinutosPlanoDados: Number(e.target.value) || 0 })}
              />
            </div>
            <div className="space-y-2">
              <Label>Quantidade Cartão de Visitas</Label>
              <Input
                type="number"
                min={0}
                value={String(template?.quantidadeCartaoVisitas ?? 0)}
                onChange={(e) => updateTemplate({ quantidadeCartaoVisitas: Number(e.target.value) || 0 })}
              />
            </div>
          </div>
          <div className="mt-4 space-y-2">
            <Label>Outros acessórios</Label>
            <div className="relative">
              <Textarea
                value={template?.outrosEquipamentos ?? ''}
                onChange={(e) => updateTemplate({ outrosEquipamentos: e.target.value.slice(0, 500) })}
                rows={2}
                placeholder="Descreva outros equipamentos"
                maxLength={500}
                className="pr-12"
              />
              <span className="absolute bottom-3 right-3 text-xs text-muted-foreground">
                {(template?.outrosEquipamentos ?? '').length}/500
              </span>
            </div>
          </div>
            </CollapsibleContent>
          </Collapsible>
        </CardContent>
      </Card>

      {/* Dados de Localização */}
      <Card>
        <CardContent className="pt-6">
          <Collapsible defaultOpen>
            <CollapsibleTrigger className="flex w-full items-center justify-between py-1 -mx-1 rounded hover:bg-muted/50 px-1 data-[state=open]:mb-4">
              <h2 className="text-base font-semibold">Dados de Localização - Checklist de Instalação</h2>
              <ChevronDown className="h-4 w-4 shrink-0 transition-transform duration-200 data-[state=open]:rotate-180" />
            </CollapsibleTrigger>
            <CollapsibleContent>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div className="space-y-2">
              <Label>Tipo de equipamento necessário *</Label>
              <Select
                value={template?.grupoAreaEquipamentoPadraoCargoFuncao ?? undefined}
                onValueChange={(v) => {
                  updateTemplate({
                    grupoAreaEquipamentoPadraoCargoFuncao: v || null,
                    equipamentoPadraoCargoFuncaoId: null,
                  });
                }}
                disabled={loadingEquipamentos}
              >
                <SelectTrigger className={cn(formErrors.grupoAreaEquipamentoPadraoCargoFuncao && 'border-destructive')}>
                  <SelectValue placeholder="Selecionar" />
                </SelectTrigger>
                <SelectContent>
                  {equipamentosAninhados
                    .filter((g) => g.grupoArea != null && g.grupoArea !== '')
                    .map((g) => (
                      <SelectItem key={g.grupoArea!} value={g.grupoArea!}>
                        {g.grupoArea}
                      </SelectItem>
                    ))}
                </SelectContent>
              </Select>
              {formErrors.grupoAreaEquipamentoPadraoCargoFuncao && (
                <p className="text-xs text-destructive">{formErrors.grupoAreaEquipamentoPadraoCargoFuncao}</p>
              )}
            </div>
            <div className="space-y-2">
              <Label>Cargo x Máquina</Label>
              <Select
                value={template?.equipamentoPadraoCargoFuncaoId ?? undefined}
                onValueChange={(v) => updateTemplate({ equipamentoPadraoCargoFuncaoId: v || null })}
                disabled={!grupoAreaSelecionado || cargosDoGrupo.length === 0}
              >
                <SelectTrigger><SelectValue placeholder={grupoAreaSelecionado ? 'Selecionar' : 'Selecione o tipo de equipamento'} /></SelectTrigger>
                <SelectContent>
                  {cargosDoGrupo
                    .filter((c) => c.idCargoFuncao != null && c.idCargoFuncao !== '')
                    .map((c) => (
                      <SelectItem key={c.idCargoFuncao!} value={c.idCargoFuncao!}>
                        {c.nomeCargoFuncao ?? ''}
                      </SelectItem>
                    ))}
                </SelectContent>
              </Select>
            </div>
            <div className="space-y-2">
              <Label>Proprietário da Máquina</Label>
              <Select
                value={template?.proprietarioMaquina && PROPRIETARIO_MAQUINA_OPCOES.includes(template.proprietarioMaquina as (typeof PROPRIETARIO_MAQUINA_OPCOES)[number]) ? template.proprietarioMaquina ?? undefined : undefined}
                onValueChange={(v) => updateTemplate({ proprietarioMaquina: v || null })}
              >
                <SelectTrigger><SelectValue placeholder="Selecionar" /></SelectTrigger>
                <SelectContent>
                  {PROPRIETARIO_MAQUINA_OPCOES.map((p) => (
                    <SelectItem key={p} value={p}>
                      {p}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            <div className="space-y-2">
              <Label>Softwares Necessários</Label>
              <div className="relative">
                <Textarea
                  value={template?.softwaresNecessarios ?? ''}
                  onChange={(e) => updateTemplate({ softwaresNecessarios: e.target.value.slice(0, 500) })}
                  rows={2}
                  placeholder="Descreva os softwares"
                  maxLength={500}
                  className="pr-12"
                />
                <span className="absolute bottom-3 right-3 text-xs text-muted-foreground">
                  {(template?.softwaresNecessarios ?? '').length}/500
                </span>
              </div>
            </div>
          </div>
          <div className="mt-4 space-y-2">
            <Label>Observações</Label>
            <Textarea
              value={template?.observacoesLocalizacao ?? ''}
              onChange={(e) => updateTemplate({ observacoesLocalizacao: e.target.value.slice(0, 500) })}
              rows={2}
              maxLength={500}
            />
          </div>
            </CollapsibleContent>
          </Collapsible>
        </CardContent>
      </Card>

      {/* Acessos do Usuário */}
      <Card>
        <CardContent className="pt-6">
          <Collapsible defaultOpen>
            <CollapsibleTrigger className="flex w-full items-center justify-between py-1 -mx-1 rounded hover:bg-muted/50 px-1 data-[state=open]:mb-4">
              <h2 className="text-base font-semibold">Acessos do Usuário</h2>
              <ChevronDown className="h-4 w-4 shrink-0 transition-transform duration-200 data-[state=open]:rotate-180" />
            </CollapsibleTrigger>
            <CollapsibleContent>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div className="space-y-2">
              <ColaboradorSearchField
                label="Superior Imediato *"
                token={token}
                valueCodigo={template?.colaboradorCodigoInternoColaboradorSuperiorImediato}
                valueDisplayName={template?.nomeColaboradorSuperiorImediato}
                onSelect={onSelectSuperior}
                placeholder="Pesquise o nome do Superior"
              />
              {formErrors.colaboradorCodigoInternoColaboradorSuperiorImediato && (
                <p className="text-xs text-destructive">{formErrors.colaboradorCodigoInternoColaboradorSuperiorImediato}</p>
              )}
            </div>
            <div className="space-y-2">
              <Label>Email corporativo</Label>
              <Input
                type="email"
                value={template?.emailCorporativo ?? ''}
                onChange={(e) => updateTemplate({ emailCorporativo: e.target.value.slice(0, 200) })}
                maxLength={200}
              />
            </div>
            <div className="space-y-2">
              <Label>Login de Rede</Label>
              <Input
                value={template?.loginRede ?? ''}
                onChange={(e) => updateTemplate({ loginRede: e.target.value.slice(0, 50) })}
                maxLength={50}
              />
            </div>
            <div className="space-y-2">
              <Label>Tipo de acesso</Label>
              <Select
                value={template?.tipoLoginRede && TIPO_ACESSO_OPCOES.includes(template.tipoLoginRede as (typeof TIPO_ACESSO_OPCOES)[number]) ? template.tipoLoginRede ?? undefined : undefined}
                onValueChange={(v) => {
                  updateTemplate({ tipoLoginRede: v || null });
                  if (v) clearFormError('tipoLoginRede');
                }}
              >
                <SelectTrigger className={cn(formErrors.tipoLoginRede && 'border-destructive')}>
                  <SelectValue placeholder="Selecionar" />
                </SelectTrigger>
                <SelectContent>
                  {TIPO_ACESSO_OPCOES.map((t) => (
                    <SelectItem key={t} value={t}>
                      {t}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
              {formErrors.tipoLoginRede && (
                <p className="text-xs text-destructive">{formErrors.tipoLoginRede}</p>
              )}
            </div>
          </div>
          <div className="mt-4 space-y-2">
            <Label>Observações de acesso do Usuário *</Label>
            <div className="relative">
              <Textarea
                value={template?.observacoesAcessoUsuario ?? ''}
                onChange={(e) => updateTemplate({ observacoesAcessoUsuario: e.target.value.slice(0, 1000) })}
                rows={2}
                placeholder="Outras observações"
                maxLength={1000}
                className={cn('pr-16', formErrors.observacoesAcessoUsuario && 'border-destructive')}
              />
              <span className="absolute bottom-3 right-3 text-xs text-muted-foreground">
                {(template?.observacoesAcessoUsuario ?? '').length}/1000
              </span>
            </div>
            {formErrors.observacoesAcessoUsuario && (
              <p className="text-xs text-destructive">{formErrors.observacoesAcessoUsuario}</p>
            )}
          </div>
            </CollapsibleContent>
          </Collapsible>
        </CardContent>
      </Card>

      {/* Sistemas Liberados */}
      <Card>
        <CardContent className="pt-6">
          <Collapsible defaultOpen>
            <CollapsibleTrigger className="flex w-full items-center justify-between py-1 -mx-1 rounded hover:bg-muted/50 px-1 data-[state=open]:mb-4">
              <h2 className="text-base font-semibold">Sistemas Liberados</h2>
              <ChevronDown className="h-4 w-4 shrink-0 transition-transform duration-200 data-[state=open]:rotate-180" />
            </CollapsibleTrigger>
            <CollapsibleContent>
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-3">
            {opcoesSistemas.map((item) => {
              const label = item.descricao || '';
              const key = item.id ?? label;
              return (
                <div key={key} className="flex items-center space-x-2">
                  <Checkbox
                    id={`sistema-${String(key).replace(/\s/g, '-')}`}
                    checked={isSistemaChecked(item)}
                    onCheckedChange={(c) => setSistemaChecked(item, !!c)}
                  />
                  <Label htmlFor={`sistema-${String(key).replace(/\s/g, '-')}`} className="text-sm font-normal cursor-pointer">
                    {label}
                  </Label>
                </div>
              );
            })}
          </div>
            </CollapsibleContent>
          </Collapsible>
        </CardContent>
      </Card>

      {/* Diretórios de Rede */}
      <Card>
        <CardContent className="pt-6">
          <Collapsible defaultOpen>
            <CollapsibleTrigger className="flex w-full items-center justify-between py-1 -mx-1 rounded hover:bg-muted/50 px-1 data-[state=open]:mb-4">
              <h2 className="text-base font-semibold">Diretórios de Rede</h2>
              <ChevronDown className="h-4 w-4 shrink-0 transition-transform duration-200 data-[state=open]:rotate-180" />
            </CollapsibleTrigger>
            <CollapsibleContent>
          <TooltipProvider>
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-3">
              {opcoesDiretorios.map((item) => {
                const dirItem = getDiretorioItem(item);
                const leitura = !!dirItem?.leitura;
                const escrita = !!dirItem?.escrita;
                const label = item.descricao || '';
                const key = item.id ?? label;
                const descricaoExibida = label.length > 15 ? `${label.slice(0, 15)}…` : label;
                return (
                  <div key={key} className="flex flex-wrap items-center gap-4">
                    <Tooltip>
                      <TooltipTrigger asChild>
                        <span className="text-sm w-full md:w-auto shrink-0 cursor-default underline decoration-dotted decoration-muted-foreground">
                          {descricaoExibida}
                        </span>
                      </TooltipTrigger>
                      <TooltipContent side="top" className="max-w-xs">
                        <p className="text-sm">{label}</p>
                      </TooltipContent>
                    </Tooltip>
                  <div className="flex gap-4">
                    <div className="flex items-center space-x-2">
                      <Checkbox
                        id={`dir-leitura-${String(key).replace(/\s/g, '-')}`}
                        checked={leitura}
                        onCheckedChange={(c) => setDiretorioPerm(item, 'leitura', !!c)}
                      />
                      <Label htmlFor={`dir-leitura-${String(key).replace(/\s/g, '-')}`} className="text-sm font-normal cursor-pointer">
                        Leitura
                      </Label>
                    </div>
                    <div className="flex items-center space-x-2">
                      <Checkbox
                        id={`dir-escrita-${String(key).replace(/\s/g, '-')}`}
                        checked={escrita}
                        onCheckedChange={(c) => setDiretorioPerm(item, 'escrita', !!c)}
                      />
                      <Label htmlFor={`dir-escrita-${String(key).replace(/\s/g, '-')}`} className="text-sm font-normal cursor-pointer">
                        Escrita
                      </Label>
                    </div>
                  </div>
                  </div>
                );
              })}
            </div>
          </TooltipProvider>
            </CollapsibleContent>
          </Collapsible>
        </CardContent>
      </Card>

      {/* Grupos de E-mail */}
      <Card>
        <CardContent className="pt-6">
          <Collapsible defaultOpen>
            <CollapsibleTrigger className="flex w-full items-center justify-between py-1 -mx-1 rounded hover:bg-muted/50 px-1 data-[state=open]:mb-4">
              <h2 className="text-base font-semibold">Grupos de E-mail</h2>
              <ChevronDown className="h-4 w-4 shrink-0 transition-transform duration-200 data-[state=open]:rotate-180" />
            </CollapsibleTrigger>
            <CollapsibleContent>
          <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-4 gap-3 mb-4">
            {opcoesGruposEmails.map((item) => {
              const label = item.descricao || '';
              const key = item.id ?? label;
              return (
                <div key={key} className="flex items-center space-x-2">
                  <Checkbox
                    id={`grupo-email-${String(key).replace(/\s/g, '-')}`}
                    checked={isGrupoEmailChecked(item)}
                    onCheckedChange={(c) => setGrupoEmailChecked(item, !!c)}
                  />
                  <Label htmlFor={`grupo-email-${String(key).replace(/\s/g, '-')}`} className="text-sm font-normal cursor-pointer">
                    {label}
                  </Label>
                </div>
              );
            })}
          </div>
          <div className="space-y-2 mb-4">
            <Label>Grupo de E-mail - Contrato</Label>
            <Input
              value={((template?.grupoEmailContrato ?? '').trim() || GRUPO_EMAIL_CONTRATO_DEFAULT)}
              onChange={(e) => {
                const value = e.target.value.slice(0, 255);
                updateTemplate({ grupoEmailContrato: value });
                if (value.trim()) clearFormError('grupoEmailContrato');
              }}
              placeholder={GRUPO_EMAIL_CONTRATO_DEFAULT}
              maxLength={255}
              className={cn(formErrors.grupoEmailContrato && 'border-destructive')}
            />
            {formErrors.grupoEmailContrato && (
              <p className="text-xs text-destructive">{formErrors.grupoEmailContrato}</p>
            )}
          </div>
          <div className="space-y-2">
            <Label>Outros grupos</Label>
            <div className="flex gap-2">
              <Input
                value={outroGrupoEmailInput}
                onChange={(e) => setOutroGrupoEmailInput(e.target.value.slice(0, 255))}
                onKeyDown={(e) => e.key === 'Enter' && (e.preventDefault(), handleAdicionarOutroGrupo())}
                placeholder="Incluir um email de grupo"
                className="flex-1"
                maxLength={255}
              />
              <Button
                type="button"
                variant="primary"
                size="icon"
                aria-label="Adicionar email"
                onClick={handleAdicionarOutroGrupo}
              >
                <Plus className="h-4 w-4" />
              </Button>
            </div>
            {outrosGruposList.length > 0 && (
              <div className="flex flex-wrap gap-2 mt-2">
                {outrosGruposList.map((email) => (
                  <span
                    key={email}
                    className={cn(
                      'inline-flex items-center gap-1.5 rounded-md border bg-muted/50 px-2.5 py-1 text-sm'
                    )}
                  >
                    {email}
                    <button
                      type="button"
                      onClick={() => handleRemoverOutroGrupo(email)}
                      aria-label={`Remover ${email}`}
                      className="p-0.5 rounded hover:bg-muted"
                    >
                      <Trash2 className="h-4 w-4 text-muted-foreground" />
                    </button>
                  </span>
                ))}
              </div>
            )}
          </div>
            </CollapsibleContent>
          </Collapsible>
        </CardContent>
      </Card>

      </>
      )}

      {/* Observações do Aprovador */}
      {!isOrg9 && (
      <Card>
        <CardContent className="pt-6">
          <Collapsible defaultOpen>
            <CollapsibleTrigger className="flex w-full items-center justify-between py-1 -mx-1 rounded hover:bg-muted/50 px-1 data-[state=open]:mb-4">
              <h2 className="text-base font-semibold">Observações do Aprovador</h2>
              <ChevronDown className="h-4 w-4 shrink-0 transition-transform duration-200 data-[state=open]:rotate-180" />
            </CollapsibleTrigger>
            <CollapsibleContent>
          <div className="relative">
            <Textarea
              value={template?.observacoesAprovadorAcessos ?? ''}
              onChange={(e) => {
                const v = e.target.value;
                if (v.length <= 500) updateTemplate({ observacoesAprovadorAcessos: v });
              }}
              rows={4}
              placeholder="Outras observações"
              className="pr-16"
            />
            <span className="absolute bottom-3 right-3 text-xs text-muted-foreground">
              {(template?.observacoesAprovadorAcessos ?? '').length}/500
            </span>
          </div>
            </CollapsibleContent>
          </Collapsible>
        </CardContent>
      </Card>
      )}

      {/* Footer fixo */}
      <div className="fixed bottom-0 right-0 left-0 border-t bg-background p-4 flex justify-end">
        <Button
          onClick={() =>
            handleSalvarValidar((retorno) => {
              if (isOrg9 && !(template?.emailPessoal ?? '').trim()) {
                toast.error('Preencha o campo Email Pessoal para prosseguir com o envio.');
                return;
              }
              generateTemplateContratacaoPDF(retorno, undefined, { modoOrg9: isOrg9 });
              setModalEnvioOpen(true);
            })
          }
          disabled={saving}
          className="gap-2"
        >
          <Download className="h-4 w-4" aria-hidden />
          {isModoCriacao ? 'Salvar Template' : 'Salvar Edição'}
        </Button>
      </div>

      <EnvioTemplateContratacaoModal
        open={modalEnvioOpen}
        onOpenChange={setModalEnvioOpen}
        onEnviar={handleEnviarEmailTemplate}
        isOrg9={isOrg9}
        emailPessoalInicial={(template?.emailPessoal ?? '').trim()}
      />
    </div>
  );
}
