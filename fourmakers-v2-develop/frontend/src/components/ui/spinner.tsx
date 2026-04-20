import * as React from "react";

import { cn } from "@/lib/utils";

export type SpinnerProps = React.SVGAttributes<SVGSVGElement> & {
  size?: number;
};

export function Spinner({ className, size = 24, ...props }: SpinnerProps) {
  return (
    <svg
      viewBox="0 0 24 24"
      width={size}
      height={size}
      className={cn("animate-spin", className)}
      fill="none"
      role="status"
      aria-label="Carregando"
      {...props}
    >
      <circle
        cx="12"
        cy="12"
        r="9"
        stroke="currentColor"
        strokeWidth="3"
        opacity="0.2"
      />
      <path
        d="M21 12a9 9 0 0 0-9-9"
        stroke="currentColor"
        strokeWidth="3"
        strokeLinecap="round"
      />
    </svg>
  );
}

