import { useMemo } from 'react';
import { Briefcase, Download, FileText, RefreshCw, Calendar, TrendingUp, Plus, Minus, MapPin, CheckCircle, Key, Users, Info, Eye, EyeOff, Calculator, Save, ArrowLeft, AlertTriangle } from '@/components/ui/system-icons';
import { useLocation, useNavigate } from 'react-router-dom';
import { Spinner } from '@/components/ui/spinner';
import type { SimulationData } from '@domain/entities/SimulatorTypes';
import { AccordionCard, InputField, CurrencyInput, SelectField, Tooltip, ForecastCardItem } from '@presentation/components/simulator/ui/SimulatorUI';
import { PageHeader, PageBreadcrumb } from '@presentation/components/common';
import { SimulatorVagaCandidatoSelectors } from '@presentation/components/simulator';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogDescription } from '@/components/ui/dialog';
import { Label } from '@/components/ui/label';
import { Textarea } from '@/components/ui/textarea';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Badge } from '@/components/ui/badge';
import { Switch } from '@/components/ui/switch';
import { Tooltip as UiTooltip, TooltipContent, TooltipProvider, TooltipTrigger } from '@/components/ui/tooltip';
import { useSimulator, type ViewMode } from '@presentation/hooks/useSimulator';
import { DIAS_PRESENCIAIS_OPCOES } from '@presentation/hooks/useModelosTrabalho';
import { formatCPF } from '@shared/utils/cpfUtils';
import { toast } from 'sonner';

const MSG_INFO_VAGA_EDIT = 'A informação só pode ser alterada na edição de dados da vaga.';

const FROM_KANBAN_CANDIDATOS = 'kanban-candidatos';

/** Linha do comparativo com ícone de conformidade e tooltip (Remuneração Pretendida). */
function DetalheItemComValidacao({
  label,
  valueFormatted,
  validacao,
}: {
  label: string;
  valueFormatted: string;
  validacao?: { dentroDaPolitica: boolean; mensagem: string } | null;
}) {
  const conforme = validacao?.dentroDaPolitica ?? true;
  const mensagem = validacao?.mensagem ?? '';
  const valorClasse = conforme ? 'font-medium text-foreground' : 'font-medium text-destructive';
  const labelClasse = conforme ? 'text-foreground' : 'text-destructive';

  return (
    <div className="flex justify-between items-center gap-2">
      <span className={`flex items-center gap-1.5 ${labelClasse}`}>
        {label}
        {validacao != null && (
          <TooltipProvider>
            <UiTooltip>
              <TooltipTrigger asChild>
                <span className="inline-flex flex-shrink-0" aria-label={mensagem}>
                  {conforme ? (
                    <CheckCircle className="w-4 h-4 text-success" />
                  ) : (
                    <AlertTriangle className="w-4 h-4 text-destructive" />
                  )}
                </span>
              </TooltipTrigger>
              <TooltipContent className="z-[120] max-w-xs text-xs" side="right" sideOffset={8} collisionPadding={16}>
                {mensagem}
              </TooltipContent>
            </UiTooltip>
          </TooltipProvider>
        )}
      </span>
      <span className={valorClasse}>{valueFormatted}</span>
    </div>
  );
}

