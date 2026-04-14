/**
 * Habilidades em formato expandível: mesmo estilo visual das Experiências
 * (card com ícone, nome da categoria, "XX habilidades", chevron).
 */

import { useState, useEffect, useRef } from 'react';
import {
  Code2,
  Globe,
  LayoutGrid,
  Heart,
  Languages,
  ChevronDown,
  ChevronUp,
} from 'lucide-react';
import { Badge } from '@/components/ui/badge';
import type {
  CompetenciaProfile360,
  DominioProfile360,
  MetodologiaProfile360,
  SoftSkillProfile360,
  IdiomaProfile360,
} from '@domain/entities/Profile360';

const CATEGORY_ICONS: Record<string, React.ComponentType<{ className?: string }>> = {
  competencias: Code2,
  dominios: Globe,
  metodologias: LayoutGrid,
  softskills: Heart,
  idiomas: Languages,
};

interface SkillsSectionExpandedProps {
  competencias?: CompetenciaProfile360[];
  dominios?: DominioProfile360[];
  metodologias?: MetodologiaProfile360[];
  softskills?: SoftSkillProfile360[];
  idiomas?: IdiomaProfile360[];
}

function buildCategories(props: SkillsSectionExpandedProps): Array<{
  key: string;
  label: string;
  count: number;
  items: Array<{ id: string | number; label: string }>;
}> {
  const out: Array<{
    key: string;
    label: string;
    count: number;
    items: Array<{ id: string | number; label: string }>;
  }> = [];

  if (props.competencias?.length) {
    out.push({
      key: 'competencias',
      label: 'Competências',
      count: props.competencias.length,
      items: props.competencias.map((c) => ({
        id: c.id,
        label: `${c.competencia.descricao}${c.nivel ? ` (${c.nivel.descricao})` : ''}`,
      })),
    });
  }
  if (props.dominios?.length) {
    out.push({
      key: 'dominios',
      label: 'Domínios',
      count: props.dominios.length,
      items: props.dominios.map((d) => ({
        id: d.id,
        label: `${d.dominio.descricao}${d.nivel ? ` (${d.nivel.descricao})` : ''}`,
      })),
    });
  }
  if (props.metodologias?.length) {
    out.push({
      key: 'metodologias',
      label: 'Metodologias',
      count: props.metodologias.length,
      items: props.metodologias.map((m, i) => ({
        id: m.metodologia?.id ?? i,
        label: `${m.metodologia?.descricao ?? ''}${m.nivel ? ` (${m.nivel.descricao})` : ''}`,
      })),
    });
  }
  if (props.softskills?.length) {
    out.push({
      key: 'softskills',
      label: 'Soft Skills',
      count: props.softskills.length,
      items: props.softskills.map((s) => ({
        id: s.id,
        label: `${s.softSkill.descricao}${s.nivel ? ` (${s.nivel.descricao})` : ''}`,
      })),
    });
  }
  if (props.idiomas?.length) {
    out.push({
      key: 'idiomas',
      label: 'Idiomas',
      count: props.idiomas.length,
      items: props.idiomas.map((i) => ({
        id: i.id,
        label: `${i.idioma.descricao}${i.nivel ? ` (${i.nivel.descricao})` : ''}`,
      })),
    });
  }

  return out;
}

export function SkillsSectionExpanded(props: SkillsSectionExpandedProps) {
  const list = buildCategories(props);
  const [expandedKeys, setExpandedKeys] = useState<Set<string>>(new Set());
  const initialExpandedDone = useRef(false);

  useEffect(() => {
    if (list.length > 0 && !initialExpandedDone.current) {
      initialExpandedDone.current = true;
      setExpandedKeys(new Set([list[0].key]));
    }
  }, [list]);

  const toggle = (key: string) => {
    setExpandedKeys((prev) => {
      const next = new Set(prev);
      if (next.has(key)) next.delete(key);
      else next.add(key);
      return next;
    });
  };

  if (list.length === 0) {
    return (
      <div className="py-4 text-center text-muted-foreground text-sm">
        Nenhuma habilidade cadastrada
      </div>
    );
  }

  return (
    <div className="space-y-4">
      <h2 className="text-xl font-semibold text-foreground">Habilidades</h2>
      <div className="space-y-3">
        {list.map((cat) => {
          const isExpanded = expandedKeys.has(cat.key);
          const Icon = CATEGORY_ICONS[cat.key] ?? Code2;
          const countLabel = cat.count === 1 ? '1 habilidade' : `${cat.count} habilidades`;

          return (
            <div
              key={cat.key}
              className="rounded-xl border bg-card transition-all hover:shadow-md"
            >
              <div
                className="flex items-center justify-between p-4 cursor-pointer"
                onClick={() => toggle(cat.key)}
                role="button"
                tabIndex={0}
                onKeyDown={(e) => (e.key === 'Enter' || e.key === ' ') && toggle(cat.key)}
              >
                <div className="flex items-center gap-3 flex-1 min-w-0">
                  <div className="flex h-10 w-10 flex-shrink-0 items-center justify-center rounded-lg bg-violet-100 dark:bg-violet-950">
                    <Icon className="h-5 w-5 text-violet-600 dark:text-violet-400" />
                  </div>
                  <div className="flex-1 min-w-0">
                    <h3 className="font-semibold text-foreground text-base">{cat.label}</h3>
                    <p className="text-sm text-muted-foreground mt-0.5">{countLabel}</p>
                  </div>
                </div>
                <div className="flex-shrink-0">
                  {isExpanded ? (
                    <ChevronUp className="h-5 w-5 text-muted-foreground" />
                  ) : (
                    <ChevronDown className="h-5 w-5 text-muted-foreground" />
                  )}
                </div>
              </div>
              {isExpanded && (
                <div className="px-4 pb-4 pt-0 border-t border-border/50">
                  <div className="flex flex-wrap gap-2 pt-3">
                    {cat.items.map((item) => (
                      <Badge
                        key={item.id}
                        variant="outline"
                        className="py-1.5 text-sm font-medium"
                      >
                        {item.label}
                      </Badge>
                    ))}
                  </div>
                </div>
              )}
            </div>
          );
        })}
      </div>
    </div>
  );
}
