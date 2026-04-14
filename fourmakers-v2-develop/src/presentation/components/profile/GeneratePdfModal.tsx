import { useMemo, useState } from 'react';
import { Button } from '@/components/ui/button';
import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { Label } from '@/components/ui/label';
import { RadioGroup, RadioGroupItem } from '@/components/ui/radio-group';
import { Download, X } from '@/components/ui/system-icons';
import type { ProfilePdfNameMode } from '@shared/utils/generateProfile360PDF';

interface GeneratePdfModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onDownload: (nameMode: ProfilePdfNameMode) => void;
}

const DEFAULT_MODE: ProfilePdfNameMode = 'name_with_surname_initials';

export function GeneratePdfModal({ open, onOpenChange, onDownload }: GeneratePdfModalProps) {
  const [nameMode, setNameMode] = useState<ProfilePdfNameMode>(DEFAULT_MODE);

  const modeOptions = useMemo(
    () => [
      { id: 'pdf-name-default', value: 'name_with_surname_initials' as const, label: 'Nome + Iniciais do sobrenome' },
      { id: 'pdf-name-full', value: 'full_name' as const, label: 'Nome completo' },
      { id: 'pdf-name-initials', value: 'initials_only' as const, label: 'Somente Iniciais' },
    ],
    [],
  );

  return (
    <Dialog
      open={open}
      onOpenChange={(nextOpen) => {
        if (!nextOpen) setNameMode(DEFAULT_MODE);
        onOpenChange(nextOpen);
      }}
    >
      <DialogContent className="max-w-lg p-0 gap-0 [&>button]:hidden">
        <DialogHeader className="flex flex-row items-start gap-4 px-6 py-5 border-b">
          <div className="flex h-12 w-12 shrink-0 items-center justify-center rounded-lg bg-muted">
            <Download className="h-6 w-6 text-primaryText" />
          </div>
          <div className="flex-1">
            <DialogTitle className="text-xl font-semibold">Gerar PDF</DialogTitle>
            <DialogDescription className="sr-only">Escolha como deseja exibir o nome do profissional no currículo em PDF.</DialogDescription>
          </div>
          <Button
            type="button"
            variant="ghost"
            size="icon"
            className="h-9 w-9"
            aria-label="Fechar"
            onClick={() => onOpenChange(false)}
          >
            <X className="h-5 w-5" />
          </Button>
        </DialogHeader>

        <div className="px-6 py-6">
          <h3 className="text-lg leading-tight font-semibold text-primaryText">
            De que forma você deseja exibir o nome do profissional?
          </h3>

          <RadioGroup
            value={nameMode}
            onValueChange={(value) => setNameMode(value as ProfilePdfNameMode)}
            className="mt-5 space-y-3"
          >
            {modeOptions.map((option) => (
              <div key={option.id} className="flex items-center gap-3">
                <RadioGroupItem value={option.value} id={option.id} />
                <Label htmlFor={option.id} className="cursor-pointer text-base font-medium text-primaryText">
                  {option.label}
                </Label>
              </div>
            ))}
          </RadioGroup>
        </div>

        <div className="px-6 pb-6 flex justify-end">
          <Button type="button" className="min-w-32" onClick={() => onDownload(nameMode)}>
            Download
          </Button>
        </div>
      </DialogContent>
    </Dialog>
  );
}
