import { useState } from "react";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Switch } from "@/components/ui/switch";
import { Search, X, ChevronDown, ChevronUp } from "@/components/ui/system-icons";
import { Badge } from "@/components/ui/badge";
import { Collapsible, CollapsibleContent, CollapsibleTrigger } from "@/components/ui/collapsible";

export const RegrasPersonalizadasTab = () => {
  const [selectedColaboradores, setSelectedColaboradores] = useState([
    "Doug Strickland",
    "Luciana Ottati Freitas"
  ]);
  const [searchColaborador, setSearchColaborador] = useState("");
  const [ativarTodasVerbas, setAtivarTodasVerbas] = useState(false);
  const [openVerbas, setOpenVerbas] = useState<{ [key: string]: boolean }>({
    devolucao: true,
    adiantamento: true,
  });

  const removeColaborador = (name: string) => {
    setSelectedColaboradores(selectedColaboradores.filter(c => c !== name));
  };

  const toggleVerba = (verba: string) => {
    setOpenVerbas(prev => ({ ...prev, [verba]: !prev[verba] }));
  };

  return (
    <div className="space-y-8">
      {/* Filtros */}
      <Card>
        <CardHeader>
          <CardTitle>Filtros</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="grid grid-cols-3 gap-4">
            <div className="space-y-2">
              <Label>Cliente</Label>
              <Input placeholder="Selecione o cliente" />
            </div>
            
            <div className="space-y-2">
              <Label>Projeto</Label>
              <Input placeholder="Selecione o projeto" />
            </div>
            
            <div className="flex items-end gap-2">
              <Button variant="outline">Limpar</Button>
              <Button className="bg-primary hover:bg-primary/90">Buscar</Button>
            </div>
          </div>
        </CardContent>
      </Card>

      {/* Regras Personalizadas */}
      <Card>
        <CardHeader>
          <CardTitle>Regras personalizadas</CardTitle>
          <CardDescription>
            Você pode definir as regras para cada cliente ou projeto
          </CardDescription>
        </CardHeader>
        <CardContent className="space-y-8">
          {/* Colaboradores de execução */}
          <div className="space-y-4">
            <div>
              <h3 className="text-lg font-semibold mb-1">Colaboradores de execução</h3>
              <p className="text-sm text-muted-foreground">
                Você pode visualizar e adicionar colaboradores
              </p>
            </div>

            {/* Campo de busca */}
            <div className="relative max-w-md">
              <div className="flex">
                <div className="flex items-center justify-center bg-primary text-white px-4 rounded-l-md">
                  <Search className="h-5 w-5" />
                </div>
                <Input 
                  placeholder="Busque o colaborador"
                  value={searchColaborador}
                  onChange={(e) => setSearchColaborador(e.target.value)}
                  className="rounded-l-none"
                />
              </div>
            </div>

            {/* Colaboradores selecionados */}
            {selectedColaboradores.length > 0 && (
              <div className="flex flex-wrap gap-2 p-4 bg-muted/50 rounded-lg">
                {selectedColaboradores.map((colaborador) => (
                  <Badge 
                    key={colaborador} 
                    variant="outline" 
                    className="px-3 py-1.5 text-sm bg-white border-blue-200"
                  >
                    {colaborador}
                    <button
                      onClick={() => removeColaborador(colaborador)}
                      className="ml-2 hover:text-destructive"
                    >
                      <X className="h-3 w-3" />
                    </button>
                  </Badge>
                ))}
              </div>
            )}
          </div>

          {/* Verbas */}
          <div className="space-y-4 pt-4 border-t">
            <div className="flex items-center justify-between">
              <div>
                <h3 className="text-lg font-semibold mb-1">Verbas</h3>
                <p className="text-sm text-muted-foreground">
                  Você pode criar as verbas e definir os parâmetros nesta seção.
                </p>
              </div>
              <div className="flex items-center gap-3">
                <Switch 
                  checked={ativarTodasVerbas}
                  onCheckedChange={setAtivarTodasVerbas}
                />
                <span className="text-sm font-medium">Ativar todos</span>
              </div>
            </div>

            {/* Verba Devolução */}
            <Collapsible open={openVerbas.devolucao} onOpenChange={() => toggleVerba('devolucao')}>
              <Card className="border-2">
                <CardContent className="pt-6">
                  <CollapsibleTrigger className="w-full" asChild>
                    <Button
                      variant="ghost"
                      className="w-full flex items-center justify-between p-0 h-auto hover:bg-transparent"
                    >
                      <div className="flex items-center gap-4 flex-1">
                        <div className="flex items-center gap-3">
                          <Switch defaultChecked />
                          <span className="text-sm font-medium">Ativa</span>
                        </div>
                        
                        <div className="flex-1 text-left">
                          <div className="font-semibold text-lg">Devolução</div>
                          <div className="text-sm text-muted-foreground">Débito</div>
                        </div>
                      </div>
                      
                      <div className="flex items-center gap-6">
                        <div>
                          <div className="text-sm text-muted-foreground mb-1">Valor limite por despesa</div>
                          <div className="text-lg font-semibold">500,00</div>
                        </div>
                        
                        <div className="flex items-center gap-3">
                          <Switch defaultChecked />
                          <span className="text-sm font-medium">Custo Cliente</span>
                        </div>
                        
                        {openVerbas.devolucao ? (
                          <ChevronUp className="h-5 w-5" />
                        ) : (
                          <ChevronDown className="h-5 w-5" />
                        )}
                      </div>
                    </Button>
                  </CollapsibleTrigger>
                  
                  <CollapsibleContent className="mt-4 pt-4 border-t">
                    <div className="text-sm text-muted-foreground">
                      Detalhes adicionais da verba podem ser exibidos aqui
                    </div>
                  </CollapsibleContent>
                </CardContent>
              </Card>
            </Collapsible>

            {/* Verba Adiantamento */}
            <Collapsible open={openVerbas.adiantamento} onOpenChange={() => toggleVerba('adiantamento')}>
              <Card className="border-2">
                <CardContent className="pt-6">
                  <CollapsibleTrigger className="w-full" asChild>
                    <Button
                      variant="ghost"
                      className="w-full flex items-center justify-between p-0 h-auto hover:bg-transparent"
                    >
                      <div className="flex items-center gap-4 flex-1">
                        <div className="flex items-center gap-3">
                          <Switch defaultChecked />
                          <span className="text-sm font-medium">Ativa</span>
                        </div>
                        
                        <div className="flex-1 text-left">
                          <div className="font-semibold text-lg">Adiantamento</div>
                          <div className="text-sm text-muted-foreground">Crédito</div>
                        </div>
                      </div>
                      
                      <div className="flex items-center gap-6">
                        <div>
                          <div className="text-sm text-muted-foreground mb-1">Valor limite por despesa</div>
                          <div className="text-lg font-semibold">1.000,00</div>
                        </div>
                        
                        <div className="flex items-center gap-3">
                          <Switch defaultChecked />
                          <span className="text-sm font-medium">Custo Cliente</span>
                        </div>
                        
                        {openVerbas.adiantamento ? (
                          <ChevronUp className="h-5 w-5" />
                        ) : (
                          <ChevronDown className="h-5 w-5" />
                        )}
                      </div>
                    </Button>
                  </CollapsibleTrigger>
                  
                  <CollapsibleContent className="mt-4 pt-4 border-t">
                    <div className="text-sm text-muted-foreground">
                      Detalhes adicionais da verba podem ser exibidos aqui
                    </div>
                  </CollapsibleContent>
                </CardContent>
              </Card>
            </Collapsible>
          </div>
        </CardContent>
      </Card>
    </div>
  );
};
