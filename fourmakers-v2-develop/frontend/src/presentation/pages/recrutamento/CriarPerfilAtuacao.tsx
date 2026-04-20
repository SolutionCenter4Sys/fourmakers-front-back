import { useState, useRef } from 'react';
import { useNavigate } from 'react-router-dom';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Textarea } from '@/components/ui/textarea';
import { Popover, PopoverContent, PopoverTrigger } from '@/components/ui/popover';
import { Command, CommandEmpty, CommandGroup, CommandInput, CommandItem, CommandList } from '@/components/ui/command';
import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { ScrollArea } from '@/components/ui/scroll-area';
import { Separator } from '@/components/ui/separator';
import { Alert, AlertDescription, AlertTitle } from '@/components/ui/alert';
import { PageBreadcrumb, PageHeader } from '@presentation/components/common';
import { usePerfilAtuacao, usePerfilAtuacaoPage } from '@presentation/hooks/recrutamento'
import type { PerfilSkillType } from '@domain/entities/PerfilAtuacao';
import { skillTypeLabels } from '@shared/utils/perfilAtuacao';
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from '@/components/ui/tooltip';
import { Spinner } from '@/components/ui/spinner';
import { MapPin, Trash2, ChevronsUpDown, Check, Wand2, Save, Info, Plus, ArrowLeft, Star, StarFilled } from '@/components/ui/system-icons';
import { AILoadingModal } from '@presentation/components/profile/AILoadingModal';

const skillTypes: PerfilSkillType[] = ['COMPETENCIA', 'SOFTSKILL', 'METODOLOGIA', 'DOMINIONEGOCIO', 'IDIOMA'];

