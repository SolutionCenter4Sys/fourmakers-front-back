import * as React from 'react';
import { cn } from '@/lib/utils';

export interface BellGearIconProps extends React.SVGAttributes<SVGSVGElement> {
  size?: number;
  className?: string;
}

/**
 * Ícone combinado sino + engrenagem para "Configurações de Notificações".
 * Engrenagem no canto superior esquerdo integrada ao sino.
 */
export function BellGearIcon({ size = 24, className, ...props }: BellGearIconProps) {
  return (
    <svg
      width={size}
      height={size}
      viewBox="0 0 24 24"
      fill="none"
      stroke="currentColor"
      strokeWidth="1.5"
      strokeLinecap="round"
      strokeLinejoin="round"
      className={cn('shrink-0', className)}
      aria-hidden
      {...props}
    >
      {/* Engrenagem (canto superior esquerdo) */}
      <circle cx="7.2" cy="7.2" r="3" />
      <path d="M7.2 4.2v.8M7.2 10.2v.8M4.2 7.2h.8M10.2 7.2h.8M5.1 5.1l.55.55M9.3 9.3l.55.55M5.1 9.3l.55-.55M9.3 5.1l.55-.55" />
      {/* Sino */}
      <path d="M12 5.8c-2 0-3.6 1.5-3.6 3.4v2.2c0 .8-.4 1.5-.9 2v1.2h9v-1.2c-.5-.5-.9-1.2-.9-2V9.2c0-1.9-1.6-3.4-3.6-3.4z" />
      <path d="M9.5 18.2h5" />
      <path d="M12 18.2v.6" />
    </svg>
  );
}
