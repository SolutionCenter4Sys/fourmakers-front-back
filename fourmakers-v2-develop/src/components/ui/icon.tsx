import type * as React from "react";

import { cn } from "@/lib/utils";

export type IconVariant = "default" | "outlined";

export type MaterialSymbolAxis = {
  fill?: 0 | 1;
  wght?: number;
  opsz?: number;
  grad?: number;
};

export type IconProps = Omit<React.HTMLAttributes<HTMLSpanElement>, "title"> & {
  codigoIcone?: string | null;
  name?: string | null;
  variant?: IconVariant;
  size?: number;
  axes?: MaterialSymbolAxis;
  fallbackName?: string;
  title?: string;
};

const normalizeCodigoIcone = (codigoIcone: string | null | undefined) => (codigoIcone ?? "").trim();

const parseIcon = (codigoIcone: string): { name: string; variant: IconVariant } => {
  const trimmed = normalizeCodigoIcone(codigoIcone);
  if (!trimmed) return { name: "", variant: "default" };

  if (trimmed.endsWith("_outlined")) {
    return { name: trimmed.replace(/_outlined$/, ""), variant: "outlined" };
  }

  // Backend pode enviar `_rounded`; no front usamos a família default (sem carregar Round).
  if (trimmed.endsWith("_rounded")) {
    return { name: trimmed.replace(/_rounded$/, ""), variant: "default" };
  }

  return { name: trimmed, variant: "default" };
};

const isValidLigatureName = (name: string) => /^[a-z0-9_]+$/i.test(name);

const tailwindSpacingUnitPx = 4;

const parseTailwindSizeToken = (token: string) => {
  const bracketed = token.match(/^(?:h|w|size)-\[(?<value>-?\d+(?:\.\d+)?)(?<unit>px|rem)\]$/);
  if (bracketed?.groups?.value && bracketed.groups.unit) {
    const value = Number(bracketed.groups.value);
    if (Number.isNaN(value)) return undefined;
    return bracketed.groups.unit === "px" ? value : value * 16;
  }

  const numeric = token.match(/^(?:h|w|size)-(?<value>\d+(?:\.\d+)?)$/);
  if (numeric?.groups?.value) {
    const value = Number(numeric.groups.value);
    if (Number.isNaN(value)) return undefined;
    return value * tailwindSpacingUnitPx;
  }

  return undefined;
};

const inferSizeFromClassName = (className?: string) => {
  if (!className) return undefined;
  const tokens = className.split(/\s+/).filter(Boolean);
  const sizes = tokens.map(parseTailwindSizeToken).filter((value): value is number => typeof value === "number");
  return sizes.length ? Math.max(...sizes) : undefined;
};

export const Icon = ({
  codigoIcone,
  name,
  variant,
  size,
  axes,
  fallbackName = "help",
  className,
  title,
  style,
  ...props
}: IconProps) => {
  const parsed = parseIcon(codigoIcone ?? "");
  const resolvedVariant = variant ?? parsed.variant;
  const resolvedName = (name ?? parsed.name).trim();
  const finalName = resolvedName && isValidLigatureName(resolvedName) ? resolvedName : fallbackName;

  const fillByVariant: 0 | 1 = resolvedVariant === "outlined" ? 0 : 1;
  const finalAxes: Required<MaterialSymbolAxis> = {
    fill: axes?.fill ?? fillByVariant,
    wght: axes?.wght ?? 400,
    opsz: axes?.opsz ?? 20,
    grad: axes?.grad ?? 0,
  };

  const resolvedSize = size ?? inferSizeFromClassName(className) ?? 20;

  return (
    <span
      {...props}
      className={cn("material-symbols-outlined inline-flex items-center justify-center notranslate", className)}
      style={{
        fontSize: resolvedSize,
        lineHeight: 1,
        fontVariationSettings: `"FILL" ${finalAxes.fill}, "wght" ${finalAxes.wght}, "GRAD" ${finalAxes.grad}, "opsz" ${finalAxes.opsz}`,
        fontFeatureSettings: "normal",
        ...style,
      }}
      translate="no"
      suppressHydrationWarning
      aria-hidden
      title={title ?? finalName}
    >
      {finalName}
    </span>
  );
};