export const CriarPerfilAtuacao = () => {
  const {
    idPerfilFromUrl,
    novaVagaMode,
    profileLoading,
    formData,
    optionsLoading,
    submitting,
    aiLoading,
    clients,
    managers,
    clientsLoading,
    managersLoading,
    setClientSearch,
    setManagerSearch,
    iaToken,
    setIaToken,
    showTokenModal,
    setShowTokenModal,
    permanenceOptions,
    workModels,
    locationOptions,
    employmentTypes,
    experienceLevels,
    handleInputChange,
    handleWorkModelChange,
    handleRemoveSkill,
    openSkillModal,
    skillModalOpen,
    setSkillModalOpen,
    skillModalType,
    skillSearch,
    setSkillSearch,
    skillResults,
    skillLoading,
    skillHasSearched,
    selectedSkill,
    handleSelectSkill,
    skillLevels,
    skillLevelsLoading,
    selectedLevelId,
    handleSelectLevel,
    handleRemovePendingSkill,
    handleTogglePendingSkillRelevante,
    handleToggleSkillRelevante,
    editingSkillForLevel,
    openEditLevelModal,
    closeEditLevelModal,
    editLevelSelectedId,
    handleSelectLevelForEdit,
    handleUpdateSkillLevel,
    handleCreateSkillFromSearch,
    pendingSkills,
    handleSavePendingSkills,
    handleCepLookup,
    handleSubmit,
    handleGenerateWithAI,
    handleFillLinkedinTemplate,
    aiSummary,
    fieldErrors,
    formatCep,
    formatarValorMonetario,
    skillIdsWithLevelError,
  } = usePerfilAtuacao();
  const navigate = useNavigate();
  const [summaryOpen, setSummaryOpen] = useState(false);
  const [infoPopoverOpen, setInfoPopoverOpen] = useState<'workModel' | 'permanence' | 'location' | null>(null);
  const infoHoverCloseRef = useRef<ReturnType<typeof setTimeout> | null>(null);
  const {
    linkedinRows,
    nameRef,
    managerRef,
    employmentRef,
    experienceRef,
    cepRef,
    linkedinRef,
    skillsRef,
    handleSubmitWithScroll,
    skillsByType,
    isHybrid,
    isRemote,
    isClientPopoverOpen,
    setIsClientPopoverOpen,
    isManagerPopoverOpen,
    setIsManagerPopoverOpen,
  } = usePerfilAtuacaoPage({ formData, fieldErrors, handleSubmit, workModels });

  const handleSave = async () => {
    const result = await handleSubmitWithScroll();
    if (result?.success && novaVagaMode) {
      navigate('/recrutamento');
    }
  };

  const renderSkillSection = (type: PerfilSkillType) => {
    const skills = skillsByType[type];
    const required = type === 'COMPETENCIA' || type === 'SOFTSKILL';
    const hasError =
      (type === 'COMPETENCIA' && fieldErrors.hardSkills) || (type === 'SOFTSKILL' && fieldErrors.softSkills);
    return (
      <Card className={hasError ? 'border-destructive border-2' : 'border-borderSoft'}>
        <CardHeader className="pb-2 flex flex-row items-center justify-between">
          <CardTitle className="text-base font-semibold">
            {skillTypeLabels[type]} {required ? <span className="text-destructive">*</span> : null}
          </CardTitle>
          <Tooltip>
            <TooltipTrigger asChild>
              <Button variant="outline" size="icon" onClick={() => openSkillModal(type)} aria-label="Adicionar habilidade">
                <Plus className="h-4 w-4" />
              </Button>
            </TooltipTrigger>
            <TooltipContent side="left">Adicionar habilidade</TooltipContent>
          </Tooltip>
        </CardHeader>
        <CardContent className="space-y-3">
          <div className="flex flex-wrap gap-2">
            {skills.map(skill => {
              const label = skill.levelName ? `${skill.name} - ${skill.levelName}` : skill.name;
              const hasLevelError = skillIdsWithLevelError.includes(skill.id);
              const relevante = skill.relevante ?? true;
              const badgeClass = hasLevelError
                ? 'border-2 border-destructive bg-surfaceSubtle text-primaryText'
                : relevante
                  ? 'bg-btnPrimary text-inverseText border border-primary'
                  : 'bg-btnSecondary text-btnSecondaryText border border-borderDefault';
              return (
                <Badge
                  key={skill.id}
                  variant="secondary"
                  data-relevante={relevante}
                  className={`group inline-flex max-w-[min(100%,280px)] min-w-0 shrink items-center gap-2 rounded-full border px-3 py-2 ${badgeClass}`}
                >
                  <button
                    type="button"
                    onClick={() => handleToggleSkillRelevante(type, skill.id)}
                    className="shrink-0 flex items-center justify-center rounded p-0.5 text-inherit hover:opacity-80 focus:outline-none focus:ring-2 focus:ring-ring focus:ring-offset-1"
                    aria-label={relevante ? 'Marcar como desejável' : 'Marcar como imprescindível'}
                    title={relevante ? 'Imprescindível (clique para desejável)' : 'Desejável (clique para imprescindível)'}
                  >
                    {relevante ? <StarFilled className="h-3.5 w-3.5" /> : <Star className="h-3.5 w-3.5" />}
                  </button>
                  <Tooltip>
                    <TooltipTrigger asChild>
                      <button
                        type="button"
                        onClick={() => openEditLevelModal(skill, 'page', type)}
                        className="min-w-0 flex-1 truncate text-left font-inherit hover:underline focus:outline-none focus:ring-2 focus:ring-ring focus:ring-offset-1 rounded"
                        aria-label={`Editar nível de ${skill.name}`}
                      >
                        {label}
                      </button>
                    </TooltipTrigger>
                    <TooltipContent side="top" className="max-w-[min(320px,90vw)]">
                      {label} (clique para editar nível)
                    </TooltipContent>
                  </Tooltip>
                  <button
                    type="button"
                    onClick={() => handleRemoveSkill(type, skill.id)}
                    className="shrink-0 flex items-center justify-center rounded p-0.5 text-inherit hover:opacity-80 focus:outline-none focus:ring-2 focus:ring-ring focus:ring-offset-1 group-data-[relevante=false]:hover:text-destructive"
                    aria-label={`Remover ${skill.name}`}
                  >
                    <Trash2 className="h-3.5 w-3.5" />
                  </button>
                </Badge>
              );
            })}
            {skills.length === 0 && <span className="text-xs text-muted-foreground">Nenhuma skill adicionada</span>}
          </div>
        </CardContent>
      </Card>
    );
  };

  return (
    <TooltipProvider>
    <div className="container mx-auto max-w-6xl p-4 space-y-6">
      <PageBreadcrumb
        items={[
          { label: 'Dashboard', href: '/dashboard' },
          { label: 'Recrutamento', href: '/recrutamento' },
          { label: 'Perfil de atuação' },
        ]}
      />

      <div className="flex items-center gap-3">
        <Button
          variant="ghost"
          size="icon"
          aria-label="Voltar"
          onClick={() => navigate('/recrutamento')}
        >
          <ArrowLeft className="h-5 w-5" />
        </Button>
        <PageHeader
          title={idPerfilFromUrl ? 'Editar perfil de atuação' : novaVagaMode ? 'Criar vaga (perfil completo)' : 'Perfil de atuação'}
          description={idPerfilFromUrl ? 'Ajuste os dados do perfil de atuação.' : novaVagaMode ? 'Preencha todos os campos do perfil e adicione ao menos 2 Hard Skills e 1 Soft Skill. Ao salvar, o perfil será criado e a vaga será gerada automaticamente.' : 'Crie ou ajuste perfis completos de gestores externos com apoio de IA, validações e skills obrigatórias.'}
        />
      </div>

      {novaVagaMode && !profileLoading && (
        <Alert className="border-info/50 bg-info/5">
          <Info className="h-4 w-4" />
          <AlertTitle>Criar vaga</AlertTitle>
          <AlertDescription>
            Para criar uma nova vaga é necessário preencher todos os campos do formulário e incluir no mínimo 2 Hard Skills e 1 Soft Skill. Após salvar, a vaga será criada automaticamente a partir deste perfil.
          </AlertDescription>
        </Alert>
      )}

      {profileLoading ? (
        <div className="flex items-center justify-center gap-2 py-12 text-muted-foreground">
<Spinner size={24} aria-hidden />
          <span>Carregando perfil...</span>
        </div>
      ) : (
      <>
      <div className="grid grid-cols-1 gap-4 lg:grid-cols-3">
        <div className="lg:col-span-2 space-y-4">
          <Card className="border-borderSoft">
            <CardHeader>
              <CardTitle>Identificação e vínculo</CardTitle>
            </CardHeader>
            <CardContent className="space-y-4">
              <div className="grid gap-4 sm:grid-cols-2">
                <div className="sm:col-span-2 space-y-2">
                  <Label>Nome do Perfil *</Label>
                  <Input
                    ref={nameRef}
                    placeholder="Ex: Desenvolvedor Full Stack Sênior"
                    value={formData.name}
                    onChange={e => handleInputChange('name', e.target.value)}
                    className={fieldErrors.name ? 'border-destructive focus-visible:ring-destructive' : undefined}
                  />
                </div>
                <div className="space-y-2">
                  <Label>Cliente</Label>
                  <Popover open={isClientPopoverOpen} onOpenChange={setIsClientPopoverOpen}>
                    <PopoverTrigger asChild>
                      <Button
                        variant="outline"
                        className={`w-full justify-between text-sm font-normal ${fieldErrors.clientCode ? 'border-destructive' : ''}`}
                      >
                        {formData.clientName || 'Selecione o cliente'}
                        <ChevronsUpDown className="h-4 w-4 opacity-50" />
                      </Button>
                    </PopoverTrigger>
                    <PopoverContent className="p-0 w-[320px]">
                      <Command shouldFilter={false}>
                        <CommandInput placeholder="Buscar cliente..." onValueChange={setClientSearch} className="text-sm" />
                        <CommandList>
                          {clientsLoading ? (
                            <CommandEmpty>Carregando...</CommandEmpty>
                          ) : clients.length === 0 ? (
                            <CommandEmpty>Nenhum cliente encontrado</CommandEmpty>
                          ) : (
                            <CommandGroup>
                              {clients.map(client => (
                                <CommandItem
                                  key={client.id}
                                  value={client.name}
                                  onSelect={() => {
                                    handleInputChange('clientCode', client.id);
                                    handleInputChange('clientName', client.name);
                                    handleInputChange('managerCode', '');
                                    handleInputChange('managerInternalCode', '');
                                    handleInputChange('managerName', '');
                                    setClientSearch('');
                                    setManagerSearch('');
                                    setIsClientPopoverOpen(false);
                                    setIsManagerPopoverOpen(false);
                                  }}
                                  className="text-sm"
                                >
                                  <Check className={`mr-2 h-4 w-4 ${formData.clientCode === client.id ? 'opacity-100' : 'opacity-0'}`} />
                                  {client.name}
                                </CommandItem>
                              ))}
                            </CommandGroup>
                          )}
                        </CommandList>
                      </Command>
                    </PopoverContent>
                  </Popover>
                </div>
                <div className="space-y-2">
                  <Label>Gestor *</Label>
                  <Popover open={isManagerPopoverOpen} onOpenChange={setIsManagerPopoverOpen}>
                    <PopoverTrigger asChild>
                      <Button
                        variant="outline"
                        className={`w-full justify-between text-sm font-normal ${fieldErrors.managerCode ? 'border-destructive' : ''}`}
                        ref={managerRef}
                      >
                        {formData.managerName || 'Selecione o gestor'}
                        <ChevronsUpDown className="h-4 w-4 opacity-50" />
                      </Button>
                    </PopoverTrigger>
                    <PopoverContent className="p-0 w-[320px]">
                      <Command shouldFilter={false}>
                        <CommandInput
                          placeholder="Buscar gestor..."
                          onValueChange={setManagerSearch}
                          className="text-sm"
                        />
                        <CommandList>
                          {!formData.clientCode ? (
                            <CommandEmpty>Selecione um cliente primeiro</CommandEmpty>
                          ) : managersLoading ? (
                            <CommandEmpty>Carregando...</CommandEmpty>
                          ) : managers.length === 0 ? (
                            <CommandEmpty>Nenhum gestor encontrado</CommandEmpty>
                          ) : (
                            <CommandGroup>
                              {managers.map(manager => (
                                <CommandItem
                                  key={manager.id}
                                  value={manager.name}
                                  onSelect={() => {
                                    handleInputChange('managerCode', manager.id);
                                    handleInputChange('managerInternalCode', manager.internalCode);
                                    handleInputChange('managerName', manager.name);
                                    setManagerSearch('');
                                    setIsManagerPopoverOpen(false);
                                  }}
                                  className="text-sm"
                                >
                                  <Check className={`mr-2 h-4 w-4 ${formData.managerCode === manager.id ? 'opacity-100' : 'opacity-0'}`} />
                                  {manager.name}
                                </CommandItem>
                              ))}
                            </CommandGroup>
                          )}
                        </CommandList>
                      </Command>
                    </PopoverContent>
                  </Popover>
                </div>
              </div>
            </CardContent>
          </Card>

          <Card className="border-borderSoft">
            <CardHeader>
              <CardTitle>Geração por IA</CardTitle>
              <p className="text-sm text-muted-foreground">Descreva o perfil e deixe a IA preencher os campos automaticamente.</p>
            </CardHeader>
            <CardContent className="space-y-4">
              <div className="space-y-2">
                <Label>Descreva o perfil</Label>
                <Textarea
                  placeholder="Ex: Preciso de um desenvolvedor full stack..."
                  value={formData.aiPrompt}
                  onChange={e => handleInputChange('aiPrompt', e.target.value)}
                  rows={4}
                />
              </div>
          <div className="flex gap-2">
            <Button type="button" onClick={handleGenerateWithAI} disabled={aiLoading || optionsLoading || profileLoading}>
{aiLoading ? <Spinner className="mr-2" size={16} aria-hidden /> : <Wand2 className="h-4 w-4 mr-2" />}
              Gerar com IA
            </Button>
            {aiSummary && (
              <Button type="button" variant="outline" size="icon" onClick={() => setSummaryOpen(true)} aria-label="Ver resumo da IA">
                <Info className="h-4 w-4" />
              </Button>
            )}
          </div>
            </CardContent>
          </Card>

          <Card className="border-borderSoft">
            <CardHeader>
              <CardTitle>Configurações de trabalho e localização</CardTitle>
            </CardHeader>
            <CardContent className="space-y-4">
              <div className="grid gap-4 sm:grid-cols-2">
                <div className="space-y-2">
                  <div className="flex items-center gap-2">
                    <Label>Modelo de Trabalho *</Label>
                    <Popover open={infoPopoverOpen === 'workModel'} onOpenChange={open => setInfoPopoverOpen(open ? 'workModel' : null)}>
                      <PopoverTrigger asChild>
                        <Button
                          type="button"
                          variant="ghost"
                          size="icon"
                          className="h-6 w-6 shrink-0"
                          aria-label="Informações sobre modelo de trabalho"
                          onMouseEnter={() => {
                            if (infoHoverCloseRef.current) {
                              clearTimeout(infoHoverCloseRef.current);
                              infoHoverCloseRef.current = null;
                            }
                            setInfoPopoverOpen('workModel');
                          }}
                          onMouseLeave={() => {
                            infoHoverCloseRef.current = setTimeout(() => setInfoPopoverOpen(null), 200);
                          }}
                        >
                          <Info className="h-4 w-4 text-muted-foreground" />
                        </Button>
                      </PopoverTrigger>
                      <PopoverContent side="bottom" className="max-w-[320px] p-3 text-sm" onOpenAutoFocus={e => e.preventDefault()}>
                        Informe o modelo de trabalho desejado para este perfil (presencial, remoto ou híbrido).
                      </PopoverContent>
                    </Popover>
                  </div>
                  <Select value={formData.workModelId} onValueChange={handleWorkModelChange}>
                    <SelectTrigger className={fieldErrors.workModelId ? 'border-destructive' : undefined}>
                      <SelectValue placeholder="Selecione" />
                    </SelectTrigger>
                    <SelectContent>
                      {workModels.map(model => (
                        <SelectItem key={model.id} value={model.id}>
                          {model.name}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </div>
                {isHybrid && (
                  <div>
                    <Label>Dias presenciais (híbrido)</Label>
                    <Input
                      type="number"
                      min={1}
                      max={4}
                      step={1}
                      value={Math.min(4, Math.max(1, formData.hybridDays ?? 1))}
                      onChange={e => {
                        const raw = Number(e.target.value);
                        const clamped = Number.isNaN(raw) ? 1 : Math.min(4, Math.max(1, raw));
                        handleInputChange('hybridDays', clamped);
                      }}
                      className={fieldErrors.hybridDays ? 'border-destructive focus-visible:ring-destructive' : undefined}
                    />
                  </div>
                )}
                <div className="space-y-2">
                  <div className="flex items-center gap-2">
                    <Label>Período Mínimo de Experiência</Label>
                    <Popover open={infoPopoverOpen === 'permanence'} onOpenChange={open => setInfoPopoverOpen(open ? 'permanence' : null)}>
                      <PopoverTrigger asChild>
                        <Button
                          type="button"
                          variant="ghost"
                          size="icon"
                          className="h-6 w-6 shrink-0"
                          aria-label="Informações sobre período mínimo de experiência"
                          onMouseEnter={() => {
                            if (infoHoverCloseRef.current) {
                              clearTimeout(infoHoverCloseRef.current);
                              infoHoverCloseRef.current = null;
                            }
                            setInfoPopoverOpen('permanence');
                          }}
                          onMouseLeave={() => {
                            infoHoverCloseRef.current = setTimeout(() => setInfoPopoverOpen(null), 200);
                          }}
                        >
                          <Info className="h-4 w-4 text-muted-foreground" />
                        </Button>
                      </PopoverTrigger>
                      <PopoverContent side="bottom" className="max-w-[320px] p-3 text-sm" onOpenAutoFocus={e => e.preventDefault()}>
                        Informe o período mínimo de permanência em experiências anteriores.
                      </PopoverContent>
                    </Popover>
                  </div>
                  <Select value={formData.permanenceId} onValueChange={value => handleInputChange('permanenceId', value)}>
                    <SelectTrigger className={fieldErrors.permanenceId ? 'border-destructive' : undefined}>
                      <SelectValue placeholder="Selecione" />
                    </SelectTrigger>
                    <SelectContent>
                      {permanenceOptions.map(option => (
                        <SelectItem key={option.id} value={option.id}>
                          {option.name}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </div>
                <div className="space-y-2">
                  <div className="flex items-center gap-2">
                    <Label>Profissionais de outros estados</Label>
                    <Popover open={infoPopoverOpen === 'location'} onOpenChange={open => setInfoPopoverOpen(open ? 'location' : null)}>
                      <PopoverTrigger asChild>
                        <Button
                          type="button"
                          variant="ghost"
                          size="icon"
                          className="h-6 w-6 shrink-0"
                          aria-label="Informações sobre profissionais de outros estados"
                          onMouseEnter={() => {
                            if (infoHoverCloseRef.current) {
                              clearTimeout(infoHoverCloseRef.current);
                              infoHoverCloseRef.current = null;
                            }
                            setInfoPopoverOpen('location');
                          }}
                          onMouseLeave={() => {
                            infoHoverCloseRef.current = setTimeout(() => setInfoPopoverOpen(null), 200);
                          }}
                        >
                          <Info className="h-4 w-4 text-muted-foreground" />
                        </Button>
                      </PopoverTrigger>
                      <PopoverContent side="bottom" className="max-w-[320px] p-3 text-sm" onOpenAutoFocus={e => e.preventDefault()}>
                        Informe se profissionais de outros estados aptos a mudança podem participar da seleção para este perfil.
                      </PopoverContent>
                    </Popover>
                  </div>
                  <Select value={formData.locationId} onValueChange={value => handleInputChange('locationId', value)}>
                    <SelectTrigger className={fieldErrors.locationId ? 'border-destructive' : undefined}>
                      <SelectValue placeholder="Selecione" />
                    </SelectTrigger>
                    <SelectContent>
                      {locationOptions.map(option => (
                        <SelectItem key={option.id} value={option.id}>
                          {option.name}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </div>
              </div>

              {!isRemote && (
                <div className="grid gap-4 sm:grid-cols-3">
                  <div>
                    <Label>CEP</Label>
                    <Input
                      ref={cepRef}
                      placeholder="00000-000"
                      value={formData.cep}
                      onChange={e => {
                        const formatted = formatCep(e.target.value);
                        handleInputChange('cep', formatted);
                        if (formatted.replace(/\D/g, '').length === 8) {
                          void handleCepLookup(formatted);
                        }
                      }}
                      className={fieldErrors.cep ? 'border-destructive focus-visible:ring-destructive' : undefined}
                    />
                  </div>
                  <div>
                    <Label>Estado</Label>
                    <Input
                      value={formData.uf}
                      onChange={e => handleInputChange('uf', e.target.value)}
                      className={fieldErrors.uf ? 'border-destructive focus-visible:ring-destructive' : undefined}
                    />
                  </div>
                  <div>
                    <Label>Cidade</Label>
                    <div className="flex items-center gap-2">
                      <MapPin className="h-4 w-4 text-muted-foreground" />
                      <Input
                        value={formData.city}
                        onChange={e => handleInputChange('city', e.target.value)}
                        className={fieldErrors.city ? 'border-destructive focus-visible:ring-destructive' : undefined}
                      />
                    </div>
                  </div>
                </div>
              )}
            </CardContent>
          </Card>

          <Card className="border-borderSoft">
            <CardHeader>
              <CardTitle>Investimento</CardTitle>
            </CardHeader>
            <CardContent className="grid gap-4 sm:grid-cols-2">
              <div>
                <Label>Custo (R$)</Label>
                <Input
                  placeholder="R$ 0,00"
                  value={formData.cost}
                  onChange={e => handleInputChange('cost', formatarValorMonetario(e.target.value))}
                  className={fieldErrors.cost ? 'border-destructive focus-visible:ring-destructive' : undefined}
                />
              </div>
              <div>
                <Label>Ratecard (R$)</Label>
                <Input
                  placeholder="R$ 0,00"
                  value={formData.ratecard}
                  onChange={e => handleInputChange('ratecard', formatarValorMonetario(e.target.value))}
                />
              </div>
            </CardContent>
          </Card>

          <Card className="border-borderSoft">
            <CardHeader className="space-y-1">
              <div>
                <CardTitle>Configurações LinkedIn</CardTitle>
                <p className="text-sm text-muted-foreground">Metadados e informações para divulgação</p>
              </div>
            </CardHeader>
            <CardContent className="space-y-4">
              <div className="grid gap-4 sm:grid-cols-2">
                <div>
                  <Label>
                    Tipo de Emprego <span className="text-destructive">*</span>
                  </Label>
                  <Select value={formData.employmentTypeId} onValueChange={value => handleInputChange('employmentTypeId', value)}>
                    <SelectTrigger ref={employmentRef} className={fieldErrors.employmentTypeId ? 'border-destructive' : undefined}>
                      <SelectValue placeholder="Selecione" />
                    </SelectTrigger>
                    <SelectContent>
                      {employmentTypes.map(option => (
                        <SelectItem key={option.id} value={option.id}>
                          {option.name}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </div>
                <div>
                  <Label>
                    Nível de Experiência <span className="text-destructive">*</span>
                  </Label>
                  <Select value={formData.experienceLevelId} onValueChange={value => handleInputChange('experienceLevelId', value)}>
                    <SelectTrigger ref={experienceRef} className={fieldErrors.experienceLevelId ? 'border-destructive' : undefined}>
                      <SelectValue placeholder="Selecione" />
                    </SelectTrigger>
                    <SelectContent>
                      {experienceLevels.map(option => (
                        <SelectItem key={option.id} value={option.id}>
                          {option.name}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </div>
              </div>
              <div>
                <Label>Desafios do Perfil</Label>
                <Textarea
                  rows={4}
                  value={formData.responsibilities}
                  onChange={e => handleInputChange('responsibilities', e.target.value)}
                  placeholder="Ex: Conduzir discovery, priorizar backlog com o gestor, facilitar ritos ágeis e evoluir a experiência dos usuários."
                  className={fieldErrors.responsibilities ? 'border-destructive focus-visible:ring-destructive' : undefined}
                />
              </div>
              <div className="space-y-2">
                <div className="flex items-center justify-between">
                  <Label className="mb-0">Informações para LinkedIn</Label>
                  <Button type="button" variant="ghost" onClick={handleFillLinkedinTemplate}>
                    <Save className="mr-2 h-4 w-4" />
                    Atualizar com Template
                  </Button>
                </div>
                <Textarea
                  ref={linkedinRef}
                  rows={linkedinRows}
                  value={formData.linkedinInfo}
                  onChange={e => handleInputChange('linkedinInfo', e.target.value)}
                  placeholder="Texto para divulgação no LinkedIn..."
                  className="font-mono text-sm"
                />
              </div>
            </CardContent>
          </Card>
        </div>

        <div className="space-y-4">
          <Card className="border-borderSoft">
            <CardHeader>
              <CardTitle>Skills</CardTitle>
            </CardHeader>
            <CardContent className="space-y-3" ref={skillsRef}>
              <p className="text-xs text-muted-foreground flex items-center gap-1.5">
                <Info className="h-3.5 w-3.5 shrink-0" />
                Ao clicar na estrela, você alterna entre habilidade imprescindível (destacada) e desejável (normal).
              </p>
              {skillTypes.map(type => (
                <div key={type}>{renderSkillSection(type)}</div>
              ))}
            </CardContent>
          </Card>
        </div>
      </div>

      <Dialog open={skillModalOpen} onOpenChange={setSkillModalOpen}>
        <DialogContent className="max-w-2xl bg-surfaceElevated border-borderSoft">
          <DialogHeader>
            <DialogTitle>Adicionar {skillTypeLabels[skillModalType]}</DialogTitle>
            <DialogDescription>Busque competências e selecione o nível de proficiência para adicionar ao perfil.</DialogDescription>
          </DialogHeader>
          <div className="space-y-3">
            <Input
              placeholder="Busque pelo menos 2 caracteres"
              value={skillSearch}
              onChange={e => setSkillSearch(e.target.value)}
            />
            <ScrollArea className="h-48 border rounded-md p-2">
              {skillLoading && (
                <div className="flex justify-center py-6">
<Spinner size={16} aria-hidden />
                </div>
              )}
              {!skillLoading && !skillHasSearched && <p className="text-sm text-muted-foreground">Digite para buscar.</p>}
              {!skillLoading && skillHasSearched && skillResults.length === 0 && (
                <div className="flex flex-col gap-2">
                  <p className="text-sm text-muted-foreground">Nenhuma habilidade encontrada.</p>
                  {skillSearch.trim().length >= 2 && (
                    <Button
                      type="button"
                      variant="secondary"
                      size="sm"
                      onClick={handleCreateSkillFromSearch}
                      className="w-fit"
                    >
                      Criar habilidade
                    </Button>
                  )}
                </div>
              )}
              {!skillLoading && skillResults.length > 0 && (
                <div className="space-y-1">
                  {skillResults.map(skill => (
                    <button
                      key={skill.id}
                      className="w-full text-left px-2 py-1.5 rounded-md hover:bg-muted"
                      onClick={() => handleSelectSkill(skill)}
                    >
                      {skill.name}
                    </button>
                  ))}
                </div>
              )}
            </ScrollArea>
            {selectedSkill ? (
              <div className="space-y-2">
                <Separator />
                <p className="text-sm font-medium">
                  Selecione o nível para: <span className="text-accent">{selectedSkill.name}</span>
                </p>
                <div className="flex flex-wrap gap-3">
{skillLevelsLoading && <Spinner size={16} aria-hidden />}
                  {!skillLevelsLoading &&
                    skillLevels.map(level => {
                      const selected = selectedLevelId === String(level.id);
                      return (
                        <button
                          key={level.id}
                          type="button"
                          onClick={() => handleSelectLevel(String(level.id))}
                          className={`inline-flex items-center gap-2 rounded-full border px-3 py-1 text-sm font-medium transition ${
                            selected
                              ? 'border-primary text-primary bg-primary/10'
                              : 'border-input text-foreground hover:border-primary/60'
                          }`}
                          aria-pressed={selected}
                        >
                          <span
                            className={`inline-block h-4 w-4 rounded-full border ${
                              selected ? 'border-primary bg-primary' : 'border-muted-foreground'
                            }`}
                          />
                          {level.descricao}
                        </button>
                      );
                    })}
                </div>
              </div>
            ) : null}

            <div className="space-y-2">
              <Separator />
              <Alert className="border-info/50 bg-info/5 py-2">
                <Info className="h-4 w-4" />
                <AlertDescription>
                  <span className="block">Ao clicar na estrela, você alterna entre habilidade imprescindível (destacada) e desejável (normal).</span>
                  <div className="mt-2 flex flex-wrap items-center gap-2 text-xs text-muted-foreground">
                    <span>Exemplo:</span>
                    <Badge
                      variant="secondary"
                      className="inline-flex items-center gap-2 rounded-full border border-primary bg-btnPrimary px-3 py-2 text-inverseText"
                    >
                      <StarFilled className="h-3.5 w-3.5 shrink-0" aria-hidden />
                      <span>Habilidade imprescindível</span>
                    </Badge>
                    <span className="text-muted-foreground">&gt;</span>
                    <Badge
                      variant="secondary"
                      className="inline-flex items-center gap-2 rounded-full border border-borderDefault bg-btnSecondary px-3 py-2 text-btnSecondaryText"
                    >
                      <Star className="h-3.5 w-3.5 shrink-0" aria-hidden />
                      <span>Habilidade desejável</span>
                    </Badge>
                  </div>
                </AlertDescription>
              </Alert>
              <p className="text-sm font-medium text-foreground">Habilidades selecionadas</p>
              <div className="flex flex-wrap gap-2">
                {pendingSkills.map(skill => {
                  const label = skill.levelName ? `${skill.name} - ${skill.levelName}` : skill.name;
                  const pendingHasLevelError = !skill.levelName?.trim() || skill.levelName?.trim().toLowerCase() === 'a definir';
                  const badgeKey = skill.id === '0' ? `0-${skill.name}` : skill.id;
                  const relevante = skill.relevante ?? true;
                  const badgeClass = pendingHasLevelError
                    ? 'border-2 border-destructive bg-surfaceSubtle text-primaryText'
                    : relevante
                      ? 'bg-btnPrimary text-inverseText border border-primary'
                      : 'bg-btnSecondary text-btnSecondaryText border border-borderDefault';
                  return (
                    <Badge
                      key={badgeKey}
                      variant="secondary"
                      data-relevante={relevante}
                      className={`group inline-flex max-w-[min(100%,280px)] min-w-0 shrink items-center gap-2 rounded-full border px-3 py-2 ${badgeClass}`}
                    >
                      <button
                        type="button"
                        onClick={() => handleTogglePendingSkillRelevante(skill.id)}
                        className="shrink-0 flex items-center justify-center rounded p-0.5 text-inherit hover:opacity-80 focus:outline-none focus:ring-2 focus:ring-ring focus:ring-offset-1"
                        aria-label={relevante ? 'Marcar como desejável' : 'Marcar como imprescindível'}
                        title={relevante ? 'Imprescindível (clique para desejável)' : 'Desejável (clique para imprescindível)'}
                      >
                        {relevante ? <StarFilled className="h-3.5 w-3.5" /> : <Star className="h-3.5 w-3.5" />}
                      </button>
                      <Tooltip>
                        <TooltipTrigger asChild>
                          <button
                            type="button"
                            onClick={() => openEditLevelModal(skill, 'modal')}
                            className="min-w-0 flex-1 truncate text-left font-inherit hover:underline focus:outline-none focus:ring-2 focus:ring-ring focus:ring-offset-1 rounded"
                            aria-label={`Editar nível de ${skill.name}`}
                          >
                            {label}
                          </button>
                        </TooltipTrigger>
                        <TooltipContent side="top" className="max-w-[min(320px,90vw)]">
                          {label} (clique para editar nível)
                        </TooltipContent>
                      </Tooltip>
                      <button
                        type="button"
                        onClick={() => handleRemovePendingSkill(skill.id, skill.name)}
                        className="shrink-0 flex items-center justify-center rounded p-0.5 text-inherit hover:opacity-80 focus:outline-none focus:ring-2 focus:ring-ring focus:ring-offset-1 group-data-[relevante=false]:hover:text-destructive"
                        aria-label={`Remover ${skill.name}`}
                      >
                        <Trash2 className="h-3.5 w-3.5" />
                      </button>
                    </Badge>
                  );
                })}
                {pendingSkills.length === 0 && <span className="text-xs text-muted-foreground">Nenhuma selecionada</span>}
              </div>
            </div>

            <div className="flex justify-end gap-2">
              <Button variant="outline" onClick={() => setSkillModalOpen(false)}>
                Cancelar
              </Button>
              <Button onClick={handleSavePendingSkills} disabled={pendingSkills.length === 0}>
                Confirmar {pendingSkills.length > 0 ? `(${pendingSkills.length})` : ''}
              </Button>
            </div>
          </div>
        </DialogContent>
      </Dialog>

      <Dialog open={!!editingSkillForLevel} onOpenChange={open => !open && closeEditLevelModal()}>
        <DialogContent className="max-w-lg bg-surfaceElevated border-borderSoft">
          <DialogHeader>
            <DialogTitle>Editar nível</DialogTitle>
            <DialogDescription>
              {editingSkillForLevel ? (
                <>
                  Altere o nível de proficiência para: <span className="text-accent font-medium">{editingSkillForLevel.skill.name}</span>
                </>
              ) : null}
            </DialogDescription>
          </DialogHeader>
          <div className="space-y-3">
            <p className="text-sm font-medium">Selecione o nível</p>
            <div className="flex flex-wrap gap-3">
              {skillLevelsLoading && <Spinner size={16} aria-hidden />}
              {!skillLevelsLoading &&
                skillLevels.map(level => {
                  const selected = editLevelSelectedId === String(level.id);
                  return (
                    <button
                      key={level.id}
                      type="button"
                      onClick={() => handleSelectLevelForEdit(String(level.id))}
                      className={`inline-flex items-center gap-2 rounded-full border px-3 py-1 text-sm font-medium transition ${
                        selected
                          ? 'border-primary text-primary bg-primary/10'
                          : 'border-input text-foreground hover:border-primary/60'
                      }`}
                      aria-pressed={selected}
                    >
                      <span
                        className={`inline-block h-4 w-4 rounded-full border ${
                          selected ? 'border-primary bg-primary' : 'border-muted-foreground'
                        }`}
                      />
                      {level.descricao}
                    </button>
                  );
                })}
            </div>
            <div className="flex justify-end gap-2 pt-2">
              <Button variant="outline" onClick={closeEditLevelModal}>
                Cancelar
              </Button>
              <Button
                onClick={handleUpdateSkillLevel}
                disabled={!editLevelSelectedId || skillLevels.every(l => String(l.id) !== editLevelSelectedId)}
              >
                Salvar
              </Button>
            </div>
          </div>
        </DialogContent>
      </Dialog>

      <Dialog open={summaryOpen} onOpenChange={setSummaryOpen}>
        <DialogContent className="max-w-3xl max-h-[90vh] overflow-y-auto">
          <DialogHeader>
            <DialogTitle>Resumo da Geração por IA</DialogTitle>
            <DialogDescription>Dados sugeridos pela IA para preenchimento do perfil.</DialogDescription>
          </DialogHeader>
          <div className="space-y-4">
            <div className="flex items-center justify-between rounded-lg border border-border px-3 py-2 bg-muted/40">
              <span className="text-sm font-medium">Completude do Perfil</span>
              <span className="text-sm font-semibold text-success">
                {aiSummary?.completude ? `${Math.round(aiSummary.completude)}%` : '—'}
              </span>
            </div>
            <div className="grid gap-3 sm:grid-cols-2">
              {[
                { label: 'Nome do Perfil', key: 'nome_perfil' },
                { label: 'Custo', key: 'custo_perfil' },
                { label: 'Ratecard', key: 'ratecard_perfil' },
                { label: 'Informações Relevantes', key: 'informacoes_relevantes' },
                { label: 'Permanência', key: 'permanencia' },
                { label: 'Modelo de Trabalho', key: 'modelo_trabalho' },
                { label: 'Localidade', key: 'localidade' },
                { label: 'Cidade', key: 'cidade' },
                { label: 'Estado', key: 'estado' },
                { label: 'Dias Presenciais', key: 'hibrido_dias' },
                { label: 'CEP', key: 'cep' },
              ].map(item => (
                <div key={item.key} className="rounded-lg border border-border p-3 bg-background">
                  <p className="text-xs text-muted-foreground">{item.label}</p>
                  <p className="text-sm font-medium">{aiSummary?.resumo?.[item.key] || '—'}</p>
                </div>
              ))}
            </div>
          </div>
        </DialogContent>
      </Dialog>

      <Dialog open={showTokenModal} onOpenChange={setShowTokenModal}>
        <DialogContent className="max-w-md">
          <DialogHeader>
            <DialogTitle>Token para IA (ambiente dedicado)</DialogTitle>
            <DialogDescription>Informe o Bearer Token para chamadas de IA em ambiente dedicado.</DialogDescription>
          </DialogHeader>
          <div className="space-y-3">
            <Label htmlFor="ia-token">Bearer Token</Label>
            <Textarea
              id="ia-token"
              value={iaToken}
              onChange={e => setIaToken(e.target.value)}
              placeholder="Cole o token para chamadas de IA"
              rows={4}
            />
            <div className="flex justify-end gap-2">
              <Button variant="outline" onClick={() => setShowTokenModal(false)}>
                Fechar
              </Button>
              <Button
                onClick={() => {
                  setIaToken(iaToken.trim());
                  setShowTokenModal(false);
                }}
              >
                Salvar
              </Button>
            </div>
            <p className="text-xs text-muted-foreground">
              Este token é usado apenas para a chamada de IA enquanto o endpoint estiver em outro ambiente.
            </p>
          </div>
        </DialogContent>
      </Dialog>

      <AILoadingModal open={aiLoading} />

      <div className="flex justify-end">
        <Button
          onClick={handleSave}
          disabled={submitting || optionsLoading || profileLoading}
        >
{(submitting || optionsLoading || profileLoading) ? <Spinner className="mr-2" size={16} aria-hidden /> : <Save className="h-4 w-4 mr-2" />}
          {novaVagaMode ? 'Salvar vaga' : 'Salvar perfil'}
        </Button>
      </div>
      </>
      )}
    </div>
    </TooltipProvider>
  );
};

export default CriarPerfilAtuacao;
