import { Card, CardContent } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Search } from "@/components/ui/system-icons";

interface SearchCardProps {
  searchTerm: string;
  onSearchChange: (value: string) => void;
  placeholder?: string;
  actionButton?: React.ReactNode;
}

export const SearchCard = ({ 
  searchTerm, 
  onSearchChange, 
  placeholder = "Buscar...",
  actionButton 
}: SearchCardProps) => {
  return (
    <Card>
      <CardContent className="p-6">
        <div className="flex items-center justify-between gap-4">
          <div className="relative flex-1 max-w-md">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
            <Input
              placeholder={placeholder}
              value={searchTerm}
              onChange={(e) => onSearchChange(e.target.value)}
              className="pl-10"
            />
          </div>
          {actionButton && actionButton}
        </div>
      </CardContent>
    </Card>
  );
};
