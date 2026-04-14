import { cn } from '@/lib/utils';

export interface CountBadgeProps {
  /** Quantidade a exibir (ex.: número de candidatos ou length de uma lista). */
  count: number;
  /** Classes adicionais. */
  className?: string;
}

/**
 * Badge de quantidade: exibe um número em formato de tag.
 * - count === 0: estilo disabled (muted).
 * - count > 0: destaque roxo com texto branco.
 * Reutilizável para ícone + tag (ex.: ícone de olho + número de inscritos).
 */
export function CountBadge({ count, className }: CountBadgeProps) {
  const isEmpty = count === 0;
  return (
    <span
      className={cn(
        'inline-flex min-w-[1.25rem] shrink-0 items-center justify-center rounded-full px-1.5 py-0.5 text-[11px] font-semibold tabular-nums',
        isEmpty
          ? 'bg-muted text-muted-foreground'
          : 'bg-[var(--color-accent)] text-white',
        className
      )}
      aria-label={isEmpty ? 'Nenhum item' : `${count} itens`}
    >
      {count}
    </span>
  );
}
