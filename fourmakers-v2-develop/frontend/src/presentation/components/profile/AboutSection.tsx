import { Edit2, User } from "@/components/ui/system-icons";
import { Button } from "@/components/ui/button";
import { Textarea } from "@/components/ui/textarea";
import { useState, useEffect } from "react";

interface AboutSectionProps {
  sobre?: string;
  readOnly?: boolean;
}

export const AboutSection = ({ sobre = "", readOnly = false }: AboutSectionProps) => {
  const [isEditing, setIsEditing] = useState(false);
  const [about, setAbout] = useState(sobre);

  // Atualizar quando o prop mudar
  useEffect(() => {
    if (sobre) {
      setAbout(sobre);
    }
  }, [sobre]);

  return (
    <div className="space-y-3">
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-2">
          <User className="h-5 w-5 text-muted-foreground" />
          <h2 className="text-xl font-semibold text-foreground">Sobre</h2>
        </div>
        {!readOnly && (
          <Button
            variant="ghost"
            size="sm"
            onClick={() => setIsEditing(!isEditing)}
            className="h-8"
          >
            <Edit2 className="h-3.5 w-3.5" />
          </Button>
        )}
      </div>
      
      {isEditing && !readOnly ? (
        <div className="space-y-3">
          <Textarea
            value={about}
            onChange={(e) => setAbout(e.target.value)}
            className="min-h-[100px] resize-none text-base"
            placeholder="Conte um pouco sobre você..."
          />
          <div className="flex justify-end gap-2">
            <Button
              variant="outline"
              size="sm"
              onClick={() => setIsEditing(false)}
            >
              Cancelar
            </Button>
            <Button size="sm" onClick={() => setIsEditing(false)}>
              Salvar
            </Button>
          </div>
        </div>
      ) : (
        <p className="text-base text-muted-foreground leading-relaxed whitespace-pre-wrap">
          {about || "Nenhuma informação disponível."}
        </p>
      )}
    </div>
  );
};
