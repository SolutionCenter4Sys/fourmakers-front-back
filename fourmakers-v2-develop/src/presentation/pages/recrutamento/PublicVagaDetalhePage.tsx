import { useEffect, useMemo, useState } from 'react';
import { Button } from '@/components/ui/button';
import { Spinner } from '@/components/ui/spinner';
import { Card, CardContent } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { Checkbox } from '@/components/ui/checkbox';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import { RadioGroup, RadioGroupItem } from '@/components/ui/radio-group';
import { Label } from '@/components/ui/label';
import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { InputOTP, InputOTPGroup, InputOTPSlot } from '@/components/ui/input-otp';
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from '@/components/ui/alert-dialog';
import { CheckCircle2, Upload, Edit, ChevronDown } from '@/components/ui/system-icons';
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from '@/components/ui/popover';
import { ScrollArea } from '@/components/ui/scroll-area';
import { formatCPF } from '@shared/utils/cpfUtils';
import { formatCepForInput } from '@shared/utils/calculations';
import { loadCandidaturaAnalyticsScript } from '@shared/utils/candidaturaAnalyticsScript';
import {
  formatCurrencyInputBR,
  formatWhatsApp,
  matchNivelPorDescricaoParaInclusao,
} from '@presentation/utils/publicVagaUtils';
import { cn } from '@/lib/utils';
import { toast } from 'sonner';
import logoFourmakers from '@/assets/logo-fourmakers.svg';
import loginHero1 from '@/assets/login-hero.jpg';
import loginHero2 from '@/assets/login-hero-2.jpg';
import loginHero3 from '@/assets/login-hero-3.jpg';
import loginHero4 from '@/assets/login-hero-4.jpg';
import loginHero5 from '@/assets/login-hero-5.jpg';
import loginHero6 from '@/assets/login-hero-6.jpg';
import loginHero7 from '@/assets/login-hero-7.jpg';
import loginHero8 from '@/assets/login-hero-8.jpg';
import loginHero9 from '@/assets/login-hero-9.jpg';
import loginHero10 from '@/assets/login-hero-10.jpg';
import {
  usePublicVagaDetalhe,
  type PublicVagaLocationState,
  TIPOS_CONTRATO,
  MODALIDADES,
  MAX_FILE_CURRICULO_MB,
  ACCEPT_CURRICULO,
  CURRICULO_LABEL,
} from '@presentation/hooks/recrutamento'

const heroImages = [
  loginHero1, loginHero2, loginHero3, loginHero4, loginHero5,
  loginHero6, loginHero7, loginHero8, loginHero9, loginHero10,
];

/** Indicador de etapas: passo 1 Login/Cadastro, passo 2 Dados para vaga, passo 3 Atualize seu perfil. Atributos data-candidatura-* para analytics. */
function StepsIndicatorPublico({ currentStep }: { currentStep: 1 | 2 | 3 }) {
  const steps = [
    { num: 1, label: 'Login/Cadastro' },
    { num: 2, label: 'Dados para vaga' },
    { num: 3, label: 'Atualize seu perfil' },
  ] as const;
  return (
    <div
      className="w-full mb-6"
      data-testid="public-vaga-steps-indicator"
      data-candidatura-step-indicator
      data-candidatura-current-step={currentStep}
    >
      <div className="relative flex justify-between px-2">
        <div className="absolute left-0 right-0 top-4 h-0.5 bg-border -translate-y-1/2 z-0" aria-hidden />
        {steps.map((step) => {
          const isActive = step.num === currentStep;
          return (
            <div key={step.num} className="relative z-10 flex flex-col items-center flex-1" data-candidatura-step-num={step.num}>
              <div
                className={cn(
                  'flex h-8 w-8 shrink-0 items-center justify-center rounded-full border-2 text-sm font-semibold shadow-sm bg-background',
                  isActive
                    ? 'border-primary bg-primary text-primary-foreground'
                    : 'border-border text-foreground'
                )}
              >
                {step.num}
              </div>
              <span
                className={cn(
                  'mt-2 text-xs font-medium text-center',
                  isActive ? 'text-primary' : 'text-muted-foreground'
                )}
              >
                {step.label}
              </span>
            </div>
          );
        })}
      </div>
    </div>
  );
}

export type { PublicVagaLocationState };

