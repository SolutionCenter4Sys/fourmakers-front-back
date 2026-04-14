import { Button } from '@/components/ui/button';
import { HelpCircle } from 'lucide-react';

interface TourButtonProps {
  onStart: () => void;
  label?: string;
  'data-testid'?: string;
}

/**
 * Botão flutuante que inicia o guide tour da página atual.
 * Posicionado no canto superior direito do container.
 */
export function TourButton({ onStart, label = 'Tour guiado', 'data-testid': testId }: TourButtonProps) {
  return (
    <Button
      variant="outline"
      size="sm"
      onClick={onStart}
      className="gap-2"
      data-testid={testId ?? 'tour-button'}
    >
      <HelpCircle className="h-4 w-4" />
      {label}
    </Button>
  );
}
