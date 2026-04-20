import { Code, Heart, Globe, Layers, Briefcase } from "@/components/ui/system-icons";
import { Badge } from "@/components/ui/badge";
import type { CompetenciaProfile360, DominioProfile360, MetodologiaProfile360, SoftSkillProfile360, IdiomaProfile360 } from "@domain/entities/Profile360";

interface SkillsSectionProps {
  competencias?: CompetenciaProfile360[];
  dominios?: DominioProfile360[];
  metodologias?: MetodologiaProfile360[];
  softskills?: SoftSkillProfile360[];
  idiomas?: IdiomaProfile360[];
}

export const SkillsSection = ({ 
  competencias = [], 
  dominios = [], 
  metodologias = [],
  softskills = [],
  idiomas = []
}: SkillsSectionProps) => {
  return (
    <div className="space-y-5">
      <h2 className="text-xl font-semibold text-foreground">Habilidades</h2>
      
      <div className="space-y-5">
        {/* Competências (Hard Skills) */}
        {competencias.length > 0 && (
          <div className="space-y-2.5">
            <div className="flex items-center gap-2">
              <Code className="h-4 w-4 text-muted-foreground" />
              <h4 className="text-sm font-medium text-foreground">Competências</h4>
            </div>
            <div className="flex flex-wrap gap-2">
              {competencias.map((comp) => (
                <Badge
                  key={comp.id}
                  variant="outline"
                  className="py-1.5 text-base font-medium"
                >
                  {comp.competencia.descricao} {comp.nivel && `(${comp.nivel.descricao})`}
                </Badge>
              ))}
            </div>
          </div>
        )}

        {/* Domínios */}
        {dominios.length > 0 && (
          <div className="space-y-2.5">
            <div className="flex items-center gap-2">
              <Layers className="h-4 w-4 text-muted-foreground" />
              <h4 className="text-sm font-medium text-foreground">Domínios</h4>
            </div>
            <div className="flex flex-wrap gap-2">
              {dominios.map((dom) => (
                <Badge
                  key={dom.id}
                  variant="outline"
                  className="py-1.5 text-base font-medium"
                >
                  {dom.dominio.descricao} {dom.nivel && `(${dom.nivel.descricao})`}
                </Badge>
              ))}
            </div>
          </div>
        )}

        {/* Metodologias */}
        {metodologias.length > 0 && (
          <div className="space-y-2.5">
            <div className="flex items-center gap-2">
              <Briefcase className="h-4 w-4 text-muted-foreground" />
              <h4 className="text-sm font-medium text-foreground">Metodologias</h4>
            </div>
            <div className="flex flex-wrap gap-2">
              {metodologias.map((met, idx) => (
                <Badge
                  key={met.metodologia.id || idx}
                  variant="outline"
                  className="py-1.5 text-base font-medium"
                >
                  {met.metodologia.descricao} {met.nivel && `(${met.nivel.descricao})`}
                </Badge>
              ))}
            </div>
          </div>
        )}

        {/* Soft Skills */}
        {softskills.length > 0 && (
          <div className="space-y-2.5">
            <div className="flex items-center gap-2">
              <Heart className="h-4 w-4 text-muted-foreground" />
              <h4 className="text-sm font-medium text-foreground">Soft Skills</h4>
            </div>
            <div className="flex flex-wrap gap-2">
              {softskills.map((soft) => (
                <Badge
                  key={soft.id}
                  variant="outline"
                  className="py-1.5 text-base font-medium"
                >
                  {soft.softSkill.descricao} {soft.nivel && `(${soft.nivel.descricao})`}
                </Badge>
              ))}
            </div>
          </div>
        )}

        {/* Idiomas */}
        {idiomas.length > 0 && (
          <div className="space-y-2.5">
            <div className="flex items-center gap-2">
              <Globe className="h-4 w-4 text-muted-foreground" />
              <h4 className="text-sm font-medium text-foreground">Idiomas</h4>
            </div>
            <div className="flex flex-wrap gap-2">
              {idiomas.map((idioma) => (
                <Badge
                  key={idioma.id}
                  variant="outline"
                  className="py-1.5 text-base font-medium"
                >
                  {idioma.idioma.descricao} {idioma.nivel && `(${idioma.nivel.descricao})`}
                </Badge>
              ))}
            </div>
          </div>
        )}

        {competencias.length === 0 && dominios.length === 0 && metodologias.length === 0 && softskills.length === 0 && idiomas.length === 0 && (
          <div className="text-center py-4 text-muted-foreground">Nenhuma habilidade cadastrada</div>
        )}
      </div>
    </div>
  );
};
