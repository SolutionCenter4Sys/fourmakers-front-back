import { useState, useEffect } from "react";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Checkbox } from "@/components/ui/checkbox";
import { Button } from "@/components/ui/button";
import { VerbaCard } from "./VerbaCard";
import type { VerbaData } from "@shared/types/reembolso";
import { useReembolsoVerbasInicial } from "@/hooks/useReembolsoParametros";

export const RegrasTab = () => {
  const { verbas: verbasInicial } = useReembolsoVerbasInicial();
  const [verbas, setVerbas] = useState<VerbaData[]>([]);

  useEffect(() => {
    if (verbasInicial.length > 0) {
      setVerbas(verbasInicial);
    }
  }, [verbasInicial]);

  const updateVerba = (index: number, updates: Partial<VerbaData>) => {
    setVerbas(prev => prev.map((verba, i) => 
      i === index ? { ...verba, ...updates } : verba
    ));
  };

  return (
    <div className="space-y-8">
      {/* Regras Gerais */}
      <Card>
        <CardHeader>
          <CardTitle>Regras gerais</CardTitle>
          <CardDescription>
            Insira os dias para definir prazos de envio, pagamento e validade dos comprovantes.
          </CardDescription>
        </CardHeader>
        <CardContent>
          <div className="grid grid-cols-3 gap-6">
            {/* Primeira coluna */}
            <div className="space-y-6">
              <div className="space-y-2">
                <Label className="flex items-center gap-2">
                  <span className="inline-block w-2 h-2 rounded-full bg-blue-500"></span>
                  Dia limite para o envio<span className="text-destructive">*</span>
                </Label>
                <Input type="number" defaultValue="20" />
              </div>
              
              <div className="space-y-2">
                <Label className="flex items-center gap-2">
                  <span className="inline-block w-2 h-2 rounded-full bg-green-500"></span>
                  Dia de pagamento<span className="text-destructive">*</span>
                </Label>
                <Input type="number" defaultValue="30" />
              </div>
            </div>

            {/* Segunda coluna */}
            <div className="space-y-6">
              <div className="space-y-2">
                <Label className="flex items-center gap-2">
                  <span className="inline-block w-2 h-2 rounded-full bg-blue-500"></span>
                  Dia alternativo para o envio
                </Label>
                <Input type="number" defaultValue="0" />
              </div>
              
              <div className="space-y-2">
                <Label className="flex items-center gap-2">
                  <span className="inline-block w-2 h-2 rounded-full bg-green-500"></span>
                  Dia alternativo de pagamento
                </Label>
                <Input type="number" defaultValue="0" />
              </div>
            </div>

            {/* Terceira coluna */}
            <div className="space-y-2">
              <Label>Validade de comprovantes (dias corridos)</Label>
              <Input type="number" defaultValue="0" />
            </div>
          </div>

          <div className="mt-8 pt-8 border-t">
            <div className="space-y-4">
              <h3 className="text-lg font-semibold">Aprovadores</h3>
              <p className="text-sm text-muted-foreground">Parâmetros de aprovações</p>
              
              <div className="flex items-center space-x-2">
                <Checkbox id="aprovador-proprio" />
                <label
                  htmlFor="aprovador-proprio"
                  className="text-sm font-medium leading-none peer-disabled:cursor-not-allowed peer-disabled:opacity-70"
                >
                  Concede ao aprovador a permissão para aprovar suas próprias solicitações
                </label>
              </div>
            </div>
          </div>
        </CardContent>
      </Card>

      {/* Verbas */}
      <Card>
        <CardHeader className="flex flex-row items-center justify-between">
          <div>
            <CardTitle>Verbas</CardTitle>
            <CardDescription>
              Você pode criar as verbas e definir os parâmetros nesta seção.
            </CardDescription>
          </div>
          <Button className="bg-primary hover:bg-primary/90">Nova verba</Button>
        </CardHeader>
        <CardContent>
          <div className="space-y-4">
            {verbas.map((verba, index) => (
              <VerbaCard 
                key={verba.id}
                verba={verba}
                onUpdate={(updates) => updateVerba(index, updates)}
              />
            ))}
          </div>
        </CardContent>
      </Card>
    </div>
  );
};