const Simulator = () => {
  const location = useLocation();
  const navigate = useNavigate();
  const fromKanbanCandidatos = (location.state as { from?: string } | null)?.from === FROM_KANBAN_CANDIDATOS;

  const {
    formData,
    viewMode,
    setViewMode,
    handleInputChange,
    updateForm,
    proposedResults,
    desiredResults,
    desiredGross,
    planDependentsCount,
    planDependentsUnit,
    planDependentsTotal,
    totalEducationCost,
    updateVariableExpensesCount,
    updateVariableExpenseItem,
    handleClearForm,
    handleExportPDF,
    handleGenerateCalculation,
    handleSaveCandidaturaData,
    savingCandidaturaData,
    isCompliant,
    showDetails,
    setShowDetails,
    detailsEnabled,
    showTokenModal,
    setShowTokenModal,
    tempToken,
    setTempToken,
    handleSaveToken,
    handleSelectVaga,
    handleSelectCandidato,
    vagasList,
    candidatosList,
    loadingVagasList,
    loadingCandidatosList,
    selectedVagaId,
    selectedCandidateId,
    selectedCandidaturaId,
    candidaturaDetails,
    loadingCandidaturaDetails,
    calculationLoading,
    calculationResult,
    calculationValidations,
    jobHourlyCost,
    cepLookupStatus,
    cepFieldError,
    setChildrenCount,
    showCompanyCost,
    setShowCompanyCost,
    formatCurrency,
    formatCepForInput,
    MOBILITY_RATE_PER_KM,
    HOURS_PER_MONTH,
    MEAL_VOUCHER_BASE,
    AGE_RANGES,
    HEALTH_PLANS,
    workModels,
    loadingModelosTrabalho,
  } = useSimulator();

  const renderValidationMessage = (field: keyof typeof calculationValidations) => {
    const validation = calculationValidations[field];
    if (!validation) return null;
    const toneClass = validation.level === 'success' ? 'text-success' : 'text-destructive';
    return <p className={`mt-1 text-[11px] ${toneClass}`}>{validation.message}</p>;
  };

  const proposedFromApi = calculationResult?.retorno?.remuneracaoProposta;
  const desiredFromApi = calculationResult?.retorno?.remuneracaoPretendida;
  const proposedAjudaDeCusto = proposedFromApi?.ajudaDeCusto ?? 0;
  const desiredAjudaDeCusto = desiredFromApi?.ajudaDeCusto ?? formData.desiredAjudaDeCusto ?? 0;

  /** Somente vagas com quantidadeCandidatosPorEstagio válido (array com pelo menos 1 item) entram no combo. [] / null / ausente = não exibe. */
  const vagasParaSeletor = useMemo(
    () =>
      vagasList.filter(
        (v) => Array.isArray(v.quantidadeCandidatosPorEstagio) && v.quantidadeCandidatosPorEstagio.length > 0,
      ),
    [vagasList],
  );

  return (
    <div className="container mx-auto p-4 space-y-6 pb-40">
      <PageBreadcrumb 
        items={[
          { label: 'Simulador' }
        ]}
      />

      <PageHeader
        title="Formulário para Negociação de Remuneração e Benefícios"
        description="Conduza conversas com candidatos com mais segurança, registre dados relevantes e gere insumos para futuras negociações."
        titlePrefix={
          fromKanbanCandidatos ? (
            <Button
              variant="ghost"
              size="icon"
              className="shrink-0 rounded-pillToken h-9 w-9 text-muted-foreground hover:text-foreground"
              onClick={() => navigate(-1)}
              aria-label="Voltar à página anterior"
            >
              <ArrowLeft className="h-5 w-5" aria-hidden />
            </Button>
          ) : undefined
        }
        actions={
          <div className="flex flex-wrap gap-2">
            <Select value={viewMode} onValueChange={(value) => setViewMode(value as ViewMode)}>
              <SelectTrigger className="w-[180px]">
                <div className="flex items-center gap-2">
                  {viewMode === 'recruiter' ? <Briefcase className="w-4 h-4" /> : <Users className="w-4 h-4" />}
                  <SelectValue />
                </div>
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="form">Formulário</SelectItem>
                <SelectItem value="recruiter">Visão Recrutador</SelectItem>
                <SelectItem value="collaborator">Visão Colaborador</SelectItem>
              </SelectContent>
            </Select>

            <Button variant="ghost" onClick={handleClearForm}>
              <RefreshCw className="w-4 h-4 mr-2" />
              Limpar
            </Button>
          </div>
        }
      />

      <SimulatorVagaCandidatoSelectors
        vagas={vagasParaSeletor}
        candidatos={candidatosList}
        selectedVagaId={selectedVagaId}
        selectedCandidaturaId={selectedCandidaturaId}
        loadingVagas={loadingVagasList}
        loadingCandidatos={loadingCandidatosList}
        loadingCandidaturaDetails={loadingCandidaturaDetails}
        candidateNameFromDetails={
          selectedCandidateId && candidaturaDetails?.colaborador?.codigoInternoColaborador === selectedCandidateId
            ? candidaturaDetails?.colaborador?.nomeCompleto ?? null
            : null
        }
        selectedCandidateId={selectedCandidateId || undefined}
        onSelectVaga={handleSelectVaga}
        onSelectCandidato={handleSelectCandidato}
      />

      {/* Accordion Sections */}
      <AccordionCard title="Informações da Vaga">
        <p className="text-sm text-muted-foreground mb-4">{MSG_INFO_VAGA_EDIT}</p>
        <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
          <div className="md:col-span-1">
            <InputField
              label="Perfil de Atuação"
              value={formData.role}
              readOnly
              onClick={() => toast.info(MSG_INFO_VAGA_EDIT)}
              className="cursor-default"
            />
          </div>
          <div className="md:col-span-1">
            <InputField
              label="Gestor"
              value={formData.manager}
              readOnly
              onClick={() => toast.info(MSG_INFO_VAGA_EDIT)}
              className="cursor-default"
            />
          </div>
          <div className="md:col-span-1">
            <InputField
              label="Cliente"
              value={formData.client}
              readOnly
              onClick={() => toast.info(MSG_INFO_VAGA_EDIT)}
              className="cursor-default"
            />
          </div>
          <div className="md:col-span-1">
            <InputField
              label="Data de Início"
              type="date"
              value={formData.startDate}
              onChange={(e) => handleInputChange('startDate', e.target.value)}
              placeholder="yyyy-mm-dd"
              title="Editável apenas para simulação da previsão de recebimentos (13 meses)"
            />
          </div>
          <div className="md:col-span-1">
            <InputField
              label="CEP do Trabalho"
              value={formData.workZipCode}
              readOnly
              onClick={() => toast.info(MSG_INFO_VAGA_EDIT)}
              className="cursor-default"
              placeholder="00000-000"
            />
          </div>
          <div className="md:col-span-1">
            <CurrencyInput
              label="Custo Hora Vaga"
              value={jobHourlyCost}
              onChange={() => {}}
              readOnly
              readOnlyLookEnabled
              onFocus={() => toast.info(MSG_INFO_VAGA_EDIT)}
              onClick={() => toast.info(MSG_INFO_VAGA_EDIT)}
            />
          </div>
          <div className="md:col-span-2">
            <CurrencyInput
              label={`Remuneração Vaga (${HOURS_PER_MONTH}h)`}
              value={jobHourlyCost * HOURS_PER_MONTH}
              onChange={() => {}}
              readOnly
              readOnlyLookEnabled
              onFocus={() => toast.info(MSG_INFO_VAGA_EDIT)}
              onClick={() => toast.info(MSG_INFO_VAGA_EDIT)}
            />
          </div>
          <div className="md:col-span-2 flex flex-col md:flex-row md:flex-wrap gap-4">
            <div className="space-y-2 min-w-0 md:flex-1 md:min-w-[160px]">
              <SelectField
                label="Modelo de Trabalho"
                options={loadingModelosTrabalho ? [] : workModels.map((m) => ({ label: m.descricao, value: m.id }))}
                value={formData.modeloTrabalhoId || ''}
                onChange={() => {}}
                readOnly
                onReadOnlyClick={() => toast.info(MSG_INFO_VAGA_EDIT)}
              />
            </div>
            {(() => {
              const modelo = workModels.find((m) => m.id === formData.modeloTrabalhoId);
              const is100Remoto = modelo?.descricao === '100% Remoto' || modelo?.codigo === 3;
              return !is100Remoto && modelo ? (
                <div className="space-y-2 min-w-0 md:flex-1 md:min-w-[160px]">
                  <SelectField
                    label="Dias presenciais"
                    options={DIAS_PRESENCIAIS_OPCOES}
                    value={formData.workOnsiteDays || ''}
                    onChange={() => {}}
                    readOnly
                    onReadOnlyClick={() => toast.info(MSG_INFO_VAGA_EDIT)}
                  />
                </div>
              ) : null;
            })()}
            <div className="flex flex-col gap-2 justify-end min-w-0 md:flex-shrink-0">
              <Label className="text-sm font-medium text-foreground">Cargo de Confiança</Label>
              <div className="flex items-center h-10">
                <Switch
                  checked={formData.isTrustPosition}
                  onCheckedChange={() => toast.info(MSG_INFO_VAGA_EDIT)}
                />
              </div>
            </div>
          </div>
        </div>
      </AccordionCard>

      <AccordionCard
        title="Dados do Candidato"
        description="Coletamos dados pessoais exclusivamente para identificação do candidato, comunicação durante o processo seletivo e cumprimento de obrigações legais, garantindo rastreabilidade, segurança e conformidade com a LGPD."
      >
        <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
          <div className="md:col-span-2">
            <InputField label="Nome do Candidato" value={formData.candidateName} onChange={(e) => handleInputChange('candidateName', e.target.value)} placeholder="Nome completo" />
          </div>
          <InputField
            label="CPF"
            value={formatCPF(formData.cpf)}
            onChange={(e) => {
              const raw = e.target.value.replace(/\D/g, '').slice(0, 11);
              handleInputChange('cpf', raw);
            }}
            placeholder="000.000.000-00"
          />
          <InputField
            label="RG"
            value={formData.rg}
            onChange={(e) => handleInputChange('rg', e.target.value)}
            placeholder="00.000.000-0"
          />
          <InputField
            label="Telefone"
            value={formData.phone}
            onChange={(e) => handleInputChange('phone', e.target.value)}
            placeholder="(00) 00000-0000"
          />
          <InputField label="Data de Nascimento" type="date" value={formData.birthDate} onChange={(e) => handleInputChange('birthDate', e.target.value)} />
          <div className="md:col-span-2">
            <InputField
              label="E-mail pessoal"
              type="email"
              value={formData.personalEmail}
              onChange={(e) => handleInputChange('personalEmail', e.target.value)}
              placeholder="nome@email.com"
            />
          </div>
          <div className="flex flex-col gap-1">
            <InputField
              label="CEP"
              value={formData.zipCode}
              onChange={(e) => handleInputChange('zipCode', formatCepForInput(e.target.value))}
              maxLength={9}
              inputMode="numeric"
              placeholder="00000-000"
              error={!!cepFieldError}
            />
            {cepFieldError ? (
              <span className="text-[11px] text-destructive">{cepFieldError}</span>
            ) : cepLookupStatus === 'loading' ? (
              <span className="text-[11px] text-muted-foreground">Buscando endereço pelo CEP…</span>
            ) : cepLookupStatus === 'error' ? (
              <span className="text-[11px] text-destructive">Não foi possível localizar o CEP. Verifique e tente novamente.</span>
            ) : null}
          </div>
          <div className="md:col-span-2">
            <InputField label="Endereço" value={formData.address} onChange={(e) => handleInputChange('address', e.target.value)} placeholder="Rua, Avenida..." />
          </div>
          <InputField label="Número" value={formData.addressNumber} onChange={(e) => handleInputChange('addressNumber', e.target.value)} placeholder="Nº" />
          <InputField label="Complemento" value={formData.addressComplement} onChange={(e) => handleInputChange('addressComplement', e.target.value)} placeholder="Apto, Bloco..." />
          <InputField label="Cidade" value={formData.city} onChange={(e) => handleInputChange('city', e.target.value)} placeholder="Cidade" />
          <InputField label="Estado" value={formData.state} onChange={(e) => handleInputChange('state', e.target.value)} placeholder="UF" />
        </div>
      </AccordionCard>

      <AccordionCard
        title="Preferências do Candidato"
        description="Mapeie expectativas de remuneração e formato de trabalho para negociar com clareza."
      >
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <CurrencyInput
            label="Pretensão Líquida (Ref.)"
            value={formData.desiredNetSalary}
            onChange={(val) => handleInputChange('desiredNetSalary', val)}
          />
        </div>
      </AccordionCard>

      <AccordionCard
        title="Inclusão e Acessibilidade"
        description="Esta pergunta nos ajuda a garantir um processo seletivo mais acessível e inclusivo, oferecendo adaptações, recursos e benefícios previstos em lei. A resposta não influencia decisões de contratação."
      >
        <div className="space-y-4">
          <div className="space-y-2">
            <Label className="text-sm font-medium text-foreground">Você se identifica como pessoa com deficiência (PCD)?</Label>
            <div className="grid grid-cols-1 gap-2 sm:grid-cols-3">
              {[
                { label: "Sim", value: "yes" as const },
                { label: "Não", value: "no" as const },
                { label: "Prefiro não informar", value: "prefer_not" as const },
              ].map((opt) => (
                <label
                  key={opt.value}
                  className="flex items-center gap-2 rounded-lg border border-borderSoft bg-surfaceElevated px-3 py-2 text-sm text-primaryText cursor-pointer hover:bg-surfaceSubtle"
                >
                  <input
                    type="radio"
                    name="pcdAnswer"
                    value={opt.value}
                    checked={formData.pcdAnswer === opt.value}
                    onChange={() => {
                      handleInputChange("pcdAnswer", opt.value);
                      if (opt.value !== "yes") handleInputChange("pcdNotes", "");
                    }}
                  />
                  <span>{opt.label}</span>
                </label>
              ))}
            </div>
          </div>

          {formData.pcdAnswer === "yes" ? (
            <div className="flex flex-col gap-2">
              <Label className="text-sm font-medium text-foreground">
                Deseja informar se há alguma adaptação ou recurso que possamos considerar durante o processo ou no trabalho?
              </Label>
              <Textarea
                value={formData.pcdNotes}
                onChange={(e) => handleInputChange("pcdNotes", e.target.value)}
                placeholder="Descreva aqui, se desejar"
                rows={4}
                className="min-h-[120px]"
              />
            </div>
          ) : null}
        </div>
      </AccordionCard>

      <AccordionCard
        title="Mobilidade"
        description="Mobilidade é o valor pago pelo deslocamento estimado entre sua casa e o local de trabalho, ida e volta. Esse dado garante uma proposta alinhada à sua rotina."
      >
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          <div className="flex flex-col gap-2 w-full">
            <Label className={calculationValidations.mobilidade?.level === 'error' ? 'text-destructive' : ''}>Distância (Ida/Volta)</Label>
            <div className={`relative flex items-center rounded-lg border bg-input px-3 h-10 ${calculationValidations.mobilidade?.level === 'error' ? 'border-destructive' : 'border-input'}`}>
              <MapPin className="w-4 h-4 text-muted-foreground mr-2" />
              <input 
                type="number" 
                min="0" 
                className="w-full bg-transparent border-none focus:ring-0 outline-none text-foreground text-sm [appearance:textfield] [&::-webkit-outer-spin-button]:appearance-none [&::-webkit-inner-spin-button]:appearance-none"
                value={formData.dailyKm || ''} 
                onChange={(e) => handleInputChange('dailyKm', parseFloat(e.target.value) || 0)} 
                placeholder="0" 
                aria-invalid={calculationValidations.mobilidade?.level === 'error'}
              />
              <span className="text-muted-foreground text-sm ml-2">km</span>
            </div>
          </div>
            <div className="flex flex-col gap-2 w-full">
              <Label>Custo Mobilidade (Mês)</Label>
              <div className="p-2.5 bg-info/10 dark:bg-info/20 border border-info/30 rounded-lg text-info font-bold text-sm h-10 flex items-center">
                {formatCurrency(proposedResults.mobilityCost)}
              </div>
              <p className="text-[11px] text-muted-foreground">
                Valor pago por km: {formatCurrency(MOBILITY_RATE_PER_KM)}
              </p>
              {renderValidationMessage('mobilidade')}
            </div>
        </div>
      </AccordionCard>

      <AccordionCard
        title="Família & Dependentes"
        description="Queremos conhecer melhor sua realidade familiar. Essas informações nos ajudam a oferecer benefícios que façam sentido para você e sua família."
      >
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <Card>
            <CardContent className="p-4 flex items-center justify-between">
              <div className="flex items-center gap-2">
                <span className="text-sm font-medium text-foreground">Mora com quantas pessoas?</span>
              </div>
              <div className="inline-flex items-center gap-2 rounded-pillToken border border-borderSoft bg-surfaceSubtle px-md py-2xs">
                <Button
                  variant="ghost"
                  size="icon"
                  className="h-8 w-8"
                  onClick={() => handleInputChange('residentsCount', Math.max(0, (formData.residentsCount ?? 0) - 1))}
                  aria-label="Diminuir quantidade de residentes"
                >
                  −
                </Button>
                <span className="min-w-[32px] text-center font-semibold text-primaryText">
                  {formData.residentsCount ?? 0}
                </span>
                <Button
                  variant="ghost"
                  size="icon"
                  className="h-8 w-8"
                  onClick={() => handleInputChange('residentsCount', (formData.residentsCount ?? 0) + 1)}
                  aria-label="Aumentar quantidade de residentes"
                >
                  +
                </Button>
              </div>
            </CardContent>
          </Card>

          <Card>
            <CardContent className="p-4 flex items-center justify-between">
              <div className="flex items-center gap-2">
                <span className="text-sm font-medium text-foreground">Dependentes IRRF</span>
                <Tooltip text={`Cada dependente deduz ${formatCurrency(189.59)} da base de cálculo.`} />
              </div>
              <div className="inline-flex items-center gap-2 rounded-pillToken border border-borderSoft bg-surfaceSubtle px-md py-2xs">
                <Button
                  variant="ghost"
                  size="icon"
                  className="h-8 w-8"
                  onClick={() => handleInputChange('dependentsIRPF', Math.max(0, formData.dependentsIRPF - 1))}
                  aria-label="Diminuir dependentes IRRF"
                >
                  −
                </Button>
                <span className="min-w-[32px] text-center font-semibold text-primaryText">
                  {formData.dependentsIRPF}
                </span>
                <Button
                  variant="ghost"
                  size="icon"
                  className="h-8 w-8"
                  onClick={() => handleInputChange('dependentsIRPF', formData.dependentsIRPF + 1)}
                  aria-label="Aumentar dependentes IRRF"
                >
                  +
                </Button>
              </div>
            </CardContent>
          </Card>
        </div>

        <div className="mt-4 space-y-4">
          <Card>
            <CardContent className="p-4 space-y-4">
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div className="flex items-center justify-between gap-4">
                  <span className="text-sm font-medium text-foreground">Possui cônjuge / companheiro(a)?</span>
                  <Switch
                    checked={formData.hasSpouse}
                    onCheckedChange={(checked) => {
                      handleInputChange('hasSpouse', checked);
                      if (!checked) handleInputChange('spouseBirthDate', '');
                    }}
                  />
                </div>
                {formData.hasSpouse ? (
                  <InputField
                    label="Data de Nascimento do Cônjuge"
                    type="date"
                    value={formData.spouseBirthDate}
                    onChange={(e) => handleInputChange('spouseBirthDate', e.target.value)}
                  />
                ) : (
                  <div />
                )}
              </div>

              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div className="flex items-center justify-between gap-4">
                  <span className="text-sm font-medium text-foreground">Possui filhos?</span>
                  <div className="flex items-center gap-3">
                    {formData.hasChildren ? (
                      <div className="inline-flex items-center gap-2 rounded-pillToken border border-borderSoft bg-surfaceSubtle px-md py-2xs">
                        <Button
                          variant="ghost"
                          size="icon"
                          className="h-8 w-8"
                          onClick={() => setChildrenCount(Math.max(1, (formData.childrenCount || 1) - 1))}
                          aria-label="Diminuir quantidade de filhos"
                        >
                          −
                        </Button>
                        <span className="min-w-[32px] text-center font-semibold text-primaryText">
                          {formData.childrenCount || 1}
                        </span>
                        <Button
                          variant="ghost"
                          size="icon"
                          className="h-8 w-8"
                          onClick={() => setChildrenCount((formData.childrenCount || 1) + 1)}
                          aria-label="Aumentar quantidade de filhos"
                        >
                          +
                        </Button>
                      </div>
                    ) : null}
                    <Switch
                      checked={formData.hasChildren}
                      onCheckedChange={(checked) => {
                        handleInputChange('hasChildren', checked);
                        if (checked) {
                          if (!formData.childrenCount || formData.childrenCount < 1) setChildrenCount(1);
                        } else {
                          updateForm({ childrenCount: 0, childrenBirthDates: [] });
                        }
                      }}
                    />
                  </div>
                </div>
                <div />
              </div>

              {formData.hasChildren ? (
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  {Array.from({ length: formData.childrenCount || 0 }).map((_, index) => (
                    <InputField
                      key={index}
                      label={`Data de Nascimento Filho ${index + 1}`}
                      type="date"
                      value={formData.childrenBirthDates?.[index] || ''}
                      onChange={(e) => {
                        const next = [...(formData.childrenBirthDates || [])];
                        next[index] = e.target.value;
                        updateForm({ childrenBirthDates: next });
                      }}
                    />
                  ))}
                </div>
              ) : null}
            </CardContent>
          </Card>
        </div>
      </AccordionCard>

       <AccordionCard
         title="Seguro Saúde"
         description="Queremos entender se você ou seus filhos até 24 anos estão estudando para verificar se existe a possibilidade de incluir auxílio educação no pacote de benefícios, de acordo com as políticas vigentes."
       >
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
              <div className="md:col-span-3">
                <Card>
                  <CardContent className="p-4 flex items-center justify-between gap-4">
                    <div className="flex flex-col">
                      <span className="text-sm font-medium text-foreground">Possui seguro saúde?</span>
                      <span className="text-xs text-muted-foreground">Informe os dados atuais para comparação e entendimento de cobertura.</span>
                    </div>
                    <Switch
                      checked={formData.hasHealthInsurance}
                      onCheckedChange={(checked) => {
                        handleInputChange('hasHealthInsurance', checked);
                        if (!checked) {
                          updateForm({
                            healthInsuranceCurrentValue: 0,
                            healthInsuranceProvider: '',
                            healthInsuranceAccommodation: '',
                            healthInsuranceHasCopay: false,
                            healthInsuranceNotes: '',
                          });
                        }
                      }}
                    />
                  </CardContent>
                </Card>
              </div>

              {formData.hasHealthInsurance ? (
                <>
                  <CurrencyInput
                    label="Valor Atual (R$)"
                    value={formData.healthInsuranceCurrentValue}
                    onChange={(val) => handleInputChange('healthInsuranceCurrentValue', val)}
                  />
                  <SelectField
                    label="Operadora"
                    options={[
                      { label: 'Amil', value: 'Amil' },
                      { label: 'Bradesco Saúde', value: 'Bradesco Saúde' },
                      { label: 'SulAmérica', value: 'SulAmérica' },
                      { label: 'Unimed', value: 'Unimed' },
                      { label: 'Porto Seguro Saúde', value: 'Porto Seguro Saúde' },
                      { label: 'NotreDame Intermédica', value: 'NotreDame Intermédica' },
                      { label: 'Hapvida', value: 'Hapvida' },
                      { label: 'São Cristóvão Saúde', value: 'São Cristóvão Saúde' },
                      { label: 'Prevent Senior', value: 'Prevent Senior' },
                      { label: 'Alice', value: 'Alice' },
                      { label: 'Omint', value: 'Omint' },
                      { label: 'Care Plus', value: 'Care Plus' },
                      { label: 'Seguros Unimed', value: 'Seguros Unimed' },
                      { label: 'Outra', value: 'Outra' },
                    ]}
                    value={formData.healthInsuranceProvider}
                    onChange={(e) => handleInputChange('healthInsuranceProvider', e.target.value)}
                  />
                  <SelectField
                    label="Acomodação"
                    options={[
                      { label: 'Apartamento', value: 'Apartamento' },
                      { label: 'Enfermaria', value: 'Enfermaria' },
                    ]}
                    value={formData.healthInsuranceAccommodation}
                    onChange={(e) => handleInputChange('healthInsuranceAccommodation', e.target.value as SimulationData['healthInsuranceAccommodation'])}
                  />

                  <div className="md:col-span-3">
                    <Card>
                      <CardContent className="p-4 flex items-center justify-between gap-4">
                        <div className="flex flex-col">
                          <span className="text-sm font-medium text-foreground">Possui coparticipação?</span>
                          <span className="text-xs text-muted-foreground">Marque caso exista cobrança por uso (consultas/exames).</span>
                        </div>
                        <Switch
                          checked={formData.healthInsuranceHasCopay}
                          onCheckedChange={(checked) => handleInputChange('healthInsuranceHasCopay', checked)}
                        />
                      </CardContent>
                    </Card>
                  </div>

                  <div className="md:col-span-3 flex flex-col gap-2 w-full">
                    <Label className="text-sm font-medium text-foreground">Observações</Label>
                    <Textarea
                      value={formData.healthInsuranceNotes}
                      onChange={(e) => handleInputChange('healthInsuranceNotes', e.target.value)}
                      placeholder="Detalhes sobre o plano"
                      rows={4}
                      className="min-h-[120px]"
                    />
                  </div>
                </>
              ) : null}

              <div className="md:col-span-3">
                <Card>
                  <CardContent className="p-4 flex items-center justify-between gap-4">
                    <div className="flex flex-col">
                      <span className="text-sm font-medium text-foreground">Possui interesse no plano Foursys?</span>
                      <span className="text-xs text-info mt-1">
                        O seu plano de saúde é custeado pela empresa.
                      </span>
                    </div>
                    <Switch
                      checked={formData.wantsFoursysPlan}
                      onCheckedChange={(checked) => {
                        handleInputChange('wantsFoursysPlan', checked);
                        if (!checked) {
                          updateForm({
                            wantsFoursysPlanIncludeDependents: false,
                            wantsFoursysPlanDependentsCount: 0,
                          });
                        } else {
                          if (formData.wantsFoursysPlanIncludeDependents && (!formData.wantsFoursysPlanDependentsCount || formData.wantsFoursysPlanDependentsCount < 1)) {
                            updateForm({ wantsFoursysPlanDependentsCount: 1 });
                          }
                        }
                      }}
                    />
                  </CardContent>
                </Card>
              </div>

              {formData.wantsFoursysPlan ? (
                <>
                  {formData.birthDate ? (
                    <SelectField
                      label="Faixa Etária"
                      options={AGE_RANGES}
                      value={formData.ageRange}
                      onChange={() => {}}
                      disabled
                    />
                  ) : (
                    <div className="flex flex-col gap-2 w-full">
                      <span className="text-sm font-medium text-foreground">Faixa Etária</span>
                      <p className="text-sm text-muted-foreground">Informe a data de nascimento do candidato.</p>
                    </div>
                  )}
                  <SelectField 
                    label="Categoria do Plano" 
                    options={[
                      { label: HEALTH_PLANS.general.label, value: 'general' }, 
                      { label: HEALTH_PLANS.supervisor.label, value: 'supervisor' }, 
                      { label: HEALTH_PLANS.executive.label, value: 'executive' }
                    ]}
                    value={formData.healthPlanRole} 
                    onChange={(e) => handleInputChange('healthPlanRole', e.target.value)} 
                  />
                  <CurrencyInput 
                    label="Valor do Plano Candidato" 
                    value={formData.healthPlanValue} 
                    onChange={() => {}} 
                    disabled 
                    inputClassName="bg-muted text-muted-foreground" 
                  />

                  <div className="md:col-span-3">
                    <Card>
                  <CardContent className="p-4 flex items-center justify-between gap-4">
                    <div className="flex flex-col">
                      <span className="text-sm font-medium text-foreground">Incluir dependentes</span>
                      <span className="text-xs text-info mt-1">
                        O valor dos dependentes será descontado em folha.
                      </span>
                    </div>
                    <Switch
                      checked={formData.wantsFoursysPlanIncludeDependents}
                          onCheckedChange={(checked) => {
                            handleInputChange('wantsFoursysPlanIncludeDependents', checked);
                            if (checked) {
                              if (!formData.wantsFoursysPlanDependentsCount || formData.wantsFoursysPlanDependentsCount < 1) {
                                updateForm({ wantsFoursysPlanDependentsCount: 1 });
                              }
                            } else {
                              updateForm({ wantsFoursysPlanDependentsCount: 0 });
                            }
                          }}
                        />
                      </CardContent>
                    </Card>
                  </div>

                  {formData.wantsFoursysPlanIncludeDependents ? (
                    <>
                      <div className="md:col-span-1">
                        <Label className="text-sm font-medium text-foreground">Quantidade de Dependentes</Label>
                        <div className="inline-flex items-center gap-2 rounded-pillToken border border-borderSoft bg-surfaceSubtle px-md py-2xs mt-2">
                          <Button
                            variant="ghost"
                            size="icon"
                            className="h-8 w-8"
                            onClick={() =>
                              updateForm({
                                wantsFoursysPlanDependentsCount: Math.max(
                                  1,
                                  (formData.wantsFoursysPlanDependentsCount || 1) - 1,
                                ),
                              })
                            }
                            aria-label="Diminuir dependentes do plano"
                          >
                            −
                          </Button>
                          <span className="min-w-[32px] text-center font-semibold text-primaryText">
                            {formData.wantsFoursysPlanDependentsCount || 1}
                          </span>
                          <Button
                            variant="ghost"
                            size="icon"
                            className="h-8 w-8"
                            onClick={() =>
                              updateForm({
                                wantsFoursysPlanDependentsCount: (formData.wantsFoursysPlanDependentsCount || 1) + 1,
                              })
                            }
                            aria-label="Aumentar dependentes do plano"
                          >
                            +
                          </Button>
                        </div>
                      </div>
                      <div className="md:col-span-2">
                        <CurrencyInput
                          label="Valor do Plano Dependentes"
                          value={(formData.healthPlanValue || 0) * (formData.wantsFoursysPlanDependentsCount || 1)}
                          onChange={() => {}}
                          disabled
                          inputClassName="bg-muted text-muted-foreground"
                        />
                      </div>
                    </>
                  ) : null}
                </>
              ) : null}
          </div>
      </AccordionCard>

      <AccordionCard
        title="Alimentação"
        description="O cartão refeição é igual para todo mundo (R$ 660,00). Já o cartão alimentação a gente ajusta para fazer sentido no seu dia a dia. Essa pergunta nos ajuda a oferecer algo que faça mais sentido para você."
      >
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div className="space-y-1">
              <CurrencyInput
                label="Cartão Refeição"
                value={MEAL_VOUCHER_BASE}
                onChange={() => {}}
                disabled
                inputClassName="bg-muted text-muted-foreground"
                error={calculationValidations.valeRefeicao?.level === 'error'}
              />
              {renderValidationMessage('valeRefeicao')}
            </div>
            <div className="space-y-1">
              <CurrencyInput
                label="Cartão Alimentação"
                value={formData.foodAllowance}
                onChange={(val) => handleInputChange('foodAllowance', val)}
                error={calculationValidations.valeAlimentacao?.level === 'error'}
              />
              {renderValidationMessage('valeAlimentacao')}
            </div>
          </div>
          <Card className="mt-4">
            <CardContent className="p-4 flex items-center justify-between">
              <div className="flex flex-col">
                <span className="text-sm font-medium text-foreground">Total do Benefício</span>
                <span className="text-xs text-muted-foreground">Benefício Refeição + Benefício Alimentação</span>
              </div>
              <span className="text-lg font-bold text-primary">{formatCurrency(MEAL_VOUCHER_BASE + (formData.foodAllowance || 0))}</span>
            </CardContent>
          </Card>
      </AccordionCard>

      <AccordionCard
        title="Educação"
        description="Queremos entender se você ou seus filhos até 24 anos estão estudando para verificar se existe a possibilidade de incluir auxílio educação no pacote de benefícios, de acordo com as políticas vigentes."
      >
        <div className="grid grid-cols-1 gap-4">
          <Card>
            <CardContent className="p-4 space-y-4">
              <div className="flex items-center justify-between gap-4">
                <span className="text-sm font-medium text-foreground">Estuda atualmente?</span>
                <Switch
                  checked={formData.studiesCurrently}
                  onCheckedChange={(checked) => {
                    handleInputChange('studiesCurrently', checked);
                    if (!checked) handleInputChange('educationMonthlyCost', 0);
                  }}
                />
              </div>
                  {formData.studiesCurrently ? (
                    <CurrencyInput
                      label="Custo Mensal com Educação (R$)"
                      value={formData.educationMonthlyCost}
                      onChange={(val) => handleInputChange('educationMonthlyCost', val)}
                    />
                  ) : null}
            </CardContent>
          </Card>

          <Card>
            <CardContent className="p-4 space-y-4">
              <div className="flex items-center justify-between gap-4">
                <span className="text-sm font-medium text-foreground">Filhos (até 24 anos) estudam?</span>
                <Switch
                  checked={formData.childrenStudyUpTo24}
                  onCheckedChange={(checked) => {
                    handleInputChange('childrenStudyUpTo24', checked);
                    if (!checked) handleInputChange('childrenEducationMonthlyCost', 0);
                  }}
                />
              </div>
              {formData.childrenStudyUpTo24 ? (
                <CurrencyInput
                  label="Custo Mensal com Educação dos Filhos (R$)"
                  value={formData.childrenEducationMonthlyCost}
                  onChange={(val) => handleInputChange('childrenEducationMonthlyCost', val)}
                />
              ) : null}
            </CardContent>
          </Card>

        {formData.studiesCurrently || formData.childrenStudyUpTo24 ? (
          <div className="space-y-1">
            <CurrencyInput
              label="Custo Total Educação (R$)"
              value={totalEducationCost}
              onChange={() => {}}
              disabled
              inputClassName="bg-muted text-muted-foreground"
              error={calculationValidations.auxilioEducacao?.level === 'error'}
            />
            {renderValidationMessage('auxilioEducacao')}
          </div>
        ) : null}
      </div>
    </AccordionCard>

      {viewMode === 'collaborator' && (
      <AccordionCard title="Gastos Variáveis">
          <div className="space-y-4">
              <Card className="bg-destructive/10 dark:bg-destructive/20 border-destructive/30">
                <CardContent className="p-3">
                  <div className="flex items-start gap-2 text-sm text-destructive">
                    <Info className="w-4 h-4 mt-0.5 flex-shrink-0" />
                    <span>Estes valores serão <strong>descontados</strong> da Remuneração Líquida (Ex: Assinaturas, Coparticipações).</span>
                  </div>
                </CardContent>
              </Card>
              <div className="flex flex-col gap-2 w-full max-w-[200px]">
                  <Label>Quantidade</Label>
                  <div className="flex items-center bg-secondary rounded-lg border border-input w-full h-10">
                       <Button 
                         variant="ghost" 
                         size="icon" 
                         className="h-10 w-10"
                         onClick={() => updateVariableExpensesCount(false)}
                       >
                         <Minus className="w-4 h-4" />
                       </Button>
                      <div className="flex-1 text-center font-semibold text-foreground">{formData.variableExpenses.length}</div>
                      <Button 
                        variant="ghost" 
                        size="icon" 
                        className="h-10 w-10"
                        onClick={() => updateVariableExpensesCount(true)}
                      >
                        <Plus className="w-4 h-4" />
                      </Button>
                  </div>
              </div>
              <div className="space-y-2">
                  {formData.variableExpenses.map((expense, index) => (
                      <div key={expense.id} className="grid grid-cols-1 md:grid-cols-2 gap-4 items-end">
                          <InputField label="Descrição" placeholder="Ex: Assinatura IA, Home Office..." value={expense.description} onChange={(e) => updateVariableExpenseItem(index, 'description', e.target.value)} />
                          <CurrencyInput 
                            label="Valor a Descontar" 
                            placeholder="0,00" 
                            value={expense.value} 
                            onChange={(val) => updateVariableExpenseItem(index, 'value', val)} 
                            inputClassName="text-destructive" 
                          />
                      </div>
                  ))}
                   {formData.variableExpenses.length === 0 && (
                     <p className="text-sm text-muted-foreground italic p-2">Nenhum gasto variável adicionado.</p>
                   )}
              </div>
          </div>
      </AccordionCard>
      )}
      
      {viewMode !== 'form' && (
        <>
          <Card className="border border-info/20 bg-surfaceElevated shadow-softToken">
            <CardContent className="p-4 text-sm text-primaryText">
              <p className="font-semibold text-info">A informação não garante o benefício.</p>
            </CardContent>
          </Card>
          <Card className="mb-24">
           <CardHeader className="bg-warning/10 dark:bg-warning/20 border-b border-warning/30">
              <CardTitle className="flex items-center gap-2 text-yellow-900 dark:text-yellow-100">
                <Calendar className="w-5 h-5" /> 
                Previsão de Recebimentos (13 Meses)
              </CardTitle>
           </CardHeader>
           <CardContent className="p-6">
             {proposedResults.forecast.length === 0 ? (
               <p className="text-sm text-muted-foreground py-4">
                 Defina a <strong>Data de Início</strong> na seção Informações da Vaga para simular a previsão dos 13 meses a partir dessa data.
               </p>
             ) : (
             <div className="grid grid-cols-1 gap-4">
               {proposedResults.forecast.map((month, idx) => (
                  <ForecastCardItem 
                    key={idx} 
                    month={month} 
                    index={idx} 
                    formatCurrency={formatCurrency} 
                    vacationModel={formData.vacationModel} 
                    onModelChange={(model) => handleInputChange('vacationModel', model)} 
                    forceExpanded={viewMode === 'collaborator'} 
                  />
               ))}
             </div>
             )}
           </CardContent>
          </Card>
        </>
      )}

      {/* Summary Footer */}
      {viewMode !== 'form' && (
        <div
          className="fixed bottom-0 right-0 z-40 bg-background border-t border-border"
          style={{ left: "var(--sidebar-offset, 0px)" }}
        >
          <div className="container mx-auto p-4">
            <div className="flex flex-col lg:flex-row justify-between items-center gap-6">
              <div className="grid grid-cols-2 sm:grid-cols-4 gap-4 flex-1 w-full">
                <div className="flex h-full flex-col justify-between border-l-4 border-primary pl-3">
                  <span className="text-xs text-muted-foreground uppercase tracking-wide">Líquido em Conta</span>
                  <span className={`text-xs font-bold leading-tight ${proposedResults.netSalary < 0 ? 'text-destructive' : 'text-primary'}`}>
                    {formatCurrency(proposedResults.netSalary)}
                  </span>
                </div>
                <div className="flex h-full flex-col justify-between border-l-4 border-info pl-3">
                  <span className="text-xs text-muted-foreground uppercase tracking-wide">Descontos</span>
                  <span className="text-xs font-bold leading-tight text-info">
                    {formatCurrency(proposedResults.totalDeductions)}
                  </span>
                </div>
                <div className="flex h-full flex-col justify-between border-l-4 border-info pl-3">
                  <span className="text-xs text-muted-foreground uppercase tracking-wide">Benefícios</span>
                  <span className="text-xs font-bold leading-tight text-info">
                    {formatCurrency(proposedResults.monthlyBenefits)}
                  </span>
                </div>
                <div className="flex h-full flex-col justify-between border-l-4 border-success pl-3">
                  <span className="text-xs text-muted-foreground uppercase tracking-wide">Ganho Real</span>
                  <span className="text-xs font-bold leading-tight text-success">
                    {formatCurrency(proposedResults.netSalary + proposedResults.monthlyBenefits)}
                  </span>
                </div>
              </div>

              <div className="flex flex-col gap-2 w-full lg:w-auto">
                <div className="flex gap-2">
                  <Button
                    variant="secondary"
                    onClick={handleSaveCandidaturaData}
                    disabled={!selectedCandidateId || !selectedCandidaturaId || savingCandidaturaData}
                    className="flex-1 lg:flex-none"
                  >
                    {savingCandidaturaData ? (
                      <Spinner className="mr-2 h-4 w-4" />
                    ) : (
                      <Save className="w-4 h-4 mr-2" />
                    )}
                    Salvar Dados
                  </Button>
                  <Button
                    onClick={handleGenerateCalculation}
                    disabled={calculationLoading}
                    className="flex-1 lg:flex-none"
                  >
                    {calculationLoading ? (
                      <Spinner className="mr-2 h-4 w-4" />
                    ) : (
                      <Calculator className="w-4 h-4 mr-2" />
                    )}
                    Gerar Cálculo
                  </Button>
                  <Button
                    variant="ghost"
                    onClick={() => setShowDetails(true)}
                    disabled={!detailsEnabled}
                    className="flex-1 lg:flex-none"
                    title={detailsEnabled ? 'Ver detalhes do cálculo' : 'Clique em Gerar Cálculo para habilitar'}
                  >
                    <FileText className="w-4 h-4 mr-2" />
                    Detalhes
                  </Button>
                </div>
              </div>
            </div>
          </div>
        </div>
      )}

      {/* Details Dialog */}
      <Dialog open={showDetails} onOpenChange={setShowDetails}>
        <DialogContent className="max-w-6xl max-h-[90vh] flex flex-col overflow-hidden">
          <DialogHeader>
            <DialogTitle className="flex items-center gap-2">
              <FileText className="w-5 h-5" />
              Detalhes Comparativos do Cálculo
            </DialogTitle>
            <DialogDescription>
              Comparação detalhada entre remuneração pretendida e proposta
            </DialogDescription>
          </DialogHeader>

          <div className="flex-1 min-h-0 overflow-y-auto overflow-x-visible">
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6 mb-6">
            <Card className={!isCompliant ? 'bg-destructive/10 dark:bg-destructive/20 border-destructive/30' : 'opacity-60'}>
              <CardHeader>
                <div className="flex items-center justify-between mb-4">
                  <div className="flex items-center gap-2">
                    <Badge className={isCompliant ? 'bg-success text-white border-0' : 'bg-destructive text-white border-0'}>
                      {isCompliant ? 'Em Conformidade' : 'Não Conforme'}
                    </Badge>
                  </div>
                  <CardTitle className="text-lg">Remuneração Pretendida</CardTitle>
                </div>
              </CardHeader>
              <CardContent className="space-y-1">
                <div className="flex justify-between text-sm">
                  <span className="text-muted-foreground">Rendimento Total</span>
                  <span className="font-bold text-foreground">{formatCurrency(desiredFromApi ? (desiredFromApi.remuneracaoTotalLiquidaMensal ?? 0) + (desiredFromApi.valeRefeicao ?? 0) + (desiredFromApi.valeAlimentacao ?? 0) : desiredGross + desiredResults.monthlyBenefits)}</span>
                </div>
                <div className="flex justify-between text-sm">
                  <span className="text-muted-foreground">Líquido Mensal</span>
                  <span className="font-bold text-foreground">{formatCurrency(desiredFromApi?.remuneracaoTotalLiquidaMensal ?? desiredResults.netSalary)}</span>
                </div>
                <div className="flex justify-between text-sm">
                  <span className="text-muted-foreground">Ganho Real</span>
                  <span className="font-bold text-foreground">{formatCurrency(desiredFromApi?.remuneracaoTotalLiquidaMensalComVerbasAnuais ?? (desiredResults.netSalary + desiredResults.monthlyBenefits))}</span>
                </div>
          {viewMode !== 'collaborator' && (
            <div className="flex justify-between text-sm">
              <span className="text-muted-foreground flex items-center gap-2">
                Custo Empresa
                <button
                        type="button"
                        className="text-muted-foreground hover:text-foreground"
                        onClick={() => setShowCompanyCost((prev) => !prev)}
                        aria-label={showCompanyCost ? 'Ocultar custo empresa' : 'Mostrar custo empresa'}
                        title={showCompanyCost ? 'Ocultar' : 'Mostrar'}
                      >
                        {showCompanyCost ? <EyeOff className="w-4 h-4" /> : <Eye className="w-4 h-4" />}
                      </button>
                    </span>
                    <span className="font-bold text-primary">
                      {showCompanyCost ? formatCurrency(desiredFromApi?.custoTotalEmpresa ?? desiredResults.companyCost) : '••••••'}
                    </span>
                  </div>
                )}
              </CardContent>
            </Card>

            <Card>
              <CardHeader>
                <div className="flex items-center justify-between mb-4">
                  <div className="flex items-center gap-2">
                    <Badge variant="secondary" className="font-normal text-muted-foreground">
                      Proposta da Empresa
                    </Badge>
                  </div>
                  <CardTitle className="text-lg">Remuneração Proposta</CardTitle>
                </div>
              </CardHeader>
              <CardContent className="space-y-1">
                <div className="flex justify-between text-sm">
                  <span className="text-muted-foreground">Rendimento Total</span>
                  <span className="font-bold text-foreground">{formatCurrency(proposedFromApi ? (proposedFromApi.remuneracaoTotalLiquidaMensal ?? 0) + (proposedFromApi.valeRefeicao ?? 0) + (proposedFromApi.valeAlimentacao ?? 0) : formData.grossSalary + proposedResults.monthlyBenefits)}</span>
                </div>
                <div className="flex justify-between text-sm">
                  <span className="text-muted-foreground">Líquido Mensal</span>
                  <span className="font-bold text-foreground">{formatCurrency(proposedFromApi?.remuneracaoTotalLiquidaMensal ?? proposedResults.netSalary)}</span>
                </div>
                <div className="flex justify-between text-sm">
                  <span className="text-muted-foreground">Ganho Real</span>
                  <span className="font-bold text-foreground">{formatCurrency(proposedFromApi?.remuneracaoTotalLiquidaMensalComVerbasAnuais ?? (proposedResults.netSalary + proposedResults.monthlyBenefits))}</span>
                </div>
                {viewMode !== 'collaborator' && (
                  <div className="flex justify-between text-sm">
                    <span className="text-muted-foreground flex items-center gap-2">
                      Custo Empresa
                      <button
                        type="button"
                        className="text-muted-foreground hover:text-foreground"
                        onClick={() => setShowCompanyCost((prev) => !prev)}
                        aria-label={showCompanyCost ? 'Ocultar custo empresa' : 'Mostrar custo empresa'}
                        title={showCompanyCost ? 'Ocultar' : 'Mostrar'}
                      >
                        {showCompanyCost ? <EyeOff className="w-4 h-4" /> : <Eye className="w-4 h-4" />}
                      </button>
                    </span>
                    <span className="font-bold text-primary">
                      {showCompanyCost ? formatCurrency(proposedFromApi?.custoTotalEmpresa ?? proposedResults.companyCost) : '••••••'}
                    </span>
                  </div>
                )}
              </CardContent>
            </Card>
          </div>

          <div className="mb-4">
            <h3 className="text-lg font-bold text-foreground">Comparativo de Remuneração Salarial</h3>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            <Card>
              <CardHeader>
                <CardTitle className="text-center">Remuneração Pretendida</CardTitle>
              </CardHeader>
              <CardContent className="space-y-4">
                {formData.isTrustPosition ? (
                  <div className="flex justify-center">
                    <Badge variant="accent" className="text-xs">
                      Cargo de confiança
                    </Badge>
                  </div>
                ) : null}
                <div>
                  <h5 className="text-success font-bold text-xs uppercase mb-2">Proventos</h5>
                  <div className="space-y-2 text-sm">
                    <DetalheItemComValidacao
                      label="Salário em carteira"
                      valueFormatted={formatCurrency(desiredFromApi?.clt?.salarioBruto ?? desiredGross)}
                      validacao={desiredFromApi?.validacaoCLT}
                    />
                    <DetalheItemComValidacao
                      label="Refeição"
                      valueFormatted={formatCurrency(desiredFromApi?.valeRefeicao ?? MEAL_VOUCHER_BASE)}
                      validacao={desiredFromApi?.validacaoValeRefeicao}
                    />
                    <DetalheItemComValidacao
                      label="Alimentação"
                      valueFormatted={formatCurrency(desiredFromApi?.valeAlimentacao ?? formData.foodAllowance)}
                      validacao={desiredFromApi?.validacaoValeAlimentacao}
                    />
                    <DetalheItemComValidacao
                      label="Mobilidade"
                      valueFormatted={formatCurrency(desiredFromApi?.mobilidade ?? desiredResults.mobilityCost)}
                      validacao={desiredFromApi?.validacaoMobilidade}
                    />
                    <DetalheItemComValidacao
                      label="Educação"
                      valueFormatted={formatCurrency(desiredFromApi?.auxilioEducacao ?? totalEducationCost)}
                      validacao={desiredFromApi?.validacaoAuxilioEducacao}
                    />
                    <DetalheItemComValidacao
                      label="Ajuda de Custo"
                      valueFormatted={formatCurrency(desiredAjudaDeCusto)}
                      validacao={desiredFromApi?.validacaoAjudaDeCusto}
                    />
                    <div className="flex justify-between border-t border-border pt-1 font-bold text-foreground">
                      <span>Rendimento Total</span>
                      <span>{formatCurrency(desiredFromApi ? (desiredFromApi.clt?.salarioBruto ?? 0) + (desiredFromApi.valeRefeicao ?? 0) + (desiredFromApi.valeAlimentacao ?? 0) + (desiredFromApi.mobilidade ?? 0) + (desiredFromApi.auxilioEducacao ?? 0) + (desiredFromApi.ajudaDeCusto ?? 0) : desiredGross + desiredResults.monthlyBenefits)}</span>
                    </div>
                  </div>
                </div>
                <div>
                  <h5 className="text-info font-bold text-xs uppercase mb-2">Descontos</h5>
                  <div className="space-y-2 text-sm">
                    <div className="flex justify-between">
                      <span className="text-foreground">INSS</span>
                      <span className="font-medium text-info">- {formatCurrency(desiredFromApi?.clt?.descontoInss ?? desiredResults.inss)}</span>
                    </div>
                    <div className="flex justify-between">
                      <span className="text-foreground">IRRF Salário</span>
                      <span className="font-medium text-info">- {formatCurrency(desiredFromApi?.clt?.descontoIrrf ?? desiredResults.irrf)}</span>
                    </div>
                    {planDependentsTotal > 0 && (
                      <div className="flex justify-between">
                        <span className="text-foreground flex items-center gap-2">
                          Plano Dependentes
                          <TooltipProvider>
                            <UiTooltip>
                              <TooltipTrigger asChild>
                                <button type="button" className="text-muted-foreground hover:text-foreground">
                                  <Info className="w-4 h-4" />
                                </button>
                              </TooltipTrigger>
                              <TooltipContent className="text-xs max-w-xs">
                                Valor unitário por dependente: {formatCurrency(planDependentsUnit)} • Quantidade: {planDependentsCount}
                              </TooltipContent>
                            </UiTooltip>
                          </TooltipProvider>
                        </span>
                        <span className="font-medium text-info">- {formatCurrency(planDependentsTotal)}</span>
                      </div>
                    )}
                    {viewMode === 'collaborator' && (
                      <div className="flex justify-between">
                        <span className="text-foreground">Gastos Variáveis</span>
                        <span className="font-medium text-info">- {formatCurrency(desiredResults.totalVariableExpenses)}</span>
                      </div>
                    )}
                    <div className="flex justify-between border-t border-border pt-1 font-bold text-foreground">
                      <span>Total de Descontos</span>
                      <span>{formatCurrency(desiredFromApi?.clt?.totalDescontos ?? desiredResults.totalDeductions)}</span>
                    </div>
                  </div>
                </div>
                <Card className="bg-muted">
                  <CardContent className="p-3 space-y-2">
                    <div className="flex justify-between font-bold text-lg text-foreground gap-3">
                      <span className="flex flex-col leading-tight">
                        <span>Total Remuneração Líquida</span>
                        <span className="text-xs font-normal text-muted-foreground">(Crédito em conta + Cartões)</span>
                      </span>
                      <span>{formatCurrency(desiredFromApi?.remuneracaoTotalLiquidaMensal ?? (desiredResults.netSalary + desiredResults.monthlyBenefits))}</span>
                    </div>
                    <div className="flex justify-between text-sm text-muted-foreground">
                      <span>Ganho em Líquido em Dinheiro</span>
                      <span>{formatCurrency(desiredFromApi?.clt?.salarioLiquido ?? desiredResults.netSalary)}</span>
                    </div>
                    <div className="flex justify-between text-sm text-muted-foreground">
                      <span>Ganho em Cartão (Alimentação + Refeição)</span>
                      <span>{formatCurrency(desiredFromApi ? (desiredFromApi.valeRefeicao ?? 0) + (desiredFromApi.valeAlimentacao ?? 0) : MEAL_VOUCHER_BASE + formData.foodAllowance)}</span>
                    </div>
                    <div className="border-t border-border pt-2 flex justify-between text-primary font-bold text-lg gap-3">
                      <span className="flex flex-col leading-tight">
                        <span>Total Real do Ganho Mensal</span>
                        <span className="text-xs font-normal text-muted-foreground">
                          (Remuneração líquida + 1/3 férias + 13° Salário + FGTS)
                        </span>
                      </span>
                      <span>
                        {formatCurrency(
                          desiredFromApi?.remuneracaoTotalLiquidaMensalComVerbasAnuais ??
                            (desiredResults.netSalary +
                              desiredResults.monthlyBenefits +
                              desiredGross / 36 +
                              desiredGross / 12 +
                              desiredGross * 0.08),
                        )}
                      </span>
                    </div>
                  </CardContent>
                </Card>
                <div className="border-t border-border pt-4">
                  <h5 className="text-sm font-bold text-primary mb-1">Previsão Anual</h5>
                  <div className="text-2xl font-bold text-foreground mb-2">{formatCurrency(desiredFromApi?.previsaoAnual ?? desiredResults.annualTotal)}</div>
                  <div className="flex justify-between items-center text-xs text-muted-foreground">
                    <span className="flex items-center gap-1">
                      <TrendingUp className="w-3 h-3" /> 
                      Média Mensal
                    </span>
                    <span className="font-bold">{formatCurrency(desiredFromApi?.previsaoAnual != null ? desiredFromApi.previsaoAnual / 12 : desiredResults.monthlyAverage)}</span>
                  </div>
                </div>
              </CardContent>
            </Card>

            <Card>
              <CardHeader>
                <CardTitle className="text-center">Remuneração Proposta</CardTitle>
              </CardHeader>
              <CardContent className="space-y-4">
                {formData.isTrustPosition ? (
                  <div className="flex justify-center">
                    <Badge variant="accent" className="text-xs">
                      Cargo de confiança
                    </Badge>
                  </div>
                ) : null}
                <div>
                  <h5 className="text-success font-bold text-xs uppercase mb-2">Proventos</h5>
                  <div className="space-y-2 text-sm">
                    <div className="flex justify-between">
                      <span className="text-foreground">Salário em carteira</span>
                      <span className="font-medium text-foreground">{formatCurrency(proposedFromApi?.clt?.salarioBruto ?? formData.grossSalary)}</span>
                    </div>
                    <div className="flex justify-between">
                      <span className="text-foreground">Refeição</span>
                      <span className="font-medium text-foreground">{formatCurrency(proposedFromApi?.valeRefeicao ?? MEAL_VOUCHER_BASE)}</span>
                    </div>
                    <div className="flex justify-between">
                      <span className="text-foreground">Alimentação</span>
                      <span className="font-medium text-foreground">{formatCurrency(proposedFromApi?.valeAlimentacao ?? formData.foodAllowance)}</span>
                    </div>
                    <div className="flex justify-between">
                      <span className="text-foreground">Mobilidade</span>
                      <span className="font-medium text-foreground">{formatCurrency(proposedFromApi?.mobilidade ?? proposedResults.mobilityCost)}</span>
                    </div>
                    <div className="flex justify-between">
                      <span className="text-foreground">Educação</span>
                      <span className="font-medium text-foreground">{formatCurrency(proposedFromApi?.auxilioEducacao ?? totalEducationCost)}</span>
                    </div>
                    <div className="flex justify-between">
                      <span className="text-foreground">Ajuda de Custo</span>
                      <span className="font-medium text-foreground">{formatCurrency(proposedAjudaDeCusto)}</span>
                    </div>
                    <div className="flex justify-between border-t border-border pt-1 font-bold text-foreground">
                      <span>Rendimento Total</span>
                      <span>{formatCurrency(proposedFromApi ? (proposedFromApi.clt?.salarioBruto ?? 0) + (proposedFromApi.valeRefeicao ?? 0) + (proposedFromApi.valeAlimentacao ?? 0) + (proposedFromApi.mobilidade ?? 0) + (proposedFromApi.auxilioEducacao ?? 0) + (proposedFromApi.ajudaDeCusto ?? 0) : formData.grossSalary + proposedResults.monthlyBenefits)}</span>
                    </div>
                  </div>
                </div>
                <div>
                  <h5 className="text-info font-bold text-xs uppercase mb-2">Descontos</h5>
                  <div className="space-y-2 text-sm">
                    <div className="flex justify-between">
                      <span className="text-foreground">INSS</span>
                      <span className="font-medium text-info">- {formatCurrency(proposedFromApi?.clt?.descontoInss ?? proposedResults.inss)}</span>
                    </div>
                    <div className="flex justify-between">
                      <span className="text-foreground">IRRF Salário</span>
                      <span className="font-medium text-info">- {formatCurrency(proposedFromApi?.clt?.descontoIrrf ?? proposedResults.irrf)}</span>
                    </div>
                    {planDependentsTotal > 0 && (
                      <div className="flex justify-between">
                        <span className="text-foreground flex items-center gap-2">
                          Plano Dependentes
                          <TooltipProvider>
                            <UiTooltip>
                              <TooltipTrigger asChild>
                                <button type="button" className="text-muted-foreground hover:text-foreground">
                                  <Info className="w-4 h-4" />
                                </button>
                              </TooltipTrigger>
                              <TooltipContent className="text-xs max-w-xs">
                                Valor unitário por dependente: {formatCurrency(planDependentsUnit)} • Quantidade: {planDependentsCount}
                              </TooltipContent>
                            </UiTooltip>
                          </TooltipProvider>
                        </span>
                        <span className="font-medium text-info">- {formatCurrency(planDependentsTotal)}</span>
                      </div>
                    )}
                    {viewMode === 'collaborator' && (
                      <div className="flex justify-between">
                        <span className="text-foreground">Gastos Variáveis</span>
                        <span className="font-medium text-info">- {formatCurrency(proposedResults.totalVariableExpenses)}</span>
                      </div>
                    )}
                    <div className="flex justify-between border-t border-border pt-1 font-bold text-foreground">
                      <span>Total de Descontos</span>
                      <span>{formatCurrency(proposedFromApi?.clt?.totalDescontos ?? proposedResults.totalDeductions)}</span>
                    </div>
                  </div>
                </div>
                <Card className="bg-success/10 dark:bg-success/20 border-success/30">
                  <CardContent className="p-3 space-y-2">
                    <div className="flex justify-between font-bold text-lg text-foreground gap-3">
                      <span className="flex flex-col leading-tight">
                        <span>Total Remuneração Líquida</span>
                        <span className="text-xs font-normal text-muted-foreground">(Crédito em conta + Cartões)</span>
                      </span>
                      <span>{formatCurrency(proposedFromApi?.remuneracaoTotalLiquidaMensal ?? (proposedResults.netSalary + proposedResults.monthlyBenefits))}</span>
                    </div>
                    <div className="flex justify-between text-sm text-muted-foreground">
                      <span>Ganho em Líquido em Dinheiro</span>
                      <span>{formatCurrency(proposedFromApi?.clt?.salarioLiquido ?? proposedResults.netSalary)}</span>
                    </div>
                    <div className="flex justify-between text-sm text-muted-foreground">
                      <span>Ganho em Cartão (Alimentação + Refeição)</span>
                      <span>{formatCurrency(proposedFromApi ? (proposedFromApi.valeRefeicao ?? 0) + (proposedFromApi.valeAlimentacao ?? 0) : MEAL_VOUCHER_BASE + formData.foodAllowance)}</span>
                    </div>
                    <div className="border-t border-success/30 pt-2 flex justify-between text-success font-bold text-lg gap-3">
                      <span className="flex flex-col leading-tight">
                        <span>Total Real do Ganho Mensal</span>
                        <span className="text-xs font-normal text-muted-foreground">
                          (Remuneração líquida + 1/3 férias + 13° Salário + FGTS)
                        </span>
                      </span>
                      <span>
                        {formatCurrency(
                          proposedFromApi?.remuneracaoTotalLiquidaMensalComVerbasAnuais ??
                            (proposedResults.netSalary +
                              proposedResults.monthlyBenefits +
                              formData.grossSalary / 36 +
                              formData.grossSalary / 12 +
                              formData.grossSalary * 0.08),
                        )}
                      </span>
                    </div>
                  </CardContent>
                </Card>
                <div className="border-t border-border pt-4">
                  <h5 className="text-sm font-bold text-primary mb-1">Previsão Anual</h5>
                  <div className="text-2xl font-bold text-foreground mb-2">{formatCurrency(proposedFromApi?.previsaoAnual ?? proposedResults.annualTotal)}</div>
                  <div className="flex justify-between items-center text-xs text-muted-foreground">
                    <span className="flex items-center gap-1">
                      <TrendingUp className="w-3 h-3" /> 
                      Média Mensal
                    </span>
                    <span className="font-bold">{formatCurrency(proposedFromApi?.previsaoAnual != null ? proposedFromApi.previsaoAnual / 12 : proposedResults.monthlyAverage)}</span>
                  </div>
                </div>
              </CardContent>
            </Card>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-2 gap-4 pt-4 border-t border-border">
            <Button variant="outline" onClick={() => handleExportPDF('desired')} className="w-full">
              <Download className="w-4 h-4 mr-2" /> 
              Baixar Remuneração Pretendida
            </Button>
            <Button onClick={() => handleExportPDF('proposed')} className="w-full">
              <Download className="w-4 h-4 mr-2" /> 
              Baixar Remuneração Proposta
            </Button>
          </div>
          </div>
        </DialogContent>
      </Dialog>

      {/* Token Update Dialog */}
      <Dialog open={showTokenModal} onOpenChange={setShowTokenModal}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle className="flex items-center gap-2">
              <Key className="w-5 h-5 text-primary" />
              Atualizar Token API
            </DialogTitle>
            <DialogDescription>
              Insira o novo token de autenticação para atualizar o acesso aos dados do colaborador e gestores.
            </DialogDescription>
          </DialogHeader>
          <div className="space-y-4">
            <div>
              <Label htmlFor="token">Bearer Token</Label>
              <Textarea
                id="token"
                className="h-32 font-mono"
                value={tempToken}
                onChange={(e) => setTempToken(e.target.value)}
                placeholder="eyJhbGciOi..."
              />
            </div>
            <div className="flex justify-end gap-3">
              <Button variant="outline" onClick={() => setShowTokenModal(false)}>
                Cancelar
              </Button>
              <Button onClick={handleSaveToken}>
                Salvar Alterações
              </Button>
            </div>
          </div>
        </DialogContent>
      </Dialog>
    </div>
  );
};

export default Simulator;
