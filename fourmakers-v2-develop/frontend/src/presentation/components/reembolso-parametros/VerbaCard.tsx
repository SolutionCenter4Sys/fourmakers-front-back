import { Card, CardContent } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Switch } from "@/components/ui/switch";
import type { VerbaData } from "@shared/types/reembolso";

interface VerbaCardProps {
  verba: VerbaData;
  onUpdate: (data: Partial<VerbaData>) => void;
}

export const VerbaCard = ({ verba, onUpdate }: VerbaCardProps) => {
  return (
    <Card className="border-2">
      <CardContent className="pt-6">
        <div className="grid grid-cols-5 gap-4 items-start">
          <div className="flex items-center gap-3">
            <Switch 
              checked={verba.isActive}
              onCheckedChange={(checked) => onUpdate({ isActive: checked })}
            />
            <span className="text-sm font-medium">Ativa</span>
          </div>
          
          <div className="space-y-2">
            <Label className="text-xs">
              Tipo do custo*
              <span className="ml-1 inline-flex items-center justify-center w-4 h-4 rounded-full bg-muted text-xs">i</span>
            </Label>
            <Select 
              value={verba.tipoCusto}
              onValueChange={(value: "debito" | "credito" | "fixa" | "variavel") => onUpdate({ tipoCusto: value })}
            >
              <SelectTrigger>
                <SelectValue />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="debito">Débito</SelectItem>
                <SelectItem value="credito">Crédito</SelectItem>
                <SelectItem value="fixa">Despesa fixa</SelectItem>
                <SelectItem value="variavel">Despesa variável</SelectItem>
              </SelectContent>
            </Select>
          </div>
          
          <div className="space-y-2">
            <Label className="text-xs">Nome da categoria*</Label>
            <Input 
              value={verba.nomeCategoria}
              onChange={(e) => onUpdate({ nomeCategoria: e.target.value })}
              className="bg-blue-50 dark:bg-blue-950/20" 
            />
          </div>
          
          <div className="space-y-2">
            <Label className="text-xs">Unidade*</Label>
            <Input 
              value={verba.unidade}
              onChange={(e) => onUpdate({ unidade: e.target.value })}
              placeholder="Unidade" 
            />
          </div>
          
          <div className="space-y-2">
            <Label className="text-xs">
              {verba.tipoCusto === "fixa" || verba.tipoCusto === "variavel" 
                ? "Valor*" 
                : "Valor limite por despesa*"}
            </Label>
            <Input 
              type="number" 
              value={verba.valor}
              onChange={(e) => onUpdate({ valor: parseFloat(e.target.value) })}
            />
          </div>
          
          <div className="flex items-center gap-3 col-start-5">
            <Switch 
              checked={verba.isCustoCliente}
              onCheckedChange={(checked) => onUpdate({ isCustoCliente: checked })}
            />
            <span className="text-sm font-medium">Custo Cliente</span>
          </div>
        </div>
      </CardContent>
    </Card>
  );
};
