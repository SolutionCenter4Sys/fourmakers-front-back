import { Card, CardContent } from "@/components/ui/card";
import type { LucideIcon } from "@/components/ui/system-icons";
import { cn } from "@/lib/utils";

interface StatCardProps {
  title: string;
  value: string;
  icon: LucideIcon;
  color: string;
  bgColor: string;
  description?: string;
  subDescription?: string;
}

export const StatCard = ({ 
  title, 
  value, 
  icon: Icon, 
  color, 
  bgColor,
  description,
  subDescription,
  vertical = false
}: StatCardProps & { vertical?: boolean }) => {
  if (vertical) {
    return (
      <Card>
        <CardContent className="p-4">
          <div className="flex items-center gap-4">
            {/* Coluna menor com ícone */}
            <div className={cn("p-3 rounded-lg flex-shrink-0", bgColor)}>
              <Icon className={cn("h-6 w-6", color)} />
            </div>
            {/* Coluna maior com número e nome */}
            <div className="flex-1 min-w-0">
              <p className={cn("text-2xl font-bold mb-1", color)}>
                {value}
              </p>
              <p className="text-sm font-medium text-muted-foreground leading-tight">
                {title}
              </p>
              {description && (
                <p className="text-sm text-muted-foreground mt-1">{description}</p>
              )}
              {subDescription && (
                <p className="text-xs text-muted-foreground mt-0.5">{subDescription}</p>
              )}
            </div>
          </div>
        </CardContent>
      </Card>
    );
  }

  return (
    <Card>
      <CardContent className="p-6">
        <div className="flex items-center justify-between">
          <div className="flex-1">
            <p className="text-sm font-medium text-muted-foreground mb-1">
              {title}
            </p>
            <p className="text-2xl font-bold text-foreground mb-1">
              {value}
            </p>
            {description && (
              <p className="text-sm text-muted-foreground">{description}</p>
            )}
            {subDescription && (
              <p className="text-xs text-muted-foreground mt-0.5">{subDescription}</p>
            )}
          </div>
          <div className={cn("p-3 rounded-lg", bgColor)}>
            <Icon className={cn("h-6 w-6", color)} />
          </div>
        </div>
      </CardContent>
    </Card>
  );
};