export function PublicVagaDetalhePage() {
  const {
    codigoFromUrl,
    detalhe,
    loadingPublico,
    user,
    loginStatus,
    showCadastroForm,
    setShowCadastroForm,
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
    skillOpcoes,
    niveisByTipo,
    loadingNiveis,
    setSkillOpcao,
    setSkillNivel,
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
    navigate,
    modalidade,
    estado,
    tecnicas,
    socioemocionais,
    outrosSkills,
    listaSkillsFaltantes,
    logado,
    validandoSessaoPersistida,
    loginError,
  } = usePublicVagaDetalhe();

  /** Script de analytics (ContentSquare) para heatmaps/session replay na candidatura. Centralizado em shared/utils. */
  useEffect(() => {
    const cleanup = loadCandidaturaAnalyticsScript();
    return () => { cleanup?.(); };
  }, []);

  const [preferenciaContatoPopoverOpen, setPreferenciaContatoPopoverOpen] = useState(false);
  const preferenciaContatoLabels = useMemo(
    () =>
      opcoesContatoSelectedIds
        .map((id) => opcoesContato.find((o) => o.id === id)?.descricao)
        .filter(Boolean) as string[],
    [opcoesContatoSelectedIds, opcoesContato]
  );

  const randomHeroImage = useMemo(
    () => heroImages[Math.floor(Math.random() * heroImages.length)],
    []
  );

  return (
    <div className="min-h-screen flex">
      {/* Coluna Esquerda - Detalhes da vaga (card branco igual à página interna) */}
      <div className="flex w-full lg:w-1/2 relative overflow-hidden bg-gradient-to-br from-purple-900 to-indigo-900">
        <img
          src={randomHeroImage}
          alt=""
          className="absolute inset-0 w-full h-full object-cover"
        />
        <div className="absolute inset-0 bg-gradient-to-br from-purple-600/80 via-purple-700/75 to-indigo-700/80" />
        <div className="absolute -bottom-0 -right-0 opacity-20">
          <img
            src="/fourmakers_favicon.svg"
            alt=""
            className="w-[600px] h-[600px] [filter:brightness(0)_saturate(100%)_invert(35%)_sepia(91%)_saturate(3458%)_hue-rotate(257deg)_brightness(92%)_contrast(103%)]"
          />
        </div>

        <div className={`relative z-10 flex flex-col h-full pt-24 px-12 ${isLg ? 'pb-8' : 'pb-24'}`}>
          <Card
            className="max-w-3xl border-borderSoft bg-card shadow-softToken overflow-hidden flex flex-col"
            style={{ maxHeight: isLg ? '90vh' : 'none' }}
          >
            <CardContent className="p-6 space-y-6 flex flex-col flex-1 min-h-0 overflow-y-auto">
              {loadingPublico ? (
                <div className="flex flex-col items-center justify-center py-12" data-testid="public-vaga-loading-detalhe">
                  <Spinner className="h-10 w-10 text-muted-foreground" />
                  <p className="text-sm text-muted-foreground mt-4">Carregando detalhes da vaga...</p>
                </div>
              ) : detalhe ? (
                <>
                  <div>
                    <h1 className="text-xl font-semibold text-primaryText">
                      Detalhes da Vaga – {detalhe.titulo ?? 'Vaga'}
                    </h1>
                    <div className="text-sm text-muted-foreground mt-1">
                      Código: {detalhe.codigo ?? codigoFromUrl ?? '—'}
                    </div>
                  </div>
                  <div>
                    <Badge variant="secondary" className="rounded-md bg-primary/10 text-primary">
                      {modalidade}{estado}
                    </Badge>
                  </div>
                  {tecnicas.length > 0 && (
                    <div>
                      <h3 className="text-sm font-medium text-muted-foreground mb-2">Habilidades Técnicas:</h3>
                      <div className="flex flex-wrap gap-2">
                        {tecnicas.map((s) => (
                          <Badge
                            key={s.id}
                            variant="outline"
                            className="rounded-md border-borderSoft bg-surfaceSubtle text-primaryText font-normal"
                          >
                            {s.skillDescription} – {s.skillNivelDescription}
                          </Badge>
                        ))}
                      </div>
                    </div>
                  )}
                  {socioemocionais.length > 0 && (
                    <div>
                      <h3 className="text-sm font-medium text-muted-foreground mb-2">Habilidades Socioemocionais:</h3>
                      <div className="flex flex-wrap gap-2">
                        {socioemocionais.map((s) => (
                          <Badge
                            key={s.id}
                            variant="outline"
                            className="rounded-md border-borderSoft bg-surfaceSubtle text-primaryText font-normal"
                          >
                            {s.skillDescription} – {s.skillNivelDescription}
                          </Badge>
                        ))}
                      </div>
                    </div>
                  )}
                  {outrosSkills.length > 0 && (
                    <div>
                      <h3 className="text-sm font-medium text-muted-foreground mb-2">Idiomas / Metodologias:</h3>
                      <div className="flex flex-wrap gap-2">
                        {outrosSkills.map((s) => (
                          <Badge
                            key={s.id}
                            variant="outline"
                            className="rounded-md border-borderSoft bg-surfaceSubtle text-primaryText font-normal"
                          >
                            {s.skillDescription} – {s.skillNivelDescription}
                          </Badge>
                        ))}
                      </div>
                    </div>
                  )}
                  <div>
                    <h3 className="text-sm font-medium text-muted-foreground mb-2">Descrição da vaga</h3>
                    <p className="text-sm text-primaryText whitespace-pre-line leading-relaxed">
                      {detalhe.descricao ?? 'Sem descrição disponível.'}
                    </p>
                  </div>
                </>
              ) : (
                <div className="flex flex-col items-center justify-center py-12 text-center">
                  <p className="text-lg font-medium text-primaryText mb-2">
                    {codigoFromUrl ? `Vaga ${codigoFromUrl}` : 'Detalhes da vaga'}
                  </p>
                  <p className="text-sm text-muted-foreground max-w-sm">
                    Não foi possível carregar os detalhes desta vaga. Você pode fazer login ou cadastrar-se para se candidatar.
                  </p>
                </div>
              )}
            </CardContent>
          </Card>
        </div>
      </div>

      {/* Mobile: barra fixa para abrir formulário */}
      {!isLg && !mobileSheetOpen && (
        <div className="fixed bottom-0 left-0 right-0 z-40 p-4 bg-background border-t shadow-lg">
          <Button className="w-full" size="lg" onClick={() => setMobileSheetOpen(true)} data-candidatura-action="abrir-formulario-mobile">
            {logado ? 'Candidatar-se' : 'Entrar ou Cadastre-se'}
          </Button>
        </div>
      )}

      {/* Coluna Direita - Login / Cadastro ou logado (desktop in flow; mobile = bottom sheet). Atributos data-candidatura-* para heatmaps/ContentSquare. */}
      <div
        className={
          isLg
            ? 'w-full lg:w-1/2 flex items-center justify-center bg-background p-8'
            : mobileSheetOpen
              ? 'fixed bottom-0 left-0 right-0 z-50 max-h-[90vh] rounded-t-2xl flex flex-col bg-background shadow-2xl overflow-hidden'
              : 'hidden'
        }
        data-candidatura-page="true"
      >
        {!isLg && mobileSheetOpen && (
          <div className="flex shrink-0 items-center justify-between border-b px-4 py-3">
            <span className="font-medium">Candidatura</span>
            <Button variant="ghost" size="sm" onClick={() => setMobileSheetOpen(false)} data-candidatura-action="fechar-formulario-mobile">
              Fechar
            </Button>
          </div>
        )}
        <div className={isLg ? 'w-full max-w-xl' : 'flex-1 overflow-y-auto p-4'}>
          <Card className={isLg ? 'shadow-xl border-0' : 'shadow-none border-0'}>
            <CardContent className="pt-8">
              <div className="flex flex-col items-center mb-8">
                <img
                  src={logoFourmakers}
                  alt="FourMakers"
                  className="h-12 w-auto dark:invert"
                />
                <p className="text-sm text-muted-foreground text-center mt-4">
                  {logado
                    ? validandoSessaoPersistida
                      ? 'Bem-vindo de volta! Estamos validando sua sessão para continuar a candidatura.'
                      : `Olá, ${user?.nomeColaborador ?? 'usuário'}! Complete seus dados abaixo para se candidatar a esta vaga.`
                    : 'Para se candidatar ou ver os detalhes da vaga, faça login ou cadastre-se.'}
                </p>
              </div>

              {logado ? (
                validandoSessaoPersistida ? (
                  <div className="flex flex-col items-center justify-center py-8" data-candidatura-step="validando-sessao">
                    <Spinner className="h-10 w-10 text-primary" />
                    <p className="text-sm text-muted-foreground mt-4">Validando sua sessão...</p>
                  </div>
                ) : loadingDados ? (
                  <div className="flex flex-col items-center justify-center py-8">
                    <Spinner className="h-10 w-10 text-primary" />
                    <p className="text-sm text-muted-foreground mt-4">Carregando seus dados...</p>
                  </div>
                ) : stepPublico === 4 ? (
                  <div className="space-y-4" data-candidatura-step="4" data-candidatura-resultado="sucesso">
                    <StepsIndicatorPublico currentStep={3} />
                    <p className="text-sm text-foreground">
                      Aumente suas chances de destaque e aproveite para atualizar seu perfil profissional!
                    </p>
                    {curriculoAtualizadoComSucesso ? (
                      <div className="rounded-lg border border-success bg-success/10 p-4 text-center text-sm text-success">
                        <CheckCircle2 className="h-10 w-10 text-success mx-auto mb-2" />
                        <p className="font-medium">Perfil atualizado com sucesso!</p>
                        <p className="mt-1 text-foreground">Seu currículo foi processado e seus dados foram atualizados.</p>
                      </div>
                    ) : (
                      <>
                        <div className="space-y-2">
                          <Label>Atualizar perfil com currículo (PDF)</Label>
                          <div className="flex flex-col gap-2">
                            <input
                              type="file"
                              accept={ACCEPT_CURRICULO}
                              className="text-sm file:mr-2 file:rounded-md file:border-0 file:bg-primary file:px-4 file:py-2 file:text-primary-foreground file:text-sm"
                              onChange={(e) => setFileCurriculo(e.target.files?.[0] ?? null)}
                            />
                            {fileCurriculo && (
                              <Button
                                type="button"
                                size="sm"
                                disabled={submittingInscricao}
                                onClick={handleSyncCurriculoPdf}
                              >
                                {submittingInscricao && <Spinner className="mr-2 h-3 w-3" />}
                                Atualizar perfil com PDF
                              </Button>
                            )}
                          </div>
                          <p className="text-xs text-muted-foreground">{CURRICULO_LABEL}</p>
                          {syncPdfError && (
                            <p className="text-xs text-destructive">Falha ao processar o PDF. Tente outro arquivo ou preencha manualmente.</p>
                          )}
                        </div>
                        <div className="flex flex-col gap-2 pt-2">
                          <Button
                            type="button"
                            variant="outline"
                            className="w-full"
                            onClick={() => window.open(`${window.location.origin}/page/cvdigital`, '_blank')}
                          >
                            Preencher manualmente
                          </Button>
                        </div>
                      </>
                    )}
                    <div className="flex flex-col gap-2 pt-2">
                      <Button type="button" className="w-full" onClick={() => navigate('/dashboard')} data-candidatura-action="acessar-plataforma">
                        Acessar plataforma
                      </Button>
                    </div>
                  </div>
                ) : inscricaoEnviada ? (
                  <div className="rounded-lg border bg-muted/50 p-4 text-center text-sm text-muted-foreground" data-candidatura-step="4" data-candidatura-resultado="inscricao-ok">
                    <CheckCircle2 className="h-10 w-10 text-primary mx-auto mb-2" />
                    <p className="font-medium text-foreground">Inscrição realizada com sucesso!</p>
                    <p className="mt-2">Acompanhe o andamento pela plataforma.</p>
                    <Button className="mt-4 w-full" onClick={() => navigate('/dashboard')} data-candidatura-action="entrar-plataforma">
                      Entrar na plataforma
                    </Button>
                  </div>
                ) : candidaturaJaExiste ? (
                  <div className="rounded-lg border bg-muted/50 p-4 text-center text-sm text-muted-foreground" data-candidatura-step="4" data-candidatura-resultado="ja-candidato">
                    <p className="font-medium text-foreground">Você já possui uma candidatura para esta vaga.</p>
                    <p className="mt-2">Entre na plataforma para acompanhar o andamento.</p>
                    <Button className="mt-4 w-full" onClick={() => navigate('/dashboard')} data-candidatura-action="entrar-plataforma">
                      Entrar na plataforma
                    </Button>
                  </div>
                ) : stepPublico === 3 && dadosColaborador ? (
                  <div
                    className="space-y-4"
                    data-testid="public-vaga-inscricao-step-form"
                    data-candidatura-step="2"
                  >
                    <StepsIndicatorPublico currentStep={2} />
                    <div className="grid gap-4 sm:grid-cols-2" data-candidatura-section="dados-vaga">
                      <div className="space-y-2">
                        <Label>CPF *</Label>
                        <Input
                          placeholder="000.000.000-00"
                          value={cpfInscricao}
                          onChange={(e) => setCpfInscricao(formatCPF(e.target.value))}
                          className="w-full"
                          data-candidatura-field="cpf"
                        />
                      </div>
                      <div className="space-y-2">
                        <Label>CEP *</Label>
                        <Input
                          placeholder="00000-000"
                          value={cepInscricao}
                          onChange={(e) => {
                            const formatted = formatCepForInput(e.target.value);
                            setCepInscricao(formatted);
                            if (formatted.replace(/\D/g, '').length < 8) {
                              setCidadeInscricao('');
                              setEstadoInscricao('');
                            } else if (formatted.replace(/\D/g, '').length === 8) {
                              void buscarCep(formatted);
                            }
                          }}
                          className="w-full"
                          disabled={loadingCep}
                          data-candidatura-field="cep"
                        />
                        {loadingCep && <p className="text-xs text-muted-foreground">Buscando endereço...</p>}
                      </div>
                    </div>
                    <div className="grid gap-4 sm:grid-cols-2">
                      <div className="space-y-2">
                        <Label>Cidade</Label>
                        <Input
                          placeholder="Preenchido pelo CEP"
                          value={cidadeInscricao}
                          onChange={(e) => setCidadeInscricao(e.target.value)}
                          className="w-full"
                          data-candidatura-field="cidade"
                        />
                      </div>
                      <div className="space-y-2">
                        <Label>Estado</Label>
                        <Input
                          placeholder="UF"
                          value={estadoInscricao}
                          onChange={(e) => setEstadoInscricao(e.target.value)}
                          className="w-full"
                          data-candidatura-field="estado"
                        />
                      </div>
                    </div>
                    <div className="space-y-2">
                      <Label>Informe sua pretensão de remuneração líquida? *</Label>
                      <div className="relative">
                        <span className="absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground">R$</span>
                        <Input
                          placeholder="0,00"
                          value={pretensaoSalarialDisplay}
                          onChange={(e) => setPretensaoSalarialDisplay(formatCurrencyInputBR(e.target.value))}
                          className="pl-10 w-full"
                          data-candidatura-field="pretensao-salarial"
                        />
                      </div>
                    </div>
                    <div className="space-y-2">
                      <Label>Modelo de trabalho *</Label>
                      <Select
                        value={modeloTrabalhoId}
                        onValueChange={setModeloTrabalhoId}
                        disabled={loadingModelos}
                      >
                        <SelectTrigger
                          className="w-full"
                          data-testid="public-vaga-modelo-trabalho-trigger"
                          data-candidatura-field="modelo-trabalho"
                        >
                          <SelectValue placeholder={loadingModelos ? 'Carregando...' : 'Selecione o modelo de trabalho'} />
                        </SelectTrigger>
                        <SelectContent>
                          {modelosTrabalho.map((m) => (
                            <SelectItem key={m.id} value={m.id}>
                              {m.descricao}
                            </SelectItem>
                          ))}
                        </SelectContent>
                      </Select>
                    </div>
                    <div className="space-y-2">
                      <Label>Preferência de contato *</Label>
                      <Popover open={preferenciaContatoPopoverOpen} onOpenChange={setPreferenciaContatoPopoverOpen}>
                        <PopoverTrigger asChild>
                          <Button
                            type="button"
                            variant="outline"
                            role="combobox"
                            className={cn(
                              'w-full justify-between font-normal h-auto min-h-10 py-2',
                              !preferenciaContatoLabels.length && 'text-muted-foreground'
                            )}
                            disabled={loadingOpcoes}
                            data-testid="public-vaga-preferencia-contato-trigger"
                            data-candidatura-field="preferencia-contato"
                          >
                            <span className="truncate">
                              {loadingOpcoes
                                ? 'Carregando...'
                                : preferenciaContatoLabels.length > 0
                                  ? preferenciaContatoLabels.join(', ')
                                  : 'Selecione uma ou mais opções'}
                            </span>
                            <ChevronDown className="h-4 w-4 shrink-0 opacity-50" />
                          </Button>
                        </PopoverTrigger>
                        <PopoverContent className="w-[var(--radix-popover-trigger-width)] p-0" align="start">
                          {opcoesContato.length === 0 ? (
                            <div className="p-4 text-sm text-muted-foreground text-center">
                              Nenhuma opção disponível.
                            </div>
                          ) : (
                            <ScrollArea className="max-h-60">
                              <div className="p-1">
                                {opcoesContato.map((item) => (
                                  <label
                                    key={item.id}
                                    className={cn(
                                      'flex items-center gap-2 rounded-md px-2 py-2 text-sm cursor-pointer hover:bg-primary hover:text-primary-foreground'
                                    )}
                                  >
                                    <Checkbox
                                      checked={opcoesContatoSelectedIds.includes(item.id)}
                                      onCheckedChange={() => toggleOpcaoContato(item.id)}
                                    />
                                    <span>{item.descricao}</span>
                                  </label>
                                ))}
                              </div>
                            </ScrollArea>
                          )}
                        </PopoverContent>
                      </Popover>
                    </div>
                    <div
                      className="space-y-3"
                      data-testid="public-vaga-competencias-section"
                      data-candidatura-section="competencias"
                      data-candidatura-step="3"
                    >
                      <Label>Atualize suas competências</Label>
                      {listaSkillsFaltantes.length === 0 ? (
                        <p className="text-sm text-muted-foreground">Você já possui todas as competências mapeadas para esta vaga.</p>
                      ) : (
                        <>
                          <p className="text-sm text-muted-foreground">Para cada habilidade, marque se deseja incluir no seu perfil profissional e ajuste o nível de senioridade, se necessário.</p>
                          <div className="overflow-x-auto pb-2 -mx-1 scrollbar-thin scrollbar-track-muted scrollbar-thumb-muted-foreground/30">
                            <div className="flex gap-3 min-h-0" style={{ scrollSnapType: 'x mandatory' }}>
                              {listaSkillsFaltantes.map((s) => {
                                const skillKey = s.id || `skill-${s.skillId}-${s.tipoSkillId}`;
                                const opcao = skillOpcoes[skillKey];
                                const nivelDescricao = opcao?.nivelDescricao ?? s.skillNivelDescription ?? '';
                                const niveis = niveisByTipo[s.tipoSkillId] ?? [];
                                const isIncluir = opcao?.opcao === 'incluir';
                                const isLoading = addingOrUpdatingSkillId === skillKey;
                                return (
                                  <Card
                                    key={skillKey}
                                    className={cn(
                                      'flex-shrink-0 w-[300px] snap-start border bg-card',
                                      isIncluir && 'border-success bg-success/10'
                                    )}
                                    style={{ scrollSnapAlign: 'start' }}
                                    data-testid={`public-vaga-skill-card-${s.skillId}-${s.tipoSkillId}`}
                                    data-candidatura-skill-card
                                    data-candidatura-skill-id={s.skillId}
                                  >
                                    <CardContent className={cn('p-4 space-y-3', isIncluir && 'text-success')}>
                                      <div className="flex items-start justify-between gap-2">
                                        <h4 className={cn('font-semibold text-sm truncate flex-1', isIncluir ? 'text-success' : 'text-foreground')} title={s.skillDescription}>
                                          {s.skillDescription}
                                        </h4>
                                        <div className="flex items-center gap-1 flex-shrink-0">
                                          {loadingNiveis ? (
                                            <Badge variant="secondary" className="text-xs font-normal">Carregando...</Badge>
                                          ) : (
                                            <Popover>
                                              <PopoverTrigger asChild>
                                                <button
                                                  type="button"
                                                  className={cn(
                                                    'inline-flex items-center gap-1 rounded-md border px-2 py-0.5 text-xs font-medium focus:outline-none focus:ring-2 focus:ring-ring',
                                                    isIncluir ? 'border-success bg-success/10 text-success hover:bg-success/20' : 'bg-muted/50 text-foreground hover:bg-muted'
                                                  )}
                                                  aria-label="Alterar nível de senioridade"
                                                  disabled={isLoading}
                                                  data-candidatura-action="alterar-nivel-skill"
                                                >
                                                  <span className="max-w-[80px] truncate">{nivelDescricao || 'Nível'}</span>
                                                  <Edit className="h-3 w-3 shrink-0 opacity-70" />
                                                </button>
                                              </PopoverTrigger>
                                              <PopoverContent className="w-48 p-2" align="end">
                                                {niveis.length === 0 ? (
                                                  <p className="text-xs text-muted-foreground">Níveis não disponíveis.</p>
                                                ) : (
                                                  <ul className="space-y-0.5">
                                                    {niveis.map((n) => (
                                                      <li key={n.id}>
                                                        <button
                                                          type="button"
                                                          className={cn(
                                                            'w-full text-left px-2 py-1.5 rounded text-sm',
                                                            n.descricao === nivelDescricao ? 'bg-primary text-primary-foreground' : 'hover:bg-muted'
                                                          )}
                                                          onClick={() => {
                                                            setSkillNivel(skillKey, n.id, n.descricao);
                                                            if (opcao?.opcao === 'incluir') {
                                                              atualizarNivelHardSkillPublico(skillKey, n.id);
                                                            }
                                                          }}
                                                        >
                                                          {n.descricao}
                                                        </button>
                                                      </li>
                                                    ))}
                                                  </ul>
                                                )}
                                              </PopoverContent>
                                            </Popover>
                                          )}
                                        </div>
                                      </div>
                                      <p className={cn('text-xs', isIncluir ? 'text-success' : 'text-muted-foreground')}>Você já possui esta habilidade?</p>
                                      <label className="flex items-start gap-2 cursor-pointer" data-candidatura-action="incluir-skill">
                                        <Checkbox
                                          checked={isIncluir}
                                          disabled={isLoading}
                                          data-testid={`public-vaga-skill-incluir-checkbox-${s.skillId}-${s.tipoSkillId}`}
                                          data-candidatura-field="skill-incluir-perfil"
                                          onCheckedChange={(checked) => {
                                            const incluir = checked === true;
                                            if (incluir) {
                                              const cardDesc = (opcao?.nivelDescricao ?? s.skillNivelDescription ?? '').trim();
                                              const niveisLista = niveisByTipo[s.tipoSkillId] ?? [];
                                              const matched =
                                                cardDesc && niveisLista.length > 0
                                                  ? matchNivelPorDescricaoParaInclusao(niveisLista, cardDesc)
                                                  : null;
                                              const nivelId =
                                                matched?.id ?? opcao?.nivelId ?? niveisLista[0]?.id;
                                              setSkillOpcao(
                                                skillKey,
                                                'incluir',
                                                cardDesc || s.skillNivelDescription || undefined,
                                                nivelId
                                              );
                                            } else {
                                              removerSkillPublico(skillKey).then(() => {
                                                setSkillOpcao(skillKey, 'nao_incluir', s.skillNivelDescription ?? undefined);
                                              });
                                            }
                                          }}
                                          className="mt-0.5"
                                        />
                                        <span className="text-xs">Sim, já possuo e quero incluir no meu perfil 360</span>
                                      </label>
                                      {isLoading && (
                                        <div className="flex items-center gap-2 text-xs text-muted-foreground">
                                          <Spinner className="h-3 w-3" />
                                          Salvando...
                                        </div>
                                      )}
                                    </CardContent>
                                  </Card>
                                );
                              })}
                            </div>
                          </div>
                          <div className="rounded-lg bg-warning/10 border border-warning/30 p-3 text-sm text-warning">
                            Confirme suas competências para atualizarmos o seu perfil na plataforma Fourmakers.
                          </div>
                        </>
                      )}
                    </div>
                    <div className="flex gap-2 pt-2">
                      <Button type="button" variant="outline" className="flex-1" onClick={() => setStepPublico(2)} disabled data-candidatura-action="voltar">
                        Voltar
                      </Button>
                      <Button
                        type="button"
                        className="flex-1"
                        disabled={submittingCandidatura || !canSubmitCandidatura}
                        onClick={handleRealizarInscricao}
                        data-testid="public-vaga-realizar-inscricao-button"
                        data-candidatura-action="realizar-inscricao"
                      >
                        {submittingCandidatura && <Spinner className="mr-2 h-4 w-4" />}
                        Realizar inscrição
                      </Button>
                    </div>
                  </div>
                ) : /* Passo 2 "Dados complementares" oculto; código mantido para reativação futura */ false && dadosColaborador ? (
                  <Form {...inscricaoForm}>
                    <form onSubmit={inscricaoForm.handleSubmit(handleInscricaoSubmit)} className="space-y-4">
                      <StepsIndicatorPublico currentStep={2} />
                      <FormField
                        control={inscricaoForm.control}
                        name="cargoAtualUltimo"
                        render={({ field }) => (
                          <FormItem>
                            <FormLabel>Seu cargo atual ou último *</FormLabel>
                            <FormControl>
                              <Input placeholder="Digite aqui" className="w-full" {...field} />
                            </FormControl>
                            <FormMessage />
                          </FormItem>
                        )}
                      />
                      <FormField
                        control={inscricaoForm.control}
                        name="salarioAtualUltimo"
                        render={({ field }) => (
                          <FormItem>
                            <FormLabel>Salário bruto atual ou último *</FormLabel>
                            <FormControl>
                              <div className="relative">
                                <span className="absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground">R$</span>
                                <Input
                                  placeholder="0,00"
                                  className="pl-10 w-full"
                                  {...field}
                                  onChange={(e) => field.onChange(formatCurrencyInputBR(e.target.value))}
                                />
                              </div>
                            </FormControl>
                            <FormMessage />
                          </FormItem>
                        )}
                      />
                      <FormField
                        control={inscricaoForm.control}
                        name="tipoContratoAtualUltimo"
                        render={({ field }) => (
                          <FormItem>
                            <FormLabel>Tipo de contrato atual ou último *</FormLabel>
                            <Select onValueChange={field.onChange} value={field.value}>
                              <FormControl>
                                <SelectTrigger className="w-full">
                                  <SelectValue placeholder="Selecione.." />
                                </SelectTrigger>
                              </FormControl>
                              <SelectContent>
                                {TIPOS_CONTRATO.map((op) => (
                                  <SelectItem key={op.value} value={op.value}>
                                    {op.label}
                                  </SelectItem>
                                ))}
                              </SelectContent>
                            </Select>
                            <FormMessage />
                          </FormItem>
                        )}
                      />
                      <FormField
                        control={inscricaoForm.control}
                        name="modalidadeAtualUltima"
                        render={({ field }) => (
                          <FormItem>
                            <FormLabel>Qual modalidade atual ou última *</FormLabel>
                            <FormControl>
                              <RadioGroup
                                onValueChange={field.onChange}
                                value={field.value}
                                className="flex flex-wrap gap-4"
                              >
                                {MODALIDADES.map((m) => (
                                  <div key={m.value} className="flex items-center space-x-2">
                                    <RadioGroupItem value={m.value} id={`mod-${m.value}`} />
                                    <Label htmlFor={`mod-${m.value}`} className="font-normal cursor-pointer">
                                      {m.label}
                                    </Label>
                                  </div>
                                ))}
                              </RadioGroup>
                            </FormControl>
                            <FormMessage />
                          </FormItem>
                        )}
                      />
                      <div className="space-y-2">
                        <FormField
                          control={inscricaoForm.control}
                          name="contatoPrincipal"
                          render={({ field }) => (
                            <FormItem>
                              <FormLabel>WhatsApp *</FormLabel>
                              <FormControl>
                                <Input
                                  placeholder="(00) 00000-0000"
                                  type="tel"
                                  maxLength={16}
                                  {...field}
                                  onChange={(e) => field.onChange(formatWhatsApp(e.target.value))}
                                />
                              </FormControl>
                              <FormMessage />
                            </FormItem>
                          )}
                        />
                        <FormField
                          control={inscricaoForm.control}
                          name="aceitaSugestoes"
                          render={({ field }) => (
                            <FormItem className="flex flex-row items-start space-x-2 space-y-0">
                              <FormControl>
                                <Checkbox
                                  checked={field.value}
                                  onCheckedChange={field.onChange}
                                />
                              </FormControl>
                              <Label className="text-sm font-normal cursor-pointer">
                                Aceito receber sugestões de vagas e informações pelo WhatsApp.
                              </Label>
                            </FormItem>
                          )}
                        />
                      </div>
                      <div className="space-y-2">
                        <Label>Insira seu currículo</Label>
                        <div
                          className="border-2 border-dashed rounded-lg p-6 text-center cursor-pointer hover:bg-muted/50 transition-colors"
                          onClick={() => document.getElementById('input-curriculo-public')?.click()}
                          onDragOver={(e) => { e.preventDefault(); e.currentTarget.classList.add('bg-muted/50'); }}
                          onDragLeave={(e) => { e.currentTarget.classList.remove('bg-muted/50'); }}
                          onDrop={(e) => {
                            e.preventDefault();
                            e.currentTarget.classList.remove('bg-muted/50');
                            const f = e.dataTransfer?.files?.[0];
                            if (f && ['.pdf', '.doc', '.docx', '.png', '.jpeg', '.jpg'].some((ext) => f.name.toLowerCase().endsWith(ext)) && f.size <= MAX_FILE_CURRICULO_MB * 1024 * 1024) {
                              setFileCurriculo(f);
                              setSyncPdfError(false);
                            } else {
                              toast.error(`Arquivo inválido ou maior que ${MAX_FILE_CURRICULO_MB}MB. ${CURRICULO_LABEL}`);
                            }
                          }}
                        >
                          <input
                            id="input-curriculo-public"
                            type="file"
                            accept={ACCEPT_CURRICULO}
                            className="hidden"
                            onChange={(e) => {
                              const f = e.target.files?.[0];
                              if (f && f.size <= MAX_FILE_CURRICULO_MB * 1024 * 1024) {
                                setFileCurriculo(f);
                                setSyncPdfError(false);
                              } else if (f) toast.error(`Tamanho máximo: ${MAX_FILE_CURRICULO_MB}MB`);
                            }}
                          />
                          <Upload className="h-10 w-10 mx-auto text-muted-foreground mb-2" />
                          <p className="text-sm text-muted-foreground">
                            Clique para enviar seu currículo ou arraste-o para essa área.
                          </p>
                          <p className="text-xs text-muted-foreground mt-1">
                            {CURRICULO_LABEL}
                          </p>
                          {fileCurriculo != null ? (
                            <p className="text-sm font-medium text-primary mt-2">{fileCurriculo?.name ?? ''}</p>
                          ) : null}
                          {syncPdfError && (
                            <div className="mt-3 p-3 rounded-lg bg-destructive/10 border border-destructive/30 text-left">
                              <p className="text-sm text-destructive font-medium">
                                Não foi possível extrair os dados do PDF. Prefira o formato exportado pelo LinkedIn.
                              </p>
                              <p className="text-xs text-muted-foreground mt-1">
                                Você pode enviar outro arquivo ou avançar sem atualizar o perfil pelo currículo.
                              </p>
                              <div className="flex gap-2 mt-3">
                                <Button
                                  type="button"
                                  variant="outline"
                                  size="sm"
                                  onClick={() => {
                                  setFileCurriculo(null);
                                  setSyncPdfError(false);
                                  const input = document.getElementById('input-curriculo-public') as HTMLInputElement | null;
                                  if (input) { input.value = ''; input.click(); }
                                }}
                                >
                                  Subir novamente
                                </Button>
                                <Button
                                  type="button"
                                  size="sm"
                                  onClick={() => handleAvançarSemPdf()}
                                  disabled={submittingInscricao}
                                >
                                  {submittingInscricao && <Spinner className="mr-2 h-4 w-4" />}
                                  Avançar mesmo assim
                                </Button>
                              </div>
                            </div>
                          )}
                        </div>
                      </div>
                      <div className="flex gap-2 pt-2">
                        <Button type="button" variant="outline" className="flex-1" disabled>
                          Voltar
                        </Button>
                        <Button type="submit" className="flex-1" disabled={submittingInscricao}>
                          {submittingInscricao && <Spinner className="mr-2 h-4 w-4" />}
                          Avançar
                        </Button>
                      </div>
                    </form>
                  </Form>
                ) : (
                  <div className="rounded-lg border bg-muted/50 p-4 text-center text-sm text-muted-foreground">
                    Não foi possível carregar seus dados. Tente recarregar a página.
                  </div>
                )
              ) : (
                <div data-candidatura-step="1">
                  <StepsIndicatorPublico currentStep={1} />
                  {showCadastroForm ? (
                <Form {...cadastroForm}>
                  <form onSubmit={cadastroForm.handleSubmit(handleCadastroSubmit)} className="space-y-4">
                    <FormField
                      control={cadastroForm.control}
                      name="nomeCompleto"
                      render={({ field }) => (
                        <FormItem>
                          <FormLabel>Nome completo *</FormLabel>
                          <FormControl>
                            <Input placeholder="Seu nome completo" {...field} data-candidatura-field="nomeCompleto" />
                          </FormControl>
                          <FormMessage />
                        </FormItem>
                      )}
                    />
                    <FormField
                      control={cadastroForm.control}
                      name="cpf"
                      render={({ field }) => (
                        <FormItem>
                          <FormLabel>CPF *</FormLabel>
                          <FormControl>
                            <Input
                              placeholder="000.000.000-00"
                              maxLength={14}
                              {...field}
                              onChange={(e) => field.onChange(formatCPF(e.target.value))}
                              data-candidatura-field="cpf"
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
                            <Input type="email" placeholder="seu.email@exemplo.com" {...field} data-candidatura-field="email" />
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
                            <Input type="password" placeholder="Mínimo 6 caracteres" {...field} data-candidatura-field="senha" />
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
                            <Input type="password" placeholder="Repita a senha" {...field} data-candidatura-field="confirmaSenha" />
                          </FormControl>
                          <FormMessage />
                        </FormItem>
                      )}
                    />
                    <div className="flex items-start space-x-2" data-candidatura-field="aceita-termos">
                      <Checkbox
                        id="aceita-termos-public"
                        checked={aceitaTermos}
                        onCheckedChange={(c) => setAceitaTermos(c === true)}
                      />
                      <label htmlFor="aceita-termos-public" className="text-sm leading-none cursor-pointer">
                        Li e aceito os{' '}
                        <span className="text-primary underline">Termos de Uso e Condições</span>
                      </label>
                    </div>
                    <div className="flex gap-2">
                      <Button
                        type="button"
                        variant="outline"
                        className="flex-1"
                        onClick={() => {
                          setShowCadastroForm(false);
                          cadastroForm.reset();
                          setAceitaTermos(false);
                        }}
                        data-candidatura-action="voltar"
                      >
                        Voltar
                      </Button>
                      <Button
                        type="submit"
                        className="flex-1"
                        disabled={!aceitaTermos || isCadastrando}
                        data-testid="public-vaga-cadastro-submit-button"
                        data-candidatura-action="cadastrar"
                      >
                        {isCadastrando && <Spinner className="mr-2 h-4 w-4" />}
                        Cadastrar
                      </Button>
                    </div>
                  </form>
                </Form>
              ) : (
                <>
                  <Form {...loginForm}>
                    <form
                      onSubmit={loginForm.handleSubmit(handleLoginSubmit)}
                      className="space-y-4"
                      data-testid="public-vaga-login-form"
                    >
                      <FormField
                        control={loginForm.control}
                        name="email"
                        render={({ field }) => (
                          <FormItem>
                            <FormLabel>E-mail</FormLabel>
                            <FormControl>
                              <Input
                                type="email"
                                placeholder="seu.email@exemplo.com"
                                {...field}
                                data-testid="public-vaga-login-email"
                                data-candidatura-field="email"
                              />
                            </FormControl>
                            <FormMessage />
                          </FormItem>
                        )}
                      />
                      <Button
                        type="submit"
                        className="w-full h-11"
                        disabled={loginStatus === 'sending'}
                        data-testid="public-vaga-login-submit-button"
                        data-candidatura-action="entrar"
                      >
                        {loginStatus === 'sending' && <Spinner className="mr-2 h-4 w-4" />}
                        Entrar
                      </Button>
                    </form>
                  </Form>
                  <div className="flex items-center gap-4 my-6">
                    <div className="flex-1 h-px bg-border" />
                    <span className="text-sm text-muted-foreground">ou</span>
                    <div className="flex-1 h-px bg-border" />
                  </div>
                  <Button
                    type="button"
                    variant="outline"
                    className="w-full h-11"
                    onClick={() => {
                      setShowCadastroForm(true);
                      cadastroForm.reset();
                      setAceitaTermos(false);
                    }}
                    data-testid="public-vaga-cadastre-se-button"
                    data-candidatura-action="cadastre-se"
                  >
                    Cadastre-se
                  </Button>
                </>
                  )}
                </div>
              )}
            </CardContent>
          </Card>

          <p className="text-center text-sm text-muted-foreground mt-6">
            © 2025 FourMakers. Todos os direitos reservados.
          </p>
        </div>
      </div>

      {/* Modal token 6 dígitos */}
      <Dialog open={showTokenModal} onOpenChange={setShowTokenModal}>
        <DialogContent
          className="sm:max-w-lg top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2"
          data-testid="public-vaga-otp-modal"
        >
          {loginStatus === 'validating' && (
            <div
              className="absolute inset-0 flex items-center justify-center bg-background/80 z-50 rounded-lg"
              data-testid="public-vaga-otp-validating-overlay"
            >
              <Spinner className="h-12 w-12 text-primary" />
            </div>
          )}
          <DialogHeader className="flex flex-row items-center justify-start gap-2">
            <CheckCircle2 className="h-8 w-8 text-success" />
            <DialogTitle className="text-2xl font-bold m-0">E-mail enviado</DialogTitle>
          </DialogHeader>
          <DialogDescription className="text-base text-muted-foreground max-w-sm text-left">
            Foi enviado um e-mail com o código de acesso. Informe o código de 6 dígitos abaixo.
          </DialogDescription>
          <div className="grid gap-4 py-4">
            <div className="text-left">
              <h4 className="text-lg font-medium mb-4">Código</h4>
              <InputOTP
                maxLength={6}
                value={otpValue}
                onChange={(v) => setOtpValue(v)}
                onComplete={handleOTPComplete}
                data-testid="public-vaga-otp-input"
              >
                <InputOTPGroup className="flex justify-center w-full gap-4">
                  {[0, 1, 2, 3, 4, 5].map((i) => (
                    <InputOTPSlot key={i} index={i} className="w-16 h-16 text-2xl rounded-md border border-input" />
                  ))}
                </InputOTPGroup>
              </InputOTP>
            </div>
            <div className="text-left text-sm text-muted-foreground">
              {!isResendEnabled ? (
                <span>Não recebeu? Solicite um novo código em 00:{timeLeft < 10 ? `0${timeLeft}` : timeLeft}</span>
              ) : (
                <>
                  Está com problemas?{' '}
                  <Button variant="link" className="p-0 h-auto" onClick={handleResendCode}>
                    Reenviar código
                  </Button>
                </>
              )}
            </div>
          </div>
        </DialogContent>
      </Dialog>

      <AlertDialog open={showErrorDialog} onOpenChange={setShowErrorDialog}>
        <AlertDialogContent data-testid="public-vaga-login-error-dialog">
          <AlertDialogHeader>
            <AlertDialogTitle>Erro</AlertDialogTitle>
            <AlertDialogDescription>
              {loginError ?? 'Ocorreu um erro. Tente novamente.'}
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogAction onClick={() => setShowErrorDialog(false)}>Fechar</AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  );
}
