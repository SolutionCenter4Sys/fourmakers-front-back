import { useCallback, useMemo, useRef, useState } from 'react';
import type { PerfilAtuacaoFormData } from '@domain/entities/PerfilAtuacao';

interface Params {
  formData: PerfilAtuacaoFormData;
  fieldErrors: Record<string, boolean>;
  handleSubmit: () => Promise<{ success: boolean; missing?: string[] } | undefined>;
  workModels: { id: string; type?: string }[];
}

export const usePerfilAtuacaoPage = ({ formData, fieldErrors, handleSubmit, workModels }: Params) => {
  const nameRef = useRef<HTMLInputElement>(null);
  const managerRef = useRef<HTMLButtonElement>(null);
  const employmentRef = useRef<HTMLButtonElement>(null);
  const experienceRef = useRef<HTMLButtonElement>(null);
  const cepRef = useRef<HTMLInputElement>(null);
  const linkedinRef = useRef<HTMLTextAreaElement>(null);
  const skillsRef = useRef<HTMLDivElement>(null);
  const [isClientPopoverOpen, setIsClientPopoverOpen] = useState(false);
  const [isManagerPopoverOpen, setIsManagerPopoverOpen] = useState(false);

  const linkedinRows = useMemo(() => Math.max(5, formData.linkedinInfo.split('\n').length + 1), [formData.linkedinInfo]);
  const selectedWorkModel = useMemo(() => workModels.find(w => w.id === formData.workModelId), [workModels, formData.workModelId]);
  const isHybrid = selectedWorkModel?.type === 'hibrido';
  const isRemote = selectedWorkModel?.type === 'remoto';

  const skillsByType = useMemo(
    () => ({
      COMPETENCIA: formData.hardSkills,
      SOFTSKILL: formData.softSkills,
      METODOLOGIA: formData.methodologies,
      DOMINIONEGOCIO: formData.businessDomains,
      IDIOMA: formData.languages,
    }),
    [formData.businessDomains, formData.hardSkills, formData.languages, formData.methodologies, formData.softSkills],
  );

  const scrollToFirstError = useCallback((missing: string[]) => {
    const first = missing[0];
    const target =
      first === 'name'
        ? nameRef.current
        : first === 'managerCode'
          ? managerRef.current
          : first === 'employmentTypeId'
            ? employmentRef.current
            : first === 'experienceLevelId'
              ? experienceRef.current
              : first === 'cep'
                ? cepRef.current
                : first === 'hardSkills' || first === 'softSkills'
                  ? skillsRef.current
                  : linkedinRef.current;
    if (target?.scrollIntoView) {
      target.scrollIntoView({ behavior: 'smooth', block: 'center' });
      target.focus?.();
    }
  }, []);

  const handleSubmitWithScroll = useCallback(async () => {
    const result = await handleSubmit();
    if (result?.missing?.length) {
      scrollToFirstError(result.missing);
    }
    return result;
  }, [handleSubmit, scrollToFirstError]);

  return {
    fieldErrors,
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
  };
};
